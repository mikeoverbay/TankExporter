<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.Startup_Timer = New System.Windows.Forms.Timer(Me.components)
        Me.MM = New System.Windows.Forms.MenuStrip()
        Me.m_file = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_test = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_test_res_mods = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator21 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_pythonLog = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator22 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_clear_PythonLog = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_file = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_save = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_import_2016_fbx = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_import_GLB = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator8 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_import_primitives_fbx = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator29 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_remove_fbx = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_write_primitive = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_write_non_tank_primitive = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator18 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_build_wotmod = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_load_primitive = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator28 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tangent_normalMaps = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_edit_visual = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_log = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator34 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_Open_game_folder = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_open_wot_temp_folder = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_region_combo = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripSeparator11 = New System.Windows.Forms.ToolStripSeparator()
        Me.M_Path = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_res_mods_path = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_rebuild_XML = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_clear_temp_folder_data = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_reload_api_data = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_UI_settings = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_settings = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_hide_show_components = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator27 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_set_vertex_winding_order = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator26 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_FXAA = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_bloom_off = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator23 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_shadows = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_shadowQuality = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_select_light = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator24 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_simple_lighting = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_enableBloom = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_environment = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_shadow_preview = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator25 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_Shader_Debug = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_edit_shaders = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator31 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_dump_tanks = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_enable_tarrain_decals = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator19 = New System.Windows.Forms.ToolStripSeparator()
        Me.M_Exit = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_ExportExtract = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_extract = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator32 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_export_to_glTF = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_2013_fbx = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator33 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_export_STL = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_export_to_obj = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator15 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_view_res_mods_folder = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_clean_res_mods = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator17 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_screen_cap = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_model_info = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_show_fbx = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_pick_camo = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_edit_camo = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_load_textures = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_GMM_toy_cb = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_lighting = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_donate = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_decal = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_forums = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_help = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_hide_right_plane = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator14 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator12 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator10 = New System.Windows.Forms.ToolStripSeparator()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer()
        Me.VertexColor_cb = New System.Windows.Forms.CheckBox()
        Me.show_textures_cb = New System.Windows.Forms.CheckBox()
        Me.wire_cb = New System.Windows.Forms.CheckBox()
        Me.grid_cb = New System.Windows.Forms.CheckBox()
        Me.gun_cb = New System.Windows.Forms.CheckBox()
        Me.turret_cb = New System.Windows.Forms.CheckBox()
        Me.hull_cb = New System.Windows.Forms.CheckBox()
        Me.chassis_cb = New System.Windows.Forms.CheckBox()
        Me.intro_label = New System.Windows.Forms.Label()
        Me.decal_panel = New System.Windows.Forms.Panel()
        Me.copy_Decal_btn = New System.Windows.Forms.Button()
        Me.hide_BB_cb = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.decal_alpha_slider = New System.Windows.Forms.TrackBar()
        Me.decal_level_slider = New System.Windows.Forms.TrackBar()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.dgv = New System.Windows.Forms.DataGridView()
        Me.decalName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.decalID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.uv_rotate = New System.Windows.Forms.DomainUpDown()
        Me.save_decal_btn = New System.Windows.Forms.Button()
        Me.load_decal_btn = New System.Windows.Forms.Button()
        Me.track_decal_cb = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Uwrap = New System.Windows.Forms.DomainUpDown()
        Me.Vwrap = New System.Windows.Forms.DomainUpDown()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.d_texture_name = New System.Windows.Forms.Label()
        Me.mouse_pick_cb = New System.Windows.Forms.CheckBox()
        Me.d_move_down = New System.Windows.Forms.Button()
        Me.d_move_up = New System.Windows.Forms.Button()
        Me.m_sel_texture = New System.Windows.Forms.Button()
        Me.m_delete = New System.Windows.Forms.Button()
        Me.m_new = New System.Windows.Forms.Button()
        Me.current_decal_lable = New System.Windows.Forms.Label()
        Me.PB3 = New System.Windows.Forms.PictureBox()
        Me.font_holder = New System.Windows.Forms.Label()
        Me.pb2 = New System.Windows.Forms.Panel()
        Me.pb1 = New System.Windows.Forms.PictureBox()
        Me.info_Label = New System.Windows.Forms.Label()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.SearchBox = New System.Windows.Forms.TextBox()
        Me.TC1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.TabPage7 = New System.Windows.Forms.TabPage()
        Me.TabPage8 = New System.Windows.Forms.TabPage()
        Me.TabPage9 = New System.Windows.Forms.TabPage()
        Me.TabPage10 = New System.Windows.Forms.TabPage()
        Me.TC2 = New System.Windows.Forms.TabControl()
        Me.TabPage11 = New System.Windows.Forms.TabPage()
        Me.tank_label = New System.Windows.Forms.Label()
        Me.iconbox = New System.Windows.Forms.PictureBox()
        Me.conMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.m_load = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator13 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_load_crashed = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator16 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_reload_textures = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator30 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.decal_ds = New System.Data.DataSet()
        Me.d_table = New System.Data.DataTable()
        Me.DataColumn1 = New System.Data.DataColumn()
        Me.DataColumn2 = New System.Data.DataColumn()
        Me.MM.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.decal_panel.SuspendLayout()
        CType(Me.decal_alpha_slider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.decal_level_slider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgv, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PB3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pb1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.TC1.SuspendLayout()
        Me.TC2.SuspendLayout()
        CType(Me.iconbox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.conMenu.SuspendLayout()
        CType(Me.decal_ds, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.d_table, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Startup_Timer
        '
        Me.Startup_Timer.Interval = 500
        '
        'MM
        '
        Me.MM.AutoSize = False
        Me.MM.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_file, Me.m_ExportExtract, Me.m_show_model_info, Me.m_show_fbx, Me.m_pick_camo, Me.m_edit_camo, Me.m_load_textures, Me.m_GMM_toy_cb, Me.m_lighting, Me.m_donate, Me.m_decal, Me.m_forums, Me.m_help, Me.m_hide_right_plane})
        Me.MM.Location = New System.Drawing.Point(0, 0)
        Me.MM.Name = "MM"
        Me.MM.Size = New System.Drawing.Size(968, 27)
        Me.MM.TabIndex = 1
        '
        'm_file
        '
        Me.m_file.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_test, Me.m_load_file, Me.m_save, Me.ToolStripSeparator7, Me.m_import_2016_fbx, Me.m_import_GLB, Me.ToolStripSeparator8, Me.m_import_primitives_fbx, Me.ToolStripSeparator29, Me.m_remove_fbx, Me.ToolStripSeparator1, Me.m_write_primitive, Me.m_write_non_tank_primitive, Me.ToolStripSeparator18, Me.m_build_wotmod, Me.ToolStripSeparator9, Me.m_load_primitive, Me.ToolStripSeparator28, Me.m_tangent_normalMaps, Me.m_edit_visual, Me.m_show_log, Me.ToolStripSeparator34, Me.m_Open_game_folder, Me.m_open_wot_temp_folder, Me.ToolStripSeparator4, Me.m_region_combo, Me.ToolStripSeparator11, Me.M_Path, Me.m_res_mods_path, Me.ToolStripSeparator2, Me.m_rebuild_XML, Me.m_clear_temp_folder_data, Me.m_reload_api_data, Me.ToolStripSeparator3, Me.m_UI_settings, Me.ToolStripSeparator31, Me.m_dump_tanks, Me.m_enable_tarrain_decals, Me.ToolStripSeparator19, Me.M_Exit})
        Me.m_file.Name = "m_file"
        Me.m_file.Size = New System.Drawing.Size(90, 23)
        Me.m_file.Text = "&File / Settings"
        '
        'm_test
        '
        Me.m_test.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_test_res_mods, Me.ToolStripSeparator21, Me.m_pythonLog, Me.ToolStripSeparator22, Me.m_clear_PythonLog})
        Me.m_test.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_test.ForeColor = System.Drawing.Color.Black
        Me.m_test.Name = "m_test"
        Me.m_test.Size = New System.Drawing.Size(230, 22)
        Me.m_test.Text = "Start Wot"
        '
        'm_test_res_mods
        '
        Me.m_test_res_mods.Name = "m_test_res_mods"
        Me.m_test_res_mods.Size = New System.Drawing.Size(194, 22)
        Me.m_test_res_mods.Text = "Launch World of Tanks"
        '
        'ToolStripSeparator21
        '
        Me.ToolStripSeparator21.Name = "ToolStripSeparator21"
        Me.ToolStripSeparator21.Size = New System.Drawing.Size(191, 6)
        '
        'm_pythonLog
        '
        Me.m_pythonLog.Name = "m_pythonLog"
        Me.m_pythonLog.Size = New System.Drawing.Size(194, 22)
        Me.m_pythonLog.Text = "Load Python.log"
        '
        'ToolStripSeparator22
        '
        Me.ToolStripSeparator22.Name = "ToolStripSeparator22"
        Me.ToolStripSeparator22.Size = New System.Drawing.Size(191, 6)
        '
        'm_clear_PythonLog
        '
        Me.m_clear_PythonLog.Name = "m_clear_PythonLog"
        Me.m_clear_PythonLog.Size = New System.Drawing.Size(194, 22)
        Me.m_clear_PythonLog.Text = "Clear Python.log"
        '
        'm_load_file
        '
        Me.m_load_file.Name = "m_load_file"
        Me.m_load_file.Size = New System.Drawing.Size(230, 22)
        Me.m_load_file.Text = "Load"
        '
        'm_save
        '
        Me.m_save.Name = "m_save"
        Me.m_save.Size = New System.Drawing.Size(230, 22)
        Me.m_save.Text = "Save"
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(227, 6)
        '
        'm_import_2016_fbx
        '
        Me.m_import_2016_fbx.Name = "m_import_2016_fbx"
        Me.m_import_2016_fbx.Size = New System.Drawing.Size(230, 22)
        Me.m_import_2016_fbx.Text = "Import 2014 FBX"
        '
        'm_import_GLB
        '
        Me.m_import_GLB.Name = "m_import_GLB"
        Me.m_import_GLB.Size = New System.Drawing.Size(230, 22)
        Me.m_import_GLB.Text = "Import GLB"
        '
        'ToolStripSeparator8
        '
        Me.ToolStripSeparator8.Name = "ToolStripSeparator8"
        Me.ToolStripSeparator8.Size = New System.Drawing.Size(227, 6)
        '
        'm_import_primitives_fbx
        '
        Me.m_import_primitives_fbx.Name = "m_import_primitives_fbx"
        Me.m_import_primitives_fbx.Size = New System.Drawing.Size(230, 22)
        Me.m_import_primitives_fbx.Text = "Import Primitives FBX"
        '
        'ToolStripSeparator29
        '
        Me.ToolStripSeparator29.Name = "ToolStripSeparator29"
        Me.ToolStripSeparator29.Size = New System.Drawing.Size(227, 6)
        '
        'm_remove_fbx
        '
        Me.m_remove_fbx.Name = "m_remove_fbx"
        Me.m_remove_fbx.Size = New System.Drawing.Size(230, 22)
        Me.m_remove_fbx.Text = "Remove Models"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(227, 6)
        '
        'm_write_primitive
        '
        Me.m_write_primitive.Enabled = False
        Me.m_write_primitive.Name = "m_write_primitive"
        Me.m_write_primitive.Size = New System.Drawing.Size(230, 22)
        Me.m_write_primitive.Text = "Write Primitive"
        '
        'm_write_non_tank_primitive
        '
        Me.m_write_non_tank_primitive.Enabled = False
        Me.m_write_non_tank_primitive.Name = "m_write_non_tank_primitive"
        Me.m_write_non_tank_primitive.Size = New System.Drawing.Size(230, 22)
        Me.m_write_non_tank_primitive.Text = "Write NON tank Primitive"
        '
        'ToolStripSeparator18
        '
        Me.ToolStripSeparator18.Name = "ToolStripSeparator18"
        Me.ToolStripSeparator18.Size = New System.Drawing.Size(227, 6)
        '
        'm_build_wotmod
        '
        Me.m_build_wotmod.Name = "m_build_wotmod"
        Me.m_build_wotmod.Size = New System.Drawing.Size(230, 22)
        Me.m_build_wotmod.Text = "Build wotmod file"
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(227, 6)
        '
        'm_load_primitive
        '
        Me.m_load_primitive.Name = "m_load_primitive"
        Me.m_load_primitive.Size = New System.Drawing.Size(230, 22)
        Me.m_load_primitive.Text = "Load primitives_processed"
        '
        'ToolStripSeparator28
        '
        Me.ToolStripSeparator28.Name = "ToolStripSeparator28"
        Me.ToolStripSeparator28.Size = New System.Drawing.Size(227, 6)
        '
        'm_tangent_normalMaps
        '
        Me.m_tangent_normalMaps.Checked = True
        Me.m_tangent_normalMaps.CheckOnClick = True
        Me.m_tangent_normalMaps.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_tangent_normalMaps.Name = "m_tangent_normalMaps"
        Me.m_tangent_normalMaps.Size = New System.Drawing.Size(230, 22)
        Me.m_tangent_normalMaps.Text = "Tangent FBX Normal Maps"
        '
        'm_edit_visual
        '
        Me.m_edit_visual.Name = "m_edit_visual"
        Me.m_edit_visual.Size = New System.Drawing.Size(230, 22)
        Me.m_edit_visual.Text = "Show Visual Files"
        '
        'm_show_log
        '
        Me.m_show_log.Name = "m_show_log"
        Me.m_show_log.Size = New System.Drawing.Size(230, 22)
        Me.m_show_log.Text = "Show Log File"
        '
        'ToolStripSeparator34
        '
        Me.ToolStripSeparator34.Name = "ToolStripSeparator34"
        Me.ToolStripSeparator34.Size = New System.Drawing.Size(227, 6)
        '
        'm_Open_game_folder
        '
        Me.m_Open_game_folder.Name = "m_Open_game_folder"
        Me.m_Open_game_folder.Size = New System.Drawing.Size(230, 22)
        Me.m_Open_game_folder.Text = "Open WOT Folder"
        '
        'm_open_wot_temp_folder
        '
        Me.m_open_wot_temp_folder.Name = "m_open_wot_temp_folder"
        Me.m_open_wot_temp_folder.Size = New System.Drawing.Size(230, 22)
        Me.m_open_wot_temp_folder.Text = "Open wot_temp folder"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(227, 6)
        '
        'm_region_combo
        '
        Me.m_region_combo.Items.AddRange(New Object() {"NA", "EU", "RU", "ASIA"})
        Me.m_region_combo.Name = "m_region_combo"
        Me.m_region_combo.Size = New System.Drawing.Size(121, 23)
        Me.m_region_combo.Text = "NA"
        '
        'ToolStripSeparator11
        '
        Me.ToolStripSeparator11.Name = "ToolStripSeparator11"
        Me.ToolStripSeparator11.Size = New System.Drawing.Size(227, 6)
        Me.ToolStripSeparator11.Visible = False
        '
        'M_Path
        '
        Me.M_Path.Name = "M_Path"
        Me.M_Path.Size = New System.Drawing.Size(230, 22)
        Me.M_Path.Text = "Path to Game folder"
        '
        'm_res_mods_path
        '
        Me.m_res_mods_path.Name = "m_res_mods_path"
        Me.m_res_mods_path.Size = New System.Drawing.Size(230, 22)
        Me.m_res_mods_path.Text = "Path to res_mods "
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(227, 6)
        '
        'm_rebuild_XML
        '
        Me.m_rebuild_XML.Name = "m_rebuild_XML"
        Me.m_rebuild_XML.Size = New System.Drawing.Size(230, 22)
        Me.m_rebuild_XML.Text = "Rebuild lookup XML"
        '
        'm_clear_temp_folder_data
        '
        Me.m_clear_temp_folder_data.Name = "m_clear_temp_folder_data"
        Me.m_clear_temp_folder_data.Size = New System.Drawing.Size(230, 22)
        Me.m_clear_temp_folder_data.Text = "Clear Temp Folder"
        '
        'm_reload_api_data
        '
        Me.m_reload_api_data.Name = "m_reload_api_data"
        Me.m_reload_api_data.Size = New System.Drawing.Size(230, 22)
        Me.m_reload_api_data.Text = "Reload WoT API data"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(227, 6)
        '
        'm_UI_settings
        '
        Me.m_UI_settings.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_settings, Me.m_hide_show_components, Me.ToolStripSeparator27, Me.m_set_vertex_winding_order, Me.ToolStripSeparator26, Me.m_FXAA, Me.m_bloom_off, Me.ToolStripSeparator23, Me.m_shadows, Me.m_shadowQuality, Me.m_select_light, Me.ToolStripSeparator24, Me.m_simple_lighting, Me.m_enableBloom, Me.m_show_environment, Me.m_shadow_preview, Me.ToolStripSeparator25, Me.m_Shader_Debug, Me.m_edit_shaders})
        Me.m_UI_settings.Name = "m_UI_settings"
        Me.m_UI_settings.Size = New System.Drawing.Size(230, 22)
        Me.m_UI_settings.Text = "UI and View Settings"
        '
        'm_settings
        '
        Me.m_settings.Name = "m_settings"
        Me.m_settings.Size = New System.Drawing.Size(349, 22)
        Me.m_settings.Text = "FOV and Mouse Settings"
        '
        'm_hide_show_components
        '
        Me.m_hide_show_components.Name = "m_hide_show_components"
        Me.m_hide_show_components.Size = New System.Drawing.Size(349, 22)
        Me.m_hide_show_components.Text = "Hide/Show Each Model Part"
        '
        'ToolStripSeparator27
        '
        Me.ToolStripSeparator27.Name = "ToolStripSeparator27"
        Me.ToolStripSeparator27.Size = New System.Drawing.Size(346, 6)
        '
        'm_set_vertex_winding_order
        '
        Me.m_set_vertex_winding_order.Name = "m_set_vertex_winding_order"
        Me.m_set_vertex_winding_order.Size = New System.Drawing.Size(349, 22)
        Me.m_set_vertex_winding_order.Text = "Set Vertex Winding Order (Affects writing Primitives)"
        '
        'ToolStripSeparator26
        '
        Me.ToolStripSeparator26.Name = "ToolStripSeparator26"
        Me.ToolStripSeparator26.Size = New System.Drawing.Size(346, 6)
        '
        'm_FXAA
        '
        Me.m_FXAA.Checked = True
        Me.m_FXAA.CheckOnClick = True
        Me.m_FXAA.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_FXAA.Name = "m_FXAA"
        Me.m_FXAA.Size = New System.Drawing.Size(349, 22)
        Me.m_FXAA.Text = "FXAA Anti-Aliasing"
        '
        'm_bloom_off
        '
        Me.m_bloom_off.CheckOnClick = True
        Me.m_bloom_off.Name = "m_bloom_off"
        Me.m_bloom_off.Size = New System.Drawing.Size(349, 22)
        Me.m_bloom_off.Text = "Bloom Off"
        '
        'ToolStripSeparator23
        '
        Me.ToolStripSeparator23.Name = "ToolStripSeparator23"
        Me.ToolStripSeparator23.Size = New System.Drawing.Size(346, 6)
        '
        'm_shadows
        '
        Me.m_shadows.CheckOnClick = True
        Me.m_shadows.Name = "m_shadows"
        Me.m_shadows.Size = New System.Drawing.Size(349, 22)
        Me.m_shadows.Text = "Shadows"
        '
        'm_shadowQuality
        '
        Me.m_shadowQuality.Name = "m_shadowQuality"
        Me.m_shadowQuality.Size = New System.Drawing.Size(349, 22)
        Me.m_shadowQuality.Text = "Set Shadow Quality"
        '
        'm_select_light
        '
        Me.m_select_light.Name = "m_select_light"
        Me.m_select_light.Size = New System.Drawing.Size(349, 22)
        Me.m_select_light.Text = "Set Shadow Light"
        '
        'ToolStripSeparator24
        '
        Me.ToolStripSeparator24.Name = "ToolStripSeparator24"
        Me.ToolStripSeparator24.Size = New System.Drawing.Size(346, 6)
        '
        'm_simple_lighting
        '
        Me.m_simple_lighting.CheckOnClick = True
        Me.m_simple_lighting.Name = "m_simple_lighting"
        Me.m_simple_lighting.Size = New System.Drawing.Size(349, 22)
        Me.m_simple_lighting.Text = "Simple Lighting"
        '
        'm_enableBloom
        '
        Me.m_enableBloom.CheckOnClick = True
        Me.m_enableBloom.Name = "m_enableBloom"
        Me.m_enableBloom.Size = New System.Drawing.Size(349, 22)
        Me.m_enableBloom.Text = "Show Bloom Texture"
        '
        'm_show_environment
        '
        Me.m_show_environment.CheckOnClick = True
        Me.m_show_environment.Name = "m_show_environment"
        Me.m_show_environment.Size = New System.Drawing.Size(349, 22)
        Me.m_show_environment.Text = "Show Environment"
        '
        'm_shadow_preview
        '
        Me.m_shadow_preview.CheckOnClick = True
        Me.m_shadow_preview.Name = "m_shadow_preview"
        Me.m_shadow_preview.Size = New System.Drawing.Size(349, 22)
        Me.m_shadow_preview.Text = "Shadow Preview"
        '
        'ToolStripSeparator25
        '
        Me.ToolStripSeparator25.Name = "ToolStripSeparator25"
        Me.ToolStripSeparator25.Size = New System.Drawing.Size(346, 6)
        '
        'm_Shader_Debug
        '
        Me.m_Shader_Debug.Name = "m_Shader_Debug"
        Me.m_Shader_Debug.Size = New System.Drawing.Size(349, 22)
        Me.m_Shader_Debug.Text = "Tank Shader Debug Settings"
        '
        'm_edit_shaders
        '
        Me.m_edit_shaders.Name = "m_edit_shaders"
        Me.m_edit_shaders.Size = New System.Drawing.Size(349, 22)
        Me.m_edit_shaders.Text = "Edit Shaders"
        '
        'ToolStripSeparator31
        '
        Me.ToolStripSeparator31.Name = "ToolStripSeparator31"
        Me.ToolStripSeparator31.Size = New System.Drawing.Size(227, 6)
        '
        'm_dump_tanks
        '
        Me.m_dump_tanks.Name = "m_dump_tanks"
        Me.m_dump_tanks.Size = New System.Drawing.Size(230, 22)
        Me.m_dump_tanks.Text = "Dump All Tanks as FBX"
        Me.m_dump_tanks.Visible = False
        '
        'm_enable_tarrain_decals
        '
        Me.m_enable_tarrain_decals.Name = "m_enable_tarrain_decals"
        Me.m_enable_tarrain_decals.Size = New System.Drawing.Size(230, 22)
        Me.m_enable_tarrain_decals.Text = "Enable Loading Terrain Decals"
        '
        'ToolStripSeparator19
        '
        Me.ToolStripSeparator19.Name = "ToolStripSeparator19"
        Me.ToolStripSeparator19.Size = New System.Drawing.Size(227, 6)
        '
        'M_Exit
        '
        Me.M_Exit.Name = "M_Exit"
        Me.M_Exit.Size = New System.Drawing.Size(230, 22)
        Me.M_Exit.Text = "Exit"
        '
        'm_ExportExtract
        '
        Me.m_ExportExtract.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_extract, Me.ToolStripSeparator32, Me.m_export_to_glTF, Me.m_2013_fbx, Me.ToolStripSeparator33, Me.m_export_STL, Me.m_export_to_obj, Me.ToolStripSeparator15, Me.m_view_res_mods_folder, Me.ToolStripSeparator6, Me.m_clean_res_mods, Me.ToolStripSeparator17, Me.m_screen_cap})
        Me.m_ExportExtract.Enabled = False
        Me.m_ExportExtract.Name = "m_ExportExtract"
        Me.m_ExportExtract.Size = New System.Drawing.Size(94, 23)
        Me.m_ExportExtract.Text = "Export/Extract"
        '
        'm_extract
        '
        Me.m_extract.Name = "m_extract"
        Me.m_extract.Size = New System.Drawing.Size(228, 22)
        Me.m_extract.Text = "Extract to res_mods folder"
        '
        'ToolStripSeparator32
        '
        Me.ToolStripSeparator32.Name = "ToolStripSeparator32"
        Me.ToolStripSeparator32.Size = New System.Drawing.Size(225, 6)
        '
        'm_export_to_glTF
        '
        Me.m_export_to_glTF.Name = "m_export_to_glTF"
        Me.m_export_to_glTF.Size = New System.Drawing.Size(228, 22)
        Me.m_export_to_glTF.Text = "Export glb"
        '
        'm_2013_fbx
        '
        Me.m_2013_fbx.Name = "m_2013_fbx"
        Me.m_2013_fbx.Size = New System.Drawing.Size(228, 22)
        Me.m_2013_fbx.Text = "Export 2014 FBX"
        '
        'ToolStripSeparator33
        '
        Me.ToolStripSeparator33.Name = "ToolStripSeparator33"
        Me.ToolStripSeparator33.Size = New System.Drawing.Size(225, 6)
        '
        'm_export_STL
        '
        Me.m_export_STL.Name = "m_export_STL"
        Me.m_export_STL.Size = New System.Drawing.Size(228, 22)
        Me.m_export_STL.Text = "Export STL"
        Me.m_export_STL.Visible = False
        '
        'm_export_to_obj
        '
        Me.m_export_to_obj.Name = "m_export_to_obj"
        Me.m_export_to_obj.Size = New System.Drawing.Size(228, 22)
        Me.m_export_to_obj.Text = "Export OBJ"
        Me.m_export_to_obj.Visible = False
        '
        'ToolStripSeparator15
        '
        Me.ToolStripSeparator15.Name = "ToolStripSeparator15"
        Me.ToolStripSeparator15.Size = New System.Drawing.Size(225, 6)
        Me.ToolStripSeparator15.Visible = False
        '
        'm_view_res_mods_folder
        '
        Me.m_view_res_mods_folder.Name = "m_view_res_mods_folder"
        Me.m_view_res_mods_folder.Size = New System.Drawing.Size(228, 22)
        Me.m_view_res_mods_folder.Text = "View in res_mods folder"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(225, 6)
        '
        'm_clean_res_mods
        '
        Me.m_clean_res_mods.Name = "m_clean_res_mods"
        Me.m_clean_res_mods.Size = New System.Drawing.Size(228, 22)
        Me.m_clean_res_mods.Text = "Remove Tank From res_mods"
        '
        'ToolStripSeparator17
        '
        Me.ToolStripSeparator17.Name = "ToolStripSeparator17"
        Me.ToolStripSeparator17.Size = New System.Drawing.Size(225, 6)
        '
        'm_screen_cap
        '
        Me.m_screen_cap.Name = "m_screen_cap"
        Me.m_screen_cap.Size = New System.Drawing.Size(228, 22)
        Me.m_screen_cap.Text = "Screen Capture"
        '
        'm_show_model_info
        '
        Me.m_show_model_info.Name = "m_show_model_info"
        Me.m_show_model_info.Size = New System.Drawing.Size(77, 23)
        Me.m_show_model_info.Text = "Model Info"
        '
        'm_show_fbx
        '
        Me.m_show_fbx.CheckOnClick = True
        Me.m_show_fbx.Name = "m_show_fbx"
        Me.m_show_fbx.Size = New System.Drawing.Size(71, 23)
        Me.m_show_fbx.Text = "Show FBX"
        Me.m_show_fbx.Visible = False
        '
        'm_pick_camo
        '
        Me.m_pick_camo.Enabled = False
        Me.m_pick_camo.Name = "m_pick_camo"
        Me.m_pick_camo.Size = New System.Drawing.Size(84, 23)
        Me.m_pick_camo.Text = "Camouflage"
        '
        'm_edit_camo
        '
        Me.m_edit_camo.Name = "m_edit_camo"
        Me.m_edit_camo.Size = New System.Drawing.Size(107, 23)
        Me.m_edit_camo.Text = "Edit Camouflage"
        Me.m_edit_camo.Visible = False
        '
        'm_load_textures
        '
        Me.m_load_textures.Checked = True
        Me.m_load_textures.CheckOnClick = True
        Me.m_load_textures.CheckState = System.Windows.Forms.CheckState.Checked
        Me.m_load_textures.ForeColor = System.Drawing.Color.Red
        Me.m_load_textures.Name = "m_load_textures"
        Me.m_load_textures.Size = New System.Drawing.Size(94, 23)
        Me.m_load_textures.Text = "Show Textures"
        '
        'm_GMM_toy_cb
        '
        Me.m_GMM_toy_cb.CheckOnClick = True
        Me.m_GMM_toy_cb.Name = "m_GMM_toy_cb"
        Me.m_GMM_toy_cb.Size = New System.Drawing.Size(70, 23)
        Me.m_GMM_toy_cb.Text = "GMM Toy"
        Me.m_GMM_toy_cb.Visible = False
        '
        'm_lighting
        '
        Me.m_lighting.Name = "m_lighting"
        Me.m_lighting.Size = New System.Drawing.Size(63, 23)
        Me.m_lighting.Text = "Lighting"
        '
        'm_donate
        '
        Me.m_donate.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_donate.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.m_donate.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.m_donate.Name = "m_donate"
        Me.m_donate.Size = New System.Drawing.Size(66, 23)
        Me.m_donate.Text = "DONATE"
        '
        'm_decal
        '
        Me.m_decal.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_decal.CheckOnClick = True
        Me.m_decal.Name = "m_decal"
        Me.m_decal.Size = New System.Drawing.Size(73, 23)
        Me.m_decal.Text = "Decal Tool"
        Me.m_decal.Visible = False
        '
        'm_forums
        '
        Me.m_forums.Name = "m_forums"
        Me.m_forums.Size = New System.Drawing.Size(59, 23)
        Me.m_forums.Text = "Forums"
        '
        'm_help
        '
        Me.m_help.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_help.Image = Global.Tank_Exporter.My.Resources.Resources.question
        Me.m_help.Name = "m_help"
        Me.m_help.Size = New System.Drawing.Size(28, 23)
        Me.m_help.Text = "Help"
        '
        'm_hide_right_plane
        '
        Me.m_hide_right_plane.Name = "m_hide_right_plane"
        Me.m_hide_right_plane.Size = New System.Drawing.Size(107, 23)
        Me.m_hide_right_plane.Text = "Hide Right Plane"
        '
        'ToolStripSeparator14
        '
        Me.ToolStripSeparator14.Name = "ToolStripSeparator14"
        Me.ToolStripSeparator14.Size = New System.Drawing.Size(218, 6)
        '
        'ToolStripSeparator5
        '
        Me.ToolStripSeparator5.Name = "ToolStripSeparator5"
        Me.ToolStripSeparator5.Size = New System.Drawing.Size(218, 6)
        '
        'ToolStripSeparator12
        '
        Me.ToolStripSeparator12.Name = "ToolStripSeparator12"
        Me.ToolStripSeparator12.Size = New System.Drawing.Size(218, 6)
        '
        'ToolStripSeparator10
        '
        Me.ToolStripSeparator10.Name = "ToolStripSeparator10"
        Me.ToolStripSeparator10.Size = New System.Drawing.Size(218, 6)
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.IsSplitterFixed = True
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 27)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer3)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Size = New System.Drawing.Size(968, 506)
        Me.SplitContainer1.SplitterDistance = 942
        Me.SplitContainer1.SplitterWidth = 1
        Me.SplitContainer1.TabIndex = 2
        '
        'SplitContainer3
        '
        Me.SplitContainer3.BackColor = System.Drawing.Color.Black
        Me.SplitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer3.ForeColor = System.Drawing.Color.White
        Me.SplitContainer3.IsSplitterFixed = True
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SplitContainer3.Panel1.Controls.Add(Me.VertexColor_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.show_textures_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.wire_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.grid_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.gun_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.turret_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.hull_cb)
        Me.SplitContainer3.Panel1.Controls.Add(Me.chassis_cb)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.gradiant
        Me.SplitContainer3.Panel2.Controls.Add(Me.intro_label)
        Me.SplitContainer3.Panel2.Controls.Add(Me.decal_panel)
        Me.SplitContainer3.Panel2.Controls.Add(Me.PB3)
        Me.SplitContainer3.Panel2.Controls.Add(Me.font_holder)
        Me.SplitContainer3.Panel2.Controls.Add(Me.pb2)
        Me.SplitContainer3.Panel2.Controls.Add(Me.pb1)
        Me.SplitContainer3.Panel2.Controls.Add(Me.info_Label)
        Me.SplitContainer3.Size = New System.Drawing.Size(942, 506)
        Me.SplitContainer3.SplitterDistance = 56
        Me.SplitContainer3.SplitterWidth = 1
        Me.SplitContainer3.TabIndex = 3
        '
        'VertexColor_cb
        '
        Me.VertexColor_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.VertexColor_cb.BackColor = System.Drawing.Color.Gray
        Me.VertexColor_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.VertexColor_cb.FlatAppearance.BorderSize = 2
        Me.VertexColor_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.VertexColor_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.VertexColor_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.VertexColor_cb.ForeColor = System.Drawing.Color.White
        Me.VertexColor_cb.Image = Global.Tank_Exporter.My.Resources.Resources.vertex_color_on
        Me.VertexColor_cb.Location = New System.Drawing.Point(2, 411)
        Me.VertexColor_cb.Name = "VertexColor_cb"
        Me.VertexColor_cb.Size = New System.Drawing.Size(48, 48)
        Me.VertexColor_cb.TabIndex = 9
        Me.VertexColor_cb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.VertexColor_cb.UseVisualStyleBackColor = False
        '
        'show_textures_cb
        '
        Me.show_textures_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.show_textures_cb.BackColor = System.Drawing.Color.Gray
        Me.show_textures_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.images_off
        Me.show_textures_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.show_textures_cb.FlatAppearance.BorderSize = 2
        Me.show_textures_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.show_textures_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.show_textures_cb.ForeColor = System.Drawing.Color.White
        Me.show_textures_cb.Location = New System.Drawing.Point(2, 360)
        Me.show_textures_cb.Name = "show_textures_cb"
        Me.show_textures_cb.Size = New System.Drawing.Size(48, 48)
        Me.show_textures_cb.TabIndex = 8
        Me.show_textures_cb.UseVisualStyleBackColor = False
        '
        'wire_cb
        '
        Me.wire_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.wire_cb.BackColor = System.Drawing.Color.Gray
        Me.wire_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.box_wire
        Me.wire_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.wire_cb.FlatAppearance.BorderSize = 2
        Me.wire_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.wire_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.wire_cb.ForeColor = System.Drawing.Color.White
        Me.wire_cb.Location = New System.Drawing.Point(2, 309)
        Me.wire_cb.Name = "wire_cb"
        Me.wire_cb.Size = New System.Drawing.Size(48, 48)
        Me.wire_cb.TabIndex = 7
        Me.wire_cb.UseVisualStyleBackColor = False
        '
        'grid_cb
        '
        Me.grid_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.grid_cb.BackColor = System.Drawing.Color.Gray
        Me.grid_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.grid_blank
        Me.grid_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.grid_cb.Checked = True
        Me.grid_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.grid_cb.FlatAppearance.BorderSize = 2
        Me.grid_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.grid_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.grid_cb.ForeColor = System.Drawing.Color.White
        Me.grid_cb.Location = New System.Drawing.Point(2, 258)
        Me.grid_cb.Name = "grid_cb"
        Me.grid_cb.Size = New System.Drawing.Size(48, 48)
        Me.grid_cb.TabIndex = 6
        Me.grid_cb.ThreeState = True
        Me.grid_cb.UseVisualStyleBackColor = False
        '
        'gun_cb
        '
        Me.gun_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.gun_cb.BackColor = System.Drawing.Color.Gray
        Me.gun_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.gun
        Me.gun_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.gun_cb.Checked = True
        Me.gun_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.gun_cb.FlatAppearance.BorderSize = 2
        Me.gun_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.gun_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.gun_cb.ForeColor = System.Drawing.Color.White
        Me.gun_cb.Location = New System.Drawing.Point(2, 156)
        Me.gun_cb.Name = "gun_cb"
        Me.gun_cb.Size = New System.Drawing.Size(48, 48)
        Me.gun_cb.TabIndex = 5
        Me.gun_cb.UseVisualStyleBackColor = False
        '
        'turret_cb
        '
        Me.turret_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.turret_cb.BackColor = System.Drawing.Color.Gray
        Me.turret_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.tower
        Me.turret_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.turret_cb.Checked = True
        Me.turret_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.turret_cb.FlatAppearance.BorderSize = 2
        Me.turret_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.turret_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.turret_cb.ForeColor = System.Drawing.Color.White
        Me.turret_cb.Location = New System.Drawing.Point(2, 105)
        Me.turret_cb.Name = "turret_cb"
        Me.turret_cb.Size = New System.Drawing.Size(48, 48)
        Me.turret_cb.TabIndex = 4
        Me.turret_cb.UseVisualStyleBackColor = False
        '
        'hull_cb
        '
        Me.hull_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.hull_cb.BackColor = System.Drawing.Color.Gray
        Me.hull_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.hull
        Me.hull_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.hull_cb.Checked = True
        Me.hull_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.hull_cb.FlatAppearance.BorderSize = 2
        Me.hull_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.hull_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.hull_cb.ForeColor = System.Drawing.Color.White
        Me.hull_cb.Location = New System.Drawing.Point(2, 54)
        Me.hull_cb.Name = "hull_cb"
        Me.hull_cb.Size = New System.Drawing.Size(48, 48)
        Me.hull_cb.TabIndex = 3
        Me.hull_cb.UseVisualStyleBackColor = False
        '
        'chassis_cb
        '
        Me.chassis_cb.Appearance = System.Windows.Forms.Appearance.Button
        Me.chassis_cb.BackColor = System.Drawing.Color.Gray
        Me.chassis_cb.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.chassis
        Me.chassis_cb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.chassis_cb.Checked = True
        Me.chassis_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chassis_cb.FlatAppearance.BorderSize = 2
        Me.chassis_cb.FlatAppearance.CheckedBackColor = System.Drawing.Color.Sienna
        Me.chassis_cb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.chassis_cb.ForeColor = System.Drawing.Color.White
        Me.chassis_cb.Location = New System.Drawing.Point(2, 3)
        Me.chassis_cb.Name = "chassis_cb"
        Me.chassis_cb.Size = New System.Drawing.Size(48, 48)
        Me.chassis_cb.TabIndex = 2
        Me.chassis_cb.UseVisualStyleBackColor = False
        '
        'intro_label
        '
        Me.intro_label.AutoSize = True
        Me.intro_label.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.intro_label.Font = New System.Drawing.Font("Segoe Print", 20.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.intro_label.ForeColor = System.Drawing.Color.Black
        Me.intro_label.Location = New System.Drawing.Point(25, 33)
        Me.intro_label.Name = "intro_label"
        Me.intro_label.Size = New System.Drawing.Size(296, 47)
        Me.intro_label.TabIndex = 6
        Me.intro_label.Text = "Welcome to version "
        '
        'decal_panel
        '
        Me.decal_panel.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.decal_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.decal_panel.Controls.Add(Me.copy_Decal_btn)
        Me.decal_panel.Controls.Add(Me.hide_BB_cb)
        Me.decal_panel.Controls.Add(Me.Label7)
        Me.decal_panel.Controls.Add(Me.decal_alpha_slider)
        Me.decal_panel.Controls.Add(Me.decal_level_slider)
        Me.decal_panel.Controls.Add(Me.Label6)
        Me.decal_panel.Controls.Add(Me.dgv)
        Me.decal_panel.Controls.Add(Me.uv_rotate)
        Me.decal_panel.Controls.Add(Me.save_decal_btn)
        Me.decal_panel.Controls.Add(Me.load_decal_btn)
        Me.decal_panel.Controls.Add(Me.track_decal_cb)
        Me.decal_panel.Controls.Add(Me.Label5)
        Me.decal_panel.Controls.Add(Me.Uwrap)
        Me.decal_panel.Controls.Add(Me.Vwrap)
        Me.decal_panel.Controls.Add(Me.Label2)
        Me.decal_panel.Controls.Add(Me.Label4)
        Me.decal_panel.Controls.Add(Me.Label3)
        Me.decal_panel.Controls.Add(Me.d_texture_name)
        Me.decal_panel.Controls.Add(Me.mouse_pick_cb)
        Me.decal_panel.Controls.Add(Me.d_move_down)
        Me.decal_panel.Controls.Add(Me.d_move_up)
        Me.decal_panel.Controls.Add(Me.m_sel_texture)
        Me.decal_panel.Controls.Add(Me.m_delete)
        Me.decal_panel.Controls.Add(Me.m_new)
        Me.decal_panel.Controls.Add(Me.current_decal_lable)
        Me.decal_panel.ForeColor = System.Drawing.Color.White
        Me.decal_panel.Location = New System.Drawing.Point(402, 77)
        Me.decal_panel.Name = "decal_panel"
        Me.decal_panel.Size = New System.Drawing.Size(257, 382)
        Me.decal_panel.TabIndex = 5
        '
        'copy_Decal_btn
        '
        Me.copy_Decal_btn.AutoSize = True
        Me.copy_Decal_btn.ForeColor = System.Drawing.Color.Black
        Me.copy_Decal_btn.Location = New System.Drawing.Point(150, 3)
        Me.copy_Decal_btn.Name = "copy_Decal_btn"
        Me.copy_Decal_btn.Size = New System.Drawing.Size(41, 23)
        Me.copy_Decal_btn.TabIndex = 26
        Me.copy_Decal_btn.Text = "Copy"
        Me.copy_Decal_btn.UseVisualStyleBackColor = True
        '
        'hide_BB_cb
        '
        Me.hide_BB_cb.AutoSize = True
        Me.hide_BB_cb.Location = New System.Drawing.Point(5, 99)
        Me.hide_BB_cb.Name = "hide_BB_cb"
        Me.hide_BB_cb.Size = New System.Drawing.Size(70, 17)
        Me.hide_BB_cb.TabIndex = 25
        Me.hide_BB_cb.Text = "Hide BBs"
        Me.hide_BB_cb.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(118, 33)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(33, 13)
        Me.Label7.TabIndex = 24
        Me.Label7.Text = "Level"
        '
        'decal_alpha_slider
        '
        Me.decal_alpha_slider.AutoSize = False
        Me.decal_alpha_slider.Location = New System.Drawing.Point(32, 30)
        Me.decal_alpha_slider.Maximum = 100
        Me.decal_alpha_slider.Name = "decal_alpha_slider"
        Me.decal_alpha_slider.Size = New System.Drawing.Size(86, 24)
        Me.decal_alpha_slider.TabIndex = 7
        Me.decal_alpha_slider.TickFrequency = 0
        Me.decal_alpha_slider.TickStyle = System.Windows.Forms.TickStyle.None
        Me.decal_alpha_slider.Value = 100
        '
        'decal_level_slider
        '
        Me.decal_level_slider.AutoSize = False
        Me.decal_level_slider.Location = New System.Drawing.Point(147, 30)
        Me.decal_level_slider.Maximum = 100
        Me.decal_level_slider.Name = "decal_level_slider"
        Me.decal_level_slider.Size = New System.Drawing.Size(86, 24)
        Me.decal_level_slider.TabIndex = 23
        Me.decal_level_slider.TickFrequency = 0
        Me.decal_level_slider.TickStyle = System.Windows.Forms.TickStyle.None
        Me.decal_level_slider.Value = 100
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(170, 53)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(57, 13)
        Me.Label6.TabIndex = 22
        Me.Label6.Text = "UV Rotate"
        '
        'dgv
        '
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.dgv.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.dgv.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.dgv.BackgroundColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.White
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgv.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.decalName, Me.decalID})
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        DataGridViewCellStyle5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle5.ForeColor = System.Drawing.Color.White
        DataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        DataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer), CType(CType(0, Byte), Integer))
        DataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.dgv.DefaultCellStyle = DataGridViewCellStyle5
        Me.dgv.GridColor = System.Drawing.Color.Black
        Me.dgv.Location = New System.Drawing.Point(-2, 139)
        Me.dgv.MultiSelect = False
        Me.dgv.Name = "dgv"
        Me.dgv.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.[Single]
        Me.dgv.RowHeadersVisible = False
        Me.dgv.RowHeadersWidth = 20
        Me.dgv.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.dgv.Size = New System.Drawing.Size(257, 241)
        Me.dgv.TabIndex = 27
        '
        'decalName
        '
        Me.decalName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.Color.White
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.decalName.DefaultCellStyle = DataGridViewCellStyle3
        Me.decalName.HeaderText = "Decal Name"
        Me.decalName.Name = "decalName"
        Me.decalName.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.decalName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.decalName.Width = 170
        '
        'decalID
        '
        Me.decalID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.Color.White
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White
        Me.decalID.DefaultCellStyle = DataGridViewCellStyle4
        Me.decalID.HeaderText = "Decal ID"
        Me.decalID.Name = "decalID"
        Me.decalID.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.decalID.Width = 60
        '
        'uv_rotate
        '
        Me.uv_rotate.Items.Add("-360")
        Me.uv_rotate.Items.Add("-270")
        Me.uv_rotate.Items.Add("-180")
        Me.uv_rotate.Items.Add("-90")
        Me.uv_rotate.Items.Add("0.0")
        Me.uv_rotate.Items.Add("90")
        Me.uv_rotate.Items.Add("180")
        Me.uv_rotate.Items.Add("270")
        Me.uv_rotate.Items.Add("360")
        Me.uv_rotate.Location = New System.Drawing.Point(173, 69)
        Me.uv_rotate.Name = "uv_rotate"
        Me.uv_rotate.Size = New System.Drawing.Size(60, 20)
        Me.uv_rotate.TabIndex = 21
        Me.uv_rotate.Text = "0.0"
        '
        'save_decal_btn
        '
        Me.save_decal_btn.AutoSize = True
        Me.save_decal_btn.ForeColor = System.Drawing.Color.Black
        Me.save_decal_btn.Location = New System.Drawing.Point(190, 91)
        Me.save_decal_btn.Name = "save_decal_btn"
        Me.save_decal_btn.Size = New System.Drawing.Size(43, 23)
        Me.save_decal_btn.TabIndex = 20
        Me.save_decal_btn.Text = "Save"
        Me.save_decal_btn.UseVisualStyleBackColor = True
        '
        'load_decal_btn
        '
        Me.load_decal_btn.ForeColor = System.Drawing.Color.Black
        Me.load_decal_btn.Location = New System.Drawing.Point(190, 113)
        Me.load_decal_btn.Name = "load_decal_btn"
        Me.load_decal_btn.Size = New System.Drawing.Size(43, 23)
        Me.load_decal_btn.TabIndex = 19
        Me.load_decal_btn.Text = "Load"
        Me.load_decal_btn.UseVisualStyleBackColor = True
        '
        'track_decal_cb
        '
        Me.track_decal_cb.AutoSize = True
        Me.track_decal_cb.Location = New System.Drawing.Point(5, 76)
        Me.track_decal_cb.Name = "track_decal_cb"
        Me.track_decal_cb.Size = New System.Drawing.Size(85, 17)
        Me.track_decal_cb.TabIndex = 18
        Me.track_decal_cb.Text = "Track Decal"
        Me.track_decal_cb.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(102, 100)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(14, 13)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "V"
        '
        'Uwrap
        '
        Me.Uwrap.Items.Add("-4")
        Me.Uwrap.Items.Add("-3")
        Me.Uwrap.Items.Add("-2")
        Me.Uwrap.Items.Add("-1")
        Me.Uwrap.Items.Add("1")
        Me.Uwrap.Items.Add("2")
        Me.Uwrap.Items.Add("3")
        Me.Uwrap.Items.Add("4")
        Me.Uwrap.Location = New System.Drawing.Point(119, 69)
        Me.Uwrap.Name = "Uwrap"
        Me.Uwrap.Size = New System.Drawing.Size(35, 20)
        Me.Uwrap.TabIndex = 12
        Me.Uwrap.Text = "1"
        '
        'Vwrap
        '
        Me.Vwrap.Items.Add("-4")
        Me.Vwrap.Items.Add("-3")
        Me.Vwrap.Items.Add("-2")
        Me.Vwrap.Items.Add("-1")
        Me.Vwrap.Items.Add("1")
        Me.Vwrap.Items.Add("2")
        Me.Vwrap.Items.Add("3")
        Me.Vwrap.Items.Add("4")
        Me.Vwrap.Location = New System.Drawing.Point(119, 95)
        Me.Vwrap.Name = "Vwrap"
        Me.Vwrap.Size = New System.Drawing.Size(35, 20)
        Me.Vwrap.TabIndex = 13
        Me.Vwrap.Text = "1"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(2, 33)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(34, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "Alpha"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(101, 74)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(15, 13)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "U"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(101, 53)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(60, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "UV Repeat"
        '
        'd_texture_name
        '
        Me.d_texture_name.ForeColor = System.Drawing.Color.Yellow
        Me.d_texture_name.Location = New System.Drawing.Point(5, 123)
        Me.d_texture_name.Name = "d_texture_name"
        Me.d_texture_name.Size = New System.Drawing.Size(155, 13)
        Me.d_texture_name.TabIndex = 9
        Me.d_texture_name.Text = "_____________________________"
        '
        'mouse_pick_cb
        '
        Me.mouse_pick_cb.AutoSize = True
        Me.mouse_pick_cb.Location = New System.Drawing.Point(5, 53)
        Me.mouse_pick_cb.Name = "mouse_pick_cb"
        Me.mouse_pick_cb.Size = New System.Drawing.Size(82, 17)
        Me.mouse_pick_cb.TabIndex = 6
        Me.mouse_pick_cb.Text = "Mouse Pick"
        Me.mouse_pick_cb.UseVisualStyleBackColor = True
        '
        'd_move_down
        '
        Me.d_move_down.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.d_move_down.BackColor = System.Drawing.Color.Gray
        Me.d_move_down.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.d_move_down.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.d_move_down.Image = Global.Tank_Exporter.My.Resources.Resources.control_270
        Me.d_move_down.Location = New System.Drawing.Point(168, 114)
        Me.d_move_down.Name = "d_move_down"
        Me.d_move_down.Size = New System.Drawing.Size(20, 20)
        Me.d_move_down.TabIndex = 5
        Me.d_move_down.UseVisualStyleBackColor = False
        '
        'd_move_up
        '
        Me.d_move_up.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.d_move_up.BackColor = System.Drawing.Color.Gray
        Me.d_move_up.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.d_move_up.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.d_move_up.Image = Global.Tank_Exporter.My.Resources.Resources.control_090
        Me.d_move_up.Location = New System.Drawing.Point(168, 92)
        Me.d_move_up.Name = "d_move_up"
        Me.d_move_up.Size = New System.Drawing.Size(20, 20)
        Me.d_move_up.TabIndex = 4
        Me.d_move_up.UseVisualStyleBackColor = False
        '
        'm_sel_texture
        '
        Me.m_sel_texture.AutoSize = True
        Me.m_sel_texture.ForeColor = System.Drawing.Color.Black
        Me.m_sel_texture.Location = New System.Drawing.Point(89, 3)
        Me.m_sel_texture.Name = "m_sel_texture"
        Me.m_sel_texture.Size = New System.Drawing.Size(60, 23)
        Me.m_sel_texture.TabIndex = 3
        Me.m_sel_texture.Text = "Texture"
        Me.m_sel_texture.UseVisualStyleBackColor = True
        '
        'm_delete
        '
        Me.m_delete.ForeColor = System.Drawing.Color.Black
        Me.m_delete.Location = New System.Drawing.Point(192, 3)
        Me.m_delete.Name = "m_delete"
        Me.m_delete.Size = New System.Drawing.Size(48, 23)
        Me.m_delete.TabIndex = 2
        Me.m_delete.Text = "Delete"
        Me.m_delete.UseVisualStyleBackColor = True
        '
        'm_new
        '
        Me.m_new.AutoSize = True
        Me.m_new.ForeColor = System.Drawing.Color.Black
        Me.m_new.Location = New System.Drawing.Point(45, 3)
        Me.m_new.Name = "m_new"
        Me.m_new.Size = New System.Drawing.Size(43, 23)
        Me.m_new.TabIndex = 1
        Me.m_new.Text = "New"
        Me.m_new.UseVisualStyleBackColor = True
        '
        'current_decal_lable
        '
        Me.current_decal_lable.AutoSize = True
        Me.current_decal_lable.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.current_decal_lable.ForeColor = System.Drawing.Color.Yellow
        Me.current_decal_lable.Location = New System.Drawing.Point(0, 3)
        Me.current_decal_lable.Margin = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.current_decal_lable.Name = "current_decal_lable"
        Me.current_decal_lable.Size = New System.Drawing.Size(20, 24)
        Me.current_decal_lable.TabIndex = 17
        Me.current_decal_lable.Text = "_"
        Me.current_decal_lable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'PB3
        '
        Me.PB3.Location = New System.Drawing.Point(33, 69)
        Me.PB3.Name = "PB3"
        Me.PB3.Size = New System.Drawing.Size(100, 50)
        Me.PB3.TabIndex = 4
        Me.PB3.TabStop = False
        Me.PB3.Visible = False
        '
        'font_holder
        '
        Me.font_holder.AutoSize = True
        Me.font_holder.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.font_holder.ForeColor = System.Drawing.Color.White
        Me.font_holder.Location = New System.Drawing.Point(108, 405)
        Me.font_holder.Name = "font_holder"
        Me.font_holder.Size = New System.Drawing.Size(111, 13)
        Me.font_holder.TabIndex = 1
        Me.font_holder.Text = "For font only"
        Me.font_holder.Visible = False
        '
        'pb2
        '
        Me.pb2.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.pb2.Location = New System.Drawing.Point(70, 137)
        Me.pb2.Name = "pb2"
        Me.pb2.Size = New System.Drawing.Size(200, 100)
        Me.pb2.TabIndex = 3
        Me.pb2.Visible = False
        '
        'pb1
        '
        Me.pb1.BackColor = System.Drawing.Color.Black
        Me.pb1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.pb1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pb1.InitialImage = Nothing
        Me.pb1.Location = New System.Drawing.Point(0, 23)
        Me.pb1.Name = "pb1"
        Me.pb1.Size = New System.Drawing.Size(881, 479)
        Me.pb1.TabIndex = 0
        Me.pb1.TabStop = False
        '
        'info_Label
        '
        Me.info_Label.BackColor = System.Drawing.Color.Black
        Me.info_Label.Dock = System.Windows.Forms.DockStyle.Top
        Me.info_Label.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.info_Label.ForeColor = System.Drawing.Color.Silver
        Me.info_Label.Location = New System.Drawing.Point(0, 0)
        Me.info_Label.Name = "info_Label"
        Me.info_Label.Size = New System.Drawing.Size(881, 23)
        Me.info_Label.TabIndex = 1
        Me.info_Label.Text = "Label1"
        '
        'SplitContainer2
        '
        Me.SplitContainer2.BackColor = System.Drawing.SystemColors.Menu
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer2.IsSplitterFixed = True
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.SearchBox)
        Me.SplitContainer2.Panel1.Controls.Add(Me.TC1)
        Me.SplitContainer2.Panel1.Controls.Add(Me.TC2)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.SplitContainer2.Panel2.Controls.Add(Me.tank_label)
        Me.SplitContainer2.Panel2.Controls.Add(Me.iconbox)
        Me.SplitContainer2.Size = New System.Drawing.Size(25, 506)
        Me.SplitContainer2.SplitterDistance = 480
        Me.SplitContainer2.SplitterWidth = 1
        Me.SplitContainer2.TabIndex = 1
        '
        'SearchBox
        '
        Me.SearchBox.Dock = System.Windows.Forms.DockStyle.Top
        Me.SearchBox.Location = New System.Drawing.Point(0, 0)
        Me.SearchBox.Multiline = True
        Me.SearchBox.Name = "SearchBox"
        Me.SearchBox.Size = New System.Drawing.Size(25, 20)
        Me.SearchBox.TabIndex = 6
        Me.SearchBox.Text = "Type Your Tank"
        Me.SearchBox.WordWrap = False
        '
        'TC1
        '
        Me.TC1.Controls.Add(Me.TabPage1)
        Me.TC1.Controls.Add(Me.TabPage2)
        Me.TC1.Controls.Add(Me.TabPage3)
        Me.TC1.Controls.Add(Me.TabPage4)
        Me.TC1.Controls.Add(Me.TabPage5)
        Me.TC1.Controls.Add(Me.TabPage6)
        Me.TC1.Controls.Add(Me.TabPage7)
        Me.TC1.Controls.Add(Me.TabPage8)
        Me.TC1.Controls.Add(Me.TabPage9)
        Me.TC1.Controls.Add(Me.TabPage10)
        Me.TC1.ItemSize = New System.Drawing.Size(24, 21)
        Me.TC1.Location = New System.Drawing.Point(0, 23)
        Me.TC1.Name = "TC1"
        Me.TC1.SelectedIndex = 0
        Me.TC1.Size = New System.Drawing.Size(22, 400)
        Me.TC1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TC1.TabIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.Color.DimGray
        Me.TabPage1.Location = New System.Drawing.Point(4, 25)
        Me.TabPage1.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage1.Size = New System.Drawing.Size(14, 371)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "1"
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.Color.DimGray
        Me.TabPage2.Location = New System.Drawing.Point(4, 25)
        Me.TabPage2.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage2.Size = New System.Drawing.Size(14, 371)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "2"
        '
        'TabPage3
        '
        Me.TabPage3.BackColor = System.Drawing.Color.DimGray
        Me.TabPage3.Location = New System.Drawing.Point(4, 25)
        Me.TabPage3.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage3.Size = New System.Drawing.Size(14, 371)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "3"
        '
        'TabPage4
        '
        Me.TabPage4.BackColor = System.Drawing.Color.DimGray
        Me.TabPage4.Location = New System.Drawing.Point(4, 25)
        Me.TabPage4.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage4.Size = New System.Drawing.Size(14, 371)
        Me.TabPage4.TabIndex = 3
        Me.TabPage4.Text = "4"
        '
        'TabPage5
        '
        Me.TabPage5.BackColor = System.Drawing.Color.DimGray
        Me.TabPage5.Location = New System.Drawing.Point(4, 25)
        Me.TabPage5.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage5.Name = "TabPage5"
        Me.TabPage5.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage5.Size = New System.Drawing.Size(14, 371)
        Me.TabPage5.TabIndex = 4
        Me.TabPage5.Text = "5"
        '
        'TabPage6
        '
        Me.TabPage6.BackColor = System.Drawing.Color.DimGray
        Me.TabPage6.Location = New System.Drawing.Point(4, 25)
        Me.TabPage6.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage6.Size = New System.Drawing.Size(14, 371)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "6"
        '
        'TabPage7
        '
        Me.TabPage7.BackColor = System.Drawing.Color.DimGray
        Me.TabPage7.Location = New System.Drawing.Point(4, 25)
        Me.TabPage7.Name = "TabPage7"
        Me.TabPage7.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage7.Size = New System.Drawing.Size(14, 371)
        Me.TabPage7.TabIndex = 6
        Me.TabPage7.Text = "7"
        '
        'TabPage8
        '
        Me.TabPage8.BackColor = System.Drawing.Color.DimGray
        Me.TabPage8.Location = New System.Drawing.Point(4, 25)
        Me.TabPage8.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage8.Name = "TabPage8"
        Me.TabPage8.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage8.Size = New System.Drawing.Size(14, 371)
        Me.TabPage8.TabIndex = 7
        Me.TabPage8.Text = "8"
        '
        'TabPage9
        '
        Me.TabPage9.BackColor = System.Drawing.Color.DimGray
        Me.TabPage9.Location = New System.Drawing.Point(4, 25)
        Me.TabPage9.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage9.Name = "TabPage9"
        Me.TabPage9.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage9.Size = New System.Drawing.Size(14, 371)
        Me.TabPage9.TabIndex = 8
        Me.TabPage9.Text = "9"
        '
        'TabPage10
        '
        Me.TabPage10.BackColor = System.Drawing.Color.DimGray
        Me.TabPage10.Location = New System.Drawing.Point(4, 25)
        Me.TabPage10.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage10.Name = "TabPage10"
        Me.TabPage10.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage10.Size = New System.Drawing.Size(14, 371)
        Me.TabPage10.TabIndex = 9
        Me.TabPage10.Text = "10"
        '
        'TC2
        '
        Me.TC2.Controls.Add(Me.TabPage11)
        Me.TC2.ItemSize = New System.Drawing.Size(24, 21)
        Me.TC2.Location = New System.Drawing.Point(0, 23)
        Me.TC2.Name = "TC2"
        Me.TC2.SelectedIndex = 0
        Me.TC2.Size = New System.Drawing.Size(22, 480)
        Me.TC2.TabIndex = 0
        Me.TC2.Visible = False
        '
        'TabPage11
        '
        Me.TabPage11.BackColor = System.Drawing.Color.DimGray
        Me.TabPage11.Location = New System.Drawing.Point(4, 25)
        Me.TabPage11.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPage11.Name = "TabPage11"
        Me.TabPage11.Padding = New System.Windows.Forms.Padding(3, 3, 0, 0)
        Me.TabPage11.Size = New System.Drawing.Size(14, 451)
        Me.TabPage11.TabIndex = 0
        Me.TabPage11.Text = "Result"
        '
        'tank_label
        '
        Me.tank_label.AutoSize = True
        Me.tank_label.Dock = System.Windows.Forms.DockStyle.Left
        Me.tank_label.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tank_label.ForeColor = System.Drawing.Color.White
        Me.tank_label.Location = New System.Drawing.Point(0, 0)
        Me.tank_label.Name = "tank_label"
        Me.tank_label.Size = New System.Drawing.Size(48, 16)
        Me.tank_label.TabIndex = 3
        Me.tank_label.Text = "Label1"
        '
        'iconbox
        '
        Me.iconbox.BackColor = System.Drawing.Color.Transparent
        Me.iconbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.iconbox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.iconbox.Location = New System.Drawing.Point(0, 0)
        Me.iconbox.Name = "iconbox"
        Me.iconbox.Size = New System.Drawing.Size(25, 25)
        Me.iconbox.TabIndex = 2
        Me.iconbox.TabStop = False
        '
        'conMenu
        '
        Me.conMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_load, Me.ToolStripSeparator13, Me.m_load_crashed, Me.ToolStripSeparator16, Me.m_reload_textures, Me.ToolStripSeparator30, Me.ToolStripMenuItem1})
        Me.conMenu.Name = "conMenu"
        Me.conMenu.Size = New System.Drawing.Size(167, 110)
        '
        'm_load
        '
        Me.m_load.Name = "m_load"
        Me.m_load.Size = New System.Drawing.Size(166, 22)
        Me.m_load.Text = "Load This.."
        '
        'ToolStripSeparator13
        '
        Me.ToolStripSeparator13.Name = "ToolStripSeparator13"
        Me.ToolStripSeparator13.Size = New System.Drawing.Size(163, 6)
        '
        'm_load_crashed
        '
        Me.m_load_crashed.Name = "m_load_crashed"
        Me.m_load_crashed.Size = New System.Drawing.Size(166, 22)
        Me.m_load_crashed.Text = "Load Crashed..."
        '
        'ToolStripSeparator16
        '
        Me.ToolStripSeparator16.Name = "ToolStripSeparator16"
        Me.ToolStripSeparator16.Size = New System.Drawing.Size(163, 6)
        '
        'm_reload_textures
        '
        Me.m_reload_textures.Name = "m_reload_textures"
        Me.m_reload_textures.Size = New System.Drawing.Size(166, 22)
        Me.m_reload_textures.Text = "Reload Textures"
        '
        'ToolStripSeparator30
        '
        Me.ToolStripSeparator30.Name = "ToolStripSeparator30"
        Me.ToolStripSeparator30.Size = New System.Drawing.Size(163, 6)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(166, 22)
        Me.ToolStripMenuItem1.Text = "Tanks Description"
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'decal_ds
        '
        Me.decal_ds.DataSetName = "NewDataSet"
        Me.decal_ds.Tables.AddRange(New System.Data.DataTable() {Me.d_table})
        '
        'd_table
        '
        Me.d_table.Columns.AddRange(New System.Data.DataColumn() {Me.DataColumn1, Me.DataColumn2})
        Me.d_table.TableName = "Table1"
        '
        'DataColumn1
        '
        Me.DataColumn1.ColumnName = "Decal Name"
        '
        'DataColumn2
        '
        Me.DataColumn2.ColumnName = "Decal ID"
        Me.DataColumn2.DataType = GetType(Integer)
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(968, 533)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MM)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MM
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Tank Exporter"
        Me.MM.ResumeLayout(False)
        Me.MM.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.Panel2.PerformLayout()
        CType(Me.SplitContainer3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.ResumeLayout(False)
        Me.decal_panel.ResumeLayout(False)
        Me.decal_panel.PerformLayout()
        CType(Me.decal_alpha_slider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.decal_level_slider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgv, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PB3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pb1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.PerformLayout()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.Panel2.PerformLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.TC1.ResumeLayout(False)
        Me.TC2.ResumeLayout(False)
        CType(Me.iconbox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.conMenu.ResumeLayout(False)
        CType(Me.decal_ds, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.d_table, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents pb1 As System.Windows.Forms.PictureBox
    Friend WithEvents Startup_Timer As System.Windows.Forms.Timer
    Friend WithEvents MM As System.Windows.Forms.MenuStrip
    Friend WithEvents m_file As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents M_Exit As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents M_Path As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents TC1 As System.Windows.Forms.TabControl
    Friend WithEvents TC2 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage5 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage7 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage8 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage9 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage10 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage11 As System.Windows.Forms.TabPage
    Friend WithEvents iconbox As System.Windows.Forms.PictureBox
    Friend WithEvents info_Label As System.Windows.Forms.Label
    Friend WithEvents conMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents m_load As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents font_holder As System.Windows.Forms.Label
    Friend WithEvents m_clear_temp_folder_data As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_reload_api_data As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_file As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_save As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents tank_label As System.Windows.Forms.Label
    Friend WithEvents m_Open_game_folder As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents m_load_textures As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator5 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_show_log As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_res_mods_path As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_pick_camo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_edit_shaders As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_lighting As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_help As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator7 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents m_remove_fbx As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_fbx As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator9 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents chassis_cb As System.Windows.Forms.CheckBox
    Friend WithEvents hull_cb As System.Windows.Forms.CheckBox
    Friend WithEvents turret_cb As System.Windows.Forms.CheckBox
    Friend WithEvents gun_cb As System.Windows.Forms.CheckBox
    Friend WithEvents grid_cb As System.Windows.Forms.CheckBox
    Friend WithEvents wire_cb As System.Windows.Forms.CheckBox
    Friend WithEvents show_textures_cb As System.Windows.Forms.CheckBox
    Friend WithEvents pb2 As System.Windows.Forms.Panel
    Friend WithEvents m_edit_visual As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_write_primitive As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_model_info As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_simple_lighting As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator10 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator11 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator12 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_Shader_Debug As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_show_environment As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PB3 As System.Windows.Forms.PictureBox
    Friend WithEvents m_shadow_preview As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_shadows As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_shadowQuality As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_select_light As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_decal As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents decal_panel As System.Windows.Forms.Panel
    Friend WithEvents m_delete As System.Windows.Forms.Button
    Friend WithEvents m_new As System.Windows.Forms.Button
    Friend WithEvents m_sel_texture As System.Windows.Forms.Button
    Friend WithEvents d_move_down As System.Windows.Forms.Button
    Friend WithEvents d_move_up As System.Windows.Forms.Button
    Friend WithEvents mouse_pick_cb As System.Windows.Forms.CheckBox
    Friend WithEvents decal_alpha_slider As System.Windows.Forms.TrackBar
    Friend WithEvents d_texture_name As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Uwrap As System.Windows.Forms.DomainUpDown
    Friend WithEvents Vwrap As System.Windows.Forms.DomainUpDown
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents current_decal_lable As System.Windows.Forms.Label
    Friend WithEvents track_decal_cb As System.Windows.Forms.CheckBox
    Friend WithEvents save_decal_btn As System.Windows.Forms.Button
    Friend WithEvents load_decal_btn As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents uv_rotate As System.Windows.Forms.DomainUpDown
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents decal_level_slider As System.Windows.Forms.TrackBar
    Friend WithEvents hide_BB_cb As System.Windows.Forms.CheckBox
    Friend WithEvents copy_Decal_btn As System.Windows.Forms.Button
    Friend WithEvents m_reload_textures As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator13 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_ExportExtract As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_extract As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_edit_camo As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents VertexColor_cb As System.Windows.Forms.CheckBox
    Friend WithEvents ToolStripSeparator6 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_clean_res_mods As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_donate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_FXAA As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator14 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_enableBloom As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_bloom_off As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator15 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_view_res_mods_folder As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_crashed As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator16 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator17 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_screen_cap As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator18 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_build_wotmod As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_GMM_toy_cb As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_settings As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator19 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_test As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_test_res_mods As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator21 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_pythonLog As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator22 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_clear_PythonLog As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_UI_settings As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator23 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator24 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator25 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_hide_show_components As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator26 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator27 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_set_vertex_winding_order As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_load_primitive As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator28 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_import_primitives_fbx As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator8 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripSeparator29 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_write_non_tank_primitive As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_tangent_normalMaps As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator30 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator31 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents m_dump_tanks As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_enable_tarrain_decals As ToolStripMenuItem
    Friend WithEvents m_region_combo As ToolStripComboBox

    Friend WithEvents SearchBox As TextBox
    Friend WithEvents ToolStripSeparator32 As ToolStripSeparator
    Friend WithEvents m_export_to_glTF As ToolStripMenuItem
    Friend WithEvents m_export_to_obj As ToolStripMenuItem
    Friend WithEvents m_forums As ToolStripMenuItem
    Friend WithEvents m_hide_right_plane As ToolStripMenuItem
    Friend WithEvents m_rebuild_XML As ToolStripMenuItem
    Friend WithEvents m_import_GLB As ToolStripMenuItem
    Friend WithEvents m_export_STL As ToolStripMenuItem
    Friend WithEvents m_import_2016_fbx As ToolStripMenuItem
    Friend WithEvents m_2013_fbx As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator33 As ToolStripSeparator
    Friend WithEvents dgv As DataGridView
    Friend WithEvents decal_ds As DataSet
    Friend WithEvents d_table As DataTable
    Friend WithEvents DataColumn1 As DataColumn
    Friend WithEvents DataColumn2 As DataColumn
    Friend WithEvents ToolStripSeparator34 As ToolStripSeparator
    Friend WithEvents m_open_wot_temp_folder As ToolStripMenuItem
    Friend WithEvents intro_label As Label
    Friend WithEvents decalName As DataGridViewTextBoxColumn
    Friend WithEvents decalID As DataGridViewTextBoxColumn
End Class
