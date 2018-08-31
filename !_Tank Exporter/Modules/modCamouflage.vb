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


Module modCamouflage

    Public bb_texture_list(0) As String
    Public bb_tank_tiling(0) As vect4
    Public bb_texture_ids(0) As Integer
    Public bb_camo_texture_ids(0) As Integer
    Public bb_processed_texture_ids(0) As Integer
    Public c0() As vect4
    Public c1() As vect4
    Public c2() As vect4
    Public c3() As vect4

    Public Sub apply_texture(ByVal id As Integer)
        SELECTED_CAMO_BUTTON = id
        If frmEditCamo.Visible Then
            frmEditCamo.camo_id = id
            frmEditCamo.camo_change()
            frmEditCamo.set_selected()
        End If
        'current_camo_selection = id
        For i = 1 To object_count
            If Not _object(i).name.ToLower.Contains("chassis") Then
                _object(i).exclude_camo = 0
                _object(i).use_camo = 1
            Else
                _object(i).exclude_camo = 1
                _object(i).use_camo = 0
            End If
        Next
        'save_camo_texture(id, Temp_Storage)
        STOP_BUTTON_SCAN = False
    End Sub

    Public Sub show_winter_buttons()
        load_camo_buttons("winter")
        STOP_BUTTON_SCAN = False
    End Sub

    Public Sub show_summer_buttons()
        load_camo_buttons("summer")
        STOP_BUTTON_SCAN = False
    End Sub

    Public Sub show_dessert_buttons()
        load_camo_buttons("desert")
        STOP_BUTTON_SCAN = False
    End Sub

    Public Sub load_camo_buttons(ByVal type As String)
        For i = 1 To object_count
            If Not _object(i).name.ToLower.Contains("chassis") Then
                _object(i).use_camo = 0
            End If
        Next
        SELECTED_CAMO_BUTTON = 0
        OLD_WINDOW_HEIGHT = frmMain.pb1.Height
        WINDOW_HEIGHT_DELTA = 0
        LAST_SEASON = BUTTON_ID
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("chassis") Then
                _object(i).exclude_camo = 1
            End If
        Next
        '===================================
        Dim d = custom_tables(CURRENT_DATA_SET).Copy
        '===================================

        Dim t = d.Tables("camouflage")
        Dim qq = From row In t.AsEnumerable
        Select _
        armorC = row.Field(Of String)("armorcolor")
        ARMORCOLOR = get_vect4(qq(0))
        Dim ar = TANK_NAME.Split(":")

        Dim t_name = Path.GetFileNameWithoutExtension(ar(0))
        '===================================
        t = d.Tables("colors")
        Dim q = From row In t.AsEnumerable _
                Where type = row.Field(Of String)("kind") _
                Select _
                texture = row.Field(Of String)("texture"), _
                tank_tiling = row.Field(Of String)(t_name), _
                c0 = row.Field(Of String)("c0"), _
                c1 = row.Field(Of String)("c1"), _
                c2 = row.Field(Of String)("c2"), _
                c3 = row.Field(Of String)("c3")

        Try
            ReDim bb_texture_list(q.Count)

        Catch ex As Exception
            t.Dispose()
            MsgBox("This tank can not have camouflage appied to it!", MsgBoxStyle.Information, "Not Going to happen...")
            CAMO_BUTTONS_VISIBLE = False
            season_Buttons_VISIBLE = False
            M_DOWN = False
            BUTTON_ID = 0
            Return
        End Try

        ReDim bb_tank_tiling(q.Count)
        ReDim bb_texture_ids(q.Count)
        ReDim c0(q.Count)
        ReDim c1(q.Count)
        ReDim c2(q.Count)
        ReDim c3(q.Count)

        Dim cnt As Integer = 0
        For Each l In q
            If l.texture.Contains("IGR") Or l.texture.Contains("Clan") Or l.texture.ToLower.Contains("victim") Then
                GoTo skip
            End If
            Try
                bb_texture_list(cnt) = l.texture
                bb_tank_tiling(cnt) = New vect4
                bb_tank_tiling(cnt) = get_vect4_no_conversion(l.tank_tiling)

            Catch ex As Exception
                Dim v_t As vect4
                v_t.x = 1.0
                v_t.y = 1.0
                bb_tank_tiling(cnt) = v_t
            End Try
            c0(cnt) = New vect4
            c1(cnt) = New vect4
            c2(cnt) = New vect4
            c3(cnt) = New vect4

            c0(cnt) = get_vect4(l.c0)
            c1(cnt) = get_vect4(l.c1)
            c2(cnt) = get_vect4(l.c2)
            c3(cnt) = get_vect4(l.c3)


            Dim ms As New MemoryStream
            Dim ent = frmMain.packages(11)(l.texture)
            If ent IsNot Nothing Then
                ent.Extract(ms)
                bb_texture_ids(cnt) = get_texture_from_stream(ms)
            End If
            cnt += 1
