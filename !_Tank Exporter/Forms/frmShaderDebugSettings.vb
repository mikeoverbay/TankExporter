
#Region "imports"
Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Web
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports Tao.FreeGlut
Imports Tao.FreeGlut.Glut
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Tao.DevIl
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports Ionic.Zip
Imports System.Drawing.Imaging
Imports System.Globalization
#End Region


Public Class frmShaderDebugSettings

    Private Sub frmShaderDebugSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub




    Private Sub frmShaderDebugSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Show()
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()

        Add_handlers()

    End Sub

    Private Sub add_handlers()

        AddHandler F.CheckedChanged, AddressOf a_group
        AddHandler G.CheckedChanged, AddressOf a_group
        AddHandler D.CheckedChanged, AddressOf a_group
        AddHandler specContrib.CheckedChanged, AddressOf a_group

        AddHandler diffuseContrib.CheckedChanged, AddressOf b_group
        AddHandler baseColor.CheckedChanged, AddressOf b_group
        AddHandler Metallic.CheckedChanged, AddressOf b_group
        AddHandler Roughness.CheckedChanged, AddressOf b_group

    End Sub
    Private Sub a_group(sender As RadioButton, e As EventArgs)
        If Not sender.Checked Then Return
        b_normal.Checked = True
        Dim mask = CInt(sender.Tag)
        section_a = 0
        section_a = mask
        If stop_updating Then frmMain.draw_scene()
    End Sub
    Private Sub b_group(sender As RadioButton, e As EventArgs)
        If Not sender.Checked Then Return
        Dim mask = CInt(sender.Tag)
        section_b = 0
        section_b = mask
        If stop_updating Then frmMain.draw_scene()
    End Sub

    Private Sub a_normal_CheckedChanged(sender As RadioButton, e As EventArgs) Handles a_normal.CheckedChanged
        If a_normal.Checked Then
            section_a = 0
        End If
        If stop_updating Then frmMain.draw_scene()
    End Sub

    Private Sub b_normal_CheckedChanged(sender As RadioButton, e As EventArgs) Handles b_normal.CheckedChanged
        If b_normal.Checked Then
            section_b = 0
        End If
        If stop_updating Then frmMain.draw_scene()

    End Sub
End Class