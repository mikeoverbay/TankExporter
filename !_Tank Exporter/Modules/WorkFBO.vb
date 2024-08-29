

Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl
Imports Tao.FreeGlut

Module WorkFBO


    Public worker_fbo As New worker_fbo_

    Public Class worker_fbo_
        Public worker_fbo As Integer
        Public mWIDTH, mHEIGTH As Integer
        Public out_tex As Integer
        Private attachments() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
        Public Sub shutdown_worker_fbo()
            If out_tex > 0 Then
                Gl.glDeleteTextures(1, out_tex)
            End If
            If worker_fbo > 0 Then
                Gl.glDeleteFramebuffersEXT(1, worker_fbo)
                Dim e = Gl.glGetError
            End If

        End Sub
        Public Sub reset_worker_fbo(ByVal width As Integer, ByVal heigth As Integer)
            mHEIGTH = heigth
            mWIDTH = width
            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)

            If out_tex > 0 Then
                Gl.glDeleteTextures(1, out_tex)
            End If


            If worker_fbo > 0 Then
                Gl.glDeleteFramebuffersEXT(1, worker_fbo)
                Dim e = Gl.glGetError
            End If

            ' - rgba color
            Gl.glGenTextures(1, out_tex)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, out_tex)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, mWIDTH, mHEIGTH, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e2 = Gl.glGetError

            If e2 < 0 Then
                MsgBox("A error was thrown by Opengl creating Worker fbo textures! Error = " + e2.ToString, MsgBoxStyle.Exclamation, "Gl Error")
                Return
            End If
            'Create the FBO
            Gl.glGenFramebuffersEXT(1, worker_fbo)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, worker_fbo)
            Dim e3 = Gl.glGetError
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, out_tex, 0)
            Dim e5 = Gl.glGetError

            'attach draw buffers
            Gl.glDrawBuffers(1, attachments)

            'attach draw buffers
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                'MsgBox("Failed to create Worker_fbo", MsgBoxStyle.Critical, "Not good!")

            End If

        End Sub

        Public Sub detach_textures()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub
        Private Sub attach_texture(ByVal img As Integer)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, img, 0)

        End Sub
        Public Sub blur(ByVal img As Integer)
            Gl.glUseProgram(shader_list.blurBchannel_shader)
            Gl.glUniform1i(blurB_image, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Dim w, h As Integer

            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Dim miplevel = 0
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, img)
            Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, miplevel, Gl.GL_TEXTURE_WIDTH, w)
            Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, miplevel, Gl.GL_TEXTURE_HEIGHT, h)
            reset_worker_fbo(w, h)
            ResizeGL(Me.mWIDTH, Me.mHEIGTH)

            Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glOrtho(0, Me.mWIDTH, -Me.mHEIGTH, 0, -200.0, 100.0) 'Select Ortho Mode
            Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
            Gl.glLoadIdentity() 'Reset The Matrix


            For i = 1 To 1

                attach_texture(Me.out_tex)
                Gl.glUniform1i(blurB_switch, 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, img)
                draw_rect()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)


                Gl.glUniform1i(blurB_switch, 0)
                attach_texture(img)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, Me.out_tex)
                draw_rect()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Next
            attach_texture(Me.out_tex)
            Gl.glUseProgram(0)
            'below is debug shit.

            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0) 'rebind default FBO

            w = frmMain.pb1.Width
            h = frmMain.pb1.Height
            ResizeGL(w, h)

            Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glOrtho(0, w, -h, 0, -200.0, 100.0) 'Select Ortho Mode
            Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glDisable(Gl.GL_DEPTH_TEST)

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, img)

            frmMain.draw_main_rec(New Point, w, h)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

            Gl.glDisable(Gl.GL_TEXTURE_2D)



        End Sub

        Private Sub set_worker_view()
            ResizeGL(Me.mWIDTH, Me.mHEIGTH)
            Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glOrtho(0, Me.mWIDTH, -Me.mHEIGTH, 0, -200.0, 100.0) 'Select Ortho Mode
            Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glDisable(Gl.GL_CULL_FACE)
        End Sub

        Public Sub draw_to_fbo_no_clip(ByVal image As Integer)
            'draws to the buffer with no clipping.
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            set_worker_view()
            ResizeGL(Me.mWIDTH, Me.mHEIGTH)

            Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glOrtho(0, Me.mWIDTH, -Me.mHEIGTH, 0, -200.0, 100.0) 'Select Ortho Mode
            Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glDisable(Gl.GL_CULL_FACE)

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)

            draw_rect(0.0!, 0.0!)

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        End Sub

        Public Sub draw_to_fbo(ByVal image As Integer)
            'draws to the buffer and clips .0625 percent off around the image.
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            set_worker_view()

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image)

            Dim border_w = Me.mWIDTH * 0.0625!
            Dim border_h = Me.mHEIGTH * 0.0625!
            Dim bw = (1.0! / Me.mWIDTH) * border_w
            Dim bh = (1.0! / Me.mHEIGTH) * border_w

            draw_rect(bw, bh)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        End Sub
        Private Sub draw_rect(ByVal bw As Single, ByVal bh As Single)
            Dim p = New Point(0, 0)
            Dim w = Me.mWIDTH
            Dim h = Me.mHEIGTH

            Gl.glBegin(Gl.GL_QUADS)
            '  CW...
            '  1 ------ 2
            '  |        |
            '  |        |
            '  4 ------ 3
            '
            Gl.glTexCoord2f(0.0! + bw, 1.0! - bh)
            Gl.glVertex2f(p.X, p.Y)

            Gl.glTexCoord2f(1.0! - bw, 1.0! - bh)
            Gl.glVertex2f(p.X + w, p.Y)

            Gl.glTexCoord2f(1.0! - bw, 0.0! + bh)
            Gl.glVertex2f(p.X + w, p.Y - h)

            Gl.glTexCoord2f(0.0! + bw, 0.0! + bh)
            Gl.glVertex2f(p.X, p.Y - h)
            Gl.glEnd()

        End Sub
        Private Sub draw_rect()
            Dim p = New Point(0, 0)
            Dim w = Me.mWIDTH
            Dim h = Me.mHEIGTH
            Gl.glBegin(Gl.GL_QUADS)
            '  CW...
            '  1 ------ 2
            '  |        |
            '  |        |
            '  4 ------ 3
            '
            Gl.glTexCoord2f(0.0!, 1.0!)
            Gl.glVertex2f(p.X, p.Y)

            Gl.glTexCoord2f(1.0!, 1.0!)
            Gl.glVertex2f(p.X + w, p.Y)

            Gl.glTexCoord2f(1.0!, 0.0!)
            Gl.glVertex2f(p.X + w, p.Y - h)

            Gl.glTexCoord2f(0.0!, 0.0!)
            Gl.glVertex2f(p.X, p.Y - h)
            Gl.glEnd()

        End Sub

    End Class
End Module
