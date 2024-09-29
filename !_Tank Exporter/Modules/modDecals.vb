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
Imports Assimp

#End Region



Module modDecals

    Public decal_draw_box As Integer


    Public current_decal_data_pnt As Integer = -1
    Public cur_selected_decal_texture As Integer = -1
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

    ' Define a structure representing a 3D vector with X, Y, and Z components.
    Public Structure Vect3s
        Public Property X As Single
        Public Property Y As Single
        Public Property Z As Single

        ' Constructor to initialize the vector components
        Public Sub New(ByVal x As Single, ByVal y As Single, ByVal z As Single)
            Me.X = x
            Me.Y = y
            Me.Z = z
        End Sub

        ' Method to calculate the length (magnitude) of the vector
        Public Function Length() As Single
            Return Math.Sqrt(X * X + Y * Y + Z * Z)
        End Function

        ' Method to set the vector components
        Public Sub SetValues(ByVal x As Single, ByVal y As Single, ByVal z As Single)
            Me.X = x
            Me.Y = y
            Me.Z = z
        End Sub

        ' Override the ToString method for easy display of vector components
        Public Overrides Function ToString() As String
            Return $"({X}, {Y}, {Z})"
        End Function
    End Structure

    Public decal_matrix_list As New List(Of DecalMatrix)
    Public Class DecalMatrix
        Public Property Alpha As Single
        Public Property Level As Single
        Public Property Scale As Vect3s
        Public Property Translate As Vect3s
        Public Property Rotation As Vect3s
        Public Property UWrap As Single
        Public Property VWrap As Single
        Public Property UVRot As Single
        Public Property UWrapIndex As Integer
        Public Property VWrapIndex As Integer
        Public Property UVRotIndex As Integer
        Public Property DecalTexture As String
        Public Property DisplayMatrix() As Single()
        Public Property YRotateMatrix() As Single()
        Public Property XRotateMatrix() As Single()
        Public Property ZRotateMatrix() As Single()
        Public Property ScaleMatrix() As Single()
        Public Property TranslateMatrix() As Single()

        Private Const PI2 As Single = 6.283
        Public Property DecalIndex As Integer

        Public Sub New()
            ' Initialize matrices with identity
            LoadIdentity()
        End Sub

        ' Deep copy method
        Public Sub Set_UI_and_Matrices()
            ' Example assignments using this instance
            g_decal_scale = Me.Scale
            g_decal_translate = Me.Translate
            g_decal_rotate = Me.Rotation
            If frmMain.track_decal_cb.Checked Then
                look_point_x = Me.Translate.X
                look_point_y = Me.Translate.Y
                look_point_z = Me.Translate.Z
            End If
            frmMain.d_texture_name.Text = Me.DecalTexture

            ' Set rotation matrices
            SetRotationMatrices()
        End Sub

        Public Sub SetRotationMatrices()

            ' Create  rotation matrix
            SetXRotationMatrix(Rotation.X)
            SetYRotationMatrix(Rotation.Y)
            SetZRotationMatrix(Rotation.Z)


            ' Create scale matrix
            Me.ScaleMatrix = {
                Me.Scale.X, 0.0, 0.0, 0.0,
                0.0, Me.Scale.Y, 0.0, 0.0,
                0.0, 0.0, Me.Scale.Z, 0.0,
                0.0, 0.0, 0.0, 1.0}

            ' Create translation matrix
            Me.TranslateMatrix = {
                1.0, 0.0, 0.0, 0.0,
                0.0, 1.0, 0.0, 0.0,
                0.0, 0.0, 1.0, 0.0,
                Me.Translate.X, Me.Translate.Y, Me.Translate.Z, 1.0}

        End Sub

        Public Sub LoadIdentity()
            ' Set default values when the decal is created
            Me.Alpha = 100
            Me.Level = 100
            Me.Scale = New Vect3s(1.0, 1.0, 1.0)
            Me.UWrap = 1.0
            Me.VWrap = 1.0
            Me.UVRot = 0
            Me.UWrapIndex = 4
            Me.VWrapIndex = 4
            Me.UVRotIndex = 4

            Me.Translate = New Vect3s(0.0, 0.0, 0.0)

            ' Check and set default scale
            If Me.Scale.Length() <= 0.0 Then
                Me.Scale = New Vect3s(1.0, 1.0, 1.0)
            End If

            ' Initialize identity matrices
            Me.DisplayMatrix = {
            1.0, 0.0, 0.0, 0.0,
            0.0, 1.0, 0.0, 0.0,
            0.0, 0.0, 1.0, 0.0,
            0.0, 0.0, 0.0, 1.0}

            Me.XRotateMatrix = DisplayMatrix.Clone()
            Me.YRotateMatrix = DisplayMatrix.Clone()
            Me.ZRotateMatrix = DisplayMatrix.Clone()
            Me.ScaleMatrix = DisplayMatrix.Clone()
            Me.TranslateMatrix = {
            1.0, 0.0, 0.0, 0.0,
            0.0, 1.0, 0.0, 0.0,
            0.0, 0.0, 1.0, 0.0,
            U_look_point_x, U_look_point_y, U_look_point_z, 1.0}

            ' Preset translation to where we are looking.
            g_decal_translate = New Vect3s(U_look_point_x, U_look_point_y, U_look_point_z)
            Me.SetXRotationMatrix(Math.PI / 2.0)
            Me.SetYRotationMatrix(0.0) 'Math.PI / 2.0
            Me.SetZRotationMatrix(0.0)
            Me.Translate = g_decal_translate
        End Sub

        Public Sub Transform()
            ' Push the current matrix onto the stack
            Gl.glPushMatrix()

            ' Load the identity matrix to reset transformations
            Gl.glLoadIdentity()

            ' Apply transformations in the correct order:
            Gl.glTranslatef(Me.Translate.X, Me.Translate.Y, Me.Translate.Z)
            Gl.glMultMatrixf(Me.ZRotateMatrix)        ' Apply rotation around Z axis
            Gl.glMultMatrixf(Me.YRotateMatrix)        ' Apply rotation around Y axis
            Gl.glMultMatrixf(Me.XRotateMatrix)        ' Apply rotation around X axis
            Gl.glMultMatrixf(Me.ScaleMatrix)          ' Apply scaling

            ' Retrieve the final transformation matrix
            Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, Me.DisplayMatrix)

            ' Restore the previous matrix state
            Gl.glPopMatrix()
        End Sub

        ' Utility function to format and display a 4x4 matrix


        ' Function for setting rotation matrix around Y-axis (Forward direction)
        Public Function SetYRotationMatrix(ByVal deltaY As Single) As Single
            ' Update the rotation around the X-axis because Y rotation controls pitch (up/down).
            Dim tempRotation = Me.Rotation
            tempRotation.Y = deltaY
            Me.Rotation = tempRotation ' Update the Rotation property

            ' Calculate the Y-axis rotation matrix
            Dim s As Single = Math.Sin(Me.Rotation.Y)
            Dim c As Single = Math.Cos(Me.Rotation.Y)
            Me.YRotateMatrix = {
        c, 0.0, s, 0.0,
        0.0, 1.0, 0.0, 0.0,
        -s, 0.0, c, 0.0,
        0.0, 0.0, 0.0, 1.0}

            Return 0
        End Function

        ' Subroutine for setting rotation matrix around X-axis (Up direction)
        Public Sub SetXRotationMatrix(ByVal deltaX As Single)
            ' Update the rotation around the Y-axis because X rotation controls yaw (left/right).
            Dim tempRotation = Me.Rotation
            tempRotation.X = deltaX
            Me.Rotation = tempRotation ' Update the Rotation property

            ' Calculate the X-axis rotation matrix
            Dim s As Single = Math.Sin(Me.Rotation.X)
            Dim c As Single = Math.Cos(Me.Rotation.X)
            Me.XRotateMatrix = {
        1.0, 0.0, 0.0, 0.0,
        0.0, c, -s, 0.0,
        0.0, s, c, 0.0,
        0.0, 0.0, 0.0, 1.0}
        End Sub

        ' Subroutine for setting rotation matrix around Z-axis (Roll direction)
        Public Sub SetZRotationMatrix(ByVal deltaZ As Single)
            ' Update the rotation around the Z-axis.
            Dim tempRotation = Me.Rotation
            tempRotation.Z = deltaZ
            Me.Rotation = tempRotation ' Update the Rotation property

            ' Calculate the Z-axis rotation matrix
            Dim s As Single = Math.Sin(Me.Rotation.Z)
            Dim c As Single = Math.Cos(Me.Rotation.Z)
            Me.ZRotateMatrix = {
        c, -s, 0.0, 0.0,
        s, c, 0.0, 0.0,
        0.0, 0.0, 1.0, 0.0,
        0.0, 0.0, 0.0, 1.0}
        End Sub

        Public Sub SetScaleMatrix(ByVal s As Vect3s)
            If s.X < 0.1 Then s.X = 0.1
            If s.Y < 0.1 Then s.Y = 0.1
            If s.Z < 0.1 Then s.Z = 0.1

            Me.Scale = s
            Me.ScaleMatrix = {
            s.X, 0.0, 0.0, 0.0,
            0.0, s.Y, 0.0, 0.0,
            0.0, 0.0, s.Z, 0.0,
            0.0, 0.0, 0.0, 1.0}
        End Sub

        Public Sub SetTranslateMatrix(ByVal v As Vect3s)
            Me.Translate = v
            Me.TranslateMatrix(12) = v.X
            Me.TranslateMatrix(13) = v.Y
            Me.TranslateMatrix(14) = v.Z
        End Sub

        Public Class DecalMatrix
            ' Existing properties and methods...

            ' Method to deep copy this object
        End Class

    End Class
    Public Sub updateGUI()
        Dim index = current_decal_data_pnt
        Try

            frmMain.decal_alpha_slider.Value = decal_matrix_list(index).Alpha
            frmMain.decal_level_slider.Value = decal_matrix_list(index).Level
            frmMain.Uwrap.SelectedIndex = decal_matrix_list(index).UWrapIndex
            frmMain.Vwrap.SelectedIndex = decal_matrix_list(index).VWrapIndex
            frmMain.uv_rotate.SelectedIndex = decal_matrix_list(index).UVRotIndex
            frmMain.current_decal_lable.Text = current_decal_data_pnt
        Catch ex As Exception

        End Try
    End Sub
    Public Function DecalMatrixClone(ByRef decal_in As DecalMatrix) As DecalMatrix
        Dim newDecalMatrix = New DecalMatrix
        newDecalMatrix.Alpha = decal_in.Alpha
        newDecalMatrix.Level = decal_in.Level
        newDecalMatrix.Scale = decal_in.Scale ' Assuming Vect3s has a Clone decal_inthod
        newDecalMatrix.Translate = decal_in.Translate
        newDecalMatrix.Rotation = decal_in.Rotation
        newDecalMatrix.UWrap = decal_in.UWrap
        newDecalMatrix.VWrap = decal_in.VWrap
        newDecalMatrix.UVRot = decal_in.UVRot
        newDecalMatrix.UWrapIndex = decal_in.UWrapIndex
        newDecalMatrix.VWrapIndex = decal_in.VWrapIndex
        newDecalMatrix.UVRotIndex = decal_in.UVRotIndex
        newDecalMatrix.DecalIndex = decal_in.DecalIndex
        newDecalMatrix.DecalTexture = decal_in.DecalTexture
        newDecalMatrix.DisplayMatrix = CType(decal_in.DisplayMatrix.Clone(), Single()) ' Deep copy array
        newDecalMatrix.YRotateMatrix = CType(decal_in.YRotateMatrix.Clone(), Single())
        newDecalMatrix.XRotateMatrix = CType(decal_in.XRotateMatrix.Clone(), Single())
        newDecalMatrix.ZRotateMatrix = CType(decal_in.ZRotateMatrix.Clone(), Single())
        newDecalMatrix.ScaleMatrix = CType(decal_in.ScaleMatrix.Clone(), Single())
        newDecalMatrix.TranslateMatrix = CType(decal_in.TranslateMatrix.Clone(), Single())
        newDecalMatrix.DecalIndex = decal_in.DecalIndex
        Return newDecalMatrix
    End Function

    ' Assume Vect3 is a custom structure or class representing a vector with X, Y, and Z components and a Length method.

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



    ' Method to create a cube and store it in the decal_draw_box display list
    Public Sub CreateCube()
        ' Generate a new display list and store the list ID in decal_draw_box
        decal_draw_box = Gl.glGenLists(1)

        ' Start defining the display list
        Gl.glNewList(decal_draw_box, Gl.GL_COMPILE)

        ' Draw the cube using GL_QUADS for each face with vertices transformed manually for Z-axis rotation
        Gl.glBegin(Gl.GL_QUADS)

        ' Front face (rotated around Z-axis)
        Gl.glVertex3f(0.5F, -0.5F, 0.5F) ' Bottom left -> (y, -x, z) -> (0.5, -0.5, 0.5)
        Gl.glVertex3f(0.5F, 0.5F, 0.5F)  ' Bottom right -> (y, -x, z) -> (0.5, 0.5, 0.5)
        Gl.glVertex3f(-0.5F, 0.5F, 0.5F) ' Top right -> (y, -x, z) -> (-0.5, 0.5, 0.5)
        Gl.glVertex3f(-0.5F, -0.5F, 0.5F) ' Top left -> (y, -x, z) -> (-0.5, -0.5, 0.5)

        ' Back face (rotated around Z-axis)
        Gl.glVertex3f(0.5F, 0.5F, -0.5F)  ' Bottom right -> (y, -x, z) -> (0.5, 0.5, -0.5)
        Gl.glVertex3f(0.5F, -0.5F, -0.5F) ' Bottom left -> (y, -x, z) -> (0.5, -0.5, -0.5)
        Gl.glVertex3f(-0.5F, -0.5F, -0.5F) ' Top left -> (y, -x, z) -> (-0.5, -0.5, -0.5)
        Gl.glVertex3f(-0.5F, 0.5F, -0.5F)  ' Top right -> (y, -x, z) -> (-0.5, 0.5, -0.5)

        ' Left face (rotated around Z-axis)
        Gl.glVertex3f(-0.5F, -0.5F, -0.5F) ' Bottom back -> (y, -x, z) -> (-0.5, -0.5, -0.5)
        Gl.glVertex3f(-0.5F, -0.5F, 0.5F)  ' Bottom front -> (y, -x, z) -> (-0.5, -0.5, 0.5)
        Gl.glVertex3f(0.5F, -0.5F, 0.5F)   ' Top front -> (y, -x, z) -> (0.5, -0.5, 0.5)
        Gl.glVertex3f(0.5F, -0.5F, -0.5F)  ' Top back -> (y, -x, z) -> (0.5, -0.5, -0.5)

        ' Right face (rotated around Z-axis)
        Gl.glVertex3f(0.5F, 0.5F, 0.5F)    ' Bottom front -> (y, -x, z) -> (0.5, 0.5, 0.5)
        Gl.glVertex3f(-0.5F, 0.5F, 0.5F)   ' Bottom back -> (y, -x, z) -> (-0.5, 0.5, 0.5)
        Gl.glVertex3f(-0.5F, 0.5F, -0.5F)  ' Top back -> (y, -x, z) -> (-0.5, 0.5, -0.5)
        Gl.glVertex3f(0.5F, 0.5F, -0.5F)   ' Top front -> (y, -x, z) -> (0.5, 0.5, -0.5)

        ' Top face (rotated around Z-axis)
        Gl.glVertex3f(0.5F, 0.5F, 0.5F)    ' Top front right -> (y, -x, z) -> (0.5, 0.5, 0.5)
        Gl.glVertex3f(0.5F, -0.5F, 0.5F)   ' Top front left -> (y, -x, z) -> (0.5, -0.5, 0.5)
        Gl.glVertex3f(0.5F, -0.5F, -0.5F)  ' Top back left -> (y, -x, z) -> (0.5, -0.5, -0.5)
        Gl.glVertex3f(0.5F, 0.5F, -0.5F)   ' Top back right -> (y, -x, z) -> (0.5, 0.5, -0.5)

        ' Bottom face (rotated around Z-axis)
        Gl.glVertex3f(-0.5F, 0.5F, -0.5F)  ' Bottom back left -> (y, -x, z) -> (-0.5, 0.5, -0.5)
        Gl.glVertex3f(-0.5F, -0.5F, -0.5F) ' Bottom back right -> (y, -x, z) -> (-0.5, -0.5, -0.5)
        Gl.glVertex3f(-0.5F, -0.5F, 0.5F)  ' Bottom front right -> (y, -x, z) -> (-0.5, -0.5, 0.5)
        Gl.glVertex3f(-0.5F, 0.5F, 0.5F)   ' Bottom front left -> (y, -x, z) -> (-0.5, 0.5, 0.5)

        Gl.glEnd() ' End of drawing GL_QUADS

        ' End the display list
        Gl.glEndList()
    End Sub


    ' Adds a new decal entry to both the DataGridView and the decal_matrix_list.
    Public Sub add_decal()
        ' Reset the update event before performing operations
        updateEvent.Reset()
        ' Pause briefly to ensure any ongoing operations are completed
        Thread.Sleep(50)

        ' Determine the index where the new row should be inserted (after the current selection)
        Dim selectedIndex As Integer = If(frmMain.dgv.SelectedRows.Count > 0, frmMain.dgv.SelectedRows(0).Index + 1, frmMain.dgv.Rows.Count)

        ' Insert a new row at the next index in the DataGridView
        frmMain.dgv.Rows.Insert(selectedIndex)

        ' Get the newly inserted row
        Dim newRow As DataGridViewRow = frmMain.dgv.Rows(selectedIndex)

        ' Set default values for the new row
        newRow.Cells("DecalName").Value = decal_textures(0).colorMap_name ' Default to the first decal name
        newRow.Cells("DecalID").Value = 0
        newRow.Cells("Alpha").Value = 100.0!
        newRow.Cells("Level").Value = 100.0!
        newRow.Cells("U_Wrap").Value = 1.0!
        newRow.Cells("V_Wrap").Value = 1.0!
        newRow.Cells("UV_Rot").Value = 4
        newRow.Cells("ScaleX").Value = 1.0!
        newRow.Cells("ScaleY").Value = 1.0!
        newRow.Cells("ScaleZ").Value = 1.0!
        newRow.Cells("TranslateX").Value = 0.0!
        newRow.Cells("TranslateY").Value = 0.0!
        newRow.Cells("TranslateZ").Value = 0.0!
        newRow.Cells("RotationX").Value = -PI / 2.0!
        newRow.Cells("RotationY").Value = 0.0!
        newRow.Cells("RotationZ").Value = 0.0!
        newRow.Cells("U_Wrap_Index").Value = 4
        newRow.Cells("V_Wrap_Index").Value = 4
        newRow.Cells("UV_Rot_Index").Value = 4

        ' Optionally, select the newly inserted row
        frmMain.dgv.ClearSelection()
        frmMain.dgv.Rows(selectedIndex).Selected = True

        ' Ensure decal_matrix_list is a List(Of DecalMatrix) and not an array
        If decal_matrix_list Is Nothing Then
            decal_matrix_list = New List(Of DecalMatrix)() ' Initialize as a list if not already done
        End If

        Dim newDecal As New DecalMatrix
        ' Initialize the matrices as identity matrices (4x4 matrices with 16 elements)
        ReDim newDecal.DisplayMatrix(15)
        ReDim newDecal.XRotateMatrix(15)
        ReDim newDecal.YRotateMatrix(15)
        ReDim newDecal.ZRotateMatrix(15)
        ReDim newDecal.TranslateMatrix(15)
        ReDim newDecal.ScaleMatrix(15)
        ' Load identity matrices
        newDecal.LoadIdentity()
        newDecal.Set_UI_and_Matrices()
        newDecal.DecalIndex = 0

        ' Insert the new structure into the list at the desired index
        decal_matrix_list.Insert(selectedIndex, newDecal)
        current_decal_data_pnt = selectedIndex

        ' Set the default rotation matrix to -1.5707 for X-axis rotation
        If Not LOADING Then

        End If
        'newDecal.SetXRotationMatrix(-PI / 2)

        ' Update the current selected decal texture index
        cur_selected_decal_texture = 0
        setthisdecal(0)

        ' Update the current decal label text
        frmMain.current_decal_lable.Text = current_decal_data_pnt

        ' Re-enable the update event after operations are complete
        frmMain.set_g_decal_current()
        updateEvent.Set()
    End Sub
    ' Creates and initializes a new decal_matrix_list_ structure at the specified index in the decal_matrix_list.
    Public Sub setthisdecal(ByVal texturePnt As Integer)
        decal_matrix_list(current_decal_data_pnt).DecalIndex = texturePnt
    End Sub

    Public Sub load_this_Decal(ByVal j As Integer)
        If decal_textures(j).colorMap_Id = 0 Then
            Try
                Dim name As String = decal_textures(j).full_path
                decal_textures(j).colorMap_Id = LoadTextureDDS(name)
                Dim ts = name.Replace("_AM.dds", "_NM.dds")
                decal_textures(j).normalMap_Id = LoadTextureDDS(ts)
                ts = name.Replace("_AM.dds", "_GMM.dds")
                decal_textures(j).gmmMap_id = LoadTextureDDS(ts)

            Catch ex As Exception

            End Try
        End If
    End Sub

    Public Sub load_decal_textures()
        If My.Settings.stop_loading_decals Then Return

        Dim dPath As String = decal_path + "\resources\decals\"
        Dim dir_info() As String = Nothing
        Try
            dir_info = Directory.GetFiles(dPath)
        Catch ex As Exception
            MsgBox("decals not found", vbCritical, "missing decals")
            End
        End Try
        Dim f_cnt = dir_info.Count
        Dim c_names(f_cnt) As String
        Dim c_c As Integer

        For Each f In dir_info
            If Not f.ToLower.Contains("nm.d") And Not f.ToLower.Contains("gmm.d") And f.ToLower.Contains("_am") Then
                c_names(c_c) = f
                c_c += 1
            End If
        Next
        ReDim decal_textures(c_c - 1)
        Dim ts As String = ""
        For j = 0 To c_c - 1
            If decal_textures(j).colorMap_Id = 0 Then

                If File.Exists(c_names(j).Replace("_AM.dds", "_GMM.dds")) Then
                    decal_textures(j) = New decal_texture_
                    decal_textures(j).full_path = c_names(j)
                    decal_textures(j).colorMap_name = Path.GetFileNameWithoutExtension(c_names(j))

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


    Private Function DummyDeepCopy(original As DecalMatrix) As DecalMatrix
        ' For now, just create a new instance of DecalMatrix
        ' Later, this subroutine can be filled with actual deep copy logic
        Return New DecalMatrix()
    End Function

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

        If decal_matrix_list.Count > 0 Then

            Gl.glDisable(Gl.GL_LIGHTING)
            Gl.glDisable(Gl.GL_CULL_FACE)
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            'Gl.glDisable(Gl.GL_DEPTH_TEST)
            For i = 0 To decal_matrix_list.Count - 1
                Gl.glColor3ub(CByte(i + 1), 0.0, 0.0)
                Gl.glPushMatrix()
                decal_matrix_list(i).transform()
                Gl.glMultMatrixf(decal_matrix_list(i).DisplayMatrix)
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
            If frmPickDecal.Visible Then
                'frmPickDecal.set_selecion(picked_decal)
            End If
            Return
        End If
        picked_decal = -1
        'Gdi.SwapBuffers(pb1_hDC)

    End Sub

    ' Function to load data from a CSV file into a DataGridView

    Public Sub ExportDataGridViewToCSV(dgv As DataGridView, filePath As String)
        Try
            Using writer As New System.IO.StreamWriter(filePath)
                ' Write the header row
                Dim headers = dgv.Columns.Cast(Of DataGridViewColumn)().[Select](Function(column) column.HeaderText).ToArray()
                writer.WriteLine(String.Join(",", headers))

                ' Write the data rows
                For Each row As DataGridViewRow In dgv.Rows
                    If Not row.IsNewRow Then
                        Dim cells = row.Cells.Cast(Of DataGridViewCell)().[Select](Function(cell) If(cell.Value IsNot Nothing, cell.Value.ToString(), String.Empty)).ToArray()
                        writer.WriteLine(String.Join(",", cells))
                    End If
                Next
            End Using

            MessageBox.Show("Data exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error exporting data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Dim LOADING As Boolean = False
    Public Sub load_decal_layout()
        Dim filepath = Temp_Storage + "\decal_layout.csv"
        Try
            LOADING = True
            ' Clear existing rows and columns
            frmMain.dgv.Rows.Clear()
            frmMain.dgv.Columns.Clear()

            ' Open the CSV file for reading
            Using reader As New System.IO.StreamReader(filepath)
                ' Read the header line
                Dim headerLine As String = reader.ReadLine()
                If headerLine Is Nothing Then
                    MessageBox.Show("The file is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                ' Split the header line to create columns
                Dim headers As String() = headerLine.Split(","c)
                For Each header As String In headers
                    frmMain.dgv.Columns.Add(header, header)
                Next

                ' Read the data rows
                While Not reader.EndOfStream
                    Dim line As String = reader.ReadLine()
                    If Not String.IsNullOrWhiteSpace(line) Then
                        ' Split the line into values and add to DataGridView
                        Dim values As String() = line.Split(","c)
                        frmMain.dgv.Rows.Add(values)
                    End If
                End While
            End Using
            Dim tg = frmMain.dgv
            PopulateDecalMatrixListFromDataGridView()
            With frmMain.dgv
                ' Disable auto resizing of columns
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None

                ' Set specific widths and disable resizing for column 0
                .Columns(0).Width = 170
                .Columns(0).Resizable = DataGridViewTriState.False
                .Columns(0).SortMode = DataGridViewColumnSortMode.NotSortable

                ' Set specific widths and disable resizing for column 2
                .Columns(1).Width = 60
                .Columns(1).Resizable = DataGridViewTriState.False
                .Columns(1).SortMode = DataGridViewColumnSortMode.NotSortable

                ' Optional: Disable user resizing of the entire DataGridView columns
                .AllowUserToResizeColumns = False
            End With
            current_decal_data_pnt = 0
            frmMain.set_g_decal_current()
            'Dim dgv2 = frmMain.dgv
            'MessageBox.Show("Data loaded successfully from CSV.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            LOADING = False
            MessageBox.Show("Error loading data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        LOADING = False

    End Sub
    ' Populates the global decal_matrix_list from the global frmMain.dgv DataGridView.
    Private Sub PopulateDecalMatrixListFromDataGridView()
        ' Clear the existing decal_matrix_list to ensure it's empty before populating
        decal_matrix_list.Clear()

        ' Loop through each row in the DataGridView
        Dim idx As Integer = 0
        For Each row As DataGridViewRow In frmMain.dgv.Rows
            ' Skip the new row placeholder if present
            If row.IsNewRow Then Continue For

            ' Create a new instance of DecalMatrix
            Dim decal As New DecalMatrix
            decal.LoadIdentity()

            decal.DecalTexture = Convert.ToString(row.Cells("DecalName").Value)

            ' Populate fields from DataGridView cells
            decal.Alpha = Convert.ToSingle(row.Cells("Alpha").Value)
            decal.Level = Convert.ToSingle(row.Cells("Level").Value)

            ' Scale values
            Dim tempScale As Vect3s = decal.Scale
            tempScale.X = Convert.ToSingle(row.Cells("ScaleX").Value)
            tempScale.Y = Convert.ToSingle(row.Cells("ScaleY").Value)
            tempScale.Z = Convert.ToSingle(row.Cells("ScaleZ").Value)
            decal.Scale = tempScale

            ' Translate values
            Dim tempTranslate As Vect3s = decal.Translate
            tempTranslate.X = Convert.ToSingle(row.Cells("TranslateX").Value)
            tempTranslate.Y = Convert.ToSingle(row.Cells("TranslateY").Value)
            tempTranslate.Z = Convert.ToSingle(row.Cells("TranslateZ").Value)
            decal.Translate = tempTranslate

            ' Rotation values
            Dim tempRotation As Vect3s = decal.Rotation
            tempRotation.X = Convert.ToSingle(row.Cells("RotationX").Value)
            tempRotation.Y = Convert.ToSingle(row.Cells("RotationY").Value)
            tempRotation.Z = Convert.ToSingle(row.Cells("RotationZ").Value)
            decal.Rotation = tempRotation

            ' UV properties
            decal.UWrap = Convert.ToSingle(row.Cells("U_Wrap").Value)
            decal.VWrap = Convert.ToSingle(row.Cells("V_Wrap").Value)
            decal.UVRot = Convert.ToSingle(row.Cells("UV_Rot").Value)

            ' UV indices
            decal.UWrapIndex = Convert.ToInt32(row.Cells("U_Wrap_Index").Value)
            decal.VWrapIndex = Convert.ToInt32(row.Cells("V_Wrap_Index").Value)
            decal.UVRotIndex = Convert.ToInt32(row.Cells("UV_Rot_Index").Value)


            ' Initialize the matrices as identity matrices (4x4 matrices with 16 elements)

            ' Assign texture IDs if the texture matches
            For i = 0 To decal_textures.Length - 1
                If decal.DecalTexture = decal_textures(i).colorMap_name Then
                    load_this_Decal(i)
                    decal.DecalIndex = i
                End If
            Next


            ' Add to the decal_matrix_list
            decal.Set_UI_and_Matrices()
            ' Set up matrices
            decal_matrix_list.Add(decal)
            current_decal_data_pnt = idx
            updateGUI()
            idx += 1
        Next
    End Sub

End Module
