
Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl
Imports Tao.FreeGlut



Module modBloom
    Public blm_fbo As New blm_fbo_

    Public Class blm_fbo_
        Public blm_fbo As Integer
        Private SCR_WIDTH, SCR_HEIGHT As Integer
        Public blm_tex1, blm_tex2, blm_tex3 As Integer
        Private attachments() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
        Public Sub shutdown_blm_fbo()
            If blm_tex1 > 0 Then
                Gl.glDeleteTextures(1, blm_tex1)
            End If
            If blm_tex2 > 0 Then
                Gl.glDeleteTextures(1, blm_tex2)
            End If

            If blm_fbo > 0 Then
                Gl.glDeleteFramebuffersEXT(1, blm_fbo)
                Dim e = Gl.glGetError
            End If

        End Sub
        Public Sub reset_blm_fbo()
            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)

            G_Buffer.getsize(SCR_WIDTH, SCR_HEIGHT) ' get size from main render fbo
            SCR_WIDTH *= 0.5
            SCR_HEIGHT *= 0.5
            If blm_tex1 > 0 Then
                Gl.glDeleteTextures(1, blm_tex1)
            End If
            If blm_tex2 > 0 Then
                Gl.glDeleteTextures(1, blm_tex2)
            End If
            If blm_tex3 > 0 Then
                Gl.glDeleteTextures(1, blm_tex3)
            End If

            If blm_fbo > 0 Then
                Gl.glDeleteFramebuffersEXT(1, blm_fbo)
                Dim e = Gl.glGetError
            End If

            ' - rgba color
            Gl.glGenTextures(1, blm_tex1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, blm_tex1)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH * 2, SCR_HEIGHT * 2, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e2 = Gl.glGetError

            ' - rgba color
            Gl.glGenTextures(1, blm_tex2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, blm_tex2)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            e2 = Gl.glGetError
            ' - rgba color
            Gl.glGenTextures(1, blm_tex3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, blm_tex3)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            e2 = Gl.glGetError
            If e2 < 0 Then
                MsgBox("A error was thrown by Opengl creating blm_fbo textures! Error = " + e2.ToString, MsgBoxStyle.Exclamation, "Gl Error")
                Return
            End If
            'Create the FBO
            Gl.glGenFramebuffersEXT(1, blm_fbo)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, blm_fbo)
            Dim e3 = Gl.glGetError
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, blm_tex2, 0)
            Dim e5 = Gl.glGetError

            'attach draw buffers
            Gl.glDrawBuffers(1, attachments)

            'attach draw buffers
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create blm_fbo", MsgBoxStyle.Critical, "Not good!")

            End If



        End Sub

        Public Sub attact_blm_tex2()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, blm_tex2, 0)
            Gl.glDrawBuffers(1, attachments)
        End Sub
        Public Sub attact_blm_tex3()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, blm_tex3, 0)
            Gl.glDrawBuffers(1, attachments)
        End Sub
        Public Sub detach_textures()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub
        Public Sub blur()
            Gl.glUseProgram(shader_list.gaussian_shader)
            Gl.glUniform1i(gaus_image, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)

            ResizeGL(Me.SCR_WIDTH, Me.SCR_HEIGHT)

            Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glOrtho(0, Me.SCR_WIDTH, -Me.SCR_HEIGHT, 0, -200.0, 100.0) 'Select Ortho Mode
            Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
            Gl.glLoadIdentity() 'Reset The Matrix


            For i = 1 To 4

                attact_blm_tex3()
                Gl.glUniform1i(gaus_switch, 1)
                If i = 1 Then
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, blm_tex1)
                Else
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, blm_tex2)

                End If
                'Gl.glBindTexture(Gl.GL_TEXTURE_2D, blm_tex1)
                draw_rect()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                attact_blm_tex2()
                'Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
                Gl.glUniform1i(gaus_switch, 0)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, blm_tex3)
                'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 12)
                draw_rect()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Next
            Gl.glUseProgram(0)



        End Sub
        Private Sub draw_rect()
            Dim p = New Point(0, 0)
            Dim w = Me.SCR_WIDTH
            Dim h = Me.SCR_HEIGHT
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
