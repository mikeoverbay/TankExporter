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


Module modShadow
    Public shadowMapSize As Integer = 4096
    Public depthId As Integer
    Public depthBuffer, tempDepth As Integer
    Public shadowFramebuffer As Integer
    Public MV(15) As Single
    Public lightProjection(15) As Single
    Private attach_depthBuffer() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
    Private attach_tempDepth() As Integer = {Gl.GL_COLOR_ATTACHMENT1_EXT}
    Dim bias() As Single = {0.5, 0.0, 0.0, 0.0, _
                                0.0, 0.5, 0.0, 0.0, _
                                0.0, 0.0, 0.5, 0.0, _
                                0.5, 0.5, 0.5, 1.0}
    Public Sub reset_shadowFbo()
        If depthBuffer > 0 Then
            Gl.glDeleteTextures(1, depthBuffer)
            Gl.glFinish()
        End If
        If depthBuffer > 0 Then
            Gl.glDeleteTextures(1, tempDepth)
            Gl.glFinish()
        End If
        If depthId > 0 Then
            Gl.glDeleteRenderbuffersEXT(1, depthId)
            Gl.glFinish()
        End If
        If shadowFramebuffer > 0 Then
            Gl.glDeleteFramebuffersEXT(1, shadowFramebuffer)
            Gl.glFinish()
        End If
        make_shadow_fbo()
        Gl.glFinish()
    End Sub
    Public Sub make_shadow_fbo()
        Gl.glGenTextures(1, depthBuffer)
        Dim er = Gl.glGetError

        er = Gl.glGetError
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)

        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGb32F_ARB, CInt(shadowMapSize), CInt(shadowMapSize), 0, Gl.GL_RGBA, Gl.GL_FLOAT, Nothing)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        er = Gl.glGetError


        ''''''''''''''''''''''''''
        Gl.glGenTextures(1, tempDepth)
        er = Gl.glGetError

        er = Gl.glGetError
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, tempDepth)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)

        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB32F_ARB, CInt(shadowMapSize), CInt(shadowMapSize), 0, Gl.GL_RGBA, Gl.GL_FLOAT, Nothing)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        er = Gl.glGetError
        If er = 1285 Then
            If MsgBox("You do not have enough contiguous video memory." + vbCrLf + _
                   "Try restaring and setting the shadow to best" + vbCrLf + _
                   "before loading a tank or importing a FBX." + vbCrLf + _
                   "Exit so this setting is saved and restart again." + vbCrLf + _
                   "Would you like me to reset the shadow quality to lowest and Restart TE?", MsgBoxStyle.YesNo, "Out Of Video Memory!") = MsgBoxResult.Yes Then
                My.Settings.shadow_quality = "512"
                My.Settings.Save()
                DisableOpenGL()
                Application.Restart()
            End If
        End If

        ''''''''''''''''''''''''''
        ' create framebuffer
        Gl.glGenFramebuffersEXT(1, shadowFramebuffer)
        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, shadowFramebuffer)
        er = Gl.glGetError
        Gl.glGenRenderbuffersEXT(1, depthId)
        Gl.glBindRenderbufferEXT(Gl.GL_RENDERBUFFER_EXT, depthId)
        Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT24, shadowMapSize, shadowMapSize)
        Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, depthId) '24f depth
        er = Gl.glGetError

        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, depthBuffer, 0) '32f color
        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, tempDepth, 0) '32f color

        Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

        If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
            MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
            Return
        End If
        frmMain.PB3.Width = shadowMapSize
        frmMain.PB3.Height = shadowMapSize

    End Sub
    Private Sub attach_depth()
        detach_textures()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, depthBuffer, 0) '32f color
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDrawBuffers(1, attach_depthBuffer)


    End Sub
    Private Sub attach_temp()
        detach_textures()
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, tempDepth)
        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, tempDepth, 0) '32f color
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDrawBuffers(1, attach_tempDepth)

        'Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

        'If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
        'MsgBox("Failed to create Deferred FBO", MsgBoxStyle.Critical, "Not good!")
        'Return
        'End If

    End Sub
    Private Sub detach_textures()
        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT1_EXT, Gl.GL_TEXTURE_2D, 0, 0)
    End Sub

    Public Sub render_depth_to_depth_texture(ByVal lightId As Integer)


        If Not (Wgl.wglMakeCurrent(pb3_hDC, pb3_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 3 ")
        End If
        'frmMain.PB3.BringToFront()
        frmMain.PB3.Visible = False
        Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, shadowFramebuffer)
        attach_depth()
        Gl.glDrawBuffer(Gl.GL_BACK)

        Gl.glViewport(0, 0, shadowMapSize, shadowMapSize)
        'ResizeGL()
        'Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
        'Gl.glPolygonOffset(2.0, 2.0)
        Gl.glClearDepth(1.0)
        Gl.glClearColor(0.0, 0.0, 0.0, 1.0)
        Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT Or Gl.GL_COLOR_BUFFER_BIT)



        Dim lx, ly, lz As Single
        lx = 0.0!
        ly = 1.0!
        lz = 0.0!
        Dim cx, cy, cz As Single
        Select Case selected_light

            Case 0
                cx = position0(0)
                cy = position0(1)
                cz = position0(2)
            Case 1
                cx = position1(0)
                cy = position1(1)
                cz = position1(2)
            Case 2
                cx = position2(0)
                cy = position2(1)
                cz = position2(2)
        End Select

        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()
        'Glu.gluPerspective(40.0F, 1.0, 0.1F, 1000.0F)
        Dim view_size As Double = 8.0
        Gl.glOrtho(-view_size, view_size, -view_size, view_size, -0, 25.0)
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, lightProjection)

        'Glu.gluLookAt(lx, ly, lz, cx, cy, cz, 0.0F, 1.0F, 0.0F)
        Glu.gluLookAt(cx, cy, cz, lx, ly, lz, 0.0F, 1.0F, 0.0F)

        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDepthMask(Gl.GL_TRUE)
        Gl.glDepthRange(0.0, 1.0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)

        Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
        Gl.glPolygonOffset(2.0, 2.0)

        'render carraige
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        For jj = object_count To 1 Step -1
            If _group(jj).is_carraige And Not _group(jj).doubleSided Then
                Gl.glUniform1i(depth_alphaTest, _group(jj).alphaTest)
                Gl.glUniform1i(depth_alphaRef, _group(jj).alphaRef)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                If _object(jj).visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            End If
        Next
        Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
        'render tracks
        Gl.glUseProgram(shader_list.depth_shader) '<<<==================================== depth shader
        'Gl.glUseProgram(shader_list.basic_shader) '<<<==================================== debug shader
        Gl.glFrontFace(Gl.GL_CW)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glEnable(Gl.GL_CULL_FACE)
        For jj = object_count To 1 Step -1
            If _group(jj).doubleSided Then
                Gl.glUniform1i(depth_alphaTest, _group(jj).alphaTest)
                Gl.glUniform1i(depth_alphaRef, _group(jj).alphaRef)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                If _object(jj).visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            End If
        Next
        'render carraige
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        For jj = object_count To 1 Step -1
            If _group(jj).is_carraige And Not _group(jj).doubleSided Then
                Gl.glUniform1i(depth_alphaTest, _group(jj).alphaTest)
                Gl.glUniform1i(depth_alphaRef, _group(jj).alphaRef)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                If _object(jj).visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            End If
        Next

        Gl.glUniform1i(depth_normalMap, 0)
        Gl.glEnable(Gl.GL_CULL_FACE)

        If MODEL_LOADED _
                        And frmMain.m_load_textures.Checked _
                        And Not frmMain.m_show_fbx.Checked _
                        And Not frmMain.m_show_bsp2.Checked _
                        And Not frmMain.m_simple_lighting.Checked Then

            Gl.glCullFace(Gl.GL_BACK)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glDrawBuffer(Gl.GL_BACK)

            'FIRST DRAW ONLY TO THE Z BUFFER. THIS SHOULD STOP SHADOWING THROUGH THE MODEL
            Gl.glFrontFace(Gl.GL_CCW)
            For jj = object_count To 1 Step -1
                If Not _group(jj).is_carraige And Not _group(jj).doubleSided Then


                    Gl.glUniform1i(depth_alphaTest, 0)
                    Gl.glUniform1i(depth_alphaRef, _group(jj).alphaRef)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)

                    If _object(jj).visible Then
                        Gl.glCallList(_object(jj).main_display_list)
                    End If
                End If


            Next
            Gl.glDrawBuffer(Gl.GL_BACK)
            Gl.glUniform1i(depth_alphaTest, 0)
            '==========================================================
            'terrain
            Gl.glPushMatrix()
            Gl.glTranslatef(0.0, -0.06, 0.0)
            Gl.glRotatef(0.25, -1.0, 0.0, 1.0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, terrain_textureNormalId)
            Gl.glCallList(terrain_modelId)
            Gl.glPopMatrix()
            '==========================================================
            Gl.glFrontFace(Gl.GL_CW)
            For jj = object_count To 1 Step -1
                If Not _group(jj).is_carraige And Not _group(jj).doubleSided Then
                    Gl.glUniform1i(depth_alphaTest, 0)
                    Gl.glUniform1i(depth_alphaRef, _group(jj).alphaRef)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)

                    If _object(jj).visible Then
                        Gl.glCallList(_object(jj).main_display_list)
                    End If
                End If

            Next

        End If
        Gl.glUseProgram(0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)


        '====================================


        Gl.glReadBuffer(Gl.GL_BACK)

        Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)

        'create projection matrix for the lights location;
        Gl.glPushMatrix()
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, MV)
        Gl.glGetFloatv(Gl.GL_PROJECTION_MATRIX, lightProjection)
        Gl.glLoadIdentity()
        Gl.glLoadMatrixf(bias)
        ' concatating all matrices into one.
        Gl.glMultMatrixf(lightProjection)
        Gl.glMultMatrixf(MV)
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, lightProjection)
        Gl.glPopMatrix()
        '###############################################################################################
        '
        'GoTo nope
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, shadowMapSize, -shadowMapSize, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        'Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glUseProgram(shader_list.gaussian_shader)
        Gl.glUniform1i(gaus_image, 0)
        'GoTo nope
        'blur the depth map
        Dim qual As Integer = 1
        'adjust bluring based on shadow map size
        Select Case shadowMapSize
            Case 4096
                qual = 1
            Case 2048
                qual = 4
            Case 1024
                qual = 5
            Case 512
                qual = 6
        End Select
        For i = 1 To qual

            attach_temp()

            Gl.glUniform1i(gaus_switch, 0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 12)
            draw_rect()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            attach_depth()
            'Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
            Gl.glUniform1i(gaus_switch, 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, tempDepth)
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 12)
            draw_rect()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Next
        Gl.glUseProgram(0)
