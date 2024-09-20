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
    Public d_sb As New StringBuilder
    Public bbs(8) As vec3
    Public shadow_fbo As New shadow_fbo_
    Public shadowMapSize As Integer = 4096
    Public depthId As Integer
    Public depthBuffer As Integer
    Public shadowFramebuffer As Integer
    Public MV(15) As Single
    Public lightProjection() As Single = { _
                                            1.0, 0.0, 0.0, 0.0, _
                                            0.0, 1.0, 0.0, 0.0, _
                                            0.0, 0.0, 1.0, 0.0, _
                                            0.0, 0.0, 0.0, 1.0}
    Dim bias() As Single = { _
                                0.5, 0.0, 0.0, 0.0, _
                                0.0, 0.5, 0.0, 0.0, _
                                0.0, 0.0, 0.5, 0.0, _
                                0.5, 0.5, 0.5, 1.0}

    Public Class shadow_fbo_
        Private attach_depthBuffer() As Integer = {Gl.GL_COLOR_ATTACHMENT0_EXT}
        Private attach_tempDepth() As Integer = {Gl.GL_COLOR_ATTACHMENT1_EXT}

        Public Sub shadow_fbo_shut_down()
            If depthBuffer > 0 Then
                Gl.glDeleteTextures(1, depthBuffer)
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

        End Sub
        Public Sub reset_shadowFbo()
            BlurShadowFBO.init()
            If depthBuffer > 0 Then
                Gl.glDeleteTextures(1, depthBuffer)
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
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP)
            Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB32F_ARB, CInt(shadowMapSize), CInt(shadowMapSize), 0, Gl.GL_RGB, Gl.GL_FLOAT, Nothing)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            er = Gl.glGetError



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
            Gl.glRenderbufferStorageEXT(Gl.GL_RENDERBUFFER_EXT, Gl.GL_DEPTH_COMPONENT16, shadowMapSize, shadowMapSize)
            Gl.glFramebufferRenderbufferEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_DEPTH_ATTACHMENT_EXT, Gl.GL_RENDERBUFFER_EXT, depthId) '16f depth
            er = Gl.glGetError

            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, depthBuffer, 0) '16f color
            Gl.glDrawBuffers(1, attach_depthBuffer)

            Dim Status = Gl.glCheckFramebufferStatusEXT(Gl.GL_FRAMEBUFFER_EXT)

            If Status <> Gl.GL_FRAMEBUFFER_COMPLETE_EXT Then
                MsgBox("Failed to create Shadow FBO", MsgBoxStyle.Critical, "Not good!")
                Return
            End If
            frmMain.PB3.Width = shadowMapSize
            frmMain.PB3.Height = shadowMapSize

        End Sub
        Public Sub attach_depth()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, depthBuffer, 0) '32f color
        End Sub
        Private Sub detach_textures()
            Gl.glFramebufferTexture2DEXT(Gl.GL_FRAMEBUFFER_EXT, Gl.GL_COLOR_ATTACHMENT0_EXT, Gl.GL_TEXTURE_2D, 0, 0)
        End Sub
    End Class
    Public w_max, w_min, h_max, h_min

    Private Sub get_max_min_x(ByRef v As Single)

        If v > w_max Then w_max = v
        If v < w_min Then w_min = v

        'If v.z > w_max Then w_max = v.z
        'If v.z < w_min Then w_min = v.z

    End Sub
    Private Sub get_max_min_z(ByRef v As Single)
        If v > h_max Then h_max = v
        If v < h_min Then h_min = v

        'If v.x > h_max Then h_max = v.x
        'If v.x < h_min Then h_min = v.x

    End Sub

    Private Sub find_extremes()
        Dim v As vect3
        Dim z = (Abs(z_min) - z_max) / 2.0
        For jj = 1 To object_count
            For i As UInt32 = 0 To _group(jj).nVertices_ - 1
                v.x = _group(jj).vertices(i).x
                v.y = _group(jj).vertices(i).y
                v.z = _group(jj).vertices(i).z - z

                v = trans_vertex(v, lightProjection)
                If v.x > w_max Then w_max = v.x
                If v.x < w_min Then w_min = v.x
                If v.z > h_max Then h_max = v.z
                If v.z < h_min Then h_min = v.z

            Next
        Next
    End Sub

    Public Sub render_depth_to_depth_texture(ByVal lightId As Integer)
        w_max = -10000
        w_min = 100000
        h_max = -10000
        h_min = 10000


        Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, shadowFramebuffer)
        shadow_fbo.attach_depth()
        Gl.glDrawBuffer(Gl.GL_BACK)

        Gl.glViewport(0, 0, shadowMapSize, shadowMapSize)

        Gl.glClearDepth(1.0)
        Gl.glClearColor(0.0, 0.0, 0.0, 1.0)
        Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT Or Gl.GL_COLOR_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)

        Dim lx, ly, lz As Single
        lx = tank_center_X
        ly = tank_center_Y
        lz = tank_center_Z
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

        Gl.glPushMatrix()
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()
        Dim view_size As Double = 8.0
        'd_sb.Clear() 'debug window

        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix
        Dim r = Sqrt(cx ^ 2 + cy ^ 2 + cz ^ 2)
        Dim rt = Asin(cy / r)
        'Gl.glRotatef(rt * 57.2957795, 0.0!, 0.0!, 1.0)
        'Glu.gluLookAt(cx, cy, cz, lx, ly, lz, 0.0, 1.0F, 0.0F)
        Glu.gluLookAt(cx, 0.0, cz, lx, 0.0, lz, 0.0, 1.0F, 0.0F)
        Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, lightProjection)
        'transform BB to light view
        make_bounding_points()
        Gl.glPopMatrix()
        '==========================================================================
        For i = 1 To 8
            get_max_min_x(bbs(i).x)
        Next
        For i = 1 To 8
            get_max_min_z(bbs(i).z)
        Next
        'find_extremes()
        'd_sb.AppendLine("================================")
        'd_sb.AppendLine("w_min:" + w_min.ToString)
        'd_sb.AppendLine("w_max:" + w_max.ToString)
        'd_sb.AppendLine("h_min:" + h_min.ToString)
        'd_sb.AppendLine("h_max:" + h_max.ToString)
        'frmDebug.TextBox1.Text = d_sb.ToString


        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()
        If w_min > h_min Then
            w_min = h_min
        Else
            h_min = w_min
        End If

        If w_max < h_max Then
            w_max = h_max
        Else
            h_max = w_max
        End If
        Gl.glOrtho(w_min, w_max, h_min, h_max, 6.0!, 30.0!)
        Glu.gluLookAt(cx, cy, cz, lx, ly, lz, 0.0, 1.0F, 0.0F)
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix
        '==========================================================================

        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDepthMask(Gl.GL_TRUE)
        Gl.glDepthRange(0.0, 1.0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        '==========================================
        'draw test bp points
        If False Then
            Gl.glColor3f(1.0, 0.0, 0.0) 'red
            Dim phs As Single = 0.1
            For i = 1 To 8
                Gl.glPushMatrix()
                Gl.glTranslatef(bbs(i).x, bbs(i).y, bbs(i).z)
                glutSolidSphere(phs, 5, 5)
                Gl.glPopMatrix()
            Next

            Gl.glLineWidth(4)

            Gl.glBegin(Gl.GL_LINE_LOOP)
            Gl.glVertex3f(x_max, y_max, z_max)
            Gl.glVertex3f(x_min, y_max, z_max)
            Gl.glVertex3f(x_min, y_max, z_min)
            Gl.glVertex3f(x_max, y_max, z_min)
            Gl.glVertex3f(x_max, y_max, z_max)
            Gl.glEnd()
            Gl.glBegin(Gl.GL_LINE_LOOP)
            Gl.glVertex3f(x_max, y_min, z_max)
            Gl.glVertex3f(x_min, y_min, z_max)
            Gl.glVertex3f(x_min, y_min, z_min)
            Gl.glVertex3f(x_max, y_min, z_min)
            Gl.glVertex3f(x_max, y_min, z_max)
            Gl.glEnd()

            'Dim c = (y_max + y_min) / 2.0!
            'Gl.glBegin(Gl.GL_LINE_LOOP)
            'Gl.glVertex3f(w_max, c, w_max)
            'Gl.glVertex3f(w_min, c, w_max)
            'Gl.glVertex3f(w_min, c, w_min)
            'Gl.glVertex3f(w_max, c, w_min)
            'Gl.glVertex3f(w_max, c, w_max)
            'Gl.glEnd()
        End If

        '==========================================

        'Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
        Gl.glPolygonOffset(2.0, 2.0)
        Gl.glUseProgram(shader_list.depth_shader) '<<<==================================== depth shader
        Gl.glUniform1i(depth_normalMap, 0)
        'render carraige
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        For jj = object_count To 1 Step -1
            If _group(jj).is_carraige And Not _group(jj).doubleSided Then
                Gl.glUniform1i(depth_alphaTest, _group(jj).alphaTest)
                Gl.glUniform1i(depth_alphaRef, _group(jj).alphaRef)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                If _object(jj).visible And _group(jj).component_visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            End If
        Next
        Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
        'render tracks

        Gl.glFrontFace(Gl.GL_CW)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glEnable(Gl.GL_CULL_FACE)
        For jj = object_count To 1 Step -1
            If _group(jj).doubleSided Then
                Gl.glUniform1i(depth_alphaTest, _group(jj).alphaTest)
                Gl.glUniform1i(depth_alphaRef, _group(jj).alphaRef)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                If _object(jj).visible And _group(jj).component_visible Then
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
                If _object(jj).visible And _group(jj).component_visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If
            End If
        Next

        Gl.glUniform1i(depth_normalMap, 0)
        Gl.glEnable(Gl.GL_CULL_FACE)

        If MODEL_LOADED _
                        And frmMain.m_load_textures.Checked _
                        And Not frmMain.m_show_fbx.Checked _
                        And Not frmMain.m_simple_lighting.Checked Then

            Gl.glCullFace(Gl.GL_BACK)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Gl.glDrawBuffer(Gl.GL_BACK)

            'FIRST DRAW ONLY TO THE Z BUFFER. THIS SHOULD STOP SHADOWING THROUGH THE MODEL
            Gl.glFrontFace(Gl.GL_CCW)
            For jj = object_count To 1 Step -1
                Gl.glDisable(Gl.GL_CULL_FACE)
                If Not _group(jj).is_carraige Then


                    Gl.glUniform1i(depth_alphaTest, _group(jj).alphaTest)
                    Gl.glUniform1i(depth_alphaRef, _group(jj).alphaRef)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)

                    If _object(jj).visible And _group(jj).component_visible Then
                        Gl.glCallList(_object(jj).main_display_list)
                    End If
                End If
                Gl.glEnable(Gl.GL_CULL_FACE)


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

                    If _object(jj).visible And _group(jj).component_visible Then
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
 
        'BlurShadowFBO.blur_depth_texture()
