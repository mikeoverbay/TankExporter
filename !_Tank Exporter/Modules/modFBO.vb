Imports System.IO
Imports System.Math
Imports System
Imports Tao.OpenGl
Imports Tao.FreeGlut

Module modFBO
    Public G_Buffer As New GBuffer_
    Public gBufferFBO As Integer
    Public gColor, gDepth, gPosition, gNormal, gNormal2, gFXAA As Integer
    Public grDepth As Integer
    Public rendered_shadow_texture As Integer
    Public rendered_shadow_temp As Integer
    Public Class GBuffer_
        Private attachments() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
        Private attachments_cn() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT}
        Private attachments_cnfn() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_COLOR_ATTACHMENT3_EXT}

        Public Sub shut_down()
            delete_textures_and_fob_objects()
            blm_fbo.shutdown_blm_fbo()
        End Sub
        Private Sub delete_textures_and_fob_objects()
            Dim e As Integer
            If rendered_shadow_texture > 0 Then
                Gl.glDeleteTextures(1, rendered_shadow_texture)
                e = Gl.glGetError
            End If
            If rendered_shadow_temp > 0 Then
                Gl.glDeleteTextures(1, rendered_shadow_temp)
                e = Gl.glGetError
            End If
            If gColor > 0 Then
                Gl.glDeleteTextures(1, gColor)
                e = Gl.glGetError
            End If
            If gDepth > 0 Then
                Gl.glDeleteTextures(1, gDepth)
                e = Gl.glGetError
            End If
            'If gPosition > 0 Then
            '    Gl.glDeleteTextures(1, gPosition)
            '    e = Gl.glGetError
            'End If
            If gNormal > 0 Then
                Gl.glDeleteTextures(1, gNormal)
                e = Gl.glGetError
            End If
            If gNormal2 > 0 Then
                Gl.glDeleteTextures(1, gNormal2)
                e = Gl.glGetError
            End If
            If grDepth > 0 Then
                Gl.glDeleteRenderbuffersEXT(1, grDepth)
                e = Gl.glGetError
            End If
            If gBufferFBO > 0 Then
                Gl.glDeleteFramebuffersEXT(1, gBufferFBO)
                e = Gl.glGetError
            End If
        End Sub
        Public Sub getsize(ByRef w As Integer, ByRef h As Integer)
            Dim w1, h1 As Integer
            w1 = frmMain.pb1.Width
            h1 = frmMain.pb1.Height
            w = w1 + (w1 Mod 2)
            h = h1 + (h1 Mod 2)
        End Sub
        Private Sub create_textures()
            Dim SCR_WIDTH, SCR_HEIGHT As Integer
            getsize(SCR_WIDTH, SCR_HEIGHT)
            'depth buffer
            Dim e1 = Gl.glGetError

            ' - Normal buffer
            Gl.glGenTextures(1, gNormal)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB16F_ARB, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGB, Gl.GL_FLOAT, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            ' - Normal2 buffer
            Gl.glGenTextures(1, gNormal2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal2)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB16F_ARB, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGB, Gl.GL_FLOAT, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            ' - depth buffer
            Gl.glGenTextures(1, gDepth)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepth)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA16F_ARB, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_FLOAT, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            ' - rendered shadow texture
            Gl.glGenTextures(1, rendered_shadow_texture)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_texture)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            ' - rendered shadow texture
            Gl.glGenTextures(1, rendered_shadow_temp)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_temp)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            ' - Color color buffer
            Gl.glGenTextures(1, gColor)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e2 = Gl.glGetError
            ' - Color color buffer
            Gl.glGenTextures(1, gFXAA)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, SCR_WIDTH, SCR_HEIGHT, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
            Dim e3 = Gl.glGetError

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        End Sub
        Public Sub attach_CNFN()
            ' detachFBOtextures()
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, gNormal, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_TEXTURE_2D, gFXAA, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT3_EXT, Gl.GL_TEXTURE_2D, gNormal2, 0)
            Gl.glDrawBuffers(4, attachments_cnfn)
            'Dim er = Gl.glGetError
        End Sub
        Public Sub attachColor_And_NormalTexture()
            ' detachFBOtextures()
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, gNormal, 0)
            Gl.glDrawBuffers(2, attachments_cn)
            'Dim er = Gl.glGetError
        End Sub
        Public Sub attachColor_And_blm_tex1()
            ' detachFBOtextures()
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, blm_fbo.blm_tex1, 0)
            Gl.glDrawBuffers(2, attachments_cn)
            Dim er = Gl.glGetError
        End Sub
        Public Sub attachColor_And_Normal_FOG_Texture()
            Dim attachers3() = {Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT}
            detachFBOtextures()
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, gNormal, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_TEXTURE_2D, gFXAA, 0)
            Gl.glDrawBuffers(3, attachers3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            'Dim er = Gl.glGetError
        End Sub

        Public Sub attach_Shadow_render_texture()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, rendered_shadow_texture, 0)
            Gl.glDrawBuffers(1, attachments)
        End Sub
        Public Sub attach_Shadow_render_temp()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, rendered_shadow_temp, 0)
            Gl.glDrawBuffers(1, attachments)
        End Sub
        Public Sub attachColorTexture()
            'detachFBOtextures()
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Gl.glDrawBuffers(1, attachments)
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            'Dim er = Gl.glGetError
        End Sub
        Public Sub attachFXAAtexture()
            'detachFBOtextures()
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gFXAA, 0)
            Gl.glDrawBuffers(1, attachments)
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            'Dim er = Gl.glGetError
        End Sub
        Public Sub detachFBOtextures()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, 0, 0)
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT2_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub
        Public Function init() As Boolean
            If Not _Started Then Return False
            pan_location = 0
            If frmMain.update_thread.IsAlive Then
                frmMain.update_thread.Suspend()
            End If
            Threading.Thread.Sleep(50)
            Dim SCR_WIDTH, SCR_HEIGHT As Integer
            getsize(SCR_WIDTH, SCR_HEIGHT)

            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
            Dim e1 = Gl.glGetError

            blm_fbo.reset_blm_fbo() ' reset blm_fbo as its size must match

            delete_textures_and_fob_objects()
            'Create the gBuffer textures
            create_textures()
            Dim e2 = Gl.glGetError

            'Create the FBO
            Gl.glGenFramebuffersEXT(1, gBufferFBO)
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)
            Dim e3 = Gl.glGetError

            Gl.glGenRenderbuffersEXT(1, grDepth)
            Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, grDepth)
            Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT24, SCR_WIDTH, SCR_HEIGHT)
            Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, grDepth)
            Dim e4 = Gl.glGetError


            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, gColor, 0)
            Dim e5 = Gl.glGetError

            'attach draw buffers
            Gl.glDrawBuffers(1, attachments)

            'attach draw buffers
            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
                Return False
            End If


            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
            If frmMain.update_thread.IsAlive Then
                frmMain.update_thread.Resume()
            End If

            Return True
        End Function

        Public Sub get_depth_buffer(ByVal w As Integer, ByVal h As Integer)
            Dim e1 = Gl.glGetError
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepth)
            Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_DEPTH_COMPONENT24, 0, 0, w, h, 0)
            Dim e2 = Gl.glGetError
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        End Sub


    End Class


End Module
