Imports System.Windows.Forms
Public Class frmComponents

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

    Private Sub continue_bt_Click(sender As Object, e As EventArgs) Handles continue_bt.Click
        Me.Hide()
    End Sub

    Private Sub frmComponents_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True 'cant destroy the viewtree data
        Me.Hide()
    End Sub
End Class