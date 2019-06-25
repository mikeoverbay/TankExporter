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


Public Class frmScreenCap
    Public r_size As New Size
    Public r_trans As Boolean
    Public r_terrain As Boolean
    Public r_color_flag As Boolean
    Public r_color_val(3) As Single
    Public RENDER_OUT As Boolean
    Public r_filename As String
    Private r_color = Color.DarkGray
    Public Sub set_background_mode()
        r_trans = trans_rb.Checked
        r_terrain = terrain_rb.Checked
        r_color_flag = color_rb.Checked
        ColorDialog1.Color = r_color
        r_color_val(0) = CSng(r_color.R / 255)
        r_color_val(1) = CSng(r_color.G / 255)
        r_color_val(2) = CSng(r_color.B / 255)
        r_color_val(3) = 1.0

    End Sub

    Public Sub set_size()
        If x1980x1200_rb.Checked Then
            r_size.Width = 1920
            r_size.Height = 1200
        End If
        If x1920_rb.Checked Then
            r_size.Width = 1920
            r_size.Height = 1080
        End If
        If x1440_rb.Checked Then
            r_size.Width = 1440
            r_size.Height = 900
        End If
        If x1280_rb.Checked Then
            r_size.Width = 1280
            r_size.Height = 800
        End If
        If x1366_rb.Checked Then
            r_size.Width = 1366
            r_size.Height = 768
        End If
        If wot_carousel_rb.Checked Then
            r_size.Width = 160
            r_size.Height = 100
        End If
        If wot_panel_rb.Checked Then
            r_size.Width = 420
            r_size.Height = 307
        End If
        If wot_wotmod_rb.Checked Then
            r_size.Width = 302
            r_size.Height = 170
        End If
        If x64_rb.Checked Then
            set_custom_size()
        End If

    End Sub
    Private Sub x1980x1200_rb_CheckedChanged(sender As Object, e As EventArgs) Handles x1980x1200_rb.CheckedChanged
    End Sub
    Private Sub x1920_rb_CheckedChanged(sender As Object, e As EventArgs) Handles x1920_rb.CheckedChanged
    End Sub

    Private Sub x1440_rb_CheckedChanged(sender As Object, e As EventArgs) Handles x1440_rb.CheckedChanged
    End Sub

    Private Sub x1280_rb_CheckedChanged(sender As Object, e As EventArgs) Handles x1280_rb.CheckedChanged
    End Sub

    Private Sub x1366_rb_CheckedChanged(sender As Object, e As EventArgs) Handles x1366_rb.CheckedChanged
    End Sub

    Private Sub x64_rb_CheckedChanged(sender As Object, e As EventArgs) Handles x64_rb.CheckedChanged
    End Sub

    Private Sub wot_carousel_rb_CheckedChanged(sender As Object, e As EventArgs) Handles wot_carousel_rb.CheckedChanged
    End Sub

    Private Sub wot_panel_rb_CheckedChanged(sender As Object, e As EventArgs) Handles wot_panel_rb.CheckedChanged
    End Sub

    Private Sub wot_wotmod_rb_CheckedChanged(sender As Object, e As EventArgs) Handles wot_wotmod_rb.CheckedChanged
    End Sub

    Private Sub trans_rb_CheckedChanged(sender As Object, e As EventArgs) Handles trans_rb.CheckedChanged
        r_trans = trans_rb.Checked
    End Sub

    Private Sub terrain_rb_CheckedChanged(sender As Object, e As EventArgs) Handles terrain_rb.CheckedChanged
        r_terrain = terrain_rb.Checked
    End Sub

    Private Sub color_rb_CheckedChanged(sender As Object, e As EventArgs) Handles color_rb.CheckedChanged
        r_color_flag = color_rb.Checked
        If color_rb.Checked Then
            pick_color_btn.Enabled = True
        Else
            pick_color_btn.Enabled = False
        End If
    End Sub

    Private Sub pick_color_btn_Click(sender As Object, e As EventArgs) Handles pick_color_btn.Click
        ColorDialog1.Color = r_color
        If ColorDialog1.ShowDialog <> Forms.DialogResult.Cancel Then
            r_color = ColorDialog1.Color
            r_color_val(0) = CSng(r_color.R / 255)
            r_color_val(1) = CSng(r_color.G / 255)
            r_color_val(2) = CSng(r_color.B / 255)
            r_color_val(3) = 1.0
        End If
    End Sub

    Private Sub save_btn_Click(sender As Object, e As EventArgs) Handles save_btn.Click
        If r_size.Height = 0 Or r_size.Height = 0 Then
            MsgBox("You need to put in a valid size!", MsgBoxStyle.Exclamation, "Opps...")
            Return
        End If
        CUSTOM_IMAGE_MODE = False
        set_size()
        SaveFileDialog1.Title = "Save Screen Capture PNG file..."
        SaveFileDialog1.Filter = "Save PNG Image (*.png*)|*.png"
        If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            If image_rb.Checked Then
                OpenFileDialog1.Title = "Load Custom Background Image..."
                OpenFileDialog1.Filter = "PNG (*.png)|*.png|JPG (*.jpj)|*.jpg|DDS (*.dds)|*.dds"
                OpenFileDialog1.InitialDirectory = My.Settings.custom_image
                If Not OpenFileDialog1.ShowDialog = Forms.DialogResult.OK Then
                    Return
                End If
                My.Settings.custom_image = OpenFileDialog1.FileName
                r_terrain = False
                r_color_flag = False
                r_trans = False
                If Gl.glIsTexture(custom_image_text_Id) Then 'remove the texture if it was already loaded
                    Gl.glDeleteTextures(1, custom_image_text_Id)
                End If
                Dim i_path = OpenFileDialog1.FileName
                If i_path.ToLower.Contains(".dds") Then
                    custom_image_text_Id = load_dds_file(i_path)
                End If
                If i_path.ToLower.Contains(".jpg") Then
                    custom_image_text_Id = load_jpg_file(i_path)
                End If
                If i_path.ToLower.Contains(".png") Then
                    custom_image_text_Id = load_png_file(i_path)
                End If
                CUSTOM_IMAGE_MODE = True
            End If
            r_filename = SaveFileDialog1.FileName
            RENDER_OUT = True
            frmMain.pb1.Dock = DockStyle.None
            frmMain.pb1.Size = r_size
            Application.DoEvents()
            G_Buffer.init()
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)
            G_Buffer.attachColorTexture()
            frmMain.draw_scene()
            RENDER_OUT = False
            CUSTOM_IMAGE_MODE = False
            save_image()
            frmMain.pb1.Dock = DockStyle.Fill
            Application.DoEvents()
            G_Buffer.init()
            Me.Hide()
        End If
    End Sub

    Private Sub frmScreenCap_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub frmScreenCap_Load(sender As Object, e As EventArgs) Handles Me.Load
        set_custom_size()
    End Sub

    Private Sub frmScreenCap_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        frmMain.draw_scene()
    End Sub
    Private Sub save_image()
        Application.DoEvents()
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        'frmMain.gl_stop = True
        'While gl_busy
        'End While
        set_background_mode()
        set_size()
        Dim w, h As Integer
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        w = frmMain.pb1.Width
        h = frmMain.pb1.Height
        Gl.glFinish()
        Dim tId As Integer = Il.ilGenImage
        Il.ilBindImage(tId)
        Il.ilTexImage(w, h, 0, 4, Il.IL_RGBA, Il.IL_UNSIGNED_BYTE, Nothing)

        Gl.glReadPixels(0, 0, w, h, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
        Gl.glFinish()

        Gl.glFinish()
        Il.ilSave(Il.IL_PNG, r_filename) ' save to temp
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Il.ilBindImage(0)
        Il.ilDeleteImage(tId)
        Application.DoEvents()


    End Sub

    Private Sub cancel_btn_Click(sender As Object, e As EventArgs) Handles cancel_btn.Click
        Me.Hide()
    End Sub

    Private Sub x_size_tb_TextChanged(sender As Object, e As EventArgs) Handles x_size_tb.TextChanged
        If Not IsNumeric(x_size_tb.Text) Or x_size_tb.Text.Contains(" ") Or x_size_tb.Text.Contains(".") Then
            x_size_tb.Text = Strings.Left(x_size_tb.Text, x_size_tb.Text.Length - 1)
            x_size_tb.SelectionStart = x_size_tb.Text.Length
        End If
        set_custom_size()
    End Sub

    Private Sub y_size_tb_TextChanged(sender As Object, e As EventArgs) Handles y_size_tb.TextChanged
        If Not IsNumeric(y_size_tb.Text) Or y_size_tb.Text.Contains(" ") Or y_size_tb.Text.Contains(".") Then
            y_size_tb.Text = Strings.Left(y_size_tb.Text, y_size_tb.Text.Length - 1)
            y_size_tb.SelectionStart = y_size_tb.Text.Length
        End If
        set_custom_size()
    End Sub
    Private Sub set_custom_size()
        Try
            r_size.Width = Convert.ToInt32(x_size_tb.Text)
            r_size.Height = Convert.ToInt32(y_size_tb.Text)

        Catch ex As Exception

        End Try

    End Sub

End Class