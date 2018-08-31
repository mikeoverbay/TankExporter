Public Class frmEditVisual
    Private current_xml = 1
    Private change_cnt As Integer = 0
    Private Sub frmEditVisual_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        'e.Cancel = True
        check_modified()
        'Me.Hide()
    End Sub

    Private Sub frmEditVisual_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Show()
        current_xml = 1
        change_cnt = -1.0
        set_menu_colors()
        Application.DoEvents()
        RichTextBox1.Text = XML_Strings(current_xml)
    End Sub

    Private Sub m_c_xml_Click(sender As Object, e As EventArgs) Handles m_c_xml.Click
        check_modified()
        current_xml = 1
        set_menu_colors()
        get_xml()
    End Sub

    Private Sub m_h_xml_Click(sender As Object, e As EventArgs) Handles m_h_xml.Click
        check_modified()
        current_xml = 2
        set_menu_colors()
        get_xml()
    End Sub

    Private Sub m_t_xml_Click(sender As Object, e As EventArgs) Handles m_t_xml.Click
        check_modified()
        current_xml = 3
        set_menu_colors()
        get_xml()
    End Sub

    Private Sub m_g_xml_Click(sender As Object, e As EventArgs) Handles m_g_xml.Click
        check_modified()
        current_xml = 4
        set_menu_colors()
        get_xml()
    End Sub

    Private Sub set_menu_colors()
        m_c_xml.ForeColor = Color.Black
        m_h_xml.ForeColor = Color.Black
        m_t_xml.ForeColor = Color.Black
        m_g_xml.ForeColor = Color.Black

        If current_xml = 1 Then
            m_c_xml.ForeColor = Color.Red
        End If
        If current_xml = 2 Then
            m_h_xml.ForeColor = Color.Red
        End If
        If current_xml = 3 Then
            m_t_xml.ForeColor = Color.Red
        End If
        If current_xml = 4 Then
            m_g_xml.ForeColor = Color.Red
        End If

    End Sub
    Private Sub check_modified()
        If change_cnt > 0 Then
            If MsgBox("You modified this XML. Update Data?", MsgBoxStyle.YesNo, "Changes..") = MsgBoxResult.Yes Then
                For i = 0 To 30
                    Dim ast = RichTextBox1.Text.Replace("<primitiveGroup>" + ControlChars.CrLf.ToCharArray() + "<PG_ID>" + i.ToString + "</PG_ID>", "<primitiveGroup>" + i.ToString)
                    RichTextBox1.Text = ast
                Next
                XML_Strings(current_xml) = RichTextBox1.Text
            End If
        End If
    End Sub
    Private Sub get_xml()
        RichTextBox1.Text = XML_Strings(current_xml)
        change_cnt = 0
    End Sub

    Private Sub RichTextBox1_TextChanged(sender As Object, e As EventArgs) Handles RichTextBox1.TextChanged
        change_cnt += 1
    End Sub


    Private Sub m_copy_Click(sender As Object, e As EventArgs) Handles m_copy.Click
        If RichTextBox1.SelectedText.Length > 0 Then
            Clipboard.SetText(RichTextBox1.SelectedText)
        End If
    End Sub

    Private Sub m_cut_Click(sender As Object, e As EventArgs) Handles m_cut.Click
        If RichTextBox1.SelectedText.Length > 0 Then
            Clipboard.SetText(RichTextBox1.SelectedText)
            RichTextBox1.SelectedText = RichTextBox1.SelectedText.Replace(RichTextBox1.SelectedText, "")
        End If
    End Sub

    Private Sub m_paste_Click(sender As Object, e As EventArgs) Handles m_paste.Click
        If Clipboard.GetText.Length > 0 Then
            RichTextBox1.Paste()
        End If
    End Sub



    Private Sub m_down_Click(sender As Object, e As EventArgs) Handles m_down.Click
        Try
            RichTextBox1.ZoomFactor += 0.1!
            Application.DoEvents()
            Application.DoEvents()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub m_up_Click(sender As Object, e As EventArgs) Handles m_up.Click
        Try
            RichTextBox1.ZoomFactor -= 0.1!
            Application.DoEvents()
            Application.DoEvents()
        Catch ex As Exception
        End Try
    End Sub
End Class