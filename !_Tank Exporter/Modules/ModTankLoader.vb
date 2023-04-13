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
Imports Skill.FbxSDK
#End Region


Module ModTankLoader
#Region "variables"

    Public section_names(30) As names_
    Public Structure names_
        Public names() As String
    End Structure
    Public uv2_data() As uv_
    Public uv2_data_fbx() As Single
    Dim has_uv2, has_color As Boolean
    Dim xmlget_mode, entry_count As Integer
    Public log_text As New StringBuilder
    Public start_up_log As New StringBuilder
    Public x_max As Single = -10000
    Public x_min As Single = 10000
    Public y_min As Single = -10000
    Public y_max As Single = 10000
    Public z_max As Single = -10000
    Public z_min As Single = 10000
    Public obj_start, master_cnt, object_start, stride, ind_scale, object_count As Integer
    Public file_name As String = "vehicles/american/A01_T1_Cunningham/"
    Public current_tank_name As String = file_name
    Public short_tank_name As String = ""
    Public bsp_data() As Byte
    Public color_data() As Byte
    Dim color_rgb() As vColor_
    Public color_size As Integer
    Public bsp_materials_data() As Byte
    Public save_has_uv2, MODEL_LOADED, IGNORE_TEXTURES As Boolean
    Public loaded_from_resmods As Boolean
    Dim ih As IndexHeader
    Dim vh As VerticesHeader
    'Dim temo_text_string As String = ""
    Public turret_trans, tank_location, hull_trans As vect3
    Public turret_location, gun_trans2, gun_trans, gun_location As vect3

    Public part_counts As part_counts_
    Public Structure part_counts_
        Public chassis_cnt As Integer
        Public hull_cnt As Integer
        Public turret_cnt As Integer
        Public gun_cnt As Integer
    End Structure

    Public Structure vertex_type
        Dim x, y, z, nx, ny, nz, u, v As Single
    End Structure
    Private Structure item_list_
        Public section_name_id As Integer
        Public identifier() As String
        Public vert_name As String
        Public vert_data() As Byte
        Public indi_name As String
        Public indi_data() As Byte
        Public uv2_name As String
        Public uv2_data() As Byte
        Public color_name As String
        Public color_data() As Byte
        Public color_count As Integer
        Public bsp2_data() As Byte
        Public bsp2_material_data() As Byte
        Public index As Integer
        Public has_color As Boolean
        Public has_bsp2 As Boolean
        Public has_uv2 As Boolean
        Public uv2_cnt As Integer
        Public has_bsp_material As Boolean
    End Structure
    Public Class vertice_
        Public found As Boolean
        Public x, y, z As Single
        Public n As UInt32
        Public u, v As Single
        Public index_1 As Byte
        Public index_2 As Byte
        Public index_3 As Byte
        Public index_4 As Byte
        Public weight_1 As Byte
        Public weight_2 As Byte
        Public weight_3 As Byte
        Public weight_4 As Byte
        Public nx, ny, nz As Single
        Public tx, ty, tz As Single
        Public bnx, bny, bnz As Single
        Public t As UInt32
        Public bn As UInt32
        Public u2 As Single
        Public v2 As Single
        Public r, g, b, a As Byte
    End Class
    Structure primGroup
        Public startIndex_ As Long
        Public nPrimitives_ As Long
        Public startVertex_ As Long
        Public nVertices_ As Long
    End Structure
    Structure IndexHeader
        Public ind_h As String
        Public nIndices_ As UInt32
        Public nInd_groups As UShort
    End Structure
    Structure VerticesHeader
        Public header_text As String
        Public nVertice_count As UInt32
    End Structure

    Public Class triangle
        'vertices
        Public v1 As vect3
        Public v2 As vect3
        Public v3 As vect3
        'normals
        Public n1 As vect3
        Public n2 As vect3
        Public n3 As vect3
        Public color1 As vec3
        Public color2 As vec3
        Public color3 As vec3
        Public c1 As vect4
        Public c2 As vect4
        Public c3 As vect4

        'UVs
        Public uv1 As uv_
        Public uv2 As uv_
        Public uv3 As uv_
        Public uv1_2 As uv_
        Public uv2_2 As uv_
        Public uv3_2 As uv_


        Public bn As UInt32
        Public b1 As vect3
        Public b2 As vect3
        Public b3 As vect3
        Public t1 As vect3
        Public t2 As vect3
        Public t3 As vect3

        Public tan As UInt32
        Public found1 As Boolean = False
        Public found2 As Boolean = False
        Public found3 As Boolean = False
        Public excluded As Boolean = False
        Public id As UInt32
        Public uv2_id_1 As UInt32
        Public uv2_id_2 As UInt32
        Public uv2_id_3 As UInt32
        Public indi_id_1 As Integer
        Public indi_id_2 As Integer
        Public indi_id_3 As Integer
    End Class
    Public Class uv_
        Public u As Single
        Public v As Single

    End Class

    Public _object() As obj
    Public Class obj
        Public vertex_pick_list As Integer
        Public exclude_camo As Integer
        Public camo_tiling As vect4
        Public use_camo As Integer
        Public matrix(16) As Double
        Public tris() As triangle = {New triangle}
        Public name As String
        Public ID As UInt32
        Public ANM As Integer
        Public count As UInt32 = 0
        Public old_count As UInt32 = 0
        Public visible As Boolean = True
        Public selected As Boolean = False
        Public main_display_list As UInt32
        Public uv2_display_list As UInt32
        Public scale As New vect3
        Public translate As New vect3
        Public row0 As vect3
        Public row1 As vect3
        Public row2 As vect3
        Public row3 As vect3
        Public v2 As New vect3
        Public v3 As New vect3
        Public gun As Boolean = False
        Public center_x As New Single
        Public center_y As New Single
        Public center_z As New Single
        Public rot_x As New Single
        Public rot_y As New Single
        Public rot_z As New Single
        Public ix_min, ix_max, iy_min, iy_max, iz_min, iz_max As New Single
        Public m_x, m_y, m_z As New Single
        Public loc_x, loc_y, loc_z As New Single
        Public modified As New Boolean
        Public bb_max As vect3
        Public bb_min As vect3
        Public colorMap As String
        Public normalMap As String
        Public is_track As Integer

        Public Sub find_center()
            If Me.count = 0 Then Return
            Me.ix_max = -10000 : Me.ix_min = 10000
            Me.iy_max = -10000 : Me.iy_min = 10000
            Me.iz_max = -10000 : Me.iz_min = 10000

            For l As UInteger = 1 To count - 1
                'find x_max and x_min
                If Me.tris(l).v1.x > Me.ix_max Then Me.ix_max = Me.tris(l).v1.x
                If Me.tris(l).v2.x > Me.ix_max Then Me.ix_max = Me.tris(l).v2.x
                If Me.tris(l).v3.x > Me.ix_max Then Me.ix_max = Me.tris(l).v3.x
                If Me.tris(l).v1.x < Me.ix_min Then Me.ix_min = Me.tris(l).v1.x
                If Me.tris(l).v2.x < Me.ix_min Then Me.ix_min = Me.tris(l).v2.x
                If Me.tris(l).v3.x < Me.ix_min Then Me.ix_min = Me.tris(l).v3.x
                'find y_max and y_min
                If Me.tris(l).v1.y > Me.iy_max Then Me.iy_max = Me.tris(l).v1.y
                If Me.tris(l).v2.y > Me.iy_max Then Me.iy_max = Me.tris(l).v2.y
                If Me.tris(l).v3.y > Me.iy_max Then Me.iy_max = Me.tris(l).v3.y
                If Me.tris(l).v1.y < Me.iy_min Then Me.iy_min = Me.tris(l).v1.y
                If Me.tris(l).v2.y < Me.iy_min Then Me.iy_min = Me.tris(l).v2.y
                If Me.tris(l).v3.y < Me.iy_min Then Me.iy_min = Me.tris(l).v3.y
                'find z_maz and z_min
                If Me.tris(l).v1.z > Me.iz_max Then Me.iz_max = Me.tris(l).v1.z
                If Me.tris(l).v2.z > Me.iz_max Then Me.iz_max = Me.tris(l).v2.z
                If Me.tris(l).v3.z > Me.iz_max Then Me.iz_max = Me.tris(l).v3.z
                If Me.tris(l).v1.z < Me.iz_min Then Me.iz_min = Me.tris(l).v1.z
                If Me.tris(l).v2.z < Me.iz_min Then Me.iz_min = Me.tris(l).v2.z
                If Me.tris(l).v3.z < Me.iz_min Then Me.iz_min = Me.tris(l).v3.z

            Next
            If Me.ix_max > x_max Then x_max = Me.ix_max
            If Me.iy_max > y_max Then y_max = Me.iy_max
            If Me.iz_max > z_max Then z_max = Me.iz_max

            If Me.ix_min < x_min Then x_min = Me.ix_min
            If Me.iy_min < y_min Then y_min = Me.iy_min
            If Me.iz_min < z_min Then z_min = Me.iz_min

            Me.center_x = (Me.ix_max + Me.ix_min) / 2
            Me.center_y = (Me.iy_max + Me.iy_min) / 2
            Me.center_z = (Me.iz_max + Me.iz_min) / 2
            Me.loc_x = Me.center_x
            Me.loc_y = Me.center_y
            Me.loc_z = Me.center_z
            Return
        End Sub

        Public Sub update_x_loc()
            If Me.count = 0 Then Return
            For l As UInteger = 1 To Me.count
                Me.tris(l).v1.x += Me.m_x
                Me.tris(l).v2.x += Me.m_x
                Me.tris(l).v3.x += Me.m_x
            Next
            Return
        End Sub
        Public Sub update_y_loc()
            If Me.count = 0 Then Return
            For l As UInteger = 1 To Me.count
                Me.tris(l).v1.y += Me.m_y
                Me.tris(l).v2.y += Me.m_y
                Me.tris(l).v3.y += Me.m_y
            Next
            Return
        End Sub
        Public Sub update_z_loc()
            If Me.count = 0 Then Return
            For l As UInteger = 1 To Me.count
                Me.tris(l).v1.z += Me.m_z
                Me.tris(l).v2.z += Me.m_z
                Me.tris(l).v3.z += Me.m_z
            Next
            Return
        End Sub
    End Class
    Public _group() As _grps
    Public Structure _grps
        Public comp As comp_
        Public cPoints() As FbxVector4
        Public vertex_pick_list As Integer
        Public is_GAmap As Integer
        Public texture_count As Integer
        Public is_new_model As Boolean
        Public bsp2_id As Integer
        Public bsp2_tree_id As Integer
        Public texture_id As Integer
        Public name As String
        Public table_entry_name As String
        Public BPVT_mode As Boolean
        ' Public mfm_path As String
        Public matrix() As Double
        Public shadowMapID As Integer
        Public alphaRef As Integer
        Public alphaTest As Integer
        Public color_Id As Integer
        Public is_glass As Integer
        Public detail_Id As Integer
        Public color_name As String
        Public AM_in_res_mods As Boolean
        Public metalGMM_Id As Integer
        Public ANM As Integer
        Public metalGMM_name As String
        Public GMM_in_res_mods As Boolean
        Public colorIDmap As String
        Public detail_power As Single
        Public doubleSided As Boolean
        Public ao_name As String
        Public AO_in_res_mods As Boolean
        Public ao_id As Integer
        Public normal_Id As Integer
        Public specular_id As Integer
        Public normal_name As String
        Public ANM_in_res_mods As Boolean
        Public specular_name As String
        Public Spec_in_res_mods
        Public detail_name As String
        Public multi_textured As Boolean
        Public metal_textured As Boolean
        Public hasColorID As Integer
        Public bumped As Boolean
        Public blend_only As Boolean
        Public indicies() As uvect3
        Public vertices() As vertice_
        Public bsp2_data() As Byte
        Public color_data() As Byte
        Public bsp2_material_data() As Byte
        Public startVertex_ As UInt32
        Public startIndex_ As UInt32
        Public nVertices_ As UInt32
        Public nPrimitives_ As UInt32
        Public is_tank As Boolean
        Public tank_shift As vect3
        Public detail_tile As vec2
        Public has_uv2 As Integer
        Public use_uv2 As Integer
        Public has_color As Integer
        Public has_Vcolor As Integer
        Public header As String
        Public call_list As Integer
        Public rotation As Skill.FbxSDK.FbxVector4
        Public translation As Skill.FbxSDK.FbxVector4
        Public scale As Skill.FbxSDK.FbxVector4
        Public is_carraige As Boolean
        Public visible As Boolean
        Public component_visible As Boolean
        Public reverse_winding As Boolean
        Public tank_part As String

        Public skinned As Boolean

        Public is_atlas_type As Integer
        Public g_atlas_size As vect4
        Public g_atlas_indexs As vect4
        Public atlasNormalGlossSpec As String
        Public atlasMetallicAO As String
        Public atlasBlend As String
        Public atlasAlbedoHeight As String
        Public AM_Height_id As Integer
        Public GBMT_Height_id As Integer
        Public MAO_Height_id As Integer
        Public atlas_uv_coords As vect4
        Public g_dirtColor As vect4
        Public g_tile2Tint As vect4
        Public g_tile1Tint As vect4
        Public g_tile0Tint As vect4
        Public g_dirtParams As vect4

        Public is_detail_type As Integer
        Public g_detailMap As String
        Public g_detailMap_id As Integer
        Public g_detailInfluences As vect4
        Public g_detailRejectTiling As vect4

        Public dirtMap As String
        'Public ATLAS_AM_ID As Integer
        'Public ATLAS_GBMT_ID As Integer
        'Public ATLAS_MAO_ID As Integer
        Public AM_ID As Integer
        Public AM_ID_name As String
        Public GBMT_ID As Integer
        Public GBMT_ID_name As String
        Public MAO_ID As Integer
        Public MAO_ID_name As String
        Public ATLAS_BLEND_ID As Integer
        Public ATLAS_DIRT_ID As Integer
        Public use_normapMap As Integer
        Public x_repete As Single
        Public y_repete As Single

        Public AM_atlas As Integer
        Public GBMT_atlas As Integer
        Public MAO_atlas As Integer
        Public atlas_size As vec2
        Public image_size As vec2

        Public swizzle() As Integer

        Public fbx_texture_id As Integer
        Public fbx_N_texture_id As Integer
        Public rendered As Boolean
    End Structure
    Public Structure uvect3
        Public v1 As UInt32
        Public v2 As UInt32
        Public v3 As UInt32
    End Structure
    Public Structure compare_
        Public v1 As vect3
        Public v2 As vect3
        Public v3 As vect3
        Public color As vec3
    End Structure
    Dim ordered_names(1) As item_list_

