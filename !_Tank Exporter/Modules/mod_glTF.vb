

Imports Aspose.ThreeD
Imports Aspose.ThreeD.Entities
Imports Aspose.ThreeD.Utilities
Imports Aspose.ThreeD.Shading
Imports System.Net.WebRequestMethods
Imports System.IO
Imports SharpDX.Direct3D11

Module mod_glTF
    Public EXPORT_TYPE As Integer = 1
    Public Sub make_glTF()

        Dim ar() As String
        If PRIMITIVES_MODE Then
            ar = Path.GetFileNameWithoutExtension(frmMain.OpenFileDialog1.FileName).Split("~")
        Else
            ar = TANK_NAME.Split(":")
        End If
        file_name = current_tank_name
        Select Case EXPORT_TYPE
            Case 1
                frmMain.SaveFileDialog1.Filter = "glTF|*.glTF"
                frmMain.SaveFileDialog1.Title = "Save glTF.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".glTF"
                Exit Select
            Case 2
                frmMain.SaveFileDialog1.Filter = "FBX|*.fbx"
                frmMain.SaveFileDialog1.Title = "Save FBX.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".fbx"
                Exit Select
            Case 3
                frmMain.SaveFileDialog1.Filter = "OBJ|*.obj"
                frmMain.SaveFileDialog1.Title = "Save OBJ.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".obj"
                Exit Select
            Case 4
                frmMain.SaveFileDialog1.Filter = "Collada|*.dae"
                frmMain.SaveFileDialog1.Title = "Save Collada.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".dae"
                Exit Select
        End Select
        frmMain.SaveFileDialog1.AddExtension = True
        frmMain.SaveFileDialog1.RestoreDirectory = True


        Dim result = frmMain.SaveFileDialog1.ShowDialog
        Dim out_path = frmMain.SaveFileDialog1.FileName

        If Not result = DialogResult.OK Then
            Return
        End If
        My.Settings.fbx_path = out_path

        Dim name As String = Path.GetFileName(ar(0))
        Dim save_path = Path.GetDirectoryName(My.Settings.fbx_path) + "\" + name
        export_fbx_textures(False) 'export all textures

        Dim scene_ As New Scene()
        For item = 1 To object_count

            Dim model_name = _group(item).name.Replace("/", "\")
            model_name = model_name.Replace(":", "~")
            model_name = model_name.Replace("vehicles\", "")
            model_name = model_name.Replace("primitives_processed", "pri")
            model_name = model_name.Replace("\lod0\", "\l\")

            Dim m As New Mesh
            Dim mmode As MappingMode
            mmode = 0 'PolygonVertex
            For i = 0 To _group(item).indicies.Length - 1

                m.CreatePolygon(_group(item).indicies(i).v1, _group(item).indicies(i).v2, _group(item).indicies(i).v3)
            Next

            Dim norm As New VertexElementNormal
            ReDim normals(_group(item).vertices.Length - 2)
            For i = 0 To _group(item).vertices.Length - 2
                normals(i).x = _group(item).vertices(i).nx
                normals(i).y = _group(item).vertices(i).ny
                normals(i).x = _group(item).vertices(i).nz
            Next
            norm.SetData(normals)
            m.AddElement(norm)

            Dim uvs_1 As New VertexElementUV()
            ReDim uvs(_group(item).vertices.Length - 2)
            For i = 0 To _group(item).vertices.Length - 2
                uvs(i).x = _group(item).vertices(i).u
                uvs(i).y = -_group(item).vertices(i).v
            Next
            uvs_1.SetData(uvs)
            m.AddElement(uvs_1)





            If _group(item).has_uv2 = 1 Then
                ReDim uvs(_group(item).vertices.Length - 2)
                For i = 0 To _group(item).vertices.Length - 2
                    uvs(i).x = _group(item).vertices(i).u2
                    uvs(i).y = -_group(item).vertices(i).v2
                Next

                Dim vElement2 As New VertexElementUV()
                vElement2.SetData(uvs)
                m.AddElement(vElement2)

            End If

            Dim vcolor As New VertexElementVertexColor
            ReDim normals(_group(item).vertices.Length - 2)
            For i = 0 To _group(item).vertices.Length - 2
                normals(i).x = _group(item).vertices(i).index_1
                normals(i).y = _group(item).vertices(i).index_2
                normals(i).x = _group(item).vertices(i).index_3
                normals(i).w = _group(item).vertices(i).index_4
            Next
            vcolor.SetData(normals)

            m.AddElement(vcolor)

            ReDim vertices(_group(item).vertices.Length - 2)

            For i = 0 To _group(item).vertices.Length - 2
                Dim v As Vector4
                v.x = _group(item).vertices(i).x
                v.y = _group(item).vertices(i).y
                v.z = _group(item).vertices(i).z

                m.ControlPoints.Add(v)
            Next

            m.ControlPoints.AddRange(vertices)
            Dim uvscale As Vector2
            uvscale.x = 6.0
            uvscale.y = 6.0

            Dim co As Vector3
            co.x = 0.6
            co.y = 0.6
            co.z = 0.6
            Dim m1 As New Object

            Dim tx As New Texture(save_path + "\" + Path.GetFileName(_group(item).color_name).Replace(".dds", ".png"))
            tx.FileName = tx.Name
            tx.SetProperty("mix vertex color", 0.0)

            Dim txn As New Texture(save_path + "\" + Path.GetFileName(_group(item).normal_name).Replace(".dds", ".png"))
            txn.FileName = txn.Name

            '  tx.UVScale() = uvscale
            tx.MagFilter = TextureFilter.Linear
            tx.MinFilter = TextureFilter.Linear
            tx.MipFilter = TextureFilter.Anisotropic
            tx.EnableMipMap = True

            ' txn.UVScale() = uvscale
            txn.SetProperty("Strength", 1.0)
            txn.MagFilter = TextureFilter.Linear
            txn.MinFilter = TextureFilter.Linear
            txn.MipFilter = TextureFilter.Anisotropic
            txn.EnableMipMap = True

            Select Case EXPORT_TYPE
                Case 2, 3, 4
                    m1 = New LambertMaterial("")
                    m1.SetTexture(Material.MapDiffuse, tx)
                    m1.SetTexture(Material.MapNormal, txn)
                    Exit Select
                Case 1
                    m1 = New PbrMaterial(co)
                    Dim txgm As New Texture(save_path + "\" + Path.GetFileName(_group(item).metalGMM_name).Replace(".dds", ".png"))
                    txgm.FileName = txgm.Name
                    txgm.UVScale() = uvscale
                    txgm.SetProperty("Strength", 1.0)
                    txgm.MagFilter = TextureFilter.Linear
                    txgm.MinFilter = TextureFilter.Linear
                    txgm.MipFilter = TextureFilter.Anisotropic
                    txgm.EnableMipMap = True

                    m1.AlbedoTexture() = tx
                    m1.NormalTexture() = txn
                    m1.MetallicRoughness() = txgm

                    m1.MetallicFactor = 1.0
                    m1.RoughnessFactor = 1.0
                    Exit Select

            End Select
            m1.Name = "Material00" + item.ToString
            'Dim txao As New Texture
            If Not _group(item).ao_name Is Nothing Then
                'txao = New Texture(save_path + "/" + Path.GetFileName(_group(item).ao_name).Replace(".dds", ".png"))

                'txao.FileName = txao.Name
            End If


            m1.SetProperty("Specular", 0.05)
            m1.SetProperty("Shininess", 0.0)

            m.Name = model_name

            Dim base As Node = scene_.RootNode.CreateChildNode(model_name, m)
            base.Name = model_name
            base.Transform.TransformMatrix = Matrix4.Identity
            Dim mat As Matrix4

            mat.m00 = _object(item).matrix(0)
            mat.m01 = _object(item).matrix(1)
            mat.m02 = _object(item).matrix(2)
            mat.m03 = _object(item).matrix(3)

            mat.m10 = _object(item).matrix(4)
            mat.m11 = _object(item).matrix(5)
            mat.m12 = _object(item).matrix(6)
            mat.m13 = _object(item).matrix(7)

            mat.m20 = _object(item).matrix(8)
            mat.m21 = _object(item).matrix(9)
            mat.m22 = _object(item).matrix(10)
            mat.m23 = _object(item).matrix(11)

            mat.m30 = _object(item).matrix(12)
            mat.m31 = _object(item).matrix(13)
            mat.m32 = _object(item).matrix(14)
            mat.m33 = _object(item).matrix(15)
            Dim rq As Quaternion
            Dim tv As Vector3
            Dim sv As Vector3
            mat.Decompose(tv, sv, rq)
            round_error(rq.x)
            round_error(rq.y)
            round_error(rq.z)
            round_error(rq.w)
            sv.x *= -1
            tv.x *= -1

            If _object(item).name.ToLower.Contains("gun") Then
                sv.y *= -1
                sv.z *= -1
            End If
            If _object(item).name.ToLower.Contains("chassis") Then
                sv.y *= -1
                sv.z *= -1
            End If

            base.Entity = m
            base.Material = m1
            base.Transform.SetRotation(rq.w, rq.x, rq.y, rq.z)
            base.Transform.SetTranslation(tv.x, tv.y, tv.z)
            base.Transform.SetScale(sv.x, sv.y, sv.z)
            scene_.RootNode.ChildNodes.Add(base)

            Debug.WriteLine(m.Name)
        Next


        ' Export the scn to a 3D format (e.g., FBX)
        Select Case EXPORT_TYPE
            Case 1
                scene_.Save(out_path, FileFormat.GLTF2)
                Exit Select
            Case 2
                scene_.Save(out_path, FileFormat.FBX7700Binary)
                Exit Select
            Case 3
                scene_.Save(out_path, FileFormat.WavefrontOBJ)
                Exit Select
            Case 4
                scene_.Save(out_path, FileFormat.Collada)
                Exit Select
        End Select
        'scn.Save(out_path, FileFormat.Collada)

    End Sub

    Private Sub round_error(ByRef val As Single)
        val = Math.Round(val, 6, MidpointRounding.AwayFromZero)
    End Sub
End Module
