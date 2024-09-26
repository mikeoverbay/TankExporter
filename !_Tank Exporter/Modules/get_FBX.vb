Imports System.IO
Imports Assimp
Imports Assimp.Vector3D
Imports Assimp.Matrix4x4
Imports Assimp.Unmanaged
Imports Skill.FbxSDK
Imports System.Globalization
Imports System.Numerics
Imports System.Drawing.Drawing2D

Module get_FBX
    Sub Open_2016_fbx()


        frmMain.OpenFileDialog1.Filter = "FBX|*.fbx"
        frmMain.OpenFileDialog1.Title = "Save FBX.."
        frmMain.OpenFileDialog1.FileName = My.Settings.fbx_path
        frmMain.OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(My.Settings.fbx_path)
        Dim result = frmMain.OpenFileDialog1.ShowDialog
        If result = DialogResult.Cancel Then
            Return
        End If
        My.Settings.fbx_path = frmMain.OpenFileDialog1.FileName
        Dim open_path = My.Settings.fbx_path
        My.Settings.Save()
        My.Settings.Save()
        My.Settings.Save()
        ' Load an FBX file
        Dim importer As New AssimpContext()

        frmMain.clean_house()
        remove_loaded_fbx()

        Try
            Dim scene As Scene = importer.ImportFile(open_path, Assimp.PostProcessSteps.Triangulate Or Assimp.PostProcessSteps.JoinIdenticalVertices)
            Dim materials As List(Of Assimp.Material) = scene.Materials.ToList

            ' Check if the scene is loaded successfully
            If scene IsNot Nothing Then
                Console.WriteLine("FBX file loaded successfully.")

                ' Iterate through all meshes in the scene
                ReDim Preserve fbxgrp(scene.MeshCount)
                Dim item = 1

                For Each mat In scene.Materials
                    If mat.HasTextureDiffuse Then
                        fbxgrp(item).color_name = mat.TextureDiffuse.FilePath
                    End If
                    If mat.HasTextureNormal Then
                        fbxgrp(item).normal_name = mat.TextureNormal.FilePath
                    End If
                    item += 1
                Next
                item = 1
                For Each mesh As Mesh In scene.Meshes
                    If mesh.Name.ToLower.Contains("~") Then
                        fbxgrp(item).name = "vehicles\" + mesh.Name 'existing tank part
                    Else
                        fbxgrp(item).name = mesh.Name ' new model
                    End If
                    ReDim fbxgrp(item).vertices(mesh.VertexCount - 1)
                    ReDim fbxgrp(item).indices(mesh.FaceCount - 1)

                    fbxgrp(item).nPrimitives_ = mesh.FaceCount
                    fbxgrp(item).nVertices_ = mesh.VertexCount

                    ' Debug output for mesh data
                    'Debug.WriteLine("Processing Mesh: " & mesh.Name)
                    'Debug.WriteLine("Vertex Count: " & mesh.VertexCount)
                    'Debug.WriteLine("Face Count: " & mesh.FaceCount)

                    GetMeshTransformations(scene, item)

                    Dim cnt = 0
                    For Each face In mesh.Faces
                        'Console.WriteLine("Face Indices: " & String.Join(", ", face.Indices))
                        ' Ensure each face has exactly 3 indices (triangulated mesh)
                        If face.Indices.Count = 3 Then
                            fbxgrp(item).indices(cnt) = New uvect3
                            fbxgrp(item).indices(cnt).v1 = face.Indices(0)
                            fbxgrp(item).indices(cnt).v2 = face.Indices(1)
                            fbxgrp(item).indices(cnt).v3 = face.Indices(2)
                            cnt += 1
                        Else
                            MsgBox("Non-triangulated face found.", MsgBoxStyle.Critical, "Triangulate before exporting!")
                            remove_loaded_fbx()
                            Return
                        End If
                    Next

                    cnt = 0
                    For Each vert In mesh.Vertices
                        If cnt = mesh.VertexCount Then Exit For ' Ensure we do not exceed the array bounds
                        fbxgrp(item).vertices(cnt) = New vertice_
                        fbxgrp(item).vertices(cnt).x = vert.X
                        fbxgrp(item).vertices(cnt).y = vert.Y
                        fbxgrp(item).vertices(cnt).z = vert.Z
                        If mesh.HasNormals Then
                            fbxgrp(item).vertices(cnt).nx = mesh.Normals(cnt).X
                            fbxgrp(item).vertices(cnt).ny = mesh.Normals(cnt).Y
                            fbxgrp(item).vertices(cnt).nz = mesh.Normals(cnt).Z
                        End If
                        If mesh.HasTextureCoords(0) Then
                            fbxgrp(item).vertices(cnt).u = mesh.TextureCoordinateChannels(0)(cnt).X
                            fbxgrp(item).vertices(cnt).v = -mesh.TextureCoordinateChannels(0)(cnt).Y
                        End If
                        If mesh.HasTextureCoords(1) Then
                            fbxgrp(item).vertices(cnt).u2 = mesh.TextureCoordinateChannels(1)(cnt).X
                            fbxgrp(item).vertices(cnt).v2 = -mesh.TextureCoordinateChannels(1)(cnt).Y
                            fbxgrp(item).has_uv2 = 1
                        Else
                            fbxgrp(item).has_uv2 = 0
                        End If
                        If mesh.HasVertexColors(1) Then
                            fbxgrp(item).has_color = True
                            Dim c = mesh.VertexColorChannels(1)(cnt)
                            fbxgrp(item).vertices(cnt).weight_1 = CByte(c.R * 255)
                            fbxgrp(item).vertices(cnt).weight_2 = CByte(c.G * 255)
                            fbxgrp(item).vertices(cnt).weight_3 = CByte(c.B * 255)
                            fbxgrp(item).vertices(cnt).weight_4 = CByte(c.A * 255)
                        End If
                        If mesh.HasVertexColors(0) Then
                            fbxgrp(item).has_color = 1
                            Dim c = mesh.VertexColorChannels(0)(cnt)
                            fbxgrp(item).vertices(cnt).index_1 = CByte(c.R * 255)
                            fbxgrp(item).vertices(cnt).index_2 = CByte(c.G * 255)
                            fbxgrp(item).vertices(cnt).index_3 = CByte(c.B * 255)
                        End If
                        cnt += 1
                    Next
                    'Debug.WriteLine(cnt.ToString + "  " + item.ToString)

                    'Console.WriteLine("Mesh Name: " & mesh.Name)
                    'Console.WriteLine("Number of Vertices: " & mesh.VertexCount)
                    'Console.WriteLine("Number of Faces: " & mesh.FaceCount)
                    create_TBNS(item)
                    item += 1
                Next
                For i = 1 To fbxgrp.Length - 1
                    If fbxgrp(i).color_name IsNot Nothing Then
                        fbxgrp(i).color_Id = get_fbx_texture(Path.GetDirectoryName(open_path) + "\" + fbxgrp(i).color_name)
                    Else
                        fbxgrp(i).color_Id = white_id ' fall back so we have something to render :)
                    End If

                    If fbxgrp(i).normal_name IsNot Nothing Then
                        fbx_bumped = 1
                        fbxgrp(i).normal_Id = get_fbx_texture(Path.GetDirectoryName(open_path) + "\" + fbxgrp(i).normal_name)
                    Else
                        fbxgrp(i).normal_Id = 0
                    End If
                Next

                'clean up 
                scene = Nothing
                materials = Nothing
                GC.Collect()
                GC.WaitForFullGCComplete()

                '===================================================================
                process_fbx_data()
                '===================================================================
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

                'Catch ex As Exception

                'End Try

                frmMain.info_Label.Text = "Creating Display Lists"
                Application.DoEvents()
                For i = 1 To fbxgrp.Length - 1
                    Dim id = Gl.glGenLists(1)
                    Gl.glNewList(id, Gl.GL_COMPILE)
                    fbxgrp(i).call_list = id
                    make_fbx_display_lists(fbxgrp(i).nPrimitives_, i)
                    Gl.glEndList()
                Next
                FBX_LOADED = True
                LOADING_FBX = False
                frmMain.info_Label.Visible = False
                frmMain.m_show_fbx.Checked = True
                If MODEL_LOADED Then
                    frmMain.m_show_fbx.Visible = True
                End If
            Else
                MsgBox("error in fbx", MsgBoxStyle.Exclamation, "error")
                remove_loaded_fbx() ' flush the stored data if any
                Return
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "error")
            remove_loaded_fbx()
        End Try
    End Sub
    Public Sub EchoOpenGLMatrix(openGLMatrix() As Double)
        ' Create a string to store the formatted matrix
        Dim matrixString As String = ""

        ' Format the matrix (4x4, column-major order)
        matrixString &= String.Format("{0,10:F4} {1,10:F4} {2,10:F4} {3,10:F4}" & Environment.NewLine,
                                  openGLMatrix(0), openGLMatrix(1), openGLMatrix(2), openGLMatrix(3))
        matrixString &= String.Format("{0,10:F4} {1,10:F4} {2,10:F4} {3,10:F4}" & Environment.NewLine,
                                  openGLMatrix(4), openGLMatrix(5), openGLMatrix(6), openGLMatrix(7))
        matrixString &= String.Format("{0,10:F4} {1,10:F4} {2,10:F4} {3,10:F4}" & Environment.NewLine,
                                  openGLMatrix(8), openGLMatrix(9), openGLMatrix(10), openGLMatrix(11))
        matrixString &= String.Format("{0,10:F4} {1,10:F4} {2,10:F4} {3,10:F4}" & Environment.NewLine,
                                  openGLMatrix(12), openGLMatrix(13), openGLMatrix(14), openGLMatrix(15))

        ' Output the formatted matrix to Debug
        Debug.WriteLine(matrixString)
    End Sub


    Private Sub ApplyTransformations(ByRef matrix() As Double)
        ' Apply scaling and translation to the matrix
        'matrix(0) *= 0.01    ' Scaling X (0.01)
        'matrix(5) *= 0.01    ' Scaling Y (0.01)
        'matrix(10) *= 0.01   ' Scaling Z (0.01)
        'matrix(12) *= 0.01   ' Translation X (0.01)
        'matrix(13) *= 0.01   ' Translation Y (0.01)
        'matrix(14) *= 0.01   ' Translation Z (0.01)
    End Sub

    Private Sub GetMeshTransformations(scene As Assimp.Scene, ByVal item As Integer)
        ' Check if item index is valid
        If item - 1 < 0 OrElse item - 1 >= scene.RootNode.Children.Count Then
            Debug.WriteLine("Invalid node index.")
            Return
        End If

        ' Get the node and associated mesh
        Dim node = scene.RootNode.Children(item - 1)
        If node.MeshIndices.Count = 0 Then
            Debug.WriteLine("Node has no meshes.")
            Return
        End If



        ' Get the transformation matrix for this node (applies to this mesh)
        Dim meshTransform As Assimp.Matrix4x4 = scene.RootNode.Children(item - 1).Transform
        ' Initialize arrays for translation, rotation, and scale
        Dim translation(2) As Single
        Dim rotation(2) As Single
        Dim scale(2) As Single

        ' Decompose the matrix
        Dim openGLMatrix(15) As Single

        fbxgrp(item).matrix = New Double(15) {}
        DecomposeMatrix(meshTransform, fbxgrp(item).matrix)

        ' Convert Assimp matrix to OpenGL format
        'ConvertAiMatrix4x4ToOpenGL(meshTransform, openGLMatrix)

        ' Apply scaling and translation
        ApplyTransformations(fbxgrp(item).matrix)

        ' Output the matrix and node index
        EchoOpenGLMatrix(fbxgrp(item).matrix)
        Dim op As String = "Mesh " + item.ToString
        Debug.WriteLine(op)
    End Sub
    Private Sub DecomposeMatrix(assimpMatrix As Assimp.Matrix4x4, ByRef rowMajorMatrix() As Double)
        ' Extract the 4x4 matrix elements from Assimp (column-major order)
        Dim m00 As Single = assimpMatrix.A1
        Dim m01 As Single = assimpMatrix.B1
        Dim m02 As Single = assimpMatrix.C1
        Dim m03 As Single = assimpMatrix.D1
        Dim m10 As Single = assimpMatrix.A2
        Dim m11 As Single = assimpMatrix.B2
        Dim m12 As Single = assimpMatrix.C2
        Dim m13 As Single = assimpMatrix.D2
        Dim m20 As Single = assimpMatrix.A3
        Dim m21 As Single = assimpMatrix.B3
        Dim m22 As Single = assimpMatrix.C3
        Dim m23 As Single = assimpMatrix.D3
        Dim m30 As Single = assimpMatrix.A4
        Dim m31 As Single = assimpMatrix.B4
        Dim m32 As Single = assimpMatrix.C4
        Dim m33 As Single = assimpMatrix.D4

        ' Extract scale factors
        Dim scaleX As Single = Math.Sqrt(m00 * m00 + m10 * m10 + m20 * m20)
        Dim scaleY As Single = Math.Sqrt(m01 * m01 + m11 * m11 + m21 * m21)
        Dim scaleZ As Single = Math.Sqrt(m02 * m02 + m12 * m12 + m22 * m22)

        ' Check for zero scale to prevent division by zero
        If scaleX = 0 Then scaleX = 1
        If scaleY = 0 Then scaleY = 1
        If scaleZ = 0 Then scaleZ = 1

        ' Normalize matrix by scale
        Dim normM00 As Single = m00 / scaleX
        Dim normM01 As Single = m01 / scaleY
        Dim normM02 As Single = m02 / scaleZ
        Dim normM10 As Single = m10 / scaleX
        Dim normM11 As Single = m11 / scaleY
        Dim normM12 As Single = m12 / scaleZ
        Dim normM20 As Single = m20 / scaleX
        Dim normM21 As Single = m21 / scaleY
        Dim normM22 As Single = m22 / scaleZ

        ' Extract translation (m30, m31, m32 are already translations)
        Dim translationX As Single = m30
        Dim translationY As Single = m31
        Dim translationZ As Single = m32

        ' Reconstruct matrix in row-major order (OpenGL compatible)
        rowMajorMatrix(0) = Math.Round(normM00, 6)
        rowMajorMatrix(4) = Math.Round(normM01, 6)
        rowMajorMatrix(8) = Math.Round(normM02, 6)
        rowMajorMatrix(12) = Math.Round(translationX, 6)

        rowMajorMatrix(1) = Math.Round(normM10, 6)
        rowMajorMatrix(5) = Math.Round(normM11, 6)
        rowMajorMatrix(9) = Math.Round(normM12, 6)
        rowMajorMatrix(13) = Math.Round(translationY, 6)

        rowMajorMatrix(2) = Math.Round(normM20, 6)
        rowMajorMatrix(6) = Math.Round(normM21, 6)
        rowMajorMatrix(10) = Math.Round(normM22, 6)
        rowMajorMatrix(14) = Math.Round(translationZ, 6)

        ' Set the rest (homogeneous coordinates)
        rowMajorMatrix(3) = Math.Round(m03, 6)
        rowMajorMatrix(7) = Math.Round(m13, 6)
        rowMajorMatrix(11) = Math.Round(m23, 6)
        rowMajorMatrix(15) = Math.Round(m33, 6) ' Homogeneous coordinate, usually 1
    End Sub

End Module
