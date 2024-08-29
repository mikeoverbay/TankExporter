Module blurFBO
    Public BlurShadowFBO As New BlurFbo_

    Public Class BlurFbo_
        Public blurfbo_texture As Integer
        Private attachments() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
        Public fbo As Integer
        Public width, height As Integer

        Public Sub blur_depth_texture()
            'make this as contained as possible
            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, Me.fbo)

            Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glOrtho(0, Me.width, -Me.height, 0, -0.1, 1.0) 'Select Ortho Mode

            Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
            Gl.glDisable(Gl.GL_CULL_FACE)

            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
            draw_b_rect()

            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, shadowFramebuffer)
            Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
            Gl.glLoadIdentity() 'Reset The Matrix
            Gl.glOrtho(0, shadowMapSize, -shadowMapSize, 0, -0.1, 1.0) 'Select Ortho Mode
            shadow_fbo.attach_depth()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Me.blurfbo_texture)
            draw_depth_rect()
        End Sub
        Private Sub draw_b_rect()
            Dim p = New Point(0, 0)
            Dim w = Me.width
            Dim h = Me.height
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
        Private Sub draw_depth_rect()
            Dim p = New Point(0, 0)
            Dim w = Me.width * 2
            Dim h = Me.height * 2
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
        Public Sub shut_down()
            If blurfbo_texture > 0 Then
                Gl.glDeleteTextures(1, blurfbo_texture)
            End If
            If fbo > 0 Then
                Gl.glDeleteFramebuffersEXT(1, fbo)
            End If

        End Sub

        Public Sub create_textures()
            ' - rgba color
            Gl.glGenTextures(1, blurfbo_texture)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, blurfbo_texture)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB16F_ARB, CInt(shadowMapSize), CInt(shadowMapSize), 0, Gl.GL_RGB, Gl.GL_FLOAT, Nothing)


        End Sub
        Public Sub attach_texture(ByVal id As Integer)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)

        End Sub
        Public Sub init()
            Me.width = shadowMapSize / 2
            Me.height = Me.width

            shut_down()
            create_textures()

            Gl.glGenFramebuffersEXT(1, Me.fbo)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, Me.fbo)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, Me.blurfbo_texture, 0) '16f color
            Gl.glDrawBuffers(1, attachments)

            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                'MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
                Return
            End If

        End Sub
    End Class
End Module
