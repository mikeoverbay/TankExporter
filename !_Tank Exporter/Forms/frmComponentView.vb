Public Class frmComponentView

    Private Sub frmComponentView_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Visible = False
    End Sub

    Private Sub frmComponentView_Load(sender As Object, e As EventArgs) Handles Me.Load
        splitter.Panel1.HorizontalScroll.Maximum = 0
        splitter.Panel1.AutoScroll = False
        splitter.Panel1.VerticalScroll.Visible = False
        splitter.Panel1.AutoScroll = True

        splitter.Panel2.HorizontalScroll.Maximum = 0
        splitter.Panel2.AutoScroll = False
        splitter.Panel2.VerticalScroll.Visible = False
        splitter.Panel2.AutoScroll = True

    End Sub

    Public Sub clear_group_list()
        splitter.Panel1.Controls.Clear()

    End Sub

    Public Sub clear_fbx_list()
        splitter.Panel2.Controls.Clear()
    End Sub

    Public Sub add_to_group_list(id As Integer, name As String)
        Dim cb As New CheckBox
        cb.AutoSize = True
        cb.Text = id.ToString("00") + ": " + name
        cb.BackColor = Color.Transparent
        cb.Location = New Point(5, (cb.Height * id) + 5)
        cb.Tag = id
        cb.Checked = True
        AddHandler cb.CheckedChanged, AddressOf update_group_view
        splitter.Panel1.Controls.Add(cb)
    End Sub
    Public Sub add_to_fbx_list(id As Integer, name As String)
        Dim cb As New CheckBox
        cb.AutoSize = True
        cb.Text = id.ToString("00") + ": " + name
        cb.BackColor = Color.Transparent
        cb.Location = New Point(5, (cb.Height * id) + 5)
        cb.Tag = id
        cb.Checked = True
        AddHandler cb.CheckedChanged, AddressOf update_fbx_view
        splitter.Panel2.Controls.Add(cb)
    End Sub

    Private Sub m_g_show_all_Click(sender As Object, e As EventArgs) Handles m_g_show_all.Click
        If splitter.Panel1.Controls.Count = 0 Then
            Return
        End If
        For Each c In splitter.Panel1.Controls
            c = DirectCast(c, CheckBox)
            c.checked = True
        Next
    End Sub

    Private Sub m_g_hide_all_Click(sender As Object, e As EventArgs) Handles m_g_hide_all.Click
        If splitter.Panel1.Controls.Count = 0 Then
            Return
        End If
        For Each c In splitter.Panel1.Controls
            c = DirectCast(c, CheckBox)
            c.checked = False
        Next

    End Sub

    Private Sub m_fbx_show_all_Click(sender As Object, e As EventArgs) Handles m_fbx_show_all.Click
        If splitter.Panel2.Controls.Count = 0 Then
            Return
        End If
        For Each c In splitter.Panel2.Controls
            c = DirectCast(c, CheckBox)
            c.checked = True
        Next

    End Sub

    Private Sub m_fbx_hide_all_Click(sender As Object, e As EventArgs) Handles m_fbx_hide_all.Click
        If splitter.Panel2.Controls.Count = 0 Then
            Return
        End If
        For Each c In splitter.Panel2.Controls
            c = DirectCast(c, CheckBox)
            c.checked = False
        Next
    End Sub
    Private Sub update_group_view(sender As CheckBox, e As EventArgs)
        _group(CInt(sender.tag)).component_visible = sender.checked
    End Sub
    Private Sub update_fbx_view(sender As CheckBox, e As EventArgs)
        fbxgrp(CInt(sender.Tag)).component_visible = sender.Checked
    End Sub
End Class