Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Web
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports Tao.FreeGlut
Imports Tao.FreeGlut.Glut
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Tao.DevIl
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports Ionic.Zip
Imports Ionic.Zlib

Module modMenu
    Public button_list(0) As menuitem
    Public M_SELECT_COLOR As Integer = 255
    Public buttonState As Integer = 0
    Public m_mouse As vec2
    Public Menu_Subs As New m_subs_
    '--------------------------------------------------------------
    Public Sub create_GL_menu()
        Dim b1 As New menuitem
        b1.function_ = "open_file"
        b1.texture_name = "open.png"
        b1.add_button()
        '---
        Dim b2 As New menuitem
        b2.function_ = "close"
        b2.texture_name = "close.png"
        b2.add_button()
        '---
        Dim b3 As New menuitem
        b3.function_ = "list"
        b3.texture_name = "list.png"
        b3.add_button()
        '---
        Dim b4 As New menuitem
        b4.function_ = "export"
        b4.texture_name = "export.png"
        b4.add_button()

        frmMain.pb1.BringToFront()
    End Sub
    Public Class m_subs_
        Public Sub open_file()
            M_SELECT_COLOR = 255
            'Stop
        End Sub
        Public Sub close()
            M_SELECT_COLOR = 255

        End Sub
        Public Sub list()
            M_SELECT_COLOR = 255

        End Sub
        Public Sub export()
            M_SELECT_COLOR = 255

        End Sub
    End Class

    Public Function check_menu_select()
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Gl.glDisable(Gl.GL_LIGHTING)
        Dim bw = button_list(0).total_width
        Dim ww = frmMain.pb1.Width
        Dim space As Single = 0
        Dim red, green, blue As Byte
        Dim pos As vec2
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        pos.x = (ww / 2) - (((space * button_list.Length - 3) + bw) / 2)
        pos.y = -5.0
        For i = 0 To button_list.Length - 2
            red = (i + 1) * 10
            button_list(i).color = red
            Gl.glColor3ub(red, green, blue)
            Dim next_pos = button_list(i).size.x
            Gl.glPushMatrix()
            Gl.glTranslatef(pos.x, pos.y, 0.0)
            Gl.glCallList(button_list(i).displayId)
            Gl.glPopMatrix()
            pos.x += (next_pos + space)

        Next
        Dim x, y As Single
        x = m_mouse.x
        y = m_mouse.y
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        M_SELECT_COLOR = 0
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        red = pixel(0)
        If red > 0 Then
            M_SELECT_COLOR = red
            Return True
        Else
            M_SELECT_COLOR = 0
        End If
        Return False
    End Function

    Public Sub draw_menu()
        Dim bw = button_list(0).total_width
        Dim ww = frmMain.pb1.Width
        Dim space As Single = 0
        Dim pos As vec2
        pos.x = (ww / 2) - (((space * button_list.Length - 3) + bw) / 2)
        pos.y = -5.0
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        For i = 0 To button_list.Length - 2
            Gl.glColor3f(1.0, 1.0, 1.0)
            Dim next_pos = button_list(i).size.x
            Gl.glPushMatrix()
            Gl.glTranslatef(pos.x, pos.y, 0.0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, button_list(i).textureId)
            If M_SELECT_COLOR = button_list(i).color Then
                Gl.glColor3f(1.0, 0.0, 0.0)
            End If
            Gl.glCallList(button_list(i).displayId)
            Gl.glPopMatrix()
            pos.x += (next_pos + space)

        Next
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_BLEND)
    End Sub

    Public Class menuitem
        Public location As vec2
        Public color As UInt32
        Public size As vec2
        Public displayId As UInt32
        Public textureId As UInt32
        Public texture_name As String
        Public function_ As String



        Public Sub makebutton()
            Me.displayId = Gl.glGenLists(1)
            Gl.glNewList(Me.displayId, Gl.GL_COMPILE)
            Gl.glBegin(Gl.GL_QUADS)
            Gl.glTexCoord2f(0.0, 1.0)
            Gl.glVertex3f(location.x, location.y, 0.0)
            Gl.glTexCoord2f(0.0, 0.0)
            Gl.glVertex3f(location.x, location.y - size.y, 0.0)
            Gl.glTexCoord2f(1.0, 0.0)
            Gl.glVertex3f(location.x + size.x, location.y - size.y, 0.0)
            Gl.glTexCoord2f(1.0, 1.0)
            Gl.glVertex3f(location.x + size.x, location.y, 0.0)
            Gl.glEnd()
            Gl.glEndList()


        End Sub

        Public Sub add_button()
            Me.textureId = load_texture(Me.texture_name, Me.size)
            Me.makebutton()
            Dim l = button_list.Length
            button_list(l - 1) = New menuitem
            button_list(l - 1) = Me
            ReDim Preserve button_list(l)
        End Sub
        Public Function total_width() As Single
            Dim w As Single
            For i = 0 To button_list.Length - 2
                w += button_list(i).size.x
            Next
            Return w
        End Function
    End Class

    Public Function load_texture(tex As String, ByRef size As vec2) As Integer
        Dim path = Application.StartupPath + "\textures\" + tex
        Dim ID As Integer
        Dim texID = Ilu.iluGenImage()   ' Generation of one image name
        Il.ilBindImage(texID) ' Binding of image name 
        Dim success As Boolean = Il.ilLoad(Il.IL_PNG, path)
        If success Then
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)
            size.x = width
            size.y = height
            success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            Ilu.iluFlipImage()
            'Ilu.iluMirror()

            Gl.glGenTextures(1, ID)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, ID)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
            'Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            'Gl.glGenerateMipmapEXT(Gl.GL_TEXTURE_2D)
            Ilu.iluDeleteImage(texID)
        End If
        Return ID
    End Function
End Module
