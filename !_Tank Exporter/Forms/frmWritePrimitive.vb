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
        frmMain.Focus()
        Me.Hide()
    End Sub
End Class