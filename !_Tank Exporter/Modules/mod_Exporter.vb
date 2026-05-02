

'Imports System.Net.WebRequestMethods
Imports System.IO
Imports System.Runtime.InteropServices
Imports Aspose.ThreeD
Imports Aspose.ThreeD.Entities
Imports Aspose.ThreeD.Formats
Imports Aspose.ThreeD.Shading
Imports Aspose.ThreeD.Utilities
'Imports Skill.FbxSDK.FbxAxisSystem


Module mod_Exporter

    ' ── fbx_reader.dll P/Invoke declarations ────────────────────────────────
    ' Mirrors the FbxVertex struct in fbx_reader.h (pragma pack 1, 48 bytes).
    ' All Aspose.3D export transforms are already reversed inside the DLL.
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure FbxVertex_Native
        Public x As Single, y As Single, z As Single         ' world position
        Public nx As Single, ny As Single, nz As Single      ' normal (X already un-negated)
        Public u As Single, v As Single                      ' UV0  (v already = 1-stored)
        Public u2 As Single, v2 As Single                    ' UV1  (v2 already = -stored)
        Public index_1 As Byte, index_2 As Byte, index_3 As Byte, index_4 As Byte    ' bone indices
        Public weight_1 As Byte, weight_2 As Byte, weight_3 As Byte, weight_4 As Byte ' bone weights
    End Structure

    ' Triangle face — three vertex indices into the vertex array.
    <StructLayout(LayoutKind.Sequential)>
    Public Structure FbxFace_Native
        Public v1 As UInteger, v2 As UInteger, v3 As UInteger
    End Structure

    ' Load a binary FBX file.  Returns an opaque scene handle, or IntPtr.Zero on failure.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl, CharSet:=CharSet.Ansi)>
    Private Function fbx_load(ByVal path As String) As IntPtr
    End Function

    ' Release all memory allocated by fbx_load.  Safe to call with IntPtr.Zero.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Sub fbx_free(ByVal scene As IntPtr)
    End Sub

    ' Number of meshes in the scene.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_count(ByVal scene As IntPtr) As Integer
    End Function

    ' Display name of mesh[idx] (from the FBX Model node).  Returns a const char* — use PtrToStringAnsi.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_name(ByVal scene As IntPtr, ByVal idx As Integer) As IntPtr
    End Function

    ' Diffuse texture filename for mesh[idx].  Returns "" if none.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_diffuse(ByVal scene As IntPtr, ByVal idx As Integer) As IntPtr
    End Function

    ' Normal-map texture filename for mesh[idx].  Returns "" if none.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_normal_tex(ByVal scene As IntPtr, ByVal idx As Integer) As IntPtr
    End Function

    ' Number of deduplicated vertices in mesh[idx].
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_vert_count(ByVal scene As IntPtr, ByVal idx As Integer) As Integer
    End Function

    ' Number of triangles in mesh[idx].
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_face_count(ByVal scene As IntPtr, ByVal idx As Integer) As Integer
    End Function

    ' 1 if mesh[idx] has a second UV channel (u2/v2 populated), else 0.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_has_uv2(ByVal scene As IntPtr, ByVal idx As Integer) As Integer
    End Function

    ' Number of vertex-colour channels (0=no skinning, 1=indices only, 2=indices+weights).
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_color_ch(ByVal scene As IntPtr, ByVal idx As Integer) As Integer
    End Function

    ' Copy all vertices of mesh[idx] into a pinned FbxVertex_Native array.
    ' out must hold at least fbx_mesh_vert_count elements.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Sub fbx_mesh_get_verts(ByVal scene As IntPtr, ByVal idx As Integer, ByVal out As IntPtr)
    End Sub

    ' Copy all triangle faces of mesh[idx] into a pinned FbxFace_Native array.
    ' out must hold at least fbx_mesh_face_count elements.
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Sub fbx_mesh_get_faces(ByVal scene As IntPtr, ByVal idx As Integer, ByVal out As IntPtr)
    End Sub

    ' Returns a pointer to 16 doubles — column-major 4x4 local transform matrix.
    ' Built from Lcl Translation/Rotation/Scaling (Euler XYZ degrees).
    ' Translation at [12],[13],[14].  Copy immediately; valid only until fbx_free().
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_matrix(ByVal scene As IntPtr, ByVal idx As Integer) As IntPtr
    End Function

    ' Returns a pointer to the "primitive_table" custom property string for mesh[idx].
    ' Written by export_aspose_fbx via base.SetProperty("primitive_table", ...).
    ' e.g. "hull/lod0/indices" — the section name in .primitives_processed.
    ' Returns pointer to "" if not present (older FBX files). Valid until fbx_free().
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_primitive_table(ByVal scene As IntPtr, ByVal idx As Integer) As IntPtr
    End Function

    ' Returns the "primitive_index" custom property for mesh[idx].
    ' The 1-based _group() slot index stored at export time.
    ' Returns -1 if not present (older FBX files).
    <DllImport("fbx_reader.dll", CallingConvention:=CallingConvention.Cdecl)>
    Private Function fbx_mesh_primitive_index(ByVal scene As IntPtr, ByVal idx As Integer) As Integer
    End Function
    ' ─────────────────────────────────────────────────────────────────────────
    Public EXPORT_TYPE As Integer = 1
    Public Sub export_aspose_fbx()
        Dim normals() As Aspose.ThreeD.Utilities.Vector4
        Dim uvs() As Aspose.ThreeD.Utilities.Vector4
        Dim ar() As String
        If PRIMITIVES_MODE Then
            ar = Path.GetFileNameWithoutExtension(frmMain.OpenFileDialog1.FileName).Split("~")
        Else
            ar = TANK_NAME.Split(":")
        End If
        file_name = current_tank_name
        frmMain.SaveFileDialog1.InitialDirectory = My.Settings.fbx_path
        EXPORT_TYPE = 2
        Select Case EXPORT_TYPE
            Case 1
                frmMain.SaveFileDialog1.Filter = "GLB|*.glb"
                frmMain.SaveFileDialog1.Title = "Save GLB.."
                frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".glb"
                Exit Select
            Case 2
                frmMain.SaveFileDialog1.Filter = "FBX|*.fbx"
                frmMain.SaveFileDialog1.Title = "Save FBX.."

                If CRASH_MODE Then
                    frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + "_CRASHED.fbx"
                Else
                    frmMain.SaveFileDialog1.FileName = Path.GetFileName(ar(0)) + ".fbx"
                End If

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
        Dim save_path = Path.GetDirectoryName(out_path) + "\" + name
        export_fbx_textures(False, 0) 'export all textures

        Dim scene_ As New Scene()
        scene_.Name = TANK_NAME

        For item = 1 To object_count
            If item = 23 Then
                Stop
            End If
            Dim off = 0 '_group(item).startVertex_

            Dim model_name = _group(item).name.Replace("/", "\")
            model_name = model_name.Replace(":", "~")
            model_name = model_name.Replace("vehicles\", "")
            model_name = model_name.Replace("primitives_processed", "pri")
            model_name = model_name.Replace("\lod0\", "\l\")

            Dim m As New Mesh 'create mesh
            m.Name = model_name
            Dim base = scene_.RootNode.CreateChildNode(model_name)

            'create mesh pirmitive face indice set
            If Not String.IsNullOrEmpty(_group(item).color_name) Then

                If _group(item).color_name.Contains("turret") Or
                        _group(item).color_name.Contains("hull") Then

                    For i As UInteger = 1 To _group(item).nPrimitives_
                        m.CreatePolygon(_group(item).indices(i).v1 - off, _group(item).indices(i).v2 - off,
                                      _group(item).indices(i).v3 - off)
                    Next
                Else
                    For i As UInteger = 1 To _group(item).nPrimitives_
                        m.CreatePolygon(_group(item).indices(i).v1 - off, _group(item).indices(i).v2 - off,
                                      _group(item).indices(i).v3 - off)
                    Next

                End If
            Else
                For i As UInteger = 1 To _group(item).nPrimitives_
                    m.CreatePolygon(_group(item).indices(i).v1 - off, _group(item).indices(i).v2 - off,
                                      _group(item).indices(i).v3 - off)
                Next
            End If

            Dim norm As New VertexElementNormal
            ReDim normals(_group(item).nVertices_ - 1)
            'fix_winding_order_group(item)

            Dim isTrack As Boolean = _group(item).color_name.Contains("track")

            For i As UInt32 = 0 To _group(item).nVertices_ - 1
                normals(i).X = -_group(item).vertices(i).nx
                normals(i).Y = _group(item).vertices(i).ny
                normals(i).Z = If(isTrack, -_group(item).vertices(i).nz, _group(item).vertices(i).nz)
            Next

            If _group(item).color_name IsNot Nothing Then

                ' Set up the base color texture
                Dim DnF = Path.GetFileName(_group(item).color_name)
                Dim tx As New Texture(DnF.Replace(".dds", ".png"))
                tx.FileName = tx.Name
                'tx.Name = tx.FileName
                tx.MagFilter = TextureFilter.Linear
                tx.MinFilter = TextureFilter.Linear
                tx.MipFilter = TextureFilter.Anisotropic
                tx.EnableMipMap = True


                ' Set up the normal texture
                DnF = Path.GetFileName(_group(item).normal_name)
                Dim txn As New Texture(DnF.Replace(".dds", ".png"))
                txn.FileName = txn.Name
                'txn.Name = txn.FileName
                txn.MagFilter = TextureFilter.Linear
                txn.MinFilter = TextureFilter.Linear
                txn.MipFilter = TextureFilter.Anisotropic
                txn.EnableMipMap = True


                ' Set up the metallic-roughness texture
                DnF = Path.GetFileName(_group(item).GMM_name)
                Dim txgm As New Texture(DnF.Replace(".dds", ".png"))
                txgm.FileName = txgm.Name
                'txgm.Name = txgm.FileName
                txgm.MagFilter = TextureFilter.Linear
                txgm.MinFilter = TextureFilter.Linear
                txgm.MipFilter = TextureFilter.Anisotropic
                txgm.EnableMipMap = True
                ' Set up the AO texture
                Dim txao As New Texture


                If _group(item).ao_name IsNot Nothing Then

                    DnF = Path.GetFileName(_group(item).ao_name)
                    txao = New Texture(DnF.Replace(".dds", ".png"))
                    txao.FileName = txao.Name
                    'txao.Name = txao.FileName
                    txao.MagFilter = TextureFilter.Linear
                    txao.MinFilter = TextureFilter.Linear
                    txao.MipFilter = TextureFilter.Anisotropic
                    txao.EnableMipMap = True
                End If

                norm.SetData(normals)
                m.AddElement(norm)

                Dim uvs_1 As New VertexElementUV()
                ReDim uvs(_group(item).nVertices_ - 1)
                For i = 0 To _group(item).nVertices_ - 1
                    uvs(i).X = _group(item).vertices(i).u
                    uvs(i).Y = (-_group(item).vertices(i).v) + 1
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
                If _group(item).has_color = 1 Then
                    Dim vcolor As New VertexElementVertexColor
                    ReDim normals(_group(item).nVertices_ - 1)
                    For i As UInt32 = 0 To _group(item).nVertices_ - 1
                        normals(i).X = CDbl(_group(item).vertices(i).r)
                        normals(i).Y = CDbl(_group(item).vertices(i).g)
                        normals(i).Z = CDbl(_group(item).vertices(i).b)
                        normals(i).W = CDbl(_group(item).vertices(i).a)
                    Next
                    vcolor.SetData(normals)

                    m.AddElement(vcolor)

                    Dim vcolor2 As New VertexElementVertexColor
                    ReDim normals(_group(item).nVertices_ - 1)
                    For i As UInt32 = 0 To _group(item).nVertices_ - 1
                        normals(i).X = _group(item).vertices(i).ir
                        normals(i).Y = _group(item).vertices(i).ig
                        normals(i).Z = _group(item).vertices(i).ib
                        normals(i).W = _group(item).vertices(i).ia
                    Next
                    vcolor2.SetData(normals)
                    m.AddElement(vcolor2)
                End If


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


                Select Case EXPORT_TYPE
                    Case 2, 3, 4
                        Dim m2 = New LambertMaterial("material_" + scene_.RootNode.Materials.Count.ToString)
                        scene_.RootNode.Materials.Add(m2)

                        m2.Name = "Material00" + item.ToString

                        m2.SetTexture(Material.MapDiffuse, tx)
                        m2.SetTexture(Material.MapNormal, txn)
                        m2.AmbientColor = New Vector3(0.3, 0.3, 0.3)
                        m2.DiffuseColor = New Vector3(0.7, 0.7, 0.7)
                        base.Material = scene_.RootNode.Materials(scene_.RootNode.Materials.Count - 1)
                        Exit Select
                    Case 1
                        ' Create a PBR material with a base color
                        ' Assign textures to the PBR material
                        Dim m1 = New PbrMaterial(co)
                        scene_.RootNode.Materials.Add(m1)
                        m1.AlbedoTexture = tx
                        m1.NormalTexture = txn
                        m1.MetallicRoughness = txgm
                        If _group(item).ao_name IsNot Nothing Then
                            m1.OcclusionTexture = txao
                        End If
                        ' Set PBR material properties
                        m1.MetallicFactor = 0.9
                        m1.RoughnessFactor = 0.9
                        m1.Name = "Material00" + scene_.RootNode.Materials.Count.ToString
                        m1.SetProperty("Specular", 0.05)
                        m1.SetProperty("Shininess", 0.1)
                        base.Material = scene_.RootNode.Materials(scene_.RootNode.Materials.Count - 1)
                        Exit Select

                End Select
            End If

            If _group(item).color_name IsNot Nothing Then

            End If



            base.Name = model_name
            ' ── write primitive table name as FBX custom property ─────────────
            ' This embeds the exact section_names entry (e.g. "hull/lod0/indices")
            ' onto the Model node in Properties70. When the FBX is re-imported,
            ' fbx_reader can read this directly to sort meshes back into the
            ' correct slot without relying on Blender's export order or ~ naming.
            If Not String.IsNullOrEmpty(_group(item).table_entry_name) Then
                base.SetProperty("primitive_table", _group(item).table_entry_name)
            End If
            ' Also store the 1-based item index as a fallback integer slot reference.
            base.SetProperty("primitive_index", item)


            'base.Entity = m
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
            If EXPORT_TYPE = 2 Or EXPORT_TYPE = 3 Then

                Dim rs As Quaternion
                Dim ts, ss As Vector3
                Dim err = mat.Decompose(ts, ss, rs)
                rs.Normalize()
                Dim t_v = New Vector3(-ts.X, ts.Y, ts.Z)
                Dim s_v = New Vector3(-ss.X, ss.Y, ss.Z)
                If _group(item).color_name IsNot Nothing Then

                    If _group(item).color_name.ToLower.Contains("chassis") And Not CRASH_MODE Then
                        s_v.Z *= -1.0
                        s_v.Y *= -1.0
                    End If

                    If _group(item).color_name.ToLower.Contains("tracks") Then
                        s_v.Z *= -1.0
                        s_v.Y *= -1.0
                    End If
                    If _group(item).color_name.ToLower.Contains("gun") And Not CRASH_MODE Then
                        s_v.Z *= -1.0
                        s_v.Y *= -1.0
                    End If
                End If

                base.Transform.Scaling = s_v
                base.Transform.Translation = t_v
                base.Transform.Rotation = rs



            Else
                mat.m00 = tMatrix(0) * -1.0
                If _object(item).name.ToLower.Contains("turret") Then
                    mat.m30 = tMatrix(12) * -1.0
                End If

                base.Transform.TransformMatrix = mat
            End If


            ' Ensure the material is added to the root node's materials collection
            base.AddEntity(m)

            Debug.WriteLine(m.Name)
        Next


        ' Export the scn to a 3D format (e.g., FBX)
        Select Case EXPORT_TYPE
            Case 1
                Dim save_options As New GltfSaveOptions(FileFormat.GLTF2_Binary)
                save_options.FileName = out_path
                save_options.EmbedAssets = True
                save_options.ExportTextures = False
                save_options.SaveExtras = True
                scene_.Save(save_options.FileName, save_options)
                Exit Select
            Case 2
                Dim save_options As New FbxSaveOptions(FileFormat.FBX7400Binary)
                save_options.EmbedTextures = False
                save_options.VideoForTexture = False
                save_options.GenerateVertexElementMaterial = True
                scene_.Save(out_path, save_options)
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


    Public Sub import_fbx_homebrew()
        frmMain.OpenFileDialog1.Filter = "FBX|*.fbx"
        frmMain.OpenFileDialog1.Title = "Open FBX.."
        frmMain.OpenFileDialog1.FileName = My.Settings.fbx_path
        If My.Settings.fbx_path.Length = 0 Then My.Settings.fbx_path = "C:\"
        frmMain.OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(My.Settings.fbx_path)
        If frmMain.OpenFileDialog1.ShowDialog = DialogResult.Cancel Then Return
        My.Settings.fbx_path = frmMain.OpenFileDialog1.FileName
        My.Settings.Save()

        Dim open_path = My.Settings.fbx_path
        CRASH_MODE = open_path.ToLower.Contains("crash")   ' reset each load — was never cleared before

        frmMain.clean_house()
        remove_loaded_fbx()
        frmMain.info_Label.Visible = True

        Dim hScene As IntPtr = IntPtr.Zero

        Try
            frmMain.info_Label.Text = "loading FBX: " & open_path
            Application.DoEvents()

            ' ── load file via DLL ────────────────────────────────────────────
            hScene = fbx_load(open_path)
            If hScene = IntPtr.Zero Then
                Throw New Exception("fbx_reader.dll could not parse: " & open_path)
            End If

            Dim meshCount = fbx_mesh_count(hScene)
            If meshCount = 0 Then Throw New Exception("No meshes found in FBX file.")

            ' fbxgrp / _group are 1-based; index 0 is unused
            ReDim Preserve fbxgrp(meshCount)
            ReDim Preserve _group(meshCount)

            For i = 0 To meshCount - 1
                Dim item = i + 1   ' 1-based slot
                frmMain.info_Label.Text = "reading model: " & item.ToString
                Application.DoEvents()

                ' ── name & flags ─────────────────────────────────────────────
                Dim meshName = Marshal.PtrToStringAnsi(fbx_mesh_name(hScene, i))
                fbxgrp(item).name = meshName
                fbxgrp(item).is_new_model = Not meshName.ToLower.Contains("~")
                _group(item).name = meshName
                _group(item).is_new_model = fbxgrp(item).is_new_model
                ' ── primitive ordering tags ───────────────────────────────────
                ' Written by export_aspose_fbx via base.SetProperty().
                ' primitive_table  = the section name in .primitives_processed
                '                    e.g. "hull/lod0/indices"
                ' primitive_index  = the 1-based _group() slot index at export time.
                ' Both are "" / -1 on older FBX files that pre-date this feature;
                ' process_fbx_data() falls back to the ~ name convention in that case.
                Dim rawTable = Marshal.PtrToStringAnsi(fbx_mesh_primitive_table(hScene, i))
                fbxgrp(item).primitive_table = If(String.IsNullOrEmpty(rawTable), Nothing, rawTable)
                fbxgrp(item).primitive_index = fbx_mesh_primitive_index(hScene, i)

                ' ── texture filenames ─────────────────────────────────────────
                fbxgrp(item).color_name = Marshal.PtrToStringAnsi(fbx_mesh_diffuse(hScene, i))
                fbxgrp(item).normal_name = Marshal.PtrToStringAnsi(fbx_mesh_normal_tex(hScene, i))
                If String.IsNullOrEmpty(fbxgrp(item).color_name) Then fbxgrp(item).color_name = Nothing
                If String.IsNullOrEmpty(fbxgrp(item).normal_name) Then fbxgrp(item).normal_name = Nothing
                _group(item).color_name = fbxgrp(item).color_name
                _group(item).normal_name = fbxgrp(item).normal_name

                ' ── vertex data ───────────────────────────────────────────────
                Dim vc = fbx_mesh_vert_count(hScene, i)
                Dim fc = fbx_mesh_face_count(hScene, i)
                fbxgrp(item).nVertices_ = vc
                fbxgrp(item).nPrimitives_ = fc
                Debug.WriteLine("vc " + vc.ToString)
                Debug.WriteLine("face cnt" + fc.ToString)
                ' Determine stride from colour channel count:
                '   0 channels → 32  (pos+normal+UV0)
                '   1 channel  → 37  (+ bone indices)
                '   2 channels → 40  (+ bone indices + weights)
                Dim colorCh = fbx_mesh_color_ch(hScene, i)
                fbxgrp(item).has_color = If(colorCh > 0, 1, 0)
                fbxgrp(item).has_uv2 = fbx_mesh_has_uv2(hScene, i)
                Select Case colorCh
                    Case 1 : fbxgrp(item).stride = 37
                    Case 2 : fbxgrp(item).stride = 40
                    Case Else : fbxgrp(item).stride = 32
                End Select

                ' ── copy vertices from DLL into managed array ─────────────────
                ' Pin a native-layout array, hand its address to the DLL, then unpin.
                Dim nativeVerts(vc - 1) As FbxVertex_Native
                Dim hvPin = GCHandle.Alloc(nativeVerts, GCHandleType.Pinned)
                fbx_mesh_get_verts(hScene, i, hvPin.AddrOfPinnedObject())
                hvPin.Free()


                ReDim fbxgrp(item).vertices(vc - 1)
                For v = 0 To vc - 1
                    fbxgrp(item).vertices(v) = New vertice_
                    With nativeVerts(v)
                        fbxgrp(item).vertices(v).x = .x : fbxgrp(item).vertices(v).y = .y : fbxgrp(item).vertices(v).z = .z
                        fbxgrp(item).vertices(v).nx = .nx : fbxgrp(item).vertices(v).ny = .ny : fbxgrp(item).vertices(v).nz = - .nz
                        'chasssis,turret and gun need flipped

                        'hull does not so fix the previous flip
                        If fbxgrp(item).color_name.Contains("turret") Or fbxgrp(item).color_name.Contains("hull") Then
                            '
                            fbxgrp(item).vertices(v).nz = .nz
                            'fbxgrp(item).vertices(v).ny = -fbxgrp(item).vertices(v).ny
                        End If

                        fbxgrp(item).vertices(v).u = .u : fbxgrp(item).vertices(v).v = .v
                        fbxgrp(item).vertices(v).u2 = .u2 : fbxgrp(item).vertices(v).v2 = .v2
                        fbxgrp(item).vertices(v).index_1 = .index_1 : fbxgrp(item).vertices(v).index_2 = .index_2
                        fbxgrp(item).vertices(v).index_3 = .index_3 : fbxgrp(item).vertices(v).index_4 = .index_4
                        fbxgrp(item).vertices(v).weight_1 = .weight_1 : fbxgrp(item).vertices(v).weight_2 = .weight_2
                        fbxgrp(item).vertices(v).weight_3 = .weight_3 : fbxgrp(item).vertices(v).weight_4 = .weight_4
                    End With
                Next

                ' ── copy faces from DLL ───────────────────────────────────────
                Dim nativeFaces(fc - 1) As FbxFace_Native
                Dim hfPin = GCHandle.Alloc(nativeFaces, GCHandleType.Pinned)
                fbx_mesh_get_faces(hScene, i, hfPin.AddrOfPinnedObject())
                hfPin.Free()

                Dim nm = fbxgrp(item).name.ToLower

                ReDim fbxgrp(item).indices(fc - 1)

                'If nm.Contains("turret") OrElse nm.Contains("hull") Then
                '    ' exported swapped (v2,v1,v3) — swap back on import
                '    For f = 0 To fc - 1
                '        fbxgrp(item).indices(f) = New uvect3
                '        fbxgrp(item).indices(f).v1 = CInt(nativeFaces(f).v2)
                '        fbxgrp(item).indices(f).v2 = CInt(nativeFaces(f).v1)
                '        fbxgrp(item).indices(f).v3 = CInt(nativeFaces(f).v3)
                '    Next
                'Else
                '    ' chassis, tracks, gun, and all others exported straight — import straight

                'End If

                For f = 0 To fc - 1
                    fbxgrp(item).indices(f) = New uvect3
                    fbxgrp(item).indices(f).v1 = CInt(nativeFaces(f).v1)
                    fbxgrp(item).indices(f).v2 = CInt(nativeFaces(f).v2)
                    fbxgrp(item).indices(f).v3 = CInt(nativeFaces(f).v3)
                Next
                ' ── local transform matrix ────────────────────────────────────
                ' fbx_mesh_matrix returns a const double* to 16 doubles inside the DLL.
                ' Copy them immediately into fbxgrp(item).matrix (Double(15)).
                Dim pMat As IntPtr = fbx_mesh_matrix(hScene, i)
                If pMat <> IntPtr.Zero Then
                    ReDim fbxgrp(item).matrix(15)
                    Dim matBytes(16 * 8 - 1) As Byte
                    Marshal.Copy(pMat, matBytes, 0, 16 * 8)
                    For m_ = 0 To 15
                        fbxgrp(item).matrix(m_) = BitConverter.ToDouble(matBytes, m_ * 8)
                    Next
                End If


            Next

            ' DLL data is fully copied — release the scene now
            fbx_free(hScene)
            hScene = IntPtr.Zero

            ' ── load textures for OpenGL display ─────────────────────────────
            For i = 1 To fbxgrp.Length - 1
                frmMain.info_Label.Text = "loading texture set: " & i.ToString
                Application.DoEvents()
                If Not fbxgrp(i).name.Contains("~") Then
                    fbxgrp(i).is_new_model = True
                    _group(i).is_new_model = True
                Else
                    fbxgrp(i).is_new_model = False
                    _group(i).is_new_model = False
                End If
                If fbxgrp(i).color_name IsNot Nothing Then
                    fbxgrp(i).color_Id = get_fbx_texture(Path.GetDirectoryName(open_path) & "\" & fbxgrp(i).color_name.Replace("..", ""))
                Else
                    fbxgrp(i).color_Id = white_id
                End If
                If fbxgrp(i).normal_name IsNot Nothing Then
                    fbx_bumped = 1
                    fbxgrp(i).normal_Id = get_fbx_texture(Path.GetDirectoryName(open_path) & "\" & fbxgrp(i).normal_name.Replace("..", ""))
                Else
                    fbxgrp(i).normal_Id = 0
                End If

            Next

            '===================================================================
            process_fbx_data()
            '===================================================================

            For i = 1 To object_count - 1
                tank_center_X += _object(i).center_x
                tank_center_Y += _object(i).center_y
                tank_center_Z += _object(i).center_z
            Next
            tank_center_X /= object_count - 1
            tank_center_Y /= object_count - 1
            tank_center_Z /= object_count - 1
            look_point_x = tank_center_X
            look_point_y = tank_center_Y
            look_point_z = tank_center_Z

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
            If MODEL_LOADED Then frmMain.m_show_fbx.Visible = True

        Catch ex As Exception
            If hScene <> IntPtr.Zero Then fbx_free(hScene)
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "error")
            remove_loaded_fbx()
        End Try

        frmMain.info_Label.Visible = False
        view_radius = -10.0!
        look_point_y = 1.0
    End Sub
    Private Sub round_error(ByRef val As Single)
        val = Math.Round(val, 6, MidpointRounding.AwayFromZero)
    End Sub
End Module
