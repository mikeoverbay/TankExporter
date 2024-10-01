Imports System.Windows.Forms
Public Class frmComponents
    Private Const WM_SYSCOMMAND As Integer = &H112
    Private Const SC_CLOSE As Integer = &HF060
    Private userClickedCloseButton As Boolean = False
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_SYSCOMMAND AndAlso m.WParam.ToInt32() = SC_CLOSE Then
            ' User clicked the 'X' button
            userClickedCloseButton = True
        End If

        MyBase.WndProc(m)
    End Sub

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

    Private Sub cancel_btn_Click(sender As Object, e As EventArgs) Handles cancel_btn.Click
        WORKING = False
        Me.Close()
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

    Private Sub frmComponents_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If userClickedCloseButton Then
            WORKING = False
        End If
        e.Cancel = True 'cant destroy the viewtree data
        Me.Hide()

    End Sub

End Class