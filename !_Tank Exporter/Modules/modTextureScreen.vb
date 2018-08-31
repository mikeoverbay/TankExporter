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

#End Region



Module modTextureScreen
#Region "Variables"
    Public TANKPARTS_VISIBLE As Boolean
    Public TANK_TEXTURES_VISIBLE As Boolean
    Public Selected_TankPart As Integer
#End Region

    Public Sub reset_tank_buttons()
        For i = 0 To tankpart_buttons.Length - 2
            tankpart_buttons(i).selected = 0
            tankpart_buttons(i).state = 0
        Next
    End Sub

    Public Sub load_tank_buttons()
        Dim b As New TankPart_btn
        Dim y As Single = -10
        b.state = 0
        b.tankpart_id = load_png_file(Application.StartupPath + "\resources\chassis.png")
        b.size = New Point(40, 40)
        b.location = New Point(10, y)
        b.callmode = "chassis"
        b.add()

        b = New TankPart_btn
        b.state = 0
        b.tankpart_id = load_png_file(Application.StartupPath + "\resources\hull.png")
        b.size = New Point(40, 40)
        b.location = New Point(10, y)
        b.callmode = "hull"
        b.add()

        b = New TankPart_btn
        b.state = 0
        b.tankpart_id = load_png_file(Application.StartupPath + "\resources\tower.png")
        b.size = New Point(40, 40)
        b.location = New Point(10, y)
        b.callmode = "turret"
        b.add()

        b = New TankPart_btn
        b.state = 0
        b.tankpart_id = load_png_file(Application.StartupPath + "\resources\gun.png")
        b.size = New Point(40, 40)
        b.location = New Point(10, y)
        b.callmode = "gun"
        b.add()
        delete_image_start += 4
        relocate_tankbuttons()

    End Sub
    Public Sub draw_texture_screen()
        For i = 0 To tankpart_buttons.Length - 2
            tankpart_buttons(i).draw()
        Next
    End Sub

    Public Sub tank_button_clicked(ByVal part As String)
        If Not MODEL_LOADED Then
            Return
        End If
       
        ReDim texture_buttons(0)
        GC.Collect()
        GC.WaitForFullGCComplete()
        Dim id As Integer = -1
        Dim r_cnt As Integer = 0
        Dim b_size As Integer = 50
        For i = 1 To object_count
            If _group(i).name.ToLower.Contains(part) Then
                id = i
                r_cnt += 1
                If id > -1 Then
                    'DIFFUSE
                    If _group(id).color_Id > 0 Then
                        Dim b As New TankTexture_btn
                        b.name = _group(id).color_name
                        b.gl_textureID = _group(id).color_Id
                        b.part_ID = i
                        b.size = New Point(b_size, b_size)
                        b.location = New Point(10, -60 * r_cnt)
                        b.state = 0
                        b.selected = 0
                        b.add()
                    End If
                    'AO
                    If _group(id).ao_id > 0 Then
                        Dim b As New TankTexture_btn
                        b.name = _group(id).ao_name
                        b.gl_textureID = _group(id).ao_id
                        b.part_ID = i
                        b.size = New Point(b_size, b_size)
                        b.location = New Point(10, -60 * r_cnt)
                        b.state = 0
                        b.selected = 0
                        b.add()
                    End If
                    'GMM
                    If _group(id).metalGMM_Id > 0 Then
                        Dim b As New TankTexture_btn
                        b.name = _group(id).metalGMM_name
                        b.gl_textureID = _group(id).metalGMM_Id
                        b.part_ID = i
                        b.size = New Point(b_size, b_size)
                        b.location = New Point(10, -60 * r_cnt)
                        b.state = 0
                        b.selected = 0
                        b.add()
                    End If
                    'NORMAL
                    If _group(id).normal_Id > 0 Then
                        Dim b As New TankTexture_btn
                        b.name = _group(id).normal_name
                        b.gl_textureID = _group(id).normal_Id
                        b.part_ID = i
                        b.size = New Point(b_size, b_size)
                        b.location = New Point(10, -60 * r_cnt)
                        b.state = 0
                        b.selected = 0
                        b.add()
                    End If
                End If
            End If
        Next
        relocate_texturebuttons()
        TANK_TEXTURES_VISIBLE = True
    End Sub
    Public Sub texture_button_clicked(ByVal tex_id As Integer, ByVal part_id As Integer)
        current_image = tex_id
        current_tank_part = part_id
        frmTextureViewer.Visible = True
        frmTextureViewer.set_current_image()

    End Sub

    Public Sub relocate_tankbuttons()
        Dim cnt = tankpart_buttons.Length - 1
        Dim butt_width = tankpart_buttons(0).size.X
        Dim space As Integer = 10
        Dim sw = frmMain.pb1.Width
        Dim rw = (butt_width * cnt) + (space * (cnt - 1))
        Dim stepsize = butt_width + space
        Dim ss = (sw / 2.0) - (rw / 2.0)
        For i = 0 To cnt - 1
            tankpart_buttons(i).location.X = ss
            'tankpart_buttons(i).location.Y -= WINDOW_HEIGHT_DELTA
            ss += stepsize
        Next
    End Sub

    Public Sub relocate_texturebuttons()
        Dim cnt = texture_buttons.Length - 1
        Dim butt_width = texture_buttons(0).size.X
        Dim space As Integer = 10
        Dim sw = frmMain.pb1.Width
        Dim rw = (butt_width * cnt) + (space * (cnt - 1))
        Dim stepsize = butt_width + space
        Dim ss = (sw / 2.0) - (rw / 2.0)
        For i = 0 To cnt - 1
            texture_buttons(i).location.X = ss
            'tankpart_buttons(i).location.Y -= WINDOW_HEIGHT_DELTA
            ss += stepsize
        Next
    End Sub

End Module