#End Region
    Public Function search_shared_pkgs(ss As String) As ZipEntry
        For Each f In shared_pkg_search_list
            Using zipf As New ZipFile(f)
                For Each entry In zipf
                    If Not entry.IsDirectory Then
                        If entry.FileName.Contains("vehicles") Then

                            'Debug.WriteLine(entry.FileName)
                            If entry.FileName.ToLower = ss.ToLower Then
                                zipf.Dispose()
                                GC.Collect()
                                Return entry
                            End If
                        End If
                    End If
                Next
            End Using
        Next
        ss = ss.Replace("\", "/")
        Debug.WriteLine(ss)
        Using zipf As New ZipFile(Path.GetDirectoryName(shared_pkg_search_list(0)) + "\particles.pkg")
            For Each entry In zipf
                If Not entry.IsDirectory Then
                    If entry.FileName.Contains("vehicles") Then

                        'Debug.WriteLine(entry.FileName)
                        If entry.FileName.ToLower = ss.ToLower Then
                            zipf.Dispose()
                            GC.Collect()
                            Return entry
                        End If
                    End If
                End If
            Next
        End Using
        Return Nothing

    End Function
    Public Function build_primitive_data(_add As Boolean) As Boolean
        '------------
        Dim f_name_vertices, f_name_indices, f_name_uv2, f_name_color As String
        Dim tbuf() As vertice_

        Dim pg_flag As Boolean = False

        has_uv2 = False
        has_color = False

        Dim er As Integer
        er = Gl.glGetError
        Dim i As UInt32

        er = Gl.glGetError
        '------------
        Dim jj As UInt32
        Dim section_sizes(100) As UInt32
        Dim section_locations(100) As ULong
        Dim pGroups(1) As primGroup
        Dim File_len As ULong
        '----------------------------------------------------------------------------------- add or new?

        'clear min and maxes
        '----------------------------- add or new?
        If Not _add Then

            master_cnt = 0
            object_start = 1
        Else
            'master_cnt = Convert.ToUInt32(poly_count.Text)
        End If
        Dim dum As String = ""
        Dim ln_break As Integer = 0
        Dim f_ln_breaks As Integer = 0
        Dim cnt As UInt32 = 0
        Dim big_l As UInt32
        Dim na As String = ""

        '

        xmlget_mode = 0
        ' 1 = Chassis
        ' 2 = Hull
        ' 3 = Turret
        ' 4 = Gun
        ' 5 Segment
        'Hack to remove _skins from british tank
        'C:\Games\World_of_Tanks\res_mods\1.1.0.1\vehicles\british\GB93_Caernarvon_AX\_skins\RoyalCorps\normal\lod0
        'If file_name.Contains("/_skin") And Not frmComponents.look_for_skins_cb.Checked Then
        '    Dim s_parts = file_name.Split("/")
        '    file_name = s_parts(0) + "/" + s_parts(1) + "/" + s_parts(2) + "/" + s_parts(5) + "/" + s_parts(6) + "/" + s_parts(7)

        'End If
        If InStr(Path.GetFileNameWithoutExtension(file_name), "Chassis") > 0 Then
            xmlget_mode = 1
        End If
        If InStr(Path.GetFileNameWithoutExtension(file_name), "Hull") > 0 Then
            xmlget_mode = 2
        End If
        If InStr(Path.GetFileNameWithoutExtension(file_name), "Turret") > 0 Then
            xmlget_mode = 3
        End If
        If InStr(Path.GetFileNameWithoutExtension(file_name), "Gun_") > 0 Then
            xmlget_mode = 4
        End If
        If InStr(Path.GetFileNameWithoutExtension(file_name), "segment") > 0 Then
            xmlget_mode = 5
        End If
        If PRIMITIVES_MODE Then xmlget_mode = 0
        section_names(xmlget_mode) = New names_


        file_name = file_name.Replace("\", "/")
        If CRASH_MODE Then
            file_name = file_name.Replace("/normal/", "/crash/")
        End If
        '============================'============================
        'open visual
        Dim old_file_name = file_name
        If Not openVisual(file_name) Then
            log_text.Append("File Not Found :" + file_name + vbCrLf)
            Return False
        End If
        file_name = old_file_name.Replace("model", "primitives_processed")
        '============================'============================
        'this gets all the entries in the xml file..
        'IE.. bone and wheel locations
        'NOT LONGER LOADING THIS!
        'If xmlget_mode = 1 Then
        '    praseVisualXml()
        'End If


        'Some tanks have the primuitive used stored in the visual files.
        'NOT ALL so we have to trap it.
        file_name = get_actual_primitive_name()
        If file_name = "" Then
            file_name = old_file_name.Replace("model", "primitives_processed")
        Else
            file_name &= ".primitives_processed"
        End If
        '####################################################################################
        'Since I wrote this, The order has been fucking backwards!
        'This fixes that major screwup with out affecting existing
        'models people have been working on.
        If Not Get_ordered_names() Then
            Return False 'Something went wrong with getting names in visual file
        End If
        '####################################################################################
        'Load Prmitive Data
        cnt = 0
        Dim r As New MemoryStream
        Dim b_reader As New BinaryReader(r)
        Dim buf() As Byte
        'first.. look in the res_mod folder for this tank model
        Dim t_path = My.Settings.res_mods_path + "\" + file_name.Replace("/", "\")
        If File.Exists(t_path) And Not LOADING_FBX Then
            'yes... read the file
            buf = File.ReadAllBytes(t_path)
            r = New MemoryStream(buf)
            File_len = r.Length
            loaded_from_resmods = True
        Else
            'no.. get the file from the package...
            If current_tank_package = 0 Then 'trap excep thrown by stand alone primitive load.
                log_text.Append("File Not Found in package.." + file_name + vbCrLf)
                Return False
            End If
            Dim entry As ZipEntry = frmMain.packages(current_tank_package)(file_name)
            If entry IsNot Nothing Then
                entry.Extract(r)
            Else
                Try
                    entry = frmMain.packages_2(current_tank_package)(file_name)
                    If entry Is Nothing Then
                        entry = frmMain.packages_3(current_tank_package)(file_name)

                    End If
                Catch ex As Exception
                End Try
                If entry IsNot Nothing Then
                    entry.Extract(r)
                Else
                    entry = search_shared_pkgs(file_name)
                    If entry IsNot Nothing Then
                        entry.Extract(r)
                    Else
                        'last ditch try searching all packages for the model
                        entry = find_tank_and_return_entry_in_pkgs(file_name)
                        If entry Is Nothing Then
                            log_text.Append("File Not Found in package.." + file_name + vbCrLf)
                            Return False
                        Else
                            entry.Extract(r)
                        End If
                    End If
                End If
            End If
            r.Position = 0
            File_len = r.Length
            ReDim buf(File_len)
            For i = 0 To File_len - 1
                buf(i) = b_reader.ReadByte
            Next
        End If

        r.Dispose()
        '####################################################################################
        Dim stream As MemoryStream = New MemoryStream(buf)
        b_reader = New BinaryReader(stream)
        'set data_heaps size

        Dim dummy As UInt32
        'need to get start of section table
        b_reader.BaseStream.Seek(File_len - 4, SeekOrigin.Begin)
        Dim position As Long = b_reader.BaseStream.Position

        'position is start of list of sections
        Dim sec_table_start As Long = b_reader.ReadInt32

        'sec_table_start points at offset in file
        Try
            b_reader.BaseStream.Seek(File_len - 4 - sec_table_start, SeekOrigin.Begin)
        Catch ex As Exception
            MsgBox("Corrupt file! : " + file_name, MsgBoxStyle.Exclamation, "Fail!!")
            Return False
        End Try
        ReDim section_names(xmlget_mode).names(10)
        ReDim Preserve section_names(xmlget_mode).names(99)
        ReDim Preserve section_locations(99)
        Dim location As ULong = 4 ' we start with offset of 4
        For i = 0 To 99
            If b_reader.BaseStream.Position < File_len - 4 Then
                section_sizes(i) = b_reader.ReadUInt32
                section_locations(i) = location
                location += section_sizes(i)
                location += location Mod 4
                'read 16 bytes of unused junk
                dummy = b_reader.ReadUInt32
                dummy = b_reader.ReadUInt32
                dummy = b_reader.ReadUInt32
                dummy = b_reader.ReadUInt32
                'get section names length
                Dim sec_name_len As UInt32 = b_reader.ReadUInt32
                'get this sections name
                For read_at As UInteger = 1 To sec_name_len
                    na = na & b_reader.ReadChar
                Next
                section_names(xmlget_mode).names(i) = na.Trim

                Dim l = na.Length Mod 4 'read off pad characters
                If l > 0 Then
                    b_reader.ReadChars(4 - l)
                End If
                na = ""
            Else
                ReDim Preserve section_sizes(i - 1)
                ReDim Preserve section_locations(i - 1)
                ReDim Preserve section_names(xmlget_mode).names(i - 1)
                entry_count = i
                Exit For
            End If
        Next

        Dim by As Byte = 0
        b_reader.BaseStream.Seek(0, SeekOrigin.Begin)
        dummy = b_reader.ReadUInt32 ' read off special tag characters (4)
        'this saves the different sections to their own files in the C:\ root
#If 1 Then
        f_name_vertices = "zz"
        f_name_indices = "zz"
        f_name_uv2 = "zz"
        f_name_color = "zz"
        has_uv2 = False
        has_color = False
        'End If
        Dim sub_groups As Integer = 0
        Dim section_id As Integer = 0
        'section_names(xmlget_mode).names = get_section_names()    ' added july 4th 2013
        For i = 0 To entry_count - 1
            'find type name in section names.
            If InStr(section_names(xmlget_mode).names(i), "vertices") > 0 Then
                'Debug.WriteLine("vertices")
                f_name_vertices = section_names(xmlget_mode).names(i).Trim
                sub_groups += 1 ' if this has sub models
                If xmlget_mode = 1 Then
                    CHASSIS_COUNT += 1
                End If
            End If
            If InStr(section_names(xmlget_mode).names(i), "indices") > 0 Then
                'Debug.WriteLine("indices")
                f_name_indices = section_names(xmlget_mode).names(i).Trim
                section_id = i
            End If
            If InStr(section_names(xmlget_mode).names(i).ToLower, "uv2") > 0 Then
                'Debug.WriteLine("uv2")
                f_name_uv2 = section_names(xmlget_mode).names(i).Trim
            End If
            If InStr(section_names(xmlget_mode).names(i), "colour") > 0 Then
                'Debug.WriteLine("colour")
                f_name_color = section_names(xmlget_mode).names(i).Trim
            End If

            Try

                Dim id As Integer = 0
                Dim pk As Long = b_reader.BaseStream.Position
                'loop through order_names and find matching type name
                For id = 0 To ordered_names.Length - 1
                    If ordered_names(id).vert_name = f_name_vertices Then
                        ReDim ordered_names(id).vert_data(section_sizes(i))
                        stream.Position = section_locations(i)
                        ordered_names(id).vert_data = b_reader.ReadBytes(section_sizes(i))
                        GoTo next_m
                    End If
                    If ordered_names(id).indi_name = f_name_indices Then
                        ordered_names(id).section_name_id = section_id
                        ReDim ordered_names(id).indi_data(section_sizes(i))
                        stream.Position = section_locations(i)
                        ordered_names(id).indi_data = b_reader.ReadBytes(section_sizes(i))
                        GoTo next_m
                    End If
                    If ordered_names(id).color_name = f_name_color Then
                        ordered_names(id).has_color = True
                        stream.Position = section_locations(i)
                        b_reader.BaseStream.Position += 132

                        ordered_names(id).color_count = b_reader.ReadUInt32
                        ReDim ordered_names(id).color_data(section_sizes(i) - 137)
                        ordered_names(id).color_data = b_reader.ReadBytes(section_sizes(i) - 136)
                        GoTo next_m
                    End If
                    If ordered_names(id).uv2_name = f_name_uv2 Then
                        ordered_names(id).has_uv2 = True
                        stream.Position = section_locations(i)
                        b_reader.BaseStream.Position += 132

                        ordered_names(id).uv2_cnt = b_reader.ReadUInt32
                        ReDim ordered_names(id).uv2_data(section_sizes(i) - 137)
                        ordered_names(id).uv2_data = b_reader.ReadBytes(section_sizes(i) - 136)
                        GoTo next_m
                    End If

                Next
next_m:
                Dim l = b_reader.BaseStream.Position Mod 4 'read off pad characters
                If l > 0 Then
                    b_reader.ReadChars(4 - l)
                End If
                f_name_vertices = "zz"
                f_name_indices = "zz"
                f_name_uv2 = "zz"
                f_name_color = "zz"
            Catch ex As Exception
                log_text.Append("Something is wrong with primitives contents table.")
                Return False
            End Try
        Next
        b_reader.Close()
        buf = Nothing
#End If
        Dim loop_count As Integer = 0
        Dim section_count As Integer = 0
        ' add_flag .... so we dont set this to false if it was already set.
        ' This has to happen BEFORE the next_sub_section loop!
        Dim add_flag As Boolean = _add
        'Try

        Dim sg = sub_groups - 1
        While sub_groups > 0
            sub_groups -= 1 ' take one off.. if there is one, this results is zero and collects only one model set
            If ordered_names(sg - sub_groups).has_color Then ' we need to convert them in to RGBA so they can he wrote to a FBX as vertex colors
                ReDim color_rgb(ordered_names(sg - sub_groups).color_count - 1)
                For i = 0 To (ordered_names(sg - sub_groups).color_count - 1)
                    color_rgb(i).r = CSng(ordered_names(sg - sub_groups).color_data((i * 4) + 0) / 255)
                    color_rgb(i).g = CSng(ordered_names(sg - sub_groups).color_data((i * 4) + 1) / 255)
                    color_rgb(i).b = CSng(ordered_names(sg - sub_groups).color_data((i * 4) + 2) / 255)
                    color_rgb(i).a = CSng(ordered_names(sg - sub_groups).color_data((i * 4) + 3) / 255)
                Next
            End If

            ' changed july 7/11/14
            f_name_vertices = ordered_names(sg - sub_groups).vert_name
            f_name_indices = ordered_names(sg - sub_groups).indi_name
            f_name_uv2 = ordered_names(sg - sub_groups).uv2_name
            If ordered_names(sg - sub_groups).uv2_name.Length > 2 Then
                has_uv2 = True
                save_has_uv2 = True
            Else
                has_uv2 = False
                save_has_uv2 = False
            End If


            If ordered_names(sg - sub_groups).color_name.Length > 0 Then
                has_color = True
            Else
                has_color = False
            End If

            If sub_groups > 0 Then pg_flag = True
            'open indices.sec
            Dim ri = New MemoryStream(ordered_names(section_count).indi_data)
            Dim ib_reader = New BinaryReader(ri)
            ib_reader.BaseStream.Seek(0, SeekOrigin.Begin)
            Dim cr As Byte
            Dim dr As Boolean = False
            ' get its name string
            For i = 0 To 63
                cr = ib_reader.ReadByte
                If cr = 0 Then dr = True
                If cr > 30 And cr <= 123 Then
                    If Not dr Then
                        na = na & Chr(cr)

                    End If
                End If
            Next
            Dim r_count As UInt32 = 0
            ih.ind_h = na
            If InStr(na, "list32") > 0 Then
                ind_scale = 4
            Else
                ind_scale = 2
            End If
            Try
                ih.nIndices_ = ib_reader.ReadUInt32
                ih.nInd_groups = ib_reader.ReadUInt32

            Catch ex As Exception
                MsgBox("data in " + file_name + " is unreadable!", MsgBoxStyle.Exclamation, "Error!")
                Return False
            End Try

            dr = False
            na = ""
            ReDim pGroups(ih.nInd_groups)

            Dim nOffset As UInteger = (ih.nIndices_ * ind_scale) + 72
            ib_reader.BaseStream.Seek(nOffset, SeekOrigin.Begin)
            'Get the groups.. IE. get addresses, offsets and counts for the parts in the model
            For i = 0 To ih.nInd_groups - 1
                pGroups(i).startIndex_ = ib_reader.ReadUInt32
                pGroups(i).nPrimitives_ = ib_reader.ReadUInt32
                pGroups(i).startVertex_ = ib_reader.ReadUInt32
                pGroups(i).nVertices_ = ib_reader.ReadUInt32

            Next
            'get basic vertices info
            Dim vr = New MemoryStream(ordered_names(section_count).vert_data)
            Dim vb_reader = New BinaryReader(vr)
            vb_reader.BaseStream.Seek(0, SeekOrigin.Begin)
            Dim curpos As ULong = vb_reader.BaseStream.Position
            ' get its name string
            For i = 0 To 63
                cr = vb_reader.ReadByte
                If cr = 0 Then dr = True
                If cr > 64 And cr <= 123 Then
                    If Not dr Then
                        na = na & Chr(cr)
                    End If
                End If
            Next
            dr = False
            vh.header_text = na
            na = ""
            '-------------------------------
            ' get stride of each vertex element
            Dim BPVT_mode As Boolean = False
            Dim realNormals As Boolean = False
            Dim c_stride As Integer = 5
            Dim compute_tangents As Boolean
            If vh.header_text = "xyznuv" Then
                stride = 32
                realNormals = True
                compute_tangents = True
            End If
            If vh.header_text = "BPVTxyznuv" Then
                BPVT_mode = True
                stride = 24
                realNormals = False
                compute_tangents = True
            End If
            If vh.header_text = "xyznuviiiwwtb" > 0 Then
                stride = 37
            End If
            If vh.header_text = "BPVTxyznuviiiww" Then
                BPVT_mode = True
                stride = 32
                compute_tangents = True
            End If
            If vh.header_text = "BPVTxyznuviiiwwtb" Then
                BPVT_mode = True
                stride = 40
            End If
            If vh.header_text = "xyznuvtb" Then
                stride = 32
            End If
            If vh.header_text = "BPVTxyznuvtb" Then
                BPVT_mode = True
                stride = 32
            End If
            If BPVT_mode Then
                vb_reader.BaseStream.Position = curpos + 132
            End If
            vh.nVertice_count = vb_reader.ReadUInt32
            'now that we have a count.. lets see if we need to get the uv2 coords
            If ordered_names(sg - sub_groups).has_uv2 Then
                If ordered_names(sg - sub_groups).uv2_name.Length > 2 Then
                    ordered_names(sg - sub_groups).uv2_cnt = get_uv2(ordered_names(sg - sub_groups).uv2_data, ordered_names(sg - sub_groups).uv2_cnt)
                Else
                    log_text.Append("UV2 referenced in Visual but does not exist. :" + f_name_uv2 + vbCrLf)
                    has_uv2 = False
                End If
            End If
            '----------------------------------------------------------------------------------- add or new?
            If Not _add Then
                object_start = 1
                big_l = ih.nInd_groups 'get object count
            Else
                object_start = object_count + 1
                big_l = object_count + ih.nInd_groups
            End If

            ReDim Preserve _object(big_l)   'create spaces for n' objects
            For i = object_start To big_l
                _object(i) = New obj    ' make new objects
            Next
            ReDim Preserve _group(big_l)

            'lets fill the tbuf with the data now
            'and store the indices and vertex data for export

            ReDim tbuf(vh.nVertice_count)
            i = 0
            Dim p As Integer = 6
            For k As UInt32 = object_start To big_l

                _group(k).bsp2_id = -1

                _group(k).BPVT_mode = BPVT_mode

                has_uv2 = ordered_names(sg - sub_groups).has_uv2
                save_has_uv2 = has_uv2
                If has_uv2 Then
                    _group(k).has_uv2 = 1
                Else
                    _group(k).has_uv2 = 0
                End If
                _group(k).has_color = 0

                Dim pos As UInt32
                Try

                    If pg_flag Then
                        pos = pGroups(k - object_start).nVertices_ - 1
                        _group(k).startVertex_ = pGroups(k - object_start).startVertex_
                        _group(k).startIndex_ = pGroups(k - object_start).startIndex_
                        _group(k).nVertices_ = pGroups(k - object_start).nVertices_
                        _group(k).nPrimitives_ = pGroups(k - object_start).nPrimitives_
                    Else
                        pos = pGroups(k - object_start).nVertices_ - 1
                        _group(k).startVertex_ = pGroups(k - object_start).startVertex_
                        _group(k).startIndex_ = pGroups(k - object_start).startIndex_
                        _group(k).nVertices_ = pGroups(k - object_start).nVertices_
                        _group(k).nPrimitives_ = pGroups(k - object_start).nPrimitives_
                    End If
                Catch ex As Exception
                    Return False
                End Try
                ReDim _group(k).vertices(pos + 1)
                Dim color_runner As Integer = 0
                For cnt = 0 To pos
                    Try
                        tbuf(i) = New vertice_

                    Catch ex As Exception
                        Return False
                    End Try
                    tbuf(i).x = vb_reader.ReadSingle
                    tbuf(i).y = vb_reader.ReadSingle
                    tbuf(i).z = vb_reader.ReadSingle

                    round_signed_to(tbuf(i).x, 3)
                    round_signed_to(tbuf(i).y, 3)
                    round_signed_to(tbuf(i).z, 3)


                    If realNormals Then
                        tbuf(i).nx = vb_reader.ReadSingle
                        tbuf(i).ny = vb_reader.ReadSingle
                        tbuf(i).nz = vb_reader.ReadSingle
                    Else
                        tbuf(i).n = vb_reader.ReadUInt32

                    End If
                    tbuf(i).u = vb_reader.ReadSingle
                    tbuf(i).v = vb_reader.ReadSingle
                    If vh.header_text = "BPVTxyznuviiiww" Then
                        tbuf(i).index_1 = vb_reader.ReadByte()
                        tbuf(i).index_2 = vb_reader.ReadByte()
                        tbuf(i).index_3 = vb_reader.ReadByte()
                        tbuf(i).index_4 = vb_reader.ReadByte()
                        tbuf(i).weight_1 = vb_reader.ReadByte()
                        tbuf(i).weight_2 = vb_reader.ReadByte()
                        tbuf(i).weight_3 = vb_reader.ReadByte()
                        tbuf(i).weight_4 = vb_reader.ReadByte()
                        'no tangent and bitangent on BPVTxyznuviiiww type vertex
                    Else

                        If stride = 37 Or stride = 40 Then
                            tbuf(i).index_1 = vb_reader.ReadByte()
                            tbuf(i).index_2 = vb_reader.ReadByte()
                            tbuf(i).index_3 = vb_reader.ReadByte()
                            tbuf(i).index_4 = vb_reader.ReadByte()
                            tbuf(i).weight_1 = vb_reader.ReadByte()
                            tbuf(i).weight_2 = vb_reader.ReadByte()
                            tbuf(i).weight_3 = vb_reader.ReadByte()
                            tbuf(i).weight_4 = vb_reader.ReadByte()
                            tbuf(i).t = vb_reader.ReadUInt32
                            tbuf(i).bn = vb_reader.ReadUInt32
                        Else
                            If Not realNormals And Not stride = 24 Then
                                'these dont exist in XYZNUV format vertex
                                tbuf(i).t = vb_reader.ReadUInt32
                                tbuf(i).bn = vb_reader.ReadUInt32
                            End If
                        End If
                    End If
                    _group(k).vertices(cnt) = New vertice_
                    _group(k).vertices(cnt).x = tbuf(i).x
                    _group(k).vertices(cnt).y = tbuf(i).y
                    _group(k).vertices(cnt).z = tbuf(i).z
                    If realNormals Then
                        _group(k).vertices(cnt).nx = tbuf(i).nx
                        _group(k).vertices(cnt).ny = tbuf(i).ny
                        _group(k).vertices(cnt).nz = tbuf(i).nz
                    Else
                        _group(k).vertices(cnt).n = tbuf(i).n
                    End If
                    _group(k).vertices(cnt).u = tbuf(i).u
                    _group(k).vertices(cnt).v = tbuf(i).v
                    round_signed_to(_group(k).vertices(cnt).u, 4)
                    round_signed_to(_group(k).vertices(cnt).v, 4)

                    _group(k).vertices(cnt).index_1 = tbuf(i).index_1
                    _group(k).vertices(cnt).index_2 = tbuf(i).index_2
                    _group(k).vertices(cnt).index_3 = tbuf(i).index_3
                    _group(k).vertices(cnt).index_4 = tbuf(i).index_4
                    _group(k).vertices(cnt).weight_1 = tbuf(i).weight_1
                    _group(k).vertices(cnt).weight_2 = tbuf(i).weight_2
                    _group(k).vertices(cnt).weight_3 = tbuf(i).weight_3
                    _group(k).vertices(cnt).weight_4 = tbuf(i).weight_4
                    If Not realNormals Then
                        _group(k).vertices(cnt).t = tbuf(i).t
                        _group(k).vertices(cnt).bn = tbuf(i).bn
                    End If
                    If has_uv2 Then
                        _group(k).vertices(cnt).u2 = uv2_data(i).u
                        _group(k).vertices(cnt).v2 = uv2_data(i).v
                        round_signed_to(_group(k).vertices(cnt).u2, 4)
                        round_signed_to(_group(k).vertices(cnt).v2, 4)

                    End If
                    If ordered_names(sg - sub_groups).has_color Then
                        _group(k).has_color = 1
                        _group(k).vertices(cnt).r = color_rgb(color_runner).r
                        _group(k).vertices(cnt).g = color_rgb(color_runner).g
                        _group(k).vertices(cnt).b = color_rgb(color_runner).b
                        _group(k).vertices(cnt).a = color_rgb(color_runner).a
                        color_runner += 1
                    End If
                    i += 1
                Next cnt
            Next k
            '           vb_reader.Close() ' we are done with this :)
            vr.Close()
            vr.Dispose()
            vr = Nothing
            '------------------------------
            Dim line_cnt As Integer = 0
            Dim poly_cnt As Integer = 0
            Dim tot_polys As UInt32 = 0
            For i = 0 To ih.nInd_groups - 1
                tot_polys += pGroups(i).nPrimitives_
            Next

            Dim running As UInteger = 0
            For jj = object_start To big_l
                object_count += 1
                ' 
                '
                '
                Dim narray() As String
                If sg > 0 Then
                    get_translations(jj, ordered_names(sg - sub_groups).index + 1)  ' get texture name(s) and if this is multi textured
                    narray = ordered_names(sg - sub_groups).vert_name.Split(".")
                Else
                    get_translations(jj, jj + 1 - object_start) ' get texture name(s) and if this is multi textured
                    narray = ordered_names(sg - sub_groups).vert_name.Split(".")
                End If
                build_textures(jj) ' make a new texture and find out if this texture as been used... if so, existing texture will be pointed at

                log_text.Append("loaded Model:" + "ID:" + object_count.ToString + ":" + file_name + vbCrLf)


                Dim n_ = Path.GetFileNameWithoutExtension(file_name)
                Dim aa = n_.Split("_")
                _object(jj).name = file_name + ":" + current_tank_package.ToString + ":" + jj.ToString
                _group(jj).name = file_name + ":" + current_tank_package.ToString + ":" + jj.ToString
                _group(jj).header = vh.header_text ' save vertex type
                If PRIMITIVES_MODE Then
                    XML_Strings(0) = TheXML_String
                End If
                If Not PRIMITIVES_MODE Then
                    Try

                        If _object(jj).name.ToLower.Contains("chassis") Then
                            'check if wheeled vehicle.. If so, set flag to disable writing chassis primitive.
                            If TheXML_String.ToLower.Contains("pbs_wheel_skinned") Then
                                is_wheeled_vehicle = True
                            Else
                                is_wheeled_vehicle = False
                            End If
                            If _group(jj).color_name.ToLower.Contains("chass") Then
                                _group(jj).tank_part = "chassis"
                                _object(jj).is_track = 0
                                _group(jj).is_carraige = True

                            Else
                                _group(jj).tank_part = "track"
                                _object(jj).is_track = 1
                                _group(jj).is_carraige = False
                            End If
                            If XML_Strings(1).Length = 0 Then
                                XML_Strings(1) = TheXML_String
                            End If
                            _object(jj).camo_tiling = chassis_tiling
                            _object(jj).exclude_camo = 1
                            _object(jj).use_camo = 0
                        End If
                        If hull_count = 0 Then
                            If _object(jj).name.ToLower.Contains("hull") Then
                                _group(jj).tank_part = "hull"
                                If XML_Strings(2).Length = 0 Then
                                    XML_Strings(2) = TheXML_String
                                End If
                                _object(jj).camo_tiling = hull_tiling
                                _object(jj).use_camo = 0
                                _object(jj).exclude_camo = 0
                                hull_count += 1
                            End If
                        Else
                            If _object(jj).name.ToLower.Contains("hull") Then
                                _group(jj).tank_part = "hull"
                                _object(jj).camo_tiling = hull_tiling
                                _object(jj - 1).exclude_camo = 1
                                _object(jj).use_camo = 0
                            End If

                        End If
                        If turret_count = 0 Then
                            If _object(jj).name.ToLower.Contains("turret") Then
                                _group(jj).tank_part = "turret"
                                If XML_Strings(3).Length = 0 Then
                                    XML_Strings(3) = TheXML_String
                                End If
                                _object(jj).camo_tiling = turret_tiling
                                _object(jj).use_camo = 0
                                _object(jj).exclude_camo = 0
                                turret_count += 1
                            End If
                        Else
                            If _object(jj).name.ToLower.Contains("turret") Then
                                _group(jj).tank_part = "turret"
                                _object(jj).exclude_camo = 1
                                _object(jj).use_camo = 0
                            End If

                        End If
                        If _object(jj).name.ToLower.Contains("gun") Then
                            _group(jj).tank_part = "gun"
                            If XML_Strings(4).Length = 0 Then
                                XML_Strings(4) = TheXML_String
                            End If
                            _object(jj).camo_tiling = gun_tiling
                            _object(jj).use_camo = 0
                            _object(jj).exclude_camo = 0
                        End If
                        If _object(jj).name.ToLower.Contains("segment") Then
                            _group(jj).tank_part = "segment"
                            _object(jj).exclude_camo = 1
                            _object(jj).use_camo = 0
                        End If

                    Catch ex As Exception

                    End Try
                End If 'not primitive_mode
                _group(jj).component_visible = True

                Dim tn = _group(jj).name.Split(":")
                frmComponentView.add_to_group_list(jj, Path.GetFileNameWithoutExtension(tn(0)))

                _object(jj).ID = jj
                cnt = pGroups(jj - object_start).nPrimitives_
                'redim indices size
                ReDim Preserve _group(jj).indicies(cnt)
                ' get indices offset
                ib_reader.BaseStream.Seek(pGroups(jj - object_start).startIndex_ * ind_scale + 72, SeekOrigin.Begin)
                ReDim Preserve _object(jj).tris(cnt)
                _object(jj).count = cnt
                _object(jj).old_count = cnt

                For i = 1 To cnt
                    master_cnt += 1
                    running += 1
                    _object(jj).tris(i) = New triangle
                    'put in the ID marker
                    _object(jj).tris(i).id = i

                    Dim v3 As New vect3
                    Dim h_offset As Integer = 32
                    Dim p1 As UInt32
                    Dim p2 As UInt32
                    Dim p3 As UInt32


                    If ind_scale = 2 Then
                        p2 = ib_reader.ReadUInt16
                        p1 = ib_reader.ReadUInt16
                        p3 = ib_reader.ReadUInt16
                    Else
                        p2 = ib_reader.ReadUInt32
                        p1 = ib_reader.ReadUInt32
                        p3 = ib_reader.ReadUInt32
                    End If
                    'save the vertex pointers
                    _group(jj).indicies(i) = New uvect3
                    '_group(jj).is_carraige = False

                    'this needs to be sorted out for primitives
                    Dim i1, i2, i3 As Integer
                    i1 = p1 : i2 = p2 : i3 = p3
                    If file_name.ToLower.Contains("hull") Or file_name.ToLower.Contains("turret") Or PRIMITIVES_MODE Then
                        _group(jj).indicies(i).v1 = p2
                        _group(jj).indicies(i).v2 = p1
                        _group(jj).indicies(i).v3 = p3
                    Else
                        _group(jj).indicies(i).v1 = p1
                        _group(jj).indicies(i).v2 = p2
                        _group(jj).indicies(i).v3 = p3
                    End If
                    'If _group(jj).skinned And PRIMITIVES_MODE Then
                    '    p1 = i2
                    '    p2 = i1
                    '    p3 = i3
                    'Else
                    '    p1 = i1
                    '    p2 = i2
                    '    p3 = i3

                    'End If

                    '1st
                    _object(jj).tris(i).indi_id_1 = p1
                    _object(jj).tris(i).color1.x = tbuf(p1).index_1 / 255.0!
                    _object(jj).tris(i).color1.y = tbuf(p1).index_2 / 255.0!
                    _object(jj).tris(i).color1.z = tbuf(p1).index_3 / 255.0!
                    _object(jj).tris(i).c1.x = tbuf(p1).r
                    _object(jj).tris(i).c1.y = tbuf(p1).g
                    _object(jj).tris(i).c1.z = tbuf(p1).b
                    _object(jj).tris(i).c1.w = tbuf(p1).a

                    _object(jj).tris(i).v1.x = tbuf(p1).x
                    _object(jj).tris(i).v1.y = tbuf(p1).y
                    _object(jj).tris(i).v1.z = tbuf(p1).z

                    If realNormals Then
                        _object(jj).tris(i).n1.x = tbuf(p1).nx
                        _object(jj).tris(i).n1.y = tbuf(p1).ny
                        _object(jj).tris(i).n1.z = tbuf(p1).nz
                    Else
                        If BPVT_mode Then
                            v3 = unpackNormal_8_8_8(tbuf(p1).n)   ' unpack normals
                        Else
                            v3 = unpackNormal(tbuf(p1).n)   ' unpack normals

                        End If
                        _object(jj).tris(i).n1.x = v3.x
                        _object(jj).tris(i).n1.y = v3.y
                        _object(jj).tris(i).n1.z = v3.z
                    End If
                    '
                    _object(jj).tris(i).uv1 = New uv_
                    _object(jj).tris(i).uv1.u = tbuf(p1).u
                    _object(jj).tris(i).uv1.v = tbuf(p1).v
                    _object(jj).tris(i).t1 = unpackNormal(tbuf(p1).t)
                    _object(jj).tris(i).b1 = unpackNormal(tbuf(p1).bn)

                    _object(jj).tris(i).uv1_2 = New uv_
                    If has_uv2 Then
                        _object(jj).tris(i).uv1_2 = uv2_data(p1)
                        _object(jj).tris(i).uv2_id_1 = p1
                    End If

                    '2nd

                    _object(jj).tris(i).indi_id_2 = p2
                    _object(jj).tris(i).color2.x = tbuf(p2).index_1 / 255.0!
                    _object(jj).tris(i).color2.y = tbuf(p2).index_2 / 255.0!
                    _object(jj).tris(i).color2.z = tbuf(p2).index_3 / 255.0!
                    _object(jj).tris(i).c2.x = tbuf(p2).r
                    _object(jj).tris(i).c2.y = tbuf(p2).g
                    _object(jj).tris(i).c2.z = tbuf(p2).b
                    _object(jj).tris(i).c2.w = tbuf(p2).a

                    _object(jj).tris(i).v2.x = tbuf(p2).x
                    _object(jj).tris(i).v2.y = tbuf(p2).y
                    _object(jj).tris(i).v2.z = tbuf(p2).z

                    If realNormals Then
                        _object(jj).tris(i).n2.x = tbuf(p2).nx
                        _object(jj).tris(i).n2.y = tbuf(p2).ny
                        _object(jj).tris(i).n2.z = tbuf(p2).nz
                    Else
                        If BPVT_mode Then
                            v3 = unpackNormal_8_8_8(tbuf(p2).n)   ' unpack normals
                        Else
                            v3 = unpackNormal(tbuf(p2).n)   ' unpack normals
                        End If
                        _object(jj).tris(i).n2.x = v3.x
                        _object(jj).tris(i).n2.y = v3.y
                        _object(jj).tris(i).n2.z = v3.z
                    End If
                    '
                    _object(jj).tris(i).uv2 = New uv_
                    _object(jj).tris(i).uv2.u = tbuf(p2).u
                    _object(jj).tris(i).uv2.v = tbuf(p2).v
                    _object(jj).tris(i).t2 = unpackNormal(tbuf(p2).t)
                    _object(jj).tris(i).b2 = unpackNormal(tbuf(p2).bn)

                    _object(jj).tris(i).uv2_2 = New uv_
                    If has_uv2 Then
                        _object(jj).tris(i).uv2_2 = uv2_data(p2)
                        _object(jj).tris(i).uv2_id_2 = p2
                    End If

                    '3rd
                    _object(jj).tris(i).indi_id_3 = p3
                    _object(jj).tris(i).color3.x = tbuf(p3).index_1 / 255.0!
                    _object(jj).tris(i).color3.y = tbuf(p3).index_2 / 255.0!
                    _object(jj).tris(i).color3.z = tbuf(p3).index_3 / 255.0!
                    _object(jj).tris(i).c3.x = tbuf(p3).r
                    _object(jj).tris(i).c3.y = tbuf(p3).g
                    _object(jj).tris(i).c3.z = tbuf(p3).b
                    _object(jj).tris(i).c3.w = tbuf(p3).a

                    _object(jj).tris(i).v3.x = tbuf(p3).x
                    _object(jj).tris(i).v3.y = tbuf(p3).y
                    _object(jj).tris(i).v3.z = tbuf(p3).z

                    If realNormals Then
                        _object(jj).tris(i).n3.x = tbuf(p3).nx
                        _object(jj).tris(i).n3.y = tbuf(p3).ny
                        _object(jj).tris(i).n3.z = tbuf(p3).nz
                    Else
                        If BPVT_mode Then
                            v3 = unpackNormal_8_8_8(tbuf(p3).n)   ' unpack nromals
                        Else
                            v3 = unpackNormal(tbuf(p3).n)   ' unpack nromals
                        End If
                        _object(jj).tris(i).n3.x = v3.x
                        _object(jj).tris(i).n3.y = v3.y
                        _object(jj).tris(i).n3.z = v3.z
                    End If
                    '
                    _object(jj).tris(i).uv3 = New uv_
                    _object(jj).tris(i).uv3.u = tbuf(p3).u
                    _object(jj).tris(i).uv3.v = tbuf(p3).v
                    _object(jj).tris(i).t3 = unpackNormal(tbuf(p3).t)
                    _object(jj).tris(i).b3 = unpackNormal(tbuf(p3).bn)

                    _object(jj).tris(i).uv3_2 = New uv_
                    If has_uv2 Then
                        _object(jj).tris(i).uv3_2 = uv2_data(p3)
                        _object(jj).tris(i).uv2_id_3 = p3
                    End If

                Next

                ReDim _group(jj).matrix(16)
                loop_count += 1

                If compute_tangents Then
                    get_tangents(jj)
                End If
                Application.DoEvents()
                make_lists(jj)
                _object(jj).find_center() 'must be after pre transform!

                _object(jj).modified = False
                GC.Collect()
                _group(jj).table_entry_name = ordered_names(sg - sub_groups).indi_name

            Next jj
no_line:

            Application.DoEvents()
all_done:
            ib_reader.Close()
            vb_reader.Close()
            ib_reader = Nothing
            vb_reader = Nothing
            r = Nothing
            ri.Dispose()
            vr = Nothing
            ' this section is for loading the the UV2 map if it has one
            _add = True ' need to set this if we are going to loop again
            If sub_groups > 0 Then
                section_count += 1
                'im making a horrible guess that the verts and indices are always frist on the visual list!!!
                f_name_vertices = ordered_names(sg - sub_groups).vert_name
                f_name_indices = ordered_names(sg - sub_groups).indi_name
                f_name_uv2 = ordered_names(sg - sub_groups).uv2_name
                If f_name_uv2.Length > 0 Then
                    has_uv2 = True
                    f_name_uv2 = ordered_names(sg - sub_groups).uv2_name
                Else
                    has_uv2 = False
                End If
            End If
        End While ' end of sub_groups loop
        'Catch ex As Exception
        _add = add_flag
        log_text.Append("")
        tank_center_X = (x_max + x_min) / 2.0
        tank_center_Y = (y_max + y_min) / 2.0
        tank_center_Z = (z_max + z_min) / 2.0

        Dim os As String = ""
        'frmMain.Text = "File: " + file_name.Replace(".visual", ".primitives")
        Return True
    End Function
    Public Sub round_signed_to(ByRef n As Single, ByRef places As Integer)
        Dim t As Single = Truncate(n)
        Dim r As Integer = (n - t) * (10 ^ places)
        Dim r2 As Single = r / (10 ^ places)
        n = t + r2
    End Sub
    Private Sub get_tangents(ByVal id As Integer)
        Dim uv1, uv2, uv3 As uv_
        Dim v1, v2, v3 As vect3
        For i As UInteger = 1 To _object(id).count

            v1 = _object(id).tris(i).v1
            uv1 = _object(id).tris(i).uv1
            v2 = _object(id).tris(i).v2
            uv2 = _object(id).tris(i).uv2
            v3 = _object(id).tris(i).v3
            uv3 = _object(id).tris(i).uv3
            Dim tan, bn As vect3
            ComputeTangentBasis(v1, v2, v3, uvToVec3t(uv1), uvToVec3t(uv2), uvToVec3t(uv3), tan, bn) ' calculate tan and biTan
            _object(id).tris(i).t1 = tan
            _object(id).tris(i).t2 = tan
            _object(id).tris(i).t3 = tan
            _object(id).tris(i).b1 = bn
            _object(id).tris(i).b2 = bn
            _object(id).tris(i).b3 = bn
        Next

    End Sub
    Private Function uvToVec3t(ByVal uv As uv_) As vect3
        Dim v As vect3
        v.x = uv.u
        v.y = uv.v
        v.z = 0.0!
        Return v
    End Function
    Private Function Get_ordered_names()
        Dim t = xmldataset.Copy
        Dim geo, _stream, material As New DataTable
        material = t.Tables("material")

        If t.Tables.Contains("geometry") Then
            geo = t.Tables("geometry").Copy

        End If
        Dim has_stream As Boolean = False
        Try
            If t.Tables.Contains("stream") Then
                _stream = t.Tables("stream").Copy
            End If
            has_stream = True
        Catch ex As Exception

        End Try
        Dim indxcol As DataColumn = New DataColumn("index")
        indxcol.DataType = System.Type.GetType("System.Int32")
        geo.Columns.Add(indxcol)
        For i = 0 To geo.Rows.Count - 1
            geo.Rows(i).Item("index") = i
        Next

        If Not PRIMITIVES_MODE Then

            Try
                If Not geo.Columns.Contains("stream") Then
                    geo.Columns.Add("stream").DefaultValue = Nothing
                End If
            Catch ex As Exception

            End Try

            Try
                Dim p_ = From row_ In geo.AsEnumerable
                         Select
                            vert_name = row_.Field(Of String)("vertices"),
                            indi_name = row_.Field(Of String)("primitive"),
                            uv2_name = row_.Field(Of String)("stream"),
                            colour_name = row_.Field(Of String)("stream"),
                            index = row_.Field(Of Int32)("geometry_Id")
                         Order By index Descending '<--- this line failed BAD! It did NOT sort the items descending!!


                ReDim ordered_names(p_.Count)
                Dim cnt = 0
                For Each p In p_
                    ordered_names(cnt).uv2_name = ""
                    ordered_names(cnt).color_name = ""
                    ordered_names(cnt).vert_name = p.vert_name.Trim
                    ordered_names(cnt).indi_name = p.indi_name.Trim
                    If p.uv2_name IsNot Nothing Then
                        If Not p.uv2_name.Contains("colour") Then
                            ordered_names(cnt).uv2_name = p.uv2_name.Trim
                        End If
                    End If
                    If p.colour_name IsNot Nothing Then
                        If Not p.colour_name.Contains("uv2") Then
                            ordered_names(cnt).color_name = p.colour_name.Trim
                        End If
                    End If
                    ordered_names(cnt).index = p.index
                    'must clear these first!
                    If has_stream Then
                        Dim pp_ = From row In _stream.AsEnumerable
                                  Where row.Field(Of Integer)("geometry_Id") = p.index
                                  Select st = row.Field(Of String)("stream_Text")
                        For Each pa In pp_
                            If pa IsNot Nothing Then
                                If pa.Contains("colour") Then
                                    ordered_names(cnt).color_name = pa.Trim
                                End If
                                If pa.Contains("uv2") Then
                                    ordered_names(cnt).uv2_name = pa.Trim
                                End If
                            End If
                        Next
                    End If
                    cnt += 1
                Next
            Catch ex As Exception

                indxcol.Dispose()
                geo.Dispose()
                t.Dispose()
                MsgBox("Tank: " & file_name & " XML is screwy!", MsgBoxStyle.Exclamation, "Moron authors!")
                Return False
            End Try
        Else
            'If primitive mode active we are here... Sort them correctly
            Try
                If Not geo.Columns.Contains("stream") Then
                    geo.Columns.Add("stream").DefaultValue = Nothing
                End If
            Catch ex As Exception

            End Try

            Try
                Dim p_ = From row_ In geo.AsEnumerable
                         Select
                            vert_name = row_.Field(Of String)("vertices"),
                            indi_name = row_.Field(Of String)("primitive"),
                            uv2_name = row_.Field(Of String)("stream"),
                            colour_name = row_.Field(Of String)("stream"),
                            index = row_.Field(Of Int32)("geometry_Id")
                         Order By index


                ReDim ordered_names(p_.Count)
                Dim cnt = 0
                For Each p In p_
                    ordered_names(cnt).uv2_name = ""
                    ordered_names(cnt).color_name = ""
                    ordered_names(cnt).vert_name = p.vert_name.Trim
                    ordered_names(cnt).indi_name = p.indi_name.Trim
                    If p.uv2_name IsNot Nothing Then
                        If Not p.uv2_name.Contains("colour") Then
                            ordered_names(cnt).uv2_name = p.uv2_name.Trim
                        End If
                    End If
                    If p.colour_name IsNot Nothing Then
                        If Not p.colour_name.Contains("uv2") Then
                            ordered_names(cnt).color_name = p.colour_name.Trim
                        End If
                    End If
                    ordered_names(cnt).index = p.index
                    'must clear these first!
                    If has_stream Then
                        Dim pp_ = From row In _stream.AsEnumerable
                                  Where row.Field(Of Integer)("geometry_Id") = p.index
                                  Select st = row.Field(Of String)("stream_Text")
                        For Each pa In pp_
                            If pa IsNot Nothing Then
                                If pa.Contains("colour") Then
                                    ordered_names(cnt).color_name = pa.Trim
                                End If
                                If pa.Contains("uv2") Then
                                    ordered_names(cnt).uv2_name = pa.Trim
                                End If
                            End If
                        Next
                    End If
                    cnt += 1
                Next
            Catch ex As Exception

                indxcol.Dispose()
                geo.Dispose()
                t.Dispose()
                MsgBox("Tank: " & file_name & " XML is screwy!", MsgBoxStyle.Exclamation, "Moron authors!")
                Return False

            End Try
        End If
        Return True
    End Function

    Dim verts(0) As b_verts_
    Public Structure b_verts_
        Public v1, v2, v3 As SlimDX.Vector3
        Public n As vect3
        Public offv As vect3
        Public v_center As vect3
        Public Function get_center() As vect3
            Dim v As vect3
            v.x = (Me.v1.X + Me.v2.X + Me.v3.X) / 3.0
            v.y = (Me.v1.Y + Me.v2.Y + Me.v3.Y) / 3.0
            v.z = (Me.v1.Z + Me.v2.Z + Me.v3.Z) / 3.0
            v_center = v
            Return v
        End Function
        Public Function getoff() As vect3
            Dim v As vect3
            v.x = v_center.x + (0.02 * n.x)
            v.y = v_center.y + (0.02 * n.y)
            v.z = v_center.z + (0.02 * n.z)
            offv = v
            Return v
        End Function
    End Structure

    Dim mstream As New MemoryStream
    Private Function get_uv2(ByRef data() As Byte, ByVal cnt As Integer) As Integer
        Dim m As New MemoryStream(data)
        Dim br As New BinaryReader(m)
        ReDim uv2_data(cnt - 1)
        For i = 0 To cnt - 1
            uv2_data(i) = New uv_
            uv2_data(i).u = br.ReadSingle
            uv2_data(i).v = br.ReadSingle
        Next
        Return cnt
    End Function

    Public Function openVisual(ByVal filename As String) As Boolean
        Try
check_res_mods:
            filename = filename.Replace(" - Copy", "")

            Dim mstream = New MemoryStream
            If File.Exists(My.Settings.res_mods_path + "/" + filename) And Not LOADING_FBX Then
                If Not File.Exists(My.Settings.res_mods_path + "\" + filename.Replace(".model", ".visual_processed")) Then
                    If PRIMITIVES_MODE Then
                        If find_and_extract_file_in_pkgs(filename.Replace(".model", ".visual_processed")) Then
                            GoTo check_res_mods
                        End If
                    End If
                End If
                Dim buf = File.ReadAllBytes(My.Settings.res_mods_path + "\" + filename.Replace(".model", ".visual_processed"))
                mstream = New MemoryStream(buf)
                If openXml_stream(mstream, "") Then
                    file_name = filename.Replace(".model", ".primitives_processed")
                    mstream.Dispose()
                    buf = Nothing
                    Return True
                End If

                TheXML_String = File.ReadAllText(My.Settings.res_mods_path + "\" + filename.Replace(".model", ".visual_processed"))
                TheXML_String = PrettyPrint(TheXML_String)
                Dim tr As New StringReader(TheXML_String)
                Dim xmlr = New XmlTextReader(tr)
                xmldataset.ReadXml(xmlr)
                tr.Dispose()
                file_name = filename.Replace(".model", ".primitives_processed")
                GC.Collect()
                Return True
            End If
look_again:
            If PRIMITIVES_MODE Then
                If find_and_extract_file_in_pkgs(filename) Then
                    GoTo check_res_mods
                End If
            End If
            Dim e As ZipEntry = frmMain.packages(current_tank_package)(filename)
            If e IsNot Nothing Then
                e.Extract(mstream)
            Else

                Try
                    e = frmMain.packages_3(current_tank_package)(filename)
                Catch ex As Exception
                End Try
                If e IsNot Nothing Then
                    e.Extract(mstream)
                Else
                    e = frmMain.packages_2(current_tank_package)(filename)
                    If e IsNot Nothing Then
                        e.Extract(mstream)
                    Else
                        If filename.Contains("Turret_01") Then
                            filename = filename.Replace("Turret_01", "Turret_02")
                            GoTo look_again
                        End If
                    End If
                End If
            End If

            e = search_shared_pkgs(filename)
            If e IsNot Nothing Then
                e.Extract(mstream)
            End If

            openXml_stream(mstream, "")
            Dim d As DataSet = xmldataset
            Dim tbl = d.Tables("map_")
            If tbl.Columns.Contains("nodelessVisual") Then
                filename = filename.Replace(".model", ".visual_processed")
                file_name = filename.Replace(".visual_processed", ".primitives_processed")
                GoTo get_visual
            End If


            Dim q = From row In tbl.AsEnumerable
                    Select s = row.Field(Of String)("nodefullVisual")

            filename = q(0) + ".visual_processed"
            file_name = q(0) + ".primitives_processed"
get_visual:
            mstream = New MemoryStream
            e = frmMain.packages(current_tank_package)(filename)
            If e IsNot Nothing Then
                e.Extract(mstream)
            Else
                Try
                    e = frmMain.packages_2(current_tank_package)(filename)
                Catch ex As Exception
                End Try
                If e IsNot Nothing Then
                    e.Extract(mstream)
                Else
                    Try
                        e = frmMain.packages_3(current_tank_package)(filename)

                    Catch ex As Exception
                        e = Nothing
                    End Try
                    If e IsNot Nothing Then
                        e.Extract(mstream)
                    Else
                        e = search_shared_pkgs(filename)
                        If e IsNot Nothing Then
                            e.Extract(mstream)
                        Else
                            Try
                                e = frmMain.packages_2(11)(filename)
                            Catch ex As Exception
                            End Try
                            If e IsNot Nothing Then
                                e.Extract(mstream)
                            End If
                        End If
                    End If
                End If
            End If
            openXml_stream(mstream, "")
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function




    Public Function get_section_names() As Array
        'added july 4th, 2013
        'This keeps the order of the indices and vertices names correct.
        'They are not always the same order in all files.
        Dim name_list(100) As String
        Dim tbl As New DataTable
        Dim t As DataSet = xmldataset.Copy
        tbl = t.Tables("geometry")
        Dim query = From Nrow In tbl.AsEnumerable
                    Select
                        row0 = Nrow.Field(Of String)("vertices"),
                        row1 = Nrow.Field(Of String)("primitive")
        Dim cnt As Integer = 0
        For Each item In query
            name_list(cnt) = item.row0
            cnt += 1
            name_list(cnt) = item.row1
            cnt += 1
        Next

        tbl.Dispose()
        t.Dispose()
        ReDim Preserve name_list(cnt - 1)
        Return name_list
    End Function

    Public Function get_matrix(ByVal Id As Integer,
                                ByVal identifier As String,
                                ByVal connect_point As String) As vect3
        Dim return_V As New vect3
        Dim ta(4) As String
        Dim t As DataSet = xmldataset.Copy
        t.CaseSensitive = False
        Dim tbl_nodes As New DataTable
        tbl_nodes = t.Tables("node")
        If tbl_nodes Is Nothing Then
            Return New vect3
        End If
        tbl_nodes.CaseSensitive = False
        Dim tbl_transform As New DataTable
        tbl_transform = t.Tables("transform")
        tbl_transform.CaseSensitive = False
        identifier += "*"
        Dim pnt As Integer = 0
        Try
            Dim r1 = From node_row In tbl_nodes.AsEnumerable
                     Where node_row.Field(Of String)("identifier").ToLower Like identifier.ToLower
                     Select
                     r0 = node_row.Field(Of Integer)("node_Id")
            pnt = r1(0)
        Catch ex As Exception
        End Try

        Dim r2 = From tx_row In tbl_transform.AsEnumerable
                 Where tx_row.Field(Of Integer)("node_Id") =
                 pnt Select
                 row0 = tx_row.Field(Of String)("row0"),
                 row1 = tx_row.Field(Of String)("row1"),
                 row2 = tx_row.Field(Of String)("row2"),
                 row3 = tx_row.Field(Of String)("row3")
        For Each p In r2

            ta = p.row0.Split(" ")
            _object(Id).row0.x = Convert.ToSingle(ta(0))
            _object(Id).row0.y = Convert.ToSingle(ta(1))
            _object(Id).row0.z = Convert.ToSingle(ta(2))

            ta = p.row1.Split(" ")
            _object(Id).row1.x = Convert.ToSingle(ta(0))
            _object(Id).row1.y = Convert.ToSingle(ta(1))
            _object(Id).row1.z = Convert.ToSingle(ta(2))

            ta = p.row2.Split(" ")
            _object(Id).row2.x = Convert.ToSingle(ta(0))
            _object(Id).row2.y = Convert.ToSingle(ta(1))
            _object(Id).row2.z = Convert.ToSingle(ta(2))

            ta = p.row3.Split(" ")
            _object(Id).row3.x = Convert.ToSingle(ta(0))
            _object(Id).row3.y = Convert.ToSingle(ta(1))
            _object(Id).row3.z = Convert.ToSingle(ta(2))
        Next
        Dim _0 = _object(Id).row0

        Dim Str = r2(0).row0
        Dim ary = Str.Split(" ")

        If connect_point.Length > 0 Then
            Dim r3 = From node_row In tbl_nodes.AsEnumerable
                     Where node_row.Field(Of String)("identifier") Like connect_point
                     Select
        r4 = node_row.Field(Of Integer)("node_Id")
            pnt = r3(0)
            Dim r5 = From tx_row In tbl_transform.AsEnumerable
                     Where tx_row.Field(Of Integer)("node_Id") =
                        pnt
                     Select row3 = tx_row.Field(Of String)("row3")
            Str = r5(0)
            ary = Str.Split(" ")
        End If
        return_V.x = ary(0)
        return_V.y = ary(1)
        return_V.z = ary(2)
        Return return_V

        t.Dispose()
        tbl_nodes.Dispose()
        tbl_transform.Dispose()

    End Function

    Public Sub get_translations(ByVal id As Integer, ByVal loop_count As Integer)
        Dim ta(4) As String
        Dim cnt As UInt32 = 0


        Dim tbl_prim_group As New DataTable
        tbl_prim_group.Columns.Add("primitiveGroup_Id", System.Type.GetType("System.Int32"))
        Dim tr As DataRow = tbl_prim_group.NewRow
        tr("primitiveGroup_Id") = loop_count - 1

        tbl_prim_group.Rows.Add(tr)
        Dim t As DataSet = xmldataset.Copy
        t.CaseSensitive = False
        Dim tbl_property As New DataTable
        tbl_property = t.Tables("property")
        Dim tbl_nodes As New DataTable
        tbl_nodes = t.Tables("node")
        Dim tbl_transform As New DataTable
        tbl_transform = t.Tables("transform")
        Dim boundingBox As New DataTable
        boundingBox = t.Tables("boundingbox")
        If boundingBox IsNot Nothing Then
            Dim b1 = boundingBox.Rows(0)
            Dim smin As String = b1(0)
            Dim smax As String = b1(1)
            Dim a = smin.Split(" ")
            _object(id).bb_min.x = CSng(a(0))
            _object(id).bb_min.y = CSng(a(1))
            _object(id).bb_min.z = CSng(a(2))
            a = smax.Split(" ")
            _object(id).bb_max.x = CSng(a(0))
            _object(id).bb_max.y = CSng(a(1))
            _object(id).bb_max.z = CSng(a(2))
        End If


        ' pre-set these before in case this isnt part of a tank :)
        _object(id).translate.x = 0.0
        _object(id).translate.y = 0.0
        _object(id).translate.z = 0.0
        _object(id).scale.x = 1.0
        _object(id).scale.y = 1.0
        _object(id).scale.z = 1.0

        If xmlget_mode = 1 Then ' chassis
            hull_trans = New vect3

            hull_trans = get_matrix(id, "v", "Tank")
            tank_location.x = hull_trans.x + _object(id).row3.x
            tank_location.y = hull_trans.y + _object(id).row3.y
            tank_location.z = hull_trans.z + _object(id).row3.z
            _object(id).row3.x = _object(id).row3.x
            _object(id).row3.y = 0.0
            _object(id).row3.z = _object(id).row3.z
            _object(id).row2.z *= -1.0
        End If

        If xmlget_mode = 2 Then ' hull
            turret_trans = New vect3
            turret_trans = get_matrix(id, "Hull", "HP_turretJoint")
            If turret_trans.y = 0 Then
                turret_trans = get_matrix(id, "Hull", "HP_turretJoint_2")

            End If
            If loop_count - 1 = 0 Then
                turret_location.x = turret_trans.x + tank_location.x
                turret_location.y = turret_trans.y + tank_location.y
                turret_location.z = turret_trans.z + tank_location.z
            End If
            _object(id).row3.x = tank_location.x
            _object(id).row3.y = tank_location.y
            _object(id).row3.z = tank_location.z
        End If

        If xmlget_mode = 3 Then ' turret

            gun_trans = get_matrix(id, "Turret", "HP_gunJoint")
            gun_location.x = gun_trans.x
            gun_location.y = gun_trans.y
            gun_location.z = gun_trans.z

            If tbl_property Is Nothing Then
                gun_location.x = gun_trans.x
                gun_location.y = gun_trans.y
                gun_location.z = gun_trans.z
            Else
                _object(id).row3.z = turret_location.z
                _object(id).row3.y = turret_location.y
                _object(id).row3.x = turret_location.x

            End If
        End If

        If xmlget_mode = 4 Then ' Gun
            gun_trans2 = get_matrix(id, "Gun", "G")

            _object(id).row3.x = -(gun_trans2.x - gun_location.x - turret_location.x)
            _object(id).row3.y = (gun_trans2.y + gun_location.y + turret_location.y)
            _object(id).row3.z = (gun_location.z + turret_location.z)

            If Not CRASH_MODE Then
                _object(id).row2.x *= -1.0
                _object(id).row2.y *= -1.0
                _object(id).row2.z *= -1.0

            End If

        End If

        If xmlget_mode = 5 Then
            get_matrix(id, "Scene Root", "")

        End If
        If xmlget_mode = 0 Then
            get_matrix(id, "Scene Root", "")

        End If

        store_matrix(id)

        _group(id).blend_only = False
        _group(id).multi_textured = False
        _group(id).metal_textured = False
        _group(id).bumped = False
        _group(id).detail_name = Nothing
        _group(id).color_name = Nothing
        _group(id).normal_name = Nothing
        _group(id).hasColorID = Nothing
        _group(id).alphaTest = 0
        _group(id).hasColorID = 0

        'finds the textures
        get_texturesNames_and_State(id, loop_count)

        Try
            _group(id).multi_textured = (_group(id).detail_name IsNot Nothing And _group(id).color_name IsNot Nothing)
            _group(id).blend_only = (_group(id).detail_name IsNot Nothing And _group(id).color_name Is Nothing)
            _group(id).bumped = (_group(id).normal_name IsNot Nothing)
            If InStr(_group(id).color_name, "Alpha") > 0 Then
                _group(id).blend_only = True
            End If
        Catch ex As Exception

        End Try
        _group(id).color_Id = -1
        _group(id).metalGMM_Id = -1
        _group(id).normal_Id = -1
        _group(id).detail_Id = -1

        If tbl_property IsNot Nothing Then
            tbl_property.Dispose()
        End If
        t.Dispose()
        GC.Collect()
    End Sub

    Private Sub store_matrix(jj As Integer)
        _object(jj).matrix = {
                                    _object(jj).row0.x, _object(jj).row0.y, _object(jj).row0.z, 0.0F,
                                    _object(jj).row1.x, _object(jj).row1.y, _object(jj).row1.z, 0.0F,
                                    _object(jj).row2.x, _object(jj).row2.y, _object(jj).row2.z, 0.0F,
                                    _object(jj).row3.x, _object(jj).row3.y, _object(jj).row3.z, 1.0F
                                    }

    End Sub
    Public Function get_texturesNames_and_State(id, loop_count)
        Dim delim As String() = New String(0) {"<primitiveGroup>"}
        Dim sp1 = TheXML_String.Split(delim, StringSplitOptions.None)
        If loop_count > sp1.Length - 1 Then
            MsgBox("We have a problem!" + vbCrLf + "There are more models than entries in the Visual." +
                   vbCrLf + "I can load them but with not texture info!")
            Return ""
        End If

        Return get_textures_and_names(id, sp1(loop_count))
        Return Nothing
    End Function

    '=======================================

    Public Function get_textures_and_names(ByVal id As Integer, ByVal thestring As String) As Boolean
        'the old method sucks so.. im redoing it.. for the 3rd time!
        'log_text.append( " Id: " + id.ToString + " Searching .Visual for Texture paths." + vbCrLf
        Dim primStart As Integer = 1
        _group(id).g_tile0Tint.x = 1.0!
        _group(id).g_tile0Tint.y = 1.0!
        _group(id).g_tile0Tint.z = 1.0!
        _group(id).g_tile0Tint.w = 1.0!

        _group(id).g_tile1Tint.x = 1.0!
        _group(id).g_tile1Tint.y = 1.0!
        _group(id).g_tile1Tint.z = 1.0!
        _group(id).g_tile1Tint.w = 1.0!

        _group(id).g_tile2Tint.x = 1.0!
        _group(id).g_tile2Tint.y = 1.0!
        _group(id).g_tile2Tint.z = 1.0!
        _group(id).g_tile2Tint.w = 1.0!

        _group(id).g_dirtColor.x = 1.0!
        _group(id).g_dirtColor.y = 1.0!
        _group(id).g_dirtColor.z = 1.0!
        _group(id).g_dirtColor.w = 1.0!

        _group(id).g_dirtParams.x = 1.0!
        _group(id).g_dirtParams.y = 1.0!
        _group(id).g_dirtParams.z = 1.0!
        _group(id).g_dirtParams.w = 0.0!
        _group(id).g_atlas_size.x = 0
        _group(id).g_atlas_size.y = 0
        _group(id).g_atlas_size.z = 0
        _group(id).g_atlas_size.w = 0
        _group(id).g_atlas_indexs.x = 0
        _group(id).g_atlas_indexs.y = 0
        _group(id).g_atlas_indexs.z = 0
        _group(id).g_atlas_indexs.w = 0

        'so we dont use the diffusedmap2 as diffused in search
        thestring = thestring.Replace("diffuseMap2", "difffuseMap2")
        If thestring.ToLower.Contains("texture=") Then
            log_text.Append("Texture XML format is non-standard" + vbCrLf)
            Dim sr As New StringReader(thestring)
            Dim li As String = ""
            While (True)
                li = sr.ReadLine
                If li Is Nothing Then
                    Exit While
                End If
                If li.Contains("diffuseMap") Then
                    Dim a1 = li.Split("""")
                    _group(id).color_name = a1(1).Replace(".tga", ".dds")
                    _group(id).color_Id = -1
                End If
                If li.Contains("difffuseMap2") Then
                    Dim a1 = li.Split("""")
                    _group(id).detail_name = a1(1).Replace(".tga", ".dds")
                    _group(id).detail_Id = -1
                End If
                If li.Contains("normalMap") Then
                    Dim a1 = li.Split("""")
                    _group(id).normal_name = a1(1).Replace(".tga", ".dds")
                    _group(id).normal_Id = -1
                    _group(id).bumped = True
                End If
            End While
            Return False
        End If
        Dim diff_pos As Integer
        '===================================================================================
        'July 14th, 2019.. Started on stand alone primtive loading
        diff_pos = InStr(primStart, thestring, "atlasNormalGlossSpec")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<Texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).atlasNormalGlossSpec = newS
        End If

        diff_pos = InStr(primStart, thestring, "atlasMetallicAO")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<Texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).atlasMetallicAO = newS
        End If

        diff_pos = InStr(primStart, thestring, "atlasBlend")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<Texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).atlasBlend = newS
        End If

        diff_pos = InStr(primStart, thestring, "atlasAlbedoHeight")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<Texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            If Not newS.Contains("atlas_processed") Then newS = newS.Replace(".atlas", ".atlas_processed")
            _group(id).atlasAlbedoHeight = newS
        End If

        diff_pos = InStr(primStart, thestring, "dirtMap")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<Texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).dirtMap = newS
        End If

        diff_pos = InStr(primStart, thestring, "g_detailMap")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<Texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).g_detailMap = newS
        End If

        '------------- get sizes, colors and indexes
        diff_pos = InStr(primStart, thestring, "g_tile2Tint")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_tile2Tint.x = Convert.ToSingle(ta(0))
            _group(id).g_tile2Tint.y = Convert.ToSingle(ta(1))
            _group(id).g_tile2Tint.z = Convert.ToSingle(ta(2))
            _group(id).g_tile2Tint.w = Convert.ToSingle(ta(3))
        End If

        diff_pos = InStr(primStart, thestring, "g_tile1Tint")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_tile1Tint.x = Convert.ToSingle(ta(0))
            _group(id).g_tile1Tint.y = Convert.ToSingle(ta(1))
            _group(id).g_tile1Tint.z = Convert.ToSingle(ta(2))
            _group(id).g_tile1Tint.w = Convert.ToSingle(ta(3))
        End If

        diff_pos = InStr(primStart, thestring, "g_tile0Tint")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_tile0Tint.x = Convert.ToSingle(ta(0))
            _group(id).g_tile0Tint.y = Convert.ToSingle(ta(1))
            _group(id).g_tile0Tint.z = Convert.ToSingle(ta(2))
            _group(id).g_tile0Tint.w = Convert.ToSingle(ta(3))
        End If

        diff_pos = InStr(primStart, thestring, "g_atlasSizes")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_atlas_size.x = Convert.ToSingle(ta(0))
            _group(id).g_atlas_size.y = Convert.ToSingle(ta(1))
            _group(id).g_atlas_size.z = Convert.ToSingle(ta(2))
            _group(id).g_atlas_size.w = Convert.ToSingle(ta(3))
        End If

        diff_pos = InStr(primStart, thestring, "g_atlasIndexes")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_atlas_indexs.x = Convert.ToSingle(ta(0))
            _group(id).g_atlas_indexs.y = Convert.ToSingle(ta(1))
            _group(id).g_atlas_indexs.z = Convert.ToSingle(ta(2))
            _group(id).g_atlas_indexs.w = Convert.ToSingle(ta(3))
        End If

        diff_pos = InStr(primStart, thestring, "g_dirtColor")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_dirtColor.x = Convert.ToSingle(ta(0))
            _group(id).g_dirtColor.y = Convert.ToSingle(ta(1))
            _group(id).g_dirtColor.z = Convert.ToSingle(ta(2))
            _group(id).g_dirtColor.w = Convert.ToSingle(ta(3))
        End If

        diff_pos = InStr(primStart, thestring, "g_dirtParams")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_dirtParams.x = Convert.ToSingle(ta(0))
            _group(id).g_dirtParams.y = Convert.ToSingle(ta(1))
            _group(id).g_dirtParams.z = Convert.ToSingle(ta(2))
            _group(id).g_dirtParams.w = Convert.ToSingle(ta(3))
        End If

        diff_pos = InStr(primStart, thestring, "g_detailRejectTiling")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_detailRejectTiling.x = Convert.ToSingle(ta(0))
            _group(id).g_detailRejectTiling.y = Convert.ToSingle(ta(1))
            _group(id).g_detailRejectTiling.z = Convert.ToSingle(ta(2))
            _group(id).g_detailRejectTiling.w = Convert.ToSingle(ta(3))
        End If

        diff_pos = InStr(primStart, thestring, "g_detailInfluences")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ta = newS.Split(" ")
            _group(id).g_detailInfluences.x = Convert.ToSingle(ta(0))
            _group(id).g_detailInfluences.y = Convert.ToSingle(ta(1))
            _group(id).g_detailInfluences.z = Convert.ToSingle(ta(2))
            _group(id).g_detailInfluences.w = Convert.ToSingle(ta(3))
        End If

        ' Atlas or Skinned?
        diff_pos = InStr(primStart, thestring, "<fx>")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<fx>") + "<fx>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</fx>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            If newS.Contains("skinned") Then
                _group(id).skinned = True
                _group(id).doubleSided = True ' skinned models have backwards vertex winding order
            End If
            If newS.Contains("PBS_tiled_atlas") Then
                _group(id).is_atlas_type = 1
            End If
            If newS.Contains("PBS_ext_dual") Then
                _group(id).use_uv2 = 1
            End If
            If newS.Contains("PBS_ext_detail") Then
                _group(id).is_detail_type = 1
            End If
            If newS.Contains("PBS_glass.fx") Then
                _group(id).is_glass = 1
            End If

        End If

        '===================================================================================
        diff_pos = InStr(primStart, thestring, "g_detailUVTiling")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Vector4>") + "<Vector4>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Vector4>")
            Dim newS As String = ""
            If tex1_Epos = 0 Then
                tex1_pos = InStr(diff_pos, thestring, "<Vector2>") + "<Vector2>".Length
                tex1_Epos = InStr(tex1_pos, thestring, "</Vector2>")
            End If
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")

            Dim ar = newS.Split(" ")
            _group(id).detail_tile.x = CSng(ar(0))
            _group(id).detail_tile.y = CSng(ar(1))
        Else
            _group(id).detail_tile.x = 1.0
            _group(id).detail_tile.y = 1.0

        End If
        diff_pos = InStr(primStart, thestring, "metallicDetailMap")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).detail_name = newS
            _group(id).detail_Id = -1
        End If

        diff_pos = InStr(primStart, thestring, "diffuseMap")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            If newS = exclusionMask_name Then
                GLOBAL_exclusionMask = 0
            End If
            _group(id).color_name = newS.Replace("tga", "dds")
            _group(id).color_Id = -1
        End If

        diff_pos = InStr(primStart, thestring, "dirtAlbedoMap")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            If newS = exclusionMask_name Then
                GLOBAL_exclusionMask = 0
            End If
            _group(id).color_name = newS.Replace("tga", "dds")
            _group(id).color_Id = -1
        End If

        diff_pos = InStr(primStart, thestring, "difffuseMap2")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).detail_name = newS.Replace("tga", "dds")
            'Debug.Write(newS & vbCrLf)
            _group(id).detail_Id = -1
        End If
        diff_pos = InStr(primStart, thestring, "excludeMaskAndAOMap")
        If diff_pos > 0 Then
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).ao_name = newS.Replace("tga", "dds")
            _group(id).ao_id = -1
        End If

        diff_pos = InStr(primStart, thestring, "normalMap")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).normal_name = newS.Replace("tga", "dds")
            If InStr(newS, "_ANM") > 0 Then
                _object(id).ANM = 1
                _group(id).ANM = 1
            Else
                _object(id).ANM = 0
                _group(id).ANM = 0
            End If
            _group(id).bumped = True
            _group(id).normal_Id = -1
            _group(id).use_normapMap = 1
        Else
            _group(id).bumped = False
        End If

        diff_pos = InStr(primStart, thestring, "metallicGlossMap")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).metalGMM_name = newS
            _group(id).metal_textured = True
            _group(id).metalGMM_Id = -1
        End If

        diff_pos = InStr(primStart, thestring, "glassMap")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).metalGMM_name = newS
            _group(id).metal_textured = True
            _group(id).metalGMM_Id = -1
        End If

        diff_pos = InStr(primStart, thestring, "specularMap") 'reusing the metal texture as specMap
        If diff_pos > 0 Then
            HD_TANK = False
            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).metalGMM_name = newS
            _group(id).metal_textured = False
            _group(id).metalGMM_Id = -1
        End If


        diff_pos = InStr(primStart, thestring, "alphaReference<")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Int>") + "<Int>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Int>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ref = Convert.ToInt32(newS)

            _group(id).alphaRef = ref
        Else
            _group(id).alphaRef = 0
        End If

        diff_pos = InStr(primStart, thestring, "alphaTestEnable<")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Bool>") + "<Bool>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Bool>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            Dim ref As Integer = 0
            If newS = "true" Then
                ref = 1
            End If
            _group(id).alphaTest = ref
        Else
            _group(id).alphaTest = 0
            '_group(id).alphaRef = 0 '?


        End If
        diff_pos = InStr(primStart, thestring, "g_detailPower<")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Float>") + "<Float>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Float>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            _group(id).detail_power = CSng(newS)
        Else
            _group(id).detail_power = 0
        End If

        diff_pos = InStr(primStart, thestring, "doubleSided<")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Bool>") + "<Bool>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Bool>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            'Debug.WriteLine(newS)
            If Not _group(id).doubleSided Then
                If newS = "true" Then
                    _group(id).doubleSided = True
                Else
                    _group(id).doubleSided = False
                End If
            End If

        Else
            _group(id).doubleSided = 0
        End If

        diff_pos = InStr(primStart, thestring, "colorIdMap<")
        If diff_pos > 0 Then

            Dim tex1_pos = InStr(diff_pos, thestring, "<Texture>") + "<texture>".Length
            Dim tex1_Epos = InStr(tex1_pos, thestring, "</Texture>")
            Dim newS As String = ""
            newS = Mid(thestring, tex1_pos, tex1_Epos - tex1_pos).Replace("/", "\")
            'Debug.WriteLine(newS)
            _group(id).colorIDmap = newS
            _group(id).hasColorID = 1
        Else
            _group(id).hasColorID = 0
        End If
        Return True

    End Function
    '=======================================

    Public Function unpackNormal(ByVal packed As UInt32)
        Dim pkz, pky, pkx As Int32
        pkz = packed And &HFFC00000
        pky = packed And &H4FF800
        pkx = packed And &H7FF

        Dim z As Int32 = pkz >> 22
        Dim y As Int32 = (pky << 10L) >> 21
        Dim x As Int32 = (pkx << 21L) >> 21
        Dim p As New vect3
        p.x = CSng(x) / 1023.0! '* -1.0!

        p.x = CSng(x) / 1023.0!
        p.y = CSng(y) / 1023.0!
        p.z = CSng(z) / 511.0!
        Dim len As Single = Sqrt((p.x ^ 2) + (p.y ^ 2) + (p.z ^ 2))

        'avoid division by 0
        If len = 0.0F Then len = 1.0F

        'reduce to unit size
        p.x = (p.x / len)
        p.y = (p.y / len)
        p.z = (p.z / len)
        Return p
    End Function
    Private Function unpackNormal_8_8_8(ByVal packed As UInt32) As vect3
        'Console.WriteLine(packed.ToString("x"))
        Dim pkz, pky, pkx As Int32
        'Dim sample As Byte
        pkx = CLng(packed) And &HFF Xor 127
        'sample = packed And &HFF
        pky = CLng(packed >> 8) And &HFF Xor 127
        pkz = CLng(packed >> 16) And &HFF Xor 127

        Dim x As Single = (pkx)
        Dim y As Single = (pky)
        Dim z As Single = (pkz)

        Dim p As New vect3
        If x > 127 Then
            x = -128 + (x - 128)
        End If
        'lookup(CInt(x + 127)) = sample

        If y > 127 Then
            y = -128 + (y - 128)
        End If
        If z > 127 Then
            z = -128 + (z - 128)
        End If
        p.x = CSng(x) / 127
        p.y = CSng(y) / 127
        p.z = CSng(z) / 127
        Dim len As Single = Sqrt((p.x ^ 2) + (p.y ^ 2) + (p.z ^ 2))

        'avoid division by 0
        If len = 0.0F Then len = 1.0F
        'len = 1.0
        'reduce to unit size
        p.x = -(p.x / len)
        p.y = -(p.y / len)
        p.z = -(p.z / len)
        Return p
    End Function
    Public Sub make_lists(I As Integer)
        Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)

        Gl.glDeleteLists(_object(I).main_display_list, 1)
        _object(I).main_display_list = Gl.glGenLists(1)
        Gl.glNewList(_object(I).main_display_list, Gl.GL_COMPILE)
        main_list(_object(I).count, I)
        Gl.glEndList()
        MODEL_LOADED = True
        Dim e = Gl.glGetError

    End Sub
    Public max_u, max_v, min_u, min_v As Single
    Private Sub get_uv_repetes(ByVal jj As Integer, i As Integer)
        Dim v1 = _object(jj).tris(i).uv1
        Dim v2 = _object(jj).tris(i).uv2
        Dim v3 = _object(jj).tris(i).uv3
        If v1.u > max_u Then max_u = v1.u
        If v2.u > max_u Then max_u = v2.u
        If v3.u > max_u Then max_u = v3.u

        If v1.u < min_u Then min_u = v1.u
        If v2.u < min_u Then min_u = v2.u
        If v3.u < min_u Then min_u = v3.u

        If v1.v > max_v Then max_v = v1.v
        If v2.v > max_v Then max_v = v2.v
        If v3.v > max_v Then max_v = v3.v

        If v1.v < min_v Then min_v = v1.v
        If v2.v < min_v Then min_v = v2.v
        If v3.v < min_v Then min_v = v3.v

    End Sub
    Private Sub main_list(ByVal cnt As UInteger, ByVal jj As UInt32)
        min_u = 10000.0!
        min_v = 10000.0!
        Gl.glBegin(Gl.GL_TRIANGLES)
        'trans_vertex(jj)
        For i As UInt32 = 1 To cnt
            pre_transform(jj, i)
            '1
            Gl.glNormal3f(_object(jj).tris(i).n1.x, _object(jj).tris(i).n1.y, _object(jj).tris(i).n1.z) 'normal
            Gl.glMultiTexCoord2f(0, _object(jj).tris(i).uv1.u, _object(jj).tris(i).uv1.v) 'uv1
            Gl.glMultiTexCoord3f(1, _object(jj).tris(i).t1.x, _object(jj).tris(i).t1.y, _object(jj).tris(i).t1.z) 'tangent
            Gl.glMultiTexCoord3f(2, _object(jj).tris(i).b1.x, _object(jj).tris(i).b1.y, _object(jj).tris(i).b1.z) ' bitangent
            Gl.glMultiTexCoord2f(4, _object(jj).tris(i).uv1_2.u, _object(jj).tris(i).uv1_2.v) 'uv2
            'Gl.glMultiTexCoord4f(5, _object(jj).tris(i).c1.x, _object(jj).tris(i).c1.y, _object(jj).tris(i).c1.z, _object(jj).tris(i).c1.w) 'color
            'must have these for AMD!!!
            Gl.glTexCoord2f(_object(jj).tris(i).uv1.u, _object(jj).tris(i).uv1.v)
            'Gl.glMultiTexCoord3f(3, _object(jj).tris(i).color1.x, _object(jj).tris(i).color1.y, _object(jj).tris(i).color1.z) 'color

            Gl.glVertex3f(_object(jj).tris(i).v1.x, _object(jj).tris(i).v1.y, _object(jj).tris(i).v1.z) 'vertex
            '2
            Gl.glNormal3f(_object(jj).tris(i).n2.x, _object(jj).tris(i).n2.y, _object(jj).tris(i).n2.z)
            Gl.glMultiTexCoord2f(0, _object(jj).tris(i).uv2.u, _object(jj).tris(i).uv2.v) 'uv1
            Gl.glMultiTexCoord3f(1, _object(jj).tris(i).t2.x, _object(jj).tris(i).t2.y, _object(jj).tris(i).t2.z)
            Gl.glMultiTexCoord3f(2, _object(jj).tris(i).b2.x, _object(jj).tris(i).b2.y, _object(jj).tris(i).b2.z)
            Gl.glMultiTexCoord2f(4, _object(jj).tris(i).uv2_2.u, _object(jj).tris(i).uv2_2.v) 'uv2
            'Gl.glMultiTexCoord4f(5, _object(jj).tris(i).c2.x, _object(jj).tris(i).c2.y, _object(jj).tris(i).c2.z, _object(jj).tris(i).c2.w) 'color
            'must have these for AMD!!!
            Gl.glTexCoord2f(_object(jj).tris(i).uv2.u, _object(jj).tris(i).uv2.v)

            'Gl.glMultiTexCoord3f(3, _object(jj).tris(i).color2.x, _object(jj).tris(i).color2.y, _object(jj).tris(i).color2.z) 'color

            Gl.glVertex3f(_object(jj).tris(i).v2.x, _object(jj).tris(i).v2.y, _object(jj).tris(i).v2.z)
            '3
            Gl.glNormal3f(_object(jj).tris(i).n3.x, _object(jj).tris(i).n3.y, _object(jj).tris(i).n3.z)
            Gl.glMultiTexCoord2f(0, _object(jj).tris(i).uv3.u, _object(jj).tris(i).uv3.v) 'uv1
            Gl.glMultiTexCoord3f(1, _object(jj).tris(i).t3.x, _object(jj).tris(i).t3.y, _object(jj).tris(i).t3.z)
            Gl.glMultiTexCoord3f(2, _object(jj).tris(i).b3.x, _object(jj).tris(i).b3.y, _object(jj).tris(i).b3.z)
            Gl.glMultiTexCoord2f(4, _object(jj).tris(i).uv3_2.u, _object(jj).tris(i).uv3_2.v) 'uv2
            'Gl.glMultiTexCoord4f(5, _object(jj).tris(i).c3.x, _object(jj).tris(i).c3.y, _object(jj).tris(i).c3.z, _object(jj).tris(i).c3.w) 'color
            'must have these for AMD!!!
            Gl.glTexCoord2f(_object(jj).tris(i).uv3.u, _object(jj).tris(i).uv3.v)

            'Gl.glMultiTexCoord3f(3, _object(jj).tris(i).color3.x, _object(jj).tris(i).color3.y, _object(jj).tris(i).color3.z) 'color

            Gl.glVertex3f(_object(jj).tris(i).v3.x, _object(jj).tris(i).v3.y, _object(jj).tris(i).v3.z)
            get_uv_repetes(jj, i)
        Next
        Dim x_s = max_u - min_u
        Dim y_s = max_v - min_v
        _group(jj).x_repete = x_s
        _group(jj).y_repete = y_s
        Gl.glEnd()
    End Sub

    Public Function transform_vector3(ByVal v As SlimDX.Vector3, ByVal m() As Double) As SlimDX.Vector3
        Dim vo As SlimDX.Vector3
        vo.X = (m(0) * v.X) + (m(4) * v.Y) + (m(8) * v.Z)
        vo.Y = (m(1) * v.X) + (m(5) * v.Y) + (m(9) * v.Z)
        vo.Z = (m(2) * v.X) + (m(6) * v.Y) + (m(10) * v.Z)

        vo.X += m(12)
        vo.Y += m(13)
        vo.Z += m(14)
        vo.X *= -1.0
        Return vo

    End Function

    Private Sub pre_transform(ByVal jj As Integer, ByVal i As Integer)
        _object(jj).tris(i).v1 = transform(_object(jj).tris(i).v1, _object(jj).matrix)
        _object(jj).tris(i).v2 = transform(_object(jj).tris(i).v2, _object(jj).matrix)
        _object(jj).tris(i).v3 = transform(_object(jj).tris(i).v3, _object(jj).matrix)

        _object(jj).tris(i).n1 = rotate_transform(_object(jj).tris(i).n1, _object(jj).matrix)
        _object(jj).tris(i).n2 = rotate_transform(_object(jj).tris(i).n2, _object(jj).matrix)
        _object(jj).tris(i).n3 = rotate_transform(_object(jj).tris(i).n3, _object(jj).matrix)

        _object(jj).tris(i).t1 = rotate_transform(_object(jj).tris(i).t1, _object(jj).matrix)
        _object(jj).tris(i).t2 = rotate_transform(_object(jj).tris(i).t2, _object(jj).matrix)
        _object(jj).tris(i).t3 = rotate_transform(_object(jj).tris(i).t3, _object(jj).matrix)

        _object(jj).tris(i).b1 = rotate_transform(_object(jj).tris(i).b1, _object(jj).matrix)
        _object(jj).tris(i).b2 = rotate_transform(_object(jj).tris(i).b2, _object(jj).matrix)
        _object(jj).tris(i).b3 = rotate_transform(_object(jj).tris(i).b3, _object(jj).matrix)



    End Sub

    Public Function transform(ByVal v As vect3, ByVal m() As Double) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)
        vo.x *= -1.0
        Return vo

    End Function



    Public Function rotate_scale_translate_transform(ByVal v As vect3, ByVal m() As Double) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.x *= -1.0
        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)
        Return vo

    End Function
    Public Function rotate_transform(ByVal v As vect3, ByVal m() As Double) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.x *= -1.0
        'vo.x += m(12)
        'vo.y += m(13)
        'vo.z += m(14)
        Return vo

    End Function

    Public Function gun_new_transform(ByVal v As vect3, ByVal m() As Double) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)
        'vo = v
        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)
        vo.x *= -1.0
        vo.z *= -1.0
        Return vo

    End Function
    Public Function gun_new_rotate_transform(ByVal v As vect3, ByVal m() As Double) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)
        'vo = v
        'vo.y += m(12)
        'vo.y += m(13)
        'vo.z += m(14)
        vo.x *= -1.0
        vo.z *= -1.0
        Dim l = Sqrt(vo.x ^ 2 + vo.y ^ 2 + vo.z ^ 2)
        If l = 0.0 Then l = 1.0
        vo.x /= l
        vo.y /= l
        vo.z /= l

        Return vo

    End Function


End Module
