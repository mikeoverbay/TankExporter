Imports System.IO
Imports System.Windows
Imports SharpGLTF.Geometry
Imports SharpGLTF.Geometry.VertexTypes
Imports SharpGLTF.Materials
'Imports SharpGLTF.Scenes
Imports SharpGLTF.Schema2
Imports System.Numerics
Imports Tank_Exporter.modGlobals
Imports System.Text.Json
Imports System.Text.Json.Nodes
Imports Assimp

Module get_glTF
    Private rootpath_locked As Boolean = False
    Public GLFT_outfolder As String = Nothing
    Public GLTF_infolder As String = Nothing
    Public Function open_glTF()
        GLB = True
        frmMain.OpenFileDialog1.InitialDirectory = My.Settings.fbx_path
        ' Set the file dialog filter to show GLTF files
        frmMain.OpenFileDialog1.Filter = "GLTF Files (*.gltf; *.glb)|*.gltf;*.glb"

        ' Optionally set the default file extension
        frmMain.OpenFileDialog1.DefaultExt = "glb"
        If frmMain.OpenFileDialog1.FileName = "OpenFileDialog1" Then
            frmMain.OpenFileDialog1.FileName = ""
        End If
        If Not frmMain.OpenFileDialog1.ShowDialog = Forms.DialogResult.OK Then
            Return False
        End If
        frmMain.clean_house()
        remove_loaded_fbx()
        rootpath_locked = False
        GLTF_infolder = Nothing
        frmComponentView.clear_fbx_list()
        frmReverseVertexWinding.clear_group_list()

        My.Settings.GLTF_path = Path.GetDirectoryName(frmMain.OpenFileDialog1.FileName)
        My.Settings.Save()

        frmMain.info_Label.Text = ""
        frmMain.info_Label.Visible = True
        Dim filename As String = frmMain.OpenFileDialog1.FileName
        Application.DoEvents()
        Try
            ' Load the GLTF model
            Dim model As ModelRoot = ModelRoot.Load(frmMain.OpenFileDialog1.FileName)

            ' get our file paths.
            Dim scene = model.LogicalScenes.First
            Dim node = scene.VisualChildren.First
            Dim extrasObject = node.VisualRoot.Mesh.Extras?.AsObject()

            'get paths of game data folder and gltf out folder of this model.

            If extrasObject IsNot Nothing Then
                If extrasObject.ContainsKey("exportfolder") Then
                    GLFT_outfolder = extrasObject("exportfolder").ToString() + "\"
                End If
                If extrasObject.ContainsKey("outfolder") Then
                    GLTF_infolder = extrasObject("outfolder").ToString()
                End If
            Else

                MsgBox("You forgot to export properties.")
                Return False
            End If

            For Each scene In model.LogicalScenes

                Dim cnt As Integer = 1
                ReDim Preserve _group(scene.VisualChildren.Count)

                For Each node In scene.VisualChildren
                    frmMain.info_Label.Text = "Loading Model: " + cnt.ToString
                    Application.DoEvents()
                    ReDim Preserve fbxgrp(cnt)
                    fbxgrp(cnt) = New _grps
                    _group(cnt) = New _grps
                    ProcessNode(node, cnt)
                    fbxgrp(cnt).visible = True
                    fbxgrp(cnt).component_visible = True
                    cnt += 1
                Next

            Next
        Catch ex As Exception
            frmMain.info_Label.Visible = False
            Console.WriteLine($"Error loading GLTF file: {ex.Message}")
            Return False
        End Try
        '===================================================================
        process_fbx_data()
        '===================================================================


        frmMain.info_Label.Text = "Creating Display Lists"
        Application.DoEvents()
        For i = 1 To fbxgrp.Length - 1
            fbxgrp(i).alphaTest = 1
            fbxgrp(i).visible = True

            Dim id = Gl.glGenLists(1)
            Gl.glNewList(id, Gl.GL_COMPILE)
            fbxgrp(i).call_list = id
            make_fbx_display_lists(fbxgrp(i).nPrimitives_, i)
            Gl.glEndList()
        Next
        FBX_LOADED = True
        frmMain.info_Label.Visible = False
        frmMain.m_show_fbx.Checked = True
        frmMain.m_show_fbx.Visible = True
        frmMain.m_write_primitive.Enabled = True
        If MODEL_LOADED Then
            frmMain.m_show_fbx.Visible = True
        End If
        LOADING_FBX = False ' so we dont read from the res_Mods folder
        view_radius = -10.0!
        look_point_y = 1.0
        frmMain.info_Label.Visible = False
        Return True
    End Function
    Private Sub ProcessNode(node As SharpGLTF.Schema2.Node, id As Integer)

        Dim extrasJsonNode As JsonNode = node.VisualRoot.Mesh.Extras
        Dim extrasJsonObject As JsonObject = TryCast(extrasJsonNode, JsonObject)

        ' Initialize the texture paths
        Dim baseColorTexture As String = Nothing
        Dim aoTexture As String = Nothing
        Dim metalRoughTexture As String = Nothing
        Dim normalTexture As String = Nothing
        Dim exportfolder As String = Nothing

        ' Check if the Extras JSON Node is not null
        If extrasJsonObject IsNot Nothing Then
            ' Try to get the values from the JSON object

            'get our textures 

            If extrasJsonObject.ContainsKey("base") Then
                baseColorTexture = extrasJsonObject("base").ToString()
                Debug.WriteLine(baseColorTexture + ": " + id.ToString)
                fbxgrp(id).color_name = GLFT_outfolder + baseColorTexture.Replace(".png", "") + ".png"
                fbxgrp(id).color_Id = load_png_file(fbxgrp(id).color_name)
            End If

            If extrasJsonObject.ContainsKey("ao") Then
                aoTexture = extrasJsonObject("ao").ToString()
                fbxgrp(id).ao_name = GLFT_outfolder + aoTexture.Replace(".png", "") + ".png"
                fbxgrp(id).ao_id = load_png_file(fbxgrp(id).ao_name)

            End If

            If extrasJsonObject.ContainsKey("gmm") Then
                metalRoughTexture = extrasJsonObject("gmm").ToString()
                fbxgrp(id).GMM_name = GLFT_outfolder + metalRoughTexture.Replace(".png", "") + ".png"
                fbxgrp(id).GMM_Id = load_png_file(fbxgrp(id).GMM_name)
            End If

            If extrasJsonObject.ContainsKey("normal") Then
                normalTexture = extrasJsonObject("normal").ToString()
                fbxgrp(id).normal_name = GLFT_outfolder + normalTexture.Replace(".png", "") + ".png"
                fbxgrp(id).normal_Id = load_png_file(fbxgrp(id).normal_name)
                fbxgrp(id).bumped = 1
            End If

            'is there a new model?
            If extrasJsonObject.ContainsKey("status") Then
                Dim status = extrasJsonObject("status").ToString()
                If Not status = "TANK" Then
                    fbxgrp(id).is_new_model = True
                    _group(id).is_new_model = True
                Else
                    fbxgrp(id).is_new_model = False
                    _group(id).is_new_model = False
                End If
            Else
                fbxgrp(id).is_new_model = True
                _group(id).is_new_model = True
            End If

        End If

        ' Initialize vertices array based on total vertices count, not primitive count
        ReDim fbxgrp(id).vertices(node.Mesh.Primitives.Sum(Function(p) _
                     p.GetVertexAccessor("POSITION").AsVector3Array.Count) - 1)
        fbxgrp(id).nVertices_ = fbxgrp(id).vertices.Length
        ' Initialize other arrays
        If node.Mesh.Primitives.Any(Function(p) p.GetVertexAccessor("COLOR_0") IsNot Nothing) Then
            ReDim fbxgrp(id).vertColor(fbxgrp(id).vertices.Length - 1)
        End If
        If node.Mesh.Primitives.Any(Function(p) p.GetVertexAccessor("COLOR_1") IsNot Nothing) Then
            ReDim fbxgrp(id).weight0(fbxgrp(id).vertices.Length - 1)
        End If

        ' Loop through primitives in the mesh
        For Each primitive In node.Mesh.Primitives
            fbxgrp(id).name = node.Mesh.Name
            fbxgrp(id).matrix = ConvertMatrixToOpenGLArray(node.LocalMatrix)

            ' Extract vertex data
            Dim verts = primitive.GetVertexAccessor("POSITION").AsVector3Array
            Dim norms = primitive.GetVertexAccessor("NORMAL")?.AsVector3Array
            Dim uv0 = primitive.GetVertexAccessor("TEXCOORD_0")?.AsVector2Array
            Dim uv1 = primitive.GetVertexAccessor("TEXCOORD_1")?.AsVector2Array ' Corrected to TEXCOORD_1
            Dim vcolor = primitive.GetVertexAccessor("COLOR_0")?.AsColorArray()
            Dim weight0 = primitive.GetVertexAccessor("COLOR_1")?.AsColorArray()

            fbxgrp(id).stride = 32
            fbxgrp(id).bumped = 0
            fbxgrp(id).has_color = 0

            If vcolor IsNot Nothing Then
                fbxgrp(id).has_color = 1
                fbxgrp(id).stride = 37
            End If
            If uv1 IsNot Nothing Then
                fbxgrp(id).has_uv2 = 1
            End If
            If weight0 IsNot Nothing Then
                fbxgrp(id).stride = 40
            End If

            Dim winding As Boolean = False
            Dim vertexIndex As Integer = 0 ' To keep track of global vertex index across primitives
            For i = 0 To verts.Count - 1
                fbxgrp(id).vertices(vertexIndex) = New vertice_()
                fbxgrp(id).vertices(vertexIndex).x = verts(i).X
                fbxgrp(id).vertices(vertexIndex).y = verts(i).Y
                fbxgrp(id).vertices(vertexIndex).z = verts(i).Z

                ' Assign normals if available
                If norms IsNot Nothing Then
                    Dim vn = New vect3
                    vn.x = norms(i).X
                    vn.y = norms(i).Y
                    vn.z = norms(i).Z
                    vn = normalize(vn)

                    If fbxgrp(id).name.ToLower.Contains("turret") Or
                       fbxgrp(id).name.ToLower.Contains("hull") Or
                       fbxgrp(id).name.ToLower.Contains("gun") Then

                        If Not fbxgrp(id).name.ToLower.Contains("~") Then
                            'new
                            vn.x *= -1
                            vn.y *= -1
                            vn.z *= -1
                        Else
                            'old
                            vn.z *= -1
                            'vn.z *= -1
                        End If
                    Else
                        'chassis
                        vn.z *= -1
                        'vn.z *= -1
                    End If
                    fbxgrp(id).vertices(vertexIndex).nx = vn.x
                    fbxgrp(id).vertices(vertexIndex).ny = vn.y
                    fbxgrp(id).vertices(vertexIndex).nz = vn.z
                End If

                ' Assign vertex colors if available
                If vcolor IsNot Nothing Then
                    fbxgrp(id).vertColor(vertexIndex) = New Vector4()
                    fbxgrp(id).vertColor(vertexIndex).X = vcolor(i).X
                    fbxgrp(id).vertColor(vertexIndex).Y = vcolor(i).Y
                    fbxgrp(id).vertColor(vertexIndex).Z = vcolor(i).Z
                    fbxgrp(id).vertColor(vertexIndex).W = vcolor(i).W
                End If

                ' Assign weights if available
                If weight0 IsNot Nothing Then
                    fbxgrp(id).weight0(vertexIndex) = New Vector4()
                    fbxgrp(id).weight0(vertexIndex).X = weight0(i).X
                    fbxgrp(id).weight0(vertexIndex).Y = weight0(i).Y
                    fbxgrp(id).weight0(vertexIndex).Z = weight0(i).Z
                    fbxgrp(id).weight0(vertexIndex).W = weight0(i).W
                End If

                ' Assign texture coordinates if available
                If uv0 IsNot Nothing Then
                    fbxgrp(id).vertices(vertexIndex).u = uv0(i).X
                    fbxgrp(id).vertices(vertexIndex).v = uv0(i).Y
                End If
                If uv1 IsNot Nothing Then
                    fbxgrp(id).vertices(vertexIndex).u2 = uv1(i).X
                    fbxgrp(id).vertices(vertexIndex).v2 = uv1(i).Y
                End If

                vertexIndex += 1
            Next
            check_normal(id)

            ' Process indices
            Dim indis = primitive.GetIndices()
            fbxgrp(id).nPrimitives_ = indis.Count / 3
            ReDim Preserve fbxgrp(id).indices(fbxgrp(id).nPrimitives_ - 1)


            Dim cnt = 0

            For i = 0 To indis.Count - 1 Step 3
                If fbxgrp(id).name.ToLower.Contains("turret") Or
                              fbxgrp(id).name.ToLower.Contains("hull") Or
                              fbxgrp(id).name.ToLower.Contains("gun") Then

                    If Not fbxgrp(id).name.ToLower.Contains("~") Then
                        fbxgrp(id).indices(cnt).v1 = indis(i + 0)
                        fbxgrp(id).indices(cnt).v2 = indis(i + 1)
                        fbxgrp(id).indices(cnt).v3 = indis(i + 2)
                    Else
                        fbxgrp(id).indices(cnt).v2 = indis(i + 0)
                        fbxgrp(id).indices(cnt).v1 = indis(i + 1)
                        fbxgrp(id).indices(cnt).v3 = indis(i + 2)
                    End If
                Else
                    fbxgrp(id).indices(cnt).v1 = indis(i + 0)
                    fbxgrp(id).indices(cnt).v2 = indis(i + 1)
                    fbxgrp(id).indices(cnt).v3 = indis(i + 2)
                End If
                cnt += 1
            Next
            'fix_winding_order(id)

            ' Base color texture
        Next
        If fbxgrp(id).color_Id = 0 Then
            MsgBox("missing Base Color texture." + vbCrLf + fbxgrp(id).color_name + vbCrLf +
                                "object:" + id.ToString + " Looked in folder " + GLFT_outfolder, MsgBoxStyle.Exclamation, "Missing Textures")
        End If
        If fbxgrp(id).normal_Id = 0 Then
            MsgBox("missing Normal texture." + vbCrLf + fbxgrp(id).normal_name + vbCrLf +
                                "object:" + id.ToString + " Looked in folder " + GLFT_outfolder, MsgBoxStyle.Exclamation, "Missing Textures")
        End If
        If fbxgrp(id).ao_id = 0 And Not node.Mesh.Name.ToLower.Contains("chassis") Then
            MsgBox("missing AO texture." + vbCrLf + fbxgrp(id).ao_name + vbCrLf +
                               "object:" + id.ToString + " Looked in folder " + GLFT_outfolder, MsgBoxStyle.Exclamation, "Missing Textures")
        End If
        If fbxgrp(id).GMM_Id = 0 Then
            MsgBox("missing GMM texture." + vbCrLf + fbxgrp(id).GMM_name + vbCrLf +
                               "object:" + id.ToString + " Looked in folder " + GLFT_outfolder, MsgBoxStyle.Exclamation, "Missing Textures")
        End If



    End Sub




    Public Function ConvertMatrixToOpenGLArray(matrix As System.Numerics.Matrix4x4) As Double()
        Dim openGLArray(15) As Double

        openGLArray(0) = matrix.M11
        openGLArray(1) = matrix.M12
        openGLArray(2) = matrix.M13
        openGLArray(3) = matrix.M14

        openGLArray(4) = matrix.M21
        openGLArray(5) = matrix.M22
        openGLArray(6) = matrix.M23
        openGLArray(7) = matrix.M24

        openGLArray(8) = matrix.M31
        openGLArray(9) = matrix.M32
        openGLArray(10) = matrix.M33
        openGLArray(11) = matrix.M34

        openGLArray(12) = matrix.M41
        openGLArray(13) = matrix.M42
        openGLArray(14) = matrix.M43
        openGLArray(15) = matrix.M44

        Return openGLArray

    End Function
End Module
