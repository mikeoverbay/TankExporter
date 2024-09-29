Imports Microsoft.VisualBasic.Devices
Imports System.Windows
Imports System.Math
Public Class frmFullScreen
    Private Sub frmFullScreen_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyValue
            Case Keys.Escape
                If WRITE_FBX_NOW Then
                    STOP_FBX_SAVE = True
                    Exit Select
                End If
            Case Keys.Space
                If paused Then
                    paused = False
                Else
                    paused = True
                End If

            Case Keys.F2
                If frmMain.Show_lights Then
                    frmMain.Show_lights = False
                Else
                    frmMain.Show_lights = True
                End If

            Case Keys.F3
                If frmMain.spin_light Then
                    frmMain.spin_light = False
                Else
                    frmMain.spin_light = True
                End If

            Case Keys.F4
                If frmMain.spin_camera Then
                    frmMain.spin_camera = False
                Else
                    frmMain.spin_camera = True
                End If

            Case Keys.F5
                If frmMain.chassis_cb.Checked Then
                    frmMain.chassis_cb.Checked = False
                Else
                    frmMain.chassis_cb.Checked = True
                End If

            Case Keys.F6
                If frmMain.hull_cb.Checked Then
                    frmMain.hull_cb.Checked = False
                Else
                    frmMain.hull_cb.Checked = True
                End If

            Case Keys.F7
                If frmMain.turret_cb.Checked Then
                    frmMain.turret_cb.Checked = False
                Else
                    frmMain.turret_cb.Checked = True
                End If

            Case Keys.F8
                If frmMain.gun_cb.Checked Then
                    frmMain.gun_cb.Checked = False
                Else
                    frmMain.gun_cb.Checked = True
                End If
            Case Keys.F9
                frmMain.m_edit_shaders.PerformClick()

            Case Keys.F10
                frmMain.m_shadows.PerformClick()

            Case Keys.F11
                FULL_SCREEN = FULL_SCREEN Xor True
                If Not FULL_SCREEN Then
                    Me.SendToBack()
                    Me.WindowState = FormWindowState.Normal
                    Me.Visible = False
                    G_Buffer.init()


                Else
                    Me.Visible = True
                    Me.BringToFront()
                    Me.WindowState = FormWindowState.Maximized
                    G_Buffer.init()
                End If
            Case Keys.F12
                If MODEL_LOADED Then
                    frmMain.m_screen_cap.PerformClick()
                End If
                '=================
            Case Keys.L
                frmMain.m_lighting.PerformClick()
        End Select

    End Sub

    Private Sub frmFullScreen_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp

    End Sub
    Dim mouse As vec2
    Private Sub frmFullScreen_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.KeyPreview = True    'so i catch keyboard before despatching it

    End Sub
    Private Sub fs_render_box_MouseMove(sender As Object, e As MouseEventArgs) Handles fs_render_box.MouseMove
        m_mouse.x = e.X
        m_mouse.y = e.Y
        If M_DOWN And upton.state > 0 And upton.state < 5 Then
            frmMain.scale_decal_xyz()
            Return
        End If
        If M_DOWN And upton.state > 4 And upton.state < 8 Then
            frmMain.move_xyz()
            Return
        End If
        If M_DOWN And upton.state > 7 And upton.state < 11 Then
            frmMain.rotate_decal_xyz()
            Return
        End If
        If upton.state = 102 And M_DOWN Then
            Dim delta As New Point
            delta.X = mouse.x - m_mouse.x
            delta.Y = mouse.y - m_mouse.y
            upton.position.X -= delta.X
            upton.position.Y += delta.Y
            mouse.x = m_mouse.x
            mouse.y = m_mouse.y
            Return
        End If

        If BUTTON_ID > 0 Then
            Return
        End If
        If pan_left Or pan_right Then
            Return
        End If
        'If check_menu_select() Then ' check if we are over a button
        '    Return
        'End If
        Dim dead As Integer = 5
        Dim t As Single
        Dim M_Speed As Single = mouse_speed_global
        Dim ms As Single = 0.2F * view_radius ' distance away changes speed.. THIS WORKS WELL!
        If M_DOWN Then
            If e.X > (mouse.x + dead) Then
                If e.X - mouse.x > 100 Then t = (1.0F * M_Speed)
            Else : t = CSng(Sin((e.X - mouse.x) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        look_point_x -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z -= ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle -= t
                    End If
                    If Cam_X_angle > (2 * PI) Then Cam_X_angle -= (2 * PI)
                    mouse.x = e.X
                End If
            End If
            If e.X < (mouse.x - dead) Then
                If mouse.x - e.X > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.x - e.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        look_point_x += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z += ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle += t
                    End If
                    If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                    mouse.x = e.X
                End If
            End If
            ' ------- Y moves ----------------------------------
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * M_Speed
                If z_move Then
                    look_point_y -= (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        look_point_z -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x -= ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle -= t
                    End If
                    If Cam_Y_angle < -PI / 2.0 Then Cam_Y_angle = -PI / 2.0 + 0.001
                End If
                mouse.y = e.Y
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * M_Speed
                If z_move Then
                    look_point_y += (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        look_point_z += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x += ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle += t
                    End If
                    If Cam_Y_angle > 1.3 Then Cam_Y_angle = 1.3
                End If
                mouse.y = e.Y
            End If
            'draw_scene()
            'Debug.WriteLine(Cam_X_angle.ToString("0.000") + " " + Cam_Y_angle.ToString("0.000"))
            Return
        End If
        If move_cam_z Then
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (10)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * 12 * mouse_speed_global
                view_radius += (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius < -80.0 Then
                    view_radius = -80.0
                End If
                mouse.y = e.Y
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * 12 * mouse_speed_global
                view_radius -= (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius > -0.01 Then view_radius = -0.01
                mouse.y = e.Y
            End If
            If view_radius > -0.1 Then view_radius = -0.1
            'draw_scene()
            Return
        End If
        mouse.x = e.X
        mouse.y = e.Y
        'GetOGLPos(e.X, e.Y)
        'draw_scene()
    End Sub
    Private Sub fs_render_box_MouseDown(sender As Object, e As MouseEventArgs) Handles fs_render_box.MouseDown
        'If M_SELECT_COLOR > 0 Then
        '    For i = 0 To button_list.Length - 2
        '        If M_SELECT_COLOR = button_list(i).color Then
        '            CallByName(Menu_Subs, button_list(i).function_, Microsoft.VisualBasic.CallType.Method)
        '        End If
        '    Next
        'End If
        mouse.x = e.X
        mouse.y = e.Y
        If frmMain.mouse_pick_cb.Checked And e.Button = MouseButtons.Middle Then
            If picked_decal > -1 Then
                current_decal_data_pnt = picked_decal
                picked_decal = -1

                frmMain.dgv.Rows(frmMain.dgv.SelectedRows(0).Index).Selected = False
                frmMain.dgv.Rows(current_decal_data_pnt).Selected = True


                ' Get the index of the selected row
                Dim selectedRowIndex As Integer = frmMain.dgv.SelectedRows(0).Index
                ' Set the FirstDisplayedScrollingRowIndex to the selected row index
                frmMain.dgv.FirstDisplayedScrollingRowIndex = selectedRowIndex
                'decal_matrix_list(current_decal_data_pnt).GetDecalsTransformInfo()
                frmMain.mouse_pick_cb.Checked = False

                cur_selected_decal_texture = decal_matrix_list(current_decal_data_pnt).DecalIndex
                frmPickDecal.set_selecion(decal_matrix_list(current_decal_data_pnt).DecalIndex)
                frmMain.set_g_decal_current()

                Return
            End If
        End If
        If e.Button = Forms.MouseButtons.Right Then
            move_cam_z = True
        End If
        If e.Button = Forms.MouseButtons.Middle Then
            move_mod = True
            M_DOWN = True
        End If
        If e.Button = Forms.MouseButtons.Left Then

            CAMO_BUTTON_DOWN = True
            M_DOWN = True
        End If
    End Sub
    Private Sub fs_render_box_MouseUp(sender As Object, e As MouseEventArgs) Handles fs_render_box.MouseUp
        M_DOWN = False
        CAMO_BUTTON_DOWN = False
        move_cam_z = False
        move_mod = False
    End Sub


End Class