Imports System.IO
Imports System.Windows
Imports SharpGLTF.Geometry
Imports SharpGLTF.Geometry.VertexTypes
Imports SharpGLTF.Materials
'Imports SharpGLTF.Scenes
Imports SharpGLTF.Schema2
Imports System.Numerics

Module get_glTF

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
                Dim cnt As Integer = 1
                For Each node In scene.VisualChildren
                    ReDim Preserve fbxgrp(cnt)
                    fbxgrp(cnt) = New _grps
                    ReDim Preserve _group(cnt)
                    _group(cnt) = New _grps
                    ProcessNode(node, cnt)
                    fbxgrp(cnt).name = "node_" + cnt.ToString
                    fbxgrp(cnt).visible = True
                    fbxgrp(cnt).component_visible = True
                    cnt += 1
                Next
            Next
        Catch ex As Exception
            Console.WriteLine($"Error loading GLTF file: {ex.Message}")
            Return False
        End Try


        frmMain.info_Label.Text = "Creating Display Lists"
        Application.DoEvents()
        For i = 1 To fbxgrp.Length - 1
            fbxgrp(i).alphaTest = 0

            Dim id = Gl.glGenLists(1)
            Gl.glNewList(id, Gl.GL_COMPILE)
            fbxgrp(i).call_list = id
            make_fbx_display_lists(fbxgrp(i).nPrimitives_ - 1, i)
            Gl.glEndList()
        Next
        FBX_LOADED = True
        frmMain.info_Label.Visible = False
        frmMain.m_show_fbx.Checked = True
        If MODEL_LOADED Then
            frmMain.m_show_fbx.Visible = True
        End If
        LOADING_FBX = False ' so we dont read from the res_Mods folder

        Return True
    End Function
    Private Sub ProcessNode(node As Node, id As Integer)
        If node.Mesh IsNot Nothing Then
            ReDim fbxgrp(id).vertices(node.Mesh.Primitives.Count)
            For Each primitive In node.Mesh.Primitives

                fbxgrp(id).name = node.Name
                fbxgrp(id).matrix = ConvertMatrixToOpenGLArray(node.LocalMatrix)
                ' Process vertices, normals, UVs, etc.
                Dim verts = primitive.GetVertexAccessor("POSITION").AsVector3Array

                Dim norms = primitive.GetVertexAccessor("NORMAL")?.AsVector3Array
                Dim uv0 = primitive.GetVertexAccessor("TEXCOORD_0")?.AsVector2Array
                Dim uv1 = primitive.GetVertexAccessor("TEXCOORD_0")?.AsVector2Array
                Dim vcolor = primitive.GetVertexAccessor("COLOR_0")?.AsColorArray()
                Dim weight0 = primitive.GetVertexAccessor("WEIGHTS_0")?.AsVector4Array()
                Dim weight1 = primitive.GetVertexAccessor("WEIGHTS_1")?.AsVector4Array()
                ReDim Preserve fbxgrp(id).vertices(verts.Count - 1)
                If vcolor IsNot Nothing Then
                    ReDim Preserve fbxgrp(id).vertColor(verts.Count - 1)
                End If
                If weight0 IsNot Nothing Then
                    ReDim Preserve fbxgrp(id).weight0(verts.Count - 1)
                End If


                For i = 0 To verts.Count - 1


                    fbxgrp(id).vertices(i) = New vertice_
                    fbxgrp(id).vertices(i).x = verts(i).X
                    fbxgrp(id).vertices(i).y = verts(i).Y
                    fbxgrp(id).vertices(i).z = verts(i).Z

                    fbxgrp(id).vertices(i).nx = norms(i).X
                    fbxgrp(id).vertices(i).ny = norms(i).Y
                    fbxgrp(id).vertices(i).nz = norms(i).Z

                    If vcolor IsNot Nothing Then
                        fbxgrp(id).vertColor(i) = New Vector4
                        fbxgrp(id).vertColor(i).x = vcolor(i).X
                        fbxgrp(id).vertColor(i).y = vcolor(i).Y
                        fbxgrp(id).vertColor(i).z = vcolor(i).Z
                        fbxgrp(id).vertColor(i).w = vcolor(i).W
                    End If

                    If weight0 IsNot Nothing Then
                        fbxgrp(id).weight0(i) = New Vector4
                        fbxgrp(id).weight0(i).x = weight0(i).X
                        fbxgrp(id).weight0(i).y = weight0(i).Y
                        fbxgrp(id).weight0(i).z = weight0(i).Z
                        fbxgrp(id).weight0(i).w = weight0(i).W

                    End If


                    fbxgrp(id).vertices(i).u = uv0(i).X
                    fbxgrp(id).vertices(i).v = uv0(i).Y

                    If uv1 IsNot Nothing Then
                        fbxgrp(id).vertices(i).u2 = uv1(i).X
                        fbxgrp(id).vertices(i).v2 = uv1(i).Y
                    End If


                Next
                Dim indis = primitive.GetIndices
                fbxgrp(id).nPrimitives_ = indis.Count / 3
                Dim cnt = 0
                ReDim Preserve fbxgrp(id).indices((indis.Count / 3) - 1)
                For i = 0 To indis.Count - 1 Step 3
                    fbxgrp(id).indices(cnt).v1 = indis(i + 0)
                    fbxgrp(id).indices(cnt).v2 = indis(i + 1)
                    fbxgrp(id).indices(cnt).v3 = indis(i + 2)
                Next

                If primitive.Material IsNot Nothing Then

                    ' Base color texture
                    With primitive.Material

                        Dim basecolorchannel = .FindChannel("baseColor")

                        If basecolorchannel IsNot Nothing Then
                            Dim texture = .Channels(0) 'BaseColor
                            Dim textCoordIdx = texture.TextureCoordinate
                            Dim color = texture.Color
                            Dim name = texture.Texture.PrimaryImage.Name
                            Dim data = texture.Texture.PrimaryImage.Content.Content.ToArray
                            Using ms As New MemoryStream(data)
                                fbxgrp(id).color_Id = get_png_id(ms)
                                fbxgrp(id).color_name = name
                            End Using
                        End If

                        Dim gmmchannel = .FindChannel("MetallicRoughness")

                        If gmmchannel IsNot Nothing Then
                            Dim gmtexture = .Channels(1) 'matallic roughness
                            Try

                                Dim textCoordIdx = gmtexture.TextureCoordinate
                                'Dim Color = gmtexture.Color
                                Dim name = gmtexture.Texture.PrimaryImage.Name
                                Dim Data = gmtexture.Texture.PrimaryImage.Content.Content.ToArray
                                Using ms As New MemoryStream(Data)
                                    fbxgrp(id).metalGMM_Id = get_png_id(ms)
                                    fbxgrp(id).metalGMM_name = name
                                End Using
                            Catch ex As Exception

                            End Try
                        End If

                        Dim normalchannel = .FindChannel("normal")
                        If normalchannel IsNot Nothing Then
                            Dim texture = .Channels(2) 'normal
                            Dim textCoordIdx = texture.TextureCoordinate
                            'Dim color = texture.Color
                            Dim name = texture.Texture.PrimaryImage.Name
                            Dim data = texture.Texture.PrimaryImage.Content.Content.ToArray
                            Using ms As New MemoryStream(data)
                                fbxgrp(id).normal_Id = get_png_id(ms)
                                fbxgrp(id).normal_name = name
                            End Using
                        End If

                        Dim aochannel = .FindChannel("OcclusionOcclusion")
                        If aochannel IsNot Nothing Then

                            Dim texture = .Channels(3) 'ao
                            Dim textCoordIdx = texture.TextureCoordinate
                            Dim Color = texture.Color
                            Dim name = texture.Texture.PrimaryImage.Name
                            Dim Data = texture.Texture.PrimaryImage.Content.Content.ToArray
                            Using ms As New MemoryStream(Data)
                                fbxgrp(id).normal_Id = get_png_id(ms)
                                fbxgrp(id).normal_name = name
                            End Using
                        End If

                    End With

                End If
            Next
        End If



    End Sub




    Public Function ConvertMatrixToOpenGLArray(matrix As Matrix4x4) As Double()
        Dim openGLArray(15) As Double

        openGLArray(0) = matrix.M11
        openGLArray(1) = matrix.M21
        openGLArray(2) = matrix.M31
        openGLArray(3) = matrix.M41

        openGLArray(4) = matrix.M12
        openGLArray(5) = matrix.M22
        openGLArray(6) = matrix.M32
        openGLArray(7) = matrix.M42

        openGLArray(8) = matrix.M13
        openGLArray(9) = matrix.M23
        openGLArray(10) = matrix.M33
        openGLArray(11) = matrix.M43

        openGLArray(12) = matrix.M14
        openGLArray(13) = matrix.M24
        openGLArray(14) = matrix.M34
        openGLArray(15) = matrix.M44

        Return openGLArray
    End Function
End Module
