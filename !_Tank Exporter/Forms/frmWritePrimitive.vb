Imports System.Windows.Forms
Imports System.IO

Public Class frmWritePrimitive
    Public SAVE_NAME As String = ""
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not FBX_LOADED Then
            MsgBox("You need to IMPORT and FBX first!", MsgBoxStyle.Exclamation, "Not gonna happen!")
            Me.Hide()
            Return
        End If
        'We give the user the opertunity again to extract the model. We need some where to write any changed data too.
        If m_write_crashed.Checked Then
            CRASH_MODE = True
        Else
            CRASH_MODE = False
        End If
        If Not Directory.Exists(SAVE_NAME) Then
            If MsgBox("It appears You have not extracted data for this model." + vbCrLf + _
                       "Would you like to extract the data from the .PKG files?", MsgBoxStyle.YesNo, "Extract?") = MsgBoxResult.Yes Then
                file_name = "1:dummy:" + Path.GetFileNameWithoutExtension(SAVE_NAME.Replace("/", "\"))
                frmMain.m_extract.PerformClick()
            Else
                Me.Hide()
                Return
            End If

        End If
        Dim wrote_something = cew_cb.Checked Or hew_cb.Checked Or tew_cb.Checked Or gew_cb.Checked
        If wrote_something Then
            frmMain.info_Label.Parent = frmMain.pb1
            frmMain.info_Label.Visible = True
            frmMain.PG1.Value = 0
            frmMain.PG1.Visible = True


            If cew_cb.Checked Then
                If Not CRASH_MODE Then
                    write_chassis_primitives(1)
                Else
                    write_chassis_crashed(1)
                End If
            End If

            If hew_cb.Checked Then
                write_primitives(2)
            End If
            If tew_cb.Checked Then
                write_primitives(3)
            End If
            If gew_cb.Checked Then
                write_primitives(4)
            End If
            frmMain.info_Label.Visible = False
            frmMain.info_Label.Parent = frmMain
            frmMain.PG1.Visible = False

        End If

        If hide_tracks_cb.Checked And Not CRASH_MODE Then
            hide_tracks()
        End If

        If wrote_something And copy_lods_cb.Checked Then 'only if told to write to LODs
            Dim path = My.Settings.res_mods_path
            Dim p = path + "\" + m_groups(1).f_name(0)
            p = IO.Path.GetDirectoryName(p)
            If Directory.Exists(p) Then
                'Return
                Dim d = New DirectoryInfo(p)
                Dim di = d.GetFiles
                For Each n In di
                    If n.Name.Contains(".prim") Or n.Name.Contains(".visu") Then
                        'we are going to loop for 1 to 4, rename the path, create the directry
                        'if it doesn't exist and copy the file to it.
                        For num = 1 To 4
                            Dim np = n.FullName.ToLower.Replace("lod0", "lod" + num.ToString)
                            If Not Directory.Exists(IO.Path.GetDirectoryName(np)) Then 'if not exist, create it!
                                Directory.CreateDirectory(IO.Path.GetDirectoryName(np))
                            End If
                            If File.Exists(np) Then ' If file exist, delete it!
                                File.Delete(np)
                            End If
                            File.Copy(n.FullName, np) 'Copy the file.. Visual or Primitives
                        Next


                    End If
                Next

            End If
        End If
        frmMain.Focus()
        Me.Hide()
    End Sub
    Private Sub hide_broken_tracks(ByVal p As String)
        Dim path = My.Settings.res_mods_path
        Dim a = p.Split("\normal")
        path = path + "\" + a(0) + "\crash"
        If Not Directory.Exists(path) Then
            Directory.CreateDirectory(path)
        End If
        For num = 0 To 4
            Dim np = path + "\lod" + num.ToString
            If Not Directory.Exists(np) Then 'if not exist, create it!
                Directory.CreateDirectory(np)
            End If
            If Not File.Exists(np + "\Chassis.primitives_processed") Then 'stop possible exception
                File.Copy(Application.StartupPath + "\resources\primitive\Chassis.primitives_processed", np + "\Chassis.primitives_processed") 'Copy the file.. Visual or Primitives
            End If
        Next
    End Sub

End Class