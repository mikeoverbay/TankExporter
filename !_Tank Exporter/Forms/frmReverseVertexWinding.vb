Public Class frmReverseVertexWinding
    Private Sub frmComponentView_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Visible = False
    End Sub

    Private Sub frmComponentView_Load(sender As Object, e As EventArgs) Handles Me.Load
        Panel1.HorizontalScroll.Maximum = 0
        Panel1.AutoScroll = False
        Panel1.VerticalScroll.Visible = False
        Panel1.AutoScroll = True

    End Sub

    Public Sub clear_group_list()
        Panel1.Controls.Clear()
    End Sub


    Public Sub add_to_fbx_list(id As Integer, name As String)
        Dim cb As New CheckBox
        cb.AutoSize = True
        cb.Text = id.ToString("00") + ": " + name
        cb.BackColor = Color.Transparent
        cb.Location = New Point(5, (cb.Height * id) + 5)
        cb.Tag = id
        cb.Checked = False
        AddHandler cb.CheckedChanged, AddressOf update_fbx_winding
        Panel1.Controls.Add(cb)

    End Sub

    Private Sub m_g_check_all_Click(sender As Object, e As EventArgs) Handles m_g_show_all.Click
        If Panel1.Controls.Count = 0 Then
            Return
        End If
        For Each c In Panel1.Controls
            c = DirectCast(c, CheckBox)
            c.checked = True
        Next
    End Sub

    Private Sub m_g_uncheck_all_Click(sender As Object, e As EventArgs) Handles m_g_hide_all.Click
        If Panel1.Controls.Count = 0 Then
            Return
        End If
        For Each c In Panel1.Controls
            c = DirectCast(c, CheckBox)
            c.checked = False
        Next

    End Sub

    Private Sub update_fbx_winding(sender As CheckBox, e As EventArgs)
        fbxgrp(CInt(sender.Tag)).reverse_winding = sender.Checked
    End Sub

    Private Sub frmReverseVertexWinding_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged

    End Sub
End Class