nope:
        If Not frmMain.m_shadow_preview.Checked Then
            Return
        End If
        '###############################################################################################
        Dim w, h As Integer
        G_Buffer.getsize(w, h)
        ResizeGL(w, h)

        Gl.glEnable(Gl.GL_DEPTH_TEST)

        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 3 ")
        End If
        Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()

        Glu.gluPerspective(60.0F, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 0.1F, 1000)

        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix  
        frmMain.set_eyes()
        Gl.glClearColor(0.0, 0.0, 0.75, 1.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        'Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, gBufferFBO)
        If MODEL_LOADED _
                And frmMain.m_load_textures.Checked _
                And Not frmMain.m_show_fbx.Checked _
                And Not frmMain.m_show_bsp2.Checked _
                And Not frmMain.m_simple_lighting.Checked Then

            Gl.glUseProgram(shader_list.shadowTest_shader)
            Gl.glUniform1i(shadowTest_textureMap, 1)
            Gl.glUniform1i(shadowTest_depthMap, 0)
            Gl.glUniformMatrix4fv(shadowTest_shadowProjection, 1, 0, lightProjection)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
            Gl.glFrontFace(Gl.GL_CW)
            For jj = 1 To object_count - track_info.segment_count
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If _group(jj).doubleSided Or Not _group(jj).metal_textured Then
                    'Gl.glCullFace(Gl.GL_NONE)
                    Gl.glDisable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
                Else
                    'Gl.glCullFace(Gl.GL_BACK)
                    Gl.glEnable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
                End If

                If _object(jj).visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If


            Next
            Gl.glCallList(terrain_modelId)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glUseProgram(0)

        End If
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_texture)
        Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 0, 0, w, h, 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)


        'Thread.Sleep(15)
    End Sub
    Private Sub draw_rect()
        Dim p = New Point(0, 0)
        Dim w = shadowMapSize
        Dim h = shadowMapSize
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
    Private Sub draw_rect_preview(ByVal p As Point, ByVal w As Integer, ByVal h As Integer)
        Gl.glBegin(Gl.GL_QUADS)
        '  CW...
        '  1 ------ 4
        '  |        |
        '  |        |
        '  2 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y - h)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glEnd()

    End Sub
    Public Sub draw_to_full_screen_mono_shadow_texture()
        Dim w, h As Integer
        G_Buffer.getsize(w, h)
        ResizeGL(w, h)

        Gl.glEnable(Gl.GL_DEPTH_TEST)

        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 3 ")
        End If
        Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()

        Glu.gluPerspective(60.0F, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 0.1F, 1000)

        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix  
        frmMain.set_eyes()
        Gl.glClearColor(0.0, 0.0, 0.75, 1.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        'Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, gBufferFBO)
        If MODEL_LOADED _
                And frmMain.m_load_textures.Checked _
                And Not frmMain.m_show_fbx.Checked _
                And Not frmMain.m_show_bsp2.Checked _
                And Not frmMain.m_simple_lighting.Checked Then

            Gl.glUseProgram(shader_list.shadowTest_shader)
            Gl.glUniform1i(shadowTest_textureMap, 1)
            Gl.glUniform1i(shadowTest_depthMap, 0)
            Gl.glUniformMatrix4fv(shadowTest_shadowProjection, 1, 0, lightProjection)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
            Gl.glFrontFace(Gl.GL_CW)
            For jj = 1 To object_count - track_info.segment_count
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If _group(jj).doubleSided Or Not _group(jj).metal_textured Then
                    'Gl.glCullFace(Gl.GL_NONE)
                    Gl.glDisable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
                Else
                    'Gl.glCullFace(Gl.GL_BACK)
                    Gl.glEnable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
                End If

                If _object(jj).visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If


            Next
            Gl.glCallList(terrain_modelId)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glUseProgram(0)

        End If
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_texture)
        Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 0, 0, w, h, 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        For i = 1 To 1

            attach_temp()

            Gl.glUniform1i(gaus_switch, 0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_texture)
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 12)
            draw_rect()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, gBufferFBO)
            G_Buffer.attach_Shadow_render_texture()
            'Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
            Gl.glUniform1i(gaus_switch, 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, tempDepth)
            'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 12)
            draw_rect()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)
        Next
        Gl.glUseProgram(0)


    End Sub
    Public Sub show_depth_texture()
        Dim p As New Point
        Dim w As Integer = 512
        Dim h As Integer = 512
        p.X = frmMain.pb1.Width - (w + 10)
        p.Y = -10
        '-----------------------------------------------------------------------
        'depth
        '----------------------------------------------------------------------- 
        Gl.glColor3f(1.0, 0.0, 1.0)
        Gl.glLineWidth(2.0)
        Gl.glUseProgram(shader_list.toLinear_shader) '<<<<======================================t oLinear shader
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        Gl.glDisable(Gl.GL_BLEND)

        Gl.glUniform1i(toLinear_depthMap, 0)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glDisable(Gl.GL_CULL_FACE)

        draw_rect_preview(p, w, h)

        Gl.glUseProgram(0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
        draw_rect_preview(p, w, h)

        '-----------------------------------------------------------------------
        'test render
        '-----------------------------------------------------------------------
        p.Y -= w
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Dim x, y As Integer
        G_Buffer.getsize(x, y)
        Dim aspect As Single = CSng(y / x)

        h *= aspect
        Gl.glUseProgram(shader_list.r2mono_shader)
        Gl.glUniform1i(r2mono_shadow, 0)

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_texture)

        draw_rect_preview(p, w, h)
        Gl.glUseProgram(0)

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
        draw_rect_preview(p, w, h)
        Gl.glLineWidth(1.0)

    End Sub


End Module
