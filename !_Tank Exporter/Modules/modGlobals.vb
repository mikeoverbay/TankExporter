Imports System.Text
Imports System.IO
Module modGlobals
    Public LOADING_FBX As Boolean = False
    '##################################
    Public TESTING As Boolean = False
    Public jogg As Boolean = False
    '##################################
    Public section_a, section_b As Integer
    '##################################
    Public g_decal_scale As New vect3
    Public g_decal_rotate As vect3
    Public g_decal_translate As vect3

    '##################################
    Public tank_center_X As Single
    Public tank_center_Y As Single
    Public tank_center_Z As Single

    '##################################
    Public S_level, A_level, T_level As Single
    Public selected_light As Integer = 0
    '##################################
    Public decal_path As String
    Public terrain_modelId As Integer
    Public terrain_textureId As Integer
    Public terrain_textureNormalId As Integer
    Public gradient_lookup_id As Integer
    Public terrain_noise_id As Integer
    Public dome_modelId As Integer
    Public dome_textureId As Integer
    '##################################
    Public move_cam_z, M_DOWN, move_mod, z_move As Boolean
    '##################################
    Public rot_limit_l, rot_limit_r As Single
    Public gun_limit_u, gun_limit_d As Single

    Public TankListTempFolder As String
    Public lookup(255) As Byte

    Public cube_draw_id As Integer
    Public cube_texture_id As Integer
    Public u_brdfLUT As Integer

    Public current_vertex As UInt32
    Public current_part As UInt32

    Public CENTER_SELECTION As Boolean
    Public old_h, old_w As Integer
    Public Zoom_Factor As Single = 1.0

    Public rect_location As New Point
    Public rect_size As New Point
    Public checkerboard_id As Integer
    Public current_image As Integer
    Public current_tank_part As Integer
    Public white_id As Integer
    Public exclusionMask_sd As Integer
    Public GLOBAL_exclusionMask As Integer
    Public exclusionMask_name As String
    Public exclusionMask_id As Integer
    Public HD_TANK As Boolean
    Public path_pointer1 As Integer = 0
    Public path_pointer2 As Integer = 0
    Public path_data1(0) As path_data_
    Public path_data2(0) As path_data_
    Public XML_Strings(5) As String
    Public m_position As Point
    Public w_changing As Boolean = False
    Public stop_updating As Boolean = False

    Public Structure path_data_
        Dim dist As Single
        Dim pos1 As SlimDX.Vector3
        Dim z1, y1 As Single
        Dim z2, y2 As Single
        Dim zc, yc As Single
        Dim r As Single
        Dim angle As Single
    End Structure

    Public track_info As track_info_
    Public Structure track_info_
        Public left_path1 As String
        Public left_path2 As String
        Public right_path1 As String
        Public right_path2 As String
        Public segment_length, segment_offset1, segment_offset2 As Single
        Public segment_count As Integer
        Public spline_list() As SlimDX.Vector3
        Public spline_length As Integer
    End Structure
    Public API_REGION As String = ""
    Public Z_Flipped As Boolean
    Public running As Single = 0
    Public track_length As Single = 0
    Public segment_length_adjusted As Single = 0
    Public FBX_OUT_PATH As String = ""
    Public FBX_OUT_NAME As String = ""
    Public Temp_Storage As String = ""
    Public CAMO_BUTTON_DOWN As Boolean
    Public SUMMER_ICON, WINTER_ICON, DESSERT_ICON As Integer
    Public NBUTT_norm, NBUTT_over, NBUTT_down As Integer
    Public CBUTT_norm, CBUTT_over, CBUTT_down, CBUTT_selected As Integer
    Public SELECTED_SEASON_BUTTON As Integer
    Public SELECTED_CAMO_BUTTON As Integer = -1
    Public LAST_SEASON As Integer
    Public CAMO_norm, CAMO_over, CAMO_down As Integer
    Public STOP_BUTTON_SCAN As Boolean
    Public TANK_NAME As String

    Public ARMORCOLOR As vect4

    Public OLD_WINDOW_HEIGHT As Integer
    Public WINDOW_HEIGHT_DELTA As Integer
    Public season_Buttons_VISIBLE As Boolean
    Public CAMO_BUTTONS_VISIBLE As Boolean
    Public CAMO_BUTTON_TYPE As Integer
    Public TEXTURE_BUTTONS_VISIBLE As Boolean
    Public CURRENT_DATA_SET As Integer

    Public turret_count As Integer
    Public hull_count As Integer
    Public largestAnsio As Single
    Public bad_tanks As New StringBuilder
    Public delete_image_start As Integer
    Dim chassis_primitive, chassis_visual As MemoryStream
    Dim hull_primitive, hull_visual As MemoryStream
    Dim turret_prmitive, turret_visual As MemoryStream
    Dim gun_primitive, gun_visual As MemoryStream
    Public current_tank_package As Integer
    Public custom_tables(10) As DataSet
    Public gun_tile() As vect4
    Public hull_tile() As vect4
    Public turret_tile() As vect4
    Public hull_tiling As vect4
    Public gun_tiling As vect4
    Public chassis_tiling As vect4
    Public turret_tiling As vect4
    Public bspTree(0) As bsptree_
    Public Structure bsptree_
        Public v As vect3
        Public f As Single
        Public n1, n2, n3, n4 As UInt32
        Public node2 As UInt32
    End Structure
    Public tanks() As tank_nation
    Public Structure tank_nation
        Public nation As String
        Public count As Integer
        Public tank_data() As tank_data
    End Structure
    Public Structure tank_data
        Public nation As String
        Public tier As Integer
        Public tankname As String
        Public tankpath As String
        Public hull As String
        Public chassis As String
        Public gun As String
        Public turret As String
        Public texture As String
    End Structure
    Public t_list As New List(Of String)
    Public alltanks As New StringBuilder
    'Public current_camo_selection As Integer
    Public warp_shader, bump_shader, sun_shader, corona_shader As Integer
    Public grid As Integer
    Public _Started As Boolean = False
    Public app_path As String
    Public U_Cam_X_angle, U_Cam_Y_angle, Cam_X_angle, Cam_Y_angle As Single
    Public look_point_x, look_point_y, look_point_z As Single
    Public U_look_point_x, U_look_point_y, U_look_point_z As Single
    Public angle_offset, u_View_Radius As Single
    Public view_radius As Single
    Public cam_x, cam_y, cam_z As Single
    Public eyeX, eyeY, eyeZ As Single
    Public screen_avg_counter, screen_avg_draw_time, screen_draw_time, screen_totaled_draw_time As Double
    Public pause As Boolean = False
    Public track_mars As Boolean
    Public frmState As Integer
    Public gl_busy As Boolean = False
    Public current_png_path As String = ""
    Public Structure vect3Norm
        Public nx As Single
        Public ny As Single
        Public nz As Single
    End Structure
    Public Structure vect3
        Public x, y, z As Single
    End Structure
    Public Structure vect4
        Public x, y, z, w As Single
    End Structure
    Public Structure vec2
        Public x, y As Single
    End Structure
    Public Structure _indice
        Public a, b, c As Integer
    End Structure
    Public camo_images() As camo_
    Public Structure camo_
        Public bmp As Bitmap
        Public id As Integer
    End Structure

    Public tracks() As track_
    Public Structure track_
        Dim matrix() As Single
        Dim name As String
        Dim id As String
        Public position As SlimDX.Vector3
    End Structure

    Public tier_list(1) As tnk_list

    Public Structure tnk_list
        Public tag As String
        Public username As String
        Public tier As String
        Public nation As String
        Public type As String
    End Structure

End Module
