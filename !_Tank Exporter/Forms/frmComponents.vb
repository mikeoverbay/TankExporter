Imports System.Windows.Forms
Public Class frmComponents
    Public c_idx, h_idx, g_idx, t_idx As Integer
    Private Sub tv_guns_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tv_guns.NodeMouseClick
        Dim tv = DirectCast(sender, TreeView)
        tv.SelectedNode = Nothing
        For i = 0 To tv.Nodes.Count - 1
            tv.Nodes(i).Checked = False
        Next
        tv.SelectedNode = e.Node
        tv.SelectedNode.Checked = True
    End Sub

    Private Sub tv_turrets_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tv_turrets.NodeMouseClick
        Dim tv = DirectCast(sender, TreeView)
        tv.SelectedNode = Nothing
        For i = 0 To tv.Nodes.Count - 1
            tv.Nodes(i).Checked = False
        Next
        tv.SelectedNode = e.Node
        tv.SelectedNode.Checked = True
    End Sub

    Private Sub tv_hulls_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tv_hulls.NodeMouseClick
        Dim tv = DirectCast(sender, TreeView)
        tv.SelectedNode = Nothing
        For i = 0 To tv.Nodes.Count - 1
            tv.Nodes(i).Checked = False
        Next
        tv.SelectedNode = e.Node
        tv.SelectedNode.Checked = True
    End Sub

    Private Sub tv_chassis_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles tv_chassis.NodeMouseClick
        Dim tv = DirectCast(sender, TreeView)
        tv.SelectedNode = Nothing
        For i = 0 To tv.Nodes.Count - 1
            tv.Nodes(i).Checked = False
        Next
        tv.SelectedNode = e.Node
        tv.SelectedNode.Checked = True
    End Sub

    Private Sub continue_bt_Click(sender As Object, e As EventArgs) Handles continue_bt.Click
        Me.Close()
    End Sub

    Private Sub frmComponents_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True 'cant destroy the viewtree data
        c_idx = tv_chassis.SelectedNode.Index
        h_idx = tv_hulls.SelectedNode.Index
        g_idx = tv_guns.SelectedNode.Index
        If tv_turrets.SelectedNode IsNot Nothing Then
            t_idx = tv_turrets.SelectedNode.Index
        Else
            t_idx = 0
        End If

        Me.Hide()
    End Sub

End Class