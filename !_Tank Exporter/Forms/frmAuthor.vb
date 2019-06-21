Public Class frmAuthor
    Public tank_mod_name As String
    Public FromDialogResult As Integer

    Private Sub creator_tb_TextChanged(sender As Object, e As EventArgs) Handles creator_tb.TextChanged
        mod_name_tb.Text = creator_tb.Text + ".remodel." + tank_mod_name
        description_tb.Text = "Author: " + creator_tb.Text
    End Sub

    Private Sub cancel_btn_Click(sender As Object, e As EventArgs) Handles cancel_btn.Click
        FromDialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Hide()
    End Sub


    Private Sub frmAuthor_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        'DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Hide()
    End Sub

    Private Sub ok_btn_Click(sender As Object, e As EventArgs) Handles ok_btn.Click
        FromDialogResult = System.Windows.Forms.DialogResult.OK
        My.Settings.authers_name = creator_tb.Text
        My.Settings.mod_version = version_tb.Text
        Me.Hide()
    End Sub
End Class