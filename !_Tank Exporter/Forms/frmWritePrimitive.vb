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

        Dim wrote_something = cew_cb.Checked Or hew_cb.Checked Or tew_cb.Checked
        If cew_cb.Checked Then
            write_chassis_primitives(1)
        End If

        If hew_cb.Checked Then
            write_primitives(2)
        End If
        If tew_cb.Checked Then
            write_primitives(3)
        End If
        If hide_tracks_cb.Checked Then
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
End Class