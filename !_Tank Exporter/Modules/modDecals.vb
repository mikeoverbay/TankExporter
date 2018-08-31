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
Imports System.Runtime.Serialization.Formatters.Binary

#End Region



Module modDecals
    Public current_decal As Integer = -1
    Public picked_decal As Integer = 0

    Public decal_order() As Integer

    Public decal_textures() As decal_texture_
    Public Structure decal_texture_
        Public full_path As String
        Public colorMap_name As String
        Public colorMap_Id As Integer
        Public normalMap_Id As Integer
        Public gmmMap_id As Integer
    End Structure


    Public decal_matrix_list() As decal_matrix_list_
    <Serializable> Public Structure decal_matrix_list_
        Public alpha As Single
        Public level As Single
        Public scale As vect3
        Public translate As vect3
        Public rotation As vect3
        Public u_wrap As Single
        Public v_wrap As Single
        Public uv_rot As Single
        Public u_wrap_index As Integer
        Public v_wrap_index As Integer
        Public uv_rot_index As Integer

        Public texture_id As Integer
        Public normal_id As Integer
        Public gmm_id As Integer
        Public decal_texture As String

        Public display_matrix() As Single
        Public y_rotate_matrix() As Single
        Public x_rotate_matrix() As Single
        Public z_rotate_matrix() As Single
        Public scale_matrix() As Single
        Public translate_matrix() As Single

        Public lbl As vect3
        Public lbr As vect3
        Public ltl As vect3
        Public ltr As vect3
        Public rbl As vect3
        Public rbr As vect3
        Public rtl As vect3
        Public rtr As vect3
        Public pi2 As Single

        Public Sub get_decals_transform_info()
            g_decal_scale = Me.scale
            g_decal_translate = Me.translate
            g_decal_rotate = Me.rotation
            frmMain.decal_alpha_slider.Value = Int(100 * Me.alpha)
            frmMain.decal_level_slider.Value = Int(100 * Me.level)
            frmMain.Uwrap.SelectedIndex = Me.u_wrap_index
            frmMain.Vwrap.SelectedIndex = Me.v_wrap_index
            frmMain.uv_rotate.SelectedIndex = Me.uv_rot_index
            frmMain.current_decal_lable.Text = current_decal
            look_point_x = Me.translate.x
            look_point_y = Me.translate.y
            look_point_z = Me.translate.z
            frmMain.d_texture_name.Text = Me.decal_texture

            Dim s = Sin(Me.rotation.x)
            Dim c = Cos(Me.rotation.x)
            Me.y_rotate_matrix = { _
              c, 0.0, -s, 0.0, _
              0.0, 1.0, 0.0, 0.0, _
              s, 0.0, c, 0.0, _
              0.0, 0.0, 0.0, 1.0}
            s = Sin(Me.rotation.y)
            c = Cos(Me.rotation.y)
            Me.x_rotate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, c, s, 0.0, _
                0.0, -s, c, 0.0, _
                0.0, 0.0, 0.0, 1.0}

            s = Sin(Me.rotation.z)
            c = Cos(Me.rotation.z)
            Me.z_rotate_matrix = { _
                c, s, 0.0, 0.0, _
                -s, c, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}

            Dim ss = Me.scale
            Me.scale_matrix = { _
                ss.x, 0.0, 0.0, 0.0, _
                0.0, ss.y, 0.0, 0.0, _
                0.0, 0.0, ss.z, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            Dim v = Me.translate
            Me.translate_matrix(12) = v.x
            Me.translate_matrix(13) = v.y
            Me.translate_matrix(14) = v.z
        End Sub

        Public Sub load_identity()
            'set some vaules when this decal is created
            Me.alpha = 1.0
            Me.level = 1.0
            pi2 = PI * 2.0
            Me.scale.x = 1.0
            Me.scale.y = 1.0
            Me.scale.z = 1.0
            Me.u_wrap = 1.0
            Me.v_wrap = 1.0
            Me.uv_rot = 0
            Me.u_wrap_index = 4
            Me.v_wrap_index = 4
            Me.uv_rot_index = 4

            Me.translate.x = 0.0
            Me.translate.y = 0.0
            Me.translate.z = 0.0

            g_decal_rotate.x = PI / 2.0
            g_decal_rotate.y = 0.0
            If Sqrt(scale.x ^ 2 + scale.y ^ 2 + scale.z ^ 2) = 0.0 Then
                g_decal_scale.x = 1.0
                g_decal_scale.y = 1.0
                g_decal_scale.z = 1.0

            End If
            display_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            x_rotate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            y_rotate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            z_rotate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            scale_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            translate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, 1.0, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                U_look_point_x, U_look_point_y, U_look_point_z, 1.0} ' position where we are looking
            'preset translation to where we are looking.
            g_decal_translate.x = U_look_point_x
            g_decal_translate.y = U_look_point_y
            g_decal_translate.z = U_look_point_z
            Me.set_x_rotation_matrix(PI / 2.0)
            Me.set_y_rotation_matrix(0.0)
            Me.set_z_rotation_matrix(0.0)
            Me.translate = g_decal_translate
        End Sub
        Public Sub transform()
            Gl.glPushMatrix()
            Gl.glLoadIdentity()
            Gl.glMultMatrixf(Me.translate_matrix)
            Gl.glMultMatrixf(Me.y_rotate_matrix)
            Gl.glMultMatrixf(Me.z_rotate_matrix)
            Gl.glMultMatrixf(Me.x_rotate_matrix)

            Gl.glMultMatrixf(Me.scale_matrix)
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, Me.display_matrix)
            Gl.glPopMatrix()
        End Sub

        Public Function set_y_rotation_matrix(x As Single)
            Me.rotation.x += x
            If Me.rotation.x > pi2 Then
                Me.rotation.x -= pi2
            End If
            If Me.rotation.x < -pi2 Then
                Me.rotation.x += pi2
            End If
            Dim s = Sin(Me.rotation.x)
            Dim c = Cos(Me.rotation.x)
            Me.y_rotate_matrix = { _
              c, 0.0, -s, 0.0, _
              0.0, 1.0, 0.0, 0.0, _
              s, 0.0, c, 0.0, _
              0.0, 0.0, 0.0, 1.0}
            Return Me.y_rotate_matrix
        End Function
        Public Function set_x_rotation_matrix(y As Single)
            Me.rotation.y += y
            If Me.rotation.y > pi2 Then
                Me.rotation.y -= pi2
            End If
            If Me.rotation.y < -pi2 Then
                Me.rotation.y += pi2
            End If
            Dim s = Sin(Me.rotation.y)
            Dim c = Cos(Me.rotation.y)
            Me.x_rotate_matrix = { _
                1.0, 0.0, 0.0, 0.0, _
                0.0, c, s, 0.0, _
                0.0, -s, c, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            Return Me.x_rotate_matrix
        End Function
        Public Function set_z_rotation_matrix(z As Single)
            Me.rotation.z += z
            If Me.rotation.z > pi2 Then
                Me.rotation.z -= pi2
            End If
            If Me.rotation.z < -pi2 Then
                Me.rotation.z += pi2
            End If
            Dim s = Sin(Me.rotation.z)
            Dim c = Cos(Me.rotation.z)
            Me.z_rotate_matrix = { _
                c, s, 0.0, 0.0, _
                -s, c, 0.0, 0.0, _
                0.0, 0.0, 1.0, 0.0, _
                0.0, 0.0, 0.0, 1.0}
            Return Me.z_rotate_matrix
        End Function
        Public Function set_scale_matrix(s As vect3)
            If s.x < 0.1 Then s.x = 0.1
            If s.y < 0.1 Then s.y = 0.1
            If s.z < 0.1 Then s.z = 0.1

            Me.scale = s
            Me.scale_matrix = { _
                s.x, 0.0, 0.0, 0.0, _
                0.0, s.y, 0.0, 0.0, _
                0.0, 0.0, s.z, 0.0, _
                0.0, 0.0, 0.0, 1.0}

            Return Me.scale_matrix
        End Function

        Public Sub set_translate_matrix(id As Integer, v As vect3)
            Me.translate = v
            Me.translate_matrix(12) = v.x
            Me.translate_matrix(13) = v.y
            Me.translate_matrix(14) = v.z
        End Sub
    End Structure
    Public Structure vertex_data
        Public x As Single
        Public y As Single
        Public z As Single
        Public u As Single
        Public v As Single
        Public nx As Single
        Public ny As Single
        Public nz As Single
        Public map As Integer
        Public t As vect3
        Public bt As vect3
    End Structure

    Public decal_draw_box As Integer
    Public Sub update_decal_order()
        Dim lc = decal_matrix_list.Length - 2
        Try

            ReDim decal_order(lc)
            For i = 0 To lc
                frmMain.d_list_tb.SelectionStart = frmMain.d_list_tb.GetFirstCharIndexFromLine(i)
                frmMain.d_list_tb.Select(frmMain.d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                     frmMain.d_list_tb.Lines(i).Length) ' select line
                Dim ar = frmMain.d_list_tb.SelectedText.Split(":")
                decal_order(i) = CInt(ar(1))
            Next
        Catch ex As Exception

        End Try
    End Sub
    Public Sub add_decal()
        Dim id As Integer = 0
        If decal_matrix_list Is Nothing Then
            id = 0
            ReDim Preserve decal_matrix_list(id + 1)
        Else
            id = decal_matrix_list.Length - 1
            ReDim Preserve decal_matrix_list(id + 1)
        End If
        decal_matrix_list(id) = New decal_matrix_list_
        current_decal = id
        decal_matrix_list(id).load_identity()
        decal_matrix_list(id).get_decals_transform_info()

        frmMain.d_list_tb.Text += "Decal ID :" + id.ToString + vbCrLf
        update_decal_order()
        frmMain.d_current_line = current_decal
        If id = 0 Then ' if we have a draw box already, use it...
            '0therwise, create the draw box in first decal.
            With decal_matrix_list(id)
                Gl.glDeleteLists(decal_draw_box, 1)
                decal_draw_box = Gl.glGenLists(1)
                Gl.glNewList(decal_draw_box, Gl.GL_COMPILE)
                get_box_corners(id, 0.5) ' creates coordinates

                Gl.glBegin(Gl.GL_QUADS)
                make_decal_box(id) ' draws the box
                Gl.glEnd()
                Gl.glEndList()
            End With
        End If
    End Sub

    Private Sub make_decal_box(ByVal decal As Integer)
        With decal_matrix_list(decal)
            '1 right
            Gl.glNormal3f(1.0, 0.0, 0.0)
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)
            '2 back
            Gl.glNormal3f(0.0, 0.0, -1.0)
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            '3 left
            Gl.glNormal3f(-1.0, 0.0, 0.0)
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            '4 front
            Gl.glNormal3f(0.0, 0.0, 1.0)
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            '5 top
            Gl.glNormal3f(0.0, 1.0, 0.0)
            Gl.glVertex3f(.rtr.x, .rtr.y, .rtr.z)
            Gl.glVertex3f(.ltr.x, .ltr.y, .ltr.z)
            Gl.glVertex3f(.ltl.x, .ltl.y, .ltl.z)
            Gl.glVertex3f(.rtl.x, .rtl.y, .rtl.z)
            '6 bottom
            Gl.glNormal3f(0.0, -1.0, 0.0)
            Gl.glVertex3f(.rbl.x, .rbl.y, .rbl.z)
            Gl.glVertex3f(.lbl.x, .lbl.y, .lbl.z)
            Gl.glVertex3f(.lbr.x, .lbr.y, .lbr.z)
            Gl.glVertex3f(.rbr.x, .rbr.y, .rbr.z)


        End With

    End Sub

    Private Sub get_box_corners(ByVal decal As Integer, ByVal z_scale As Single)
        With decal_matrix_list(decal)
            ' left side -----------
            .lbl.x = -0.5 'left bottom left
            .lbl.y = -0.5
            .lbl.z = -z_scale
            '
            .lbr.x = 0.5 ' left bottom right
            .lbr.y = -0.5
            .lbr.z = -z_scale
            '
            .ltl.x = -0.5 'left top left
            .ltl.y = 0.5
            .ltl.z = -z_scale
            '
            .ltr.x = 0.5 ' left top right
            .ltr.y = 0.5
            .ltr.z = -z_scale
            ' right side ----------
            .rbl.x = -0.5 ' right bottom left
            .rbl.y = -0.5
            .rbl.z = z_scale
            '
            .rbr.x = 0.5 ' right bottom right
            .rbr.y = -0.5
            .rbr.z = z_scale
            '
            .rtl.x = -0.5 ' right top left
            .rtl.y = 0.5
            .rtl.z = z_scale
            '
            .rtr.x = 0.5 ' right top right
            .rtr.y = 0.5
            .rtr.z = z_scale

        End With
    End Sub
    Public Sub load_this_Decal(ByVal j As Integer)
        If decal_textures(j).colorMap_Id = 0 Then
            Dim name As String = decal_textures(j).full_path
            decal_textures(j).colorMap_Id = load_dds_file(name)
            Dim ts = name.Replace("_AM.dds", "_NM.dds")
            decal_textures(j).normalMap_Id = load_dds_file(ts)
            ts = name.Replace("_AM.dds", "_GMM.dds")
            decal_textures(j).gmmMap_id = load_dds_file(ts)
        End If



    End Sub
    Public Sub load_decal_textures()
        Dim dPath As String = decal_path + "\maps\decals_pbs\"
        Dim dir_info = Directory.GetFiles(dPath)
        Dim f_cnt = dir_info.Count
        Dim c_names(f_cnt) As String
        Dim c_c As Integer

        For Each f In dir_info
            If Not f.ToLower.Contains("nm.d") And Not f.ToLower.Contains("gmm.d") Then
                c_names(c_c) = f
                c_c += 1
            End If
        Next
        ReDim decal_textures(c_c - 1)
        Dim ts As String = ""
        For j = 0 To c_c - 1
            If File.Exists(c_names(j).Replace("_AM.dds", "_GMM.dds")) Then
                decal_textures(j) = New decal_texture_
                decal_textures(j).full_path = c_names(j)
                decal_textures(j).colorMap_name = Path.GetFileNameWithoutExtension(c_names(j))
                'decal_textures(j).colorMap_Id = load_dds_file(c_names(j))
                'ts = c_names(j).Replace("_AM.dds", "_NM.dds")
                ' decal_textures(j).normalMap_Id = load_dds_file(ts)
                'ts = c_names(j).Replace("_AM.dds", "_GMM.dds")
                'decal_textures(j).gmmMap_id = load_dds_file(ts)
            Else
                Try
                    File.Delete(c_names(j))
                Catch ex As Exception
                End Try
                Try
                    File.Delete(c_names(j).Replace("_AM.dds", "_NM.dds"))
                Catch ex As Exception
                End Try
            End If
        Next

    End Sub

    Public Function rotate_only(ByVal v As vect3, ByVal m() As Single) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        Return vo

    End Function
    Public Function translate_to(ByVal v As vect3, ByVal m() As Single) As vect3
        Dim vo As vect3
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)
        Return vo

    End Function
    Public Function translate_only(ByVal v As vect3, ByVal m() As Single) As vect3
        Dim vo As vect3
        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)
        Return vo

    End Function
    Private Function transform(ByRef m() As Single, ByVal v As vertex_data, ByRef scale As Single, ByRef k As Integer) As vertex_data
        Dim vo As vertex_data
        v.x *= scale
        v.y *= scale
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)

        vo.u = v.u
        vo.v = v.v * -1.0

        vo.x += m(12)
        vo.y += m(13)
        vo.z += m(14)

        Return vo
    End Function
    Private Function rotate_decal_view(ByVal m() As Single) As vect3
        Dim vo As vect3
        Dim v As vect3
        v.x = 0.0
        v.y = 1.0
        v.z = 0.0
        vo.x = (m(0) * v.x) + (m(4) * v.y) + (m(8) * v.z)
        vo.y = (m(1) * v.x) + (m(5) * v.y) + (m(9) * v.z)
        vo.z = (m(2) * v.x) + (m(6) * v.y) + (m(10) * v.z)
        Dim l = Sqrt((vo.x ^ 2) + (vo.y ^ 2) + (vo.z ^ 2))
        If l = 0.0 Then l = 1.0
        vo.x /= l
        vo.y /= l
        vo.z /= l

        Return vo
    End Function

    Public Sub copy_decal()
        Dim d As New decal_matrix_list_
        d = decal_matrix_list(current_decal)
        'Return
        add_decal()
        With decal_matrix_list(current_decal)
            .decal_texture = d.decal_texture
            .texture_id = d.texture_id
            .gmm_id = d.gmm_id
            .normal_id = d.normal_id
            .alpha = d.alpha
            .level = d.level
            .u_wrap_index = d.u_wrap_index
            .v_wrap_index = d.v_wrap_index
            .u_wrap = d.u_wrap
            .v_wrap = d.v_wrap
            .uv_rot_index = d.uv_rot_index
            .uv_rot = d.uv_rot
            .rotation = d.rotation
            .scale = d.scale
            .translate = d.translate

            copy_mat4(.x_rotate_matrix, d.x_rotate_matrix)
            copy_mat4(.y_rotate_matrix, d.y_rotate_matrix)
            copy_mat4(.z_rotate_matrix, d.z_rotate_matrix)
            copy_mat4(.scale_matrix, d.scale_matrix)
            copy_mat4(.translate_matrix, d.translate_matrix)
            .get_decals_transform_info()
        End With
    End Sub
    Private Sub copy_mat4(ByRef m() As Single, ByRef s() As Single)
        For i = 0 To 15
            m(i) = s(i)
        Next


    End Sub

    Public Sub mouse_pick_decal()

        Dim er = Gl.glGetError
        Dim w, h As Integer
        G_Buffer.getsize(w, h)
        ViewPerspective(w, h)
        frmMain.set_eyes()
        If M_DOWN Then
            Return
        End If
        Gl.glReadBuffer(Gl.GL_BACK)
        Dim x = m_mouse.x
        Dim y = m_mouse.y
        Dim viewport(4) As Integer
        Dim pixel() As Byte = {0, 0, 0, 0}
        'Dim type = pixel(3)

        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glTexEnvf(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)

        Gl.glClearColor(0.0, 0.0, 0.3, 1.0)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT)

        If current_decal > -1 Then

            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            'Gl.glDisable(Gl.GL_DEPTH_TEST)
            For j = 0 To decal_matrix_list.Length - 2
                Dim i = decal_order(j)
                Gl.glColor3ub(CByte(i + 1), 0.0, 0.0)
                Gl.glPushMatrix()
                decal_matrix_list(i).transform()
                Gl.glMultMatrixf(decal_matrix_list(i).display_matrix)
                Gl.glCallList(decal_draw_box)
                Gl.glPopMatrix()
            Next

        End If


        'pick function
        Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport)
        Gl.glReadPixels(x, viewport(3) - y, 1, 1, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, pixel)
        er = Gl.glGetError
        If pixel(0) > 0 Then 'mouse is on upton window
            picked_decal = pixel(0) - 1
            Return
        End If
        picked_decal = -1
        'Gdi.SwapBuffers(pb1_hDC)

    End Sub

    Public Sub save_decal_data()

        Dim p = Temp_Storage + "\decal_layout"
        Dim f = File.Open(p, FileMode.OpenOrCreate, FileAccess.Write)
        Dim b As New BinaryWriter(f)

        Dim cnt As Integer = decal_matrix_list.Length - 2
        'write count of decals
        b.Write(cnt)
        For i = 0 To cnt - 1
            b.Write(decal_order(i))
        Next
        b.Write(decal_order(decal_order.Length - 1))
        For j = 0 To cnt
            With decal_matrix_list(j)

                b.Write(.alpha)
                b.Write(.level)
                b.Write(.scale.x)
                b.Write(.scale.y)
                b.Write(.scale.z)

                b.Write(.translate.x)
                b.Write(.translate.y)
                b.Write(.translate.z)

                b.Write(.rotation.x)
                b.Write(.rotation.y)
                b.Write(.rotation.z)

                b.Write(.u_wrap)
                b.Write(.v_wrap)
                b.Write(.uv_rot)

                b.Write(.u_wrap_index)
                b.Write(.v_wrap_index)
                b.Write(.uv_rot_index)

                b.Write(.decal_texture)

            End With

        Next
        b.Write(frmMain.d_list_tb.Text)
        b.Dispose()
        f.Close()

    End Sub

    Public Sub load_decal_data()
        Dim p = Temp_Storage + "\decal_layout"
        'Return
        If Not File.Exists(p) Then
            File.Copy(Application.StartupPath + "\resources\decal_layout\decal_layout", p)
        End If
        Dim f = File.Open(p, FileMode.Open, FileAccess.Read)
        Dim b As New BinaryReader(f)

        Dim cnt As Integer = b.ReadInt32
        ReDim decal_order(cnt + 1)
        For i = 0 To cnt
            decal_order(i) = b.ReadInt32
        Next
        'write count of decals
        decal_matrix_list = Nothing
        frmMain.d_list_tb.Text = ""
        Try
            current_decal = -1
            For j = 0 To cnt
                add_decal()
                decal_matrix_list(j) = New decal_matrix_list_
                decal_matrix_list(j).load_identity()
                With decal_matrix_list(j)

                    .alpha = b.ReadSingle
                    .level = b.ReadSingle
                    .scale.x = b.ReadSingle
                    .scale.y = b.ReadSingle
                    .scale.z = b.ReadSingle

                    .translate.x = b.ReadSingle
                    .translate.y = b.ReadSingle
                    .translate.z = b.ReadSingle

                    .rotation.x = b.ReadSingle
                    .rotation.y = b.ReadSingle
                    .rotation.z = b.ReadSingle

                    .u_wrap = b.ReadSingle
                    .v_wrap = b.ReadSingle
                    .uv_rot = b.ReadSingle

                    .u_wrap_index = b.ReadInt32
                    .v_wrap_index = b.ReadInt32
                    .uv_rot_index = b.ReadInt32

                    .decal_texture = b.ReadString

                    .get_decals_transform_info() 'setup matices and other data
                End With

                For i = 0 To decal_textures.Length - 1
                    If decal_matrix_list(j).decal_texture = decal_textures(i).colorMap_name Then
                        load_this_Decal(i)
                        decal_matrix_list(j).texture_id = decal_textures(i).colorMap_Id
                        decal_matrix_list(j).normal_id = decal_textures(i).normalMap_Id
                        decal_matrix_list(j).gmm_id = decal_textures(i).gmmMap_id
                        Exit For
                    End If
                Next

            Next
            frmMain.d_list_tb.Text = b.ReadString
            current_decal = 0
            update_decal_order()
            decal_matrix_list(current_decal).get_decals_transform_info()
        Catch ex As Exception

        End Try
        'hightlight first line
        Dim tc As Integer
        For k = 0 To decal_order.Length - 2
            If decal_order(k) = 0 Then
                tc = k
            End If
        Next
        frmMain.d_current_line = tc

        Dim sp = frmMain.d_list_tb.GetFirstCharIndexFromLine(tc) ' get prev line
        frmMain.d_list_tb.SelectionStart = sp
        frmMain.d_list_tb.Select(frmMain.d_list_tb.GetFirstCharIndexOfCurrentLine(), _
                         frmMain.d_list_tb.Lines(tc).Length) ' select prev line
        frmMain.d_sel_Len = frmMain.d_list_tb.Lines(tc).Length

        b.Dispose()
        f.Close()

    End Sub
    Private Sub read_d_data(id As Integer)

    End Sub
End Module
