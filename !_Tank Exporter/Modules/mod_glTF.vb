

Imports Aspose.ThreeD
Imports Aspose.ThreeD.Entities
Imports Aspose.ThreeD.Utilities
Imports Aspose.ThreeD.Shading
Imports System.Net.WebRequestMethods
Imports System.IO
Imports Skill.FbxSDK.FbxAxisSystem


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
        frmMain.SaveFileDialog1.InitialDirectory = My.Settings.fbx_path
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
        export_fbx_textures(False, 1) 'export all textures

        Dim scene_ As New Scene()

        For item = 1 To object_count
            If item = 23 Then
                Stop
            End If
            Dim off = _group(item).startVertex_

            Dim model_name = _group(item).name.Replace("/", "\")
            model_name = model_name.Replace(":", "~")
            model_name = model_name.Replace("vehicles\", "")
            model_name = model_name.Replace("primitives_processed", "pri")
            model_name = model_name.Replace("\lod0\", "\l\")

            Dim m As New Mesh 'create mesh
            m.Name = model_name

            'create mesh pirmitive face indice set
            For i As UInt32 = 1 To _group(item).nPrimitives_
                m.CreatePolygon(_group(item).indicies(i).v1 - off, _group(item).indicies(i).v2 - off,
                                _group(item).indicies(i).v3 - off)
            Next

            Dim norm As New VertexElementNormal
            ReDim normals(_group(item).nVertices_ - 1)
            For i As UInt32 = 0 To _group(item).nVertices_ - 1
                normals(i).X = _group(item).vertices(i).nx
                normals(i).Y = _group(item).vertices(i).ny
                If _group(item).color_name IsNot Nothing Then
                    If _group(item).color_name.ToLower.Contains("track") Then
                        normals(i).Y *= 1.0F
                    End If
                End If
                normals(i).Z = _group(item).vertices(i).nz
            Next
            norm.SetData(normals)
            m.AddElement(norm)

            Dim uvs_1 As New VertexElementUV()
            ReDim uvs(_group(item).nVertices_ - 1)
            For i = 0 To _group(item).nVertices_ - 1
                uvs(i).X = _group(item).vertices(i).u
                uvs(i).Y = -_group(item).vertices(i).v
            Next
            uvs_1.SetData(uvs)
            m.AddElement(uvs_1)

            If _group(item).has_uv2 = 1 Then
                ReDim uvs(_group(item).nVertices_ - 1)
                For i As UInt32 = 0 To _group(item).nVertices_ - 1
                    uvs(i).X = _group(item).vertices(i).u2
                    uvs(i).Y = -_group(item).vertices(i).v2
                Next

                Dim vElement2 As New VertexElementUV()
                vElement2.SetData(uvs)
                m.AddElement(vElement2)

            End If

            Dim vcolor As New VertexElementVertexColor
            ReDim normals(_group(item).nVertices_ - 1)
            For i As UInt32 = 0 To _group(item).nVertices_ - 1
                normals(i).X = _group(item).vertices(i).index_1
                normals(i).Y = _group(item).vertices(i).index_2
                normals(i).X = _group(item).vertices(i).index_3
                normals(i).W = _group(item).vertices(i).index_4
            Next
            vcolor.SetData(normals)

            m.AddElement(vcolor)

            For i As UInt32 = 0 To _group(item).nVertices_ - 1
                Dim v As Vector4
                v.X = _group(item).vertices(i).x
                v.Y = _group(item).vertices(i).y
                v.Z = _group(item).vertices(i).z
                m.ControlPoints.Add(v)
            Next
            Dim co As Vector3
            co.X = 0.6
            co.Y = 0.6
            co.Z = 0.6
            'Some turrets dont exist but are still used for translations.
            'If the are only a matrix transform, they have no textures!
            Dim m1 As New Object
            If _group(item).color_name IsNot Nothing Then

                ' Set up the base color texture
                Dim arr = _group(item).color_name.Split("\")
                Dim DnF = name + "\" + arr(arr.Length - 1)
                Dim tx As New Texture(DnF.Replace(".dds", ".png"))
                tx.FileName = tx.Name
                tx.MagFilter = TextureFilter.Linear
                tx.MinFilter = TextureFilter.Linear
                tx.MipFilter = TextureFilter.Anisotropic
                tx.EnableMipMap = True


                ' Set up the normal texture
                arr = _group(item).normal_name.Split("\")
                DnF = name + "\" + arr(arr.Length - 1)
                Dim txn As New Texture(DnF.Replace(".dds", ".png"))
                txn.FileName = txn.Name
                txn.MagFilter = TextureFilter.Linear
                txn.MinFilter = TextureFilter.Linear
                txn.MipFilter = TextureFilter.Anisotropic
                txn.EnableMipMap = True


                ' Set up the metallic-roughness texture
                arr = _group(item).metalGMM_name.Split("\")
                DnF = name + "\" + arr(arr.Length - 1)
                Dim txgm As New Texture(DnF.Replace(".dds", ".png"))
                txgm.FileName = txgm.Name
                txgm.MagFilter = TextureFilter.Linear
                txgm.MinFilter = TextureFilter.Linear
                txgm.MipFilter = TextureFilter.Anisotropic
                txgm.EnableMipMap = True
                ' Set up the AO texture
                Dim txao As New Texture
                If _group(item).ao_name IsNot Nothing Then

                    arr = _group(item).ao_name.Split("\")
                    DnF = name + "\" + arr(arr.Length - 1)
                    txao = New Texture(DnF.Replace(".dds", ".png"))
                    txao.FileName = txao.Name
                    txao.MagFilter = TextureFilter.Linear
                    txao.MinFilter = TextureFilter.Linear
                    txao.MipFilter = TextureFilter.Anisotropic
                    txao.EnableMipMap = True
                End If

                Select Case EXPORT_TYPE
                    Case 2, 3, 4
                        m1 = New LambertMaterial()
                        m1.SetTexture(Material.MapDiffuse, tx)
                        m1.SetTexture(Material.MapNormal, txn)
                        m1.AmbientColor = New Vector3(0.3, 0.3, 0.3)
                        m1.DiffuseColor = New Vector3(0.7, 0.7, 0.7)

                        Exit Select
                    Case 1
                        ' Create a PBR material with a base color

                        m1 = New PbrMaterial(co)

                        ' Assign textures to the PBR material
                        m1.AlbedoTexture = tx
                        m1.NormalTexture = txn
                        m1.MetallicRoughness = txgm
                        If _group(item).ao_name IsNot Nothing Then
                            m1.OcclusionTexture = txao
                        End If
                        ' Set PBR material properties
                        m1.MetallicFactor = 0.9
                        m1.RoughnessFactor = 0.9

                        Exit Select

                End Select
                m1.Name = "Material00" + item.ToString
                m1.SetProperty("Specular", 0.05)
                m1.SetProperty("Shininess", 0.1)

            End If
            Dim base = scene_.RootNode.CreateChildNode(model_name, m)
            base.Name = model_name

            If _group(item).color_name IsNot Nothing Then
                base.Material = m1

            End If

            base.Entity = m


            Dim mat As Matrix4
            Dim tMatrix(16) As Double
            For i As UInt32 = 0 To 15
                tMatrix(i) = _object(item).matrix(i)
            Next

            mat.m00 = tMatrix(0)
            mat.m01 = tMatrix(1)
            mat.m02 = tMatrix(2)
            mat.m03 = tMatrix(3)

            mat.m10 = tMatrix(4)
            mat.m11 = tMatrix(5)
            mat.m12 = tMatrix(6)
            mat.m13 = tMatrix(7)

            mat.m20 = tMatrix(8)
            mat.m21 = tMatrix(9)
            mat.m22 = tMatrix(10)
            mat.m23 = tMatrix(11)

            mat.m30 = tMatrix(12)
            mat.m31 = tMatrix(13)
            mat.m32 = tMatrix(14)
            mat.m33 = tMatrix(15)


            'Apply the matrix to the node
            If EXPORT_TYPE = 2 Then

                Dim rs As Quaternion
                Dim ts, ss As Vector3
                Dim err = mat.Decompose(ts, ss, rs)
                rs.Normalize()
                Dim r_v = New Vector4(rs.X, 0.0, rs.Z, 0)
                Dim t_v = New Vector3(-ts.X, ts.Y, ts.Z)
                Dim s_v = New Vector3(-ss.X, ss.Y, ss.Z)
                If _group(item).color_name IsNot Nothing Then

                    If _group(item).color_name.ToLower.Contains("chassis") Then
                        s_v.Z *= -1.0
                        s_v.Y *= -1.0
                    End If
                    If _group(item).color_name.ToLower.Contains("tracks") Then
                        s_v.Z *= -1.0
                        s_v.Y *= -1.0
                    End If
                    If _group(item).color_name.ToLower.Contains("gun") Then
                        s_v.Z *= -1.0
                        s_v.Y *= -1.0
                    End If
                End If
                base.Transform.Scaling = s_v
                base.Transform.Translation = t_v
                base.Transform.Rotation = rs
                ' Convert quaternion to Euler angles
                Dim eulerAngles As Vector3 = rs.EulerAngles()

                ' Check for NaN values and handle them
                If Double.IsNaN(eulerAngles.X) Then eulerAngles.X = 0.0
                If Double.IsNaN(eulerAngles.Y) Then eulerAngles.Y = 0.0
                If Double.IsNaN(eulerAngles.Z) Then eulerAngles.Z = 0.0


            Else
                mat.m00 = tMatrix(0) * -1.0
                If _object(item).name.ToLower.Contains("turret") Then
                    mat.m30 = tMatrix(12) * -1.0
                    'mat.m00 = tMatrix(0) * -1.0
                    'mat.m02 *= -1
                    'mat.m12 *= -1
                    'mat.m20 *= -1
                    'mat.m21 *= -1

                End If

                base.Transform.TransformMatrix = mat
            End If



            scene_.RootNode.ChildNodes.Add(base)

            Debug.WriteLine(m.Name)
        Next


        ' Export the scn to a 3D format (e.g., FBX)
        Select Case EXPORT_TYPE
            Case 1
                scene_.Save(out_path, FileFormat.GLTF2)
                Exit Select
            Case 2
                scene_.Save(out_path, FileFormat.FBX7300Binary)
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
