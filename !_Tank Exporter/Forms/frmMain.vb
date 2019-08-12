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
Imports System.IO.Compression
#End Region

Public Class frmMain
    Protected Overrides Sub OnClientSizeChanged(e As EventArgs)
        If Not _Started Then Return
        If Not allow_mouse Then Return
        G_Buffer.init()
        draw_scene()
        MyBase.OnClientSizeChanged(e)
    End Sub
#Region "variables"
    Private allow_mouse As Boolean = False
    Public cur_texture_name As String = ""
    Dim pb2_has_focus As Boolean = False
    Dim out_string As New StringBuilder
    Public Background_image_id As Integer
    Private window_state As Integer
    Public Show_lights As Boolean = False
    Public gl_stop As Boolean = False
    Public update_thread As New Thread(AddressOf update_mouse)
    Public pan_thread As New Thread(AddressOf update_pan)
    Public path_set As Boolean = False
    Public res_mods_path_set As Boolean = False
    Dim mouse As vec2
    Private mouse_down As Boolean = False
    Public mouse_delta As New Point
    Private mouse_pos As New Point
    Public mouse_find_location As New Point
    Public found_triangle_tv As Integer
    Private TOTAL_TANKS_FOUND As Integer = 0
    Private itemDefXmlString As String = ""
    Private itemDefPathString As String = ""
    Private sorted As Boolean

    Dim delay As Integer = 0
    Dim stepper As Integer = 0

    Public Shared packages(12) As Ionic.Zip.ZipFile
    Public Shared packages_2(12) As Ionic.Zip.ZipFile
    Public Shared packages_HD(12) As Ionic.Zip.ZipFile
    Public Shared packages_HD_2(12) As Ionic.Zip.ZipFile
    Public Shared shared_pkg As Ionic.Zip.ZipFile
    Public Shared shared_sandbox_pkg As Ionic.Zip.ZipFile
    Public shared_contents_build As New Ionic.Zip.ZipFile
    Public gui_pkg As Ionic.Zip.ZipFile
    Public scripts_pkg As Ionic.Zip.ZipFile
    Dim treeviews(10) As TreeView
    Public icons(10) As pngs
    Public view_status_string As String
    Public tank_mini_icons As New ImageList
    Public GMM_R, GMM_B As Single
    Public GMM_TOY_VISIBLE As Integer = 0
    Dim time As New Stopwatch
    Dim pick_timer As New Stopwatch


    Structure pngs
        Public img() As entry_

    End Structure
    Public Class entry_ : Implements IComparable(Of entry_)
        Public img As System.Drawing.Bitmap
        Public name As String
        Public short_name As String
        Public Function CompareTo(other As entry_) As Integer Implements IComparable(Of entry_).CompareTo
            If other Is Nothing Then Return 1
            Return name.CompareTo(other.name)
        End Function
    End Class
    Public node_list(10) As t_array
    Public Structure t_array
        Public item() As t_items_
    End Structure
    Public Structure t_items_
        Public name As String
        Public node As TreeNode
        Public package As String
        Public icon As System.Drawing.Bitmap
    End Structure
    Private strings(3000) As String
    Dim TreeView1 As New TreeView
    Dim TreeView2 As New TreeView
    Dim TreeView3 As New TreeView
    Dim TreeView4 As New TreeView
    Dim TreeView5 As New TreeView
    Dim TreeView6 As New TreeView
    Dim TreeView7 As New TreeView
    Dim TreeView8 As New TreeView
    Dim TreeView9 As New TreeView
    Dim TreeView10 As New TreeView
    Dim spin_light As Boolean = False
#End Region



    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        _Started = False
        Try
            update_thread.Resume()
            While update_thread.IsAlive
                _Started = False
                update_thread.Abort()
            End While
            'delete any data we created
            For i = 1 To 10
                packages(i).Dispose()
            Next
            DisableOpenGL()
            gui_pkg.Dispose()
            scripts_pkg.Dispose()
            'shared_pkg.Dispose()
            'shared_sandbox_pkg.Dispose()
        Catch ex As Exception

        End Try
        Try
            For i = 1 To 10
                packages_HD(i).Dispose()
            Next
        Catch ex As Exception

        End Try
        '--------------------------------------------------------
        '--------------------------------------------------------

    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim tab = TC1.SelectedTab
        Dim c = tab.Controls
        Try

            Dim t = DirectCast(c(0), TreeView)
            t.SelectedNode = Nothing
            t.Parent.Focus()
        Catch ex As Exception

        End Try
        '============================================================================================
        'UI short cut keys
        Select Case e.KeyValue

            Case Keys.F1
                m_help.PerformClick()

            Case Keys.F2
                If Show_lights Then
                    Show_lights = False
                Else
                    Show_lights = True
                End If

            Case Keys.F3
                If spin_light Then
                    spin_light = False
                Else
                    spin_light = True
                End If

            Case Keys.F5
                If chassis_cb.Checked Then
                    chassis_cb.Checked = False
                Else
                    chassis_cb.Checked = True
                End If

            Case Keys.F6
                If hull_cb.Checked Then
                    hull_cb.Checked = False
                Else
                    hull_cb.Checked = True
                End If

            Case Keys.F7
                If turret_cb.Checked Then
                    turret_cb.Checked = False
                Else
                    turret_cb.Checked = True
                End If

            Case Keys.F8
                If gun_cb.Checked Then
                    gun_cb.Checked = False
                Else
                    gun_cb.Checked = True
                End If

            Case Keys.F12
                If MODEL_LOADED Then
                    m_screen_cap.PerformClick()
                End If
                '=================
            Case Keys.B
                If grid_cb.CheckState = CheckState.Checked Then
                    grid_cb.CheckState = CheckState.Indeterminate
                ElseIf grid_cb.CheckState = CheckState.Indeterminate Then
                    grid_cb.CheckState = CheckState.Unchecked
                ElseIf grid_cb.CheckState = CheckState.Unchecked Then
                    grid_cb.CheckState = CheckState.Checked
                End If

            Case Keys.C
                If CENTER_SELECTION Then
                    CENTER_SELECTION = False
                Else
                    CENTER_SELECTION = True
                End If
                pb1.Focus()

            Case Keys.F
                If MODEL_LOADED Then
                    m_view_res_mods_folder.PerformClick()
                End If

            Case Keys.H
                m_help.PerformClick()

            Case Keys.Home
                reset_view()

            Case Keys.I
                If Not My.Computer.Keyboard.ShiftKeyDown Then
                    m_Import_FBX.PerformClick()
                ElseIf My.Computer.Keyboard.ShiftKeyDown Then
                    m_import_primitives_fbx.PerformClick()
                    move_mod = False
                    GoTo done
                End If

            Case Keys.L
                m_lighting.PerformClick()

            Case Keys.M
                If showMarkers_cb.Checked Then
                    showMarkers_cb.Checked = False
                Else
                    showMarkers_cb.Checked = True
                End If
            Case Keys.N
                normal_shader_mode += 1
                If normal_shader_mode > 2 Then
                    normal_shader_mode = 0
                End If

            Case Keys.T
                m_load_textures.PerformClick()

            Case Keys.U
                If frmTextureViewer.Visible Then
                    frmTextureViewer.m_show_uvs.PerformClick()
                End If

            Case Keys.V
                frmComponentView.Visible = True

            Case Keys.W
                If wire_cb.Checked Then
                    wire_cb.Checked = False
                Else
                    wire_cb.Checked = True
                End If

            Case Keys.X
                If MODEL_LOADED Then
                    m_export_to_fbx.PerformClick()
                End If

        End Select
        '============================================================================================

        If My.Computer.Keyboard.ShiftKeyDown Then
            move_mod = True
        End If

        If My.Computer.Keyboard.CtrlKeyDown Then
            z_move = True
        End If

        If CAMO_BUTTONS_VISIBLE Then
            If e.KeyCode = Keys.Right Then
                'Debug.WriteLine("Right")
                If camo_Buttons(camo_Buttons.Length - 2).location.X + pan_location > pb1.Width - 100 And Not pan_left Then
                    pan_slide = 0
                    pan_left = True
                End If
                Me.Focus()
                pb1.Focus()
            End If
            If e.KeyCode = Keys.Left Then
                'Debug.WriteLine("Left")
                If camo_Buttons(0).location.X + pan_location < 0 And Not pan_right Then
                    pan_slide = 0
                    pan_right = True
                    Me.Focus()
                    pb1.Focus()
                End If
            End If
        End If

done:
        e.Handled = True
        If stop_updating Then draw_scene()

    End Sub

    Private Sub frmMain_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If move_mod Then
            move_mod = False
        End If
        If z_move Then
            z_move = False
        End If
        If stop_updating Then draw_scene()

    End Sub

    Private Sub frmMain_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        'gl_stop = True
        If Not _Started Then Return
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        OLD_WINDOW_HEIGHT = pb1.Height
        w_changing = True
    End Sub


    Private Sub frmMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        If Not allow_mouse Then Return
        WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
        relocate_season_Bottons()
        relocate_camobuttons()
        relocate_tankbuttons()
        relocate_texturebuttons()

        Dim wm = pb1.Width Mod 2
        Dim hm = pb1.Height Mod 2

        Me.Width += wm
        Me.Height += hm
        If Not _Started Then Return
        If stop_updating Then draw_scene()
        w_changing = False
    End Sub

    Private Sub me_size_changed()
        If Not _Started Then Return
        If Not allow_mouse Then Return
        If window_state <> Me.WindowState Then
            If Not Me.WindowState = FormWindowState.Minimized Then
                'gl_stop = True
                'If season_Buttons_VISIBLE Then
                WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
                relocate_season_Bottons()
                relocate_camobuttons()
                relocate_tankbuttons()
                relocate_texturebuttons()
                OLD_WINDOW_HEIGHT = pb1.Height
                'End If
                w_changing = False
                window_state = Me.WindowState
                If Not _Started Then Return
                If stop_updating Then draw_scene()
                Return
                gl_stop = False
            End If
            window_state = Me.WindowState
        End If
        'If season_Buttons_VISIBLE Then
        WINDOW_HEIGHT_DELTA = pb1.Height - OLD_WINDOW_HEIGHT
        relocate_season_Bottons()
        relocate_camobuttons()
        relocate_tankbuttons()
        relocate_texturebuttons()
        OLD_WINDOW_HEIGHT = pb1.Height
        'End If
        G_Buffer.init()
        draw_scene()
    End Sub


    '############################################################################ form load
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        My.Settings.Upgrade() ' upgrades to keep old settings

        Dim nonInvariantCulture As System.Globalization.CultureInfo = New CultureInfo("en-US")
        nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
        System.Threading.Thread.CurrentThread.CurrentCulture = nonInvariantCulture

        Dim x, z As Single
        x = 2.846
        z = 9.586



        tank_label.Text = ""
        SplitContainer1.SplitterDistance = 720
        SplitContainer2.SplitterDistance = SplitContainer2.Height - 160
        Application.DoEvents()
        Me.Width = 1280
        Me.Height = 720
        pb1.Visible = False
        iconbox.Visible = False
        Application.DoEvents()
        pb1.Visible = True
        frmState = Me.WindowState
        info_Label.BringToFront()
        info_Label.Parent = Me
        info_Label.Size = MM.Size
        info_Label.Dock = DockStyle.Top
        MM.Location = New Point(0, 0)
        info_Label.Text = "Welcome... Version: " + Application.ProductVersion
        Me.Text = " Tank Exporter Version: " + Application.ProductVersion
        '====================================================================================================
        start_up_log.AppendLine("------ App Startup ------")
        '====================================================================================================
        SplitContainer2.Panel2.Controls.Add(tanklist)
        tanklist.Visible = False
        'tanklist.ScrollBars = ScrollBars.Vertical
        tanklist.Multiline = True
        tanklist.ForeColor = Color.White
        tanklist.BackColor = SplitContainer2.Panel2.BackColor
        tanklist.Dock = DockStyle.Fill
        tanklist.BringToFront()

        'tanklist.BackColor = iconbox.BackColor
        tanklist.Font = TreeView1.Font
        tanklist.SendToBack()
        Me.Show()
        PB3.Parent = Me
        PB3.SendToBack()
        PB3.Visible = False
        ToolStripComboBox1.Visible = False
        ToolStripComboBox1.Text = My.Settings.region_selection
        decal_panel.Parent = SplitContainer2.Panel1
        decal_panel.Visible = False
        Application.DoEvents()

        'Check for temp storage folder.. It it exist.. load the API data.. 
        'other wise make the directory and get the API data.
        Temp_Storage = Path.GetTempPath ' this gets the user temp storage folder
        Temp_Storage += "wot_temp"
        If Not System.IO.Directory.Exists(Temp_Storage) Then
            System.IO.Directory.CreateDirectory(Temp_Storage)
        End If
        'fire up OpenGL amd IL
        start_up_log.AppendLine("Starting up OpenGL......")
        Il.ilInit()
        Ilu.iluInit()
        Ilut.ilutInit()
        EnableOpenGL()
        make_shadow_fbo()
        pb1.Visible = False
        '---------------------------------------------------------------------------------------------------------------------
        m_export_tank_list.Visible = False
        m_load_file.Visible = False
        m_save.Visible = False
        m_clear_selected_tanks.Visible = False
        m_build_wotmod.Enabled = False
        m_hide_show_components.Enabled = False
        m_set_vertex_winding_order.Enabled = False
        '---------------------------------------------------------------------------------------------------------------------
        'just to convert to .te binary models;
        'load_and_save()

        Dim glstr As String
        glstr = Gl.glGetString(Gl.GL_VENDOR)
        start_up_log.AppendLine("Vendor: " + glstr)

        glstr = Gl.glGetString(Gl.GL_VERSION)
        start_up_log.AppendLine("Driver Version: " + glstr)

        glstr = Gl.glGetString(Gl.GL_SHADING_LANGUAGE_VERSION)
        start_up_log.AppendLine("Shader Version: " + glstr)

        'glstr = Gl.glGetString(Gl.GL_EXTENSIONS).Replace(" ", vbCrLf)
        'start_up_log.AppendLine("Extensions:" + vbCrLf + glstr)
        start_up_log.AppendLine("End OpenGL Information" + vbCrLf)
        start_up_log.AppendLine("OpenGL Startup Complete" + vbCrLf)

        start_up_log.AppendLine("Loading required data..")

        File.WriteAllText(Temp_Storage + "Startup_log.txt", start_up_log.ToString)

        load_type_images() ' get the tank type icons

        Application.DoEvents()
        '====================================================================================================
        _Started = True
        '====================================================================================================
        ' Setup loaction for tank data.. sucks to do it this way but UAC wont allow it any other way.
        'I'M SAVING ALL CODE RELATED TO THE OLD TANK LIST IN CASE I WORK ON TERRA AGAIN!
        TankListTempFolder = Temp_Storage + "\tanklist\"
        decal_path = Temp_Storage + "\decals"

        If Not System.IO.Directory.Exists(TankListTempFolder) Then
            System.IO.Directory.CreateDirectory(TankListTempFolder)
        End If
        If My.Settings.firstRun Then ' check for possible update to tank list.
            My.Settings.firstRun = False
            Dim ts = IO.File.ReadAllText(Application.StartupPath + "\tanks\tanknames.txt")
            File.WriteAllText(TankListTempFolder + "tanknames.txt", ts)
        End If
        '====================================================================================================
        If File.Exists(Temp_Storage + "\game_Path.txt") Then
            My.Settings.game_path = File.ReadAllText(Temp_Storage + "\game_Path.txt")
        Else
            If My.Settings.game_path = "" Then
                MsgBox("Game Location needs to be set.", MsgBoxStyle.Information)
                M_Path.PerformClick()
            End If
        End If
        If File.Exists(Temp_Storage + "\res_mods_path.txt") Then
            My.Settings.res_mods_path = File.ReadAllText(Temp_Storage + "\res_mods_Path.txt")
        Else
            If My.Settings.game_path = "C:\" Then
                MsgBox("res_mods Location needs to be set.", MsgBoxStyle.Information)
                m_res_mods_path.PerformClick()
            End If
        End If
        '====================================================================================================
        'find out if our res_mods path is out of data!
        Dim pathsxml = File.ReadAllText(My.Settings.game_path + "\paths.xml")
        Dim ar = pathsxml.Split(vbLf)
        pathsxml = ar(2).Replace(vbTab, "")
        pathsxml = pathsxml.Replace(" ", "")
        pathsxml = pathsxml.Replace("""", "")
        pathsxml = pathsxml.Replace("<PathcacheSubdirs=true>./res_mods/", "")
        pathsxml = pathsxml.Replace("</Path>", "")
        Dim rp = Path.GetFileName(My.Settings.res_mods_path)
        If rp <> pathsxml Then
            If File.Exists(My.Settings.game_path + "\paths_backup.xml") Then
                File.Delete(My.Settings.game_path + "\paths_backup.xml")
            End If
            If MsgBox("The game has updated to version: " + pathsxml + vbCrLf + "You need update the res_mods Path.", MsgBoxStyle.YesNo, "Game Update!") = MsgBoxResult.Yes Then
                m_res_mods_path.PerformClick()
            End If
        End If
        '====================================================================================================

        Dim testing_controls As Boolean = False
        If Not testing_controls Then

            info_Label.Text = "Loading Data from Packages..."
            Application.DoEvents()
            MM.Enabled = False ' Dont let the user click anything while we are loading data!
            TC1.Enabled = False
            Try

                gui_pkg = New Ionic.Zip.ZipFile(My.Settings.game_path + "\res\packages\gui.pkg")
                start_up_log.AppendLine("Loaded: " + My.Settings.game_path + "\res\packages\gui.pkg")

                scripts_pkg = New Ionic.Zip.ZipFile(My.Settings.game_path + "\res\packages\scripts.pkg")
                start_up_log.AppendLine("Loaded: " + My.Settings.game_path + "\res\packages\scripts.pkg")
                'packages(11) = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content.pkg")
                'packages(12) = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox.pkg")
                'packages(11) = shared_pkg
                'packages(12) = shared_sandbox_pkg

            Catch ex As Exception
                MsgBox("I was unable to load required pkg files! Path Issue?", MsgBoxStyle.Exclamation, "Error!")
                My.Settings.game_path = ""
                My.Settings.res_mods_path = ""
                My.Settings.Save()
                End
            End Try
            '====================================================================================================
            'MsgBox("I LOADED required pkg files!", MsgBoxStyle.Exclamation, "Error!")
            'Try
            If File.Exists(Temp_Storage + "\shared_contents_build.pkg") Then
                packages(11) = Ionic.Zip.ZipFile.Read(Temp_Storage + "\shared_contents_build.pkg")
                start_up_log.AppendLine("Loaded: " + Temp_Storage + "\shared_contents_build.pkg")
            Else
                '===================================================================================
                start_up_log.AppendLine("Finding all PBS decals in map pak files...")
                info_Label.Text = "finding Decals. This only happens once after install."

                start_up_log.AppendLine("Done Finding all PBS decals in map packages.")
                find_pbs_decals()
                '===================================================================================

                shared_contents_build = New Ionic.Zip.ZipFile(Temp_Storage + "\shared_contents_build.pkg")
                start_up_log.AppendLine("shared_contents_build.pkg does not exist. Building shared_contents_build.pkg")
                start_up_log.AppendLine("Only Entries that contain Vehicle will be read.")
                'add handler for progression call back to display progressbar value
                AddHandler (shared_contents_build.SaveProgress), New EventHandler(Of SaveProgressEventArgs)(AddressOf save_progress)

                info_Label.Text = "Reading all shared content packages. This only needs to be done once."
                Application.DoEvents()
                Application.DoEvents()

                Dim z_path = Temp_Storage + "\zip"
                IO.Directory.CreateDirectory(z_path)
                info_Label.Text = "Reading shared_content-part1.pkg"
                Application.DoEvents()
                IO.Directory.CreateDirectory(decal_path)
                '================================================================================
                'part 1
                PG1.Visible = True
                PG1.Value = 0
                Dim cnt = 0
                Dim arc = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content-part1.pkg")
                PG1.Maximum = arc.Count
                start_up_log.AppendLine("reading: \res\packages\shared_content-part1.pkg")

                For Each entry In arc
                    PG1.Value = cnt
                    cnt += 1
                    If entry.FileName.ToLower.Contains("vehicle") Then
                        entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                        Application.DoEvents()
                    End If
                Next
                cnt = 0
                Try
                    info_Label.Text = "getting decals from shared_content-part1.pkg"
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("decals_pbs") Then
                            entry.Extract(decal_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                End Try
                Try
                    info_Label.Text = "Reading shared_content_hd-part1.pkg"
                    Application.DoEvents()
                    arc = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_hd-part1.pkg")
                    PG1.Value = 0
                    PG1.Maximum = arc.Count
                    cnt = 0
                    start_up_log.AppendLine("reading: \res\packages\shared_content_hd-part1.pkg")
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("vehicle") Then
                            entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                    start_up_log.AppendLine("Could not find: \res\packages\shared_content-part1.pkg")
                End Try
                '================================================================================
                'part 2
                info_Label.Text = "Reading shared_content-part2.pkg"
                Application.DoEvents()
                arc = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content-part2.pkg")
                PG1.Value = 0
                PG1.Maximum = arc.Count
                cnt = 0
                start_up_log.AppendLine("reading: \res\packages\shared_content-part2.pkg")
                For Each entry In arc
                    PG1.Value = cnt
                    cnt += 1
                    If entry.FileName.ToLower.Contains("vehicle") Then
                        entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                        Application.DoEvents()
                    End If
                Next
                cnt = 0
                Try
                    info_Label.Text = "getting decals from shared_content_hd-part2.pkg"
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("decals_pbs") Then
                            entry.Extract(decal_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                End Try
                Try
                    info_Label.Text = "Reading shared_content_hd-part2.pkg"
                    Application.DoEvents()
                    arc = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_hd-part2.pkg")
                    PG1.Value = 0
                    PG1.Maximum = arc.Count
                    cnt = 0
                    start_up_log.AppendLine("reading: \res\packages\shared_content_hd-part2.pkg")
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("vehicle") Then
                            entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                    start_up_log.AppendLine("Could not find: \res\packages\shared_content-part2.pkg")
                End Try
                '================================================================================
                'part 1
                info_Label.Text = "Reading shared_content_sandbox-part1.pkg"
                Application.DoEvents()
                arc = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox-part1.pkg")
                PG1.Value = 0
                PG1.Maximum = arc.Count
                cnt = 0
                start_up_log.AppendLine("reading: \res\packages\shared_content_sandbox-part1.pkg")
                For Each entry In arc
                    PG1.Value = cnt
                    cnt += 1
                    If entry.FileName.ToLower.Contains("vehicle") Then
                        entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                        Application.DoEvents()
                    End If
                Next
                Try
                    info_Label.Text = "Reading shared_content_sandbox_hd-part1.pkg"
                    Application.DoEvents()
                    arc = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox_hd-part1.pkg")
                    PG1.Value = 0
                    PG1.Maximum = arc.Count
                    cnt = 0
                    start_up_log.AppendLine("reading: \res\packages\shared_content_sandbox_hd-part1.pkg")
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("vehicle") Then
                            entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                    start_up_log.AppendLine("Could not find: \res\packages\shared_content_sandbox_hd-part1.pkg")
                End Try
                '================================================================================
                'part 2
                info_Label.Text = "Reading shared_content_sandbox-part2.pkg"
                Application.DoEvents()
                arc = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox-part2.pkg")
                PG1.Value = 0
                PG1.Maximum = arc.Count
                cnt = 0
                start_up_log.AppendLine("reading: \res\packages\shared_content_sandbox-part2.pkg")
                For Each entry In arc
                    PG1.Value = cnt
                    cnt += 1
                    If entry.FileName.ToLower.Contains("vehicle") Then
                        entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                        Application.DoEvents()
                    End If
                Next
                Try
                    info_Label.Text = "Reading shared_content_sandbox_hd-part2.pkg"
                    Application.DoEvents()
                    arc = Ionic.Zip.ZipFile.Read(My.Settings.game_path + "\res\packages\shared_content_sandbox_hd-part2.pkg")
                    PG1.Value = 0
                    PG1.Maximum = arc.Count
                    cnt = 0
                    start_up_log.AppendLine("reading: \res\packages\shared_content_sandbox_hd-part2.pkg")
                    For Each entry In arc
                        PG1.Value = cnt
                        cnt += 1
                        If entry.FileName.ToLower.Contains("vehicle") Then
                            entry.Extract(z_path, ExtractExistingFileAction.OverwriteSilently)
                            Application.DoEvents()
                        End If
                    Next
                Catch ex As Exception
                    start_up_log.AppendLine("Could not find: \res\packages\shared_content_sandbox_hd-part2.pkg")
                End Try
                '================================================================================

                shared_contents_build.AddDirectory(z_path)

                GC.Collect()
                GC.WaitForFullGCComplete()
                shared_contents_build.CompressionLevel = 0 ' no compression
                shared_contents_build.ParallelDeflateThreshold = 0
                info_Label.Text = "Saving " + shared_contents_build.Entries.Count.ToString + " files to shared_contents_build.pkg.. This will take a long time!"
                start_up_log.AppendLine("Saving: " + Temp_Storage + "\shared_contents_build.pkg")
                Application.DoEvents()
                shared_contents_build.Save()
                packages(11) = New Ionic.Zip.ZipFile
                packages(11) = shared_contents_build ' save this in to 11th position
            End If
            'Catch ex As Exception
            '    start_up_log.AppendLine("Something went very wrong creating the shared_contents_build.pkg!")
            'End Try
            screen_totaled_draw_time = 1 ' to stop divide by zero exception
            If Directory.Exists(Temp_Storage + "\zip") Then
                System.IO.Directory.Delete(Temp_Storage + "\zip", True)
            End If
            Application.DoEvents()
            '====================================================================================================
            '====================================================================================================
            tank_label.Parent = iconbox
            tank_label.Text = ""
            tank_label.Location = New Point(5, 10)
            '===================================================================================
            info_Label.Text = "Getting Camo Textures..."
            'load_camo()
            '===================================================================================
            load_customization_files()
            build_list_table()
            'MsgBox("Past load_customization_files", MsgBoxStyle.Exclamation, "Debug")
            load_season_icons()
            load_tank_buttons()
            start_up_log.AppendLine("Done Creating OpenGL based Buttons.")

            Gl.glFinish()
            '===================================================================================

            If Not File.Exists(Temp_Storage + "\in_shortnames.txt") Then
                start_up_log.AppendLine("Getting DEV API data.")
                get_tank_names()
            Else
                get_tank_info_from_temp_folder()
                start_up_log.AppendLine("Data already read from DEV API.. Loaded it..")
            End If

            Application.DoEvents()
            set_treeview(TreeView1)
            Application.DoEvents()
            set_treeview(TreeView2)
            Application.DoEvents()
            set_treeview(TreeView3)
            Application.DoEvents()
            set_treeview(TreeView4)
            Application.DoEvents()
            set_treeview(TreeView5)
            Application.DoEvents()
            set_treeview(TreeView6)
            Application.DoEvents()
            set_treeview(TreeView7)
            Application.DoEvents()
            set_treeview(TreeView8)
            Application.DoEvents()
            set_treeview(TreeView9)
            Application.DoEvents()
            set_treeview(TreeView10)
            '-----------------------------
            Application.DoEvents()
            treeviews(1) = TreeView1
            Application.DoEvents()
            treeviews(2) = TreeView2
            Application.DoEvents()
            treeviews(3) = TreeView3
            Application.DoEvents()
            treeviews(4) = TreeView4
            Application.DoEvents()
            treeviews(5) = TreeView5
            Application.DoEvents()
            treeviews(6) = TreeView6
            Application.DoEvents()
            treeviews(7) = TreeView7
            Application.DoEvents()
            treeviews(8) = TreeView8
            Application.DoEvents()
            treeviews(9) = TreeView9
            Application.DoEvents()
            treeviews(10) = TreeView10
            '-----------------------------
            Application.DoEvents()
            load_tabs()
            Application.DoEvents()
        End If

        'build pkg list
        make_pgk_searh_list()

        TC1.SelectedIndex = 0
        Application.DoEvents()
        make_shaders() 'compile the shaders
        set_shader_variables() ' update uniform addresses
        TC1.SelectedIndex = 0
        'clean up the grabage
        GC.Collect()
        GC.WaitForFullGCComplete()
        Me.KeyPreview = True    'so i catch keyboard before despatching it
        w_changing = False

        'make table used for repacking 888 normal uint32 in converstion
        make_888_lookup_table()
        'load skybox model

        make_xy_grid()
        start_up_log.AppendLine("Done Creating XY Grid Display List.")

        '===================================================================================
        info_Label.Text = "loading terrain, textures, creating shadow texture, ect..."
        Application.DoEvents()
        load_resources()
        info_Label.Visible = False
        '###################################
        start_up_log.AppendLine("----- Startup Complete -----")
        File.WriteAllText(Temp_Storage + "Startup_log.txt", start_up_log.ToString)
        'show and hide to assign setting and initilize the windows
        FrmShadowSettings.Show() ' set the buttons and shadow quality
        FrmShadowSettings.Hide()
        frmLighting.Show()
        frmLighting.Hide()
        frmLightSelection.Show()
        frmLightSelection.Hide()
        frmComponents.Show()
        frmComponents.Hide()
        '###################################
        MM.Enabled = True
        TC1.Enabled = True
        '###################################
        pick_timer.Start()

        FOV = My.Settings.fov
        mouse_speed_global = My.Settings.mouse_speed
        cam_x = 0
        cam_y = 0
        cam_z = 10
        look_point_x = 0
        look_point_y = 0
        look_point_z = 0
        Cam_X_angle = (PI * 0.25) + PI
        Cam_Y_angle = -PI * 0.25
        view_radius = -8.5
        l_rot = PI * 0.25 + PI * 2
        pb1.Visible = True
        G_Buffer.init()

        Startup_Timer.Enabled = True
        Application.DoEvents()
        AddHandler Me.SizeChanged, AddressOf me_size_changed
        window_state = Me.WindowState

        allow_mouse = True
    End Sub
    '############################################################################ form load
    Private Sub reset_view()
        cam_x = 0
        cam_y = 0
        cam_z = 10
        look_point_x = 0
        look_point_y = 0
        look_point_z = 0
        Cam_X_angle = (PI * 0.25) + PI
        Cam_Y_angle = -PI * 0.25
        view_radius = -8.5
        l_rot = PI * 0.25 + PI * 2

    End Sub
    Private Sub make_888_lookup_table()
        lookup(0) = 254
        lookup(1) = 253
        lookup(2) = 252
        lookup(3) = 251
        lookup(4) = 250
        lookup(5) = 249
        lookup(6) = 248
        lookup(7) = 247
        lookup(8) = 246
        lookup(9) = 245
        lookup(10) = 244
        lookup(11) = 243
        lookup(12) = 242
        lookup(13) = 241
        lookup(14) = 240
        lookup(15) = 239
        lookup(16) = 238
        lookup(17) = 237
        lookup(18) = 236
        lookup(19) = 235
        lookup(20) = 234
        lookup(21) = 233
        lookup(22) = 232
        lookup(23) = 231
        lookup(24) = 230
        lookup(25) = 229
        lookup(26) = 228
        lookup(27) = 227
        lookup(28) = 226
        lookup(29) = 225
        lookup(30) = 224
        lookup(31) = 223
        lookup(32) = 222
        lookup(33) = 221
        lookup(34) = 220
        lookup(35) = 219
        lookup(36) = 218
        lookup(37) = 217
        lookup(38) = 216
        lookup(39) = 215
        lookup(40) = 214
        lookup(41) = 213
        lookup(42) = 212
        lookup(43) = 211
        lookup(44) = 210
        lookup(45) = 209
        lookup(46) = 208
        lookup(47) = 207
        lookup(48) = 206
        lookup(49) = 205
        lookup(50) = 204
        lookup(51) = 203
        lookup(52) = 202
        lookup(53) = 201
        lookup(54) = 200
        lookup(55) = 199
        lookup(56) = 198
        lookup(57) = 197
        lookup(58) = 196
        lookup(59) = 195
        lookup(60) = 194
        lookup(61) = 193
        lookup(62) = 192
        lookup(63) = 191
        lookup(64) = 190
        lookup(65) = 189
        lookup(66) = 188
        lookup(67) = 187
        lookup(68) = 186
        lookup(69) = 185
        lookup(70) = 184
        lookup(71) = 183
        lookup(72) = 182
        lookup(73) = 181
        lookup(74) = 180
        lookup(75) = 179
        lookup(76) = 178
        lookup(77) = 177
        lookup(78) = 176
        lookup(79) = 175
        lookup(80) = 174
        lookup(81) = 173
        lookup(82) = 172
        lookup(83) = 171
        lookup(84) = 170
        lookup(85) = 169
        lookup(86) = 168
        lookup(87) = 167
        lookup(88) = 166
        lookup(89) = 165
        lookup(90) = 164
        lookup(91) = 163
        lookup(92) = 162
        lookup(93) = 161
        lookup(94) = 160
        lookup(95) = 159
        lookup(96) = 158
        lookup(97) = 157
        lookup(98) = 156
        lookup(99) = 155
        lookup(100) = 154
        lookup(101) = 153
        lookup(102) = 152
        lookup(103) = 151
        lookup(104) = 150
        lookup(105) = 149
        lookup(106) = 148
        lookup(107) = 147
        lookup(108) = 146
        lookup(109) = 145
        lookup(110) = 144
        lookup(111) = 143
        lookup(112) = 142
        lookup(113) = 141
        lookup(114) = 140
        lookup(115) = 139
        lookup(116) = 138
        lookup(117) = 137
        lookup(118) = 136
        lookup(119) = 135
        lookup(120) = 134
        lookup(121) = 133
        lookup(122) = 132
        lookup(123) = 131
        lookup(124) = 130
        lookup(125) = 129
        lookup(126) = 128
        lookup(127) = 127
        lookup(128) = 126
        lookup(129) = 125
        lookup(130) = 124
        lookup(131) = 123
        lookup(132) = 122
        lookup(133) = 121
        lookup(134) = 120
        lookup(135) = 119
        lookup(136) = 118
        lookup(137) = 117
        lookup(138) = 116
        lookup(139) = 115
        lookup(140) = 114
        lookup(141) = 113
        lookup(142) = 112
        lookup(143) = 111
        lookup(144) = 110
        lookup(145) = 109
        lookup(146) = 108
        lookup(147) = 107
        lookup(148) = 106
        lookup(149) = 105
        lookup(150) = 104
        lookup(151) = 103
        lookup(152) = 102
        lookup(153) = 101
        lookup(154) = 100
        lookup(155) = 99
        lookup(156) = 98
        lookup(157) = 97
        lookup(158) = 96
        lookup(159) = 95
        lookup(160) = 94
        lookup(161) = 93
        lookup(162) = 92
        lookup(163) = 91
        lookup(164) = 90
        lookup(165) = 89
        lookup(166) = 88
        lookup(167) = 87
        lookup(168) = 86
        lookup(169) = 85
        lookup(170) = 84
        lookup(171) = 83
        lookup(172) = 82
        lookup(173) = 81
        lookup(174) = 80
        lookup(175) = 79
        lookup(176) = 78
        lookup(177) = 77
        lookup(178) = 76
        lookup(179) = 75
        lookup(180) = 74
        lookup(181) = 73
        lookup(182) = 72
        lookup(183) = 71
        lookup(184) = 70
        lookup(185) = 69
        lookup(186) = 68
        lookup(187) = 67
        lookup(188) = 66
        lookup(189) = 65
        lookup(190) = 64
        lookup(191) = 63
        lookup(192) = 62
        lookup(193) = 61
        lookup(194) = 60
        lookup(195) = 59
        lookup(196) = 58
        lookup(197) = 57
        lookup(198) = 56
        lookup(199) = 55
        lookup(200) = 54
        lookup(201) = 53
        lookup(202) = 52
        lookup(203) = 51
        lookup(204) = 50
        lookup(205) = 49
        lookup(206) = 48
        lookup(207) = 47
        lookup(208) = 46
        lookup(209) = 45
        lookup(210) = 44
        lookup(211) = 43
        lookup(212) = 42
        lookup(213) = 41
        lookup(214) = 40
        lookup(215) = 39
        lookup(216) = 38
        lookup(217) = 37
        lookup(218) = 36
        lookup(219) = 35
        lookup(220) = 34
        lookup(221) = 33
        lookup(222) = 32
        lookup(223) = 31
        lookup(224) = 30
        lookup(225) = 29
        lookup(226) = 28
        lookup(227) = 27
        lookup(228) = 26
        lookup(229) = 25
        lookup(230) = 24
        lookup(231) = 23
        lookup(232) = 22
        lookup(233) = 21
        lookup(234) = 20
        lookup(235) = 19
        lookup(236) = 18
        lookup(237) = 17
        lookup(238) = 16
        lookup(239) = 15
        lookup(240) = 14
        lookup(241) = 13
        lookup(242) = 12
        lookup(243) = 11
        lookup(244) = 10
        lookup(245) = 9
        lookup(246) = 8
        lookup(247) = 7
        lookup(248) = 6
        lookup(249) = 5
        lookup(250) = 4
        lookup(251) = 3
        lookup(252) = 2
        lookup(253) = 1
        lookup(254) = 0
        lookup(255) = 0

    End Sub
    Private Sub find_pbs_decals()
        Dim iPath = My.Settings.game_path + "\res\packages\"
        Dim f_info = Directory.GetFiles(iPath)
        Dim maps(100) As String
        Dim cnt As Integer = 0
        'first, lets get a list of all the map files.
        For Each m In f_info
            If Not m.Contains("_hd") And Not m.Contains("vehicles_") Then
                Dim s = Path.GetFileNameWithoutExtension(m)
                Dim ta = s.Split("_")
                If IsNumeric(ta(0)) Then 'If the file name as a number at the start, it's a map file!
                    maps(cnt) = m
                    cnt += 1
                End If
            End If
        Next
        ReDim Preserve maps(cnt - 1)
        'now lets search each map file for decals_pbs
        Dim oPath = Temp_Storage + "\decals\"
        For i = 0 To cnt - 1
            Using z As New Ionic.Zip.ZipFile(maps(i))
                For Each item In z
                    If item.FileName.Contains("decals_pbs") _
                And Not item.FileName.ToLower.Contains("snow") Then
                        item.Extract(oPath, ExtractExistingFileAction.OverwriteSilently)
                    End If
                Next
            End Using
            GC.Collect() 'clean up trash to free memory!
        Next
        load_decal_textures()


    End Sub
    Private Sub make_pgk_searh_list()
        'build a list of package files to search for items
        Dim p = My.Settings.game_path + "\res\packages\"
        Dim di = Directory.GetFiles(p)
        Dim cnt As Integer = 0
        ReDim pkg_search_list(100)
        For Each f In di
            Dim ts = Path.GetFileName(f)
            Dim ar = ts.Split("_")
            If IsNumeric(ar(0)) Then
                pkg_search_list(cnt) = f
                cnt += 1
            End If
            If ar(0) = "shared" Then
                pkg_search_list(cnt) = f
                cnt += 1
            End If
            If ar(0) = "hangar" Then
                pkg_search_list(cnt) = f
                cnt += 1
            End If
        Next
        ReDim Preserve pkg_search_list(cnt - 1)
    End Sub
    Private Sub load_resources()
        Dim t As New Stopwatch
        info_Label.Text = "loading Environment models"
        start_up_log.AppendLine("loading models..")
        Application.DoEvents()
        load_models()
        Dim tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        t.Restart()
        Dim iPath As String = Application.StartupPath + "\resources\models\"
        '==========
        info_Label.Text = "loading Environment textures"
        start_up_log.AppendLine("loading Env textures...")
        Application.DoEvents()
        load_cube_and_cube_map()
        '==========
        gradient_lookup_id = load_png_file(iPath + "borderGradient.png")
        dome_textureId = load_png_file(iPath + "dome.png")
        load_terrain()
        tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        t.Restart()
        '==========
        info_Label.Text = "loading Upton control"
        start_up_log.AppendLine("loading Upton....")
        Application.DoEvents()
        upton.load_upton()
        tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        t.Restart()
        '==========
        info_Label.Text = "loading Decal textures"
        start_up_log.AppendLine("loading Decal Textures")
        Application.DoEvents()
        load_decal_textures()
        tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        t.Restart()
        '==========
        info_Label.Text = "loading Decal layout"
        start_up_log.AppendLine("loading Decal Layout")
        Application.DoEvents()
        load_decal_data()
        tt = t.ElapsedMilliseconds.ToString
        start_up_log.AppendLine("T = " + tt + "ms")
        '==========
        iPath = Application.StartupPath + "\resources\"
        arrow_textureID(0) = load_png_file(iPath + "arrow_texture_up.png")
        arrow_textureID(1) = load_png_file(iPath + "arrow_texture_over.png")
        arrow_textureID(2) = load_png_file(iPath + "arrow_texture_down.png")
        arrow_listID = get_X_model(iPath + "arrow.x")
        '==========

        t.Stop()
    End Sub
    Private Sub load_terrain()
        Dim iPath As String = Application.StartupPath + "\resources\models\"
        'terrain_modelId = get_X_model(iPath + "terrain.x") '===========================
        terrain_textureId = load_png_file(iPath + "surface.png")
        terrain_textureNormalId = load_png_file(iPath + "surface_NORM.png")
        terrain_noise_id = load_png_file(iPath + "noise.png")
    End Sub
    Private Sub load_cube_and_cube_map()
        Dim iPath As String = Application.StartupPath + "\resources\cube\cubemap_m00_c0"
        Dim id, iler, w, h As Integer

        Gl.glEnable(Gl.GL_TEXTURE_CUBE_MAP)
        Gl.glGenTextures(1, cube_texture_id)
        Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)

        id = Il.ilGenImage
        Il.ilBindImage(id)

        For i = 0 To 5
            Dim ok = Il.ilLoad(Il.IL_PNG, iPath + i.ToString + ".png")
            iler = Il.ilGetError
            If iler = Il.IL_NO_ERROR Then
                Il.ilConvertImage(Il.IL_RGB, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
                w = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
                h = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

                ' Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)


                Gl.glTexImage2D(Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, Gl.GL_RGB8, w, h, 0, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, Gl.GL_RGB8, w, h, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
            Else
                MsgBox("Can't load cube textures!", MsgBoxStyle.Exclamation, "Shit!")
            End If

        Next
        Il.ilBindImage(0)
        Il.ilDeleteImage(id)
        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR)

        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE)
        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE)
        Gl.glTexParameteri(Gl.GL_TEXTURE_CUBE_MAP, Gl.GL_TEXTURE_WRAP_R, Gl.GL_CLAMP_TO_EDGE)

        Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0)
        Gl.glDisable(Gl.GL_TEXTURE_CUBE_MAP)

        Gl.glGenTextures(1, u_brdfLUT)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, u_brdfLUT)

        id = Il.ilGenImage
        Il.ilBindImage(id)

        Il.ilLoad(Il.IL_PNG, Application.StartupPath + "\resources\cube\env_brdf_lut.png")
        iler = Il.ilGetError
        If iler = Il.IL_NO_ERROR Then
            Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            w = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            h = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, w, h, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
        Else
            MsgBox("Can't load cube textures!", MsgBoxStyle.Exclamation, "Shit!")
        End If
        'clean up
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        id = Il.ilGenImage
        Il.ilBindImage(id)
        'get the cube model
    End Sub
    Private Sub load_models()
        load_binary_models()
        'load_x_models()
        boneMarker = get_X_model_marker(Application.StartupPath + "\resources\models\marker.x", v_marker)
        boneMarker2 = get_X_model_marker(Application.StartupPath + "\resources\models\marker_2.x", v_marker2)
        boneMarker3 = get_X_model_marker(Application.StartupPath + "\resources\models\marker_3.x", v_marker3)
        boneMarker4 = get_X_model_marker(Application.StartupPath + "\resources\models\marker_4.x", v_marker4)

    End Sub


    Private Sub load_type_images()
        tank_mini_icons.ColorDepth = ColorDepth.Depth32Bit
        tank_mini_icons.ImageSize = New Size(17, 17)
        Dim sp = Application.StartupPath + "\icons\"

        Dim e = (sp + "heavyTank.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))

        e = (sp + "mediumTank.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))

        e = (sp + "lightTank.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))

        e = (sp + "AT-SPG.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))

        e = (sp + "SPG.png")
        tank_mini_icons.Images.Add(load_type_icon(e))
        tank_mini_icons.Images.Add(load_type_icon(e))



    End Sub
    Private Function load_type_icon(path As String) As Image
        Dim b = New Bitmap(path)
        Return b

    End Function

    Private Sub save_progress(ByVal sender As Object, ByVal e As SaveProgressEventArgs)
        If e.EventType = Ionic.Zip.ZipProgressEventType.Saving_BeforeWriteEntry Then
            PG1.Visible = True
            PG1.Maximum = e.EntriesTotal
            PG1.Value = e.EntriesSaved + 1
            Application.DoEvents()
        End If
        If e.EventType = ZipProgressEventType.Saving_Completed Then
            PG1.Visible = False
        End If
    End Sub

    Dim nations() As String = {"usa.xml", "uk.xml", _
                             "china.xml", "czech.xml", _
                             "france.xml", "germany.xml", _
                             "japan.xml", "poland.xml", _
                             "ussr.xml", "sweden.xml", _
                             "italy.xml"}
    Private Sub load_customization_files()
        For Each entry In scripts_pkg

            If entry.FileName.Contains("item_defs/") Then
                If entry.FileName.Contains("item_defs/customization/") Then
                    If entry.FileName.Contains("item_defs/customization/camouflages/") Then
                        For zed = 0 To 10
                            If entry.FileName.Contains(nations(zed)) Then
                                Dim filename As String = ""
                                Dim ms As New MemoryStream
                                entry.Extract(ms)
                                openXml_stream_2(ms, Path.GetFileNameWithoutExtension(entry.FileName))
                                filename = entry.FileName
                                'Debug.WriteLine(entry.FileName)
                                Dim ta = entry.FileName.Split("/")

                                build_customization_tables(zed, filename)
                            End If
                        Next
                    End If
                End If
            End If
            'Exit For
        Next

    End Sub

    Private Sub build_list_table()
        Dim cnt As Integer = 0
        For Each entry In scripts_pkg
            If entry.FileName.ToLower.Contains("item_defs/vehicles") And _
                entry.FileName.ToLower.Contains("list.xml") Then
                Dim dt As New DataSet
                Dim ms As New MemoryStream()
                entry.Extract(ms)
                openXml_stream_2(ms, Path.GetFileNameWithoutExtension(entry.FileName))
                custom_list_tables(cnt) = New DataSet
                Dim Xreader As New StringReader(TheXML_String)
                custom_list_tables(cnt).ReadXml(Xreader)
                For Each t In custom_list_tables(cnt).Tables
                    If t.TableName <> "price" And t.TableName.ToLower <> "observer" _
                        And t.TableName.ToLower <> "map_list" _
                        And t.TableName.ToLower <> "notinshop" Then
                        Dim t2 As New DataTable
                        t2 = t.Copy
                        dt.Tables.Add(t2)
                    End If
                Next
                custom_list_tables(cnt) = dt.Copy
                'change the data to what we need
                Dim tbl = custom_list_tables(cnt).Copy
                For Each dtbl In tbl.Tables
                    Dim r = dtbl.rows(0)
                    Dim rv = r.item("tags").split(" ")
                    Dim ts As String = ""
                    Select Case rv(0)
                        Case "mediumTank"
                            ts = "Medium"
                        Case "lightTank"
                            ts = "Light"
                        Case "heavyTank"
                            ts = "Heavy"
                        Case "SPG"
                            ts = "Artillary"
                        Case "AT-SPG"
                            ts = "Destoryer"
                    End Select
                    custom_list_tables(cnt).Tables(dtbl.TableName).Columns.add("type")
                    custom_list_tables(cnt).Tables(dtbl.TableName).Rows(0).Item("type") = ts
                Next
                cnt += 1
            End If
        Next
        GC.Collect()
 
    End Sub

    Private Sub build_customization_tables(ByVal id As Integer, ByVal filename As String)
        If filename.Length < 10 Then Return
        custom_tables(id) = New DataSet
        Dim dataset As New DataSet("tank_DataSet_" + id.ToString("00"))
        'check if we are resetting the data. If the file exist, load it, otherwise, build it.
        Dim xml_path = Temp_Storage + "\tank_DataSet_" + id.ToString("00") + ".xml"
        If File.Exists(xml_path) Then
            dataset.ReadXml(xml_path)
            GoTo loaded_jump
        End If

        info_Label.Text = "Creating Camoflage File: " + "tank_DataSet_" + id.ToString("00")
        Application.DoEvents()

        dataset.Tables.Add("armorcolor")
        dataset.Tables.Add("colors")


        Dim data_set As New DataSet
        Dim docx = XDocument.Parse(TheXML_String)
        'Dim doc As New XmlDocument



        'get the armorcolor
        Dim armorcolor = docx.Descendants("armorColor")
        Dim acolor As String = ""
        'these color strings are located in each nations customization.xml file
        'Nation is IMPORTANT! See nations() array for order in load_customization_files() sub.
        Select Case id
            Case 0
                acolor = "82 72 51 0" 'usa
            Case 1
                acolor = "82 72 51 0" 'uk
            Case 2
                acolor = "61 62 42 0" ' china
            Case 3
                acolor = "15 36 36 0" 'czech
            Case 4
                acolor = "15 36 36 0" 'france
            Case 5
                acolor = "90 103 94 0" 'germany
            Case 6
                acolor = "15 36 36 0" 'japan
            Case 7
                acolor = "15 36 36 0" 'poland
            Case 8
                acolor = "61 62 42 0" 'ussr
            Case 9
                acolor = "15 36 36 0" 'sweden
            Case 10
                acolor = "15 36 36 0" 'italy

        End Select
        dataset.Tables("armorcolor").Columns.Add("aColor")
        Dim r = dataset.Tables("armorcolor").NewRow
        r("aColor") = acolor
        dataset.Tables("armorcolor").Rows.Add(r)

        Dim index As Integer = 0



        'get the textures
        For Each el In docx.Descendants("texture")
            'dataset.Tables("color").Columns.Add("texture")
            'dataset.Tables("color").Columns.Add("Id")
            'dataset.Tables("color").Columns.Add("camoName")
            'dataset.Tables("color").Columns.Add("kind")
            Dim tbl As New DataTable("colors")
            tbl.Columns.Add("Id")
            tbl.Columns.Add("c0")
            tbl.Columns.Add("c1")
            tbl.Columns.Add("c2")
            tbl.Columns.Add("c3")
            tbl.Columns.Add("kind")
            tbl.Columns.Add("colors")
            tbl.Columns.Add("camoName")
            tbl.Columns.Add("texture")

            'r("Id") = index.ToString
            'r("texture") = el.Value.ToString
            '---------
            Dim rr = el.Parent
            Dim tg = rr.Descendants("userString")
            Dim rp = rr.Parent
            Dim tus = tg.Value.ToString.Split("/")

            'get kind
            Dim kind_ = rp.Descendants("season")
            'get color
            Dim color_ = rr.Descendants("palette")

            Dim z As Integer = 0
            Dim cvs(40) As String
            For Each c In color_.Elements
                cvs(z) = c.Value.ToString
                z += 1
            Next


            Dim tiling = rr.Descendants("tiling")
            Dim cnt As Integer = 0
            Dim t_names(100) As String
            Dim s_names(100) As String
            For Each tank In tiling.Elements
                Try

                    tbl.Columns.Add(tank.Name.ToString)

                    Dim name_str = tank.Name.ToString

                    'lol wot has added some tanks 2 times to the tiling section
                    'If color.InnerXml.Contains(name_str) Then
                    '    name_str += "ERROR" + cnt.ToString("000")
                    'Else
                    t_names(cnt) = tank.Value.ToString
                    s_names(cnt) = name_str
                    cnt += 1
                    'End If
                Catch ex As Exception
                End Try
            Next
            Dim rcc = tbl.NewRow
            rcc("texture") = el.Value.ToString
            rcc("Id") = index.ToString
            rcc("camoName") = tus(tus.Length - 2)
            rcc("kind") = kind_.Value.ToLower
            rcc("c0") = cvs(0)
            rcc("c1") = cvs(1)
            rcc("c2") = cvs(2)
            rcc("c3") = cvs(3)
            For z = 0 To cnt - 1
                Dim name_str = s_names(z)

                rcc(name_str) = t_names(z)

            Next

            tbl.Rows.Add(rcc)
            dataset.Merge(tbl, False, MissingSchemaAction.Add)

            index += 1
        Next
        dataset.WriteXml(xml_path)
loaded_jump:
        custom_tables(id) = dataset.Copy

    End Sub

    Private Sub load_camo()
        Dim namelist(1000) As String
        Dim cnt As Integer = 0
        For Each ent In shared_sandbox_pkg
            If Path.GetDirectoryName(ent.FileName).Contains("Camouflage") Then
                If Path.GetDirectoryName(ent.FileName).Contains("vehicles") Then
                    namelist(cnt) = ent.FileName
                    cnt += 1
                End If

            End If
        Next
        ReDim Preserve namelist(cnt - 1)
        ReDim camo_images(cnt - 1)
        For i = 0 To cnt - 1
            camo_images(i) = New camo_
            Dim ms = New MemoryStream
            Dim e = shared_sandbox_pkg(namelist(i))
            If e IsNot Nothing Then
                e.Extract(ms)
                camo_images(i).id = get_texture_and_bitmap(ms, namelist(i), camo_images(i).bmp)
                delete_image_start = camo_images(i).id + 1
            End If
        Next
        Dim w = FrmCamo.Width - FrmCamo.ClientSize.Width
        Dim size = Sqrt(cnt - 1)
        FrmCamo.ClientSize = New Size(New System.Drawing.Point(size * 50, size * 50))
        Dim col = 0
        Dim row = 0
        For i = 0 To cnt - 1
            Dim b As New Button
            b.Width = 50
            b.Height = 50
            b.BackgroundImage = camo_images(i).bmp
            b.ImageAlign = ContentAlignment.MiddleCenter
            b.BackgroundImageLayout = ImageLayout.Stretch
            AddHandler b.Click, AddressOf handle_imgbtn_click
            Dim p = New System.Drawing.Point(col * 50, row * 50)
            b.Location = p
            b.Tag = camo_images(i).id
            b.Text = camo_images(i).id.ToString
            b.Font = font_holder.Font
            b.ForeColor = Color.White
            FrmCamo.Controls.Add(b)
            col += 1
            If col > 9 Then
                col = 0
                row += 1
            End If
        Next
    End Sub

    Private Sub handle_imgbtn_click(sender As Object, e As MouseEventArgs)
        Dim b = DirectCast(sender, Button)
        'current_camo_selection = CInt(b.Tag)
        For i = 1 To object_count
            If Not _object(i).name.ToLower.Contains("chassis") Then
                '_object(i).use_camo = current_camo_selection
            End If
        Next
    End Sub

    Private Sub clear_temp_folder()
        If MsgBox("This will clean out all temp folder data!!" + vbCrLf + _
                    "Also this will RESTART the application because it can not run with out" + vbCrLf + _
                    "the data." + vbCrLf + _
                    "This only needs to be done if there was an update to the tank data." + vbCrLf + _
                    "It will force a reload of all data and the long delay creating the shared file." + vbCrLf + _
                    "Would you like to continue?", MsgBoxStyle.YesNo, "Warning..") = MsgBoxResult.No Then
            Return
        End If
        _Started = False
        While update_thread.IsAlive
            Application.DoEvents()
        End While
        Dim f As DirectoryInfo = New DirectoryInfo(Temp_Storage)
        shared_contents_build.Dispose()
        packages(11).Dispose()
        GC.Collect()
        GC.WaitForFullGCComplete()
        If f.Exists Then
            For Each fi In f.GetFiles
                If fi.Name.Contains("Path.txt") Then
                Else
                    fi.Delete()

                End If
            Next
        End If
        Try
            f.Delete()
        Catch ex As Exception
        End Try
        DisableOpenGL()
        Application.Restart()
        End
    End Sub

    Private Sub draw_background()
        'Gdi.SwapBuffers(pb1_hDC)
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Dim w, h As Integer
        G_Buffer.getsize(w, h)
        ResizeGL(w, h)
        ViewPerspective(w, h)
        ViewOrtho()
        'Dim e = Gl.glGetError
        Dim sw! = pb1.Width
        Dim sh! = pb1.Height
        Dim p As New Point(0.0!, 0.0!)
        'Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        'e = Gl.glGetError

        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, Background_image_id)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, w)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, h)
        p.X = -((w / 2) - (sw / 2))
        p.Y = (h / 2) - (sh / 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, Background_image_id)
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
        Gdi.SwapBuffers(pb1_hDC)

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
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gdi.SwapBuffers(pb1_hDC)
        'e = Gl.glGetError

    End Sub
    Private Sub load_back_ground()
        Dim rnd As New Random
tryagain:
        Dim rn = CInt(rnd.NextDouble * 44.0)
        If rn < 8 Then
            GoTo tryagain
        End If
        Dim f = gui_pkg("gui/maps/login/back_" + rn.ToString + "_without_sparks.png")
        Dim ms As New MemoryStream
        Try

            If ms IsNot Nothing Then
                f.Extract(ms)
                Background_image_id = get_png_id(ms)
                delete_image_start += 1 ' so this texture is NOT deleted
            End If
        Catch ex As Exception
            GoTo tryagain

        End Try
    End Sub

    Private Sub set_treeview(ByRef tv As TreeView)
        Dim st_index = TC1.SelectedIndex
        Dim st = TC1.SelectedTab
        start_up_log.AppendLine("Creating TreeView :" + st_index.ToString("00"))
        tv = New mytreeview
        tv.Font = font_holder.Font.Clone
        tv.ContextMenuStrip = conMenu
        tv.DrawMode = TreeViewDrawMode.OwnerDrawText
        tv.ImageList = tank_mini_icons
        tv.Dock = DockStyle.Fill
        tv.Nodes.Clear()
        tv.BackColor = Color.DimGray
        tv.ForeColor = Color.Black
        tv.HotTracking = False
        tv.HideSelection = True
        st.Controls.Add(tv)
        If st_index < 9 Then
            TC1.SelectedIndex = st_index + 1
        End If
        Application.DoEvents()
    End Sub

    Public Sub get_tank_parts_from_xml(ByVal tank As String, ByRef data_set As DataSet)
        'once again the non-standard name calling causes issues
        'Why not use USA for the nation in all paths???? czech, japan, sweeden, poland are ok as is
        Dim turret_names() As String = {"0", "1", "2", "3", "_0", "_1", "_2", "_3"}
        If tank.Contains("american") Then
            tank = tank.Replace("american", "usa")
        End If
        If tank.Contains("british") Then
            tank = tank.Replace("british", "uk")
        End If
        If tank.Contains("chinese") Then
            tank = tank.Replace("chinese", "china")
        End If
        If tank.Contains("french") Then
            tank = tank.Replace("french", "france")
        End If
        If tank.Contains("german") Then
            tank = tank.Replace("german", "germany")
        End If
        If tank.Contains("russian") Then
            tank = tank.Replace("russian", "ussr")
        End If
        ' dont need to change poland, czech, sweden or japan
        'If tank.Contains("czech") Then
        '    tank = tank.Replace("czech", "czech")
        'End If


        Dim f = scripts_pkg("scripts\item_defs\" + tank)
        itemDefPathString = "scripts \ item_defs \ " + tank
        If f Is Nothing Then
            Return
        End If
        Dim ms As New MemoryStream
        f.Extract(ms)
        openXml_stream(ms, "nation")
        ms.Dispose()
        Dim docx = XDocument.Parse(TheXML_String)
        itemDefXmlString = TheXML_String
        Dim doc As New XmlDocument
        Dim xmlroot As XmlNode = xDoc.CreateElement(XmlNodeType.Element, "root", "")
        Dim root_node As XmlNode = doc.CreateElement("model")
        doc.AppendChild(root_node)
        'doc.DocumentElement.ParentNode.Value = "<root>" + vbCrLf + "</root>"



        'this is going to be a mess :(

        'see if there is a exclusionmask in this file.. means its the old SD style tank.
        For Each exclusion In docx.Descendants("exclusionMask")
            exclusionMask_id = -1
            Dim exclu = doc.CreateElement("exclusionMask")
            Dim excluName = doc.CreateElement("name")
            excluName.InnerText = exclusion.Value.ToString.Replace("/", "\")
            If excluName.InnerText.Length > 2 And excluName.InnerText.ToLower.Contains("_cm") Then
                GLOBAL_exclusionMask = 1
                exclu.AppendChild(excluName)
                root_node.AppendChild(exclu)
            End If
        Next



        Dim turret_tiling = doc.CreateElement("turret_tiling")
        Dim found_camo As Boolean = False
        For Each turret0 As XElement In docx.Descendants("turrets0")
            For Each model In turret0.Descendants("undamaged")
                If model.Value.ToLower.ToLower.Contains("turret_") Then

                    Dim p = model.Parent.FirstNode
                    Dim pp = p.Parent
                    Dim ppp = pp.Parent

                    For Each n In ppp.Elements
                        If n.LastNode IsNot Nothing Then

                            If n.LastNode.ToString.ToLower.Contains("tiling") And Not n.FirstNode.ToString.ToLower.Contains("gun") Then

                                Dim tile = n.Descendants("tiling")
                                Dim cct = doc.CreateElement("tiling")
                                cct.InnerText = tile.Value.ToString
                                turret_tiling.AppendChild(cct)
                                root_node.AppendChild(turret_tiling)
                                found_camo = True
                            End If

                        End If
                    Next
                    If Not found_camo Then
                        Dim cct = doc.CreateElement("tiling")
                        cct.InnerText = "1.0 1.0 0.0 0.0"
                        turret_tiling.AppendChild(cct)
                        root_node.AppendChild(turret_tiling)
                    End If
                End If

            Next

        Next
        ' root_node.AppendChild(cct)
        Dim turrets = doc.CreateElement("turrets")
        root_node.AppendChild(turrets)

        For Each turret0 As XElement In docx.Descendants("turrets0")
            For Each turret In turret0.Descendants
                'If turret.Name.ToString.ToLower.Contains("turret" + ext) Then
                'Dim turret_name As XElement = turret.FirstNode

                'add turrets name
                Dim r_node = doc.CreateElement("node")
                For Each guns As XElement In turret.Descendants("guns")
                    Dim p = guns.Parent.FirstNode
                    Dim pp = p.Parent
                    'Dim ppp = pp.Parent.FirstNode
                    Dim t_node = pp.FirstNode
                    Dim t_name As String = pp.Name.ToString
                    Dim nd_turret = doc.CreateElement("turret_name")
                    Dim tx_turret = doc.CreateTextNode(t_name)
                    Dim nd_turret_name = doc.CreateElement("turret")

                    nd_turret_name.InnerText = turret.Name.ToString
                    nd_turret.AppendChild(nd_turret_name)

                    Dim gun_name As XElement = guns.FirstNode
                    For Each gun In guns.Descendants("undamaged")

                        Dim nd_gun = doc.CreateElement("gun")
                        r_node.AppendChild(nd_gun)
                        Dim nd_gun_name = doc.CreateElement("gun_name")
                        Dim gg = gun.Parent
                        Dim ggg = gg.Parent
                        Dim g_name = ggg.Name.ToString
                        nd_gun_name.InnerText = g_name
                        nd_gun.AppendChild(nd_gun_name)

                        gun.Value = gun.Value.Replace("/", "\")
                        Dim nd_g = doc.CreateElement("model")
                        nd_g.InnerText = gun.Value
                        nd_gun.AppendChild(nd_g)
                        Dim camo_cnt As Integer = 0
                        For Each camo In gun_name.Descendants("camouflage")
                            Dim nd_c = doc.CreateElement("gun_camouflage")
                            If Not camo.Value.ToLower.Contains("gun") And camo.Value.Length > 2 Then
                                nd_c.InnerText = camo.Value
                                nd_gun.AppendChild(nd_c)
                                camo_cnt += 1
                            End If
                        Next
                        If camo_cnt = 0 Then
                            Dim nd_c = doc.CreateElement("gun_camouflage")
                            nd_c.InnerText = "1 1 0 0"
                            nd_gun.AppendChild(nd_c)

                        End If
                    Next
                    'nd_turret.LastChild.AppendChild(tx_gun)
                    r_node.AppendChild(nd_turret)
                    turrets.AppendChild(r_node)
                Next
            Next
            'turrets.LastChild.AppendChild(tx_turret)
            'End If
        Next
        For Each turret In docx.Descendants("turrets0")

            Dim tur = turret.Descendants("models")
            For Each models In tur.Descendants("undamaged")
                If models.Value.ToString.ToLower.Contains("turret_") Then
                    ' Dim t_e = doc.CreateElement("turret_model")
                    Dim t_n = doc.CreateElement("turret_model")
                    Dim no = doc.GetElementsByTagName("turret_models") ' see if thsi has been created already
                    If no.Count = 0 Then
                        Dim t_n1 = doc.CreateElement("model")
                        t_n1.InnerText = models.Value.ToString
                        t_n.AppendChild(t_n1)
                        root_node.AppendChild(t_n)

                    Else
                        'If they are out of order, the turret_model has already been created.
                        'We need to add to that element other wise it breaks the XML formating
                        With doc.SelectSingleNode("model/turret_model").CreateNavigator().AppendChild()
                            .WriteElementString("model", models.Value.ToString)
                            .WriteEndElement()
                            .Close()
                        End With

                    End If
                End If
            Next
        Next
        'root_node.AppendChild(t_root)

        Dim chassis = docx.Descendants("chassis")
        For Each ch In chassis.Descendants("undamaged")
            Dim c = doc.CreateElement("chassis")
            Dim cn = doc.CreateElement("model")
            cn.InnerText = ch.Value.ToString
            root_node.AppendChild(c)
            c.AppendChild(cn)
        Next
        Dim cnt As Integer = 0

        For Each n As XElement In docx.Descendants("hull")
            For Each h In n.Descendants("undamaged")
                Dim hn = doc.CreateElement("model")
                hn.InnerText = h.Value.ToString
                For Each camo As XElement In n.Descendants("camouflage")
                    If Not camo.Value.ToLower.Contains("hull") And camo.Value.Length > 2 Then
                        cnt += 1
                        Dim hull = doc.CreateElement("hull")
                        root_node.AppendChild(hull)

                        Dim nd = doc.CreateElement("hull_camouflage")
                        nd.InnerText = camo.Value
                        hull.AppendChild(hn)
                        hull.AppendChild(nd)
                    End If
                Next
                If cnt = 0 Then
                    Dim hull = doc.CreateElement("hull")
                    root_node.AppendChild(hull)
                    Dim nd = doc.CreateElement("hull_camouflage")
                    nd.InnerText = "1.0 1.0 0.0 0.0"
                    hull.AppendChild(hn)
                    hull.AppendChild(nd)

                End If
            Next
        Next
        Try

            Dim track = doc.CreateElement("track_info")
            cnt = 1
            Dim spline_ = docx.Descendants("splineDesc")
            Dim segr = spline_.Descendants("segmentModelRight")
            Dim segl = spline_.Descendants("segmentModelLeft")
            Dim segleftname = doc.CreateElement("left_filename")
            Dim segrightname = doc.CreateElement("right_filename")
            segleftname.InnerText = segl.Value.ToString
            segrightname.InnerText = segr.Value.ToString
            track.AppendChild(segleftname)
            track.AppendChild(segrightname)

            If xDoc.OuterXml.Contains("segment2Model") Then
                Dim segr2 = spline_.Descendants("segment2ModelRight")
                Dim segl2 = spline_.Descendants("segment2ModelLeft")
                Dim segleftname2 = doc.CreateElement("left2_filename")
                Dim segrightname2 = doc.CreateElement("right2_filename")
                segleftname2.InnerText = segl2.Value.ToString
                segrightname2.InnerText = segr2.Value.ToString
                track.AppendChild(segleftname2)
                track.AppendChild(segrightname2)
                cnt = 2


            End If
            'add seg cnt
            Dim seg_cnt = doc.CreateElement("seg_cnt")
            seg_cnt.InnerText = cnt.ToString
            track.AppendChild(seg_cnt)

            'get seglength and seg offsets
            Dim seg_ = docx.Descendants("segmentLength")
            Dim seg_length = doc.CreateElement("segment_length")
            seg_length.InnerText = seg_.Value.ToString
            track.AppendChild(seg_length)
            Dim segoffset = docx.Descendants("segmentOffset")
            Dim seg_off = doc.CreateElement("segmentOffset")
            seg_off.InnerText = segoffset.Value.ToString
            track.AppendChild(seg_off)

            If xDoc.OuterXml.Contains("segment2Offset") Then
                Dim segoffset2 = docx.Descendants("segment2Offset")
                Dim seg_off2 = doc.CreateElement("segment2Offset")
                seg_off2.InnerText = segoffset2.Value.ToString
                track.AppendChild(seg_off2)

            End If

            root_node.AppendChild(track)

        Catch ex As Exception

        End Try

        Dim fm As New MemoryStream
        doc.Save(fm)
        fm.Position = 0
        data_set.ReadXml(fm)
        ms.Dispose()
        fm.Dispose()
    End Sub

    Private Sub load_tabs()
        'Try
        For i = 1 To 10
            info_Label.Text = " Creating Nodes by tier (" + i.ToString("00") + ")"
            Application.DoEvents()
            store_in_treeview(i, treeviews(i))
            Application.DoEvents()
        Next
        'add count to log
        start_up_log.AppendLine("Total Tanks Found:" + TOTAL_TANKS_FOUND.ToString("000"))
        log_text.AppendLine("-= TANKS FOUND IN GAME =-")
        'get_tanks_sandbox()
        For i = 1 To 10
            info_Label.Text = "Adding Nodes to TreeView Lists (" + i.ToString("00") + ")"
            Dim l = node_list(i).item.Length - 2
            ReDim Preserve node_list(i).item(l) ' remove last empty item
            ReDim Preserve icons(i).img(l)
            'sort the array
            'node_list(i).item = node_list(i).item.OrderByDescending(Function(c) c.node.Name).ToArray

            Application.DoEvents()
            Application.DoEvents()
            TC1.SelectedIndex = i - 1
            Dim tn = treeviews(i)
            'add the nodes crated to the treeviews on the form
            tn.SuspendLayout()
            tn.BeginUpdate()
            For j = 0 To node_list(i).item.Length - 1
                icons(i).img(j).img = node_list(i).item(j).icon
                tn.Nodes.Add(node_list(i).item(j).node)
            Next

            'tn.Visible = False
            info_Label.Text = "Sorting Nodes (" + i.ToString("00") + ")"
            log_text.AppendLine("---- Teir : " + i.ToString("00") + " ----")
            For k = 0 To tn.Nodes.Count - 1
                tn.Nodes(k).Text = num_3_places(tn.Nodes(k).Text)
                icons(i).img(k).name = num_3_places(icons(i).img(k).name)
            Next
            tn.Sort()
            Array.Sort(icons(i).img)
            For k = 0 To tn.Nodes.Count - 1
                tn.Nodes(k).Text = back_2_places(tn.Nodes(k).Text)
                icons(i).img(k).name = back_2_places(icons(i).img(k).name)
                Dim s = get_shortname(tn.Nodes(k))
                log_text.AppendLine(pad_string(tn.Nodes(k).Text + ":") + s)
            Next
            'tn.Visible = True
            tn.EndUpdate()
            tn.ResumeLayout()

        Next
        'Catch ex As Exception
        '    MsgBox("Something went wrong adding to the Treeviews." + ex.Message, MsgBoxStyle.Exclamation, "Opps!")
        'End Try
    End Sub
    Private Function pad_string(ByVal s As String) As String
        Return s.PadRight(28, " ")
    End Function

    Public Function num_3_places(ByRef s As String) As String
        Dim a = s.ToCharArray
        Dim o(s.Length) As Char
        Dim p As Integer = 0
        Dim nf As Boolean = True
        Dim c_cnt As Integer = 0
        For i = 0 To a.Length - 1
            o(p) = a(i)
            If IsNumeric(a(i)) Then
                c_cnt += 1 ' only if there are 2 digits
            Else
                p += 1
            End If
            If a(i) = "_" And nf And c_cnt = 2 Then
                nf = False
                o(p - 1) = "0"
                o(i + 1) = "~"
                o(p + 0) = a(i - 2)
                o(p + 1) = a(i - 1)
                p = i + 1
                For z = p To a.Length - 1
                    o(z + 1) = a(z)
                Next
                'ReDim Preserve o(a.Length)
                Return New String(o).Trim
            End If
            If c_cnt = 3 Then
                Return s.Trim
            End If
        Next
        Return New String(o)
    End Function
    Public Function back_2_places(ByRef s As String) As String
        If s.Contains("~") Then
            Dim p = 0
            Dim l = InStr(s, "~")
            s = s.Replace("~", "_")
            Dim a = s.ToCharArray
            Dim o(s.Length - 2) As Char
            o(0) = a(0)
            If l = 6 Then
                o(1) = a(1)
                For i = 3 To a.Length - 1
                    o(i - 1) = a(i)
                Next
                'ReDim Preserve o(a.Length - 2)

                Return New String(o).Trim
            Else
                For i = 2 To a.Length - 1
                    o(i - 1) = a(i)
                Next
                'ReDim Preserve o(a.Length - 2)

                Return New String(o).Trim

            End If

        Else
            Return s
        End If
    End Function


    '================================================================================= Store in treeview


    Private Sub store_in_treeview(ByVal i As Integer, ByRef tn As TreeView)
        AddHandler tn.NodeMouseClick, AddressOf Me.tv_clicked
        AddHandler tn.NodeMouseHover, AddressOf Me.tv_mouse_enter
        AddHandler tn.MouseLeave, AddressOf Me.tv_mouse_leave
        tn.BackColor = Color.DimGray
        tn.CheckBoxes = False
        tn.ItemHeight = 17
        tn.HotTracking = True
        tn.ShowRootLines = False
        tn.ShowLines = False
        tn.Margin = New Padding(0)
        tn.BorderStyle = BorderStyle.None
        Dim ext As String = "-part1.pkg"
        If i < 5 Then
            ext = ".pkg"
        End If
        Dim cnt As Integer = 0
        Dim fpath = My.Settings.game_path + "/res/packages/vehicles_level_" + i.ToString("00") + ext
        If File.Exists(fpath) Then
            packages(i) = Ionic.Zip.ZipFile.Read(fpath)
            start_up_log.AppendLine("Getting Tank data from: " + fpath)
        Else
            start_up_log.AppendLine("Could not find: " + fpath)
            Return
        End If
        Dim fpath_1 = My.Settings.game_path + "/res/packages/vehicles_level_" + i.ToString("00") + "_hd" + ext
        If File.Exists(fpath_1) Then
            packages_HD(i) = Ionic.Zip.ZipFile.Read(fpath_1)
            start_up_log.AppendLine("Getting Tank data from: " + fpath_1)
        Else
            'todo
        End If
        fpath = My.Settings.game_path + "/res/packages/vehicles_level_" + i.ToString("00") + "-part2.pkg"
        If File.Exists(fpath) Then
            packages_2(i) = Ionic.Zip.ZipFile.Read(fpath)
            start_up_log.AppendLine("Getting Tank data from: " + fpath)
        Else
            'start_up_log.AppendLine("Could not find: " + fpath)
            'Return
        End If
        fpath_1 = My.Settings.game_path + "/res/packages/vehicles_level_" + i.ToString("00") + "_hd-part2.pkg"
        If File.Exists(fpath_1) Then
            packages_HD_2(i) = Ionic.Zip.ZipFile.Read(fpath_1)
            start_up_log.AppendLine("Getting Tank data from: " + fpath_1)
        Else
            'todo
        End If

        get_tank_info_by_tier(i.ToString)
        ReDim node_list(i).item(tier_list.Length)
        ReDim icons(i).img(tier_list.Length)

        For Each t In tier_list
            Dim n As New TreeNode
            Select Case t.type ' icon types
                Case "Heavy"
                    n.SelectedImageIndex = 0
                    n.StateImageIndex = 0
                    n.ImageIndex = 0
                Case "Medium"
                    n.SelectedImageIndex = 2
                    n.StateImageIndex = 2
                    n.ImageIndex = 2
                Case "Light"
                    n.SelectedImageIndex = 4
                    n.StateImageIndex = 4
                    n.ImageIndex = 4
                Case "Destoryer"
                    n.SelectedImageIndex = 6
                    n.StateImageIndex = 6
                    n.ImageIndex = 6
                Case "Artillary"
                    n.SelectedImageIndex = 8
                    n.StateImageIndex = 8
                    n.ImageIndex = 8

            End Select
            n.Name = t.nation
            n.Text = t.tag
            n.Tag = fpath + ":" + "vehicles/" + get_nation(t.nation) + "/" + t.tag
            node_list(i).item(cnt).name = t.tag
            node_list(i).item(cnt).node = n
            node_list(i).item(cnt).package = packages(i).Name
            icons(i).img(cnt) = New entry_
            icons(i).img(cnt).img = get_tank_icon(n.Text).Clone
            icons(i).img(cnt).name = t.tag
            If icons(i).img(cnt) IsNot Nothing Then
                node_list(i).item(cnt).icon = icons(i).img(cnt).img.Clone
                node_list(i).item(cnt).icon.Tag = current_png_path
                cnt += 1
                TOTAL_TANKS_FOUND += 1
            Else
                start_up_log.AppendLine("!!!!! Missing Tank Icon PNG !!!!! :" + current_png_path)
            End If
        Next
        ReDim Preserve node_list(i).item(cnt)

        Application.DoEvents()
        ReDim Preserve icons(i).img(cnt)
        Application.DoEvents()
        tn.Tag = i

    End Sub


    Private Function get_nation(ByVal n As String) As String
        Select Case n
            Case "usa"
                Return "american"
            Case "uk"
                Return "british"
            Case "china"
                Return "chinese"
            Case "czech"
                Return "czech"
            Case "france"
                Return "french"
            Case "germany"
                Return "german"
            Case "japan"
                Return "japan"
            Case "poland"
                Return "poland"
            Case "ussr"
                Return "russian"
            Case "sweden"
                Return "sweden"
            Case "italy"
                Return "italy"
        End Select
        Return "who knows what lurks in the minds of men"
    End Function




    Private Sub get_tank_info_by_tier(ByVal t As String)
        ReDim tier_list(200)
        Dim count As Integer = 0
        Try

            Dim q = From row In TankDataTable _
                        Where row.Field(Of String)("tier") = t _
                Select _
                    un = row.Field(Of String)("shortname"), _
                    tag = row.Field(Of String)("tag"), _
                    nation = row.Field(Of String)("nation"), _
                    type = row.Field(Of String)("type") _
                        Order By nation Descending

            'Dim a = q(0).un.Split(":")
            For Each item In q
                tier_list(count).tag = item.tag
                tier_list(count).username = item.un
                tier_list(count).nation = item.nation
                tier_list(count).tier = t
                tier_list(count).type = item.type
                count += 1
            Next
            ReDim Preserve tier_list(count - 1)

        Catch ex As Exception
            Return
        End Try
        Return


    End Sub
    Private Function get_user_name(ByVal fname As String) As String
        If fname.ToLower.Contains("progetto_m35") Then
            Return "Progetto M35 mod 46"
        End If
        Try

            Dim q = From row In TankDataTable _
                        Where row.Field(Of String)("tag") = fname _
                Select _
                    un = row.Field(Of String)("shortname"), _
                    tier = row.Field(Of String)("tier"), _
                    natiom = row.Field(Of String)("nation") _
                    Order By tier Descending

            'Dim a = q(0).un.Split(":")
            If q(0) IsNot Nothing Then
                Return q(0).un

            End If
        Catch ex As Exception
            Return ""
        End Try
        Return ""
    End Function
    Private Function get_tier_id(ByVal fname As String) As String
        Try
            Dim q = From row In TankDataTable _
                        Where row.Field(Of String)("tag") = fname _
                Select _
                    un = row.Field(Of String)("shortname"), _
                    tier = row.Field(Of String)("tier"), _
                    natiom = row.Field(Of String)("nation")
                    Order By tier Descending

            'Dim a = q(0).un.Split(":")
            If q(0) IsNot Nothing Then
                Return q(0).tier
            End If
        Catch ex As Exception
            log_text.Append("Odd Tank: " + fname)
            Return "0"
        End Try
        log_text.AppendLine("Odd Tank: " + fname)
        Return "0"
    End Function

    Dim public_icon_path As String
    Private Sub tv_clicked(ByVal sender As Object, ByVal e As TreeNodeMouseClickEventArgs)
        Dim tn = DirectCast(sender, TreeView)
        If e.Button = Forms.MouseButtons.Right Then
            file_name = e.Node.Tag
            iconbox.Visible = True
            iconbox.BackgroundImage = icons(tn.Tag).img(e.Node.Index).img
            Dim s = get_shortname(e.Node)
            Dim ar = s.Split(":")
            tank_label.Text = ar(0)
            Application.DoEvents()
            Application.DoEvents()
            Application.DoEvents()
            Return
        End If
        'Dim ts = tanklist.Text
        'tn.SelectedNode = Nothing
        'tn.SelectedNode = e.Node
        'If ts.Contains(tn.SelectedNode.Text) Then
        '    tn.SelectedNode.ForeColor = Color.Black
        '    ts = Replace(ts, tn.SelectedNode.Text + vbCrLf, "")
        'Else
        '    tn.SelectedNode.ForeColor = Color.White
        '    ts += tn.SelectedNode.Text + vbCrLf
        'End If
        'tanklist.Text = ts
        'tn.SelectedNode = Nothing
        'tn.Parent.Focus()

    End Sub
    Dim old_backgound_icon As System.Drawing.Bitmap
    Dim old_tank_name As String
    Private Sub tv_mouse_enter(ByVal sender As Object, ByVal e As TreeNodeMouseHoverEventArgs)
        Dim tn = DirectCast(sender, TreeView)
        iconbox.Visible = True
        'iconbox.BringToFront()
        tn.Focus()
        iconbox.BackgroundImage = icons(tn.Tag).img(e.Node.Index).img
        If Not MODEL_LOADED Then
            old_backgound_icon = iconbox.BackgroundImage.Clone
        End If
        Dim s = get_shortname(e.Node)
        Dim ar = s.Split(":")
        tank_label.Text = ar(0)
        If Not MODEL_LOADED Then
            old_tank_name = ar(0)
        End If
        file_name = e.Node.Tag
        tn.Parent.Focus()
    End Sub
    Private Sub tv_mouse_leave(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim tn = DirectCast(sender, TreeView)
        If conMenu.Visible Then
            Return
        End If
        If MODEL_LOADED And old_backgound_icon IsNot Nothing Then
            iconbox.BackgroundImage = old_backgound_icon
            tank_label.Text = old_tank_name
            tn.Parent.Focus()
            Return
        End If
        iconbox.Visible = False
        tn.Parent.Focus()
    End Sub
    Private Function get_shortname(ByVal n As TreeNode) As String
        If n.Text.ToLower.Contains("progetto_m35") Then
            Return "Progetto M35 mod 46"
        End If

        Dim q = From row In TankDataTable _
            Where row.Field(Of String)("tag") = n.Text _
    Select _
        un = row.Field(Of String)("shortname"), _
        tier = row.Field(Of String)("tier"), _
        natiom = row.Field(Of String)("nation")
        Order By tier Descending

        'Dim a = q(0).un.Split(":")
        If q(0) IsNot Nothing Then
            Return q(0).un
        End If
        Return ""
    End Function

    Private Function get_tank_icon(ByVal name As String) As Bitmap
        For Each entry In gui_pkg
            If entry.FileName.Contains(name) And entry.FileName.Contains("/icons/vehicle/") _
            And Not entry.FileName.Contains("small") _
            And Not entry.FileName.Contains("contour") _
            And Not entry.FileName.Contains("unique") _
            And Not entry.FileName.Contains("library") _
                Then
                Dim ms As New MemoryStream
                entry.Extract(ms)
                If ms IsNot Nothing Then
                    'GC.Collect()
                    current_png_path = entry.FileName
                    Return get_png(ms).Clone
                    'bmp.Tag = entry.FileName
                    'Return bmp
                End If
            End If
        Next
        Return Nothing
    End Function

    '###########################################################################################################################################
    Dim tv As Single
    Private Sub draw_environment()
        '############################################
        If frmScreenCap.RENDER_OUT And Not frmScreenCap.r_terrain Then
            Return
        End If
        G_Buffer.attachColor_And_NormalTexture()
        Dim t = time.ElapsedMilliseconds
        If CSng(t) > 5000 Then
            t = 0.0!
        End If
        If t = 0.0! Then
            time.Restart()
        End If
        tv = CSng(t) / 5000.0!
        'Dome
        '############################################
        'Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
        'dome
        Dim s As Single = 1.0
        Gl.glFrontFace(Gl.GL_CCW)
        'Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_BLEND)

        Gl.glUseProgram(shader_list.dome_shader)
        Gl.glUniform1i(dome_colorMap, 0)

        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, dome_textureId)

        Gl.glPushMatrix()
        Gl.glScalef(s, s, s)
        'Gl.glTranslatef(0.0, -4.0, 0.0)
        Gl.glColor3f(1.0, 1.0, 1.0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, dome_textureId)
        Gl.glCallList(dome_modelId)
        Gl.glPopMatrix()
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glUseProgram(0)

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
        '############################################
        'Terrain
        G_Buffer.attachColor_And_Normal_FOG_Texture()
        Gl.glPushMatrix()
        'Gl.glTranslatef(0.0, -0.06, 0.0)
        'Gl.glRotatef(0.25, -1.0, 0.0, 1.0)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glUseProgram(shader_list.terrainShader_shader)
        Gl.glUniform1i(terrain_textureMap, 0)
        Gl.glUniform1i(terrain_depthMap, 1)
        Gl.glUniform1i(terrain_normalMap, 2)
        Gl.glUniform1i(terrain_gradient, 3)
        Gl.glUniform1i(terrain_noise, 4)
        Gl.glUniform1f(terain_animation, tv)
        Gl.glUniform3f(terrain_camPosition, eyeX, eyeY, eyeZ)

        Gl.glUniformMatrix4fv(terrain_shadowProjection, 1, 0, lightProjection)
        If m_shadows.Checked Then
            Gl.glUniform1i(terrain_use_shadow, 1)
        Else
            Gl.glUniform1i(terrain_use_shadow, 0)
        End If
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, terrain_textureId)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_textures(5).colorMap_Id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, terrain_textureNormalId)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, gradient_lookup_id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, terrain_noise_id)

        Gl.glCallList(terrain_modelId)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        'Gl.glDisable(Gl.GL_TEXTURE_2D)

        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
        Gl.glUseProgram(0)
        Gl.glPopMatrix()
        Gl.glDisable(Gl.GL_BLEND)
        G_Buffer.attachColorTexture()

    End Sub
    '###########################################################################################################################################
    'decals
    Private Sub draw_v_colors(id As Integer)
        'This is just to visualize the color stream.
        'It does not look like it has any importance.
        Dim vt1, vt2, vt3 As vec3
        Dim n1, n2, n3 As vec3
        Dim c1, c2, c3 As vec3
        Gl.glBegin(Gl.GL_TRIANGLES)
        For i As UInt32 = 1 To _group(id).nPrimitives_ - 1
            '-----------------
            Dim v1 = _group(id).indicies(i).v1
            Dim v2 = _group(id).indicies(i).v2
            Dim v3 = _group(id).indicies(i).v3
            '--
            vt1.x = _group(id).vertices(v1).x
            vt1.y = _group(id).vertices(v1).y
            vt1.z = _group(id).vertices(v1).z
            '--
            n1.x = _group(id).vertices(v1).nx
            n1.y = _group(id).vertices(v1).ny
            n1.z = _group(id).vertices(v1).nz
            '--
            c1.x = _group(id).vertices(v1).r * 125.0!
            c1.y = _group(id).vertices(v1).g * 125.0!
            c1.z = _group(id).vertices(v1).b * 125.0!
            '-----------------
            vt2.x = _group(id).vertices(v2).x
            vt2.y = _group(id).vertices(v2).y
            vt2.z = _group(id).vertices(v2).z
            '--
            n2.x = _group(id).vertices(v2).nx
            n2.y = _group(id).vertices(v2).ny
            n2.z = _group(id).vertices(v2).nz
            '--
            c2.x = _group(id).vertices(v2).r * 125.0!
            c2.y = _group(id).vertices(v2).g * 125.0!
            c2.z = _group(id).vertices(v2).b * 125.0!
            '-----------------
            vt3.x = _group(id).vertices(v3).x
            vt3.y = _group(id).vertices(v3).y
            vt3.z = _group(id).vertices(v3).z
            '--
            n3.x = _group(id).vertices(v3).nx
            n3.y = _group(id).vertices(v3).ny
            n3.z = _group(id).vertices(v3).nz
            '--
            c3.x = _group(id).vertices(v3).r * 125.0!
            c3.y = _group(id).vertices(v3).g * 125.0!
            c3.z = _group(id).vertices(v3).b * 125.0!
            '-----------------
            Gl.glColor3f(c1.x, c1.y, c1.z)
            Gl.glNormal3f(n1.x, n1.y, n1.z)
            Gl.glVertex3f(vt1.x, vt1.y, vt1.z)

            Gl.glColor3f(c2.x, c2.y, c2.z)
            Gl.glNormal3f(n2.x, n2.y, n2.z)
            Gl.glVertex3f(vt2.x, vt2.y, vt2.z)

            Gl.glColor3f(c3.x, c3.y, c3.z)
            Gl.glNormal3f(n3.x, n3.y, n3.z)
            Gl.glVertex3f(vt3.x, vt3.y, vt3.z)

        Next
        Gl.glEnd()
    End Sub
    Private Sub draw_decals()
        If frmScreenCap.RENDER_OUT And Not frmScreenCap.r_terrain Then
            Return
        End If
        Dim w, h As Integer
        Dim l_array(8) As Single

        l_array(0) = position0(0)
        l_array(1) = position0(1)
        l_array(2) = position0(2)

        l_array(3) = position1(0)
        l_array(4) = position1(1)
        l_array(5) = position1(2)

        l_array(6) = position2(0)
        l_array(7) = position2(1)
        l_array(8) = position2(2)


        G_Buffer.getsize(w, h)
        If current_decal > -1 Then
            G_Buffer.get_depth_buffer(w, h) ' get depth in to gDepth

            Gl.glFrontFace(Gl.GL_CW)
            Gl.glEnable(Gl.GL_CULL_FACE)
            Gl.glDisable(Gl.GL_DEPTH_TEST)
            'Gl.glDisable(Gl.GL_LIGHTING)

            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

            'Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glDisable(Gl.GL_TEXTURE_2D)

            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendEquationSeparate(Gl.GL_FUNC_ADD, Gl.GL_FUNC_ADD)
            Gl.glBlendFuncSeparate(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA, Gl.GL_ONE, Gl.GL_ONE_MINUS_SRC_ALPHA)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)


            Gl.glDepthMask(Gl.GL_FALSE)

            Gl.glUseProgram(shader_list.decalsCpass_shader)
            If m_shadows.Checked Then
                Gl.glUniform1i(decalC_use_shadow, 1)
            Else
                Gl.glUniform1i(decalC_use_shadow, 0)
            End If

            Gl.glUniform1i(decalC_depthMap, 0)
            Gl.glUniform1i(decalC_shadowMap, 1)
            Gl.glUniform1i(decalC_gNormalMap, 2)
            Gl.glUniform1i(decalC_fog, 3)
            Gl.glUniform1i(decalC_cube, 4)
            Gl.glUniform1i(decalC_brdf, 5)

            Gl.glUniform1i(decalC_colorMap, 6)
            Gl.glUniform1i(decalC_normalMap, 7)
            Gl.glUniform1i(decalC_GMM, 8)

            Gl.glUniform3f(decalC_camLocation, eyeX, eyeY, eyeZ)
            Gl.glUniform3fv(decalC_lightPosition, 3, l_array)
            Gl.glUniformMatrix4fv(decalC_shadowProj, 1, Gl.GL_FALSE, lightProjection)
            'set up debug values
            Dim v1, v2, v3, v4 As Single
            v1 = CSng(section_a And 1) : v2 = CSng((section_a And 2) >> 1) : v3 = CSng((section_a And 4) >> 2) : v4 = CSng((section_a And 8) >> 3)
            Gl.glUniform4f(decalC_a_group, v1, v2, v3, v4)
            v1 = CSng(section_b And 1) : v2 = CSng((section_b And 2) >> 1) : v3 = CSng((section_b And 4) >> 2) : v4 = CSng((section_b And 8) >> 3)
            Gl.glUniform4f(decalC_b_group, v1, v2, v3, v4)


            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gDepth)
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer) 'shadow depth map
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gNormal)
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA) 'animated ground fog from terrain shader
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, u_brdfLUT)


            For i = 0 To decal_matrix_list.Length - 2
                Dim j = decal_order(i)
                With decal_matrix_list(j)
                    .transform()
                    Gl.glUniformMatrix4fv(decalC_decal_matrix, 1, Gl.GL_FALSE, .display_matrix)
                    Gl.glUniform2f(decalC_UVwrap, .u_wrap, .v_wrap)
                    Gl.glUniform1f(decalC_uv_rotate, .uv_rot)
                    Gl.glUniform1f(decalC_alpha, .alpha)
                    Gl.glUniform1f(decalC_level, .level)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(j).texture_id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(j).normal_id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, decal_matrix_list(j).gmm_id)

                    Gl.glCallList(decal_draw_box)
                End With
            Next
            Gl.glUseProgram(0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '8
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '7
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '6
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '5
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0) '4
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
            Gl.glDisable(Gl.GL_BLEND)
            Gl.glDepthMask(Gl.GL_TRUE)
            Gl.glEnable(Gl.GL_DEPTH_TEST)

        End If

    End Sub
    Private Sub draw_detail_primtive(ByVal jj As Integer)
        G_Buffer.attachColorTexture()

        If _group(jj).is_glass = 1 Then
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendEquationSeparate(Gl.GL_FUNC_ADD, Gl.GL_FUNC_ADD)
            Gl.glBlendFuncSeparate(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA, Gl.GL_ONE, Gl.GL_ONE_MINUS_SRC_ALPHA)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
            'Gl.glDisable(Gl.GL_DEPTH_TEST)
            Gl.glDepthMask(Gl.GL_FALSE)
        Else
            Gl.glDisable(Gl.GL_BLEND)
            Gl.glEnable(Gl.GL_DEPTH_TEST)
        End If
        'sucks these all have to be unbound first
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 10)
        Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0) '10
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 9)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '9
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '8
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '7
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '6
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '5
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0

        Gl.glUseProgram(shader_list.gDetail_shader)
        Gl.glUniform1i(gDetail_colorMap, 0)
        Gl.glUniform1i(gDetail_normalMap, 1)
        Gl.glUniform1i(gDetail_GMM_Map, 2)
        Gl.glUniform1i(gDetail_detailMap, 3)
        Gl.glUniform1i(gDetail_brdf, 4)
        Gl.glUniform1i(gDetail_cubeMap, 5)

        Gl.glUniform1i(gDetail_is_glass, _group(jj).is_glass)
        Gl.glUniform1i(gDetail_alpha_enabled, _group(jj).alphaTest)
        Gl.glUniform1i(gDetail_alpha_value, _group(jj).alphaRef)
        Gl.glUniform4f(gDetail_dirtParams, _group(jj).g_dirtParams.x, _group(jj).g_dirtParams.y, _group(jj).g_dirtParams.z, _group(jj).g_dirtParams.w)
        'switches for shader debug
        Dim v1, v2, v3, v4 As Single
        v1 = CSng(section_a And 1) : v2 = CSng((section_a And 2) >> 1) : v3 = CSng((section_a And 4) >> 2) : v4 = CSng((section_a And 8) >> 3)
        Gl.glUniform4f(gDetail_a_group, v1, v2, v3, v4)
        v1 = CSng(section_b And 1) : v2 = CSng((section_b And 2) >> 1) : v3 = CSng((section_b And 4) >> 2) : v4 = CSng((section_b And 8) >> 3)
        Gl.glUniform4f(gDetail_b_group, v1, v2, v3, v4)


        Gl.glUniform4f(gDetail_inf, _group(jj).g_detailInfluences.x, _
                                    _group(jj).g_detailInfluences.y, _
                                    _group(jj).g_detailInfluences.z, _
                                    _group(jj).g_detailInfluences.w)

        Gl.glUniform4f(g_Detail_reject_tiling, _group(jj).g_detailRejectTiling.x, _
                                    _group(jj).g_detailRejectTiling.y, _
                                    _group(jj).g_detailRejectTiling.z, _
                                    _group(jj).g_detailRejectTiling.w)

        Gl.glUniform1f(gDetail_A_level, A_level)
        Gl.glUniform1f(gDetail_S_level, S_level) ' convert to 0.0 to 1.0
        Gl.glUniform1f(gDetail_T_level, T_level)
        Gl.glUniform1i(gDetail_has_Detail, _group(jj).g_detailMap_id) 'great than zero, we have a detail map
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metalGMM_Id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).g_detailMap_id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, u_brdfLUT)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
        Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)

        If _group(jj).component_visible Then
            Gl.glCallList(_object(jj).main_display_list)
        End If
        Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0) 'while texture 5 is active.

        Gl.glUseProgram(0)

    End Sub
    Private Sub draw_Primitive()
        Gl.glFrontFace(Gl.GL_CCW)
        'affects all models
        If wire_cb.Checked Then
            Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
        End If

        Dim has_glass As Boolean = False
        For jj = 1 To object_count
            '=====================================
            'affects each model
            If _group(jj).doubleSided = True Then
                Gl.glDisable(Gl.GL_CULL_FACE)
            Else
                Gl.glEnable(Gl.GL_CULL_FACE)
            End If

            'if this is a detail entry, go draw this item
            If _group(jj).is_detail_type = 1 Then
                draw_detail_primtive(jj)
            Else
                If _group(jj).is_glass = 1 Then
                    has_glass = True
                End If
                Gl.glDisable(Gl.GL_BLEND)
                Gl.glEnable(Gl.GL_DEPTH_TEST)
                Gl.glDepthMask(Gl.GL_TRUE)
                Gl.glUseProgram(shader_list.AtlasPBR_shader)
                Gl.glUniform1i(atlasPBR_atlas_AM_map, 0)
                Gl.glUniform1i(atlasPBR_atlas_GBMT_map, 1)
                Gl.glUniform1i(atlasPBR_atlas_MAO_map, 2)
                Gl.glUniform1i(atlasPBR_BLEND_map, 3)
                Gl.glUniform1i(atlasPBR_DIRT_map, 4)
                Gl.glUniform1i(atlasPBR_colorMap, 5)
                Gl.glUniform1i(atlasPBR_colorMap2, 6)
                Gl.glUniform1i(atlasPBR_normalMap, 7)
                Gl.glUniform1i(atlasPBR_GMM_Map, 8)
                Gl.glUniform1i(atlasPBR_brdf, 9)
                Gl.glUniform1i(atlasPBR_cube, 10)


                Gl.glUniform1f(atlasPBR_specular, S_level) ' convert to 0.0 to 1.0
                Gl.glUniform1f(atlasPBR_ambient, A_level)
                Gl.glUniform1f(atlasPBR_brightness, T_level)
                'atlas maps.....................................................
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).AM_atlas)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).GBMT_atlas)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).MAO_atlas)

                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ATLAS_BLEND_ID)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ATLAS_DIRT_ID)

                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).detail_Id) 'diffuse2 map
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metalGMM_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 9)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, u_brdfLUT)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 10)
                Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)

                Gl.glUniform3f(atlasPBR_camPos, eyeX, eyeY, eyeZ)

                Gl.glUniform1i(atlasPBR_IS_ATLAS, _group(jj).is_atlas_type)
                Gl.glUniform1i(atlasPBR_USE_UV2, _group(jj).use_uv2)
                Gl.glUniform1i(atlasPBR_use_normapMap, _group(jj).use_normapMap)
                Gl.glUniform1i(atlasPBR_is_ANM, _group(jj).ANM)


                Gl.glUniform2f(atlasPBR_UVrepete, _group(jj).x_repete, _group(jj).y_repete)
                Gl.glUniform4f(atlasPBR_sizes, _group(jj).g_atlas_size.x, _group(jj).g_atlas_size.y, _group(jj).g_atlas_size.z, _group(jj).g_atlas_size.w)
                Gl.glUniform4f(atlasPBR_INDEXES, _group(jj).g_atlas_indexs.x, _group(jj).g_atlas_indexs.y, _group(jj).g_atlas_indexs.z, _group(jj).g_atlas_indexs.w)

                Gl.glUniform4f(atlasPBR_dirtParams, _group(jj).g_dirtParams.x, _group(jj).g_dirtParams.y, _group(jj).g_dirtParams.z, _group(jj).g_dirtParams.w)

                Gl.glUniform4f(atlasPBR_g_tile0Tint, _group(jj).g_tile0Tint.x, _group(jj).g_tile0Tint.y, _group(jj).g_tile0Tint.z, _group(jj).g_tile0Tint.w)
                Gl.glUniform4f(atlasPBR_g_tile1Tint, _group(jj).g_tile1Tint.x, _group(jj).g_tile1Tint.y, _group(jj).g_tile1Tint.z, _group(jj).g_tile1Tint.w)
                Gl.glUniform4f(atlasPBR_g_tile2Tint, _group(jj).g_tile2Tint.x, _group(jj).g_tile2Tint.y, _group(jj).g_tile2Tint.z, _group(jj).g_tile2Tint.w)
                Gl.glUniform4f(atlasPBR_g_dirtColor, _group(jj).g_dirtColor.x, _group(jj).g_dirtColor.y, _group(jj).g_dirtColor.z, _group(jj).g_dirtColor.w)
                Gl.glUniform2f(atlasPBR_image_size, _group(jj).image_size.x, _group(jj).image_size.y)
                Gl.glUniform1i(atlasPBR_alpha_enable, _group(jj).alphaTest)
                Gl.glUniform1i(atlasPBR_alpha_value, _group(jj).alphaRef)

                'switches for shader debug
                Dim v1, v2, v3, v4 As Single
                v1 = CSng(section_a And 1) : v2 = CSng((section_a And 2) >> 1) : v3 = CSng((section_a And 4) >> 2) : v4 = CSng((section_a And 8) >> 3)
                Gl.glUniform4f(atlasPBR_a_group, v1, v2, v3, v4)
                v1 = CSng(section_b And 1) : v2 = CSng((section_b And 2) >> 1) : v3 = CSng((section_b And 4) >> 2) : v4 = CSng((section_b And 8) >> 3)
                Gl.glUniform4f(atlasPBR_b_group, v1, v2, v3, v4)


                If _group(jj).component_visible Then
                    If Not _group(jj).is_glass = 1 Then
                        Gl.glCallList(_object(jj).main_display_list)
                    End If
                End If
            End If
            Gl.glUseProgram(0)
        Next
        If has_glass Then
            For jj = 0 To object_count
                If _group(jj).is_glass = 1 Then
                    Gl.glDisable(Gl.GL_CULL_FACE)
                    draw_detail_primtive(jj)
                    Gl.glEnable(Gl.GL_CULL_FACE)
                    Gl.glEnable(Gl.GL_DEPTH_TEST)
                    Gl.glDepthMask(Gl.GL_TRUE)
                End If
            Next
        End If
        'Gl.glActiveTexture(Gl.GL_TEXTURE0 + 11)
        'Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '10
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 10)
        Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0) '10
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 9)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '9
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '8
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '7
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '6
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '5
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0


        G_Buffer.attachColorTexture()
        If wire_cb.Checked Then
            Gl.glDisable(Gl.GL_TEXTURE_2D)
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
            If VertexColor_cb.Checked Then
                Gl.glColor3f(0.6, 0.6, 0.6)
            Else
                Gl.glColor3f(0.7, 0.7, 0.7)
            End If
            For jj = 1 To object_count
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If _object(jj).visible Then
                    If _group(jj).component_visible Then Gl.glCallList(_object(jj).main_display_list)
                End If
            Next
        End If

    End Sub

    '###########################################################################################################################################
    Public Sub draw_scene()
        Application.DoEvents()
        If gl_stop Then Return
        view_status_string = ""
        'End If
        If gBufferFBO = 0 Then
            G_Buffer.init()
        End If
        If m_shadows.Checked And Not frmComponents.Visible Then
            render_depth_to_depth_texture(0)
            'draw_to_full_screen_mono_shadow_texture()
        End If
        Dim h, w As Integer
        G_Buffer.getsize(w, h)

        If is_camo_active() Then
            m_edit_camo.Visible = True
        Else
            m_edit_camo.Visible = False
            frmEditCamo.Visible = False
        End If

        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            '  MessageBox.Show("Unable to make rendering context current")
            Return
        End If

        Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, gBufferFBO)

        G_Buffer.attachColor_And_blm_tex1()
        For jj = 1 To object_count
            If _group(jj).alphaTest = 0 Then
                _group(jj).alphaRef = 0
            End If
        Next
        Dim color_top() As Byte = {20, 20, 20}
        Dim color_bottom() As Byte = {60, 60, 60}
        Dim position() As Single = {10, 10.0F, 10, 1.0F}

        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_MODULATE)
        'Dim er = Gl.glGetError
        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Gl.glFrontFace(Gl.GL_CW)

        Gl.glPolygonOffset(1.0, 1.0)
        Gl.glLineWidth(1)
        Gl.glPointSize(2.0)

        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Dim no_background As Boolean = False
        If frmScreenCap.RENDER_OUT And frmScreenCap.r_color_flag Then
            Gl.glClearColor(frmScreenCap.r_color_val(0), frmScreenCap.r_color_val(1), frmScreenCap.r_color_val(2), 1.0)
            no_background = True
        End If

        If frmScreenCap.RENDER_OUT And frmScreenCap.r_trans Then
            Gl.glClearColor(0.0F, 0.0F, 0.0F, 0.0F)
            no_background = True
        End If

        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        G_Buffer.attachColorTexture()
        Gl.glDisable(Gl.GL_BLEND)
        ResizeGL(w, h)

        If frmScreenCap.RENDER_OUT And CUSTOM_IMAGE_MODE Then
            Gl.glColor4f(1.0, 1.0, 1.0, 0.0) 'red

            no_background = True
            ViewOrtho()
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, custom_image_text_Id)
            draw_main_rec_flip_y(New Point(0, 0), w, h)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glDisable(Gl.GL_TEXTURE_2D)
        End If


        Gl.glEnable(Gl.GL_LIGHTING)
        Gl.glEnable(Gl.GL_CULL_FACE)

        Gl.glEnable(Gl.GL_SMOOTH)
        Gl.glEnable(Gl.GL_NORMALIZE)

        Dim v As Point = pb1.Size
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glClear(Gl.GL_DEPTH_BUFFER_BIT)
        If Not no_background Then
            Gl.glClearColor(0.0F, 0.0F, 0.2353F, 1.0F)
            'gradiant background
            ViewOrtho()
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glDisable(Gl.GL_DEPTH_TEST)

            Gl.glBegin(Gl.GL_QUADS)
            Dim aspect = v.Y / v.X
            Gl.glColor3ubv(color_top)
            Gl.glVertex3f(0.0, -v.Y, 0)

            Gl.glColor3ubv(color_bottom)
            Gl.glVertex3f(0.0, 0.0, 0)
            Gl.glVertex3f(v.X, 0.0, 0)

            Gl.glColor3ubv(color_top)
            Gl.glVertex3f(v.X, -v.Y, 0)
            Gl.glEnd()
        End If
        Gl.glFrontFace(Gl.GL_CCW)

        Gl.glDepthFunc(Gl.GL_LEQUAL)
        Dim drawme As Boolean = True

        Dim l_color() = {0.3!, 0.3!, 0.3!}
        Gl.glEnable(Gl.GL_LIGHTING)

        If MODEL_LOADED Then
            If m_show_fbx.Checked Then
                view_status_string = ": FBX View "
            Else
                view_status_string = ": Model View "
            End If
        Else
            view_status_string = ": Nothing Loaded "
        End If
        If Not wire_cb.Checked Then
            view_status_string += ": Solid : "
        Else
            view_status_string += ": Facets : "
        End If
        ViewPerspective(w, h)
        'adjust light2
        set_eyes()
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position0)
        Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, position1)
        Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, position2)

        '=============================================================
        If grid_cb.Checked And Not grid_cb.CheckState = CheckState.Indeterminate Then
            draw_environment()
            draw_decals()
        Else
            Gl.glFrontFace(Gl.GL_CW)
        End If
        '=============================================================
        '-----------------------------------------------------------------------------
        'light positions
        If Show_lights Then
            '0
            Gl.glPushMatrix()
            Gl.glTranslatef(position0(0), position0(1), position0(2))
            Gl.glColor3f(1.0, 0.0, 0.0) 'red
            glutSolidSphere(0.2, 10, 10)
            Gl.glPopMatrix()
            '1
            Gl.glPushMatrix()
            Gl.glTranslatef(position1(0), position1(1), position1(2))
            Gl.glColor3f(0.0, 1.0, 0.0) 'green
            glutSolidSphere(0.2, 10, 10)
            Gl.glPopMatrix()
            '2
            Gl.glPushMatrix()
            Gl.glTranslatef(position2(0), position2(1), position2(2))
            Gl.glColor3f(0.0, 0.0, 1.0) 'blue
            glutSolidSphere(0.2, 10, 10)
            Gl.glPopMatrix()
        End If
        '-----------------------------------------------------------------------------
        'cube test
        If m_show_environment.Checked Then
            Gl.glEnable(Gl.GL_LIGHTING)
            Gl.glColor3f(0.75, 0.75, 0.75)

            Gl.glPushMatrix()
            Gl.glUseProgram(shader_list.cube_shader)
            Gl.glScalef(45.0, 45.0, 45.0)
            Gl.glEnable(Gl.GL_TEXTURE_CUBE_MAP)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)
            Gl.glCallList(cube_draw_id)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0)
            Gl.glDisable(Gl.GL_TEXTURE_CUBE_MAP)

            Gl.glUseProgram(0)
            Gl.glPopMatrix()

        End If
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        '-----------------------------------------------------------------------------
        'If temp_list > 0 Then
        '    Gl.glFrontFace(Gl.GL_CCW)
        '    Gl.glColor3f(0.25, 0.25, 0.25)
        '    Gl.glCallList(temp_list)
        'End If
        '-----------------------------------------------------------------------------
        'show bone markers if told to
        If MODEL_LOADED And showMarkers_cb.Checked Then
            Dim v_color As vColor_
            Gl.glFrontFace(Gl.GL_CCW)
            If m_show_fbx.Checked Then
                For i = 0 To fbx_boneGroups.Length - 1
                    For z = 0 To fbx_boneGroups(i).nodeCnt - 1
                        If Not fbx_boneGroups(i).node_matrices(z).mat Is Nothing Then
                            Gl.glPushMatrix()
                            Gl.glMultMatrixd(fbx_boneGroups(i).node_matrices(z).mat)
                            v_color = fbx_boneGroups(i).models(z).color
                            Gl.glColor3f(v_color.r, v_color.g, v_color.b)
                            Gl.glCallList(fbx_boneGroups(i).models(z).displayId)
                            Gl.glPopMatrix()
                        End If

                    Next
                Next
            Else
                For i = 0 To v_boneGroups.Length - 1
                    For z = 0 To v_boneGroups(i).nodeCnt - 1
                        Gl.glPushMatrix()
                        Gl.glMultMatrixd(v_boneGroups(i).node_matrices(z).mat)
                        v_color = v_boneGroups(i).models(z).color
                        Gl.glColor3f(v_color.r, v_color.g, v_color.b)
                        Gl.glCallList(v_boneGroups(i).models(z).displayId)
                        Gl.glPopMatrix()
                    Next
                Next
            End If
            Gl.glFrontFace(Gl.GL_CW)
        End If
        '-----------------------------------------------------------------------------
        Gl.glColor3f(1.0, 1.0, 1.0)

        Gl.glColor3fv(l_color)
        'Draw Imported FBX if it exists?
        If FBX_LOADED And m_show_fbx.Checked Then
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glDisable(Gl.GL_CULL_FACE)
            If wire_cb.Checked Then
                Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            End If
            If m_load_textures.Checked Then
                view_status_string += "Textured : "
                Gl.glUseProgram(shader_list.fbx_shader)
                Gl.glUniform1i(fbx_colorMap, 0)
                Gl.glUniform1i(fbx_normalMap, 1)
                Gl.glUniform1i(fbx_specularMap, 2)
                Gl.glUniform1f(fbx_specular, S_level) ' convert to 0.0 to 1.0
                Gl.glUniform1f(fbx_ambient, A_level)
                Gl.glUniform1f(fbx_level, T_level)
                If VertexColor_cb.Checked Then
                    Gl.glUniform1i(fbx_enableVcolor, 1)
                Else
                    Gl.glUniform1i(fbx_enableVcolor, 0)
                End If
                For jj = 1 To fbxgrp.Length - 1
                    If fbxgrp(jj).visible And fbxgrp(jj).component_visible Then
                        Gl.glUniform1i(fbx_texture_count, fbxgrp(jj).texture_count)
                        If m_tangent_normalMaps.Checked Then
                            Gl.glUniform1i(fbx_is_GAmap, 0)
                        Else
                            Gl.glUniform1i(fbx_is_GAmap, 1)
                        End If
                        If fbxgrp(jj).bumped Then
                            Gl.glUniform1i(fbx_bumped, 1)
                        Else
                            Gl.glUniform1i(fbx_bumped, 0)
                        End If
                        Gl.glUniform1i(fbx_alphatest, fbxgrp(jj).alphaTest)
                        If m_load_textures.Checked Then
                            Gl.glColor3f(0.5, 0.5, 0.5)
                            Gl.glActiveTexture(Gl.GL_TEXTURE0)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, fbxgrp(jj).color_Id)
                            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, fbxgrp(jj).normal_Id)
                            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                            Gl.glBindTexture(Gl.GL_TEXTURE_2D, fbxgrp(jj).specular_id)
                        End If
                        Gl.glPushMatrix()
                        Gl.glMultMatrixd(fbxgrp(jj).matrix)
                        Gl.glCallList(fbxgrp(jj).call_list)
                        Gl.glPopMatrix()
                    End If
                Next
            Else
                view_status_string += "Light Only : "
                For jj = 1 To fbxgrp.Length - 1
                    If fbxgrp(jj).visible Then
                        Gl.glPushMatrix()
                        Gl.glMultMatrixd(fbxgrp(jj).matrix)
                        If fbxgrp(jj).component_visible Then Gl.glCallList(fbxgrp(jj).call_list)
                        Gl.glPopMatrix()
                    End If
                Next

            End If
            Gl.glUseProgram(0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            If wire_cb.Checked Then
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glDisable(Gl.GL_LIGHTING)
                Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                If m_load_textures.Checked Then
                    Gl.glColor3f(0.7, 0.7, 0.7)
                Else
                    Gl.glColor3f(0.0, 0.0, 0.0)
                End If
                For jj = 1 To fbxgrp.Length - 1
                    If fbxgrp(jj).visible Then
                        Gl.glPushMatrix()
                        Gl.glMultMatrixd(fbxgrp(jj).matrix)
                        If fbxgrp(jj).component_visible Then Gl.glCallList(fbxgrp(jj).call_list)
                        Gl.glPopMatrix()
                    End If
                Next
            End If
        End If
        'Dont draw textures?
        If MODEL_LOADED And Not m_load_textures.Checked And Not m_show_fbx.Checked Then
            view_status_string += "Light Only : "
            'Gl.glEnable(Gl.GL_LIGHTING)
            If wire_cb.Checked Then
                Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            End If
            For jj = 1 To object_count
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CCW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If _group(jj).doubleSided Or Not _group(jj).metal_textured Then
                    'Gl.glCullFace(Gl.GL_NONE)
                    Gl.glDisable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
                Else
                    'Gl.glCullFace(Gl.GL_BACK)
                    Gl.glEnable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
                End If
                'Gl.glDisable(Gl.GL_CULL_FACE)
                If _object(jj).visible Then
                    If _group(jj).component_visible Then Gl.glCallList(_object(jj).main_display_list)
                End If
            Next

            If wire_cb.Checked Then
                Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                Gl.glColor3f(0.1, 0.1, 0.1)
                For jj = 1 To object_count
                    If _group(jj).is_carraige Then
                        Gl.glFrontFace(Gl.GL_CW)
                    Else
                        Gl.glFrontFace(Gl.GL_CCW)
                    End If
                    If _object(jj).visible Then
                        If _group(jj).component_visible Then Gl.glCallList(_object(jj).main_display_list)
                    End If
                Next

            End If
        End If
        '===========================================================
        'test color stream
        If False And MODEL_LOADED Then
            G_Buffer.attachColorTexture()
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glDisable(Gl.GL_BLEND)
            Gl.glFrontFace(Gl.GL_CW)
            Gl.glEnable(Gl.GL_LIGHTING)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            For i = 1 To object_count
                If _group(i).component_visible Then
                    Try
                        Call draw_v_colors(i)

                    Catch ex As Exception

                    End Try
                End If
            Next
            GoTo nothing_else
        End If
        '===========================================================
        'Stand Alone Primtives colored render
        If MODEL_LOADED And m_load_textures.Checked And Not m_show_fbx.Checked And Not m_simple_lighting.Checked And PRIMITIVES_MODE Then
            G_Buffer.attachColor_And_blm_tex1()
            draw_Primitive()
        End If

        '===========================================================
        'Draw fully rendered?
        Dim id As Integer = SELECTED_CAMO_BUTTON
        If MODEL_LOADED And m_load_textures.Checked And Not m_show_fbx.Checked And Not m_simple_lighting.Checked And Not PRIMITIVES_MODE Then
            view_status_string += "Textured : "

            G_Buffer.attachColor_And_blm_tex1()

            Gl.glUseProgram(shader_list.tank_shader)
            Gl.glUniform1i(tank_colorMap, 0)
            Gl.glUniform1i(tank_normalMap, 1)
            Gl.glUniform1i(tank_GMM, 2)
            Gl.glUniform1i(tank_AO, 3)
            Gl.glUniform1i(tank_detailMap, 4)
            Gl.glUniform1i(tank_camo, 5)
            Gl.glUniform1i(tank_cubeMap, 6)
            Gl.glUniform1i(tank_LUT, 7)
            Gl.glUniform1i(tank_shadowMap, 8)

            Gl.glUniformMatrix4fv(tank_lightMatrix, 1, 0, lightProjection)

            Gl.glUniform3f(tank_Camera, eyeX, eyeY, eyeZ)
            Gl.glUniform1f(tank_specular, S_level) ' convert to 0.0 to 1.0
            Gl.glUniform1f(tank_ambient, A_level)
            Gl.glUniform1f(tank_total, T_level)
            Gl.glUniform1i(tank_use_GMM_Toy, GMM_TOY_VISIBLE)
            Gl.glUniform2f(tank_GMM_Toy_value, GMM_R, GMM_B)
            If m_shadows.Checked Then
                Gl.glUniform1i(tank_use_shadow, 1)
            Else
                Gl.glUniform1i(tank_use_shadow, 0)
            End If
            If VertexColor_cb.Checked Then
                Gl.glUniform1i(tank_colorEnable, 1)
            Else
                Gl.glUniform1i(tank_colorEnable, 0)
            End If
            'set shader debug mask values
            Dim v1, v2, v3, v4 As Single
            v1 = CSng(section_a And 1) : v2 = CSng((section_a And 2) >> 1) : v3 = CSng((section_a And 4) >> 2) : v4 = CSng((section_a And 8) >> 3)
            Gl.glUniform4f(tank_a_group, v1, v2, v3, v4)
            v1 = CSng(section_b And 1) : v2 = CSng((section_b And 2) >> 1) : v3 = CSng((section_b And 4) >> 2) : v4 = CSng((section_b And 8) >> 3)
            Gl.glUniform4f(tank_b_group, v1, v2, v3, v4)

            If wire_cb.Checked Then
                Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
            End If

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, depthBuffer)

            For jj = 1 To object_count - track_info.segment_count
                Gl.glUniform1i(tank_is_Track, _object(jj).is_track)

                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CCW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If _group(jj).doubleSided Or Not _group(jj).metal_textured Then
                    'Gl.glCullFace(Gl.GL_NONE)
                    Gl.glDisable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
                Else
                    'Gl.glCullFace(Gl.GL_BACK)
                    Gl.glEnable(Gl.GL_CULL_FACE)
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
                End If
                Gl.glUniform1i(tank_is_GAmap, _object(jj).ANM)
                Gl.glUniform1i(tank_alphaRef, _group(jj).alphaRef)
                Gl.glUniform2f(tank_detailTiling, _group(jj).detail_tile.x, _group(jj).detail_tile.y)
                Gl.glUniform1f(tank_detailPower, _group(jj).detail_power)
                Gl.glUniform4f(tank_tile_vec4, _object(jj).camo_tiling.x, _object(jj).camo_tiling.y, _object(jj).camo_tiling.z, _object(jj).camo_tiling.w)
                Gl.glUniform1i(tank_use_camo, _object(jj).use_camo)
                Gl.glUniform1i(tank_exclude_camo, _object(jj).exclude_camo)
                Gl.glUniform1i(tank_use_CM, GLOBAL_exclusionMask)
                Gl.glUniform4f(tank_armorcolor, ARMORCOLOR.x, ARMORCOLOR.y, ARMORCOLOR.z, ARMORCOLOR.w)
                If _object(jj).use_camo > 0 Then

                    Gl.glUniform4f(tank_c0, c0(id).x, c0(id).y, c0(id).z, c0(id).w)
                    Gl.glUniform4f(tank_c1, c1(id).x, c1(id).y, c1(id).z, c1(id).w)
                    Gl.glUniform4f(tank_c2, c2(id).x, c2(id).y, c2(id).z, c2(id).w)
                    Gl.glUniform4f(tank_c3, c3(id).x, c3(id).y, c3(id).z, c3(id).w)
                    Gl.glUniform4f(tank_camo_tiling, bb_tank_tiling(id).x, bb_tank_tiling(id).y, bb_tank_tiling(id).z, bb_tank_tiling(id).w)
                End If


                If _object(jj).visible Then
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metalGMM_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                    If GLOBAL_exclusionMask = 1 And Not HD_TANK Then
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, exclusionMask_id)
                    Else
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ao_id)
                    End If
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).detail_Id)
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
                    If _object(jj).use_camo > 0 Then
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, bb_texture_ids(id))
                    End If
                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
                    Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, cube_texture_id)

                    Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, u_brdfLUT)

                    'Gl.glPushMatrix()
                    'Gl.glMultMatrixd(_object(jj).matrix)
                    If _group(jj).component_visible Then Gl.glCallList(_object(jj).main_display_list)
                    'Gl.glPopMatrix()
                End If
            Next
            Gl.glUseProgram(0)

            'clear texture bindings

            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 8)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '8
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 7)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '7
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 6)
            Gl.glBindTexture(Gl.GL_TEXTURE_CUBE_MAP, 0) '6
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '5
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
            Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
            G_Buffer.attachColorTexture()
            If wire_cb.Checked Then
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glDisable(Gl.GL_LIGHTING)
                Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                If VertexColor_cb.Checked Then
                    Gl.glColor3f(0.6, 0.6, 0.6)
                Else
                    Gl.glColor3f(0.7, 0.7, 0.7)
                End If
                For jj = 1 To object_count
                    If _group(jj).is_carraige Then
                        Gl.glFrontFace(Gl.GL_CW)
                    Else
                        Gl.glFrontFace(Gl.GL_CCW)
                    End If
                    If _object(jj).visible Then
                        If _group(jj).component_visible Then Gl.glCallList(_object(jj).main_display_list)
                    End If
                Next
            End If
        End If
        Gl.glEnable(Gl.GL_CULL_FACE)
        'simple lighting
        If MODEL_LOADED And m_load_textures.Checked And Not m_show_fbx.Checked And m_simple_lighting.Checked Then
            view_status_string += "Simple Lighting : "
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glEnable(Gl.GL_LIGHTING)

            'set lighting
            Dim mcolor(4) As Single
            Dim specReflection(4) As Single
            Dim diffuseLight(4) As Single
            mcolor(0) = A_level : mcolor(1) = A_level : mcolor(2) = A_level : mcolor(3) = 1.0
            specReflection(0) = S_level : specReflection(1) = S_level : specReflection(2) = S_level : specReflection(3) = 1.0
            diffuseLight(0) = T_level : diffuseLight(1) = T_level : diffuseLight(2) = T_level : diffuseLight(3) = 1.0
            Gl.glColor3f(A_level, A_level, A_level)
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, mcolor)
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, specReflection)
            'Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, diffuseLight)
            'Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_SPECULAR Or Gl.GL_AMBIENT_AND_DIFFUSE)

            Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, CInt(S_level * 128))

            For jj = 1 To object_count - track_info.segment_count
                If _group(jj).is_carraige Then
                    Gl.glFrontFace(Gl.GL_CW)
                Else
                    Gl.glFrontFace(Gl.GL_CCW)
                End If
                If wire_cb.Checked Then
                    Gl.glEnable(Gl.GL_POLYGON_OFFSET_FILL)
                End If

                If _object(jj).visible Then
                    Gl.glActiveTexture(Gl.GL_TEXTURE0)
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                    If _group(jj).component_visible Then Gl.glCallList(_object(jj).main_display_list)

                End If
            Next
            If wire_cb.Checked Then
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glDisable(Gl.GL_LIGHTING)
                Gl.glDisable(Gl.GL_POLYGON_OFFSET_FILL)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                Gl.glColor3f(0.0, 0.0, 0.0)
                For jj = 1 To object_count
                    If _object(jj).visible Then
                        If _group(jj).component_visible Then Gl.glCallList(_object(jj).main_display_list)
                    End If
                Next
            End If

        End If

        'Draw Surface Normals?
        If MODEL_LOADED And normal_shader_mode > 0 Then
            Gl.glUseProgram(shader_list.normal_shader)
            Gl.glUniform1i(normal_shader_mode_id, normal_shader_mode)
            If MODEL_LOADED Then
                If normal_shader_mode = 1 Then
                    view_status_string += " Normal View by Face : "
                End If
                If normal_shader_mode = 2 Then
                    view_status_string += " Normal View by Vertex : "
                End If
                If FBX_LOADED And m_show_fbx.Checked Then ' FBX if loaded
                    For jj = 1 To fbxgrp.Length - 1
                        Gl.glPushMatrix()
                        Gl.glMultMatrixd(fbxgrp(jj).matrix)
                        If fbxgrp(jj).component_visible Then Gl.glCallList(fbxgrp(jj).call_list)
                        Gl.glPopMatrix()
                    Next
                Else
                    For jj = 1 To object_count
                        If _object(jj).visible Then
                            If _group(jj).component_visible Then Gl.glCallList(_object(jj).main_display_list) 'Model
                        End If
                    Next
                End If
            End If
            Gl.glUseProgram(0)


        End If
nothing_else:
        '==========================================
        ' draw selected poly if set to do so.
        'Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        If MODEL_LOADED And frmTextureViewer.Visible And (frmTextureViewer.m_show_uvs.Checked Or frmTextureViewer.m_uvs_only.Checked) Then
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glColor4f(0.8, 0.4, 0.0, 0.8)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

            If current_part > 0 Then
                If m_show_fbx.Checked Then
                    Gl.glPushMatrix()

                    Gl.glMultMatrixd(fbxgrp(current_part).matrix)
                    Gl.glBegin(Gl.GL_TRIANGLES)
                    Dim p1 = fbxgrp(current_part).indicies(current_vertex - 1).v1
                    Dim p2 = fbxgrp(current_part).indicies(current_vertex - 1).v2
                    Dim p3 = fbxgrp(current_part).indicies(current_vertex - 1).v3
                    Dim v1 = fbxgrp(current_part).vertices(p1)
                    Dim v2 = fbxgrp(current_part).vertices(p2)
                    Dim v3 = fbxgrp(current_part).vertices(p3)

                    Gl.glVertex3f(v1.x, v1.y, v1.z)
                    Gl.glVertex3f(v2.x, v2.y, v2.z)
                    Gl.glVertex3f(v3.x, v3.y, v3.z)

                    Gl.glEnd()
                    Gl.glPopMatrix()
                Else
                    Gl.glBegin(Gl.GL_TRIANGLES)
                    Dim v1 = _object(current_part).tris(current_vertex).v1
                    Dim v2 = _object(current_part).tris(current_vertex).v2
                    Dim v3 = _object(current_part).tris(current_vertex).v3
                    Gl.glVertex3f(v1.x, v1.y, v1.z)
                    Gl.glVertex3f(v2.x, v2.y, v2.z)
                    Gl.glVertex3f(v3.x, v3.y, v3.z)

                    Gl.glEnd()
                End If
            End If
        End If
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glColor3f(0.3, 0.3, 0.3)
        If Not grid_cb.Checked And Not grid_cb.CheckState = CheckState.Indeterminate Then
            Gl.glCallList(grid)
        End If

        'track_test()
        '=================================================================================
        If m_decal.Checked And Not hide_BB_cb.Checked Then
            If current_decal > -1 Then
                Gl.glLineWidth(2.0)
                Gl.glEnable(Gl.GL_LIGHTING)
                Gl.glDisable(Gl.GL_CULL_FACE)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE)
                'Gl.glDisable(Gl.GL_DEPTH_TEST)
                For j = 0 To decal_matrix_list.Length - 2
                    If j = current_decal Then
                        Gl.glColor3f(1.0, 0.0, 0.0)
                    Else
                        Gl.glColor3f(1.0, 1.0, 1.0)
                    End If
                    Gl.glPushMatrix()
                    decal_matrix_list(j).transform()
                    Gl.glMultMatrixf(decal_matrix_list(j).display_matrix)
                    Gl.glCallList(decal_draw_box)
                    Gl.glPopMatrix()
                Next
                Gl.glLineWidth(1.0)

            End If
        End If
        '=================================================================================

        If move_mod Or z_move Then    'draw reference lines to eye center
            Gl.glColor3f(1.0, 1.0, 1.0)
            'Gl.glLineStipple(1, &HF00F)
            'Gl.glEnable(Gl.GL_LINE_STIPPLE)
            Gl.glLineWidth(1)
            Gl.glBegin(Gl.GL_LINES)
            Gl.glVertex3f(U_look_point_x, U_look_point_y + 1000, U_look_point_z)
            Gl.glVertex3f(U_look_point_x, U_look_point_y - 1000, U_look_point_z)

            Gl.glVertex3f(U_look_point_x + 1000, U_look_point_y, U_look_point_z)
            Gl.glVertex3f(U_look_point_x - 1000, U_look_point_y, U_look_point_z)

            Gl.glVertex3f(U_look_point_x, U_look_point_y, U_look_point_z + 1000)
            Gl.glVertex3f(U_look_point_x, U_look_point_y, U_look_point_z - 1000)
            Gl.glEnd()
            'Gl.glLineStipple(1, &HFFFF)
            'Gl.glDisable(Gl.GL_LINE_STIPPLE)
        End If

        Gl.glPopMatrix()
        If MODEL_LOADED Then
            draw_triangle_mouse_texture_window()
        Else
            found_triangle_tv = 0
        End If
        Gl.glFrontFace(Gl.GL_CCW)
        Gl.glEnable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_BACK, Gl.GL_FILL)
        '===========================================================
        Dim P As New Point(0, 0)
        Gl.glDisable(Gl.GL_CULL_FACE)
        '===========================================================
        '######################################################################### ORTHO MODE
        '######################################################################### ORTHO MODE
        '######################################################################### ORTHO MODE
        ViewOrtho()
        'GoTo fuckit

        'pass one FXAA
        'Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
        If m_FXAA.Checked And Not LOADING_FBX Then

            G_Buffer.attachFXAAtexture()
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

            Gl.glUseProgram(shader_list.FXAA_shader)
            Gl.glUniform1i(FXAA_color, 0)
            Gl.glUniform2f(FXAA_screenSize, CSng(w), CSng(h))



            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            Gl.glColor3f(1.0, 1.0, 1.0)

            draw_main_rec(P, w, h)
        End If

        '===========================================================


        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glUseProgram(0)
        '===========================================================
        '===========================================================
        '===========================================================
        'final render
        '===========================================================
        '===========================================================
        '===========================================================
fuckit:
        '=============================================================
        'do bloom mixing
        If Not m_bloom_off.Checked And Not LOADING_FBX Then
            If frmScreenCap.RENDER_OUT And frmScreenCap.r_color_flag Then
            Else
                Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, blm_fbo.blm_fbo)
                blm_fbo.blur()
                blm_fbo.detach_textures()

            End If

            ResizeGL(w, h)
            ViewOrtho()

            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
            Gl.glUseProgram(shader_list.bloom_shader)
            If frmScreenCap.RENDER_OUT And frmScreenCap.r_trans Then
                Gl.glUniform1i(bloom_transparent, 1)
            Else
                Gl.glUniform1i(bloom_transparent, 0)
            End If
            Gl.glUniform1i(bloom_gcolor, 0)
            Gl.glUniform1i(bloom_blm_tex1, 1)
            If m_enableBloom.Checked Then
                Gl.glUniform1i(bloom_enableBloom, 1)
            Else
                Gl.glUniform1i(bloom_enableBloom, 0)
            End If
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            If m_FXAA.Checked Then
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA)
            Else
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            End If
            Gl.glActiveTexture(Gl.GL_TEXTURE1)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, blm_fbo.blm_tex2)
            draw_main_rec(P, w, h)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glActiveTexture(Gl.GL_TEXTURE0)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Gl.glUseProgram(0)
            'copy buffer to gColor
            Gl.glReadBuffer(Gl.GL_BACK)
            If m_FXAA.Checked Then
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA)
            Else
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
            End If
            Gl.glCopyTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, 0, 0, w, h, 0)
        Else
            Gl.glBindFramebufferEXT(Gl.GL_FRAMEBUFFER_EXT, 0)

        End If
        '=============================================================


        'GoTo over
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        'm_show_fbx.Checked = True
        If m_FXAA.Checked And Not LOADING_FBX Then
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gFXAA)
        Else
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, gColor)
        End If
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        draw_main_rec(P, w, h)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glUseProgram(0)
        'menu
        'draw_menu()
        'if we are doing a screen cap, dont draw any text, timing or the bottom red panel
        If frmScreenCap.RENDER_OUT Then
            Return
        End If
        '######################################################################
        'handle upton if needed
        If m_decal.Checked Then
            upton.draw_upton()
        End If
        '######################################################################
        'draw bottom hightlighted area
        Dim top As Integer = 20
        Dim s_Location As Integer = 0
        If season_Buttons_VISIBLE Then
            s_Location = 157
            top = 177
        End If
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.6)
        Gl.glBegin(Gl.GL_TRIANGLES)
        Gl.glVertex3f(0.0, -pb1.Height, 0.0)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.4)
        Gl.glVertex3f(0.0, -pb1.Height + top, 0.0)
        Gl.glVertex3f(pb1.Width, -pb1.Height + top, 0.0)

        Gl.glVertex3f(pb1.Width, -pb1.Height + top, 0.0)
        Gl.glColor4f(0.3, 0.0, 0.0, 0.6)
        Gl.glVertex3f(pb1.Width, -pb1.Height, 0.0)
        Gl.glVertex3f(0.0, -pb1.Height, 0.0)
        Gl.glEnd()
        '######################################################################
        If m_shadow_preview.Checked Then
            show_depth_texture()
        End If
        '######################################################################
        '######################################################################
        Dim str = "FPS: " + screen_totaled_draw_time.ToString + "  " + cur_texture_name
        'swat.Stop()
        If MODEL_LOADED And frmTextureViewer.Visible And (frmTextureViewer.m_show_uvs.Checked Or frmTextureViewer.m_uvs_only.Checked) Then
            glutPrintBox(5, -40, os1.ToString, 0.0, 1.0, 0.0, 1.0) ' fps string

        End If
        'glutPrintBox(10, -60, "W = " + pb1.Width.ToString + " H = " + pb1.Height.ToString, 1.0, 1.0, 1.0, 1.0) ' fps string

        glutPrint(5, 8 - pb1.Height + s_Location, str.ToString, 0.0, 1.0, 0.0, 1.0) ' fps string
        glutPrintBox(5, -20, view_status_string, 0.0, 1.0, 0.0, 1.0) ' view status

        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        If show_textures_cb.Checked Then
            draw_texture_screen()
            If TANK_TEXTURES_VISIBLE Then
                For i = 0 To texture_buttons.Length - 2
                    texture_buttons(i).draw()
                Next
            End If
        End If

        If season_Buttons_VISIBLE Then
            For i = 0 To season_Buttons.Length - 2
                season_Buttons(i).draw()
            Next
        End If
        If CAMO_BUTTONS_VISIBLE Then

            For i = 0 To camo_Buttons.Length - 2
                camo_Buttons(i).draw()
            Next
            If camo_Buttons.Length > 1 Then
                If camo_Buttons(0).location.X < 0 Or camo_Buttons(camo_Buttons.Length - 2).location.X > pb1.Width Then
                    draw_pan_arrows()
                End If
            End If
        End If
        '====================================
        Gdi.SwapBuffers(pb1_hDC)
        Gl.glFinish()
        '====================================
        'has to be AFTER the buffer swap
        Dim et = pick_timer.ElapsedMilliseconds
        If et > 100 Then 'only do picking so often.. NOT every frame.. its to expensive in render time!
            pick_timer.Restart()
            If frmTextureViewer.Visible Then
                For i = 0 To texture_buttons.Length - 2
                    If current_part = texture_buttons(i).part_ID Then
                        If texture_buttons(i).selected Then
                            frmTextureViewer.draw()
                            If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
                                MessageBox.Show("Unable to make rendering context current")
                                End
                            End If
                            Exit For
                        End If
                    End If
                Next
                If pb2_has_focus Then
                    frmTextureViewer.draw()
                    If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
                        MessageBox.Show("Unable to make rendering context current")
                        End
                    End If
                    ViewOrtho()
                End If
            End If
            '==============================================================================
            ViewOrtho()
            If Not STOP_BUTTON_SCAN Then
                Gl.glFrontFace(Gl.GL_CW)
                If season_Buttons_VISIBLE Then
                    draw_season_pick_buttons()
                    mouse_pick_season_button(m_mouse.x, m_mouse.y)
                    'Gdi.SwapBuffers(pb1_hDC)
                End If
                If CAMO_BUTTONS_VISIBLE Then
                    If camo_Buttons.Length > 1 Then
                        If camo_Buttons(0).location.X < 0 Or camo_Buttons(camo_Buttons.Length - 2).location.X > pb1.Width Then
                            draw_pick_arrows()
                            mouse_pick_arrows(m_mouse.x, m_mouse.y)
                        End If
                    End If
                    draw_pick_camo_buttons()
                    mouse_pick_camo_button(m_mouse.x, m_mouse.y)
                End If
                If TANKPARTS_VISIBLE Then
                    draw_tankpart_pick()
                    mouse_pick_tankparts(m_mouse.x, m_mouse.y)
                End If
                If TANK_TEXTURES_VISIBLE Then
                    draw_textures_pick()
                    mouse_pick_textures(m_mouse.x, m_mouse.y)
                End If
            End If
            If TANK_TEXTURES_VISIBLE And frmTextureViewer.Visible Then
                draw_tank_pick()
                mouse_pick_tank_vertex(m_mouse.x, m_mouse.y)
                'Gdi.SwapBuffers(pb1_hDC)
            End If
            '====================================
            'this put the view in perspective!
            If mouse_pick_cb.Checked Then
                mouse_pick_decal()
            End If
            '====================================
        End If
        If m_decal.Checked Then
            upton.pick_upton()
        End If
        '====================================
        Gl.glFlush()
        'er = Gl.glGetError
        OLD_WINDOW_HEIGHT = pb1.Height
        refresh_counter += 1
    End Sub
    Public Sub draw_main_rec(ByVal p As Point, ByVal w As Integer, ByVal h As Integer)
        Gl.glBegin(Gl.GL_QUADS)
        'G_Buffer.getsize(w, h)
        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()


    End Sub
    Private Sub draw_main_rec_flip_y(ByVal p As Point, ByVal w As Integer, ByVal h As Integer)
        Gl.glBegin(Gl.GL_QUADS)
        'G_Buffer.getsize(w, h)
        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y - h)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y)
        Gl.glEnd()


    End Sub
    '###########################################################################################################################################
    '###########################################################################################################################################
    Dim pan_slide As Single = 0
    Private Sub update_pan()

        While _Started

            Dim p_speed As Single = 4.0!
            If pan_slide < 112 Then
                If pan_left Then
                    pan_slide += p_speed
                    pan_location -= p_speed
                End If
            Else
                pan_left = False
                'panST.Restart()
            End If
            If pan_slide < 112 Then
                If pan_right Then
                    pan_slide += p_speed
                    pan_location += p_speed
                End If
            Else
                pan_right = False
                'panST.Restart()
            End If
            Thread.Sleep(8)
        End While

    End Sub
    Private Sub draw_pan_arrows()
        Dim p As New Point
        p.Y = -pb1.Height + 77 + 72
        Dim center = pb1.Width / 2
        Dim c_off As Integer = 75
        p.X = center + c_off
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glEnable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)
        Dim level As Single = 0.76
        Gl.glColor3f(level, level, level)
        '===========================================
        'left side
        If pan_left_over Then
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, arrow_textureID(1))
        Else
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, arrow_textureID(0))
        End If
        If pan_left Then
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, arrow_textureID(2))
        End If
        Gl.glPushMatrix()
        Gl.glTranslatef(p.X, p.Y, 0.0)
        Gl.glScalef(0.5, 0.5, 1.0)
        Gl.glCallList(arrow_listID)
        Gl.glPopMatrix()
        '===========================================
        'right side
        If pan_right_over Then
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, arrow_textureID(1))
        Else
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, arrow_textureID(0))
        End If
        If pan_right Then
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, arrow_textureID(2))
        End If
        Gl.glPushMatrix()
        p.X = center - c_off
        Gl.glTranslatef(p.X, p.Y, 0.0)
        Gl.glScalef(-0.5, 0.5, 1.0)
        Gl.glCallList(arrow_listID)
        Gl.glPopMatrix()


        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glDisable(Gl.GL_TEXTURE_2D)

    End Sub
    Public Sub draw_pick_arrows()
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glDisable(Gl.GL_DEPTH_TEST)

        Gl.glClearColor(0, 0, 0, 0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)

        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

        Dim p As New Point
        p.Y = -pb1.Height + 77 + 72
        Dim center = pb1.Width / 2
        Dim c_off As Integer = 75
        p.X = center - c_off

        'left side
        Gl.glColor3ub(2, 0, 0)
        Gl.glPushMatrix()
        Gl.glTranslatef(p.X, p.Y, 0.0)
        Gl.glScalef(-0.5, 0.5, 1.0)
        Gl.glCallList(arrow_listID)
        'Gl.glCallList(dome_modelId)
        Gl.glPopMatrix()
        'right side
        Gl.glColor3ub(1, 0, 0)

        Gl.glPushMatrix()
        p.X = center + c_off
        Gl.glTranslatef(p.X, p.Y, 0.0)
        Gl.glScalef(0.5, 0.5, 1.0)
        Gl.glCallList(arrow_listID)
        Gl.glPopMatrix()
    End Sub
    Public Sub mouse_pick_arrows(ByVal x As Integer, ByVal y As Integer)
        'If CAMO_BUTTON_DOWN Then Return

        '==========================================
        'pick function
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim type = pixel(3)
        Dim index As UInt32 = CUInt(pixel(0))
        If pan_left Or pan_right Then
            Return
        End If
        If M_DOWN Then
            If index = 1 Then
                pan_left = True
                If camo_Buttons(camo_Buttons.Length - 2).location.X + pan_location > pb1.Width - 100 Then
                    If panST.ElapsedMilliseconds >= 500 Then
                        pan_slide = 0
                        'pan_location -= 110
                        panST.Restart()
                    End If
                End If
                Return
            End If
            If index = 2 Then
                pan_right = True
                If camo_Buttons(0).location.X + pan_location < 0 Then
                    If panST.ElapsedMilliseconds >= 500 Then
                        pan_slide = 0
                        'pan_location += 110
                        panST.Restart()
                    End If
                End If
                Return
            End If
        Else
            'pan_left = False
            'pan_right = False
            If index = 1 And Not pan_left Then
                pan_left_over = True
                Return
            End If
            If index = 2 And Not pan_right Then
                pan_right_over = True
                Return
            End If
        End If
        If Not panST.IsRunning Then
            panST.Start()
        End If
        'pan_left = False
        'pan_right = False
        pan_right_over = False
        pan_left_over = False
    End Sub


    Public Sub draw_triangle_mouse_texture_window()
        'If Not pb2.Focused Then Return



        If found_triangle_tv > 0 Then

            Gl.glEnable(Gl.GL_DEPTH_TEST)
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glColor4f(1.0, 0.0, 0.0, 0.5)
            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            Dim tv = CInt((found_triangle_tv - 1) * 3)
            If m_show_fbx.Checked Then
                Gl.glPushMatrix()
                Gl.glMultMatrixd(fbxgrp(current_tank_part).matrix)
                Gl.glBegin(Gl.GL_TRIANGLES)
                Dim p1 = fbxgrp(current_tank_part).indicies(tv + 0).v1
                Dim p2 = fbxgrp(current_tank_part).indicies(tv + 1).v1
                Dim p3 = fbxgrp(current_tank_part).indicies(tv + 2).v1
                Dim v1 = fbxgrp(current_tank_part).vertices(p1)
                Dim v2 = fbxgrp(current_tank_part).vertices(p2)
                Dim v3 = fbxgrp(current_tank_part).vertices(p3)
                Gl.glVertex3f(v2.x, v2.y, v2.z)
                Gl.glVertex3f(v1.x, v1.y, v1.z)
                Gl.glVertex3f(v3.x, v3.y, v3.z)

                Gl.glEnd()
                Gl.glPopMatrix()
            Else
                tv = found_triangle_tv
                If tv > _object(current_tank_part).tris.Length Then
                    tv = 0
                    Gl.glDisable(Gl.GL_BLEND)
                    Return
                End If
                Gl.glBegin(Gl.GL_TRIANGLES)
                Dim v1 = _object(current_tank_part).tris(tv).v1
                Dim v2 = _object(current_tank_part).tris(tv).v2
                Dim v3 = _object(current_tank_part).tris(tv).v3
                Gl.glVertex3f(v2.x, v2.y, v2.z)
                Gl.glVertex3f(v1.x, v1.y, v1.z)
                Gl.glVertex3f(v3.x, v3.y, v3.z)
                Gl.glEnd()
            End If
            Gl.glDisable(Gl.GL_BLEND)
        End If
    End Sub
    Public Sub track_test()
        'track nurb points
        If MODEL_LOADED And TESTING Then
            If object_count > 6 Then
                Gl.glColor3f(0.9, 0.9, 0.9)
                running = 0.0
                catmullrom.draw_spline()
                delay += 1
                If delay > 5 Then
                    stepper += 1
                    delay = 0
                    If stepper > tracks.Length - 2 Then
                        stepper = 0
                    End If
                End If
                stepper = tracks.Length - 1
                For i = 0 To stepper 'tracks.Length - 1
                    Gl.glColor3f(0.9, 0.0, 0.0)
                    'If tracks(i).name.Contains("Track_R") Then
                    '    Gl.glColor3f(0.9, 0.0, 0.0)

                    'End If
                    Gl.glPushMatrix()
                    Gl.glTranslatef(tracks(i).position.X, tracks(i).position.Y, tracks(i).position.Z)
                    'glutSolidSphere(0.04, 10, 10)
                    Gl.glPopMatrix()
                Next
                Gl.glColor3f(0.0, 0.9, 0.9)
                For i = 0 To path_pointer1 - 1
                    Gl.glPushMatrix()
                    Gl.glTranslatef(path_data1(i).pos1.X, path_data1(i).pos1.Y, path_data1(i).pos1.Z)
                    'Glut.glutSolidSphere(0.03, 10, 10)
                    Gl.glPopMatrix()
                    'Gl.glBegin(Gl.GL_LINES)
                    'Gl.glVertex3f(path_data1(i).pos1.X + 0.5, path_data1(i).pos1.Y, path_data1(i).pos1.Z)
                    'Gl.glVertex3f(-path_data1(i).pos1.X - 0.5, path_data1(i).pos1.Y, path_data1(i).pos1.Z)
                    'Gl.glEnd()
                Next
                Gl.glColor3f(0.5, 0.5, 0.5)

                Dim jj = object_count
                If track_info.segment_count = 2 Then
                    jj -= 1
                End If
                Gl.glUseProgram(shader_list.tank_shader)

                Gl.glActiveTexture(Gl.GL_TEXTURE0)
                Gl.glEnable(Gl.GL_TEXTURE_2D)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(object_count).color_Id)
                Gl.glUniform1i(tank_is_GAmap, _object(jj).ANM)
                Gl.glUniform1i(tank_alphaRef, _group(jj).alphaTest)
                Gl.glUniform2f(tank_detailTiling, _group(jj).detail_tile.x, _group(jj).detail_tile.y)
                Gl.glUniform1f(tank_detailPower, _group(jj).detail_power)
                Gl.glUniform4f(tank_tile_vec4, _object(jj).camo_tiling.x, _object(jj).camo_tiling.y, _object(jj).camo_tiling.z, _object(jj).camo_tiling.w)
                Gl.glUniform1i(tank_use_camo, _object(jj).use_camo)
                Gl.glUniform1i(tank_exclude_camo, 1)
                Gl.glUniform4f(tank_armorcolor, ARMORCOLOR.x, ARMORCOLOR.y, ARMORCOLOR.z, ARMORCOLOR.w)
                Gl.glUniform1i(tank_use_CM, GLOBAL_exclusionMask)

                Gl.glActiveTexture(Gl.GL_TEXTURE0)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).color_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).normal_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).metalGMM_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                If GLOBAL_exclusionMask = 1 And Not HD_TANK Then
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, exclusionMask_id)
                Else
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).ao_id)
                End If
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 4)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, _group(jj).detail_Id)
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 5)
                For i = 0 To path_pointer1 - 1
                    With path_data1(i)
                        Gl.glPushMatrix()
                        Gl.glTranslatef(.pos1.X, .yc, .zc)
                        'Glut.glutSolidSphere(0.02, 10, 10)
                        Gl.glPopMatrix()

                        Gl.glPushMatrix()
                        Gl.glScalef(1.0, 1.0, 1.0)
                        Gl.glTranslatef(.pos1.X, .pos1.Y, .pos1.Z)
                        Gl.glRotatef(.angle - 90, 1.0, 0.0, 0.0)
                        Gl.glCallList(_object(jj).main_display_list)
                        Gl.glBegin(Gl.GL_LINES)
                        'Gl.glVertex3f(0.0, 0.0, 0.0)
                        'Gl.glVertex3f(0.0, 0.0, 0.25)
                        'Gl.glVertex3f(.pos1.X, .pos1.Y, .pos1.Z)
                        'Gl.glVertex3f(.pos1.X, .yc, .zc)
                        Gl.glEnd()
                        Gl.glPopMatrix()
                    End With
                Next
                If track_info.segment_count = 2 Then
                    jj += 1
                    For i = 0 To path_pointer2 - 1
                        With path_data2(i)
                            Gl.glPushMatrix()
                            Gl.glTranslatef(.pos1.X, .yc, .zc)
                            'Glut.glutSolidSphere(0.02, 10, 10)
                            Gl.glPopMatrix()

                            Gl.glPushMatrix()
                            Gl.glScalef(1.0, 1.0, 1.0)
                            Gl.glTranslatef(.pos1.X, .pos1.Y, .pos1.Z)
                            Gl.glRotatef(.angle - 90, 1.0, 0.0, 0.0)
                            Gl.glCallList(_object(jj).main_display_list)
                            Gl.glBegin(Gl.GL_LINES)
                            'Gl.glVertex3f(0.0, 0.0, 0.0)
                            'Gl.glVertex3f(0.0, 0.0, 0.25)
                            'Gl.glVertex3f(.pos1.X, .pos1.Y, .pos1.Z)
                            'Gl.glVertex3f(.pos1.X, .yc, .zc)
                            Gl.glEnd()
                            Gl.glPopMatrix()
                        End With
                    Next

                End If
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
                Gl.glDisable(Gl.GL_TEXTURE_2D)
                Gl.glUseProgram(0)
                'clear texture bindings
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '4
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 3)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '3
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '2
                Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '1
                Gl.glActiveTexture(Gl.GL_TEXTURE0)
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0) '0
            End If
        End If
        '==========================================

    End Sub

    Public Sub draw_XZ_grid()
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glLineWidth(1)
        Gl.glBegin(Gl.GL_LINES)
        Gl.glColor3f(0.3F, 0.3F, 0.3F)
        For z As Single = -100.0F To -1.0F Step 1.0
            Gl.glVertex3f(-100.0F, 0.0F, z)
            Gl.glVertex3f(100.0F, 0.0F, z)
        Next
        For z As Single = 1.0F To 100.0F Step 1.0
            Gl.glVertex3f(-100.0F, 0.0F, z)
            Gl.glVertex3f(100.0F, 0.0F, z)
        Next
        For x As Single = -100.0F To -1.0F Step 1.0
            Gl.glVertex3f(x, 0.0F, 100.0F)
            Gl.glVertex3f(x, 0.0F, -100.0F)
        Next
        For x As Single = 1.0F To 100.0F Step 1.0
            Gl.glVertex3f(x, 0.0F, 100.0F)
            Gl.glVertex3f(x, 0.0F, -100.0F)
        Next
        Gl.glEnd()
        Gl.glLineWidth(1)
        Gl.glBegin(Gl.GL_LINES)
        Gl.glColor3f(0.6F, 0.6F, 0.6F)
        Gl.glVertex3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(-1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -1.0F)
        Gl.glEnd()
        'begin axis markers
        ' red is z+
        ' green is x-
        'blue is z-
        ' yellow x+
        Gl.glLineWidth(1)

        Gl.glBegin(Gl.GL_LINES)
        'z+ red
        Gl.glColor3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, 100.0F)
        'z- blue
        Gl.glColor3f(0.0F, 0.0F, 1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -1.0F)
        Gl.glVertex3f(0.0F, 0.0F, -100.0F)
        'x+ yellow
        Gl.glColor3f(1.0F, 1.0F, 0.0F)
        Gl.glVertex3f(1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(100.0F, 0.0F, 0.0F)
        'x- green
        Gl.glColor3f(0.0F, 1.0F, 0.0F)
        Gl.glVertex3f(-1.0F, 0.0F, 0.0F)
        Gl.glVertex3f(-100.0F, 0.0F, 0.0F)
        '---------
        Gl.glEnd()

        Gl.glEnable(Gl.GL_LIGHTING)

    End Sub
    Private Sub draw_tank_pick()
        Dim w, h As Integer
        G_Buffer.getsize(w, h)
        ViewPerspective(w, h)
        set_eyes()
        Gl.glClearColor(0.0!, 0.0!, 0.0!, 0.0!)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT Or Gl.GL_DEPTH_BUFFER_BIT)
        Gl.glDisable(Gl.GL_BLEND)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDisable(Gl.GL_LIGHTING)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        If m_show_fbx.Checked Then
            For i = 1 To fbxgrp.Length - 1
                If fbxgrp(i).visible Then 'lets not waste time drawing what we wont pick.
                    Gl.glPushMatrix()
                    Gl.glMultMatrixd(fbxgrp(i).matrix)
                    If fbxgrp(i).component_visible Then Gl.glCallList(fbxgrp(i).vertex_pick_list)
                    Gl.glPopMatrix()
                End If
            Next

        Else
            For i = 1 To object_count
                If _object(i).visible Then 'lets not waste time drawing what we wont pick.
                    If _group(i).component_visible Then Gl.glCallList(_object(i).vertex_pick_list)
                End If
            Next

        End If
        '====================================
        'Gdi.SwapBuffers(pb1_hDC)
        '====================================
    End Sub
    Dim v1 As vertice_
    Dim os1 As String


    Public Sub mouse_pick_tank_vertex(ByVal x As Integer, ByVal y As Integer)
        'pick function
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        Dim part = pixel(3)
        Dim index As UInt32 = CUInt(pixel(0))
        os1 = ""
        If part > 0 Then
            mouse_find_location.X = -1000.0
            current_part = part - 10
            index = pixel(0) + (pixel(1) * 256) + (pixel(2) * 65536)

            If index > 0 Then
                current_vertex = index
                set_v_values(current_part, index)
            Else
                current_part = 0
                current_vertex = 0
            End If

        Else
            current_part = 0
            current_vertex = 0
        End If

    End Sub
    Private Sub set_v_values(ByRef part As Integer, ByVal index As Integer)
        Dim ind As _indice
        If m_show_fbx.Checked Then
            ind.a = fbxgrp(current_part).indicies(index - 1).v1
            ind.b = fbxgrp(current_part).indicies(index - 1).v2
            ind.c = fbxgrp(current_part).indicies(index - 1).v3
            With fbxgrp(part).vertices(ind.a)
                v1 = New vertice_
                v1.index_1 = .index_1
                v1.index_2 = .index_2
                v1.index_3 = .index_3
                v1.index_4 = .index_4
                v1.weight_1 = .weight_1
                v1.weight_2 = .weight_2
                v1.weight_3 = .weight_3
                v1.weight_4 = .weight_4
                os1 = "Index: " + v1.index_1.ToString("00") + " " + v1.index_2.ToString("00") + " " + v1.index_3.ToString("00") + " " + v1.index_4.ToString("00") + _
                    " Weight: " + v1.weight_1.ToString("00") + " " + v1.weight_2.ToString("00") + " " + v1.weight_3.ToString("00") + " " + v1.weight_4.ToString("00")
            End With
        Else
            ind.a = _group(current_part).indicies(index).v1
            ind.b = _group(current_part).indicies(index).v2
            ind.c = _group(current_part).indicies(index).v3
            With _group(part).vertices(ind.a)
                v1 = New vertice_
                v1.index_1 = .index_1
                v1.index_2 = .index_2
                v1.index_3 = .index_3
                v1.index_4 = .index_4
                v1.weight_1 = .weight_1
                v1.weight_2 = .weight_2
                v1.weight_3 = .weight_3
                v1.weight_4 = .weight_4
                os1 = "Index: " + v1.index_1.ToString("00") + " " + v1.index_2.ToString("00") + " " + v1.index_3.ToString("00") + " " + v1.index_4.ToString("00") + _
                    " Weight: " + v1.weight_1.ToString("00") + " " + v1.weight_2.ToString("00") + " " + v1.weight_3.ToString("00") + " " + v1.weight_4.ToString("00")
            End With

        End If
    End Sub



    Public Sub set_eyes()

        Dim sin_x, sin_y, cos_x, cos_y As Single
        sin_x = Sin(U_Cam_X_angle + angle_offset)
        cos_x = Cos(U_Cam_X_angle + angle_offset)
        cos_y = Cos(U_Cam_Y_angle)
        sin_y = Sin(U_Cam_Y_angle)
        cam_y = Sin(U_Cam_Y_angle) * view_radius
        cam_x = (sin_x - (1 - cos_y) * sin_x) * view_radius
        cam_z = (cos_x - (1 - cos_y) * cos_x) * view_radius

        Glu.gluLookAt(cam_x + U_look_point_x, cam_y + U_look_point_y, cam_z + U_look_point_z, _
                            U_look_point_x, U_look_point_y, U_look_point_z, 0.0F, 1.0F, 0.0F)

        eyeX = cam_x + U_look_point_x
        eyeY = cam_y + U_look_point_y
        eyeZ = cam_z + U_look_point_z

    End Sub

#Region "decal transform"

    Private tempX, tempZ As Single
    Private Sub move_xyz()
        If current_decal = -1 Then Return
        Dim x, z As Single
        Dim ms As Single = Sin((view_radius / 80.0!) * (PI / 2.0)) ' distance away changes speed. THIS WORKS WELL!
        Dim speed As Single = 0.2

        If upton.state = 5 Or upton.state = 7 Then
            x = (mouse.x - m_mouse.x) * ms * speed
            z = (mouse.y - m_mouse.y) * ms * speed

            g_decal_translate.x += (x * -Cos(Cam_X_angle)) + (z * -Sin(Cam_X_angle))

            g_decal_translate.z += (z * -Cos(Cam_X_angle)) + (x * Sin(Cam_X_angle))

        End If

        If upton.state = 6 Then
            g_decal_translate.y += -(mouse.y - m_mouse.y) * ms * speed
        End If
        decal_matrix_list(current_decal).set_translate_matrix(0, g_decal_translate)
        mouse.x = m_mouse.x
        mouse.y = m_mouse.y
        If track_decal_cb.Checked Then
            look_point_x = decal_matrix_list(current_decal).translate.x
            look_point_y = decal_matrix_list(current_decal).translate.y
            look_point_z = decal_matrix_list(current_decal).translate.z
        End If
    End Sub
    Private Sub rotate_decal_xyz()
        If current_decal = -1 Then Return
        Dim z As Single
        If upton.state = 8 Then
            z = -(mouse.y - m_mouse.y) * 0.01
            g_decal_rotate.x += z
            decal_matrix_list(current_decal).set_x_rotation_matrix(z)
        End If
        If upton.state = 9 Then
            z = -(mouse.y - m_mouse.y) * 0.01
            g_decal_rotate.y += z
            decal_matrix_list(current_decal).set_y_rotation_matrix(z)
        End If
        If upton.state = 10 Then
            z = -(mouse.y - m_mouse.y) * 0.01
            g_decal_rotate.z += z
            decal_matrix_list(current_decal).set_z_rotation_matrix(z)
        End If
        'Debug.WriteLine("x " + x.ToString("0.0000") + " :z " + z.ToString("0.00000"))
        mouse.x = m_mouse.x
        mouse.y = m_mouse.y
    End Sub
    Private Sub scale_decal_xyz()
        If current_decal = -1 Then Return
        Dim v As New vect3
        Dim z As Single
        Dim ms As Double = view_radius / 80 ' distance away changes speed. THIS WORKS WELL!
        Dim speed As Single = 0.25

        z = -(mouse.y - m_mouse.y) * ms * speed
        If upton.state = 1 Then
            g_decal_scale.x += z
            If g_decal_scale.x < 0.1 Then g_decal_scale.x = 0.1
            decal_matrix_list(current_decal).set_scale_matrix(g_decal_scale)
        End If
        If upton.state = 2 Then
            g_decal_scale.z += z
            If g_decal_scale.z < 0.1 Then g_decal_scale.z = 0.1
            decal_matrix_list(current_decal).set_scale_matrix(g_decal_scale)
        End If
        If upton.state = 3 Then
            g_decal_scale.y += z
            If g_decal_scale.y < 0.1 Then g_decal_scale.y = 0.1
            decal_matrix_list(current_decal).set_scale_matrix(g_decal_scale)

        End If
        If upton.state = 4 Then
            g_decal_scale.x += z
            g_decal_scale.y += z
            g_decal_scale.z += z
            If g_decal_scale.x < 0.1 Then g_decal_scale.x = 0.1
            If g_decal_scale.y < 0.1 Then g_decal_scale.y = 0.1
            If g_decal_scale.z < 0.1 Then g_decal_scale.z = 0.1
            decal_matrix_list(current_decal).set_scale_matrix(g_decal_scale)
        End If

        mouse.x = m_mouse.x
        mouse.y = m_mouse.y

    End Sub

#End Region

#Region "PB1 Mouse"

    Private Sub pb1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles pb1.MouseDoubleClick

    End Sub

    Private Sub pb1_MouseDown(sender As Object, e As MouseEventArgs) Handles pb1.MouseDown
        'If M_SELECT_COLOR > 0 Then
        '    For i = 0 To button_list.Length - 2
        '        If M_SELECT_COLOR = button_list(i).color Then
        '            CallByName(Menu_Subs, button_list(i).function_, Microsoft.VisualBasic.CallType.Method)
        '        End If
        '    Next
        'End If
        mouse.x = e.X
        mouse.y = e.Y
        If mouse_pick_cb.Checked Then
            If picked_decal > -1 Then
                current_decal = picked_decal
                picked_decal = -1
                decal_matrix_list(current_decal).get_decals_transform_info()
                mouse_pick_cb.Checked = False
                update_decal_order()
                Dim tc As Integer = 0
                For k = 0 To decal_order.Length - 1
                    If decal_order(k) = current_decal Then
                        tc = k
                    End If
                Next
                d_current_line = tc

                Dim sp = d_list_tb.GetFirstCharIndexFromLine(tc) ' get prev line
                d_list_tb.SelectionStart = sp
                d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                                 d_list_tb.Lines(tc).Length) ' select prev line

            End If
        End If
        If e.Button = Forms.MouseButtons.Right Then
            move_cam_z = True
        End If
        If e.Button = Forms.MouseButtons.Middle Then
            move_mod = True
            M_DOWN = True
        End If
        If e.Button = Forms.MouseButtons.Left Then

            CAMO_BUTTON_DOWN = True
            M_DOWN = True
        End If
    End Sub

    Private Sub pb1_MouseEnter(sender As Object, e As EventArgs) Handles pb1.MouseEnter
        pb1.Focus()
    End Sub
    Private Sub pb1_MouseMove(sender As Object, e As MouseEventArgs) Handles pb1.MouseMove
        m_mouse.x = e.X
        m_mouse.y = e.Y
        If M_DOWN And upton.state > 0 And upton.state < 5 Then
            scale_decal_xyz()
            Return
        End If
        If M_DOWN And upton.state > 4 And upton.state < 8 Then
            move_xyz()
            Return
        End If
        If M_DOWN And upton.state > 7 And upton.state < 11 Then
            rotate_decal_xyz()
            Return
        End If
        If upton.state = 102 And M_DOWN Then
            Dim delta As New Point
            delta.X = mouse.x - m_mouse.x
            delta.Y = mouse.y - m_mouse.y
            upton.position.X -= delta.X
            upton.position.Y += delta.Y
            mouse.x = m_mouse.x
            mouse.y = m_mouse.y
            Return
        End If

        If BUTTON_ID > 0 Then
            Return
        End If
        If pan_left Or pan_right Then
            Return
        End If
        'If check_menu_select() Then ' check if we are over a button
        '    Return
        'End If
        Dim dead As Integer = 5
        Dim t As Single
        Dim M_Speed As Single = mouse_speed_global
        Dim ms As Single = 0.2F * view_radius ' distance away changes speed.. THIS WORKS WELL!
        If M_DOWN Then
            If e.X > (mouse.x + dead) Then
                If e.X - mouse.x > 100 Then t = (1.0F * M_Speed)
            Else : t = CSng(Sin((e.X - mouse.x) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        look_point_x -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z -= ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle -= t
                    End If
                    If Cam_X_angle > (2 * PI) Then Cam_X_angle -= (2 * PI)
                    mouse.x = e.X
                End If
            End If
            If e.X < (mouse.x - dead) Then
                If mouse.x - e.X > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.x - e.X) / 100)) * M_Speed
                If Not z_move Then
                    If move_mod Then ' check for modifying flag
                        look_point_x += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_z += ((t * ms) * (-Sin(Cam_X_angle)))
                    Else
                        Cam_X_angle += t
                    End If
                    If Cam_X_angle < 0 Then Cam_X_angle += (2 * PI)
                    mouse.x = e.X
                End If
            End If
            ' ------- Y moves ----------------------------------
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * M_Speed
                If z_move Then
                    look_point_y -= (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        look_point_z -= ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x -= ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle -= t
                    End If
                    If Cam_Y_angle < -PI / 2.0 Then Cam_Y_angle = -PI / 2.0 + 0.001
                End If
                mouse.y = e.Y
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (M_Speed)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * M_Speed
                If z_move Then
                    look_point_y += (t * ms)
                Else
                    If move_mod Then ' check for modifying flag
                        look_point_z += ((t * ms) * (Cos(Cam_X_angle)))
                        look_point_x += ((t * ms) * (Sin(Cam_X_angle)))
                    Else
                        Cam_Y_angle += t
                    End If
                    If Cam_Y_angle > 1.3 Then Cam_Y_angle = 1.3
                End If
                mouse.y = e.Y
            End If
            'draw_scene()
            'Debug.WriteLine(Cam_X_angle.ToString("0.000") + " " + Cam_Y_angle.ToString("0.000"))
            Return
        End If
        If move_cam_z Then
            If e.Y > (mouse.y + dead) Then
                If e.Y - mouse.y > 100 Then t = (10)
            Else : t = CSng(Sin((e.Y - mouse.y) / 100)) * 12 * mouse_speed_global
                view_radius += (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius < -80.0 Then
                    view_radius = -80.0
                End If
                mouse.y = e.Y
            End If
            If e.Y < (mouse.y - dead) Then
                If mouse.y - e.Y > 100 Then t = (10)
            Else : t = CSng(Sin((mouse.y - e.Y) / 100)) * 12 * mouse_speed_global
                view_radius -= (t * (view_radius * 0.2))    ' zoom is factored in to Cam radius
                If view_radius > -0.01 Then view_radius = -0.01
                mouse.y = e.Y
            End If
            If view_radius > -0.1 Then view_radius = -0.1
            'draw_scene()
            Return
        End If
        mouse.x = e.X
        mouse.y = e.Y
        'GetOGLPos(e.X, e.Y)
        'draw_scene()
    End Sub

    Private Sub pb1_MouseUp(sender As Object, e As MouseEventArgs) Handles pb1.MouseUp
        M_DOWN = False
        CAMO_BUTTON_DOWN = False
        move_cam_z = False
        move_mod = False
    End Sub

    Private Sub pb1_MouseWheel(sender As Object, e As MouseEventArgs) Handles pb1.MouseWheel
        If frmTextureViewer.Visible Then
            mouse_delta = New Point(0, 0)
            mouse_pos = New Point(pb2.Width / 2, pb2.Height / 2)
            If e.Delta > 0 Then
                img_scale_up()
            Else
                img_scale_down()
            End If
        End If

    End Sub




    Private Sub pb1_Paint(sender As Object, e As PaintEventArgs) Handles pb1.Paint
        If w_changing Then draw_scene()
    End Sub
#End Region

#Region "PB2 Mouse"

    Private Sub pb2_MouseDown(sender As Object, e As MouseEventArgs) Handles pb2.MouseDown
        mouse_down = True
        mouse_delta = e.Location

    End Sub

    Private Sub pb2_MouseEnter(sender As Object, e As EventArgs) Handles pb2.MouseEnter
        pb2.Focus()
        pb2_has_focus = True
    End Sub

    Private Sub pb2_MouseLeave(sender As Object, e As EventArgs) Handles pb2.MouseLeave
        pb2_has_focus = False
    End Sub

    Private Sub pb2_MouseMove(sender As Object, e As MouseEventArgs) Handles pb2.MouseMove
        mouse_find_location = e.Location
        If mouse_down Then
            Dim p As New Point
            p = e.Location - mouse_delta
            rect_location += p
            mouse_delta = e.Location
            frmTextureViewer.draw()
            Return
        End If
    End Sub

    Private Sub pb2_MouseUp(sender As Object, e As MouseEventArgs) Handles pb2.MouseUp
        mouse_down = False
    End Sub

    Private Sub pb2_MouseWheel(sender As Object, e As MouseEventArgs) Handles pb2.MouseWheel
        mouse_pos = e.Location
        mouse_delta = e.Location

        If e.Delta > 0 Then
            img_scale_up()
        Else
            img_scale_down()
        End If
    End Sub
    Public Sub set_pb2_size(ByVal size As Point)
        pb2.Location = New Point(0, 0)
        pb2.Width = size.X
        pb2.Height = size.Y
    End Sub
    Public Sub img_scale_up()
        If Zoom_Factor >= 4.0 Then
            Zoom_Factor = 4.0
            Return 'to big and the t_bmp creation will hammer memory.
        End If
        Dim amt As Single = 0.125
        Zoom_Factor += amt
        Dim z = (Zoom_Factor / 1.0) * 100.0
        frmTextureViewer.zoom.Text = "Zoom:" + vbCrLf + z.ToString("000") + "%"
        Application.DoEvents()
        'this bit of math zooms the texture around the mouses center during the resize.
        'old_w and old_h is the original size of the image in width and height
        'mouse_pos is current mouse position in the window.

        Dim offset As New Point
        Dim old_size_w, old_size_h As Double
        old_size_w = (old_w * (Zoom_Factor - amt))
        old_size_h = (old_h * (Zoom_Factor - amt))

        offset = rect_location - (mouse_pos)

        rect_size.X = Zoom_Factor * old_w
        rect_size.Y = Zoom_Factor * old_h

        Dim delta_x As Double = CDbl(offset.X / old_size_w)
        Dim delta_y As Double = CDbl(offset.Y / old_size_h)

        Dim x_offset = delta_x * (rect_size.X - old_size_w)
        Dim y_offset = delta_y * (rect_size.Y - old_size_h)
        Try

            rect_location.X += CInt(x_offset)
            rect_location.Y += CInt(y_offset)

        Catch ex As Exception

        End Try
        frmTextureViewer.draw()
    End Sub
    Public Sub img_scale_down()
        If Zoom_Factor <= 0.25 Then
            Zoom_Factor = 0.25
            Return
        End If
        Dim amt As Single = 0.125
        Zoom_Factor -= amt
        Dim z = (Zoom_Factor / 1.0) * 100.0
        frmTextureViewer.zoom.Text = "Zoom:" + vbCrLf + z.ToString("000") + "%"
        Application.DoEvents()

        'this bit of math zooms the texture around the mouses center during the resize.
        'old_w and old_h is the original size of the image in width and height
        'mouse_pos is current mouse position in the window.

        Dim offset As New Point
        Dim old_size_w, old_size_h As Double

        old_size_w = (old_w * (Zoom_Factor - amt))
        old_size_h = (old_h * (Zoom_Factor - amt))

        offset = rect_location - (mouse_pos)

        rect_size.X = Zoom_Factor * old_w
        rect_size.Y = Zoom_Factor * old_h

        Dim delta_x As Double = CDbl(offset.X / (rect_size.X + (rect_size.X - old_size_w)))
        Dim delta_y As Double = CDbl(offset.Y / (rect_size.Y + (rect_size.Y - old_size_h)))

        Dim x_offset = delta_x * (rect_size.X - old_size_w)
        Dim y_offset = delta_y * (rect_size.Y - old_size_h)
        Try

            rect_location.X += -CInt(x_offset)
            rect_location.Y += -CInt(y_offset)

        Catch ex As Exception

        End Try

        frmTextureViewer.draw()
    End Sub
#End Region


#Region "screen refresh"
    Public Function need_update() As Boolean
        'This updates the display if the mouse has changed the view angles, locations or distance.
        Dim update As Boolean = False

        If look_point_x <> U_look_point_x Then
            U_look_point_x = look_point_x
            update = True
        End If
        If look_point_y <> U_look_point_y Then
            U_look_point_y = look_point_y
            update = True
        End If
        If look_point_z <> U_look_point_z Then
            U_look_point_z = look_point_z
            update = True
        End If
        If Cam_X_angle <> U_Cam_X_angle Then
            U_Cam_X_angle = Cam_X_angle
            update = True
        End If
        If Cam_Y_angle <> U_Cam_Y_angle Then
            U_Cam_Y_angle = Cam_Y_angle
            update = True
        End If
        If view_radius <> u_View_Radius Then
            u_View_Radius = view_radius
            update = True
        End If
        If Not frmScreenCap.Visible And stop_updating And update Then
            update_screen()

        End If

        Return update
    End Function
    Dim l_rot As Single
    Dim l_timer As New Stopwatch
    Dim refresh_counter As Integer = 0
    Private Sub mouse_movement()

    End Sub
    Public Sub update_mouse()
        Dim sun_angle As Single = 0.0
        Dim sun_radius As Single = 5.0
        'This will run for the duration that Tank Exporter is open.
        'Its in a closed loop
        screen_totaled_draw_time = 10.0
        Dim swat As New Stopwatch
        swat.Start()
        Dim x, z As Single
        Dim s As Single = 2.0
        l_timer.Restart()
        Dim l_scaler As Single = 1.0!
        Dim l_radius As Single = 0.0!
        While _Started
            need_update()
            angle_offset = 0
            'scale light based on mode
            If PRIMITIVES_MODE Then
                l_scaler = 5.0!
            Else
                l_scaler = 1.0!
            End If
            Application.DoEvents()
            If Not gl_busy And Not Me.WindowState = FormWindowState.Minimized Then
                'scale light based on mode
                If Not spin_light Then
                    position0(0) = W_position0(0) * l_scaler
                    position0(1) = W_position0(1) * l_scaler
                    position0(2) = W_position0(2) * l_scaler
                End If

                position1(0) = W_position1(0) * l_scaler
                position1(1) = W_position1(1) * l_scaler
                position1(2) = W_position1(2) * l_scaler

                position2(0) = W_position2(0) * l_scaler
                position2(1) = W_position2(1) * l_scaler
                position2(2) = W_position2(2) * l_scaler

                If Not w_changing And Not stop_updating Then
                    If spin_light And l_timer.ElapsedMilliseconds > 32 Then
                        l_radius = Sqrt(position0(0) ^ 2 + position0(2) ^ 2)
                        l_timer.Restart()
                        l_rot += 0.03
                        If l_rot > 2 * PI Then
                            l_rot -= (2 * PI)
                        End If
                        sun_angle = l_rot
                        x = Cos(l_rot) * l_radius
                        z = Sin(l_rot) * l_radius

                        position0(0) = x
                        position0(1) = 10.0
                        position0(2) = z

                    End If


                    update_screen()
                    If swat.ElapsedMilliseconds >= 1000 Then
                        screen_totaled_draw_time = refresh_counter
                        refresh_counter = 0
                        swat.Restart()
                    End If

                End If
            End If

            Thread.Sleep(4)
            'Application.DoEvents()
        End While
        'Thread.CurrentThread.Abort()
    End Sub
    Private Delegate Sub update_screen_delegate()
    Private Sub update_screen()
        gl_busy = True
        Try
            If Me.InvokeRequired Then
                Me.Invoke(New update_screen_delegate(AddressOf update_screen))
            Else
                draw_scene()
            End If
        Catch ex As Exception

        End Try
        gl_busy = False
    End Sub
    Private Sub Startup_Timer_Tick(sender As Object, e As EventArgs) Handles Startup_Timer.Tick
        Startup_Timer.Enabled = False
        update_thread.IsBackground = True
        update_thread.Name = "mouse updater"
        update_thread.Priority = ThreadPriority.Normal
        update_thread.Start()
        pan_thread.IsBackground = True
        pan_thread.Name = "mouse updater"
        pan_thread.Priority = ThreadPriority.Normal
        pan_thread.Start()
    End Sub
#End Region

    Public Sub clean_house()
        Cam_X_angle = (PI * 0.17) + PI
        Cam_Y_angle = -PI * 0.18
        view_radius = -8.5
        frmModelInfo.Close() ' close so it resets on load
        frmTextureViewer.Hide() ' hide.. so we dont kill settings
        frmEditVisual.Close() ' close so it resets on load
        tank_center_X = 0.0
        tank_center_Y = 0.0
        tank_center_Z = 0.0
        look_point_x = 0
        look_point_y = 0
        look_point_z = 0

        'reset data params
        MODEL_LOADED = False
        m_pick_camo.Enabled = False
        LAST_SEASON = 10
        season_Buttons_VISIBLE = False
        CAMO_BUTTONS_VISIBLE = False
        TANKPARTS_VISIBLE = False
        TANK_TEXTURES_VISIBLE = False
        m_pick_camo.ForeColor = Color.Black
        show_textures_cb.Checked = False
        m_write_primitive.Enabled = False
        m_show_fbx.Checked = False
        m_build_wotmod.Enabled = False
        m_hide_show_components.Enabled = False
        m_set_vertex_winding_order.Enabled = False
        m_show_fbx.Enabled = True
        m_write_non_tank_primitive.Enabled = False
        frmComponentView.Visible = False
        frmReverseVertexWinding.Visible = False

        GLOBAL_exclusionMask = 0
        exclusionMask_sd = -1
        HD_TANK = True
        CHASSIS_COUNT = 0
        turret_count = 0
        hull_count = 0
        XML_Strings(1) = ""
        XML_Strings(2) = ""
        XML_Strings(3) = ""
        XML_Strings(4) = ""
        log_text.Clear()
        If Not bb_texture_list(0) = "" Then
            For i = 0 To bb_texture_list.Length - 1
                Gl.glDeleteTextures(1, bb_texture_ids(i))
                Gl.glFinish()
                Gl.glDeleteTextures(1, bb_camo_texture_ids(i))
                Gl.glFinish()
            Next
        End If
        ReDim textures(0)

        '-------------------------------------------------------
        If object_count > 0 Then
            For i = 1 To object_count
                Gl.glDeleteLists(_object(i).main_display_list, 1)
                Gl.glFinish()
                Gl.glDeleteLists(_object(i).vertex_pick_list, 1)
                Gl.glFinish()
                Gl.glDeleteLists(_object(i).uv2_display_list, 1)
                Gl.glFinish()
                '-------
                Gl.glDeleteTextures(1, _group(i).color_Id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).normal_Id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).detail_Id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).ao_id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).metalGMM_Id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).g_detailMap_id)
                Gl.glFinish()
                'atlas textures/ stand alone prmitives
                Gl.glDeleteTextures(1, _group(i).AM_ID)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).GBMT_ID)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).MAO_ID)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).AM_Height_id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).GBMT_Height_id)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).ATLAS_BLEND_ID)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).ATLAS_DIRT_ID)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).AM_atlas)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).GBMT_atlas)
                Gl.glFinish()
                Gl.glDeleteTextures(1, _group(i).MAO_atlas)
                Gl.glFinish()
            Next
        End If
        'clean up any textures in lists.
        If AM_index_texture_list IsNot Nothing Then
            For k = 0 To AM_index_texture_list.Length - 1
                If AM_index_texture_list(k).list IsNot Nothing Then

                    For i = 0 To AM_index_texture_list(k).list.Length - 1
                        With AM_index_texture_list(k).list(i)
                            If .texture_id > 0 Then
                                Gl.glDeleteTextures(1, .texture_id)
                                Gl.glFinish()
                            End If
                            If .texture_id > 0 Then
                                Gl.glDeleteTextures(1, .texture_id)
                                Gl.glFinish()
                            End If
                            If .texture_id > 0 Then
                                Gl.glDeleteTextures(1, .texture_id)
                                Gl.glFinish()
                            End If
                        End With
                        With GBMT_index_texture_list(k).list(i)
                            If .texture_id > 0 Then
                                Gl.glDeleteTextures(1, .texture_id)
                                Gl.glFinish()
                            End If
                            If .texture_id > 0 Then
                                Gl.glDeleteTextures(1, .texture_id)
                                Gl.glFinish()
                            End If
                            If .texture_id > 0 Then
                                Gl.glDeleteTextures(1, .texture_id)
                                Gl.glFinish()
                            End If
                        End With
                        Try

                            With MAO_index_texture_list(k).list(i)
                                If .texture_id > 0 Then
                                    Gl.glDeleteTextures(1, .texture_id)
                                    Gl.glFinish()
                                End If
                                If .texture_id > 0 Then
                                    Gl.glDeleteTextures(1, .texture_id)
                                    Gl.glFinish()
                                End If
                                If .texture_id > 0 Then
                                    Gl.glDeleteTextures(1, .texture_id)
                                    Gl.glFinish()
                                End If
                            End With
                        Catch ex As Exception

                        End Try
                    Next
                End If
            Next
        End If


        object_count = 0
        ReDim _group(0)
        _group(0) = New _grps
        ReDim _object(0)
        _object(0) = New obj
        object_count = 0
        GC.Collect()
        GC.WaitForFullGCComplete()
    End Sub

    Private Function validate_path(ByVal name As String)
        Dim ent = packages(current_tank_package)(name)
        If ent IsNot Nothing Then
            Return name
        End If
        ent = packages(11)(name)
        If ent IsNot Nothing Then
            Return name
        End If
        ent = packages_2(current_tank_package)(name)
        If ent IsNot Nothing Then
            Return name
        End If
        Return ""
    End Function

    '##################################################################################
    Public Sub process_tank(ByVal save_tank As Boolean)
        'need to set these before loading anyhing
        clean_house()
        remove_loaded_fbx()
        '===================================
        log_text.Append(" ======== Model Load Start =========" + vbCrLf)
        Dim ar = file_name.Split(":")
        file_name = ar(2)
        Me.Text = "File: " + file_name

        Dim ts = ar(1)
        ar = ts.Split("/")
        For i = 0 To ar.Length - 1
            If ar(i).ToLower.Contains("level_") Then
                ts = ar(i)
                Exit For
            End If
        Next
        ar = ts.Split("_")
        ts = ar(2)
        If ts.Contains("-part") Then
            ar = ts.Split("-")
        Else
            ar = ts.Split(".")

        End If
        Dim fd As String = "lod0"
        Try
            current_tank_package = CInt(ar(0))

        Catch ex As Exception
            Try
                If ts.ToLower.Contains("tanks") Then
                    current_tank_package = 11
                End If
                If ts.ToLower.Contains("\shared_sandbox") Then
                    current_tank_package = 11
                End If
            Catch eex As Exception
                MsgBox("Unable to find package file!", MsgBoxStyle.Exclamation, "Well shit...")
                Return
            End Try

        End Try
        '########################################################
        'get the tank info from scripts package
        ar = file_name.Split("/")
        Dim xml_file = ar(0) + "\" + ar(1) + "\" + ar(2) + ".xml"
        Dim t As New DataSet
        get_tank_parts_from_xml(xml_file, t)
        If t.Tables.Count = 0 Then
            Return
        End If
        '-----------------------------------
        'see if this is the old style tanks
        If GLOBAL_exclusionMask = 1 Then
            Dim et = t.Tables("exclusionMask")
            Dim eq = From row In et.AsEnumerable _
                        Select _
                        na = row.Field(Of String)("name")
            exclusionMask_name = eq(0)
            Dim en = packages(current_tank_package)(exclusionMask_name)
            Dim ms As New MemoryStream
            If en Is Nothing Then
                en = packages(11)(exclusionMask_name)
                If en Is Nothing Then
                    en = shared_sandbox_pkg(exclusionMask_name)
                End If
            End If
            If en IsNot Nothing Then
                en.Extract(ms)
                exclusionMask_id = get_texture(ms, exclusionMask_name)
            Else
                log_text.AppendLine("unable to locate : " + exclusionMask_name)
            End If
            et.Dispose()
        End If
        '-------------------------------------------------------
        'Return
        'get take part paths from table
        Dim turrets(10) As String
        Dim guns(10) As String
        Dim hulls(10) As String
        Dim chassis(10) As String
        ReDim hull_tile(10)
        ReDim gun_tile(10)
        ReDim turret_tile(10)
        Dim cnt As Integer = 0

        Dim tbl = t.Tables("gun")
        Dim q = From row In tbl.AsEnumerable _
                Select _
                g_name = row.Field(Of String)("gun_name"), _
                model = row.Field(Of String)("model"), _
                tile = row.Field(Of String)("gun_camouflage") Distinct
        cnt = 0
        '-------------------------------------------------------
        'guns
        For Each thing In q
            Dim gn = thing.model
            guns(cnt) = gn
            gun_tile(cnt) = New vect4
            If thing.tile IsNot Nothing Then

                Dim n = thing.tile.Split(" ")
                gun_tile(cnt).x = CSng(n(0))
                gun_tile(cnt).y = CSng(n(1))
                gun_tile(cnt).z = CSng(n(2))
                gun_tile(cnt).w = CSng(n(3))
                cnt += 1
            Else
                gun_tile(cnt).x = 1.0
                gun_tile(cnt).y = 1.0
                gun_tile(cnt).z = 0.0
                gun_tile(cnt).w = 0.0
                cnt += 1

            End If
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve guns(cnt)
        ReDim Preserve gun_tile(cnt)
        cnt = 0
        '-------------------------------------------------------
        '----- turret tiling
        Try
            tbl = t.Tables("turret_tiling")

            Dim q25 = From row In tbl.AsEnumerable _
                        Select _
                        tile = row.Field(Of String)("tiling")

            For Each thing In q25
                Dim n = thing.Split(" ")
                turret_tile(cnt).x = CSng(n(0))
                turret_tile(cnt).y = CSng(n(1))
                turret_tile(cnt).z = CSng(n(2))
                turret_tile(cnt).w = CSng(n(3))
                cnt += 1
            Next
            ReDim Preserve turret_tile(cnt)
            cnt = 0
        Catch ex As Exception
            tbl = t.Tables("tiling")

            Dim q25 = From row In tbl.AsEnumerable _
                        Select _
                        tile = row.Field(Of String)("tiling_Text")

            For Each thing In q25
                Dim n = thing.Split(" ")
                turret_tile(cnt).x = CSng(n(0))
                turret_tile(cnt).y = CSng(n(1))
                turret_tile(cnt).z = CSng(n(2))
                turret_tile(cnt).w = CSng(n(3))
                cnt += 1
            Next
            ReDim Preserve turret_tile(cnt)
            cnt = 0

        End Try
        '-------------------------------------
        '----- turrets
        tbl = t.Tables("turret_model")
        If tbl Is Nothing Then

        Else

            Dim q1 = From row In tbl.AsEnumerable _
                Select _
                turret = row.Field(Of String)("model")

            For Each r0 In q1
                turrets(cnt) = r0
                cnt += 1
            Next
            If cnt = 0 Then
                bad_tanks.AppendLine(file_name)
                Return
            End If
            ReDim Preserve turrets(cnt)
        End If
        cnt = 0
        '-------------------------------------------------------
        'setup treeview and its nodes
        Dim selected_turret, selected_gun As Integer
        If Not save_tank Then
            frmComponents.tv_guns.Nodes.Clear()
            frmComponents.tv_turrets.Nodes.Clear()

            Dim cn As Integer = 0
            For i = 0 To guns.Length - 2
                If validate_path(guns(i)) = guns(i) Then
                    Dim n = New TreeNode

                    n.Text = Path.GetFileNameWithoutExtension(guns(i))
                    If guns(i).Contains("_skins") Then GoTo n_gun
                    n.Tag = i
                    'tv_guns.Nodes.Add(n)
                    frmComponents.tv_guns.Nodes.Add(n)
                    cn += 1
n_gun:
                End If
            Next
            'frmComponents.tv_guns.Nodes.Add(tv_guns)
            frmComponents.tv_guns.SelectedNode = frmComponents.tv_guns.Nodes(cn - 1)
            frmComponents.tv_guns.SelectedNode.Checked = True
            selected_gun = cn
            '-------------------------------------------------------
            cn = 0
            For i = 0 To turrets.Length - 2
                If validate_path(turrets(i)) = turrets(i) Then
                    Dim n = New TreeNode
                    n.Text = Path.GetFileNameWithoutExtension(turrets(i))
                    If turrets(i).Contains("_skins") Then GoTo n_turret
                    n.Tag = i
                    frmComponents.tv_turrets.Nodes.Add(n)
                    cn += 1
n_turret:
                End If
            Next
            frmComponents.tv_turrets.SelectedNode = frmComponents.tv_turrets.Nodes(cn - 1)
            frmComponents.tv_turrets.SelectedNode.Checked = True
            selected_turret = cn
            '-------------------------------------------------------
            If frmFBX.Visible Then ' if fbx export form is visble, place the components form next to it
                Dim l = frmFBX.Location
                l.X -= frmComponents.Width
                frmComponents.Location = l
            Else
                frmComponents.Location = Me.Location + New Point(200, 200)
            End If
            frmComponents.ShowDialog(Me)
            If frmFBX.Visible Then
                frmFBX.Location = Me.Location
            End If
        End If
        '----- chassis

        tbl = t.Tables("chassis")
        Dim q2 = From row In tbl.AsEnumerable _
            Select _
            chass = row.Field(Of String)("model")
        For Each thing In q2

            chassis(cnt) = thing
            cnt += 1
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve chassis(cnt)
        cnt = 0
        '-------------------------------------------------------
        '----- hull
        tbl = t.Tables("hull")
        Dim q3 = From row In tbl.AsEnumerable
                Select _
                model = row.Field(Of String)("model"), _
                tile = row.Field(Of String)("hull_camouflage")

        For Each thing In q3
            hulls(cnt) = thing.model
            hull_tile(cnt) = New vect4
            Dim n = thing.tile.Split(" ")
            hull_tile(cnt).x = CSng(n(0))
            hull_tile(cnt).y = CSng(n(1))
            hull_tile(cnt).z = CSng(n(2))
            hull_tile(cnt).w = CSng(n(3))
            cnt += 1
        Next
        If cnt = 0 Then
            bad_tanks.AppendLine(file_name)
            Return
        End If
        ReDim Preserve hulls(cnt)
        ReDim Preserve hull_tile(cnt)
        cnt = 0
        '-------------------------------------------------------
        'Array.Sort(guns)
        'Array.Sort(turrets)
        'Array.Sort(hulls)
        'Array.Sort(chassis)
        'more hacks to deal with turret names
        Dim turret_name As String
        Try
            turret_name = turrets(turrets.Length - 3)
        Catch ex1 As Exception
            Try
                turret_name = turrets(turrets.Length - 2)
            Catch ex2 As Exception
                turret_name = turrets(turrets.Length - 1)
            End Try

        End Try
        turret_tiling = turret_tile(turrets.Length - 2)
        Dim hull_name = hulls(hulls.Length - 2)
        hull_tiling = hull_tile(hulls.Length - 2)
        Dim chassis_name = chassis(chassis.Length - 2)
        Dim gun_name As String = ""
        Dim ti, tj As New vect4
        If guns.Length = 10 Then
            gun_name = guns(guns.Length - 2)
            ti = gun_tile(guns.Length - 2)
            tj = ti
            tj.w = ti.z
            tj.z = ti.w
            gun_tiling = tj
        Else
            gun_name = guns(guns.Length - 2)
            ti = gun_tile(guns.Length - 2)
            tj = ti
            tj.w = ti.z
            tj.z = ti.w
            gun_tiling = tj
        End If
        '========================================
        Dim nation_string As String = ""
        Select Case ar(1)
            Case "american"
                CURRENT_DATA_SET = 0
                nation_string = "usa"
            Case "british"
                CURRENT_DATA_SET = 1
                nation_string = "uk"
            Case "chinese"
                CURRENT_DATA_SET = 2
                nation_string = "china"
            Case "czech"
                CURRENT_DATA_SET = 3
                nation_string = "czech"
            Case "french"
                CURRENT_DATA_SET = 4
                nation_string = "france"
            Case "german"
                CURRENT_DATA_SET = 5
                nation_string = "germany"
            Case "japan"
                CURRENT_DATA_SET = 6
                nation_string = "japan"
            Case "poland"
                CURRENT_DATA_SET = 7
                nation_string = "poland"
            Case "russian"
                CURRENT_DATA_SET = 8
                nation_string = "ussr"
            Case "sweden"
                CURRENT_DATA_SET = 9
                nation_string = "sweden"
            Case "italy"
                CURRENT_DATA_SET = 10
                nation_string = "italy"
        End Select
        TANK_NAME = "vehicles\" + ar(1) + "\" + ar(2) + ":" + current_tank_package.ToString
        '===================================
        Dim d = custom_tables(CURRENT_DATA_SET).Copy
        '===================================

        Dim tt = d.Tables("armorcolor")
        Dim qq = From row In tt.AsEnumerable
        Select _
        armorC = row.Field(Of String)("aColor")
        ARMORCOLOR = get_vect4(qq(0))
        tt.Dispose()

        'clear tank variables
        gun_trans = New vect3
        gun_trans2 = New vect3
        turret_trans = New vect3
        hull_trans = New vect3
        gun_location = New vect3
        turret_location = New vect3
        'make sure visiblity check boxes are checked
        If Not chassis_cb.Checked Then
            chassis_cb.Checked = True
        End If
        If Not hull_cb.Checked Then
            hull_cb.Checked = True
        End If
        If Not turret_cb.Checked Then
            turret_cb.Checked = True
        End If
        If Not gun_cb.Checked Then
            gun_cb.Checked = True
        End If
        '-------------------------------------------------------
        If TESTING Then

            'test stuff to grab track stuff
            tbl = t.Tables("track_info")
            Dim tq = From row In tbl.AsEnumerable
                        Select _
                        seg_cnt = row.Field(Of String)("seg_cnt")


            If tq(0).Contains("1") Then
                track_info.segment_count = 1
                Dim t1q = From row In tbl.AsEnumerable
                            Select _
                            trp = row.Field(Of String)("right_filename"), _
                            tlp = row.Field(Of String)("left_filename"), _
                            seglength = row.Field(Of String)("segment_length"), _
                            seg_off = row.Field(Of String)("segmentOffset")
                For Each tr In t1q
                    track_info.left_path1 = tr.tlp
                    track_info.right_path1 = tr.trp
                    track_info.segment_length = tr.seglength
                    track_info.segment_offset1 = tr.seg_off
                Next

            Else
                track_info.segment_count = 2
                Dim t1q = From row In tbl.AsEnumerable
                            Select _
                            trp = row.Field(Of String)("right_filename"), _
                            tlp = row.Field(Of String)("left_filename"), _
                            seglength = row.Field(Of String)("segment_length"), _
                            seg_off = row.Field(Of String)("segmentOffset"), _
                            trp2 = row.Field(Of String)("right2_filename"), _
                            tlp2 = row.Field(Of String)("left2_filename"), _
                            seg_off2 = row.Field(Of String)("segment2Offset")
                For Each tr In t1q
                    track_info.left_path1 = tr.tlp
                    track_info.right_path1 = tr.trp
                    track_info.left_path2 = tr.tlp2
                    track_info.right_path2 = tr.trp2
                    track_info.segment_length = tr.seglength
                    track_info.segment_offset1 = tr.seg_off
                    track_info.segment_offset2 = tr.seg_off2
                Next

            End If



            Dim tra() = chassis_name.Split("/")
            Dim track_path = Path.GetDirectoryName(track_info.left_path1) + "\right.track"
            Dim tent = packages(current_tank_package)(track_path)
            Dim t_data As New DataSet
            If tent IsNot Nothing Then
                Dim ms As New MemoryStream
                tent.Extract(ms)
                openXml_stream(ms, "right.track")
                get_track_section()

            End If
            If track_info.segment_offset2 > track_info.segment_offset1 Then
                Dim t_seg = track_info.segment_offset1
                track_info.segment_offset1 = track_info.segment_offset2
                track_info.segment_offset2 = t_seg
            End If
            running = 0
            path_pointer1 = 0
            track_length = 0
            For i = 0 To tracks.Length - 1
                catmullrom.CatmullRomSpline_get_length(i)
            Next
            Dim lenS = running / track_info.segment_length
            If Z_Flipped Then
                lenS -= 1.0
            Else
                lenS -= 1.0

            End If
            segment_length_adjusted = running / (Floor(lenS))
            Dim refact = track_info.segment_length / segment_length_adjusted
            track_info.segment_offset1 /= refact
            track_info.segment_offset2 /= refact
            Dim half = track_info.segment_offset2
            '========= segment 1 =========
            ReDim path_data1(CInt(Floor(lenS)) + 3)
            If Z_Flipped Then
                running = 0 + track_info.segment_offset1 + half
            Else
                running = 0 + track_info.segment_offset1 + half
            End If
            GC.Collect()
            GC.WaitForFullGCComplete()
            path_pointer1 = 0
            track_length = 0
            For i = 0 To tracks.Length - 1
                catmullrom.GetCatmullRomSpline1(i)
            Next
            ReDim Preserve path_data1(path_pointer1)
            get_tread_rotations1()
            '========= segment 2 =========
            If track_info.segment_count = 2 Then
                ReDim path_data2(CInt(Floor(lenS)) + 3)
                track_length = 0
                If Z_Flipped Then
                    running = 0 + track_info.segment_offset2 + half
                Else
                    running = 0 + track_info.segment_offset2 + half
                End If
                path_pointer2 = 0
                For i = 0 To tracks.Length - 1
                    catmullrom.GetCatmullRomSpline2(i)
                Next
                ReDim Preserve path_data2(path_pointer2)
                get_tread_rotations2()
            End If

        End If
        '================================= end testing
        '======================================================================================


        'Start of tank loading
        '======================================================================================
        If Not LOADING_FBX Then
            frmComponentView.clear_fbx_list()
        End If
        frmComponentView.clear_group_list()
        '======================================================================================


        loaded_from_resmods = False
        file_name = chassis_name
        Dim LOAD_ERROR As Boolean = True

        LOAD_ERROR = LOAD_ERROR And build_primitive_data(False) ' -- chassis
        If stop_updating Then draw_scene()

        file_name = hull_name
        LOAD_ERROR = LOAD_ERROR And build_primitive_data(True) ' -- hull
        If stop_updating Then draw_scene()

        If save_tank Then

            file_name = turret_name
            LOAD_ERROR = LOAD_ERROR And build_primitive_data(True) ' -- turret

            For gn = guns.Length - 2 To 0 Step -1
                file_name = guns(gn)
                gun_name = file_name
                If LOAD_ERROR = LOAD_ERROR And build_primitive_data(True) Then ' -- gun
                    If stop_updating Then draw_scene()
                    Exit For
                End If

            Next
        Else
            file_name = turrets(frmComponents.tv_turrets.SelectedNode.Tag)
            LOAD_ERROR = LOAD_ERROR And build_primitive_data(True) ' -- turret
            If stop_updating Then draw_scene()

            file_name = guns(frmComponents.tv_guns.SelectedNode.Tag)
            LOAD_ERROR = LOAD_ERROR And build_primitive_data(True) ' -- gun
            If stop_updating Then draw_scene()

        End If


        If TESTING Then
            file_name = track_info.left_path1
            LOAD_ERROR = LOAD_ERROR And build_primitive_data(True) ' -- track seg
            If stop_updating Then draw_scene()

            If track_info.segment_count = 2 Then
                file_name = track_info.left_path2
                LOAD_ERROR = LOAD_ERROR And build_primitive_data(True) ' -- track seg
                If stop_updating Then draw_scene()
            End If

        End If


        MODEL_LOADED = True
        If Not LOAD_ERROR Then
            'MODEL_LOADED = False
            'clean_house()
            Return
        End If
        part_counts = New part_counts_
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("chassis") Then
                part_counts.chassis_cnt += _object(i).count
            End If
            If _object(i).name.ToLower.Contains("hull") Then
                part_counts.hull_cnt += _object(i).count
            End If
            If _object(i).name.ToLower.Contains("turret") Then
                part_counts.turret_cnt += _object(i).count
            End If
            If _object(i).name.ToLower.Contains("gun") Then
                part_counts.gun_cnt += _object(i).count
            End If

        Next
        '####################################
        'All the tank parts are loaded so
        'lets create the color picking lists.
        'This should speed up color picking a lot.
        Dim r, b, g, a As Byte
        For i = 1 To object_count
            Dim cpl = Gl.glGenLists(1)
            _object(i).vertex_pick_list = cpl
            Gl.glNewList(cpl, Gl.GL_COMPILE)
            a = i + 10
            Gl.glEnable(Gl.GL_CULL_FACE)
            If _group(i).is_carraige Then
                Gl.glFrontFace(Gl.GL_CW)
            Else
                Gl.glFrontFace(Gl.GL_CCW)
            End If
            If _object(i).visible Then
                Gl.glBegin(Gl.GL_TRIANGLES)
                For k As UInt32 = 1 To _object(i).count
                    Dim v1 = _object(i).tris(k).v1
                    Dim v2 = _object(i).tris(k).v2
                    Dim v3 = _object(i).tris(k).v3
                    r = k And &HFF
                    g = (k And &HFF00) >> 8
                    b = (k And &HFF0000) >> 16
                    Gl.glColor4ub(r, g, b, a)
                    Gl.glVertex3f(v1.x, v1.y, v1.z)
                    Gl.glVertex3f(v2.x, v2.y, v2.z)
                    Gl.glVertex3f(v3.x, v3.y, v3.z)
                Next
                Gl.glEnd()
            End If
            Gl.glEndList()
        Next
        log_text.Append(" ======== Model Load Complete =========" + vbCrLf)
        For i = 1 To object_count - 1
            tank_center_X += _object(i).center_x
            tank_center_Y += _object(i).center_y
            tank_center_Z += _object(i).center_z
        Next
        tank_center_X /= object_count
        tank_center_Y /= object_count
        tank_center_Z /= object_count
        look_point_x = tank_center_X
        look_point_y = tank_center_Y
        look_point_z = tank_center_Z

        If save_tank Then
            'get rotation limits for the turret and gun
            rot_limit_l = -400.0
            rot_limit_r = 400.0

            Dim ent = scripts_pkg("scripts\item_defs\vehicles\" + nation_string + "\" + ar(2) + ".xml")
            Dim mss = New MemoryStream
            ent.Extract(mss)
            openXml_stream(mss, "")
            mss.Dispose()
            Dim docx = XDocument.Parse(TheXML_String)
            Dim xmlroot As XmlNode = xDoc.CreateElement(XmlNodeType.Element, "root", "")
            'doc.DocumentElement.ParentNode.Value = "<root>" + vbCrLf + "</root>"
            For Each n As XElement In docx.Descendants("turretYawLimits")
                n.Value = n.Value.Replace("/", "\")
                Dim ar2 = n.Value.Split(" ")
                rot_limit_l = ar2(0)
                rot_limit_r = ar2(1)
                Exit For
            Next

            For Each n As XElement In docx.Descendants("minPitch")
                n.Value = n.Value.Replace("/", "\")
                Dim ar2 = n.Value.Split(" ")
                gun_limit_u = -ar2(1)
                Exit For
            Next
            For Each n As XElement In docx.Descendants("maxPitch")
                n.Value = n.Value.Replace("/", "\")
                Dim ar2 = n.Value.Split(" ")
                gun_limit_d = -ar2(1)
                Exit For
            Next

            If rot_limit_l = -180 Then rot_limit_l = -400
            If rot_limit_r = 180 Then rot_limit_r = 400



            Dim fo = File.Open(TankListTempFolder + ar(2) + ".tank", FileMode.OpenOrCreate)
            Dim fw As New BinaryWriter(fo)
            'version changes
            'ver 1 

            Dim version As Integer = 1
            Dim rotation_limit As Single = 0.0
            'ver 1
            Dim s1 = "File format: 1 INT32 as version, INT32 as chassis and hull vertex count, INT32 as turret vertex count, INT32 as Gun vertex Count."
            Dim s2 = "3 Floats turret pivot center XYZ, " + _
                    "2 Floats rotation limits L&R,"
            Dim s3 = "3 Floats gun pivot point XYZ , 2 Floats gun limits U&D, " + _
                    "6 Floats as list of vertices:Each being (position XYZ Normal XYZ), "
            Dim s4 = "9 Floats for future use."
            fw.Write(s1)
            fw.Write(s2)
            fw.Write(s3)
            fw.Write(s4)
            fw.Write(version)

            fw.Write(part_counts.chassis_cnt + part_counts.hull_cnt)
            fw.Write(part_counts.turret_cnt)
            fw.Write(part_counts.gun_cnt)
            'turret info
            fw.Write(turret_location.x)
            fw.Write(turret_location.y)
            fw.Write(turret_location.z)
            fw.Write(rot_limit_l)
            fw.Write(rot_limit_r)
            'gun info
            fw.Write(gun_location.x)
            fw.Write(gun_location.y)
            fw.Write(gun_location.z)
            fw.Write(gun_limit_u)
            fw.Write(gun_limit_d)
            'extra vects
            '1
            fw.Write(1.0!)
            fw.Write(1.0!)
            fw.Write(1.0!)
            '2
            fw.Write(1.0!)
            fw.Write(1.0!)
            fw.Write(1.0!)
            '3
            fw.Write(1.0!)
            fw.Write(1.0!)
            fw.Write(1.0!)

            For i = 1 To object_count
                If _object(i).name.ToLower.Contains("chassis") Then
                    write_vertex_data(_object(i), fw)
                End If
                If _object(i).name.ToLower.Contains("hull") Then
                    write_vertex_data(_object(i), fw)
                End If
                If _object(i).name.ToLower.Contains("turret") Then
                    write_vertex_data(_object(i), fw)
                End If
                If _object(i).name.ToLower.Contains("gun") Then
                    write_vertex_data(_object(i), fw)
                End If

            Next

            fo.Close()
        End If
        t.Dispose()
        tbl.Dispose()
        GC.Collect()
        If FBX_LOADED Then
            m_show_fbx.Visible = True
            m_show_fbx.Checked = False
        End If
        m_pick_camo.Enabled = True
        m_hide_show_components.Enabled = True
        m_set_vertex_winding_order.Enabled = True

    End Sub

    Private Sub write_vertex_data(ByVal o As obj, ByVal fw As BinaryWriter)
        For i As Integer = 1 To o.count
            '1
            fw.Write(o.tris(i).v1.x)
            fw.Write(o.tris(i).v1.y)
            fw.Write(o.tris(i).v1.z)
            fw.Write(o.tris(i).n1.x)
            fw.Write(o.tris(i).n1.y)
            fw.Write(o.tris(i).n1.z)

            '2
            fw.Write(o.tris(i).v2.x)
            fw.Write(o.tris(i).v2.y)
            fw.Write(o.tris(i).v2.z)
            fw.Write(o.tris(i).n2.x)
            fw.Write(o.tris(i).n2.y)
            fw.Write(o.tris(i).n2.z)

            '3
            fw.Write(o.tris(i).v3.x)
            fw.Write(o.tris(i).v3.y)
            fw.Write(o.tris(i).v3.z)
            fw.Write(o.tris(i).n3.x)
            fw.Write(o.tris(i).n3.y)
            fw.Write(o.tris(i).n3.z)

        Next
    End Sub

    Private Sub get_track_section()

        Dim docx = XDocument.Parse(TheXML_String.Replace("matrix", "position"))
        Dim doc As New XmlDocument
        Dim xmlroot As XmlNode = xDoc.CreateElement(XmlNodeType.Element, "root", "")
        Dim root_node As XmlNode = doc.CreateElement("model")
        doc.AppendChild(root_node)

        For Each node In docx.Descendants("node")
            Dim node_ = doc.CreateElement("node")
            Dim name = doc.CreateElement("name")
            Dim matrix = doc.CreateElement("matrix")
            For Each n In node.Descendants("name")
                name.InnerText = n.Value.ToString
                node_.AppendChild(name)
            Next
            For Each mat In node.Descendants("position")
                matrix.InnerText = mat.Value.ToString.Replace("position", "")
                node_.AppendChild(matrix)
            Next
            root_node.AppendChild(node_)
        Next


        Dim fm As New MemoryStream
        doc.Save(fm)
        fm.Position = 0
        Dim data_set As New DataSet
        data_set.ReadXml(fm)

        Dim t = data_set.Tables("node")
        Dim q = From row In t.AsEnumerable
                Select _
                Name = row.Field(Of String)("name"), _
            Matrix = row.Field(Of String)("matrix")
        ' id = row.Field(Of String)("id"), _

        ReDim tracks(q.Count - 1)

        Dim cnt As Integer = 0
        For Each trk In q
            tracks(cnt) = New track_
            ReDim tracks(cnt).matrix(15)
            tracks(cnt).name = trk.Name
            ' tracks(cnt).id = trk.id
            Dim ar = trk.Matrix.Split(" ")
            Dim j As Integer = 0
            If ar.Length > 3 Then
                Dim mm(15) As Single
                For Each m In ar

                    If CSng(m) > 1.0 Or CSng(m) < -1.0 Then
                        mm(j) = CSng(m) * 0.01
                    Else
                        mm(j) = CSng(m)
                    End If
                    j += 1
                    If j = 16 Then Exit For
                Next
                tracks(cnt).position.X = mm(3)
                tracks(cnt).position.Y = mm(7)
                tracks(cnt).position.Z = -mm(11)

            Else
                j = 0
                Dim mm(15) As Single
                For Each m In ar

                    mm(j) = CSng(m)
                    j += 1
                    If j = 3 Then Exit For
                Next
                tracks(cnt).position.X = mm(0)
                tracks(cnt).position.Y = mm(1)
                tracks(cnt).position.Z = -mm(2)

            End If
            cnt += 1
        Next
        'check if we need to flip the Z on this track.
        If tracks(0).position = tracks(tracks.Length - 1).position Then
            ReDim Preserve tracks(tracks.Length - 2)
        End If
        Dim vv = tracks(0).position

        Z_Flipped = False
        If vv.Z < 0 Then
            For i = 0 To tracks.Length - 1
                tracks(i).position.Z *= -1.0
            Next
            Z_Flipped = True
        End If
        If vv.Y < 0 Then
            For i = 0 To tracks.Length - 1
                tracks(i).position.Z *= -1.0
            Next
        End If
        data_set.Dispose()

        fm.Dispose()


    End Sub

    Private Sub get_tread_rotations1()
        For i = 0 To path_pointer1 - 1
            path_data1(check_pos(i + 1, path_pointer1)).angle = 0
            path_data1(check_pos(i + 1, path_pointer1)).zc = 0
            path_data1(check_pos(i + 1, path_pointer1)).yc = 0
        Next
        For i = -2 To path_pointer1 - 1
            path_data1(check_pos(i + 1, path_pointer1)).angle = 0
            path_data1(check_pos(i + 1, path_pointer1)).zc = 0
            path_data1(check_pos(i + 1, path_pointer1)).yc = 0
            get_angle_and_center(i, path_data1, path_pointer1)
        Next

    End Sub
    Private Sub get_tread_rotations2()
        For i = 0 To path_pointer2 - 1
            path_data2(check_pos(i + 1, path_pointer2)).angle = 0
            path_data2(check_pos(i + 1, path_pointer2)).zc = 0
            path_data2(check_pos(i + 1, path_pointer2)).yc = 0
        Next
        For i = -1 To path_pointer2 - 1
            path_data2(check_pos(i + 1, path_pointer2)).angle = 0
            path_data2(check_pos(i + 1, path_pointer2)).zc = 0
            path_data2(check_pos(i + 1, path_pointer2)).yc = 0
            get_angle_and_center(i, path_data2, path_pointer2)
        Next

    End Sub
    Private Sub get_angle_and_center(ByVal pos As Integer, ByRef path_data() As path_data_, ByVal path_pointer As Integer)
        Dim yc, zc As Single
        Dim y1, z1, y2, z2, y3, z3 As Single
        Dim direction As Integer
        Dim cnt As Integer
        Dim z__ As Decimal

        Dim p1, p2, p3 As SlimDX.Vector3

        p1 = path_data(check_pos(pos, path_pointer)).pos1
        p2 = path_data(check_pos(pos + 1, path_pointer)).pos1
        p3 = path_data(check_pos(pos + 2, path_pointer)).pos1
        'gotta flip y and z for this old algo to work
        Dim rf As Integer = 3
        y1 = Round(p1.Y, rf)
        z1 = Round(p1.Z, rf)
        y2 = Round(p2.Y, rf)
        z2 = Round(p2.Z, rf)
        y3 = Round(p3.Y, rf)
        z3 = Round(p3.Z, rf)
        Dim s As Single = 0.5D * ((y2 - y3) * (y1 - y3) - (z2 - z3) * (z3 - z1))
        Dim sUnder As Single = (y1 - y2) * (z3 - z1) - (z2 - z1) * (y1 - y3)
        If sUnder <> 0 Then

            s /= sUnder

            yc = Round(0.5D * (y1 + y2) + s * (z2 - z1), 3) ' center y coordinate
            zc = Round(0.5D * (z1 + z2) + s * (y1 - y2), 3)  ' center y coordinate
        End If
        Dim radius As Single = CSng(Round((Sqrt(((y3 - yc) * (y3 - yc)) + ((z3 - zc) * (z3 - zc)))), 5))

        z__ = (y2 - y1) * (z3 - z2)
        z__ -= (z2 - z1) * (y3 - y2)
        If z__ < 0 Then
            cnt -= 1
        Else
            If z__ > 0 Then
                cnt += 1
            End If
        End If
        If z__ = 0 Then
            direction = 0
        End If
        If cnt > 0 Then
            direction = 3
        Else
            direction = 2
        End If
        Dim agl = Round(mAtan2(z2 - z1, y2 - y1), 6)
        If zc = 0 And yc = 0 Then
            path_data(check_pos(pos + 1, path_pointer)).angle = agl * 57.29577
            path_data(check_pos(pos + 1, path_pointer)).zc = z2
            path_data(check_pos(pos + 1, path_pointer)).yc = y2 + 0.25
            Return
            'End If
        End If
        Dim dyr1 As Single = CSng(Round(y1 - yc, 6))
        Dim dy2 As Single = CSng(Round(y3 - yc, 6))
        Dim dzr1 As Single = CSng(Round(z1 - zc, 6))
        Dim dz2 As Single = CSng(Round(z2 - zc, 6))
        Dim dy1 As Single = CSng(Round(y1 - yc, 6))
        Dim dz1 As Single = CSng(Round(z2 - zc, 6))

        Dim r1 = Sqrt((dyr1 * dyr1) + (dzr1 * dzr1))

        Dim sa = Round(mAtan2(z1 - z2, y1 - y2), 6)
        Dim ea = Round(mAtan2(z2 - z3, y2 - y3), 6)
        Dim angle = Round(mAtan2(z2 - zc, y2 - yc), 6)
        If direction = 3 Then
            angle += PI / 2.0
        Else
            angle -= PI / 2.0
        End If

        path_data(check_pos(pos + 1, path_pointer)).angle = angle * 57.29577
        path_data(check_pos(pos + 1, path_pointer)).zc = zc
        path_data(check_pos(pos + 1, path_pointer)).yc = yc
        If direction = 0 Then
            Stop
        End If
        Return



    End Sub
    Private Function check_pos(ByVal p As Integer, ByVal path_pointer As Integer)
        If p > path_pointer - 1 Then
            p -= (path_pointer - 1)
        End If
        If p < 0 Then
            p += (path_pointer - 1)
        End If
        Return p
    End Function
    Private Function mAtan2(ByVal y As Single, ByVal x As Single) As Single
        Dim theta As Single
        theta = CSng(Atan2(y, x))
        If theta < 0 Then
            theta += CSng((PI * 2))
        End If
        Return theta
    End Function


    Private Sub clear_node_selection(ByRef n As TreeNode)
        If n.ForeColor = Color.White Then
            n.ForeColor = Color.Black
        End If
    End Sub


    Private Sub set_node_white(ByRef n As TreeNode)
        n.ForeColor = Color.White
    End Sub
    Private Sub set_node_black(ByRef n As TreeNode)
        n.ForeColor = Color.Black
    End Sub

    Private Sub get_tank_xml_data(ByVal n As TreeNode)
        Dim q = From row In TankDataTable _
            Where row.Field(Of String)("tag") = n.Text _
    Select _
        un = row.Field(Of String)("shortname"), _
        tier = row.Field(Of String)("tier"), _
        natiom = row.Field(Of String)("nation"), _
        Type = row.Field(Of String)("type")
        Order By tier Descending

        'Dim a = q(0).un.Split(":")
        If q(0) IsNot Nothing Then
            out_string.Append(n.Text + ":" + q(0).un + ":" + q(0).natiom + ":" + q(0).tier + ":" + q(0).Type + ":")


        End If

    End Sub


    Private Sub export_camo()
        export_camo_to_res_mods("summer")
        export_camo_to_res_mods("winter")
        export_camo_to_res_mods("desert")

    End Sub
    Public Sub export_camo_to_res_mods(ByVal type As String)

        '===================================
        Dim d = custom_tables(CURRENT_DATA_SET).Copy
        Dim ar = TANK_NAME.Split(":")
        Dim t_name = Path.GetFileNameWithoutExtension(ar(0))
        '===================================
        Dim t = d.Tables("colors")
        Dim q = From row In t.AsEnumerable _
                Where type = row.Field(Of String)("kind") _
                Select _
                texture = row.Field(Of String)("texture"), _
                camoName = row.Field(Of String)("camoName")
        Try

        Catch ex As Exception
            t.Dispose()
            d.Dispose()
            MsgBox("This tank can not have camouflage appied to it!", MsgBoxStyle.Information, "Not Going to happen...")
            Return
        End Try

 
        Dim cnt As Integer = 0
        Dim z_path As String = My.Settings.res_mods_path

        For Each l In q
            If l.camoName IsNot Nothing Then
                Dim ent = frmMain.packages(11)(l.texture)
                ent.Extract(z_path, ExtractExistingFileAction.DoNotOverwrite)
                cnt += 1
            Else
                'Debug.WriteLine("missing camo: " + l.camoName)
            End If
        Next
        t.Dispose()
        d.Dispose()
    End Sub

    '==========================================
    Private Sub prep_tanks_xml(ByRef xml As String)
        xml = PrettyPrint(xml.Replace(">shared<", "><boobs></boobs><"))
        xml = xml.Replace("  ", vbTab)
        Dim ar = xml.Split(vbCrLf)
        Dim ts As String = ""
        For j = 0 To ar.Length - 2
            If ar(j).Contains("<bottom>") And ar(j).Contains("</bottom>") Then
                If Not ar(j).Contains("><") Then
                    ar(j) = ar(j).Replace("<bottom>", "<bottom>" + vbTab)
                    ar(j) = ar(j).Replace("</bottom>", vbTab + "</bottom>")
                End If

            End If
            If ar(j).Contains("<camouflage>") And ar(j).Contains("</camouflage>") Then
                If Not ar(j).Contains("><") Then
                    ar(j) = ar(j).Replace("<camouflage>", "<camouflage>" + vbTab)
                    ar(j) = ar(j).Replace("</camouflage>", vbTab + "</camouflage>")
                End If
            End If
            Dim hs As String = ""
            Dim a2 = ar(j).ToCharArray
            For k = 0 To a2.Length
                If a2(k) = Chr(9) Then
                    hs += a2(k)
                Else
                    Exit For
                End If
            Next
            If ar(j).Contains("<boobs></boobs>") Then
                ar(j) = ar(j).Replace(vbTab, "")
                ar(j + 1) = ar(j + 1).Replace(vbTab, "")
            End If
            If Not ar(j).Contains("<boobs></boobs>") Then
                If Not ar(j).Contains("commander") Then
                    If Not ar(j).Contains("driver") Then
                        If Not ar(j).Contains("gunner") Then
                            If Not ar(j).Contains("loader") Then
                                If Not ar(j).Contains("radioman") Then
                                    ar(j) = ar(j).Replace("><", ">" + vbCrLf + hs + "<")
                                End If
                            End If

                        End If

                    End If
                End If
            End If
            ts += ar(j) + vbCrLf
        Next
        ts = ts.Replace(">" + vbCrLf + "<boobs></boobs>" + vbCrLf + "<", ">shared<")

        xml = ts + ar(ar.Length - 1)
        If False Then

            xml = xml.Replace("<forward>", "<forward>" + vbTab)
            xml = xml.Replace("</forward>", vbTab + "</forward>")

            xml = xml.Replace("<backward>", "<backward>" + vbTab)
            xml = xml.Replace("</backward>", vbTab + "</backward>")

            'removing formating as I don't think any of this is needed.
            For z = 0 To 20
                xml = xml.Replace("<armor_" + z.ToString + ">", "<armor_" + z.ToString + ">" + vbTab)
                xml = xml.Replace("</armor_" + z.ToString + ">", vbTab + "</armor_" + z.ToString + ">")
                xml = xml.Replace(">" + vbTab + "</armor_" + z.ToString + ">", "></armor_" + z.ToString + ">")
            Next
            For z = 0 To 20
                xml = xml.Replace(vbTab + vbTab + "</armor_" + z.ToString + ">", vbTab + "</armor_" + z.ToString + ">")
            Next

            xml = xml.Replace("<vehicleDamageFactor>0.0<", vbTab + "<vehicleDamageFactor>0.0<")

            xml = xml.Replace("<turret>", "<turret>" + vbTab)
            xml = xml.Replace("</turret>", vbTab + "</turret>")

            xml = xml.Replace("<slotType>", "<slotType>" + vbTab)
            xml = xml.Replace("</slotType>", vbTab + "</slotType>")

            xml = xml.Replace("<slotId>", "<slotId>" + vbTab)
            xml = xml.Replace("</slotId>", vbTab + "</slotId>")

            xml = xml.Replace("<hideIfDamaged>", "<hideIfDamaged>" + vbTab)
            xml = xml.Replace("</hideIfDamaged>", vbTab + "</hideIfDamaged>")

            xml = xml.Replace("<isUVProportional>", "<isUVProportional>" + vbTab)
            xml = xml.Replace("</isUVProportional>", vbTab + "</isUVProportional>")

            xml = xml.Replace("<doubleSided>", "<doubleSided>" + vbTab)
            xml = xml.Replace("</doubleSided>", vbTab + "</doubleSided>")

            xml = xml.Replace("<showOn>", "<showOn>" + vbTab)
            xml = xml.Replace("</showOn>", vbTab + "</showOn>")

            xml = xml.Replace("<parentSlotId>", "<parentSlotId>" + vbTab)
            xml = xml.Replace("</parentSlotId>", vbTab + "</parentSlotId>")

            xml = xml.Replace("<paint>", "<paint>" + vbTab)
            xml = xml.Replace("</paint>", vbTab + "</paint>")

            xml = xml.Replace("<level>", "<level>" + vbTab)
            xml = xml.Replace("</level>", vbTab + "</level>")

            xml = xml.Replace("<price>", "<price>" + vbTab)
            xml = xml.Replace("</price>", vbTab + "</price>")

            xml = xml.Replace("<notInShop>", "<notInShop>" + vbTab)
            xml = xml.Replace("</notInShop>", vbTab + "</notInShop>")

            xml = xml.Replace("<leftTrack>", "<leftTrack>" + vbTab)
            xml = xml.Replace("</leftTrack>", vbTab + "</leftTrack>")

            xml = xml.Replace("<rightTrack>", "<rightTrack>" + vbTab)
            xml = xml.Replace("</rightTrack>", vbTab + "</rightTrack>")

            xml = xml.Replace("<weight>", "<weight>" + vbTab)
            xml = xml.Replace("</weight>", vbTab + "</weight>")

            xml = xml.Replace("<maxLoad>", "<maxLoad>" + vbTab)
            xml = xml.Replace("</maxLoad>", vbTab + "</maxLoad>")

            xml = xml.Replace("<brakeForce>", "<brakeForce>" + vbTab)
            xml = xml.Replace("</brakeForce>", vbTab + "</brakeForce>")

            xml = xml.Replace("<rotationSpeed>", "<rotationSpeed>" + vbTab)
            xml = xml.Replace("</rotationSpeed>", vbTab + "</rotationSpeed>")

            xml = xml.Replace("<maxHealth>", "<maxHealth>" + vbTab)
            xml = xml.Replace("</maxHealth>", vbTab + "</maxHealth>")

            xml = xml.Replace("<maxRegenHealth>", "<maxRegenHealth>" + vbTab)
            xml = xml.Replace("</maxRegenHealth>", vbTab + "</maxRegenHealth>")

            xml = xml.Replace("<isLeft>", "<isLeft>" + vbTab)
            xml = xml.Replace("</isLeft>", vbTab + "</isLeft>")

            xml = xml.Replace("<isLeading>", "<isLeading>" + vbTab)
            xml = xml.Replace("</isLeading>", vbTab + "</isLeading>")

            xml = xml.Replace("<radius>", "<radius>" + vbTab)
            xml = xml.Replace("</radius>", vbTab + "</radius>")

            xml = xml.Replace("<startIndex>", "<startIndex>" + vbTab)
            xml = xml.Replace("</startIndex>", vbTab + "</startIndex>")

            xml = xml.Replace("<count>", "<count>" + vbTab)
            xml = xml.Replace("</count>", vbTab + "</count>")

            xml = xml.Replace("<hullPosition>", "<hullPosition>" + vbTab)
            xml = xml.Replace("</hullPosition>", vbTab + "</hullPosition>")

            xml = xml.Replace("<maxClimbAngle>", "<maxClimbAngle>" + vbTab)
            xml = xml.Replace("</maxClimbAngle>", vbTab + "</maxClimbAngle>")

            xml = xml.Replace("<row0>", "<row0>" + vbTab)
            xml = xml.Replace("</row0>", vbTab + "</row0>")

            xml = xml.Replace("<row1>", "<row1>" + vbTab)
            xml = xml.Replace("</row1>", vbTab + "</row1>")

            xml = xml.Replace("<row2>", "<row2>" + vbTab)
            xml = xml.Replace("</row2>", vbTab + "</row2>")

            xml = xml.Replace("<row3>", "<row3>" + vbTab)
            xml = xml.Replace("</row3>", vbTab + "</row3>")

            xml = xml.Replace("<UTiles>", "<UTiles>" + vbTab)
            xml = xml.Replace("</UTiles>", vbTab + "</UTiles>")

            xml = xml.Replace("<VTiles>", "<VTiles>" + vbTab)
            xml = xml.Replace("</VTiles>", vbTab + "</VTiles>")

            xml = xml.Replace("<enable>", "<enable>" + vbTab)
            xml = xml.Replace("</enable>", vbTab + "</enable>")

            xml = xml.Replace("<linkBones>", "<linkBones>" + vbTab)
            xml = xml.Replace("</linkBones>", vbTab + "</linkBones>")

            xml = xml.Replace("<gravity>", "<gravity>" + vbTab)
            xml = xml.Replace("</gravity>", vbTab + "</gravity>")

            xml = xml.Replace("<elasticity>", "<elasticity>" + vbTab)
            xml = xml.Replace("</elasticity>", vbTab + "</elasticity>")

            xml = xml.Replace("<damping>", "<damping>" + vbTab)
            xml = xml.Replace("</damping>", vbTab + "</damping>")

            xml = xml.Replace("<segmentsCount>", "<segmentsCount>" + vbTab)
            xml = xml.Replace("</segmentsCount>", vbTab + "</segmentsCount>")

            xml = xml.Replace("<segmentsInnerThickness>", "<segmentsInnerThickness>" + vbTab)
            xml = xml.Replace("</segmentsInnerThickness>", vbTab + "</segmentsInnerThickness>")

            xml = xml.Replace("<back>", "<back>" + vbTab)
            xml = xml.Replace("</back>", vbTab + "</back>")

            xml = xml.Replace("<front>", "<front>" + vbTab)
            xml = xml.Replace("</front>", vbTab + "</front>")

            xml = xml.Replace("<length>", "<length>" + vbTab)
            xml = xml.Replace("</length>", vbTab + "</length>")

            xml = xml.Replace("<shiftVec>", "<shiftVec>" + vbTab)
            xml = xml.Replace("</shiftVec>", vbTab + "</shiftVec>")

            xml = xml.Replace("<jointIdx>", "<jointIdx>" + vbTab)
            xml = xml.Replace("</jointIdx>", vbTab + "</jointIdx>")

            xml = xml.Replace("<syncWithMainModel>", "<syncWithMainModel>" + vbTab)
            xml = xml.Replace("</syncWithMainModel>", vbTab + "</syncWithMainModel>")

            xml = xml.Replace("<teethCount>", "<teethCount>" + vbTab)
            xml = xml.Replace("</teethCount>", vbTab + "</teethCount>")

            xml = xml.Replace("<circularVisionRadius>", "<circularVisionRadius>" + vbTab)
            xml = xml.Replace("</circularVisionRadius>", vbTab + "</circularVisionRadius>")

            xml = xml.Replace("<gun>", "<gun>" + vbTab)
            xml = xml.Replace("</gun>", vbTab + "</gun>")

            xml = xml.Replace("<aimingTime>", "<aimingTime>" + vbTab)
            xml = xml.Replace("</aimingTime>", vbTab + "</aimingTime>")

            xml = xml.Replace("<animateEmblemSlots>", "<animateEmblemSlots>" + vbTab)
            xml = xml.Replace("</animateEmblemSlots>", vbTab + "</animateEmblemSlots>")

            xml = xml.Replace("<applyToFabric>", "<applyToFabric>" + vbTab)
            xml = xml.Replace("</applyToFabric>", vbTab + "</applyToFabric>")

            xml = xml.Replace("<showEmblemsOnGun>", "<showEmblemsOnGun>" + vbTab)
            xml = xml.Replace("</showEmblemsOnGun>", vbTab + "</showEmblemsOnGun>")

            xml = xml.Replace("<clipAngle>", "<clipAngle>" + vbTab)
            xml = xml.Replace("</clipAngle>", vbTab + "</clipAngle>")

            xml = xml.Replace("<anchorPosition>", "<anchorPosition>" + vbTab)
            xml = xml.Replace("</anchorPosition>", vbTab + "</anchorPosition>")

            xml = xml.Replace("<anchorDirection>", "<anchorDirection>" + vbTab)
            xml = xml.Replace("</anchorDirection>", vbTab + "</anchorDirection>")

            xml = xml.Replace("<smplEnginePower>", "<smplEnginePower>" + vbTab)
            xml = xml.Replace("</smplEnginePower>", vbTab + "</smplEnginePower>")

            xml = xml.Replace("<smplFwMaxSpeed>", "<smplFwMaxSpeed>" + vbTab)
            xml = xml.Replace("</smplFwMaxSpeed>", vbTab + "</smplFwMaxSpeed>")

            xml = xml.Replace("<maxAmmo>", "<maxAmmo>" + vbTab)
            xml = xml.Replace("</maxAmmo>", vbTab + "</maxAmmo>")

            xml = xml.Replace("<rotationIsAroundCenter>", "<rotationIsAroundCenter>" + vbTab)
            xml = xml.Replace("</rotationIsAroundCenter>", vbTab + "</rotationIsAroundCenter>")

        End If

        xml = xml.Replace(vbTab + vbCrLf, vbCrLf)

    End Sub
    Public Sub extract_selections()
        If Not My.Settings.res_mods_path.ToLower.Contains("res_mods") Then
            If MsgBox("You need to set the path to the res_mods folder!" + vbCrLf + _
                    "Set it Now and continue?" + vbCrLf + _
                    "It should be something like this:" + vbCrLf + _
                    "C:\Games\World_of_Tanks\res_mods\1.5.1.0", MsgBoxStyle.YesNo, "Opps..") = MsgBoxResult.Yes Then
                m_res_mods_path.PerformClick()
                If Not My.Settings.res_mods_path.ToLower.Contains("res_mods") Then
                    Return
                End If
            Else
                Return
            End If
        End If
        If frmExtract.m_export_camo_cb.Checked Then
            export_camo()
        End If

        Dim sb As New StringBuilder

        Try
            Dim all_lods As Boolean = False
            Dim models As Boolean = frmExtract.no_models.Checked
            If frmExtract.all_lods_rb.Checked Then
                all_lods = True
            Else
                all_lods = False
            End If
            Dim p As String = ""
            TC1.Enabled = False
            If Not file_name.Contains(":") Then ' fix the name if it has no package info
                file_name = "0:1:" + file_name.Replace("\", "/")
            End If

            sb.AppendLine("file_name: " + file_name)


            Dim ar = file_name.Split(":")
            '------------------------------------------
            '------------------------------------------
            'Hack to deal with tanks with weird path names...
            If ar(2).Contains("Fosh_B") Then
                ar(2) = Replace(ar(2), "Fosh_B", "Fosh_155")
            End If
            '------------------------------------------
            '------------------------------------------
            Dim tank_sr_name = Path.GetFileName(public_icon_path)
            If frmExtract.gui_cb.Checked Then
                Dim ic_160x100 = gui_pkg(public_icon_path)
                If ic_160x100 IsNot Nothing Then
                    ic_160x100.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                End If
                Dim an = tank_sr_name.Split("-")
                tank_sr_name = an(1)
                Dim sss = public_icon_path.Replace(an(0) + "-", "")
                Dim srs = Path.GetDirectoryName(sss) + public_icon_path.Replace(public_icon_path.Replace(an(1) + "-", ""), "/420x307/" + tank_sr_name)
                Dim ic_420x307 = gui_pkg(srs)
                If ic_420x307 IsNot Nothing Then
                    ic_420x307.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                End If
            End If
            If frmExtract.extract_item_def_cb.Checked Then
                Dim ts = itemDefXmlString
                Try ' catch any exception thrown

                    Dim ip = My.Settings.res_mods_path + "\" + itemDefPathString.Replace(" ", "")
                    prep_tanks_xml(itemDefXmlString)
                    itemDefXmlString = itemDefXmlString.Replace("  ", vbTab)
                    itemDefXmlString = itemDefXmlString.Replace("><", ">" + vbCrLf + "<")
                    itemDefXmlString = itemDefXmlString.Replace("<xmlref>", "<!--<xmlref>")
                    itemDefXmlString = itemDefXmlString.Replace("</xmlref>", "</xmlref>-->")
                    itemDefXmlString = itemDefXmlString.Replace("formfactor_rect1x4direction_left_to_right", "formfactor_rect1x4 direction_left_to_right")
                    If Not Directory.Exists(Path.GetDirectoryName(ip)) Then
                        Directory.CreateDirectory(Path.GetDirectoryName(ip))
                    End If
                    itemDefXmlString = itemDefXmlString.Replace("map_nation", Path.GetFileNameWithoutExtension(ip) + ".xml")
                    File.WriteAllText(ip, itemDefXmlString, Encoding.ASCII)
                Catch ex As Exception
                    itemDefXmlString = ts
                    MsgBox(file_name + vbCrLf + ex.Message, MsgBoxStyle.Critical, "Shit!!")
                    Return
                End Try

                itemDefXmlString = ts

            End If
            Dim tar = itemDefXmlString.Split("vbcrlf")
            Dim seg_path As String = ""
            For i = 0 To tar.Length - 1
                If tar(i).Contains("segmentModelLeft") Then
                    Dim sega = tar(i).Split("left>")
                    Dim sega1 = sega(1).Split("</")
                    seg_path = Path.GetDirectoryName(sega1(0)).Replace("\", "/")
                End If
            Next
            If seg_path = "" Then ' 'Dont allow a search with a empty string. It will find every thing!!
                seg_path = "donkey_breath"
            End If
            If frmExtract.m_customization.Checked Then ' export customization?

                Try ' catch any exception thrown

                    Dim nar = ar(2).Split("/")
                    Dim nation = nar(1)
                    Select Case nation
                        Case "russian"
                            nation = "ussr"
                        Case "british"
                            nation = "uk"
                        Case "american"
                            nation = "usa"
                        Case "french"
                            nation = "france"
                        Case "german"
                            nation = "germany"
                        Case "chinese"
                            nation = "china"
                    End Select
                    Dim cust_path As String = "scripts\item_defs\vehicles\" + nation + "\customization.xml"

                    sb.AppendLine("ar(2): " + ar(2))
                    sb.AppendLine("cust_path: " + cust_path)

                    Dim c_ent = scripts_pkg(cust_path)

                    sb.AppendLine("c_ent: " + c_ent.FileName)

                    If c_ent IsNot Nothing Then
                        c_ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                    End If
                Catch ex As Exception
                    MsgBox(file_name + vbCrLf + ex.Message, MsgBoxStyle.Critical, "NAR path")
                    Return
                End Try


            End If
            sb.AppendLine("My.Settings.res_mods_path: " + My.Settings.res_mods_path)

            'Throw New ArgumentException("Exception Occured")
            Dim crash As String = "crash"
            If CRASH_MODE Then
                crash = "donky_smuggler"
            End If
            '=============================================================
            'Look based on tanks name
            For i = 1 To packages.Length - 2
                search_and_extract(packages(i), ar(2))
            Next
            ' now check package_2 data
            Dim start_from As Integer = 5 ' This might change and its used in 2 places
            For i = start_from To 10
                search_and_extract(packages_2(i), ar(2))
            Next
            ' now check package_hd data
            For i = 1 To packages_HD.Length - 2
                search_and_extract(packages_HD(i), ar(2))
            Next
            'now check package_hd_2 data
            For i = start_from To packages_HD.Length - 2
                search_and_extract(packages_HD_2(i), ar(2))
            Next
            'now check built package
            search_and_extract(shared_contents_build, ar(2))
            '=============================================================
            'This has to be done because some tanks share the tracks with others.
            'I'll need to update the wotmod builder to deal with these or leave it?
            'look based on track path..
            For i = 1 To packages.Length - 2
                search_and_extract(packages(i), seg_path)
            Next
            ' now check package_2 data
            For i = start_from To 10
                search_and_extract(packages_2(i), seg_path)
            Next
            ' now check package_hd data
            For i = 1 To packages_HD.Length - 2
                search_and_extract(packages_HD(i), seg_path)
            Next
            'now check package_hd_2 data
            For i = start_from To packages_HD.Length - 2
                search_and_extract(packages_HD_2(i), seg_path)
            Next
            'now check built package
            search_and_extract(shared_contents_build, seg_path)


            If frmExtract.create_work_area_cb.Checked And Not frmExtract.no_textures.Checked Then
                p = My.Settings.res_mods_path + "\" + Path.GetDirectoryName(p)
                Dim wap = p + "\Work Area"
                Il.ilDisable(Il.IL_FILE_OVERWRITE) ' dont allow devil to overwrite existing PNGS.. Preserver the users work!
                If Not Directory.Exists(wap) Then
                    Directory.CreateDirectory(wap)
                    Dim di = Directory.GetFiles(p)
                    Dim id As Integer = 0
                    For Each img In di
                        If img.ToLower.Contains("_am_hd.dds") Or img.ToLower.Contains("_ao_hd.dds") Then
                            Dim tp = Path.GetDirectoryName(img)
                            Dim t_tn = Path.GetFileNameWithoutExtension(img)
                            Dim out_path As String = tp + "\Work Area\" + t_tn + ".png"
                            id = Il.ilGenImage()
                            Il.ilBindImage(id)
                            Ilu.iluLoadImage(img)
                            Il.ilSave(Il.IL_PNG, out_path)
                            Il.ilBindImage(0)
                            Il.ilDeleteImage(id)
                        End If

                    Next
                End If
            End If

        Catch ex As Exception
            'File.WriteAllText("C:\TE.txt", sb.ToString)
        End Try
        TC1.Enabled = True
    End Sub
    Private Sub search_and_extract(ByRef package As ZipFile, ByRef search_name As String)
        Dim crash As String = "crash"
        If CRASH_MODE Then
            crash = "donky_smuggler"
        End If
        Dim p As String = ""
        If package IsNot Nothing Then

            For Each ent In package
                If ent.FileName.Contains(search_name) Then
                    If Not ent.FileName.Contains("collision_client") Then
                        If Not ent.FileName.Contains(crash) Then
                            If Not frmExtract.no_models.Checked Then
                                Select Case frmExtract.all_lods_rb.Checked
                                    Case True
                                        If ent.FileName.ToLower.Contains("track") Then
                                            If frmExtract.ext_chassis.Checked Then
                                                If ent.FileName.ToLower.Contains("track") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                            End If
                                        End If
                                        Select Case frmExtract.ext_chassis.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("chassis") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_hull.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("hull") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_turret.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("turret") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                        Select Case frmExtract.ext_gun.Checked
                                            Case True
                                                If ent.FileName.ToLower.Contains("gun") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                        End Select
                                    Case False
                                        If ent.FileName.ToLower.Contains("track") Then
                                            If frmExtract.ext_chassis.Checked Then
                                                If ent.FileName.ToLower.Contains("track") Then
                                                    ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                End If
                                            End If
                                        End If
                                        If ent.FileName.ToLower.Contains("lod0") Then
                                            Select Case frmExtract.ext_chassis.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("chassis") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_hull.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("hull") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_turret.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("turret") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                            Select Case frmExtract.ext_gun.Checked
                                                Case True
                                                    If ent.FileName.ToLower.Contains("gun") Then
                                                        ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                    End If
                                            End Select
                                        End If
                                End Select
                            End If 'if model
                            Select Case ent.FileName.Contains("dds") And Not frmExtract.no_textures.Checked
                                Case True
                                    Select Case frmExtract.ext_chassis.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("chassis") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                p = ent.FileName
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_hull.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("hull") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                p = ent.FileName
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_turret.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("turret") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                p = ent.FileName
                                            End If
                                    End Select
                                    Select Case frmExtract.ext_gun.Checked
                                        Case True
                                            If ent.FileName.ToLower.Contains("gun") Then
                                                ent.Extract(My.Settings.res_mods_path, ExtractExistingFileAction.DoNotOverwrite)
                                                p = ent.FileName
                                            End If
                                    End Select
                            End Select
                        End If ' crash
                    End If ' collision_client
                End If ' filename match
            Next ' next entry
        End If 'isnot nothing

    End Sub


#Region "menu_button_functions"
    Private Sub m_load_Click(sender As Object, e As EventArgs) Handles m_load.Click
        CRASH_MODE = False
        PRIMITIVES_MODE = False
        TC1.Enabled = False
        m_show_fbx.Visible = False
        m_show_fbx.Checked = False
        current_tank_name = file_name
        short_tank_name = tank_label.Text
        process_tank(False) 'false .. don't save the binary tank file
        m_ExportExtract.Enabled = True
        TC1.Enabled = True
        find_icon_image(TANK_NAME)
        m_build_wotmod.Enabled = True
        m_load_textures.Enabled = True
        m_load_textures.Checked = True
    End Sub
    Private Sub m_load_crashed_Click(sender As Object, e As EventArgs) Handles m_load_crashed.Click
        CRASH_MODE = True
        PRIMITIVES_MODE = False
        TC1.Enabled = False
        m_show_fbx.Visible = False
        m_show_fbx.Checked = False
        current_tank_name = file_name
        short_tank_name = tank_label.Text
        process_tank(False) 'false .. don't save the binary tank file
        m_ExportExtract.Enabled = True
        TC1.Enabled = True
        find_icon_image(TANK_NAME)
        m_load_textures.Enabled = True
        m_load_textures.Checked = True

    End Sub

    Private Sub m_clear_temp_folder_data_Click(sender As Object, e As EventArgs) Handles m_clear_temp_folder_data.Click
        clear_temp_folder()
    End Sub

    Private Sub m_reload_api_data_Click(sender As Object, e As EventArgs) Handles m_reload_api_data.Click
        info_Label.Visible = True
        get_tank_names()
        info_Label.Visible = False
    End Sub


    Private Sub m_create_and_extract_Click(sender As Object, e As EventArgs)
        frmExtract.ShowDialog(Me)
    End Sub


    Private Sub m_clear_selected_tanks_Click(sender As Object, e As EventArgs) Handles m_clear_selected_tanks.Click
        If MsgBox("Are you sure?" + vbCrLf + "This can NOT be undone!", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.No Then
            Return
        End If
        tanklist.Text = ""
        For Each n As TreeNode In TreeView1.Nodes
            clear_node_selection(n)
        Next
        '2
        For Each n As TreeNode In TreeView2.Nodes
            clear_node_selection(n)
        Next

        '3
        For Each n As TreeNode In TreeView3.Nodes
            clear_node_selection(n)
        Next

        '4
        For Each n As TreeNode In TreeView4.Nodes
            clear_node_selection(n)
        Next

        '5
        For Each n As TreeNode In TreeView5.Nodes
            clear_node_selection(n)
        Next

        '6
        For Each n As TreeNode In TreeView6.Nodes
            clear_node_selection(n)
        Next

        '7
        For Each n As TreeNode In TreeView7.Nodes
            clear_node_selection(n)
        Next

        '8
        For Each n As TreeNode In TreeView8.Nodes
            clear_node_selection(n)
        Next

        '9
        For Each n As TreeNode In TreeView9.Nodes
            clear_node_selection(n)
        Next

        '10
        For Each n As TreeNode In TreeView10.Nodes
            clear_node_selection(n)
        Next

    End Sub


    Public Function find_icon_image(ByVal in_s As String) As Boolean

        '1
        For Each n As TreeNode In TreeView1.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(1).img(n.Index).img
                public_icon_path = icons(1).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '2
        For Each n As TreeNode In TreeView2.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(2).img(n.Index).img
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '3
        For Each n As TreeNode In TreeView3.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(3).img(n.Index).img
                public_icon_path = icons(3).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '4
        For Each n As TreeNode In TreeView4.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(4).img(n.Index).img
                public_icon_path = icons(4).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '5
        For Each n As TreeNode In TreeView5.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(5).img(n.Index).img
                public_icon_path = icons(5).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '6
        For Each n As TreeNode In TreeView6.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(6).img(n.Index).img
                public_icon_path = icons(6).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '7
        For Each n As TreeNode In TreeView7.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(7).img(n.Index).img
                public_icon_path = icons(7).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '8
        For Each n As TreeNode In TreeView8.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(8).img(n.Index).img
                public_icon_path = icons(8).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '9
        For Each n As TreeNode In TreeView9.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(9).img(n.Index).img
                public_icon_path = icons(9).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '10
        For Each n As TreeNode In TreeView10.Nodes
            If in_s.Contains(n.Text + ":") Then
                Dim s = get_shortname(n)
                Dim ar = s.Split(":")
                tank_label.Text = ar(0)
                old_tank_name = ar(0)
                iconbox.Visible = True
                old_backgound_icon = icons(10).img(n.Index).img
                public_icon_path = icons(10).img(n.Index).img.Tag
                iconbox.BackgroundImage = old_backgound_icon
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next
        'm_export_tank_list.Visible = True
        Return False
        Application.DoEvents()
    End Function

    Private Sub m_load_file_Click(sender As Object, e As EventArgs) Handles m_load_file.Click
        out_string.Length = 0
        Dim in_s = IO.File.ReadAllText(TankListTempFolder + "tanknames.txt")
        If in_s = "" Then
            Return
        End If
        tanklist.Text = ""
        file_name = ""
        Dim tank As String = ""
        '1
        For Each n As TreeNode In TreeView1.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '2
        For Each n As TreeNode In TreeView2.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '3
        For Each n As TreeNode In TreeView3.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '4
        For Each n As TreeNode In TreeView4.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '5
        For Each n As TreeNode In TreeView5.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '6
        For Each n As TreeNode In TreeView6.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '7
        For Each n As TreeNode In TreeView7.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '8
        For Each n As TreeNode In TreeView8.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '9
        For Each n As TreeNode In TreeView9.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next

        '10
        For Each n As TreeNode In TreeView10.Nodes
            If in_s.Contains(n.Text + ":") Then
                tanklist.Text += (n.Text) + vbCrLf
                n.ForeColor = Color.White
            Else
                n.ForeColor = Color.Black
            End If
        Next
        'm_export_tank_list.Visible = True
        Application.DoEvents()
    End Sub

    Private Sub m_save_Click(sender As Object, e As EventArgs) Handles m_save.Click
        out_string.Length = 0
        If tanklist.Text = "" Then
            Return
        End If
        Dim tank As String = ""
        file_name = ""
        Dim ta = tanklist.Text.Split(vbCr)

        For i = 0 To ta.Length - 2
            tank = ta(i)
            tank = tank.Replace(vbLf, "")
            '1
            For Each n As TreeNode In TreeView1.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(1).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '2
            For Each n As TreeNode In TreeView2.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(2).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '3
            For Each n As TreeNode In TreeView3.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(3).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '4
            For Each n As TreeNode In TreeView4.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(4).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '5
            For Each n As TreeNode In TreeView5.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(5).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '6
            For Each n As TreeNode In TreeView6.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(6).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '7
            For Each n As TreeNode In TreeView7.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(7).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '8
            For Each n As TreeNode In TreeView8.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(8).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '9
            For Each n As TreeNode In TreeView9.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(9).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

            '10
            For Each n As TreeNode In TreeView10.Nodes
                If n.Text = tank Then
                    get_tank_xml_data(n)
                    out_string.Append(icons(10).img(n.Index).img.Tag.ToString + vbCrLf)
                End If
            Next

        Next

        File.WriteAllText(TankListTempFolder + "tanknames.txt", out_string.ToString)
    End Sub

    Private Sub m_open_Game_folder_Click(sender As Object, e As EventArgs) Handles m_Open_game_folder.Click
        Process.Start("explorer.exe", My.Settings.game_path)
    End Sub

    Private Sub m_export_fbx_Click(sender As Object, e As EventArgs)

        If Not loaded_from_resmods Then
            If MsgBox("You are about to write a FBX loaded from the res_mods folder!" + vbCrLf + _
                       "Doing so will corrupt the chassis if the markers have been modified." _
                       , MsgBoxStyle.YesNo, "DANGER Will Robinson!") Then
            End If
            Return
        End If
        SaveFileDialog1.Filter = "AutoDesk (*.FBX)|*.fbx"
        SaveFileDialog1.Title = "Export FBX..."
        SaveFileDialog1.InitialDirectory = My.Settings.fbx_path
        SaveFileDialog1.FileName = tank_label.Text.Replace("\/", "_") + ".fbx"

        If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            remove_loaded_fbx()
            clean_house()
            My.Settings.fbx_path = SaveFileDialog1.FileName
        Else
            Return
        End If
        ReDim textures(0)
        frmFBX.ShowDialog(Me)


    End Sub

    Private Sub m_load_textures_CheckedChanged(sender As Object, e As EventArgs) Handles m_load_textures.CheckedChanged
        If m_load_textures.Checked Then
            m_load_textures.ForeColor = Color.Red
        Else
            m_load_textures.ForeColor = Color.Black
        End If
    End Sub

    Private Sub m_show_log_Click(sender As Object, e As EventArgs) Handles m_show_log.Click
        Dim t As String = Temp_Storage + "\log_text.txt"
        File.WriteAllText(t, log_text.ToString + vbCrLf + start_up_log.ToString)

        System.Diagnostics.Process.Start("notepad.exe", t)
    End Sub

    Private Sub m_res_mods_path_Click(sender As Object, e As EventArgs) Handles m_res_mods_path.Click
        FolderBrowserDialog1.SelectedPath = My.Settings.res_mods_path
        If FolderBrowserDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.res_mods_path = FolderBrowserDialog1.SelectedPath
            'If Not File.Exists(My.Settings.res_mods_path) Then
            '    MsgBox("Incorrect Path.", MsgBoxStyle.Information)
            '    m_res_mods_path.PerformClick()
            '    Return
            'End If
            res_mods_path_set = True
            My.Settings.res_mods_path = FolderBrowserDialog1.SelectedPath
            File.WriteAllText(Temp_Storage + "\res_mods_Path.txt", My.Settings.res_mods_path)
            My.Settings.Save()
            Return
        End If
    End Sub

    Private Sub M_Path_Click(sender As Object, e As EventArgs) Handles M_Path.Click
        If FolderBrowserDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.game_path = FolderBrowserDialog1.SelectedPath
            If Not File.Exists(My.Settings.game_path + "\paths.xml") Then
                MsgBox("Incorrect Path.", MsgBoxStyle.Information)
                M_Path.PerformClick()
                Return
            End If
            path_set = True
            My.Settings.game_path = FolderBrowserDialog1.SelectedPath
            File.WriteAllText(Temp_Storage + "\game_Path.txt", My.Settings.game_path)
            My.Settings.Save()
            Return
        End If
    End Sub

    Private Sub m_pick_camo_Click(sender As Object, e As EventArgs) Handles m_pick_camo.Click
        setup_camo_selection()
    End Sub

    Private Sub m_edit_shaders_Click(sender As Object, e As EventArgs) Handles m_edit_shaders.Click
        frmShaderEditor.Show()
    End Sub

    Private Sub M_Exit_Click(sender As Object, e As EventArgs) Handles M_Exit.Click
        Me.Close()
    End Sub

    Private Sub m_lighting_Click(sender As Object, e As EventArgs) Handles m_lighting.Click
        If Not frmLighting.Visible Then
            m_lighting.ForeColor = Color.Red
            frmLighting.Show()
        Else
            m_lighting.ForeColor = Color.Black
            frmLighting.Hide()
        End If
    End Sub

    Private Sub m_help_Click(sender As Object, e As EventArgs) Handles m_help.Click
        Process.Start(Application.StartupPath + "\html\index.html")
    End Sub


    Private Sub m_Import_FBX_Click(sender As Object, e As EventArgs) Handles m_Import_FBX.Click
        frmReverseVertexWinding.Hide()
        frmComponentView.Hide()
        m_load_textures.Enabled = True
        PRIMITIVES_MODE = False
        MM.Enabled = False
        TC1.Enabled = False
        info_Label.Parent = pb1
        info_Label.Text = "Select Tank to import...."
        info_Label.Visible = True
        import_FBX()
        info_Label.Visible = False
        info_Label.Parent = Me
        MM.Enabled = True
        m_ExportExtract.Enabled = True
        TC1.Enabled = True
    End Sub

    Private Sub m_show_fbx_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_fbx.CheckedChanged
        If m_show_fbx.Checked Then
            m_show_fbx.Text = "Show Model"
        Else
            m_show_fbx.Text = "Show FBX"
        End If
        If frmTextureViewer.Visible Then
            frmTextureViewer.draw()
        End If
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_remove_fbx_Click(sender As Object, e As EventArgs) Handles m_remove_fbx.Click
        m_ExportExtract.Enabled = False
        clean_house()
        remove_loaded_fbx()
        If stop_updating Then draw_scene()

    End Sub

    Private Sub chassis_cb_CheckedChanged(sender As Object, e As EventArgs) Handles chassis_cb.CheckedChanged
        If Not _Started Then Return
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("chassis") Then
                _object(i).visible = chassis_cb.Checked
            End If
        Next
        If fbxgrp.Length > 1 Then
            For i = 1 To fbxgrp.Length - 1
                If fbxgrp(i).name.ToLower.Contains("chassis") Then
                    fbxgrp(i).visible = chassis_cb.Checked
                End If
            Next
        End If
        If stop_updating Then draw_scene()
    End Sub

    Private Sub hull_cb_CheckedChanged(sender As Object, e As EventArgs) Handles hull_cb.CheckedChanged
        If Not _Started Then Return
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("hull") Then
                _object(i).visible = hull_cb.Checked
            End If
        Next
        If fbxgrp.Length > 1 Then
            For i = 1 To fbxgrp.Length - 1
                If fbxgrp(i).name.ToLower.Contains("hull") Then
                    fbxgrp(i).visible = hull_cb.Checked
                End If
            Next
        End If
        If stop_updating Then draw_scene()
    End Sub

    Private Sub turret_cb_CheckedChanged(sender As Object, e As EventArgs) Handles turret_cb.CheckedChanged
        If Not _Started Then Return
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("turret") Then
                _object(i).visible = turret_cb.Checked
            End If
        Next
        If fbxgrp.Length > 1 Then
            For i = 1 To fbxgrp.Length - 1
                If fbxgrp(i).name.ToLower.Contains("turret") Then
                    fbxgrp(i).visible = turret_cb.Checked
                End If
            Next
        End If

        If stop_updating Then draw_scene()
    End Sub

    Private Sub gun_cb_CheckedChanged(sender As Object, e As EventArgs) Handles gun_cb.CheckedChanged
        If Not _Started Then Return
        For i = 1 To object_count
            If _object(i).name.ToLower.Contains("gun") Then
                _object(i).visible = gun_cb.Checked
            End If
        Next
        If fbxgrp.Length > 1 Then
            For i = 1 To fbxgrp.Length - 1
                If fbxgrp(i).name.ToLower.Contains("gun") Then
                    fbxgrp(i).visible = gun_cb.Checked
                End If
            Next
        End If

        If stop_updating Then draw_scene()

    End Sub

    Private Sub VertexColor_cb_CheckedChanged(sender As Object, e As EventArgs) Handles VertexColor_cb.CheckedChanged
        If VertexColor_cb.Checked Then
            VertexColor_cb.Image = My.Resources.vertex_color_off
        Else
            VertexColor_cb.Image = My.Resources.vertex_color_on
        End If
    End Sub


    Private Sub wire_cb_CheckedChanged(sender As Object, e As EventArgs) Handles wire_cb.CheckedChanged
        If wire_cb.Checked Then
            wire_cb.BackgroundImage = My.Resources.box_solid
        Else
            wire_cb.BackgroundImage = My.Resources.box_wire
        End If
        If stop_updating Then draw_scene()
    End Sub

    'Private Sub m_show_bsp2_tree_CheckedChanged(sender As Object, e As EventArgs)
    '    If m_show_bsp2_tree.Checked Then
    '        m_show_bsp2_tree.ForeColor = Color.Red
    '    Else
    '        m_show_bsp2_tree.ForeColor = Color.Black
    '    End If
    'End Sub

    Private Sub show_textures_cb_CheckedChanged(sender As Object, e As EventArgs) Handles show_textures_cb.CheckedChanged
        'make sure camo crap is not visible
        If season_Buttons_VISIBLE Then
            'CAMO_BUTTONS_VISIBLE = False
            'season_Buttons_VISIBLE = False
        End If
        '---------------------------------
        If show_textures_cb.Checked Then
            reset_tank_buttons()
            'STOP_BUTTON_SCAN = False
            TANKPARTS_VISIBLE = True
        Else
            'If Not season_Buttons_VISIBLE Then
            '    STOP_BUTTON_SCAN = True
            'End If
            TANKPARTS_VISIBLE = False
            TANK_TEXTURE_ID = 0
            TANK_TEXTURES_VISIBLE = False
            cur_texture_name = ""
        End If

        If stop_updating Then draw_scene()

    End Sub


    Private Sub m_edit_visual_Click(sender As Object, e As EventArgs) Handles m_edit_visual.Click
        frmEditVisual.Show()
    End Sub

    Private Sub m_write_primitive_Click(sender As Object, e As EventArgs) Handles m_write_primitive.Click
        If FBX_LOADED Then
            frmWritePrimitive.Show()

            If is_wheeled_vehicle Then
                frmWritePrimitive.cew_cb.Checked = False
                frmWritePrimitive.cew_cb.Enabled = False
            End If
            frmWritePrimitive.Hide()
            frmWritePrimitive.ShowDialog(Me)
        End If
    End Sub

    Private Sub m_show_model_info_Click(sender As Object, e As EventArgs) Handles m_show_model_info.Click
        frmModelInfo.Show()
    End Sub

    Private Sub m_region_Click(sender As Object, e As EventArgs) Handles m_region.Click
        ToolStripComboBox1.Visible = True
    End Sub

    Private Sub ToolStripComboBox1_TextChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.TextChanged
        If Not _Started Then Return ' dont want to cause a trigger here!
        API_REGION = ToolStripComboBox1.Text
        My.Settings.region_selection = API_REGION
        ToolStripComboBox1.Visible = False
        MsgBox("You will need to clear the temp folder (under menu)" + vbCrLf + _
                "and restart Tank Exporter." + vbCrLf + _
                "This had to be done to reload data for your region!", MsgBoxStyle.Exclamation, "Warning!")

        My.Settings.Save()

    End Sub


    Private Sub m_Shader_Debug_Click(sender As Object, e As EventArgs) Handles m_Shader_Debug.Click
        frmShaderDebugSettings.Show()

    End Sub

    Private Sub frmMain_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_show_environment_CheckedChanged(sender As Object, e As EventArgs) Handles m_show_environment.CheckedChanged
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_shadow_preview_Click(sender As Object, e As EventArgs) Handles m_shadow_preview.Click
        If stop_updating Then draw_scene()

    End Sub

    Private Sub m_terrain_Click(sender As Object, e As EventArgs)
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_shadows_Click(sender As Object, e As EventArgs) Handles m_shadows.Click
        If stop_updating Then draw_scene()
    End Sub

    Private Sub m_shadowQuality_Click(sender As Object, e As EventArgs) Handles m_shadowQuality.Click
        FrmShadowSettings.Show()
    End Sub

    Private Sub m_select_light_Click(sender As Object, e As EventArgs) Handles m_select_light.Click
        frmLightSelection.Show()
    End Sub

    Private Sub m_decal_Click(sender As Object, e As EventArgs) Handles m_decal.Click
        If m_decal.Checked Then
            m_decal.ForeColor = Color.Red
            upton.position = New Point(pb1.Width - 150, -210)
            upton.state = 0
            decal_panel.Dock = DockStyle.Fill
            decal_panel.Visible = True
            decal_panel.BringToFront()
            TC1.Visible = False
            'make_test_decal(0)
        Else
            m_decal.ForeColor = Color.Black
            decal_panel.Visible = False
            TC1.Visible = True
        End If
        gl_busy = False
    End Sub

    Private Sub m_new_Click(sender As Object, e As EventArgs) Handles m_new.Click
        add_decal()
    End Sub

    Private Sub m_settings_Click(sender As Object, e As EventArgs) Handles m_settings.Click
        frmSettings.Visible = True
    End Sub

    Private Sub m_sel_texture_Click(sender As Object, e As EventArgs) Handles m_sel_texture.Click
        If current_decal < 0 Then Return
        If t_list Is Nothing Then ' create text box and fill it with all the texture names if it hasn't been created already.
            t_list = New TextBox
            t_list.Multiline = True
            t_list.Parent = decal_panel
            t_list.Width = d_list_tb.Width
            t_list.Height = d_list_tb.Height
            t_list.Location = d_list_tb.Location
            t_list.Anchor = AnchorStyles.Bottom Or AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
            t_list.ForeColor = d_list_tb.ForeColor
            t_list.BackColor = d_list_tb.BackColor
            t_list.Font = d_list_tb.Font
            t_list.ScrollBars = ScrollBars.Vertical
            For j = 0 To decal_textures.Length - 1
                t_list.Text += decal_textures(j).colorMap_name + " :" + j.ToString + vbCrLf
            Next
            AddHandler t_list.Click, AddressOf handle_t_click
        End If
        t_list.BringToFront()
        d_list_tb.SendToBack()
        current_decal_lable.Text = current_decal.ToString
    End Sub

    Private Sub m_delete_Click(sender As Object, e As EventArgs) Handles m_delete.Click
        If decal_matrix_list.Length < 2 Then Return
        Dim t_l(decal_matrix_list.Length - 2) As decal_matrix_list_
        Dim ta = d_list_tb.Text.Split(vbLf)
        Dim ts As String = ""
        For Each s In ta
            Dim ti = s.Split(":")
            If ti.Length > 1 Then ' dont mess with empty lines
                Dim tii = ti(1).Split(vbCr)
                Dim rv = CInt(tii(0))
                If rv <> current_decal Then
                    If rv > current_decal Then ' If this ones larger it needs decremented.
                        rv -= 1
                    End If
                    ts += ti(0) + ":" + rv.ToString + vbCrLf 're-assemble the line.
                End If
            End If
        Next
        d_list_tb.Text = ts
        Application.DoEvents()
        Dim cnt As Integer = 0
        For i = 0 To decal_matrix_list.Length - 2
            If i <> current_decal Then
                t_l(cnt) = decal_matrix_list(i)
                cnt += 1
            End If
        Next
        ReDim Preserve decal_matrix_list(decal_matrix_list.Length - 2)
        For i = 0 To decal_matrix_list.Length - 2
            decal_matrix_list(i) = t_l(i)
            'd_list_tb.Text += "Decal ID :" + i.ToString + vbCrLf
        Next
        'current_decal -= 1
        'If current_decal = -1 And decal_matrix_list.Length > 1 Then
        '    current_decal = 0
        'End If
        If current_decal > -1 Then
            update_decal_order()
            Dim new_line As Integer = 0
            For i = 0 To decal_order.Length - 1
                If decal_order(i) = current_decal Then
                    new_line = i
                    Exit For
                End If
            Next
            Dim sp = d_list_tb.GetFirstCharIndexFromLine(new_line) ' get line
            d_list_tb.SelectionStart = sp
            Try
                d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                                     d_list_tb.Lines(new_line).Length) ' select prev line
                decal_matrix_list(new_line).get_decals_transform_info()

            Catch ex As Exception
                Return
            End Try
            d_current_line = new_line
            d_sel_Len = d_list_tb.Lines(new_line).Length
        End If
    End Sub

    Private Sub m_reload_textures_Click(sender As Object, e As EventArgs) Handles m_reload_textures.Click
        If Not MODEL_LOADED Then
            Return
        End If
        MODEL_LOADED = False ' stop drawing the tank
        ' delete texture so we dont waste video memory!
        For i = 0 To textures.Length - 1
            Gl.glDeleteTextures(1, textures(i).c_id)
            Gl.glDeleteTextures(1, textures(i).n_id)
            If textures(i).ao_id > -1 Then
                Gl.glDeleteTextures(1, textures(i).ao_id)
            End If
            Gl.glDeleteTextures(1, textures(i).gmm_id)
            If textures(i).detail_id > -1 Then
                Gl.glDeleteTextures(1, textures(i).detail_id)
            End If
        Next
        ReDim textures(0) ' resize so it can be rebuild
        For i = 1 To _group.Length - 1
            build_textures(i) 'get the textures for this model. If its in the cache, use them.
        Next
        MODEL_LOADED = True ' enable drawing the tank
        log_text.AppendLine("---- Reloading Textures -----")
    End Sub

    Private Sub m_extract_Click(sender As Object, e As EventArgs) Handles m_extract.Click
        file_name = current_tank_name
        frmExtract.ShowDialog(Me)

    End Sub

    Private Sub m_export_to_fbx_Click(sender As Object, e As EventArgs) Handles m_export_to_fbx.Click
        If loaded_from_resmods Then
            If MsgBox("You are about to write a FBX loaded from the res_mods folder!" + vbCrLf + _
                       "Doing so will corrupt the chassis if the markers have been modified." _
                       , MsgBoxStyle.YesNo, "DANGER Will Robinson!") = MsgBoxResult.Yes Then
            Else
                Return
            End If
        End If
        SaveFileDialog1.Filter = "AutoDesk (*.FBX)|*.fbx"
        SaveFileDialog1.Title = "Export FBX..."
        Dim tfp As String = "C:\"
        If File.Exists(Temp_Storage + "\Fbx_out_folder.txt") Then
            tfp = File.ReadAllText(Temp_Storage + "\Fbx_out_folder.txt")
        End If

        SaveFileDialog1.InitialDirectory = tfp
        If CRASH_MODE Then
            SaveFileDialog1.FileName = short_tank_name.Replace("\/", "_") + "_CRASHED.fbx"
        Else
            SaveFileDialog1.FileName = short_tank_name.Replace("\/", "_") + ".fbx"
        End If
        If PRIMITIVES_MODE Then
            SaveFileDialog1.FileName = Path.GetFileNameWithoutExtension(OpenFileDialog1.FileName)
            FBX_NAME = SaveFileDialog1.FileName
        End If
        info_Label.Parent = pb1

        If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.fbx_path = SaveFileDialog1.FileName
        Else
            info_Label.Visible = False
            info_Label.Parent = Me
            Return
        End If
        File.WriteAllText(Temp_Storage + "\Fbx_out_folder.txt", Path.GetDirectoryName(SaveFileDialog1.FileName))

        frmFBX.ShowDialog(Me)
        info_Label.Visible = False
        info_Label.Parent = Me

    End Sub

    Private Sub m_edit_camo_Click(sender As Object, e As EventArgs) Handles m_edit_camo.Click
        frmEditCamo.Visible = True
    End Sub

    Private Sub m_clean_res_mods_Click(sender As Object, e As EventArgs) Handles m_clean_res_mods.Click
        Dim path = TANK_NAME
        If path.Contains(":") Then
            Dim z = path.Split(":")
            path = z(0) + "/normal/"
        End If
        Dim a = path.Split("normal")
        path = a(0)
        path = My.Settings.res_mods_path + "/" + path
        If MsgBox("If you have IMPORTANT FILES you need to save" + vbCrLf + _
                   "you need to do it before continuing!!" + vbCrLf + _
                   "Delete Files now?", MsgBoxStyle.YesNo, "WARNING!!") = MsgBoxResult.Yes Then
            If Directory.Exists(path) Then
                DeleteFilesFromFolder(path)
            End If
        End If
    End Sub
    Private Sub DeleteFilesFromFolder(path As String)
        Try
            System.IO.Directory.Delete(path, True)

        Catch ex As Exception
            MsgBox("I cant delete " + path + vbCrLf + _
                    "Someting is accessing the folders or files!", MsgBoxStyle.Exclamation, "Access Denied!")
        End Try
    End Sub

    Private Sub m_donate_Click(sender As Object, e As EventArgs) Handles m_donate.Click
        Process.Start(Application.StartupPath + "\html\donate.html")

    End Sub

    Private Sub m_view_res_mods_folder_Click(sender As Object, e As EventArgs) Handles m_view_res_mods_folder.Click
        If PRIMITIVES_MODE Then
            Dim fp = OpenFileDialog1.FileName
            fp = Path.GetDirectoryName(fp)
            If Directory.Exists(fp) Then
                Process.Start(fp)
                Return
            End If
        End If
        Dim a = TANK_NAME.Split(":")
        Dim p = a(0)
        Dim f = My.Settings.res_mods_path + "\" + p
        If Directory.Exists(f) Then
            Process.Start(f)
        Else
            MsgBox("You have not extracted anything to res_mods for this tank", MsgBoxStyle.Exclamation, "Path not found!")
        End If
    End Sub

    Private Sub m_bloom_off_CheckedChanged(sender As Object, e As EventArgs) Handles m_bloom_off.CheckedChanged
    End Sub

    Private Sub m_FXAA_CheckedChanged(sender As Object, e As EventArgs) Handles m_FXAA.CheckedChanged

    End Sub

    Private Sub m_screen_cap_Click(sender As Object, e As EventArgs) Handles m_screen_cap.Click
        stop_updating = True
        frmScreenCap.ShowDialog(Me)
        stop_updating = False
    End Sub

    Private Sub m_ExportExtract_EnabledChanged(sender As Object, e As EventArgs) Handles m_ExportExtract.EnabledChanged
        m_GMM_toy_cb.Visible = m_ExportExtract.Enabled
    End Sub

    Private Sub m_GMM_toy_cb_CheckedChanged(sender As Object, e As EventArgs) Handles m_GMM_toy_cb.CheckedChanged
        frmGMM.Visible = m_GMM_toy_cb.Checked
        If frmGMM.Visible Then
            m_GMM_toy_cb.ForeColor = Color.Red
            GMM_TOY_VISIBLE = 1
        Else
            GMM_TOY_VISIBLE = 0
            m_GMM_toy_cb.ForeColor = Color.Black
        End If
    End Sub

    Private Sub m_GMM_toy_cb_Click(sender As Object, e As EventArgs) Handles m_GMM_toy_cb.Click

    End Sub

    Private Sub m_hide_show_components_Click(sender As Object, e As EventArgs) Handles m_hide_show_components.Click
        frmComponentView.Show()
    End Sub

    Private Sub m_set_vertex_winding_order_Click(sender As Object, e As EventArgs) Handles m_set_vertex_winding_order.Click
        frmReverseVertexWinding.Show()
    End Sub

    Private Sub m_load_primitive_Click(sender As Object, e As EventArgs) Handles m_load_primitive.Click
        Dim tp As String = My.Settings.res_mods_path
        If File.Exists(Temp_Storage + "\primitive_file_load_path.txt") Then
            tp = File.ReadAllText(Temp_Storage + "\primitive_file_load_path.txt")
        End If

        OpenFileDialog1.Title = "Load primitives+processed File..."
        OpenFileDialog1.Filter = "Primitives File|*.primitives_processed|All..|*.*"
        OpenFileDialog1.InitialDirectory = tp
        'OpenFileDialog1.FileName = My.Settings.primitive_file_path
        If Not OpenFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            Return
        End If
        File.WriteAllText(Temp_Storage + "\primitive_file_load_path.txt", Path.GetDirectoryName(OpenFileDialog1.FileName))

        file_name = OpenFileDialog1.FileName.Replace("primitives_processed", "model")
        file_name = file_name.Replace("/", "\")
        file_name = file_name.Replace(My.Settings.res_mods_path + "\", "")
        remove_loaded_fbx()
        clean_house()
        PRIMITIVES_MODE = True
        frmReverseVertexWinding.Panel1.Controls.Clear()
        frmComponentView.splitter.Panel1.Controls.Clear()
        frmComponentView.splitter.Panel2.Controls.Clear()

        m_load_textures.Checked = True
        ReDim AM_index_texture_list(0)
        ReDim GBMT_index_texture_list(0)
        ReDim MAO_index_texture_list(0)

        If Not build_primitive_data(False) Then
            MsgBox("Primitive failed to open!", MsgBoxStyle.Exclamation, "Load Error!")
            PRIMITIVES_MODE = False
            m_load_textures.Checked = True
            Return
        End If

        TANK_NAME = Path.GetFileName(file_name)
        Me.Text = TANK_NAME
        m_hide_show_components.Enabled = True
        m_set_vertex_winding_order.Enabled = True
        m_ExportExtract.Enabled = True
        loaded_from_resmods = False
    End Sub

    Private Sub m_import_primitives_fbx_Click(sender As Object, e As EventArgs) Handles m_import_primitives_fbx.Click
        PRIMITIVES_MODE = False
        MM.Enabled = False
        TC1.Enabled = False
        info_Label.Parent = pb1
        info_Label.Text = "Select Tank to import...."
        info_Label.Visible = True
        import_primitives_FBX()
        info_Label.Visible = False
        info_Label.Parent = Me
        MM.Enabled = True
        m_ExportExtract.Enabled = True
        TC1.Enabled = True
        If PRIMITIVES_MODE Then
            m_load_textures.Checked = False
            m_load_textures.Enabled = False
        Else
            m_load_textures.Enabled = True
        End If

    End Sub

    Private Sub m_write_non_tank_primitive_Click(sender As Object, e As EventArgs) Handles m_write_non_tank_primitive.Click
        frmWrite_Primitives.ShowDialog(Me)
    End Sub

#End Region

    Public d_sel_Len As Integer
    Public d_current_line As Integer
    Dim d_line_count As Integer
    Private Sub d_list_tb_Click(sender As Object, e As EventArgs) Handles d_list_tb.Click
        d_list_tb.SelectionLength = 0
        Dim t = d_list_tb.Text.Split(":")
        d_line_count = t.Length - 2
        If d_line_count = -1 Then Return
        Dim a = d_list_tb.GetLineFromCharIndex(d_list_tb.GetFirstCharIndexOfCurrentLine())
        d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), d_list_tb.Lines(a).Length)
        If d_list_tb.SelectedText.Length < 4 Then
            Return
            d_sel_Len = 0
            d_current_line = 0
        Else
            Dim d = d_list_tb.SelectedText.Split(":")
            current_decal = CInt(d(1))
            decal_matrix_list(current_decal).get_decals_transform_info()
            d_sel_Len = d_list_tb.Lines(a).Length
            d_current_line = a
            decal_matrix_list(current_decal).get_decals_transform_info()
        End If

    End Sub

    Private Sub d_move_up_Click(sender As Object, e As EventArgs) Handles d_move_up.Click
        If d_current_line = 0 Then
            Return
        End If
        If d_sel_Len > 0 Then
            Dim prev_line = d_current_line - 1
            Dim sel_tex_current = d_list_tb.SelectedText ' save current text
            If sel_tex_current = "" Then Return
            Dim sp = d_list_tb.GetFirstCharIndexFromLine(prev_line) ' get prev line
            Application.DoEvents()
            d_list_tb.SelectionStart = sp
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(prev_line).Length) ' select prev line
            Application.DoEvents()
            Dim pre_text = d_list_tb.SelectedText ' save prev line text
            If pre_text = "" Then Return
            Application.DoEvents()
            d_list_tb.SelectedText = sel_tex_current ' replace current line text
            Application.DoEvents()

            d_list_tb.SelectionStart = sp + sel_tex_current.Length + 2
            d_current_line = d_list_tb.GetLineFromCharIndex(d_list_tb.GetFirstCharIndexOfCurrentLine())

            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(d_current_line).Length) ' reselect current line
            Application.DoEvents()
            d_list_tb.SelectedText = pre_text ' replace it with prev lines text
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                     d_list_tb.Lines(d_current_line).Length) ' reselect current line
            Application.DoEvents()
            update_decal_order()
            d_list_tb.SelectionStart = sp
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                       d_list_tb.Lines(prev_line).Length) ' select prev line
            d_current_line = prev_line
            decal_matrix_list(current_decal).get_decals_transform_info()
        End If
    End Sub

    Private Sub d_move_down_Click(sender As Object, e As EventArgs) Handles d_move_down.Click
        If d_current_line = d_line_count Then
            Return
        End If
        If d_sel_Len > 0 Then
            Dim next_line = d_current_line + 1
            Dim sel_tex_current = d_list_tb.SelectedText ' save current text
            If sel_tex_current = "" Then Return
            Dim sp = d_list_tb.GetFirstCharIndexFromLine(next_line) ' get prev line
            Application.DoEvents()
            d_list_tb.SelectionStart = sp
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(next_line).Length) ' select prev line
            Application.DoEvents()
            Dim next_text = d_list_tb.SelectedText ' save prev line text
            If next_text = "" Then Return
            Application.DoEvents()
            d_list_tb.SelectedText = sel_tex_current ' replace current line text
            Application.DoEvents()

            d_list_tb.SelectionStart = sp - 2
            d_current_line = d_list_tb.GetLineFromCharIndex(d_list_tb.GetFirstCharIndexOfCurrentLine())

            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(d_current_line).Length) ' reselect current line
            Application.DoEvents()
            d_list_tb.SelectedText = next_text ' replace it with prev lines text
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                     d_list_tb.Lines(d_current_line).Length) ' reselect current line
            Application.DoEvents()
            update_decal_order()
            d_list_tb.SelectionStart = sp
            d_list_tb.Select(d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                             d_list_tb.Lines(next_line).Length) ' select prev line
            d_current_line = next_line
            decal_matrix_list(current_decal).get_decals_transform_info()
        End If
    End Sub

    Private Sub d_list_tb_KeyPress(sender As Object, e As KeyPressEventArgs) Handles d_list_tb.KeyPress
        e.Handled = True
    End Sub

    Private Sub decal_alpha_slider_Scroll(sender As Object, e As EventArgs) Handles decal_alpha_slider.Scroll
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).alpha = CSng(decal_alpha_slider.Value / 100)
    End Sub
    Private Sub decal_level_slider_Scroll(sender As Object, e As EventArgs) Handles decal_level_slider.Scroll
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).level = CSng(decal_level_slider.Value / 100)

    End Sub
    Dim t_list As TextBox


    Private Sub handle_t_click(sender As TextBox, e As EventArgs)
        If current_decal < 0 Then Return
        t_list.SelectionLength = 0
        Dim t = t_list.Text.Split(":")
        d_line_count = t.Length - 2
        Dim a = t_list.GetLineFromCharIndex(t_list.GetFirstCharIndexOfCurrentLine())
        t_list.Select(t_list.GetFirstCharIndexOfCurrentLine(), t_list.Lines(a).Length)
        If t_list.SelectedText.Length < 4 Then
            Return
            d_sel_Len = 0
            d_current_line = 0
        Else
            Dim d = t_list.SelectedText.Split(":")
            Dim id = CInt(d(1))
            load_this_Decal(id)
            decal_matrix_list(current_decal).decal_texture = decal_textures(id).colorMap_name
            decal_matrix_list(current_decal).texture_id = decal_textures(id).colorMap_Id
            decal_matrix_list(current_decal).normal_id = decal_textures(id).normalMap_Id
            decal_matrix_list(current_decal).gmm_id = decal_textures(id).gmmMap_id
            d_texture_name.Text = decal_matrix_list(current_decal).decal_texture
            t_list.SendToBack()
            d_list_tb.BringToFront()
            d_move_up.BringToFront()
            d_move_down.BringToFront()
        End If
    End Sub

    Private Sub Uwrap_SelectedItemChanged(sender As Object, e As EventArgs) Handles Uwrap.SelectedItemChanged
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).u_wrap = CSng(Uwrap.SelectedItem)
        decal_matrix_list(current_decal).u_wrap_index = Uwrap.SelectedIndex
    End Sub

    Private Sub Vwrap_SelectedItemChanged(sender As Object, e As EventArgs) Handles Vwrap.SelectedItemChanged
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).v_wrap = CSng(Vwrap.SelectedItem)
        decal_matrix_list(current_decal).v_wrap_index = Vwrap.SelectedIndex

    End Sub

    Private Sub save_decal_btn_Click(sender As Object, e As EventArgs) Handles save_decal_btn.Click
        If current_decal = -1 Then Return
        If MsgBox("Are you sure?", MsgBoxStyle.YesNo, "For real?") = MsgBoxResult.Yes Then
            save_decal_data()
        End If
    End Sub

    Private Sub load_decal_btn_Click(sender As Object, e As EventArgs) Handles load_decal_btn.Click
        If MsgBox("Are you sure?", MsgBoxStyle.YesNo, "For real?") = MsgBoxResult.Yes Then
            load_decal_data()
        End If
    End Sub

    Private Sub uv_rotate_direction(sender As Object, e As EventArgs) Handles uv_rotate.SelectedItemChanged
        If current_decal = -1 Then Return
        decal_matrix_list(current_decal).uv_rot = CSng(uv_rotate.SelectedItem) * 0.017453293
        decal_matrix_list(current_decal).uv_rot_index = uv_rotate.SelectedIndex

    End Sub

    Private Sub copy_Decal_btn_Click(sender As Object, e As EventArgs) Handles copy_Decal_btn.Click
        If decal_order.Length < 1 Then
            Return
        End If
        copy_decal()
    End Sub



    Private Sub grid_cb_Click(sender As Object, e As EventArgs) Handles grid_cb.Click
        If grid_cb.Checked Then
            grid_cb.BackgroundImage = My.Resources.grid_blank
        Else
            grid_cb.BackgroundImage = My.Resources.grid_dark
        End If
        If grid_cb.CheckState = CheckState.Indeterminate Then
            grid_cb.BackgroundImage = My.Resources.grid
        End If
        If stop_updating Then draw_scene()

    End Sub

    Private Sub grid_cb_CheckStateChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub frmMain_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If Not _Started Then

        End If
    End Sub

    Private Sub pb1_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles pb1.PreviewKeyDown
        'Debug.WriteLine(e.KeyCode.ToString)
        Select Case e.KeyCode
            Case Keys.Left, Keys.Right
                e.IsInputKey = True
        End Select
    End Sub


#Region "wotmod"
    Dim searched_files As Integer = 0
    Dim segment_visual_exist As Boolean
    Dim segment_1_visual_exist As Boolean
    Dim segment_2_visual_exist As Boolean
    Dim tank_script_xml_exist As Boolean

    Dim chassis_visual_exist As Boolean
    Dim hull_visual_exist As Boolean
    Dim turret_visual_exist As Boolean
    Dim gun_visual_exist As Boolean

    Dim chassis_crash_visual_exist As Boolean
    Dim hull_crash_visual_exist As Boolean
    Dim turret_crash_visual_exist As Boolean
    Dim gun_crash_visual_exist As Boolean

    Dim track_visual_exist As Boolean

    Dim track_names(1) As String
    Dim chassis_names(1) As String
    Dim hull_names(1) As String
    Dim turret_names(1) As String
    Dim gun_names(1) As String

    Private Function find_tank_component(ByVal p As String, ByVal c1 As String, ByVal c2 As String) As Boolean
        'p = path
        'c1,c2 = names we are looking for

        'deal with empty strings
        If c1 Is Nothing Then
            c1 = "!"
        End If
        If c2 Is Nothing Then
            c2 = "!"
        End If
        'If c1.ToLower.Contains("_track_") Then
        '    p += "track"
        'End If
        If Directory.Exists(p) Then ' make sure the directory exist before searching for a file
            Dim d As New DirectoryInfo(p)
            Dim files = d.GetFiles("*.*")
            For Each item In files
                If item.Name.ToLower.Contains(c1.ToLower) And item.Name.ToLower.Contains(c2.ToLower) Then
                    Return True 'found
                End If
            Next
        End If
        Return False 'not found
    End Function
    Private Function fix_stupid_wargaming_path(ByRef s As String) As String
        Dim v_name() = {"american", "british", "chinese", "german", "russian"}
        Dim p_name() = {"usa", "uk", "china", "germany", "ussr"}
        For i = 0 To 4
            If s.ToLower.Contains(v_name(i)) Then
                s = s.Replace(v_name(i), p_name(i))
                Exit For
            End If
        Next
        Return s
    End Function
    Private Function validate_tank_data(ByVal p As String, ByVal author As String, ByVal tank As String) As Boolean

        Dim lod0_path = p + "normal\lod0\"
        Dim track_path = p + "track\"
        Dim crash_path = p + "crash\lod0\"
        Dim n_array = p.Split("\")
        Dim script_path = My.Settings.res_mods_path + "\res\scripts\item_defs\"
        script_path = fix_stupid_wargaming_path(script_path + Path.GetDirectoryName(tank))
        Dim script_name = fix_stupid_wargaming_path(script_path + "\" + Path.GetFileName(tank) + ".xml")

        segment_visual_exist = find_tank_component(track_path, "segment.", ".visual_processed")
        segment_1_visual_exist = find_tank_component(track_path, "segment_1", ".visual_processed")
        segment_2_visual_exist = find_tank_component(track_path, "segment_2", ".visual_processed")
        tank_script_xml_exist = File.Exists(script_name)

        chassis_crash_visual_exist = find_tank_component(crash_path, "chassis", ".visual_processed")
        hull_crash_visual_exist = find_tank_component(crash_path, "hull", ".visual_processed")
        turret_crash_visual_exist = find_tank_component(crash_path, "turret", ".visual_processed")
        gun_crash_visual_exist = find_tank_component(crash_path, "gun", ".visual_processed")

        chassis_visual_exist = find_tank_component(lod0_path, "chassis", ".visual_processed")
        hull_visual_exist = find_tank_component(lod0_path, "hull", ".visual_processed")
        turret_visual_exist = find_tank_component(lod0_path, "turret", ".visual_processed")
        gun_visual_exist = find_tank_component(lod0_path, "gun", ".visual_processed")

        If Not tank_script_xml_exist Then
            If MsgBox("You MUST extract the " + Path.GetFileNameWithoutExtension(tank) + ".xml" + vbCrLf + _
                   "from the scripts\item_defs\vehicle XMLs!" + vbCrLf + _
                   "Do it now?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                m_extract.PerformClick()
                Return False
            Else
                Return False
            End If
        End If

        '------------------------------------------------------------------------------
        Dim tank_xml = get_bw_xml(script_name) 'At this point we know the tanks XML is there.
        tank_xml = TheXML_String.Replace(vbCr, "") 'Copy the xml and remove any Carriage returns
        Dim xml_array = tank_xml.Split(vbLf)
        'Change the xml file depending on what parts are in the temp res folder.
        Dim tank_xml_update As Boolean = False
        For idx = 0 To xml_array.Length - 1
            If xml_array(idx).ToLower.Contains("track/segment") Then
                If segment_visual_exist Then
                    If xml_array(idx).ToLower.Contains("segment.model") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                Else
                    If segment_1_visual_exist Then
                        If xml_array(idx).ToLower.Contains("segment_1.model") Then
                            xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                            tank_xml_update = True
                        End If

                    End If
                    If segment_2_visual_exist Then
                        If xml_array(idx).ToLower.Contains("segment_2.model") Then
                            xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                            tank_xml_update = True
                        End If

                    End If
                End If

            End If
            'normal tank parts
            If xml_array(idx).ToLower.Contains("<undamaged>") Then

                If chassis_visual_exist Then
                    If xml_array(idx).ToLower.Contains("chassis") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                End If
                If hull_visual_exist Then
                    If xml_array(idx).ToLower.Contains("hull") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                End If
                If turret_visual_exist Then
                    If xml_array(idx).ToLower.Contains("turret") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                End If
                If gun_visual_exist Then
                    If xml_array(idx).ToLower.Contains("gun") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                End If

            End If
            'crash tank parts
            If xml_array(idx).ToLower.Contains("<destroyed>") Then

                If chassis_crash_visual_exist Then
                    If xml_array(idx).ToLower.Contains("chassis") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                End If
                If hull_crash_visual_exist Then
                    If xml_array(idx).ToLower.Contains("hull") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                End If
                If turret_crash_visual_exist Then
                    If xml_array(idx).ToLower.Contains("turret") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                End If
                If gun_crash_visual_exist Then
                    If xml_array(idx).ToLower.Contains("gun") Then
                        xml_array(idx) = xml_array(idx).Replace(Path.GetFileName(tank), author + "/remodels/" + Path.GetFileName(tank))
                        tank_xml_update = True
                    End If
                End If

            End If
        Next
        If tank_xml_update Then 'only rewrite the xml if something was changed
            tank_xml = ""
            For idx = 0 To xml_array.Length - 2
                tank_xml += xml_array(idx) + vbCrLf
            Next
            tank_xml += xml_array(xml_array.Length - 1)
            File.WriteAllText(script_name, tank_xml)
        End If

        '------------------------------------------------------------------------------
        'deal with track folder
        If segment_visual_exist Then
            fix_paths_track_model("segment.", track_path, author, Path.GetFileName(tank))
            fix_crash_paths_visual("segment.", track_path, author, Path.GetFileName(tank), track_names)

        Else
            If segment_1_visual_exist Then
                fix_paths_track_model("segment_1.", track_path, author, Path.GetFileName(tank))
                fix_crash_paths_visual("segment_1.", track_path, author, Path.GetFileName(tank), track_names)

            End If
            If segment_2_visual_exist Then
                fix_paths_track_model("segment_2.", track_path, author, Path.GetFileName(tank))
                fix_crash_paths_visual("segment_2.", track_path, author, Path.GetFileName(tank), track_names)

            End If

        End If
        '------------------------------------------------------------------------------
        'Find out what textures are in res_mods
        For i = 1 To object_count
            'Return False
            _group(i).AM_in_res_mods = find_tank_component(p, Path.GetFileName(_group(i).color_name), ".dds")
            _group(i).AO_in_res_mods = find_tank_component(p, Path.GetFileName(_group(i).ao_name), ".dds")
            _group(i).GMM_in_res_mods = find_tank_component(p, Path.GetFileName(_group(i).metalGMM_name), ".dds")
            _group(i).ANM_in_res_mods = find_tank_component(p, Path.GetFileName(_group(i).normal_name), ".dds")
            _group(i).Spec_in_res_mods = find_tank_component(p, Path.GetFileName(_group(i).specular_name), ".dds")
        Next

        '*** This will NEVER change a path for a track texture.. The do NOT exist normally with the tanks data!
        'normal tank
        If chassis_visual_exist Then
            fix_paths_visual("chassis", lod0_path, author, Path.GetFileName(tank))
            fix_paths_model("chassis", lod0_path, author, Path.GetFileName(tank))
        End If

        If hull_visual_exist Then
            fix_paths_visual("hull", lod0_path, author, Path.GetFileName(tank))
            fix_paths_model("hull", lod0_path, author, Path.GetFileName(tank))
        End If

        If turret_visual_exist Then
            fix_paths_visual("turret", lod0_path, author, Path.GetFileName(tank))
            fix_paths_model("turret", lod0_path, author, Path.GetFileName(tank))
        End If

        If gun_visual_exist Then
            fix_paths_visual("gun", lod0_path, author, Path.GetFileName(tank))
            fix_paths_model("gun", lod0_path, author, Path.GetFileName(tank))
        End If

        'crash tank
        If chassis_crash_visual_exist Then
            fix_crash_paths_visual("chassis", crash_path, author, Path.GetFileName(tank), chassis_names)
            fix_paths_model("chassis", crash_path, author, Path.GetFileName(tank))
        End If

        If hull_crash_visual_exist Then
            fix_crash_paths_visual("hull", crash_path, author, Path.GetFileName(tank), hull_names)
            fix_paths_model("hull", crash_path, author, Path.GetFileName(tank))
        End If

        If turret_crash_visual_exist Then
            fix_crash_paths_visual("turret", crash_path, author, Path.GetFileName(tank), turret_names)
            fix_paths_model("turret", crash_path, author, Path.GetFileName(tank))
        End If

        If gun_crash_visual_exist Then
            fix_crash_paths_visual("gun", crash_path, author, Path.GetFileName(tank), gun_names)
            fix_paths_model("gun", crash_path, author, Path.GetFileName(tank))
        End If

        Return True
    End Function
    Private Sub fix_paths_model(ByVal item As String, ByVal p As String, ByVal author As String, ByVal tn As String)
        Dim di As New DirectoryInfo(p)
        Dim files = di.GetFiles("*.model")
        For Each f In files
            If f.FullName.ToLower.Contains(item) Then
                Dim fn = f.FullName
                Dim ts = get_bw_xml(fn)
                Dim idx As Integer
                'find the part of the tank we are looking for in _groups and get and index to it.
                'this will loop more than once on left and right chassis and track parts :(
                Dim save_it As Boolean = False
                ts = ts.Replace(vbCr, "")
                Dim ta = ts.Split(vbLf)
                For idx = 1 To object_count
                    If _group(idx).tank_part.ToLower.Contains(item) Then
                        For i = 0 To ta.Length - 1
                            If ta(i).Contains("<nodefullVisual>vehicles") Or ta(i).Contains("<nodelessVisual>vehicles") Then
                                If Not ta(i).ToLower.Contains(author.ToLower) Then
                                    ta(i) = ta(i).Replace(tn, author + "/remodels/" + tn)
                                    save_it = True
                                End If
                            End If
                            If ta(i).Contains("<parent>vehicles") Then
                                ta(i) = ""
                            End If
                            If ta(i).Contains("<extent>") Then
                                ta(i) = ""
                            End If
                        Next
                    End If
                Next
                If save_it Then
                    ts = ""
                    For i = 0 To ta.Length - 2
                        If ta(i).Length > 3 Then
                            ts += ta(i) + vbCrLf
                        End If
                    Next
                    ts += ta(ta.Length - 1)
                    If ts.Contains("map_>") Then
                        ts = ts.Replace("map_", Path.GetFileName(tn) + ".model")
                    Else
                        ts = ts.Replace("map_", "")
                    End If
                    File.WriteAllText(fn, ts)
                End If
                '------------------------------------------------------------------------------
                Return

            End If
        Next

    End Sub
    Private Sub fix_paths_track_model(ByVal item As String, ByVal p As String, ByVal author As String, ByVal tn As String)
        Dim di As New DirectoryInfo(p)
        Dim files = di.GetFiles("*.model")
        For Each f In files
            If f.FullName.ToLower.Contains(item) Then
                Dim fn = f.FullName
                Dim ts = get_bw_xml(fn)
                Dim idx As Integer
                item = item.Replace(".", "")
                'find the part of the tank we are looking for in _groups and get and index to it.
                'this will loop more than once on left and right chassis and track parts :(
                Dim save_it As Boolean = False
                ts = ts.Replace(vbCr, "")
                Dim ta = ts.Split(vbLf)
                For idx = 1 To object_count
                    For i = 0 To ta.Length - 1
                        If ta(i).Contains("<nodelessVisual>vehicles") Then
                            If Not ta(i).ToLower.Contains(author.ToLower) Then
                                ta(i) = ta(i).Replace(tn, author + "/remodels/" + tn)
                                save_it = True
                            End If
                        End If
                        If ta(i).Contains("<parent>vehicles") Then
                            ta(i) = ""
                        End If
                    Next
                Next
                If save_it Then
                    ts = ""
                    For i = 0 To ta.Length - 2
                        If ta(i).Length > 3 Then
                            ts += ta(i) + vbCrLf
                        End If
                    Next
                    ts += ta(ta.Length - 1)
                    If ts.Contains("map_>") Then
                        ts = ts.Replace("map_", Path.GetFileName(tn) + ".model")
                    Else
                        ts = ts.Replace("map_", "")
                    End If
                    File.WriteAllText(fn, ts)
                End If
                '------------------------------------------------------------------------------
                Return

            End If
        Next

    End Sub
    Private Function find_all_textures_in_visual(ByVal crash_path As String, ByRef names() As String) As String
        Dim cnt As Integer = 0
        ReDim names(100)
        Dim f = get_bw_xml(crash_path)
        f = f.Replace(vbCr, "")
        Dim ar = f.Split(vbLf)
        For i = 0 To ar.Length - 1
            If ar(i).ToLower.Contains("<texture>") Then
                Dim tex1_pos = InStr(1, ar(i), "<Texture>") + "<texture>".Length
                Dim tex1_Epos = InStr(tex1_pos, ar(i), "</Texture>")
                names(cnt) = Mid(ar(i), tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
                cnt += 1
            End If
        Next
        ReDim Preserve names(cnt - 1)
        Return f
    End Function
    Private Sub fix_paths_visual(ByVal item As String, ByVal p As String, ByVal author As String, ByVal tn As String)
        Dim di As New DirectoryInfo(p)
        Dim files = di.GetFiles("*.visual_processed")
        For Each f In files
            If f.FullName.ToLower.Contains(item) Then
                Dim fn = f.FullName
                Dim ts = get_bw_xml(fn)
                ts = ts.Replace(vbCr, "")
                Dim ta = ts.Split(vbLf)
                Dim idx As Integer
                'find the part of the tank we are looking for in _groups and get and index to it.
                'this will loop more than once on left and right chassis and track parts :(
                Dim save_it As Boolean = False
                For idx = 1 To object_count
                    If _group(idx).tank_part.ToLower.Contains(item) Then
                        For i = 0 To ta.Length - 1
                            If Not ta(i).ToLower.Contains(author.ToLower) Then
                                ta(i) = replace_string(ta(i), idx, author, tn)
                                save_it = True
                            End If
                        Next
                    End If
                Next
                If save_it Then
                    ts = ""
                    For i = 0 To ta.Length - 2
                        If ta(i).Length > 3 Then
                            ts += ta(i) + vbCrLf
                        End If
                    Next
                    ts += ta(ta.Length - 1)
                    If ts.Contains("map_>") Then
                        ts = ts.Replace("map_", Path.GetFileName(tn) + ".visual_processed")
                    Else
                        ts = ts.Replace("map_", "")
                    End If
                    File.WriteAllText(fn, ts)
                End If
                '------------------------------------------------------------------------------
                Return

            End If
        Next

    End Sub
    Private Sub fix_crash_paths_visual(ByVal item As String, ByVal p As String, _
                                       ByVal author As String, ByVal tn As String, _
                                       ByRef names() As String)
        Dim save_it As Boolean = False
        Dim di As New DirectoryInfo(p)
        Dim files = di.GetFiles("*.visual_processed")
        For Each f In files
            If f.FullName.ToLower.Contains(item) Then
                Dim fn = f.FullName
                Dim file_ = find_all_textures_in_visual(fn, names)
                For Each n In names
                    n = n.Replace("\", "/")
                    If File.Exists(My.Settings.res_mods_path + "\res\" + n) Then 'check if this texture is in the tanks root path
                        Dim nr = n.Replace(tn, author + "/remodels/" + Path.GetFileName(tn))
                        If Not file_.Contains(nr) Then
                            file_ = file_.Replace(n, nr)
                            save_it = True
                        End If
                    End If
                Next
                If save_it Then
                    If file_.Contains("map_>") Then
                        file_ = file_.Replace("map_", Path.GetFileName(tn) + ".visual_processed")
                    Else
                        file_ = file_.Replace("map_", "")
                    End If
                    File.WriteAllText(fn, file_)
                End If
                '------------------------------------------------------------------------------
                Return

            End If
        Next

    End Sub
    Private Function replace_string(ByVal ts As String, ByVal idx As Integer, ByVal author As String, ByVal tn As String) As String
        Dim n, np As String
        'color
        If _group(idx).AM_in_res_mods Then
            n = _group(idx).color_name
            If n Is Nothing Then
                n = "!"
            End If
            n = n.Replace("\", "/")
            np = n.Replace(tn, author + "/remodels/" + tn)
            ts = ts.Replace(n, np)
        End If
        'AO
        If _group(idx).AO_in_res_mods Then
            n = _group(idx).ao_name
            If n Is Nothing Then
                n = "!"
            End If
            n = n.Replace("\", "/")
            np = n.Replace(tn, author + "/remodels/" + tn)
            ts = ts.Replace(n, np)
        End If
        'Normal Map
        If _group(idx).ANM_in_res_mods Then
            n = _group(idx).normal_name
            If n Is Nothing Then
                n = "!"
            End If
            n = n.Replace("\", "/")
            np = n.Replace(tn, author + "/remodels/" + tn)
            ts = ts.Replace(n, np)
        End If
        'GMM Map
        If _group(idx).GMM_in_res_mods Then
            n = _group(idx).metalGMM_name
            If n Is Nothing Then
                n = "!"
            End If
            n = n.Replace("\", "/")
            np = n.Replace(tn, author + "/remodels/" + tn)
            ts = ts.Replace(n, np)
        End If
        'Specular (Only in tanks modded to have this)
        If _group(idx).Spec_in_res_mods Then
            n = _group(idx).specular_name
            If n Is Nothing Then
                n = "!"
            End If
            n = n.Replace("\", "/")
            np = n.Replace(tn, author + "/remodels/" + tn)
            ts = ts.Replace(n, np)
        End If
        Return ts

    End Function

    '********
    Private Sub m_build_wotmod_Click(sender As Object, e As EventArgs) Handles m_build_wotmod.Click
        If Not MODEL_LOADED Then
            MsgBox("You need to let me know what tank to package." + vbCrLf _
                    + "Please load the tank you wish to create the wotmod from." _
                    , MsgBoxStyle.Exclamation, "Load a tank first")
            Return
        End If
        If CRASH_MODE Then
            MsgBox("You need to load a non-crash tank!" + vbCrLf _
        + "I can not build the WOTMOD with loaded crash data." _
        , MsgBoxStyle.Exclamation, "Load a non-crash tank first")
            Return
        End If
        Dim p = My.Settings.res_mods_path
        If Not p.Contains("res_mods") Then
            MsgBox("You need to set the path to res_mods.", MsgBoxStyle.Exclamation, "No res_mods Path")
            Return
        End If
        'make sure we have loaded the modded files!
        If Not loaded_from_resmods Then
            MsgBox("In order to create a wotmod file you" + vbCrLf + _
                       "must load tank data from res_mods." + vbCrLf + _
                       "You can create a wotmod from unmodified data" + vbCrLf + _
                       "but that would be silly.", MsgBoxStyle.Exclamation, "No Data in Res_Mods to bundle!")
            Return
        End If
        Dim tank = TANK_NAME
        If tank.Contains(":") Then
            Dim a = tank.Split(":")
            tank = a(0)
        End If
        'second chance testing for data in res_mods. Not really needed now.
        Dim tp = p + "\" + tank
        If Not Directory.Exists(Path.GetDirectoryName(tp)) Then
            MsgBox("There is no data in res_mods for this tank", MsgBoxStyle.Exclamation, "No res_mods data")
            Return
        End If
        Dim ar = tank.Split("\")
        Dim tname = ar(ar.Length - 1)
        'setup what we can on the authors form...

        frmAuthor.Visible = True
        frmAuthor.tank_mod_name = tname
        frmAuthor.creator_tb.Text = My.Settings.authers_name
        frmAuthor.mod_name_tb.Text = frmAuthor.creator_tb.Text + ".remodel." + frmAuthor.tank_mod_name
        frmAuthor.version_tb.Text = Path.GetFileName(p)
        frmAuthor.description_tb.Text = "Author: " + frmAuthor.creator_tb.Text + " Remodel of: " + tname
        frmAuthor.human_readable_tb.Text = tname + " Remodel"
        frmAuthor.Visible = False
        frmAuthor.ShowDialog(Me)
        If frmAuthor.FromDialogResult = Forms.DialogResult.Cancel Then
            Return
        End If
        'load template and replace strings with out strings...
        Dim meta = File.ReadAllText(Application.StartupPath + "\Templates\meta_template.txt")
        meta = meta.Replace("TI", frmAuthor.mod_name_tb.Text)
        meta = meta.Replace("PV", frmAuthor.version_tb.Text)
        meta = meta.Replace("HRN", frmAuthor.human_readable_tb.Text)
        meta = meta.Replace("HRD", frmAuthor.description_tb.Text)
        File.WriteAllText(Temp_Storage + "\meta.xml", meta)

        info_Label.Visible = True
        info_Label.Parent = pb1
        info_Label.Text = "Select location and name for the wotmod file..."
        Application.DoEvents()
        SaveFileDialog1.FileName = frmAuthor.mod_name_tb.Text.Replace(".", "_")
        SaveFileDialog1.Filter = "wotmod file (*.wotmod)|*.wotmod"
        SaveFileDialog1.Title = "Save wotmod..."
        SaveFileDialog1.InitialDirectory = My.Settings.wotmod_path
        If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            My.Settings.wotmod_path = SaveFileDialog1.FileName
        Else
            info_Label.Visible = False
            info_Label.Parent = Me
            Return
        End If
        My.Settings.wotmod_path = Path.GetDirectoryName(SaveFileDialog1.FileName)
        'lets find whats in the tanks data and fix paths in XMLs as needed


        Dim f_script = SaveFileDialog1.FileName.Replace(".wotmod", "_scripts.wotmod")
        Dim f_model = SaveFileDialog1.FileName.Replace(".wotmod", "_models.wotmod")

        If File.Exists(f_model) Then
            File.Delete(f_model)
        End If
        If File.Exists(f_script) Then
            File.Delete(f_script)
        End If
        Dim wotmod_model As New Ionic.Zip.ZipFile(f_model)
        wotmod_model.CompressionLevel = Ionic.Zlib.CompressionLevel.None
        wotmod_model.Encryption = EncryptionAlgorithm.None

        Dim wotmod_scripts As New Ionic.Zip.ZipFile(f_script)
        Dim scripts_exist As Boolean = False
        'wotmod_model.AddFile(Temp_Storage + "\meta.xml", "")
        'wotmod_scripts.AddFile(Temp_Storage + "\meta.xml", "")
        searched_files = 0
        Dim di = getAllFolders(p)
        searched_files = 0
        '------------------------------------------------------------------
        Dim new_path As String = My.Settings.res_mods_path + "\res\"
        If Not Directory.Exists(new_path) Then
            Directory.CreateDirectory(new_path)
        Else
            Directory.Delete(new_path, True) 'clean out old data
            Directory.CreateDirectory(new_path)
        End If
        '------------------------------------------------------------------
        For Each f In di
            If f.Contains(tname) Then
                If File.Exists(f) Then
                    Dim ff = f.Replace(My.Settings.res_mods_path, "")
                    'ff = ff.Replace("vehicles\", "vehicles\remodel\")
                    'ff = ff.Replace("\" + tn + "\", "\" + frmAuthor.creator_tb.Text + "_" + tn + "\")
                    'ff = ff.Replace("vehicles", "res\vehicles")
                    ff = ff.Replace("\", "/")
                    wotmod_model.AddFile(f, Path.GetDirectoryName(ff))

                    searched_files += 1
                End If
            End If
        Next
        wotmod_model.Save(f_model)
        For Each f In wotmod_model
            f.Extract(new_path, ExtractExistingFileAction.OverwriteSilently)
        Next
        wotmod_model.Dispose()

        'File.Delete(f_model)
        wotmod_model.Dispose()
        File.Delete(f_model)
        '------------------------------------------------------------------

        'we have all files in the temp res folder.. now lets fix paths in xmls
        If Not validate_tank_data(new_path + tank + "\", frmAuthor.creator_tb.Text, tank) Then
            Directory.Delete(new_path, True) 'somthing went wrong.. delete temp res folder and return
            GC.Collect() 'cleans out garbage in the garbage collecor
            Return
        End If

        Dim tank_sr_name = Path.GetFileName(public_icon_path)
        Dim an = tank_sr_name.Split("-")
        tank_sr_name = an(1)
        Dim sss = public_icon_path.Replace(an(0) + "-", "")
        Dim srs = Path.GetDirectoryName(sss) + public_icon_path.Replace(public_icon_path.Replace(an(1) + "-", ""), "\420x307\" + tank_sr_name)

        Dim source_420x307_path = My.Settings.res_mods_path + "\" + srs
        Dim dest_420x307_path = My.Settings.res_mods_path + "\temp\res\" + srs
        Dim p_path = Path.GetDirectoryName(tank)
        Dim new_path_save As String = My.Settings.res_mods_path + "\temp\res\" + p_path
        Dim source_scripts_path As String = My.Settings.res_mods_path + "\res\scripts"
        Dim dest_scripts_path As String = My.Settings.res_mods_path + "\temp\res\scripts"
        Dim source_gui_path As String = My.Settings.res_mods_path + "\" + public_icon_path
        Dim dest_gui_path As String = My.Settings.res_mods_path + "\temp\res\" + public_icon_path
        Dim source_path As String = My.Settings.res_mods_path + "\res\" + p_path
        Dim final_path = new_path_save + "\" + frmAuthor.creator_tb.Text + "\remodels"
        Directory.CreateDirectory(final_path)
        CopyDirectory(source_path, final_path)
        CopyDirectory(source_scripts_path, dest_scripts_path)
        If File.Exists(source_gui_path) Then
            If Not Directory.Exists(Path.GetDirectoryName(dest_gui_path)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(dest_gui_path))
            End If
            File.Copy(source_gui_path, dest_gui_path)
        End If
        If File.Exists(source_420x307_path) Then
            If Not Directory.Exists(Path.GetDirectoryName(dest_420x307_path)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(dest_420x307_path))
            End If
            File.Copy(source_420x307_path, dest_420x307_path)
        End If
        Directory.Delete(new_path, True) 'clean out old data
        'Return
        'save the meta.xml
        File.WriteAllText(My.Settings.res_mods_path + "\temp\res\meta.xml", meta)

        Dim ps As New ProcessStartInfo
        ps.FileName = Application.StartupPath + "\" + "7za.exe"
        ps.Arguments = " a -tzip " + f_model + " " + My.Settings.res_mods_path + "\temp\res\" + " -r -mx0"

        Dim r = Process.Start(ps)
        r.WaitForExit()
        Dim ps2 As New ProcessStartInfo
        ps2.FileName = Application.StartupPath + "\" + "7za.exe"
        ps2.Arguments = " rn " + f_model + " res\meta.xml meta.xml" + "  "

        Dim r2 = Process.Start(ps2)
        r2.WaitForExit()

        r.Dispose()
        r2.Dispose()

        GC.Collect() 'cleans out garbage in the garbage collecor
        'System.IO.Compression.ZipFile.CreateFromDirectory(new_path, f_model, CompressionLevel.NoCompression, False)
        info_Label.Text = "Found " + searched_files.ToString("0000") + " relevant files."
        If scripts_exist Then ' save the scripts file if the thank has them
            'wotmod_scripts.Save(f_script)
        End If
        Directory.Delete(My.Settings.res_mods_path + "\temp", True)
        MsgBox("< wotmod built >", MsgBoxStyle.OkOnly, "DONE!")
        info_Label.Visible = False
        info_Label.Parent = Me
    End Sub
    Private Sub extract_tanks_xml_file()
        Dim ts = itemDefXmlString
        Try ' catch any exception thrown

            Dim ip = My.Settings.res_mods_path + "\" + itemDefPathString.Replace(" ", "")
            prep_tanks_xml(itemDefXmlString)
            itemDefXmlString = itemDefXmlString.Replace("  ", vbTab)
            itemDefXmlString = itemDefXmlString.Replace("><", ">" + vbCrLf + "<")
            itemDefXmlString = itemDefXmlString.Replace("<xmlref>", "<!--<xmlref>")
            itemDefXmlString = itemDefXmlString.Replace("</xmlref>", "</xmlref>-->")
            itemDefXmlString = itemDefXmlString.Replace("formfactor_rect1x4direction_left_to_right", "formfactor_rect1x4 direction_left_to_right")
            If Not Directory.Exists(Path.GetDirectoryName(ip)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(ip))
            End If
            itemDefXmlString = itemDefXmlString.Replace("map_nation", Path.GetFileNameWithoutExtension(file_name) + ".xml")
            File.WriteAllText(ip, itemDefXmlString, Encoding.ASCII)
        Catch ex As Exception
            itemDefXmlString = ts
            MsgBox(file_name + vbCrLf + ex.Message, MsgBoxStyle.Critical, "Shit!!")
            Return
        End Try

        itemDefXmlString = ts

    End Sub
    Public Sub CopyDirectory(ByVal sourcePath As String, ByVal destinationPath As String)
        Dim sourceDirectoryInfo As New System.IO.DirectoryInfo(sourcePath)

        ' If the destination folder don't exist then create it
        If Not System.IO.Directory.Exists(destinationPath) Then
            System.IO.Directory.CreateDirectory(destinationPath)
        End If

        Dim fileSystemInfo As System.IO.FileSystemInfo
        For Each fileSystemInfo In sourceDirectoryInfo.GetFileSystemInfos
            Dim destinationFileName As String =
                System.IO.Path.Combine(destinationPath, fileSystemInfo.Name)

            ' Now check whether its a file or a folder and take action accordingly
            If TypeOf fileSystemInfo Is System.IO.FileInfo Then
                System.IO.File.Copy(fileSystemInfo.FullName, destinationFileName, True)
            Else
                ' Recursively call the mothod to copy all the neste folders
                CopyDirectory(fileSystemInfo.FullName, destinationFileName)
            End If
        Next
    End Sub
    Private Function getAllFolders(ByVal directory As String) As String()
        'Create object
        Dim fi As New IO.DirectoryInfo(directory)
        'Array to store paths
        Dim path() As String = {}
        'Loop through subfolders
        For Each subfolder As IO.DirectoryInfo In fi.GetDirectories()
            'Add this folders name
            Array.Resize(path, path.Length + 1)
            path(path.Length - 1) = subfolder.FullName
            'Recall function with each subdirectory
            For Each s As String In getAllFolders(subfolder.FullName)
                Dim di As New IO.DirectoryInfo(s)
                Try
                    For Each f In di.GetFiles
                        Array.Resize(path, path.Length + 1)
                        path(path.Length - 1) = f.DirectoryName + "\" + f.Name
                        searched_files += 1
                    Next
                Catch ex As Exception
                    Array.Resize(path, path.Length + 1)
                    path(path.Length - 1) = di.FullName
                    searched_files += 1
                End Try

            Next
        Next
        info_Label.Text = "Searching files for relevance: " + searched_files.ToString("0000")
        Application.DoEvents()
        Return path
    End Function

#End Region

#Region "Game Launching"

    Private Sub m_test_wotmod_Click(sender As Object, e As EventArgs) Handles m_test_wotmod.Click
        launch_test("/res_mods/")
    End Sub

    Private Sub m_test_res_mods_Click(sender As Object, e As EventArgs) Handles m_test_res_mods.Click
        launch_test("/mods/")
    End Sub
    Private Sub launch_test(ByVal find As String)
        Dim original As String
        Dim ver = Path.GetFileName(My.Settings.res_mods_path)
        If Not Directory.Exists(My.Settings.res_mods_path + "\!_test\") Then
            Directory.CreateDirectory(My.Settings.res_mods_path + "\!_test\")
        End If
        If Not Directory.Exists(My.Settings.game_path + "\mods\" + ver + "\!_test\") Then
            Directory.CreateDirectory(My.Settings.game_path + "\mods\" + ver + "\!_test\")
        End If
        If File.Exists(My.Settings.game_path + "\paths.xml") Then

            Dim paths = File.ReadAllText(My.Settings.game_path + "\paths.xml")
            If Not File.Exists(My.Settings.game_path + "\paths_backup.xml") Then
                File.WriteAllText(My.Settings.game_path + "\paths_backup.xml", paths)
                original = paths
            Else
                original = File.ReadAllText(My.Settings.game_path + "\paths_backup.xml")
            End If
            Dim ar = paths.Split(vbLf)
            For i = 0 To ar.Length - 1
                If ar(i).Contains(find) Then
                    ar(i) = ar(i).Replace("</Path>", "/!_test/</Path>")
                    Exit For
                End If
            Next
            paths = ""
            For i = 0 To ar.Length - 2
                paths += ar(i) + vbLf
            Next
            paths += ar(ar.Length - 1)
            File.WriteAllText(My.Settings.game_path + "\paths.xml", paths)
            'Return
            '====================================
            Me.WindowState = FormWindowState.Minimized
            Application.DoEvents()
            '====================================
            Dim p = Process.Start(My.Settings.game_path + "\WorldOfTanks.exe")
            p.WaitForExit()
            '====================================
            'restore paths.xml
            File.WriteAllText(My.Settings.game_path + "\paths.xml", original)
            Me.WindowState = FormWindowState.Normal
        Else
            MsgBox("I can not find the paths.xml. Have you set the game path?", MsgBoxStyle.Exclamation, "Check Game Path!")
            Return
        End If

    End Sub
    Private Sub m_pythonLog_Click(sender As Object, e As EventArgs) Handles m_pythonLog.Click
        If File.Exists(My.Settings.game_path + "\Python.log") Then
            Dim t As String = My.Settings.game_path + "\python.log"
            System.Diagnostics.Process.Start("notepad.exe", t)
        Else
            MsgBox("I can not find the Python.log. Have you set the game path?", MsgBoxStyle.Exclamation, "Check Game Path!")
            Return
        End If
    End Sub
    Private Sub m_clear_PythonLog_Click(sender As Object, e As EventArgs) Handles m_clear_PythonLog.Click
        If File.Exists(My.Settings.game_path + "\Python.log") Then
            Dim t As String = ""
            File.WriteAllText(My.Settings.game_path + "\python.log", t)
        Else
            MsgBox("I can not find the Python.log. Have you set the game path?", MsgBoxStyle.Exclamation, "Check Game Path!")
            Return
        End If
    End Sub

#End Region


End Class
