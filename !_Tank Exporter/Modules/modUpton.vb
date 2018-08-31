#Region "imports"
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
Imports System.Drawing.Imaging
Imports System.Globalization
#End Region


Module modUpton

    Public upton As New upton_

    Public Class upton_
        Public state As Integer
        Public position As New Point(0, 0)
        Private w As Integer = 140
        Private h As Integer = 200

        Public upton_img_index(10) As Integer ' stores our 10 button down images
        Public upton_user_view As Integer
        Public upton_pick_image As Integer

        Public Sub pick_upton()
            Dim er = Gl.glGetError

            If M_DOWN Then
                Return
            End If
            Gl.glReadBuffer(Gl.GL_BACK)
            Dim x = m_mouse.x
            Dim y = m_mouse.y
            Dim viewport(4) As Integer
            Dim pixel() As Byte = {0, 0, 0, 0}
            'Dim type = pixel(3)

            Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)

            Gl.glClearColor(0.0, 0.0, 0.0, 1.0)
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT)

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, upton_pick_image)
            draw_rect_preview(position, w, h)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)


            'pick function
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
            Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
            er = Gl.glGetError
            If pixel(0) = 102 Then 'mouse is on upton window
                state = pixel(0)
                Return
            End If
            If pixel(0) > 0 Then 'red holds state index
                state = pixel(0)
                Return
            End If
            state = 0 ' defualt.. mouse isn't in upton


        End Sub

        Public Sub draw_upton()
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
            If state = 255 Then state = 0
            If state = 0 Or state = 102 Then
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, upton_user_view)
                'Gl.glBindTexture(Gl.GL_TEXTURE_2D, upton_pick_image)
            Else
                If Not state = 102 Then
                    Try
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, upton_img_index(state - 1))

                    Catch ex As Exception

                    End Try
                End If
            End If
            draw_rect_preview(position, w, h)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glDisable(Gl.GL_TEXTURE_2D)

        End Sub

        Private Sub draw_rect_preview(ByVal p As Point, ByVal w As Integer, ByVal h As Integer)
            Gl.glBegin(Gl.GL_QUADS)
            '  CW...
            '  1 ------ 4
            '  |        |
            '  |        |
            '  2 ------ 3
            '
            Gl.glTexCoord2f(1.0!, 0.0!)
            Gl.glVertex2f(p.X, p.Y)

            Gl.glTexCoord2f(1.0!, 1.0!)
            Gl.glVertex2f(p.X, p.Y - h)

            Gl.glTexCoord2f(0.0!, 1.0!)
            Gl.glVertex2f(p.X + w, p.Y - h)

            Gl.glTexCoord2f(0.0!, 0.0!)
            Gl.glVertex2f(p.X + w, p.Y)

            Gl.glEnd()

        End Sub

        Public Sub load_upton()
            Dim iPath = Application.StartupPath + "\resources\upton\"
            upton_user_view = load_png_file(iPath + "user_view.png")
            upton_pick_image = load_png_file(iPath + "pick_image.png")
            For i = 0 To 9
                upton_img_index(i) = load_png_no_mips(iPath + "upton_" + i.ToString + ".png")
            Next
        End Sub

    End Class
    Public Function load_png_no_mips(fs As String)
        Dim image_id As Integer = -1

        Dim texID As UInt32
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoad(Il.IL_PNG, fs)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)


            Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE)

            success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            Gl.glGenTextures(1, image_id)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            Return image_id
        Else
            log_text.AppendLine("Png did not load:" + fs)
        End If
        Return Nothing

    End Function

End Module
