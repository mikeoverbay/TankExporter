Public Class frmSettings

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        FOV_trackbar.Value = CInt(FOV)
        TrackBar1.Value = CInt((mouse_speed_global / 3.0) * 100)
        fov_v.Text = FOV.ToString("000")
        mouse_v.Text = mouse_speed_global.ToString("0.00")
    End Sub

    Private Sub frmSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.Save()
    End Sub

    Private Sub FOV_trackbar_Scroll(sender As Object, e As EventArgs) Handles FOV_trackbar.Scroll
        Dim val = FOV_trackbar.Value / FOV_trackbar.Maximum
        FOV = val * 100.0!
        My.Settings.fov = FOV
        fov_v.Text = FOV.ToString("000")
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Dim val = TrackBar1.Value / TrackBar1.Maximum
        mouse_speed_global = val * 3
        My.Settings.mouse_speed = mouse_speed_global
        mouse_v.Text = mouse_speed_global.ToString("0.00")
    End Sub

End Class