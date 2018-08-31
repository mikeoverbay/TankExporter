Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Xml
'Imports wottools


Public Class Packed_Section
    Public Shared ReadOnly Packed_Header As Int32 = &H62A14E45
    Public Shared ReadOnly intToBase64 As Char() = New Char() {"A"c, "B"c, "C"c, "D"c, "E"c, "F"c, _
     "G"c, "H"c, "I"c, "J"c, "K"c, "L"c, _
     "M"c, "N"c, "O"c, "P"c, "Q"c, "R"c, _
     "S"c, "T"c, "U"c, "V"c, "W"c, "X"c, _
     "Y"c, "Z"c, "a"c, "b"c, "c"c, "d"c, _
     "e"c, "f"c, "g"c, "h"c, "i"c, "j"c, _
     "k"c, "l"c, "m"c, "n"c, "o"c, "p"c, _
     "q"c, "r"c, "s"c, "t"c, "u"c, "v"c, _
     "w"c, "x"c, "y"c, "z"c, "0"c, "1"c, _
     "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, _
     "8"c, "9"c, "+"c, "/"c}
    Public Const MAX_LENGTH As Integer = 256

    Public Class DataDescriptor
        Public ReadOnly address As Integer
        Public ReadOnly [end] As Integer
        Public ReadOnly type As Integer

        Public Sub New(ByVal [end] As Integer, ByVal type As Integer, ByVal address As Integer)
            Me.[end] = [end]
            Me.type = type
            Me.address = address
        End Sub

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder("[")
            sb.Append("0x")
            sb.Append(Convert.ToString([end], 16))
            sb.Append(", ")
            sb.Append("0x")
            sb.Append(Convert.ToString(type, 16))
            sb.Append("]@0x")
            sb.Append(Convert.ToString(address, 16))
            Return sb.ToString()
        End Function
    End Class

    Public Class ElementDescriptor
        Public ReadOnly nameIndex As Integer
        Public ReadOnly dataDescriptor As DataDescriptor

        Public Sub New(ByVal nameIndex As Integer, ByVal dataDescriptor As DataDescriptor)
            Me.nameIndex = nameIndex
            Me.dataDescriptor = dataDescriptor
        End Sub

        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder("[")
            sb.Append("0x")
            sb.Append(Convert.ToString(nameIndex, 16))
            sb.Append(":")
            sb.Append(dataDescriptor)
            Return sb.ToString()
        End Function
    End Class

    Public Function readStringTillZero(ByVal reader As BinaryReader) As String
        Dim work As Char() = New Char(MAX_LENGTH - 1) {}

        Dim i As Integer = 0

        Dim c As Char = reader.ReadChar()
        While c <> Convert.ToChar(&H0)
            work(System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)) = c
            c = reader.ReadChar()
        End While
        Dim s As String = ""
        For k = 1 To i
            s = s + work(k)
        Next
        Return s

    End Function

    Public Function readDictionary(ByVal reader As BinaryReader) As List(Of String)
        Dim dictionary As New List(Of String)()
        Dim counter As Integer = 0
        Dim text As String = readStringTillZero(reader)

        While Not (text.Length = 0)
            dictionary.Add(text)
            text = readStringTillZero(reader)
            counter += 1
        End While
        Return dictionary
    End Function

    Public Function readLittleEndianShort(ByVal reader As BinaryReader) As Integer
        Dim LittleEndianShort As Integer = reader.ReadInt16()
        Return LittleEndianShort
    End Function

    Public Function readLittleEndianInt(ByVal reader As BinaryReader) As Integer
        Dim LittleEndianInt As Integer = reader.ReadInt32()
        Return LittleEndianInt
    End Function
    Public Function readLittleEndianlong(ByVal reader As BinaryReader) As Long
        Dim LittleEndianlong As Int64 = reader.ReadInt64()
        Return LittleEndianlong
    End Function

    Public Function readDataDescriptor(ByVal reader As BinaryReader) As DataDescriptor
        Dim selfEndAndType As Integer = readLittleEndianInt(reader)
        Return New DataDescriptor(selfEndAndType And &HFFFFFFF, selfEndAndType >> 28, CInt(reader.BaseStream.Position))
    End Function

    Public Function readElementDescriptors(ByVal reader As BinaryReader, ByVal number As Integer) As ElementDescriptor()
        Dim elements As ElementDescriptor() = New ElementDescriptor(number - 1) {}
        For i As Integer = 0 To number - 1
            Dim nameIndex As Integer = readLittleEndianShort(reader)
            Dim dataDescriptor As DataDescriptor = readDataDescriptor(reader)
            elements(i) = New ElementDescriptor(nameIndex, dataDescriptor)
        Next
        Return elements
    End Function

    Public Function readString(ByVal reader As BinaryReader, ByVal lengthInBytes As Integer) As String
        Dim rString As New String(reader.ReadChars(lengthInBytes), 0, lengthInBytes)

        Return rString
    End Function

    Public Function readNumber(ByVal reader As BinaryReader, ByVal lengthInBytes As Integer) As String
        Dim Number As String = ""
        Select Case lengthInBytes
            Case 1
                Number = Convert.ToString(reader.ReadSByte())
                Exit Select
            Case 2
                Number = Convert.ToString(readLittleEndianShort(reader))
                Exit Select
            Case 4
                Number = Convert.ToString(readLittleEndianInt(reader))
            Case 8
                Number = Convert.ToString(readLittleEndianlong(reader))
                Exit Select
            Case Else
                Number = "0"
                Exit Select
        End Select
        Return Number

    End Function

    Public Function readLittleEndianFloat(ByVal reader As BinaryReader) As Single
        Dim LittleEndianFloat As Single = reader.ReadSingle()
        Return LittleEndianFloat
    End Function

    Public Function readFloats(ByVal reader As BinaryReader, ByVal lengthInBytes As Integer) As String
        Dim n As Integer = lengthInBytes / 4

        Dim sb As New StringBuilder()
        For i As Integer = 0 To n - 1

            If i <> 0 Then
                sb.Append(" ")
            End If
            Dim rFloat As Single = readLittleEndianFloat(reader)
            sb.Append(rFloat.ToString("0.000000"))
        Next
        Return sb.ToString()
    End Function


    Public Function readBoolean(ByVal reader As BinaryReader, ByVal lengthInBytes As Integer) As Boolean
        Dim bool As Boolean = lengthInBytes = 1
        If bool Then
            If reader.ReadSByte() <> 1 Then
                Throw New System.ArgumentException("Boolean error")
            End If
        End If

        Return bool
    End Function

    Private Shared Function byteArrayToBase64(ByVal a As SByte()) As String
        Dim aLen As Integer = a.Length
        Dim numFullGroups As Integer = aLen / 3
        Dim numBytesInPartialGroup As Integer = aLen - 3 * numFullGroups
        Dim resultLen As Integer = 4 * ((aLen + 2) / 3)
        Dim result As New StringBuilder(resultLen)

        Dim inCursor As Integer = -1
        For i As Integer = 0 To numFullGroups - 1
            Dim byte0 As Integer = a(System.Math.Max(System.Threading.Interlocked.Increment(inCursor), inCursor - 1)) And &HFF
            inCursor = inCursor
            Dim byte1 As Integer = a(System.Math.Max(System.Threading.Interlocked.Increment(inCursor), inCursor - 1)) And &HFF
            inCursor = inCursor
            Dim byte2 As Integer = a(System.Math.Max(System.Threading.Interlocked.Increment(inCursor), inCursor - 2)) And &HFF
            result.Append(intToBase64(byte0 >> 2))
            result.Append(intToBase64((byte0 << 4) And &H3F Or (byte1 >> 4)))
            result.Append(intToBase64((byte1 << 2) And &H3F Or (byte2 >> 6)))
            result.Append(intToBase64(byte2 And &H3F))
        Next

        If numBytesInPartialGroup <> 0 Then
            Dim byte0 As Integer = a(System.Math.Max(System.Threading.Interlocked.Increment(inCursor), inCursor - 1)) And &HFF
            result.Append(intToBase64(byte0 >> 2))
            If numBytesInPartialGroup = 1 Then
                result.Append(intToBase64((byte0 << 4) And &H3F))
                result.Append("==")
            Else
                Dim byte1 As Integer = a(System.Math.Max(System.Threading.Interlocked.Increment(inCursor), inCursor - 1)) And &HFF
                result.Append(intToBase64((byte0 << 4) And &H3F Or (byte1 >> 4)))
                result.Append(intToBase64((byte1 << 2) And &H3F))
                result.Append("="c)
            End If
        End If

        Return result.ToString()
    End Function

    Public Function readBase64(ByVal reader As BinaryReader, ByVal lengthInBytes As Integer) As String
        Dim bytes As SByte() = New SByte(lengthInBytes - 1) {}
        For i As Integer = 0 To lengthInBytes - 1
            bytes(i) = reader.ReadSByte()
        Next
        Return byteArrayToBase64(bytes)
    End Function

    Public Function readAndToHex(ByVal reader As BinaryReader, ByVal lengthInBytes As Integer) As String
        Dim bytes As SByte() = New SByte(lengthInBytes - 1) {}
        For i As Integer = 0 To lengthInBytes - 1
            bytes(i) = reader.ReadSByte()
        Next
        Dim sb As New StringBuilder("[ ")
        For Each b As Byte In bytes
            sb.Append(Convert.ToString((b And &HFF), 16))
            sb.Append(" ")
        Next
        sb.Append("]L:")
        sb.Append(lengthInBytes)

        Return sb.ToString()
    End Function

    Public Function readData(ByVal reader As BinaryReader, ByVal dictionary As List(Of String), ByVal element As XmlNode, ByVal xDoc As XmlDocument, ByVal offset As Integer, ByVal dataDescriptor As DataDescriptor) As Integer
        Dim lengthInBytes As Integer = dataDescriptor.[end] - offset
        If dataDescriptor.type = &H0 Then
            ' Element                
            readElement(reader, element, xDoc, dictionary)
        ElseIf dataDescriptor.type = &H1 Then
            ' String

            element.InnerText = readString(reader, lengthInBytes)
        ElseIf dataDescriptor.type = &H2 Then
            ' Integer number
            element.InnerText = readNumber(reader, lengthInBytes)
        ElseIf dataDescriptor.type = &H3 Then
            ' Floats
            Dim str As String = readFloats(reader, lengthInBytes)

            Dim strData As String() = str.Split(" "c)
            If strData.Length = 12 Then
                Dim row0 As XmlNode = xDoc.CreateElement("row0")
                Dim row1 As XmlNode = xDoc.CreateElement("row1")
                Dim row2 As XmlNode = xDoc.CreateElement("row2")
                Dim row3 As XmlNode = xDoc.CreateElement("row3")
                row0.InnerText = strData(0) + " " + strData(1) + " " + strData(2)
                row1.InnerText = strData(3) + " " + strData(4) + " " + strData(5)
                row2.InnerText = strData(6) + " " + strData(7) + " " + strData(8)
                row3.InnerText = strData(9) + " " + strData(10) + " " + strData(11)
                element.AppendChild(row0)
                element.AppendChild(row1)
                element.AppendChild(row2)
                element.AppendChild(row3)
            Else
                element.InnerText = str
            End If
        ElseIf dataDescriptor.type = &H4 Then
            ' Boolean

            If readBoolean(reader, lengthInBytes) Then
                element.InnerText = "true"
            Else
                element.InnerText = "false"

            End If
        ElseIf dataDescriptor.type = &H5 Then
            ' Base64
            element.InnerText = readBase64(reader, lengthInBytes)
        Else
            Throw New System.ArgumentException("Unknown type of """ + element.Name + ": " + dataDescriptor.ToString() + " " + readAndToHex(reader, lengthInBytes))
        End If

        Return dataDescriptor.[end]
    End Function

    Public Sub readElement(ByVal reader As BinaryReader, ByVal element As XmlNode, ByVal xDoc As XmlDocument, ByVal dictionary As List(Of String))
        Dim childrenNmber As Integer = readLittleEndianShort(reader)
        Dim selfDataDescriptor As DataDescriptor = readDataDescriptor(reader)
        Dim children As ElementDescriptor() = readElementDescriptors(reader, childrenNmber)

        Dim offset As Integer = readData(reader, dictionary, element, xDoc, 0, selfDataDescriptor)

        For Each elementDescriptor As ElementDescriptor In children
            Dim child As XmlNode = xDoc.CreateElement(dictionary(elementDescriptor.nameIndex))
            offset = readData(reader, dictionary, child, xDoc, offset, elementDescriptor.dataDescriptor)
            element.AppendChild(child)
        Next

    End Sub
End Class


'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik, @toddanglin
'Facebook: facebook.com/telerik
'=======================================================