skip:
        Next
        '===================================
        ReDim Preserve bb_texture_list(cnt)
        ReDim Preserve bb_tank_tiling(cnt)
        ReDim Preserve bb_texture_ids(cnt)
        ReDim Preserve c0(cnt)
        ReDim Preserve c1(cnt)
        ReDim Preserve c2(cnt)
        ReDim Preserve c3(cnt)
        ReDim Preserve bb_camo_texture_ids(cnt)
        '===================================
        If cnt = 0 Then Return ' nothing found
        ReDim camo_Buttons(0)
        If frmTextureViewer.Visible Then
            frmMain.pb2.Dock = DockStyle.None
        End If
        For i = 0 To cnt - 1
            Dim b = New Camobutton_
            b.c0 = c0(i)
            b.c1 = c1(i)
            b.c2 = c2(i)
            b.c3 = c3(i)
            'The x in location doesn't really matter..
            'Its recaculated by the relocate function.
            b.location = New Point(100, -frmMain.pb1.Height + 110)
            b.size = New Point(100, 100)
            b.gl_textureID = make_mixed_texture(i)
            bb_camo_texture_ids(i) = b.gl_textureID

            b.camo_texture_id = bb_texture_ids(i)
            b.callmode = type
            b.add()
        Next
        '===================================
        relocate_camobuttons()
        CAMO_BUTTONS_VISIBLE = True
        If frmTextureViewer.Visible Then
            frmMain.pb2.Visible = True
            frmMain.pb2.Dock = DockStyle.Fill
            'frmTextureViewer.draw()
            'frmTextureViewer.draw()
            'frmTextureViewer.draw()
        End If
        If frmTextureViewer.Visible Then
            frmTextureViewer.draw()
            frmTextureViewer.draw()
        End If
        '===================================
        'Debug.WriteLine("test")
    End Sub

    Public Function get_vect4(ByVal s As String) As vect4
        Dim a = s.Split(" ")
        Dim v As New vect4
        v.x = CSng(a(0)) / 255.0!
        v.y = CSng(a(1)) / 255.0!
        v.z = CSng(a(2)) / 255.0!
        v.w = CSng(a(3)) / 255.0!
        Return v
    End Function

    Private Function get_vect4_no_conversion(ByVal s As String) As vect4
        Dim a = s.Split(" ")
        Dim v As New vect4
        v.x = CSng(a(0))
        v.y = CSng(a(1))
        v.z = CSng(a(2))
        v.w = CSng(a(3))
        Return v
    End Function

    Public Sub relocate_season_Bottons()
        Dim cnt = season_Buttons.Length - 1
        Dim butt_width = season_Buttons(0).size.X
        Dim space As Integer = 10
        Dim sw = frmMain.pb1.Width
        Dim rw = (butt_width * cnt) + (space * (cnt - 1))
        Dim stepsize = butt_width + space
        Dim ss = (sw / 2.0) - (rw / 2.0)
        For i = 0 To cnt - 1
            season_Buttons(i).location.X = ss
            season_Buttons(i).location.Y -= WINDOW_HEIGHT_DELTA
            ss += stepsize
        Next
    End Sub

    Public Sub relocate_camobuttons()
        Dim cnt = camo_Buttons.Length - 1
        Dim butt_width = camo_Buttons(0).size.X
        Dim space As Integer = 10
        Dim sw = frmMain.pb1.Width
        Dim rw = (butt_width * cnt) + (space * (cnt - 1))
        Dim stepsize = butt_width + space
        Dim ss = (sw / 2.0) - (rw / 2.0)
        For i = 0 To cnt - 1
            camo_Buttons(i).location.X = ss
            camo_Buttons(i).location.Y -= WINDOW_HEIGHT_DELTA
            ss += stepsize
        Next
    End Sub

    Public Function is_camo_active() As Boolean
        For i = 1 To object_count
            If _object(i).use_camo = 1 Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Sub setup_camo_selection()
        If season_Buttons_VISIBLE Then
            CAMO_BUTTONS_VISIBLE = False
            season_Buttons_VISIBLE = False
            Return
        Else
            season_Buttons_VISIBLE = True
            OLD_WINDOW_HEIGHT = frmMain.pb1.Height
            WINDOW_HEIGHT_DELTA = 0
            LAST_SEASON = -1
        End If
        ReDim season_Buttons(0)
        For i = 0 To 2
            Dim b As New Nbutton_
            b.state = 0
            b.size = New Point(47, 47)
            b.location = New Point(100, -frmMain.pb1.Height + 167)
            b.callmode = i
            b.add()
        Next
        season_Buttons(0).gl_textureID = WINTER_ICON
        season_Buttons(1).gl_textureID = SUMMER_ICON
        season_Buttons(2).gl_textureID = DESSERT_ICON
        relocate_season_Bottons()
        season_Buttons_VISIBLE = True
    End Sub

    Public Sub save_camo_texture(ByVal id As Integer, ByVal save_path As String)
        frmMain.pb2.Visible = False
        frmMain.pb2.BringToFront()
        'frmMain.gl_stop = True
        frmMain.update_thread.Suspend()
        'While gl_busy
        '    Application.DoEvents()
        'End While
        Dim w, h As Integer
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, bb_camo_texture_ids(id))
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, w)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, h)
        Dim p As New Point(0.0!, 0.0!)
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        frmMain.pb2.Width = w
        frmMain.pb2.Height = h
        Gl.glViewport(0, 0, w, h)
        Gl.glViewport(0, 0, w, h)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, w, -h, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Dim e = Gl.glGetError
        Gl.glUseProgram(shader_list.mixer_shader)
        Gl.glUniform1i(mix_camoMap, 0)
        Gl.glUniform4f(mix_armorColor, ARMORCOLOR.x, ARMORCOLOR.y, ARMORCOLOR.z, ARMORCOLOR.w)
        Gl.glUniform4f(mix_c0, c0(id).x, c0(id).y, c0(id).z, c0(id).w)
        Gl.glUniform4f(mix_c1, c1(id).x, c1(id).y, c1(id).z, c1(id).w)
        Gl.glUniform4f(mix_c2, c2(id).x, c2(id).y, c2(id).z, c2(id).w)
        Gl.glUniform4f(mix_c3, c3(id).x, c3(id).y, c3(id).z, c3(id).w)

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, bb_texture_ids(id))
        Gl.glBegin(Gl.GL_QUADS)

        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()
        Gl.glUseProgram(0)

        Gl.glFinish()
        Dim tId As Integer = Il.ilGenImage
        Il.ilBindImage(tId)
        Il.ilTexImage(w, h, 0, 4, Il.IL_RGBA, Il.IL_UNSIGNED_BYTE, Nothing)

        Gl.glReadPixels(0, 0, w, h, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())

        Gl.glFinish()
        Il.ilSave(Il.IL_PNG, save_path + "\camouflage.png") ' save to temp
        Gl.glDisable(Gl.GL_TEXTURE_2D)

        Il.ilBindImage(0)
        Il.ilDeleteImage(tId)
        Application.DoEvents()
        frmMain.update_thread.Resume()

    End Sub
    Public Function make_mixed_texture(id As Integer) As Integer
        frmMain.pb2.Visible = False
        frmMain.pb2.BringToFront()
        'frmMain.gl_stop = True
        frmMain.update_thread.Suspend()
        'While gl_busy
        '    Application.DoEvents()
        'End While
        Dim w, h As Integer
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, bb_texture_ids(id))
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, w)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, h)
        Dim p As New Point(0.0!, 0.0!)
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        frmMain.pb2.Width = w
        frmMain.pb2.Height = h
        Gl.glViewport(0, 0, w, h)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, w, -h, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Dim e = Gl.glGetError
        Gl.glUseProgram(shader_list.mixer_shader)
        Gl.glUniform1i(mix_camoMap, 0)
        Gl.glUniform4f(mix_armorColor, ARMORCOLOR.x, ARMORCOLOR.y, ARMORCOLOR.z, ARMORCOLOR.w)
        Gl.glUniform4f(mix_c0, c0(id).x, c0(id).y, c0(id).z, c0(id).w)
        Gl.glUniform4f(mix_c1, c1(id).x, c1(id).y, c1(id).z, c1(id).w)
        Gl.glUniform4f(mix_c2, c2(id).x, c2(id).y, c2(id).z, c2(id).w)
        Gl.glUniform4f(mix_c3, c3(id).x, c3(id).y, c3(id).z, c3(id).w)

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, bb_texture_ids(id))

        Gl.glBegin(Gl.GL_QUADS)

        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()
        If False Then
            Gdi.SwapBuffers(pb2_hDC)
            '==================================================
            'draw 2 x for debuf

            Gl.glBegin(Gl.GL_QUADS)

            '  CW...
            '  1 ------ 2
            '  |        |
            '  |        |
            '  4 ------ 3
            '
            Gl.glTexCoord2f(0.0!, 0.0!)
            Gl.glVertex2f(p.X, p.Y)

            Gl.glTexCoord2f(1.0!, 0.0!)
            Gl.glVertex2f(p.X + w, p.Y)

            Gl.glTexCoord2f(1.0!, 1.0!)
            Gl.glVertex2f(p.X + w, p.Y - h)

            Gl.glTexCoord2f(0.0!, 1.0!)
            Gl.glVertex2f(p.X, p.Y - h)
            Gl.glEnd()
            Gdi.SwapBuffers(pb2_hDC)

        End If
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Dim image_id As Integer
        Gl.glGenTextures(1, image_id)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
        Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Nothing)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_FALSE)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST)
        Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
        Gl.glTexParameterf(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)

        'Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, 4, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, 0) '  Texture specification 
        Gl.glCopyTexSubImage2D(Gl.GL_TEXTURE_2D, 0, 0, 0, 0, 0, w, h)

        Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 0, 0, w, h, 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        e = Gl.glGetError

        frmMain.gl_stop = False
        frmMain.update_thread.Resume()

        Return image_id
    End Function


End Module
