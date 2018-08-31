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

#End Region



Public Class frmTextureViewer
    Dim center As vec2
    Public Sub set_current_image()
        Dim w, h As Integer
        Dim miplevel As Integer = 0
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, current_image)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, miplevel, Gl.GL_TEXTURE_WIDTH, w)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, miplevel, Gl.GL_TEXTURE_HEIGHT, h)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Zoom_Factor = 1.0
        old_w = w
        old_h = h
        rect_size = New Point(w, h)
        frmMain.pb2.Parent = Me
        frmMain.pb2.Visible = True
        frmMain.pb2.BringToFront()
        frmMain.pb2.Dock = DockStyle.Fill
        frmMain.pb2.Location = New Point(0, 0)
        rect_location = New Point((frmMain.pb2.Width - rect_size.X) / 2, (frmMain.pb2.Height - rect_size.Y) / 2)
        drawing_ = False
        frmMain.found_triangle_tv = 0
        current_vertex = 0
        draw()
        draw()

    End Sub
    Private Sub check_if_centering_on_selection()
        'if the C key is pressed, it toggles centering on the current_vertex
        If CENTER_SELECTION Then
            If Me.m_uvs_only.Checked Or Me.m_show_uvs.Checked Then
                If current_vertex > 0 Then
                    If frmMain.m_show_fbx.Checked Then
                        Dim p1 = fbxgrp(current_tank_part).indicies(current_vertex - 1).v1
                        Dim p2 = fbxgrp(current_tank_part).indicies(current_vertex - 1).v2
                        Dim p3 = fbxgrp(current_tank_part).indicies(current_vertex - 1).v3
                        Dim v1 = fbxgrp(current_tank_part).vertices(p1)
                        Dim v2 = fbxgrp(current_tank_part).vertices(p2)
                        Dim v3 = fbxgrp(current_tank_part).vertices(p3)
                        Dim u1 As New uv_
                        Dim u2 As New uv_
                        Dim u3 As New uv_
                        u1.u = v1.u
                        u1.v = v1.v
                        u2.u = v2.u
                        u2.v = v2.v
                        u3.u = v3.u
                        u3.v = v3.v

                        u1.u = rect_location.X + (u1.u * rect_size.X)
                        u2.u = rect_location.X + (u2.u * rect_size.X)
                        u3.u = rect_location.X + (u3.u * rect_size.X)

                        u1.v = -rect_location.Y + (-u1.v * rect_size.Y)
                        u2.v = -rect_location.Y + (-u2.v * rect_size.Y)
                        u3.v = -rect_location.Y + (-u3.v * rect_size.Y)

                        center.x = (u1.u + u2.u + u3.u) / 3.0!
                        center.y = (u1.v + u2.v + u3.v) / 3.0!
                        center.x = (frmMain.pb2.Width / 2.0!) - center.x
                        center.y = (frmMain.pb2.Height / 2.0!) + center.y
                        Return

                    Else


                        Dim u1 As New uv_
                        Dim u2 As New uv_
                        Dim u3 As New uv_
                        u1.u = _object(current_part).tris(current_vertex).uv1.u
                        u1.v = _object(current_part).tris(current_vertex).uv1.v
                        u2.u = _object(current_part).tris(current_vertex).uv2.u
                        u2.v = _object(current_part).tris(current_vertex).uv2.v
                        u3.u = _object(current_part).tris(current_vertex).uv3.u
                        u3.v = _object(current_part).tris(current_vertex).uv3.v

                        u1.u = rect_location.X + (u1.u * rect_size.X)
                        u2.u = rect_location.X + (u2.u * rect_size.X)
                        u3.u = rect_location.X + (u3.u * rect_size.X)

                        u1.v = -rect_location.Y + (-u1.v * rect_size.Y)
                        u2.v = -rect_location.Y + (-u2.v * rect_size.Y)
                        u3.v = -rect_location.Y + (-u3.v * rect_size.Y)

                        center.x = (u1.u + u2.u + u3.u) / 3.0!
                        center.y = (u1.v + u2.v + u3.v) / 3.0!
                        center.x = (frmMain.pb2.Width / 2.0!) - center.x
                        center.y = (frmMain.pb2.Height / 2.0!) + center.y
                        Return
                    End If
                End If
            End If
        Else
        End If
        center.x = 0
        center.y = 0

    End Sub
    Dim drawing_ As Boolean = False
    Public Sub draw()
        If drawing_ Then Return
        drawing_ = True
        If Not _Started Then
            drawing_ = False
            Return
        End If
        If Not Me.Visible Then
            drawing_ = False
            Return
        End If
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            drawing_ = False
            Return
        End If

        'set ortho mode.
        '#######################################################################################
        check_if_centering_on_selection()
        Gl.glFrontFace(Gl.GL_CW)

        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glViewport(0, 0, frmMain.pb2.Width, frmMain.pb2.Height)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, frmMain.pb2.Width, -frmMain.pb2.Height, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        '#######################################################################################
        'set rendering modals
        If m_show_uvs.Checked Or m_uvs_only.Checked Then
            If frmMain.m_show_fbx.Checked Then
                get_fbx_triangle()
            Else
                get_triangle()
            End If
        End If

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        '#######################################################################################
        'clear buffer
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        '#######################################################################################
        'draw checkboard background
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, checkerboard_id)

        Gl.glBegin(Gl.GL_QUADS)

        Dim v As Point = frmMain.pb2.Size
        Dim w As Single = v.X / 320.0 ' size of the checker board
        Dim h As Single = v.Y / 320.0 ' size of the checker board
        'h = h / w
        Gl.glTexCoord2f(0.0, 0.0)
        Gl.glVertex3f(0.0, 0.0, -0.15)
        Gl.glTexCoord2f(0.0, h)
        Gl.glVertex3f(0.0, -v.Y, -0.15)
        Gl.glTexCoord2f(w, h)
        Gl.glVertex3f(v.X, -v.Y, -0.15)
        Gl.glTexCoord2f(w, 0.0)
        Gl.glVertex3f(v.X, 0.0, -0.15)
        Gl.glEnd()
        '#######################################################################################
        'draw current_image
        If m_alpha_enabled.Checked Then
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Else
            Gl.glDisable(Gl.GL_BLEND)
        End If

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, current_image)


        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Dim u4() As Single = {0.0, 1.0}
        Dim u3() As Single = {1.0, 1.0}
        Dim u2() As Single = {1.0, 0.0}
        Dim u1() As Single = {0.0, 0.0}

        Dim p1, p2, p3, p4 As Point
        Dim L, S As New Point
        L = rect_location
        L.X += center.x
        L.Y += center.y
        S = rect_size
        L.Y *= -1
        S.Y *= -1

        p1 = L
        p2 = L
        p2.X += rect_size.X
        p3 = L + S
        p4 = L
        p4.Y += S.Y

        'draw and flip bufffers so the user can see the image
        If Not m_uvs_only.Checked Then

            Gl.glBegin(Gl.GL_QUADS)
            '---
            Gl.glTexCoord2fv(u1)
            Gl.glVertex2f(p1.X, p1.Y)
            Gl.glTexCoord2fv(u2)
            Gl.glVertex2f(p2.X, p2.Y)
            Gl.glTexCoord2fv(u3)
            Gl.glVertex2f(p3.X, p3.Y)
            Gl.glTexCoord2fv(u4)
            Gl.glVertex2f(p4.X, p4.Y)
            '--
            Gl.glEnd()
        End If
        '#######################################################################################
        'reset things
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glUseProgram(0)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        If m_show_uvs.Checked Or m_uvs_only.Checked Then
            If frmMain.m_show_fbx.Checked Then
                draw_fbx_uvs()
            Else
                draw_uvs()
            End If
        End If
        '#######################################################################################
        glutPrintBox(10, -20, frmMain.cur_texture_name, 1.0, 1.0, 1.0, 1.0) ' view status
        'flip the buffers
        Gdi.SwapBuffers(pb2_hDC)
        drawing_ = False

    End Sub
    Public Sub draw_save()
        If Not _Started Then Return
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            Return
        End If
        'set ortho mode.
        '#######################################################################################
        Gl.glViewport(0, 0, frmMain.pb2.Width, frmMain.pb2.Height)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, frmMain.pb2.Width, -frmMain.pb2.Height, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        '#######################################################################################
        'set rendering modals

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        '#######################################################################################
        'clear buffer
        Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        '#######################################################################################
        'draw checkboard background
        '#######################################################################################
        'draw current_image
        If m_alpha_enabled.Checked Then
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Else
            Gl.glDisable(Gl.GL_BLEND)
        End If

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, current_image)


        'Gl.glEnable(Gl.GL_BLEND)

        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glColor4f(0.5, 0.5, 0.5, 0.0)
        Dim u4() As Single = {0.0, 1.0}
        Dim u3() As Single = {1.0, 1.0}
        Dim u2() As Single = {1.0, 0.0}
        Dim u1() As Single = {0.0, 0.0}

        Dim p1, p2, p3, p4 As Point
        Dim L, S As New Point
        L = rect_location
        S = rect_size
        L.Y *= -1
        S.Y *= -1

        p1 = L
        p2 = L
        p2.X += rect_size.X
        p3 = L + S
        p4 = L
        p4.Y += S.Y

        'draw and flip bufffers so the user can see the image
        If Not m_uvs_only.Checked Then

            Gl.glBegin(Gl.GL_QUADS)
            '---
            Gl.glTexCoord2fv(u1)
            Gl.glVertex2f(p1.X, p1.Y)
            Gl.glTexCoord2fv(u2)
            Gl.glVertex2f(p2.X, p2.Y)
            Gl.glTexCoord2fv(u3)
            Gl.glVertex2f(p3.X, p3.Y)
            Gl.glTexCoord2fv(u4)
            Gl.glVertex2f(p4.X, p4.Y)
            '--
            Gl.glEnd()
        End If
        '#######################################################################################
        'reset things
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glUseProgram(0)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        If m_show_uvs.Checked Or m_uvs_only.Checked Then
            If frmMain.m_show_fbx.Checked Then
                draw_fbx_uvs()
            Else
                draw_uvs()
            End If
        End If
        '#######################################################################################
        'no flip for saving buffer
        'Gdi.SwapBuffers(pb2_hDC)
    End Sub

    Public Sub get_fbx_triangle()
        If current_tank_part = 0 Then
            Return
        End If
        If Not _Started Then Return

        Gl.glClearColor(0.0!, 0.0!, 0.0!, 0.0!)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Dim r, b, g As Byte
        Gl.glPushMatrix()
        Gl.glTranslatef(rect_location.X + center.x, -rect_location.Y - center.y, 0.0F)
        Gl.glScalef(rect_size.X, rect_size.Y, 1.0F)

        Dim cnt = fbxgrp(current_tank_part).nPrimitives_

        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As UInt32 = 0 To cnt - 1
            Dim t = i + 1
            r = t And &HFF
            g = (t And &HFF00) >> 8
            b = (t And &HFF0000) >> 16
            Gl.glColor3ub(r, g, b)
            Dim p1 = fbxgrp(current_tank_part).indicies(i).v1
            Dim p2 = fbxgrp(current_tank_part).indicies(i).v2
            Dim p3 = fbxgrp(current_tank_part).indicies(i).v3
            Dim v1 = fbxgrp(current_tank_part).vertices(p1)
            Dim v2 = fbxgrp(current_tank_part).vertices(p2)
            Dim v3 = fbxgrp(current_tank_part).vertices(p3)
            Dim u1 As New uv_
            Dim u2 As New uv_
            Dim u3 As New uv_
            u1.u = v1.u
            u1.v = v1.v
            u2.u = v2.u
            u2.v = v2.v
            u3.u = v3.u
            u3.v = v3.v

            Gl.glVertex2f(u1.u, -u1.v)
            Gl.glVertex2f(u2.u, -u2.v)
            Gl.glVertex2f(u3.u, -u3.v)
        Next
        Gl.glEnd()
        Gl.glPopMatrix()
        'now figure out if the cursor is on a triangle
        Dim x, y As Integer
        x = frmMain.mouse_find_location.X
        y = frmMain.mouse_find_location.Y
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        frmMain.found_triangle_tv = pixel(0) + (pixel(1) * 256) + (pixel(2) * 65536)
        If frmMain.found_triangle_tv > 0 Then
            m_mouse.x = -1000
        End If
        'Gdi.SwapBuffers(pb2_hDC)
        'frmMain.Text = frmMain.found_triangle_tv.ToString
    End Sub
    Public Sub get_triangle()
        If current_tank_part = 0 Then
            Return
        End If
        If frmMain.m_show_fbx.Checked Then
            Return
        End If
        If Not _Started Then Return

        Gl.glClearColor(0.0!, 0.0!, 0.0!, 0.0!)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Dim r, b, g As Byte
        Dim cnt = _object(current_tank_part).count
        Gl.glPushMatrix()
        Gl.glTranslatef(rect_location.X + center.x, -rect_location.Y - center.y, 0.0F)
        Gl.glScalef(rect_size.X, rect_size.Y, 1.0F)

        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As UInt32 = 1 To cnt
            r = i And &HFF
            g = (i And &HFF00) >> 8
            b = (i And &HFF0000) >> 16
            Gl.glColor3ub(r, g, b)
            Dim u1 As New uv_
            Dim u2 As New uv_
            Dim u3 As New uv_
            u1.u = _object(current_tank_part).tris(i).uv1.u
            u1.v = _object(current_tank_part).tris(i).uv1.v
            u2.u = _object(current_tank_part).tris(i).uv2.u
            u2.v = _object(current_tank_part).tris(i).uv2.v
            u3.u = _object(current_tank_part).tris(i).uv3.u
            u3.v = _object(current_tank_part).tris(i).uv3.v

            Gl.glVertex2f(u1.u, -u1.v)
            Gl.glVertex2f(u2.u, -u2.v)
            Gl.glVertex2f(u3.u, -u3.v)
        Next
        Gl.glEnd()
        Gl.glPopMatrix()
        'now figure out if the cursor is on a triangle
        Dim x, y As Integer
        x = frmMain.mouse_find_location.X
        y = frmMain.mouse_find_location.Y
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        frmMain.found_triangle_tv = pixel(0) + (pixel(1) * 256) + (pixel(2) * 65536)
        If frmMain.found_triangle_tv > 0 Then
            m_mouse.x = -1000
        End If
        'Gdi.SwapBuffers(pb2_hDC)
        'frmMain.Text = frmMain.found_triangle_tv.ToString
    End Sub

    Private Sub draw_fbx_uvs()
        Dim cnt = fbxgrp(current_tank_part).nPrimitives_
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glColor3f(0.8, 0.8, 0.0)

        Gl.glPushMatrix()
        Gl.glTranslatef(rect_location.X + center.x, -rect_location.Y - center.y, 0.0F)
        Gl.glScalef(rect_size.X, rect_size.Y, 1.0F)

        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As UInt32 = 0 To cnt - 1
            Dim p1 = fbxgrp(current_tank_part).indicies(i).v1
            Dim p2 = fbxgrp(current_tank_part).indicies(i).v2
            Dim p3 = fbxgrp(current_tank_part).indicies(i).v3
            Dim v1 = fbxgrp(current_tank_part).vertices(p1)
            Dim v2 = fbxgrp(current_tank_part).vertices(p2)
            Dim v3 = fbxgrp(current_tank_part).vertices(p3)
            Dim u1 As New uv_
            Dim u2 As New uv_
            Dim u3 As New uv_
            u1.u = v1.u
            u1.v = v1.v
            u2.u = v2.u
            u2.v = v2.v
            u3.u = v3.u
            u3.v = v3.v


            Gl.glVertex2f(u1.u, -u1.v)
            Gl.glVertex2f(u2.u, -u2.v)
            Gl.glVertex2f(u3.u, -u3.v)
        Next
        Gl.glEnd()
        Gl.glPopMatrix()

        If current_part > 0 Then
            draw_current_fbx_vertex()
        End If
    End Sub
    Private Sub draw_uvs()
        Dim cnt = _object(current_tank_part).count
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glColor3f(0.8, 0.8, 0.0)

        Gl.glPushMatrix()
        Gl.glTranslatef(rect_location.X + center.x, -rect_location.Y - center.y, 0.0F)
        Gl.glScalef(rect_size.X, rect_size.Y, 1.0F)

        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As UInt32 = 1 To cnt
            Dim u1 As New uv_
            Dim u2 As New uv_
            Dim u3 As New uv_
            u1.u = _object(current_tank_part).tris(i).uv1.u
            u1.v = _object(current_tank_part).tris(i).uv1.v
            u2.u = _object(current_tank_part).tris(i).uv2.u
            u2.v = _object(current_tank_part).tris(i).uv2.v
            u3.u = _object(current_tank_part).tris(i).uv3.u
            u3.v = _object(current_tank_part).tris(i).uv3.v

            Gl.glVertex2f(u1.u, -u1.v)
            Gl.glVertex2f(u2.u, -u2.v)
            Gl.glVertex2f(u3.u, -u3.v)
        Next
        Gl.glEnd()
        Gl.glPopMatrix()

        If current_part > 0 Then
            draw_current_vertex()
        End If
    End Sub

    Public Sub draw_current_fbx_vertex()
        'draw()
        If current_vertex = 0 Then
            Return
        End If
        If current_vertex > _object(current_part).tris.Count Then
            Return
        End If
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glColor4f(0.8, 0.4, 0.0, 0.8)
        Dim u1 As New uv_
        Dim u2 As New uv_
        Dim u3 As New uv_
        Dim p1 = fbxgrp(current_tank_part).indicies(current_vertex - 1).v1
        Dim p2 = fbxgrp(current_tank_part).indicies(current_vertex - 1).v2
        Dim p3 = fbxgrp(current_tank_part).indicies(current_vertex - 1).v3
        Dim v1 = fbxgrp(current_tank_part).vertices(p1)
        Dim v2 = fbxgrp(current_tank_part).vertices(p2)
        Dim v3 = fbxgrp(current_tank_part).vertices(p3)
        u1.u = v1.u
        u1.v = v1.v
        u2.u = v2.u
        u2.v = v2.v
        u3.u = v3.u
        u3.v = v3.v


        u1.u = rect_location.X + (u1.u * rect_size.X) + center.x
        u2.u = rect_location.X + (u2.u * rect_size.X) + center.x
        u3.u = rect_location.X + (u3.u * rect_size.X) + center.x

        u1.v = -rect_location.Y + (-u1.v * rect_size.Y) - center.y
        u2.v = -rect_location.Y + (-u2.v * rect_size.Y) - center.y
        u3.v = -rect_location.Y + (-u3.v * rect_size.Y) - center.y

        Gl.glBegin(Gl.GL_TRIANGLES)
        Gl.glVertex2f(u1.u, u1.v)
        Gl.glVertex2f(u2.u, u2.v)
        Gl.glVertex2f(u3.u, u3.v)
        Gl.glEnd()
        Gl.glDisable(Gl.GL_BLEND)
    End Sub
    Public Sub draw_current_vertex()
        'draw()
        If current_vertex = 0 Then
            Return
        End If
        If current_vertex > _object(current_part).tris.Count Then
            Return
        End If
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glColor4f(0.8, 0.4, 0.0, 0.8)
        Dim u1 As New uv_
        Dim u2 As New uv_
        Dim u3 As New uv_
        u1.u = _object(current_part).tris(current_vertex).uv1.u
        u1.v = _object(current_part).tris(current_vertex).uv1.v
        u2.u = _object(current_part).tris(current_vertex).uv2.u
        u2.v = _object(current_part).tris(current_vertex).uv2.v
        u3.u = _object(current_part).tris(current_vertex).uv3.u
        u3.v = _object(current_part).tris(current_vertex).uv3.v

        u1.u = rect_location.X + (u1.u * rect_size.X) + center.x
        u2.u = rect_location.X + (u2.u * rect_size.X) + center.x
        u3.u = rect_location.X + (u3.u * rect_size.X) + center.x

        u1.v = -rect_location.Y + (-u1.v * rect_size.Y) - center.y
        u2.v = -rect_location.Y + (-u2.v * rect_size.Y) - center.y
        u3.v = -rect_location.Y + (-u3.v * rect_size.Y) - center.y

        Gl.glBegin(Gl.GL_TRIANGLES)
        Gl.glVertex2f(u1.u, u1.v)
        Gl.glVertex2f(u2.u, u2.v)
        Gl.glVertex2f(u3.u, u3.v)
        Gl.glEnd()
        Gl.glDisable(Gl.GL_BLEND)
    End Sub

    Private Sub frmTextureViewer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        frmMain.pb2.Parent = frmMain
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmTextureViewer_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.C Then
            If CENTER_SELECTION Then
                CENTER_SELECTION = False
            Else
                CENTER_SELECTION = True
            End If
            frmMain.pb1.Focus()
        End If

    End Sub

    Private Sub frmTextureViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.KeyPreview = True    'so I catch keyboard before despatching it

    End Sub

    Private Sub frmTextureViewer_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        draw()
        Application.DoEvents()
        drawing_ = False
    End Sub

    Private Sub frmTextureViewer_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        frmMain.found_triangle_tv = 0
        current_vertex = 0
    End Sub



    Private Sub frmTextureViewer_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        draw()
        drawing_ = False
    End Sub

    Private Sub frmTextureViewer_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
    End Sub


    Private Sub m_show_uvs_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_uvs.CheckedChanged
        If m_show_uvs.Checked Then
            m_show_uvs.ForeColor = Color.Red
        Else
            m_show_uvs.ForeColor = Color.Black
        End If
        draw()
    End Sub

    Private Sub m_alpha_enabled_CheckedChanged(sender As Object, e As EventArgs) Handles m_alpha_enabled.CheckedChanged
        If m_alpha_enabled.Checked Then
            m_alpha_enabled.ForeColor = Color.Red
        Else
            m_alpha_enabled.ForeColor = Color.Black
        End If
        draw()
    End Sub

    Private Sub m_top_most_CheckedChanged(sender As Object, e As EventArgs) Handles m_top_most.CheckedChanged
        If m_top_most.Checked Then
            m_top_most.ForeColor = Color.Red
            Me.TopMost = True
        Else
            Me.TopMost = False
            m_top_most.ForeColor = Color.Black
        End If
        draw()
    End Sub

    Private Sub m_save_image_Click(sender As Object, e As EventArgs) Handles m_save_image.Click
        ToolStrip1.Enabled = False
        frmMain.update_thread.Suspend()
        If Not SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            ToolStrip1.Enabled = True
            frmMain.update_thread.Resume()

            Return
        End If
        Dim path = SaveFileDialog1.FileName
        Dim old_rect_size = rect_size
        Dim old_location = rect_location
        rect_location = New Point(0, 0)
        rect_size = New Point(old_w, old_h)
        frmMain.pb2.Dock = DockStyle.None
        frmMain.pb2.Width = old_w
        frmMain.pb2.Height = old_h

        draw_save()


        Dim t_tex As Integer
        t_tex = buffer_to_IL_ID()
        Il.ilBindImage(t_tex)
        Il.ilEnable(Il.IL_FILE_OVERWRITE)
        Dim status = Il.ilSave(Il.IL_PNG, path)
        Il.ilDeleteImage(t_tex)
        Il.ilBindImage(0)
        rect_location = old_location
        rect_size = old_rect_size
        frmMain.pb2.Dock = DockStyle.Fill
        draw()
        draw()
        ToolStrip1.Enabled = True
        frmMain.update_thread.Resume()

    End Sub
    Public Function buffer_to_IL_ID() As Integer


        Application.DoEvents()
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glFinish()
        Dim Id As Integer = Il.ilGenImage
        Il.ilBindImage(Id)
        Il.ilTexImage(old_w, old_h, 0, 4, Il.IL_RGBA, Il.IL_UNSIGNED_BYTE, Nothing)

        Gl.glReadPixels(0, 0, old_w, old_h, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())

        Gl.glFinish()
        Application.DoEvents()

        Il.ilBindImage(0)
        Return Id
    End Function

    Private Sub m_uvs_only_CheckedChanged(sender As Object, e As EventArgs) Handles m_uvs_only.CheckedChanged
        If m_uvs_only.Checked Then
            m_uvs_only.ForeColor = Color.Red
        Else
            m_uvs_only.ForeColor = Color.Black
        End If
        draw()
    End Sub

    Private Sub m_show_uvs_Click(sender As Object, e As EventArgs) Handles m_show_uvs.Click
        If m_show_uvs.Checked Then
            m_uvs_only.Checked = False
        End If
    End Sub

    Private Sub m_uvs_only_Click(sender As Object, e As EventArgs) Handles m_uvs_only.Click
        If m_uvs_only.Checked Then
            m_show_uvs.Checked = False
        End If

    End Sub
End Class