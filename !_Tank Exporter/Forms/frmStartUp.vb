Public Class frmStartUp
    Private Sub stop_loading_Decals_cb_CheckedChanged(sender As Object, e As EventArgs) Handles stop_loading_Decals_cb.CheckedChanged
        My.Settings.stop_loading_decals = stop_loading_Decals_cb.Checked

    End Sub
End Class