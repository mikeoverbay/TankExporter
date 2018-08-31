Public Class frmLightSelection

    Private Sub frmLightSelection_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmLightSelection_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        AddHandler RadioButton1.CheckedChanged, AddressOf CheckedChanged
        AddHandler RadioButton2.CheckedChanged, AddressOf CheckedChanged
        AddHandler RadioButton3.CheckedChanged, AddressOf CheckedChanged

        selected_light = CInt(My.Settings.selected_light)
        Select Case selected_light
            Case 0
                RadioButton1.Checked = True
            Case 1
                RadioButton2.Checked = True
            Case 2
                RadioButton3.Checked = True
        End Select

    End Sub

    Private Sub CheckedChanged(sender As RadioButton, e As EventArgs) Handles RadioButton1.CheckedChanged
        If sender.Checked Then
            Select Case CInt(sender.Tag)
                Case 0
                    selected_light = 0
                Case 1
                    selected_light = 1
                Case 2
                    selected_light = 2
            End Select
            My.Settings.selected_light = selected_light.ToString
            If Not _Started Then Return
            If stop_updating Then frmMain.draw_scene()
        End If
    End Sub
End Class