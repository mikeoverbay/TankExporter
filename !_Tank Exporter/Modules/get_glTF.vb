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

Module get_glTF
    Private rootpath_locked As Boolean = False
    Public GLFT_outfolder As String = Nothing
    Public GLTF_infolder As String = Nothing
    Public Function open_glTF()
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



        Dim filename As String = frmMain.OpenFileDialog1.FileName


        Try
            ' Load the GLTF model
            Dim model As ModelRoot = ModelRoot.Load(frmMain.OpenFileDialog1.FileName)

            ' Iterate through the scenes, nodes, and meshes
            For Each scene In model.LogicalScenes
                For Each node In scene.VisualChildren
                    Dim extrasObject = node.VisualRoot.Mesh.Extras?.AsObject()
                    If extrasObject IsNot Nothing AndAlso extrasObject.ContainsKey("status") Then
                        Dim status = extrasObject("status").ToString()
                        If status = "TANK" Then
                            GLFT_outfolder = extrasObject("exportfolder").ToString() + "\"
                            Exit For
                        Else
                            'grab local path
                            GLTF_infolder = extrasObject("outfolder").ToString() + "\"
                        End If
                    Else
                        'GLTF_infolder = extrasObject("exportfolder").ToString() + "\"
                    End If
                Next
            Next


            For Each scene In model.LogicalScenes

                Dim cnt As Integer = 1
                For Each node In scene.VisualChildren
                    ReDim Preserve fbxgrp(cnt)
                    fbxgrp(cnt) = New _grps
                    ReDim Preserve _group(cnt)
                    _group(cnt) = New _grps
                    ProcessNode(node, cnt)
                    fbxgrp(cnt).visible = True
                    fbxgrp(cnt).component_visible = True
                    cnt += 1
                Next
            Next
        Catch ex As Exception
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
        Return True
    End Function
    Private Sub ProcessNode(node As Node, id As Integer)

        Dim extrasJsonNode As JsonNode = node.VisualRoot.Mesh.Extras

        ' Initialize the texture paths
        Dim baseColorTexture As String = Nothing
        Dim aoTexture As String = Nothing
        Dim metalRoughTexture As String = Nothing
        Dim normalTexture As String = Nothing
        Dim status As String = Nothing
        Dim exportfolder As String = Nothing
        Dim extrasJsonObject As JsonObject = TryCast(extrasJsonNode, JsonObject)

        ' Check if the Extras JSON Node is not null
        If extrasJsonObject IsNot Nothing Then
            ' Try to get the values from the JSON object

            If extrasJsonObject.ContainsKey("base") Then
                baseColorTexture = extrasJsonObject("base").ToString()
                Debug.WriteLine(baseColorTexture + ": " + id.ToString)
            End If

            If extrasJsonObject.ContainsKey("ao") Then
                aoTexture = extrasJsonObject("ao").ToString()
            End If

            If extrasJsonObject.ContainsKey("gmm") Then
                metalRoughTexture = extrasJsonObject("gmm").ToString()
            End If

            If extrasJsonObject.ContainsKey("normal") Then
                normalTexture = extrasJsonObject("normal").ToString()
            End If
            If extrasJsonObject.ContainsKey("normal") Then
                status = extrasJsonObject("status").ToString()
            End If
            If extrasJsonObject.ContainsKey("exportfolder") Then
                exportfolder = extrasJsonObject("exportfolder").ToString()
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
            'Dim weight1 = primitive.GetVertexAccessor("COLOR_2")?.AsColorArray()

            fbxgrp(id).stride = 32
            fbxgrp(id).bumped = 1

            If vcolor IsNot Nothing Then
                fbxgrp(id).has_color = 1
                fbxgrp(id).stride = 40
            End If
            If uv1 IsNot Nothing Then
                fbxgrp(id).has_uv2 = 1
            End If
            If weight0 IsNot Nothing Then
                fbxgrp(id).stride = 40
            End If

            If norms IsNot Nothing Then
            End If
            Dim winding As Boolean = False
            If node.Mesh.Name.Contains("~") And node.Mesh.Name.ToLower.Contains("turret") Then
                winding = True
            Else
                fbxgrp(id).is_new_model = True
            End If
            Dim vertexIndex As Integer = 0 ' To keep track of global vertex index across primitives
            For i = 0 To verts.Count - 1
                fbxgrp(id).vertices(vertexIndex) = New vertice_()
                fbxgrp(id).vertices(vertexIndex).x = verts(i).X
                fbxgrp(id).vertices(vertexIndex).y = verts(i).Y
                fbxgrp(id).vertices(vertexIndex).z = verts(i).Z

                ' Assign normals if available
                If norms IsNot Nothing Then
                    fbxgrp(id).vertices(vertexIndex).nx = norms(i).X
                    fbxgrp(id).vertices(vertexIndex).ny = norms(i).Y
                    fbxgrp(id).vertices(vertexIndex).nz = norms(i).Z
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

            ' Process indices
            Dim indis = primitive.GetIndices()
            fbxgrp(id).nPrimitives_ = indis.Count / 3
            ReDim Preserve fbxgrp(id).indices(fbxgrp(id).nPrimitives_ - 1)


            Dim cnt = 0
            For i = 0 To indis.Count - 1 Step 3
                fbxgrp(id).indices(cnt).v1 = indis(i + 0)
                fbxgrp(id).indices(cnt).v2 = indis(i + 1)
                fbxgrp(id).indices(cnt).v3 = indis(i + 2)
                cnt += 1
            Next
            fix_winding_order(id)

            If primitive.Material IsNot Nothing Then
                ' Base color texture
                With primitive.Material

                    Dim basecolorchannel = .FindChannel("baseColor")
                    If basecolorchannel IsNot Nothing Then
                        Dim texture = .Channels(0) 'BaseColor
                        Dim textCoordIdx = texture.TextureCoordinate
                        Dim color = texture.Color
                        If baseColorTexture Is Nothing Then
                            If texture.Texture IsNot Nothing Then
                                Dim imageName = texture.Texture.PrimaryImage.Name
                                Dim data = texture.Texture.PrimaryImage.Content.Content.ToArray
                                Using ms As New MemoryStream(data)
                                    fbxgrp(id).color_Id = get_png_id(ms)
                                End Using
                                fbxgrp(id).color_name = imageName
                            Else
                                fbxgrp(id).color_Id = white_id
                            End If
                        Else
                            fbxgrp(id).color_name = GLFT_outfolder + baseColorTexture.Replace(".png", "") + ".png"
                            fbxgrp(id).color_Id = load_png_file(GLFT_outfolder + baseColorTexture.Replace(".png", "") + ".png")
                        End If
                    End If

                    Dim gmmchannel = .FindChannel("MetallicRoughness")
                    If gmmchannel IsNot Nothing Then
                        Dim texture = .Channels(1)
                        Dim textCoordIdx = texture.TextureCoordinate
                        'Dim Color = gmtexture.Color
                        If metalRoughTexture Is Nothing Then
                            If texture.Texture IsNot Nothing Then
                                Dim imageName = texture.Texture.PrimaryImage.Name
                                Dim data = texture.Texture.PrimaryImage.Content.Content.ToArray
                                Using ms As New MemoryStream(data)
                                    fbxgrp(id).GMM_Id = get_png_id(ms)
                                End Using
                                fbxgrp(id).GMM_name = imageName
                            End If
                        Else
                            fbxgrp(id).GMM_name = GLFT_outfolder + metalRoughTexture.Replace(".png", "") + ".png"
                            fbxgrp(id).GMM_Id = load_png_file(GLFT_outfolder + metalRoughTexture.Replace(".png", "") + ".png")
                        End If
                    End If


                    Dim normalchannel = .FindChannel("normal")
                    If normalchannel IsNot Nothing Then
                        Dim texture = .Channels(2) 'normal
                        Dim textCoordIdx = texture.TextureCoordinate
                        'Dim color = texture.Color
                        If normalTexture Is Nothing Then
                            If texture.Texture IsNot Nothing Then
                                Dim imageName = texture.Texture.PrimaryImage.Name
                                Dim data = texture.Texture.PrimaryImage.Content.Content.ToArray
                                Using ms As New MemoryStream(data)
                                    fbxgrp(id).normal_Id = get_png_id(ms)
                                End Using
                                fbxgrp(id).normal_name = imageName
                            End If
                        Else
                            fbxgrp(id).normal_name = GLFT_outfolder + normalTexture.Replace(".png", "") + ".png"
                            fbxgrp(id).normal_Id = load_png_file(GLFT_outfolder + normalTexture.Replace(".png", "") + ".png")
                        End If
                    End If

                    Dim aochannel = .FindChannel("Occlusion")
                    If aochannel IsNot Nothing Then
                        Dim texture = .Channels(3) 'ao
                        Dim textCoordIdx = texture.TextureCoordinate
                        If aoTexture Is Nothing Then
                            If texture.Texture IsNot Nothing Then
                                Dim imageName = texture.Texture.PrimaryImage.Name
                                Dim data = texture.Texture.PrimaryImage.Content.Content.ToArray
                                Using ms As New MemoryStream(data)
                                    fbxgrp(id).ao_id = get_png_id(ms)
                                End Using
                                fbxgrp(id).ao_name = imageName
                            End If
                        Else
                            fbxgrp(id).ao_name = GLFT_outfolder + aoTexture.Replace(".png", "") + ".png"
                            fbxgrp(id).ao_id = load_png_file(GLFT_outfolder + aoTexture.Replace(".png", "") + ".png")
                        End If
                    End If
                End With
            End If
        Next
        If fbxgrp(id).color_Id = 0 Then
            MsgBox("missing Base Color texture." + vbCrLf + fbxgrp(id).color_name + vbCrLf +
                                id.ToString + " Looked in folder " + GLFT_outfolder, MsgBoxStyle.Exclamation, "Missing Textures")
        End If
        If fbxgrp(id).normal_Id = 0 Then
            MsgBox("missing Normal texture." + vbCrLf + fbxgrp(id).normal_name + vbCrLf +
                                id.ToString + " Looked in folder " + GLFT_outfolder, MsgBoxStyle.Exclamation, "Missing Textures")
        End If
        If fbxgrp(id).ao_id = 0 And Not node.Mesh.Name.ToLower.Contains("chassis") Then
            MsgBox("missing AO texture." + vbCrLf + fbxgrp(id).ao_name + vbCrLf +
                               id.ToString + " Looked in folder " + GLFT_outfolder, MsgBoxStyle.Exclamation, "Missing Textures")
        End If
        If fbxgrp(id).GMM_Id = 0 Then
            MsgBox("missing GMM texture." + vbCrLf + fbxgrp(id).GMM_name + vbCrLf +
                               id.ToString + " Looked in folder " + GLFT_outfolder, MsgBoxStyle.Exclamation, "Missing Textures")
        End If



    End Sub




    Public Function ConvertMatrixToOpenGLArray(matrix As Matrix4x4) As Double()
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
