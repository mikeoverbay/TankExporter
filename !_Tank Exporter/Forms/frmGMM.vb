

Public Class frmGMM

    Dim mm As Point
    Dim delta As Point
    Dim md As Boolean

    Private Sub frmGMM_Load(sender As Object, e As EventArgs) Handles Me.Load
        frmMain.GMM_R = CSng(red_slider.Value / 100)
        red_value_tb.Text = CInt(255 * frmMain.GMM_R)
        frmMain.GMM_B = CSng(blue_slider.Value / 100)
        blue_value_tb.Text = CInt(255 * frmMain.GMM_B)

    End Sub

    Private Sub frmGMM_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If e.Button = System.Windows.Forms.MouseButtons.Left Then
            md = True
            mm = e.Location
        End If
    End Sub

    Private Sub frmGMM_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If md Then
            Me.Location = Me.Location + (e.Location - mm)
        End If
    End Sub

    Private Sub frmGMM_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        md = False
    End Sub

    Private Sub red_slider_Scroll(sender As Object, e As EventArgs) Handles red_slider.Scroll
        frmMain.GMM_R = CSng(red_slider.Value / 100)
        red_value_tb.Text = CInt(255 * frmMain.GMM_R)
    End Sub

    Private Sub blue_slider_Scroll(sender As Object, e As EventArgs) Handles blue_slider.Scroll
        frmMain.GMM_B = CSng(blue_slider.Value / 100)
        blue_value_tb.Text = CInt(255 * frmMain.GMM_B)
    End Sub
End Class