nope:
        '###############################################################################################
        'this creates the shadow mask texture



        Dim w, h As Integer
        G_Buffer.getsize(w, h)
        ResizeGL(w, h)

        Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, gBufferFBO)
        G_Buffer.attach_Shadow_render_texture()

        Gl.glEnable(Gl.GL_DEPTH_TEST)

        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 3 ")
        End If
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()

        Glu.gluPerspective(FOV, CSng((frmMain.pb1.Width) / (frmMain.pb1.Height)), 0.1F, 1000.0)

        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix  
        frmMain.set_eyes()
        Gl.glClearColor(0.0, 0.0, 0.75, 1.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glEnable(Gl.GL_DEPTH_TEST)

        If MODEL_LOADED _
                And frmMain.m_load_textures.Checked _
                And Not frmMain.m_show_fbx.Checked _
                And Not frmMain.m_simple_lighting.Checked Then

            Gl.glUseProgram(shader_list.shadowTest_shader)
            Gl.glUniform3f(shadowTest_light_pos, cx, cy, cz)

            Gl.glUniform1i(shadowTest_depthMap, 0)
            Gl.glUniform1i(shadowTest_normalMap, 1)

            Gl.glUniformMatrix4fv(shadowTest_shadowProjection, 1, 0, lightProjection)

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
            Gl.glFrontFace(Gl.GL_CW)

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            For jj = 1 To object_count - track_info.segment_count
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                Gl.glUniform1i(shadowTest_alphaRef, _group(jj).alphaRef)
                Gl.glUniform1i(shadowTest_alphaTest, _group(jj).alphaTest)
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CCW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If _group(jj).doubleSided Or Not _group(jj).metal_textured Then
                    Gl.glDisable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
                Else
                    'Gl.glCullFace(Gl.GL_BACK)
                    Gl.glEnable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
                End If

                If _object(jj).visible And _group(jj).component_visible Then
                    Gl.glCallList(_object(jj).main_display_list)
                End If


            Next
            Gl.glUniform1i(shadowTest_alphaTest, 0)
            Gl.glCallList(terrain_modelId)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glUseProgram(0)

            ViewOrtho()
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

            Gl.glUseProgram(shader_list.blurR_shader)
            Gl.glUniform1i(blurR_image, 0)

            For i = 1 To 1

                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

                G_Buffer.attach_Shadow_render_temp()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_texture)
                Gl.glUniform1i(blurR_switch, 0)
                'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 12)

                draw_scr_rect()

                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

                G_Buffer.attach_Shadow_render_texture()
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, rendered_shadow_temp)

                Gl.glUniform1i(blurR_switch, 1)

                'Gl.glBindTexture(Gl.GL_TEXTURE_2D, tempDepth)
                draw_scr_rect()
            Next
            'Gdi.SwapBuffers(pb1_hDC)
        End If
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glUseProgram(0)


        Gl.glBindFramebufferEXT(Gl.GL_DRAW_FRAMEBUFFER_EXT, 0)

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
    Private Sub draw_scr_rect()
        Dim p = New Point(0, 0)
        Dim w = frmMain.pb1.Width
        Dim h = frmMain.pb1.Height
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
    Public Sub make_bounding_points()
        ReDim bbs(8)
        Dim tv As vect3
        'y_min = -0.1
        '1
        tv.x = x_max
        tv.y = y_max
        tv.z = z_max
        inverse_rotate_only(tv, lightProjection, 1)
        '2
        tv.x = x_min
        tv.y = y_max
        tv.z = z_max
        inverse_rotate_only(tv, lightProjection, 2)
        '3
        tv.x = x_min
        tv.y = y_max
        tv.z = z_min
        inverse_rotate_only(tv, lightProjection, 3)
        '4
        tv.x = x_max
        tv.y = y_max
        tv.z = z_min
        inverse_rotate_only(tv, lightProjection, 4)
        '5
        tv.x = x_max
        tv.y = y_min
        tv.z = z_max
        inverse_rotate_only(tv, lightProjection, 5)
        '6
        tv.x = x_min
        tv.y = y_min
        tv.z = z_max
        inverse_rotate_only(tv, lightProjection, 6)
        '7
        tv.x = x_min
        tv.y = y_min
        tv.z = z_min
        inverse_rotate_only(tv, lightProjection, 7)
        '8
        tv.x = x_max
        tv.y = y_min
        tv.z = z_min
        inverse_rotate_only(tv, lightProjection, 8)

    End Sub

    Public Sub inverse_rotate_only(ByVal v As vect3, ByRef m() As Single, ByRef idx As Integer)

        'This is being a total pain in the ass
        Dim x = (Abs(x_min) - x_max) / 2.0
        Dim y = (Abs(y_min) - y_max) / 2.0
        Dim z = (Abs(z_min) - z_max) / 4.0

        Dim agl = Asin(x_max / Sqrt(z_min ^ 2 + x_max ^ 2))
        v.x -= tank_center_X
        'v.y -= tank_center_Y
        v.z -= z
        Dim l = Sqrt(v.z ^ 2 + v.x ^ 2)
        Dim o_agl = Atan2(v.y, v.x)
        v.x = l * Cos(o_agl + agl)
        v.z = l * Sin(o_agl + agl)
        'Return vo

        bbs(idx).x = v.x
        bbs(idx).y = v.y
        bbs(idx).z = v.z * 1.0
        'd_sb.AppendLine(idx.ToString + ": " + vo.x.ToString("00.000") + " " + vo.y.ToString("00.000") + " " + vo.z.ToString("00.000"))
    End Sub

    Public Function trans_vertex(ByVal v As vect3, ByRef m() As Single) As vect3

        'This is being a total pain in the ass
        Dim vo As vect3
        Dim x = (Abs(x_min) - x_max) / 2.0
        Dim y = (Abs(y_min) - y_max) / 2.0
        Dim z = (Abs(z_min) - z_max) / 2.0
        v.x += x
        'v.y += y
        'v.z += z
        'Return vo
        vo.x = -(m(0) * v.x) + (m(1) * v.y) + (m(2) * v.z)
        vo.y = (m(4) * v.x) + (m(5) * v.y) + (m(6) * v.z)
        vo.z = -(m(8) * v.x) + (m(9) * v.y) + (m(10) * v.z)
        Return vo
    End Function

End Module
