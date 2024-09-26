Imports SharpGLTF.Geometry
Imports SharpGLTF.Geometry.VertexTypes
Imports SharpGLTF.Materials
Imports SharpGLTF.Scenes
Imports SharpGLTF.Schema2
Imports System.IO

Imports System.Numerics
Imports SharpGLTF.Transforms

Imports System.Net.Mime.MediaTypeNames
Imports Tank_Exporter.modVisualParser
Imports System.Text.Json.Serialization.JsonAttribute
Imports System.Text.Json
Imports System.Text.Json.Serialization.JsonConstructorAttribute
Imports System.Text.Json.Nodes

Module put_glTF
    Public Class ExtrasData
        Public Property BaseColorTexture As String
        Public Property AoTexture As String
        Public Property MetalRoughTexture As String
        Public Property NormalTexture As String
    End Class


    Public Structure ModelData
        Public Vertices() As vect3
        Public indices() As vect3
        Public path As String
        Public VertexColors() As vect3
        Public Weights() As vect3
        Public Normals() As vect3
        Public UVs0() As vec2
        Public UVs1() As vec2
        Public Property Textures As Dictionary(Of String, String) ' Texture type to file path
    End Structure



    Public Function fixname(name As String) As String
        Return My.Settings.res_mods_path.Replace("/", "\") + "\" + name.Replace("/", "\").Replace(".dds", "_hd.dds")
    End Function

    Public Sub write_glTF()
        Dim ar() As String
        If PRIMITIVES_MODE Then
            ar = Path.GetFileNameWithoutExtension(frmMain.OpenFileDialog1.FileName).Split("~")
        Else
            ar = TANK_NAME.Split(":")
        End If

        frmMain.SaveFileDialog1.InitialDirectory = My.Settings.fbx_path
        frmMain.SaveFileDialog1.AddExtension = True
        frmMain.SaveFileDialog1.RestoreDirectory = False

        Select Case EXPORT_TYPE
            Case 1
                frmMain.SaveFileDialog1.Filter = "GLB|*.glb"
                frmMain.SaveFileDialog1.Title = "Save GLB.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".glb"
            Case 2
                frmMain.SaveFileDialog1.Filter = "FBX|*.fbx"
                frmMain.SaveFileDialog1.Title = "Save FBX.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".fbx"
            Case 3
                frmMain.SaveFileDialog1.Filter = "OBJ|*.obj"
                frmMain.SaveFileDialog1.Title = "Save OBJ.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".obj"
            Case 4
                frmMain.SaveFileDialog1.Filter = "Collada|*.dae"
                frmMain.SaveFileDialog1.Title = "Save Collada.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".dae"
        End Select

        Dim result As DialogResult = frmMain.SaveFileDialog1.ShowDialog()
        Dim out_path As String = frmMain.SaveFileDialog1.FileName
        ' Proceed with the saving process using out_path
        My.Settings.fbx_path = out_path
        My.Settings.Save()

        If Not result = DialogResult.OK Then
            Return
        End If


        My.Settings.GLTF_path = out_path

        Dim name As String = Path.GetFileName(ar(0))
        Dim save_path = Path.GetDirectoryName(out_path) + "\" + name
        export_fbx_textures(False, 1) 'export all textures. converts from dds to png.

        Dim MySceneBuilder As New SceneBuilder()
        MySceneBuilder.Name = name

        ' Create a root node
        For item = 1 To object_count
            Dim extras As New ExtrasData()

            ' Create a material and assign texture maps if available
            Dim MyMaterialBuilder As New MaterialBuilder("Material00" + item.ToString) With {.ShaderStyle = "PBRMetallicRoughness"}



            Dim baseColorTexture = save_path + "\" + Path.GetFileNameWithoutExtension(_group(item).color_name.Replace("\tracks", "")) + ".png"
            Dim aoTexture As String = save_path + "\" + Path.GetFileNameWithoutExtension(_group(item).ao_name.Replace("\tracks", "")) + ".png"
            Dim metalRoughTexture = save_path + "\" + Path.GetFileNameWithoutExtension(_group(item).metalGMM_name.Replace("\tracks", "")) + ".png"
            Dim normalTexture = save_path + "\" + Path.GetFileNameWithoutExtension(_group(item).normal_name.Replace("\tracks", "")) + ".png"

            ' Assign Base Color Texture if exists
            If Not String.IsNullOrEmpty(baseColorTexture) Then
                Try
                    MyMaterialBuilder.WithBaseColor(baseColorTexture)
                Catch ex As Exception
                End Try
            End If
            ' Check its there!
            If Not String.IsNullOrEmpty(aoTexture) Then
                Try
                    MyMaterialBuilder.WithOcclusion(aoTexture)
                Catch ex As Exception
                End Try
            End If
            ' Assign Metallic and Roughness Texture if exists
            If Not String.IsNullOrEmpty(metalRoughTexture) Then
                ' GLTF PBR Metallic-Roughness uses a combined texture with metallic in the B channel and roughness in the G channel
                Try
                    MyMaterialBuilder.WithMetallicRoughness(metalRoughTexture)
                Catch ex As Exception
                End Try
            End If
            ' Assign Normal Map if exists
            If Not String.IsNullOrEmpty(normalTexture) Then
                Try
                    MyMaterialBuilder.WithNormal(normalTexture)
                Catch ex As Exception
                End Try
            End If


            Dim off = _group(item).startVertex_

            Dim model_name = _group(item).name.Replace("/", "\")
            model_name = model_name.Replace(":", "~")
            model_name = model_name.Replace("vehicles\", "")
            model_name = model_name.Replace("primitives_processed", "pri")
            model_name = model_name.Replace("\lod0\", "\l\")


            Dim MyMeshBuilder As Object = Nothing
            Dim prim As Object = Nothing

            If Not _group(item).has_uv2 = 1 And _group(item).has_color = 1 Then 'gun
                MyMeshBuilder = New MeshBuilder(Of VertexPositionNormal, VertexColor1Texture1, VertexEmpty)(model_name)
                prim = MyMeshBuilder.UsePrimitive(MyMaterialBuilder)

                ' Handle vertices with color and single texture coordinates
                Dim vertices As New List(Of VertexBuilder(Of VertexPositionNormal, VertexColor1Texture1, VertexEmpty))()

                For i As UInt32 = 0 To _group(item).nVertices_ - 1
                    Dim color1 = New Vector4(_group(item).vertices(i).r, _group(item).vertices(i).g, _group(item).vertices(i).b, _group(item).vertices(i).a)

                    Dim v As New VertexBuilder(Of VertexPositionNormal, VertexColor1Texture1, VertexEmpty)(
            New VertexPositionNormal(
                New Vector3(_group(item).vertices(i).x, _group(item).vertices(i).y, _group(item).vertices(i).z),
                New Vector3(_group(item).vertices(i).nx, _group(item).vertices(i).ny, _group(item).vertices(i).nz)
            ),
            New VertexColor1Texture1(color1, New Vector2(_group(item).vertices(i).u, _group(item).vertices(i).v))
        )
                    vertices.Add(v)
                Next

                ' Create mesh primitives using triangle indices
                For i As UInt32 = 1 To _group(item).nPrimitives_
                    prim.AddTriangle(
            vertices(_group(item).indices(i).v1 - off),
            vertices(_group(item).indices(i).v2 - off),
            vertices(_group(item).indices(i).v3 - off)
        )
                Next

            ElseIf _group(item).has_uv2 = 1 Then ' chassis
                MyMeshBuilder = New MeshBuilder(Of VertexPositionNormal, VertexColor2Texture2, VertexEmpty)(model_name)
                prim = MyMeshBuilder.UsePrimitive(MyMaterialBuilder)

                ' Handle vertices with two colors and two sets of texture coordinates
                Dim vertices As New List(Of VertexBuilder(Of VertexPositionNormal, VertexColor2Texture2, VertexEmpty))()

                For i As UInt32 = 0 To _group(item).nVertices_ - 1
                    Dim color1 = New Vector4(_group(item).vertices(i).r, _group(item).vertices(i).g, _group(item).vertices(i).b, _group(item).vertices(i).a)
                    Dim color2 = New Vector4(_group(item).vertices(i).ir, _group(item).vertices(i).ig, _group(item).vertices(i).ib, _group(item).vertices(i).ia)

                    Dim v As New VertexBuilder(Of VertexPositionNormal, VertexColor2Texture2, VertexEmpty)(
            New VertexPositionNormal(
                New Vector3(_group(item).vertices(i).x, _group(item).vertices(i).y, _group(item).vertices(i).z),
                New Vector3(_group(item).vertices(i).nx, _group(item).vertices(i).ny, _group(item).vertices(i).nz)
            ),
            New VertexColor2Texture2(color1, color2, New Vector2(_group(item).vertices(i).u, _group(item).vertices(i).v), New Vector2(_group(item).vertices(i).u2, _group(item).vertices(i).v2))
        )
                    vertices.Add(v)
                Next

                ' Create mesh primitives using triangle indices
                For i As UInt32 = 1 To _group(item).nPrimitives_
                    prim.AddTriangle(
            vertices(_group(item).indices(i).v1 - off),
            vertices(_group(item).indices(i).v2 - off),
            vertices(_group(item).indices(i).v3 - off)
        )
                Next

            ElseIf Not _group(item).has_color = 1 Then 'hull and turret
                MyMeshBuilder = New MeshBuilder(Of VertexPositionNormal, VertexTexture1, VertexEmpty)(model_name)
                prim = MyMeshBuilder.UsePrimitive(MyMaterialBuilder)

                ' Handle vertices with only position, normal, and texture coordinates
                Dim vertices As New List(Of VertexBuilder(Of VertexPositionNormal, VertexTexture1, VertexEmpty))()

                For i As UInt32 = 0 To _group(item).nVertices_ - 1
                    Dim v As New VertexBuilder(Of VertexPositionNormal, VertexTexture1, VertexEmpty)(
            New VertexPositionNormal(
                New Vector3(_group(item).vertices(i).x, _group(item).vertices(i).y, _group(item).vertices(i).z),
                New Vector3(_group(item).vertices(i).nx, _group(item).vertices(i).ny, _group(item).vertices(i).nz)
            ),
            New VertexTexture1(New Vector2(_group(item).vertices(i).u, _group(item).vertices(i).v))
        )
                    vertices.Add(v)
                Next

                ' Create mesh primitives using triangle indices
                For i As UInt32 = 1 To _group(item).nPrimitives_
                    prim.AddTriangle(
            vertices(_group(item).indices(i).v1 - off),
            vertices(_group(item).indices(i).v2 - off),
            vertices(_group(item).indices(i).v3 - off)
        )
                Next
            End If

            ' Create a node and add the mesh to it
            ' Convert the matrix directly from _group(item).matrix to System.Numerics.Matrix4x4
            Dim matrix As New Matrix4x4(
            _object(item).matrix(0), _object(item).matrix(1), _object(item).matrix(2), _object(item).matrix(3),
            _object(item).matrix(4), _object(item).matrix(5), _object(item).matrix(6), _object(item).matrix(7),
            _object(item).matrix(8), _object(item).matrix(9), _object(item).matrix(10), _object(item).matrix(11),
            _object(item).matrix(12), _object(item).matrix(13), _object(item).matrix(14), _object(item).matrix(15)
)
            matrix.M11 *= -1.0
            If _object(item).name.ToLower.Contains("turret") Or _object(item).name.ToLower.Contains("gun") Then
                matrix.M41 *= -1.0
            End If


            ' Create a dictionary to hold the texture paths
            Dim texturePaths As New Dictionary(Of String, String)

            'save the actual parts name in the game
            Dim lts As String = _group(item).long_tank_name
            texturePaths.Add("rootfolder", lts)

            ' Collect the full texture paths for PBR textures
            If Not String.IsNullOrEmpty(baseColorTexture) Then
                texturePaths.Add("baseColorTexture", baseColorTexture)
            End If
            If Not String.IsNullOrEmpty(aoTexture) Then
                texturePaths.Add("aoTexture", aoTexture)
            End If
            If Not String.IsNullOrEmpty(metalRoughTexture) Then
                texturePaths.Add("metalRoughTexture", metalRoughTexture)
            End If
            If Not String.IsNullOrEmpty(normalTexture) Then
                texturePaths.Add("normalTexture", normalTexture)
            End If

            Dim jsonExtras As JsonNode = JsonNode.Parse(JsonSerializer.Serialize(texturePaths))

            Dim affineTransform As AffineTransform = AffineTransform.CreateDecomposed(matrix)

            ' Attach the extras data to the empty node

            MyMeshBuilder.Extras = jsonExtras

            ' Add the node to the scene

            MySceneBuilder.AddRigidMesh(MyMeshBuilder, affineTransform)

        Next

        ' Add the root node to the scene

        ' Save the scene to a GLTF file with settings
        Dim settings As New SharpGLTF.Schema2.WriteSettings
        settings.JsonIndented = True
        settings.ImageWriting = ResourceWriteMode.Default
        settings.MergeBuffers = False

        MySceneBuilder.ToGltf2().SaveGLB(out_path, settings)
    End Sub

End Module
