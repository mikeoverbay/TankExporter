Public Class frmLighting

    Private Sub frmLighting_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
        Return
    End Sub

    Private Sub ambient_slider_MouseEnter(sender As Object, e As EventArgs) Handles ambient_slider.MouseEnter
        ambient_slider.Focus()
    End Sub

    Private Sub specular_slider_MouseEnter(sender As Object, e As EventArgs) Handles specular_slider.MouseEnter
        specular_slider.Focus()
    End Sub

    Private Sub total_slider_MouseEnter(sender As Object, e As EventArgs) Handles total_slider.MouseEnter
        total_slider.Focus()
    End Sub

    Private Sub ambient_slider_ValueChanged(sender As Object, e As EventArgs) Handles ambient_slider.ValueChanged
        A_level = CSng(ambient_slider.Value / 100.0!)
        If stop_updating Then frmMain.draw_scene()

    End Sub

    Private Sub specular_slider_ValueChanged(sender As Object, e As EventArgs) Handles specular_slider.ValueChanged
        S_level = CSng(specular_slider.Value / 100.0!)
        If stop_updating Then frmMain.draw_scene()

    End Sub

    Private Sub total_slider_ValueChanged(sender As Object, e As EventArgs) Handles total_slider.ValueChanged
        T_level = CSng(total_slider.Value / 100.0!)
        If stop_updating Then frmMain.draw_scene()

    End Sub
End Class