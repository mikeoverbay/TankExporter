Public Class frmEditCamo

    Private selected As Integer = 1
    Public camo_id As Integer
    Public Sub set_colors_entry(ByRef c1 As Label, ByRef c2 As Label, ByRef c3 As Label, ByRef c4 As Label)
        Dim r, g, b, a As New Color
        Dim h1 = HScrollBar1.Value
        Dim h2 = HScrollBar2.Value
        Dim h3 = HScrollBar3.Value
        Dim h4 = HScrollBar4.Value
        r_t.Text = h1.ToString("000")
        g_t.Text = h2.ToString("000")
        b_t.Text = h3.ToString("000")
        a_t.Text = h4.ToString("000")

        c1.BackColor = Color.FromArgb(255, h1, 0, 0)
        c2.BackColor = Color.FromArgb(255, 0, h2, 0)
        c3.BackColor = Color.FromArgb(255, 0, 0, h3)
        c4.BackColor = Color.FromArgb(255, h4, h4, h4)
        Application.DoEvents()

    End Sub
    Private Sub set_colors(ByRef c1 As Label, ByRef c2 As Label, ByRef c3 As Label, ByRef c4 As Label)
        Dim r, g, b, a As New Color
        Dim h1 = HScrollBar1.Value
        Dim h2 = HScrollBar2.Value
        Dim h3 = HScrollBar3.Value
        Dim h4 = HScrollBar4.Value
        r_t.Text = h1.ToString("000")
        g_t.Text = h2.ToString("000")
        b_t.Text = h3.ToString("000")
        a_t.Text = h4.ToString("000")

        c1.BackColor = Color.FromArgb(255, h1, 0, 0)
        c2.BackColor = Color.FromArgb(255, 0, h2, 0)
        c3.BackColor = Color.FromArgb(255, 0, 0, h3)
        c4.BackColor = Color.FromArgb(255, h4, h4, h4)

        Application.DoEvents()
    End Sub
    Private Sub save_settings()

        If is_camo_active() Then
            Gl.glDeleteTextures(1, camo_Buttons(camo_id).gl_textureID)
            camo_Buttons(camo_id).gl_textureID = make_mixed_texture(camo_id)
            bb_camo_texture_ids(camo_id) = camo_Buttons(camo_id).gl_textureID
        End If

    End Sub

    Public Sub handle_color_changes()

        If RadioButton1.Checked Then
            set_colors(a_r, a_g, a_b, a_a)
            ARMORCOLOR.x = CSng(HScrollBar1.Value / 255)
            ARMORCOLOR.y = CSng(HScrollBar2.Value / 255)
            ARMORCOLOR.z = CSng(HScrollBar3.Value / 255)
            ARMORCOLOR.w = CSng(HScrollBar4.Value / 255)
            save_settings()
        End If

        If RadioButton2.Checked Then
            set_colors(c1_r, c1_g, c1_b, c1_a)
            c0(camo_id).x = CSng(HScrollBar1.Value / 255)
            c0(camo_id).y = CSng(HScrollBar2.Value / 255)
            c0(camo_id).z = CSng(HScrollBar3.Value / 255)
            c0(camo_id).w = CSng(HScrollBar4.Value / 255)
            save_settings()
        End If

        If RadioButton3.Checked Then
            set_colors(c2_r, c2_g, c2_b, c2_a)
            c1(camo_id).x = CSng(HScrollBar1.Value / 255)
            c1(camo_id).y = CSng(HScrollBar2.Value / 255)
            c1(camo_id).z = CSng(HScrollBar3.Value / 255)
            c1(camo_id).w = CSng(HScrollBar4.Value / 255)
            save_settings()
        End If

        If RadioButton4.Checked Then
            set_colors(c3_r, c3_g, c3_b, c3_a)
            c2(camo_id).x = CSng(HScrollBar1.Value / 255)
            c2(camo_id).y = CSng(HScrollBar2.Value / 255)
            c2(camo_id).z = CSng(HScrollBar3.Value / 255)
            c2(camo_id).w = CSng(HScrollBar4.Value / 255)
            save_settings()
        End If

        If RadioButton5.Checked Then
            set_colors(c4_r, c4_g, c4_b, c4_a)
            c3(camo_id).x = CSng(HScrollBar1.Value / 255)
            c3(camo_id).y = CSng(HScrollBar2.Value / 255)
            c3(camo_id).z = CSng(HScrollBar3.Value / 255)
            c3(camo_id).w = CSng(HScrollBar4.Value / 255)
            save_settings()
        End If

    End Sub

    Public Sub set_selected()
        If RadioButton1.Checked Then
            selected = 1
            HScrollBar1.Value = CInt(ARMORCOLOR.x * 255.0!)
            HScrollBar2.Value = CInt(ARMORCOLOR.y * 255.0!)
            HScrollBar3.Value = CInt(ARMORCOLOR.z * 255.0!)
            HScrollBar4.Value = CInt(ARMORCOLOR.w * 255.0!)
        End If
        If RadioButton2.Checked Then
            selected = 2
            HScrollBar1.Value = CInt(c0(camo_id).x * 255.0!)
            HScrollBar2.Value = CInt(c0(camo_id).y * 255.0!)
            HScrollBar3.Value = CInt(c0(camo_id).z * 255.0!)
            HScrollBar4.Value = CInt(c0(camo_id).w * 255.0!)
        End If
        If RadioButton3.Checked Then
            selected = 3
            HScrollBar1.Value = CInt(c1(camo_id).x * 255.0!)
            HScrollBar2.Value = CInt(c1(camo_id).y * 255.0!)
            HScrollBar3.Value = CInt(c1(camo_id).z * 255.0!)
            HScrollBar4.Value = CInt(c1(camo_id).w * 255.0!)
        End If
        If RadioButton4.Checked Then
            selected = 4
            HScrollBar1.Value = CInt(c2(camo_id).x * 255.0!)
            HScrollBar2.Value = CInt(c2(camo_id).y * 255.0!)
            HScrollBar3.Value = CInt(c2(camo_id).z * 255.0!)
            HScrollBar4.Value = CInt(c2(camo_id).w * 255.0!)
        End If
        If RadioButton5.Checked Then
            selected = 5
            HScrollBar1.Value = CInt(c3(camo_id).x * 255.0!)
            HScrollBar2.Value = CInt(c3(camo_id).y * 255.0!)
            HScrollBar3.Value = CInt(c3(camo_id).z * 255.0!)
            HScrollBar4.Value = CInt(c3(camo_id).w * 255.0!)
        End If
        handle_color_changes()
    End Sub

    Public Sub camo_change()
        HScrollBar1.Value = CInt(ARMORCOLOR.x * 255.0!)
        HScrollBar2.Value = CInt(ARMORCOLOR.y * 255.0!)
        HScrollBar3.Value = CInt(ARMORCOLOR.z * 255.0!)
        HScrollBar4.Value = CInt(ARMORCOLOR.w * 255.0!)
        set_colors_entry(a_r, a_g, a_b, a_a)

        HScrollBar1.Value = CInt(c0(camo_id).x * 255.0!)
        HScrollBar2.Value = CInt(c0(camo_id).y * 255.0!)
        HScrollBar3.Value = CInt(c0(camo_id).z * 255.0!)
        HScrollBar4.Value = CInt(c0(camo_id).w * 255.0!)
        set_colors_entry(c1_r, c1_g, c1_b, c1_a)

        HScrollBar1.Value = CInt(c1(camo_id).x * 255.0!)
        HScrollBar2.Value = CInt(c1(camo_id).y * 255.0!)
        HScrollBar3.Value = CInt(c1(camo_id).z * 255.0!)
        HScrollBar4.Value = CInt(c1(camo_id).w * 255.0!)
        set_colors_entry(c2_r, c2_g, c2_b, c2_a)

        HScrollBar1.Value = CInt(c2(camo_id).x * 255.0!)
        HScrollBar2.Value = CInt(c2(camo_id).y * 255.0!)
        HScrollBar3.Value = CInt(c2(camo_id).z * 255.0!)
        HScrollBar4.Value = CInt(c2(camo_id).w * 255.0!)
        set_colors_entry(c3_r, c3_g, c3_b, c3_a)

        HScrollBar1.Value = CInt(c3(camo_id).x * 255.0!)
        HScrollBar2.Value = CInt(c3(camo_id).y * 255.0!)
        HScrollBar3.Value = CInt(c3(camo_id).z * 255.0!)
        HScrollBar4.Value = CInt(c3(camo_id).w * 255.0!)
        set_colors_entry(c4_r, c4_g, c4_b, c4_a)



    End Sub

    Private Sub HScrollBar1_Scroll(sender As Object, e As ScrollEventArgs) Handles HScrollBar1.Scroll
        handle_color_changes()
    End Sub

    Private Sub HScrollBar2_Scroll(sender As Object, e As ScrollEventArgs) Handles HScrollBar2.Scroll
        handle_color_changes()
    End Sub

    Private Sub HScrollBar3_Scroll(sender As Object, e As ScrollEventArgs) Handles HScrollBar3.Scroll
        handle_color_changes()
    End Sub

    Private Sub HScrollBar4_Scroll(sender As Object, e As ScrollEventArgs) Handles HScrollBar4.Scroll
        handle_color_changes()
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        set_selected()
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        set_selected()
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        set_selected()
    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        set_selected()
    End Sub

    Private Sub RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton5.CheckedChanged
        set_selected()
    End Sub

    Private Sub frmEditCamo_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If is_camo_active() And Me.Visible Then
            camo_id = SELECTED_CAMO_BUTTON
            camo_change()
            set_selected()
        End If
    End Sub
End Class