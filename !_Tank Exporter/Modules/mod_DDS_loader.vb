Imports System.Runtime.InteropServices
Imports System.IO
Imports Tao.OpenGl ' Import Tao OpenGL namespace

Module mod_DDS_loader


    ' DDS Pixel Format structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure DDSPixelFormat
        Public dwSize As UInteger
        Public dwFlags As UInteger
        Public dwFourCC As UInteger
        Public dwRGBBitCount As UInteger
        Public dwRBitMask As UInteger
        Public dwGBitMask As UInteger
        Public dwBBitMask As UInteger
        Public dwABitMask As UInteger
    End Structure

    ' DDS File Header structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure DDSHeader
        Public dwSize As UInteger
        Public dwFlags As UInteger
        Public dwHeight As UInteger
        Public dwWidth As UInteger
        Public dwPitchOrLinearSize As UInteger
        Public dwDepth As UInteger
        Public dwMipMapCount As UInteger
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=11)>
        Public dwReserved1 As UInteger()
        Public ddspf As DDSPixelFormat
        Public dwCaps As UInteger
        Public dwCaps2 As UInteger
        Public dwCaps3 As UInteger
        Public dwCaps4 As UInteger
        Public dwReserved2 As UInteger
    End Structure

    ' Constants for DDS
    Public Class DDSConstants
        Public Const DDS_MAGIC As Integer = &H20534444 ' "DDS " in ASCII
        Public Const DXT5_FOURCC As UInteger = &H35545844 ' "DXT5" in ASCII
    End Class


    Public Function LoadDDSHeader(ByVal filePath As String) As DDSHeader?
        Using reader As New BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read))
            ' Read and verify the magic number
            Dim magicNumber As Integer = reader.ReadInt32()
            If magicNumber <> DDSConstants.DDS_MAGIC Then
                Console.WriteLine("Not a valid DDS file.")
                Return Nothing
            End If

            ' Read the DDS header
            Dim headerBytes As Byte() = reader.ReadBytes(Marshal.SizeOf(GetType(DDSHeader)))
            Dim handle As GCHandle = GCHandle.Alloc(headerBytes, GCHandleType.Pinned)
            Dim header As DDSHeader = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), GetType(DDSHeader))
            handle.Free()

            ' Validate the header
            If header.ddspf.dwFourCC <> DDSConstants.DXT5_FOURCC Then
                Console.WriteLine("Not a DXT5 compressed DDS file.")
                Return Nothing
            End If

            Return header
        End Using
    End Function

    Public Function LoadCompressedTextureToVRAM(ByVal filePath As String) As Integer
        ' Load the DDS header
        If Not File.Exists(filePath) Then
            Return white_id
        End If
        Dim header As DDSHeader? = LoadDDSHeader(filePath)
        If header Is Nothing Then
            Return 0
        End If
        Gl.glGetError()
        ' Extract header information
        Dim width As Integer = CInt(header.Value.dwWidth)
        Dim height As Integer = CInt(header.Value.dwHeight)
        Dim mipMapCount As Integer = CInt(header.Value.dwMipMapCount)

        ' Calculate the size of the compressed texture data
        Dim blockSize As Integer = 16 ' DXT5 has 16 bytes per 4x4 block
        Dim dataSize As Integer = 0
        Dim widthMip As Integer = width
        Dim heightMip As Integer = height

        For i As Integer = 0 To mipMapCount - 1
            Dim size As Integer = ((widthMip + 3) / 4) * ((heightMip + 3) / 4) * blockSize
            dataSize += size
            widthMip = Math.Max(1, widthMip / 2)
            heightMip = Math.Max(1, heightMip / 2)
        Next
        Dim errorCode As Integer = Gl.glGetError()
        If errorCode <> Gl.GL_NO_ERROR Then
            Console.WriteLine("OpenGL Error: " & errorCode)
        End If
        ' Read the compressed texture data
        Dim textureData(dataSize - 1) As Byte
        Using reader As New BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read))
            reader.BaseStream.Seek(4 + Marshal.SizeOf(GetType(DDSHeader)), SeekOrigin.Begin)
            reader.Read(textureData, 0, dataSize)
        End Using

        errorCode = Gl.glGetError()
        If errorCode <> Gl.GL_NO_ERROR Then
            Console.WriteLine("OpenGL Error: " & errorCode)
        End If


        ' Generate a new texture ID
        Dim textureID As Integer
        Gl.glGenTextures(1, textureID)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureID)

        ' Reset width and height for mipmap level data
        widthMip = width
        heightMip = height
        Dim offset As Integer = 0

        errorCode = Gl.glGetError()
        If errorCode <> Gl.GL_NO_ERROR Then
            Console.WriteLine("OpenGL Error: " & errorCode)
        End If

        ' Upload the compressed texture data for each mipmap level
        For i As Integer = 0 To mipMapCount - 1
            Dim size As Integer = ((widthMip + 3) / 4) * ((heightMip + 3) / 4) * blockSize
            Gl.glCompressedTexImage2D(Gl.GL_TEXTURE_2D, i, Gl.GL_COMPRESSED_RGBA_S3TC_DXT5_EXT, widthMip, heightMip, 0, size, textureData(offset))
            offset += size
            widthMip = Math.Max(1, widthMip / 2)
            heightMip = Math.Max(1, heightMip / 2)
        Next
        errorCode = Gl.glGetError()
        If errorCode <> Gl.GL_NO_ERROR Then
            Console.WriteLine("OpenGL Error: " & errorCode)
        End If

        ' Set texture parameters
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
        errorCode = Gl.glGetError()
        If errorCode <> Gl.GL_NO_ERROR Then
            Console.WriteLine("OpenGL Error: " & errorCode)
        End If

        Console.WriteLine("Loaded DDS texture to VRAM. Texture ID: " & textureID)
        Return textureID ' Return the texture ID
    End Function

End Module
