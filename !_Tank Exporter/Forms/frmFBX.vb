Imports System.IO
Public Class frmFBX

    Private Sub Start_Export_btn_Click(sender As Object, e As EventArgs) Handles Start_Export_btn.Click
        Start_Export_btn.Enabled = False
        'skip this next line. Tank data is already loaded with the new changes.
        'frmMain.process_tank(False) 'false .. don't save the binary tank file

        file_name = current_tank_name

        If export_textures.Checked Then ' export textures and current camo?
            If is_camo_active() Then ' find out if there is a camouflage in use.
                save_camo_texture(SELECTED_CAMO_BUTTON, Temp_Storage)
                Dim ar = TANK_NAME.Split(":")
                Dim name As String = Path.GetFileName(ar(0))
                FBX_Texture_path = Path.GetDirectoryName(My.Settings.fbx_path) + "\" + name + "\camouflage.png"
                If Not Directory.Exists(Path.GetDirectoryName(My.Settings.fbx_path) + "\" + name) Then
                    Directory.CreateDirectory(Path.GetDirectoryName(My.Settings.fbx_path) + "\" + name)
                End If
                If File.Exists(FBX_Texture_path) Then
                    File.Delete(FBX_Texture_path)
                End If
                File.Copy(Temp_Storage + "\camouflage.png", FBX_Texture_path)
            End If
            export_fbx_textures() 'export all textures
        End If

        modFBX.export_fbx()
        Start_Export_btn.Enabled = True
        Me.Hide()
    End Sub

    Private Sub frmFBX_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
    End Sub

    Private Sub frmFBX_Load(sender As Object, e As EventArgs) Handles Me.Load
        Label2.Text = ""
    End Sub

    Private Sub Cancel_bnt_Click(sender As Object, e As EventArgs) Handles Cancel_bnt.Click
        Me.Hide()
    End Sub
End Class