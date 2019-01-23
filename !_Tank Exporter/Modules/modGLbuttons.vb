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


Module modGLbuttons
    Public season_Buttons(0) As Nbutton_
    Public camo_Buttons(0) As Camobutton_
    'Public Winter_Buttons(0) As Wbutton_
    'Public Dessert_Buttons(0) As Dbutton_
    Public BUTTON_ID As Integer
    Public BUTTON_TYPE As Integer
    '#########################################################
    'Image Screen buttons
    Public TANK_PART_ID As Integer
    Public TANK_TEXTURE_ID As Integer
    Public TANKPART_BUTTON_DOWN As Boolean
    Public tankpart_buttons(0) As TankPart_btn
    Public texture_buttons(0) As TankTexture_btn
    Public Structure TankPart_btn
        Public gl_textureID As Integer
        Public gl_OverTextureID As Integer
        Public part_ID As Integer
        Public size As Point
        Public location As Point
        Public state As Integer
        Public buttonID As Integer
        Public callmode As String
        Public selected As Boolean
        Public tankpart_id As Integer
        Dim c0 As vect4
        Dim c1 As vect4
        Dim c2 As vect4
        Dim c3 As vect4

        Public Sub click()
            tank_button_clicked(callmode)
        End Sub
        Public Sub add()
            Dim s = tankpart_buttons.Length - 1
            ReDim Preserve tankpart_buttons(s + 1)
            Me.buttonID = s + 100
            tankpart_buttons(s) = Me
        End Sub
        Public Sub draw()
            If Me.selected Then
                Gl.glColor4f(0.8, 0.4, 0.0, 1.0)
            Else
                Gl.glColor4f(0.3, 0.2, 0.1, 1.0)
            End If

            draw_quad(Me.location, Me.size)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
            'draw button state
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Me.tankpart_id)
            draw_quad(Me.location, Me.size)

            Select Case Me.state
                Case 0
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_norm)
                Case 1
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_over)
                Case 2
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_down)
            End Select
            draw_quad(Me.location, Me.size)

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glDisable(Gl.GL_BLEND)
        End Sub
        Public Sub draw_pick_box()
            'green colors
            Gl.glColor4ub(CByte(Me.buttonID), CByte(0), CByte(0), CByte(0))
            draw_quad(Me.location, Me.size)
        End Sub
    End Structure
    Public Structure TankTexture_btn
        Public name As String
        Public gl_textureID As Integer
        Public gl_OverTextureID As Integer
        Public part_ID As Integer
        Public size As Point
        Public location As Point
        Public state As Integer
        Public buttonID As Integer
        Public callmode As String
        Public selected As Boolean
        Dim c0 As vect4
        Dim c1 As vect4
        Dim c2 As vect4
        Dim c3 As vect4

        Public Sub click()
            frmMain.cur_texture_name = Path.GetFileName(Me.name)
            texture_button_clicked(gl_textureID, part_ID)
        End Sub
        Public Sub add()
            Dim s = texture_buttons.Length - 1
            ReDim Preserve texture_buttons(s + 1)
            Me.buttonID = s + 100
            texture_buttons(s) = Me
        End Sub
        Public Sub draw()
            draw_quad(Me.location, Me.size)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
            'draw button state
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Me.gl_textureID)
            draw_quad(Me.location, Me.size)

            Select Case Me.state
                Case 0
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_norm)
                Case 1
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_over)
                Case 2
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_down)
            End Select
            draw_quad(Me.location, Me.size)
            If Me.selected Then
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_selected)
                draw_quad(Me.location, Me.size)
            End If


            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glDisable(Gl.GL_BLEND)
        End Sub
        Public Sub draw_pick_box()
            'green colors
            Gl.glColor4ub(CByte(Me.buttonID), CByte(0), CByte(0), CByte(0))
            draw_quad(Me.location, Me.size)
        End Sub
    End Structure



    '#########################################################
    Public Structure Nbutton_
        Const summer As Integer = 1
        Const winter As Integer = 2
        Const dessert As Integer = 3
        Public gl_textureID As Integer
        Public gl_OverTextureID As Integer
        Public size As Point
        Public location As Point
        Public state As Integer
        Public buttonID As Integer
        Public callmode As Integer
        Public selected As Boolean
        Public button_background As Integer
        Public Sub click()
            Select Case Me.callmode
                Case 0
                    show_winter_buttons()
                Case 1
                    show_summer_buttons()
                Case 2
                    show_dessert_buttons()
            End Select
        End Sub
        Public Sub add()
            Dim s = season_Buttons.Length - 1
            ReDim Preserve season_Buttons(s + 1)
            Me.buttonID = s + 100
            season_Buttons(s) = Me
        End Sub
        Public Sub draw()

            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
            'draw button state
            If Me.button_background = 0 Then
                Select Case Me.state
                    Case 0
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, NBUTT_norm)
                    Case 1
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, NBUTT_over)
                    Case 2
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, NBUTT_down)
                End Select
            Else
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, button_background)
            End If
            draw_quad(Me.location, Me.size)


            Gl.glColor4f(0.5, 0.5, 0.5, 0.5)
            Gl.glEnable(Gl.GL_BLEND)

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Me.gl_textureID)
            draw_quad(Me.location + New Point(2, -2), Me.size + New Point(-4, -4))

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glDisable(Gl.GL_BLEND)
        End Sub
        Public Sub draw_pick_box()
            'red colors
            Gl.glColor4f(CSng(Me.buttonID), 0, 0, 1)
            Gl.glColor4ub(CByte(Me.buttonID), CByte(0), CByte(0), CByte(0))
            draw_quad(Me.location, Me.size)
        End Sub
    End Structure
    Public Structure Camobutton_
        Public gl_textureID As Integer
        Public gl_OverTextureID As Integer
        Public camouflage_ID As Integer
        Public size As Point
        Public location As Point
        Public state As Integer
        Public buttonID As Integer
        Public callmode As String
        Public selected As Boolean
        Public camo_texture_id As Integer
        Public camoName As String
        Public textureName As String
        Dim c0 As vect4
        Dim c1 As vect4
        Dim c2 As vect4
        Dim c3 As vect4
        Dim p As Point

        Public Sub click()
            apply_texture(buttonID - 100)
        End Sub
        Public Sub add()
            Dim s = camo_Buttons.Length - 1
            ReDim Preserve camo_Buttons(s + 1)
            Me.buttonID = s + 100
            camo_Buttons(s) = Me
        End Sub
        Public Sub draw()
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
            'draw button state
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, Me.gl_textureID)
            p = New Point
            p.X = Me.location.X + pan_location
            p.Y = Me.location.Y
            draw_quad(p + New Point(2, -2), Me.size + New Point(-4, -4))

            Select Case Me.state
                Case 0
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_norm)
                Case 1
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_over)
                Case 2
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, CBUTT_down)
            End Select
            Gl.glEnable(Gl.GL_BLEND)
            draw_quad(p, Me.size)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            If Me.camoName IsNot Nothing Then
                glutPrintSmall(p.X, p.Y + 5, Me.camoName.ToString, 0.0, 1.0, 0.0, 1.0)
                If Me.selected Then
                    glutPrint(10.0, p.Y + 70, Me.textureName.ToString, 0.0, 1.0, 0.0, 1.0)
                End If
            End If

            Gl.glDisable(Gl.GL_BLEND)
        End Sub
        Public Sub draw_pick_box()
            'green colors
            Gl.glColor4ub(CByte(0), CByte(Me.buttonID), CByte(0), CByte(0))
            p = New Point
            p.X = Me.location.X + pan_location
            p.Y = Me.location.Y
            draw_quad(p, Me.size)
        End Sub
    End Structure


    Private Sub draw_quad(ByVal p As Point, ByVal size As Point)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Dim w = size.X
        Dim h = size.Y

        Gl.glBegin(Gl.GL_QUADS)

        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()

    End Sub


    Public Sub test_buttons()
        Dim dumb() = {SUMMER_ICON, WINTER_ICON, DESERT_ICON}
        Dim rn As New Random
        For k = 0 To 9
            For j = 0 To 9

                Dim b As New Nbutton_
                b.location.X = (j * 57) + 10
                b.location.Y = (-k * 57) - 10
                b.size = New Point(47, 47)
                b.gl_textureID = SUMMER_ICON
                Dim idx = CInt(Floor(rn.NextDouble * 2.49))
                b.gl_textureID = dumb(idx)
                idx = CInt(Floor(rn.NextDouble * 2.49))
                b.state = idx
                b.add()
            Next
        Next


    End Sub



    Public Sub load_season_icons()

        WINTER_ICON = load_png_file(Application.StartupPath + "\resources\winter.png")
        SUMMER_ICON = load_png_file(Application.StartupPath + "\resources\summer.png")
        DESERT_ICON = load_png_file(Application.StartupPath + "\resources\desert.png")

        NBUTT_norm = load_png_file(Application.StartupPath + "\resources\Nbutt_norm.png")
        NBUTT_over = load_png_file(Application.StartupPath + "\resources\Nbutt_over.png")
        NBUTT_down = load_png_file(Application.StartupPath + "\resources\Nbutt_down.png")

        CBUTT_norm = load_png_file(Application.StartupPath + "\resources\Cbutt_norm.png")
        CBUTT_over = load_png_file(Application.StartupPath + "\resources\Cbutt_over.png")
        CBUTT_down = load_png_file(Application.StartupPath + "\resources\Cbutt_down.png")

        CBUTT_selected = load_png_file(Application.StartupPath + "\resources\Cbutt_selected.png")
        checkerboard_id = load_png_file(Application.StartupPath + "\resources\CheckerPatternPaper.png")
        white_id = load_png_file(Application.StartupPath + "\resources\white.png")

        delete_image_start += 12 ' keep track of used textures

    End Sub

    Public Sub draw_textures_pick()
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Gl.glClearColor(0, 0, 0, 0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        For i = 0 To texture_buttons.Length - 2
            texture_buttons(i).draw_pick_box()
            texture_buttons(i).state = 0
        Next
    End Sub
    Public Sub mouse_pick_textures(ByVal x As Integer, ByVal y As Integer)
        'If CAMO_BUTTON_DOWN Then Return

        '==========================================
        'pick function
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim type = pixel(3)
        Dim index As UInt32 = CUInt(pixel(0))
        If type = 0 Then
            If index > 0 Then
                BUTTON_TYPE = 1
                BUTTON_ID = index
                index = index - 100
                If CAMO_BUTTON_DOWN Then 'its global so use it.
                    texture_buttons(index).state = 2
                    TANK_TEXTURE_ID = index
                    'If Not LAST_SEASON = BUTTON_ID Then
                    For i = 0 To texture_buttons.Length - 2
                        texture_buttons(i).selected = False
                    Next
                    'stop repeteing the command over and over
                    texture_buttons(index).click()
                    texture_buttons(index).selected = True
                    'End If
                    CAMO_BUTTON_DOWN = False
                Else
                    texture_buttons(index).state = 1
                    BUTTON_ID = 0
                End If

                Application.DoEvents()
                Return
            Else
                For i = 0 To texture_buttons.Length - 2
                    texture_buttons(i).state = 0
                Next
                TANK_TEXTURE_ID = 0
                BUTTON_ID = 0

            End If

        Else
        End If
    End Sub


    Public Sub draw_tankpart_pick()
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Gl.glClearColor(0, 0, 0, 0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        For i = 0 To tankpart_buttons.Length - 2
            tankpart_buttons(i).draw_pick_box()
            tankpart_buttons(i).state = 0
        Next

    End Sub
    Public Sub mouse_pick_tankparts(ByVal x As Integer, ByVal y As Integer)
        'If CAMO_BUTTON_DOWN Then Return

        '==========================================
        'pick function
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim type = pixel(3)
        Dim index As UInt32 = CUInt(pixel(0))
        If type = 0 Then
            If index > 0 Then
                BUTTON_TYPE = 1
                BUTTON_ID = index
                index = index - 100
                If CAMO_BUTTON_DOWN Then 'its global so use it.
                    TANK_PART_ID = index
                    tankpart_buttons(index).state = 2
                    'If Not LAST_SEASON = BUTTON_ID Then
                    For i = 0 To tankpart_buttons.Length - 2
                        tankpart_buttons(i).selected = False
                    Next
                    'stop repeteing the command over and over
                    tankpart_buttons(index).click()
                    tankpart_buttons(index).selected = True
                    'End If

                Else
                    tankpart_buttons(index).state = 1
                    BUTTON_ID = 0
                End If

                Application.DoEvents()
                Return
            Else
                For i = 0 To tankpart_buttons.Length - 2
                    tankpart_buttons(i).state = 0
                Next
                TANK_PART_ID = 0
                BUTTON_ID = 0

            End If

        Else
        End If
    End Sub

    Public Sub draw_season_pick_buttons()
        'If CAMO_BUTTON_DOWN Then Return

        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Gl.glClearColor(0, 0, 0, 0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        For i = 0 To season_Buttons.Length - 2
            season_Buttons(i).draw_pick_box()
            season_Buttons(i).selected = False
            season_Buttons(i).state = 0
        Next
    End Sub
    Public Sub mouse_pick_season_button(ByVal x As Integer, ByVal y As Integer)

        'If CAMO_BUTTON_DOWN Then Return

        '==========================================
        'pick function
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim type = pixel(3)
        Dim index As UInt32 = CUInt(pixel(0))
        If type = 0 Then
            If index > 0 Then
                BUTTON_TYPE = 1
                BUTTON_ID = index
                index = index - 100
                If CAMO_BUTTON_DOWN Then
                    season_Buttons(index).state = 2
                    If Not LAST_SEASON = BUTTON_ID Then
                        CAMO_BUTTONS_VISIBLE = False
                        STOP_BUTTON_SCAN = True
                        'stop repeteing the command over and over
                        season_Buttons(index).click()
                        pan_location = 0
                    End If

                Else
                    season_Buttons(index).state = 1
                End If

                season_Buttons(index).selected = True
                Application.DoEvents()
                Return
            Else
                For i = 0 To season_Buttons.Length - 2
                    season_Buttons(i).state = 0
                Next
                BUTTON_ID = 0

            End If

        Else
        End If

    End Sub

    Public Sub draw_pick_camo_buttons()
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Gl.glClearColor(0, 0, 0, 0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        For i = 0 To camo_Buttons.Length - 2
            camo_Buttons(i).draw_pick_box()
            camo_Buttons(i).selected = False
            camo_Buttons(i).state = 0
        Next
    End Sub
    Public Sub mouse_pick_camo_button(ByVal x As Integer, ByVal y As Integer)


        '==========================================
        'pick function
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim type = pixel(3)
        Dim index As UInt32 = CUInt(pixel(1))
        If type = 0 Then
            If index > 0 Then
                BUTTON_TYPE = 2
                BUTTON_ID = index
                index = index - 100
                If CAMO_BUTTON_DOWN Then
                    camo_Buttons(index).state = 2
                    STOP_BUTTON_SCAN = True
                    camo_Buttons(index).click()
                Else
                    camo_Buttons(index).state = 1
                End If


                camo_Buttons(index).selected = True
                Application.DoEvents()
                Return
            Else
                For i = 0 To camo_Buttons.Length - 2
                    camo_Buttons(i).state = 0
                Next
                BUTTON_ID = 0

            End If

        Else
        End If

    End Sub

End Module
