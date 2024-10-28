' Add necessary imports
Imports Assimp
Imports Assimp.Configs
Imports Assimp.Unmanaged
Imports System.IO
Imports System.Windows.Forms

Module put_fbx


    Sub write_FBX()
        Dim ar() As String
        If PRIMITIVES_MODE Then
            ar = Path.GetFileNameWithoutExtension(frmMain.OpenFileDialog1.FileName).Split("~")
        Else
            ar = TANK_NAME.Split(":")
        End If
        file_name = Path.GetFileNameWithoutExtension(current_tank_name) + ".fbx"
        frmMain.SaveFileDialog1.InitialDirectory = My.Settings.fbx_path
        ' Create a SaveFileDialog to get the save path
        frmMain.SaveFileDialog1.Filter = "FBX Files (*.fbx)|*.fbx"
        frmMain.SaveFileDialog1.Title = "Save FBX File"
        frmMain.SaveFileDialog1.FileName = file_name

        ' Show the SaveFileDialog
        Dim savePath As String = frmMain.SaveFileDialog1.FileName
        Dim result = frmMain.SaveFileDialog1.ShowDialog
        If Not result = DialogResult.OK Then
            Return
        End If
        Dim out_path = frmMain.SaveFileDialog1.FileName
        Dim name As String = Path.GetFileName(ar(0))
        Dim save_path = Path.GetDirectoryName(out_path) + "\" + name

        ' Proceed to export the model
        frmMain.SaveFileDialog1.AddExtension = True
        frmMain.SaveFileDialog1.RestoreDirectory = True

        '=================================================================
        export_fbx_textures(False, 0) 'export all textures
        '=================================================================

        ' Create a new Assimp scene
        Dim scene As New Assimp.Scene()
        ' Ensure the root node exists
        If scene.RootNode Is Nothing Then
            scene.RootNode = New Assimp.Node("RootNode")
        End If
        ' Loop through your model data and create meshes
        For i As Integer = 1 To _group.Length - 1
            Dim model_name = _group(i).name.Replace("/", "\")
            model_name = model_name.Replace(":", "~")
            model_name = model_name.Replace("vehicles\", "")
            model_name = model_name.Replace("primitives_processed", "pri")
            model_name = model_name.Replace("\lod0\", "\l\")

            ar = TANK_NAME.Split(":")
            name = Path.GetFileName(ar(0))

            Dim meshData = _group(i)

            ' Create a new material for this mesh
            Dim material As New Assimp.Material()
            material.Name = "Material_" & i.ToString

            ' Assign diffuse texture
            If Not String.IsNullOrEmpty(meshData.color_name) Then
                Dim arr = _group(i).color_name.Split("\")
                Dim diffuseTexturePath As String = name + "\" + arr(arr.Length - 1).Replace(".dds", ".png")
                Dim diffuseTextureSlot As New Assimp.TextureSlot(
            diffuseTexturePath,
            TextureType.Diffuse,
            0,
            TextureMapping.FromUV,
            0,
            0.0F,
            TextureOperation.Add,
            TextureWrapMode.Wrap,
            TextureWrapMode.Wrap,
            0
        )
                material.AddMaterialTexture(diffuseTextureSlot)
            End If

            ' Assign normal map texture
            If Not String.IsNullOrEmpty(meshData.normal_name) Then
                Dim arr = _group(i).normal_name.Split("\")
                Dim normalTexturePath As String = name + "\" + arr(arr.Length - 1).Replace(".dds", ".png")
                Dim normalTextureSlot As New Assimp.TextureSlot(
            normalTexturePath,
            TextureType.Normals,
            0,
            TextureMapping.FromUV,
            0,
            0.0F,
            TextureOperation.Add,
            TextureWrapMode.Wrap,
            TextureWrapMode.Wrap,
            0
        )
                material.AddMaterialTexture(normalTextureSlot)
            End If

            ' Add material to scene and get index
            scene.Materials.Add(material)
            Dim materialIndex As Integer = scene.Materials.Count - 1

            ' Create mesh with assigned material index
            Dim mesh As New Assimp.Mesh(meshData.name, PrimitiveType.Triangle)
            mesh.MaterialIndex = materialIndex
            '==================================================================
            ' Continue with adding vertices, faces, etc.
            ' Initialize texture coordinate channels.. etc.
            '==================================================================

            mesh.TextureCoordinateChannels(0) = New List(Of Assimp.Vector3D)()
            If meshData.has_uv2 = 1 Then
                mesh.TextureCoordinateChannels(1) = New List(Of Assimp.Vector3D)()
            End If

            ' Initialize vertex color channels based on stride
            If meshData.stride = 40 Then
                ' Initialize both color channels
                mesh.VertexColorChannels(0) = New List(Of Assimp.Color4D)()
                mesh.VertexColorChannels(1) = New List(Of Assimp.Color4D)()
            ElseIf meshData.stride = 37 Then
                ' Initialize only color channel 0
                mesh.VertexColorChannels(0) = New List(Of Assimp.Color4D)()
            End If

            ' Add vertices
            For j As Integer = 0 To meshData.nVertices_ - 1
                Dim vert = meshData.vertices(j)
                ' Add position
                mesh.Vertices.Add(New Assimp.Vector3D(vert.x, vert.y, vert.z))
                mesh.Normals.Add(New Assimp.Vector3D(vert.nx, vert.ny, vert.nz))

                ' Add texture coordinates
                mesh.TextureCoordinateChannels(0).Add(New Assimp.Vector3D(vert.u, vert.v, 0))

                If meshData.has_uv2 = 1 Then
                    mesh.TextureCoordinateChannels(1).Add(New Assimp.Vector3D(vert.u2, vert.v2, 0))
                End If

                ' Add vertex colors based on stride
                If meshData.stride = 40 Then
                    ' Add color 1 to VertexColorChannels(0)
                    Dim color1 As New Assimp.Color4D(vert.index_1, vert.index_2, vert.index_3, vert.index_4)
                    mesh.VertexColorChannels(0).Add(color1)

                    ' Add color 2 to VertexColorChannels(1)
                    Dim color2 As New Assimp.Color4D(vert.weight_1, vert.weight_2, vert.weight_3, vert.weight_4)
                    mesh.VertexColorChannels(1).Add(color2)
                ElseIf meshData.stride = 37 Then
                    ' Add color 1 to VertexColorChannels(0)
                    Dim color1 As New Assimp.Color4D(vert.index_1, vert.index_2, vert.index_3, vert.index_4)
                    mesh.VertexColorChannels(0).Add(color1)
                End If

            Next

            ' Add faces (triangles)
            For j As Integer = 1 To meshData.indices.Length - 1
                ' Create an array of indices for the face
                Dim faceIndices() As Integer = {
            CInt(meshData.indices(j).v1),
            CInt(meshData.indices(j).v2),
            CInt(meshData.indices(j).v3)
        }
                ' Create a new face with these indices
                Dim face As New Assimp.Face(faceIndices)
                ' Add the face to the mesh
                mesh.Faces.Add(face)
            Next

            scene.Meshes.Add(mesh)
            ' Assign the mesh to the root node
            scene.RootNode.MeshIndices.Add(scene.Meshes.Count - 1)
        Next

        ' Create an AssimpContext for exporting
        Dim exporter As New AssimpContext()

        ' Try to export the scene
        Dim status = exporter.ExportFile(scene, out_path, "fbx")
        Dim fmts = exporter.GetSupportedExportFormats
        If status Then
            MessageBox.Show("FBX file saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show("Error exporting FBX file: Export returned false.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub


End Module
