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
            Dim scene As Scene = importer.ImportFile(open_path, Assimp.PostProcessSteps.Triangulate _
                                                     Or Assimp.PostProcessSteps.JoinIdenticalVertices)
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
                        fbxgrp(item).stride = 37
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
                            fbxgrp(item).stride = 37
                            fbxgrp(item).has_color = 1
                            Dim c = mesh.VertexColorChannels(1)(cnt)
                            fbxgrp(item).vertices(cnt).weight_1 = CByte(c.R * 255)
                            fbxgrp(item).vertices(cnt).weight_2 = CByte(c.G * 255)
                            fbxgrp(item).vertices(cnt).weight_3 = CByte(c.B * 255)
                            fbxgrp(item).vertices(cnt).weight_4 = CByte(c.A * 255)
                        End If
                        If mesh.HasVertexColors(0) Then
                            fbxgrp(item).stride = 40
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
        ' Get the mesh index (assuming item corresponds to the mesh index)
        Dim meshIndex As Integer = item - 1

        ' Check if mesh index is valid
        If meshIndex < 0 OrElse meshIndex >= scene.MeshCount Then
            Debug.WriteLine("Invalid mesh index.")
            Return
        End If

        ' Find the node that contains this mesh
        Dim node As Assimp.Node = FindNodeWithMesh(scene.RootNode, meshIndex)
        If node Is Nothing Then
            Debug.WriteLine("No node found containing mesh index: " & meshIndex)
            Return
        End If

        ' Accumulate transformations from root to this node
        Dim meshTransform As Assimp.Matrix4x4 = GetGlobalTransform(node)

        ' Convert the matrix to OpenGL format
        Dim openGLMatrix(15) As Single
        DecomposeMatrix(meshTransform, openGLMatrix)

        ' Store the matrix in fbxgrp(item).matrix
        fbxgrp(item).matrix = New Double(15) {}
        Array.Copy(openGLMatrix, fbxgrp(item).matrix, 16)

        ' Output the matrix and mesh index
        EchoOpenGLMatrix(fbxgrp(item).matrix)
        Dim op As String = "Mesh " + item.ToString()
        Debug.WriteLine(op)
    End Sub

    Private Sub DecomposeMatrix(assimpMatrix As Assimp.Matrix4x4, ByRef openGLMatrix() As Single)
        ' Correct mapping of Assimp matrix elements
        Dim m00 As Single = assimpMatrix.A1
        Dim m01 As Single = assimpMatrix.A2
        Dim m02 As Single = assimpMatrix.A3
        Dim m03 As Single = assimpMatrix.A4
        Dim m10 As Single = assimpMatrix.B1
        Dim m11 As Single = assimpMatrix.B2
        Dim m12 As Single = assimpMatrix.B3
        Dim m13 As Single = assimpMatrix.B4
        Dim m20 As Single = assimpMatrix.C1
        Dim m21 As Single = assimpMatrix.C2
        Dim m22 As Single = assimpMatrix.C3
        Dim m23 As Single = assimpMatrix.C4
        Dim m30 As Single = assimpMatrix.D1
        Dim m31 As Single = assimpMatrix.D2
        Dim m32 As Single = assimpMatrix.D3
        Dim m33 As Single = assimpMatrix.D4

        ' Construct the OpenGL matrix in column-major order
        openGLMatrix(0) = m00
        openGLMatrix(1) = m10
        openGLMatrix(2) = m20
        openGLMatrix(3) = m30

        openGLMatrix(4) = m01
        openGLMatrix(5) = m11
        openGLMatrix(6) = m21
        openGLMatrix(7) = m31

        openGLMatrix(8) = m02
        openGLMatrix(9) = m12
        openGLMatrix(10) = m22
        openGLMatrix(11) = m32

        openGLMatrix(12) = m03
        openGLMatrix(13) = m13
        openGLMatrix(14) = m23
        openGLMatrix(15) = m33
    End Sub

    Private Function FindNodeWithMesh(node As Assimp.Node, meshIndex As Integer) As Assimp.Node
        If node.MeshIndices.Contains(meshIndex) Then
            Return node
        End If

        For Each childNode As Assimp.Node In node.Children
            Dim result As Assimp.Node = FindNodeWithMesh(childNode, meshIndex)
            If result IsNot Nothing Then
                Return result
            End If
        Next

        Return Nothing
    End Function

    Private Function GetGlobalTransform(node As Assimp.Node) As Assimp.Matrix4x4
        Dim transform As Assimp.Matrix4x4 = node.Transform
        Dim currentNode As Assimp.Node = node.Parent

        While currentNode IsNot Nothing
            transform = currentNode.Transform * transform
            currentNode = currentNode.Parent
        End While

        Return transform
    End Function

    Private Sub EchoOpenGLMatrix(matrix() As Single)
        Debug.WriteLine("OpenGL Matrix:")
        For i As Integer = 0 To 15 Step 4
            Debug.WriteLine($"{matrix(i)}, {matrix(i + 1)}, {matrix(i + 2)}, {matrix(i + 3)}")
        Next
    End Sub

End Module
