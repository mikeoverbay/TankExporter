Imports System.IO
Public Class frmWrite_Primitives

    Private Sub m_go_Click(sender As Object, e As EventArgs) Handles m_go.Click
        Dim tp As String = My.Settings.res_mods_path
        If File.Exists(Temp_Storage + "\primitive_file_save_path.txt") Then
            tp = File.ReadAllText(Temp_Storage + "\primitive_file_save_path.txt")
        End If
        SaveFileDialog1.Filter = "Primitives|*.primitives_processed"
        SaveFileDialog1.Title = "Write Primitives file..."
        SaveFileDialog1.InitialDirectory = tp
        SaveFileDialog1.FileName = FBX_NAME
        If Not SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Me.Hide()
            Return
        End If
        File.WriteAllText(Temp_Storage + "\primitive_file_save_path.txt", Path.GetDirectoryName(SaveFileDialog1.FileName))
        Dim v_path = SaveFileDialog1.FileName.Replace(".primitives", ".visual")
        If Not File.Exists(v_path) Then
            MsgBox("Unable to open: " + Path.GetFileName(v_path), MsgBoxStyle.Exclamation, "NO VISUAL FILE!")
            Me.Hide()
            Return
        End If
        Dim ms As New MemoryStream(File.ReadAllBytes(v_path))
        openXml_stream(ms, Path.GetFileName(v_path))
        XML_Strings(2) = TheXML_String.Replace("map_", "")
        Dim fn = SaveFileDialog1.FileName
        fn = fn.Replace(My.Settings.res_mods_path + "\", "")
        ReDim m_groups(2).f_name(1)
        m_groups(2).f_name(0) = fn
        write_primitives(2)
        Me.Hide()
    End Sub
End Class