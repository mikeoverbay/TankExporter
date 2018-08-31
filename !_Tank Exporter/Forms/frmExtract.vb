Public Class frmExtract

    Private Sub frmExtract_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
        Return
    End Sub

    Private Sub frmExtract_Load(sender As Object, e As EventArgs) Handles Me.Load
        TT.AutomaticDelay = 5000
        TT.InitialDelay = 1000
        TT.ReshowDelay = 500
        TT.ShowAlways = True
        TT.SetToolTip(Me.all_lods_rb, "Extract ALL LOD models")
        TT.SetToolTip(Me.lod_0_only, "Extract ONLY lod0")
        TT.SetToolTip(Me.no_models, "Do NOT extract Models")
        TT.SetToolTip(Me.ext_chassis, "Extract Chassis Texures and Models")
        TT.SetToolTip(Me.ext_hull, "Extract Hull Texures and Models")
        TT.SetToolTip(Me.ext_turret, "Extract Turret Texures and Models")
        TT.SetToolTip(Me.ext_gun, "Extract Gun Texures and Models")
        TT.SetToolTip(Me.m_customization, "Extract Customization.xml")
        TT.SetToolTip(Me.create_work_area_cb, "Create Work Area folder")

    End Sub

    Private Sub cancel_btn_Click(sender As Object, e As EventArgs) Handles cancel_btn.Click
        Me.Hide()
        Return
    End Sub

    Private Sub extract_btn_Click(sender As Object, e As EventArgs) Handles extract_btn.Click
        frmMain.extract_selections()
        Me.Hide()
    End Sub
End Class