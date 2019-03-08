Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Web
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports Tao.FreeGlut
Imports Tao.FreeGlut.Glut
Imports Tao.DevIl
Imports Microsoft.VisualBasic.Strings
Imports Ionic.Zip

Module modTextures
    Public textures() As textures_
    Public Structure textures_
        Public c_name As String
        Public c_id As Integer
        Public n_name As String
        Public n_id As Integer
        Public colorIdMap As String
        Public gmm_name As String
        Public gmm_id As Integer
        Public ao_name As String
        Public ao_id As Integer
        Public detail_id As Integer
        Public detail_name As String
        Public doubleSided As Boolean
        Public alphaRef As Integer
    End Structure

    Dim mStream As MemoryStream
    Public Function get_fbx_texture(ByVal name As String)
        'frmMain.gl_stop = True

        Dim ext = Path.GetExtension(name)
        Dim username = Environment.UserName
        If name.Contains("Users") Then
            Dim a = name.Split("\")
            Dim os As String = ""
            For i = 0 To a.Length - 1
                If a(i).Contains("Users") Then
                    a(i + 1) = username
                    Exit For
                End If
            Next
            For i = 0 To a.Length - 2
                os += a(i) + "\"
            Next
            os += a(a.Length - 1)
            name = os
        End If

        Dim id As Integer = -1
        If ext.ToLower.Contains(".png") Then
            id = load_png_file(name)
        End If
        If ext.ToLower.Contains(".dds") Then
            id = load_dds_file(name)
        End If
        If ext.ToLower.Contains(".jpg") Then
            id = load_jpg_file(name)
        End If
        'frmMain.gl_stop = False
        Return id
    End Function
    Public Sub export_fbx_textures(ByVal AC As Boolean)
        Dim ar = TANK_NAME.Split(":")
        Dim name As String = Path.GetFileName(ar(0))
        FBX_Texture_path = Path.GetDirectoryName(My.Settings.fbx_path) + "\" + name
        If Not IO.Directory.Exists(FBX_Texture_path) Then
            System.IO.Directory.CreateDirectory(FBX_Texture_path)
        End If
        Dim abs_name As String = ""
        frmMain.info_Label.Visible = True

        frmMain.update_thread.Suspend()
        Threading.Thread.Sleep(100)
        For i = 0 To textures.Length - 2
            With textures(i)
                'color
                abs_name = FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(.c_name)
                If AC And Not .c_name.Contains("chassis") And Not .c_name.Contains("track") Then
                    Dim part As Integer = 0
                    For k = 1 To object_count
                        If Path.GetFileNameWithoutExtension(_group(k).color_name) = Path.GetFileNameWithoutExtension(.c_name) Then
                            part = k
                            Exit For
                        End If
                    Next
                    save_fbx_textureCamouflaged(.c_id, .ao_id, SELECTED_CAMO_BUTTON, abs_name, part)
                Else
                    save_fbx_texture(.c_id, abs_name)
                End If
                'normal
                abs_name = FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(.n_name)
                save_fbx_texture(.n_id, abs_name)
                'ao
                abs_name = FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(.ao_name)
                save_fbx_texture(.ao_id, abs_name)
                'gmm
                abs_name = FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(.gmm_name)
                save_fbx_texture(.gmm_id, abs_name)
                'detail
                abs_name = FBX_Texture_path + "\" + Path.GetFileNameWithoutExtension(.detail_name)
                save_fbx_texture(.detail_id, abs_name)

            End With

        Next
        frmMain.pb2.Visible = False

        frmMain.info_Label.Visible = False
        frmMain.update_thread.Resume()

    End Sub
    Public Sub save_fbx_textureCamouflaged(ByVal color_id As Integer, ByVal ao_id As Integer, ByVal camo_id As Integer, ByVal save_path As String, ByVal part As Integer)
        If color_id = -1 Then Return
        frmMain.info_Label.Text = "Exporting : " + save_path + ".png"
        If File.Exists(save_path + ".png") Then ' stop saving exiting FBX textures.. It crashes 3DS Max
            Return
        End If
        frmMain.pb2.Visible = False
        frmMain.pb2.Location = New Point(0, 0)
        frmMain.pb2.BringToFront()
        Application.DoEvents()
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glUseProgram(0)
        'frmMain.gl_stop = True
        'While gl_busy
        'End While
        Dim w, h As Integer
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, color_id)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, w)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, h)
        Dim p As New Point(0.0!, 0.0!)
        frmMain.pb2.Width = w
        frmMain.pb2.Height = h
        Gl.glViewport(0, 0, w, h)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, w, -h, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        Dim e = Gl.glGetError

        Gl.glUseProgram(shader_list.camoExporter_shader)
        Gl.glUniform1i(CE_camo_Map, 0)
        Gl.glUniform1i(CE_AO_Map, 1)
        Gl.glUniform1i(CE_AM_Map, 2)
        Gl.glUniform4f(CE_tile, _object(part).camo_tiling.x, _object(part).camo_tiling.y, _object(part).camo_tiling.z, _object(part).camo_tiling.w)
        Gl.glUniform4f(CE_camo_tile, bb_tank_tiling(SELECTED_CAMO_BUTTON).x, bb_tank_tiling(SELECTED_CAMO_BUTTON).y, bb_tank_tiling(SELECTED_CAMO_BUTTON).z, bb_tank_tiling(SELECTED_CAMO_BUTTON).w)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 2)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, color_id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, ao_id)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, bb_camo_texture_ids(camo_id))

        Gl.glBegin(Gl.GL_QUADS)

        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()
        Gl.glUseProgram(0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 1)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
        Gl.glActiveTexture(Gl.GL_TEXTURE0 + 0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Gl.glFinish()
        Dim tId As Integer = Il.ilGenImage
        Il.ilBindImage(tId)
        Il.ilTexImage(w, h, 0, 4, Il.IL_RGBA, Il.IL_UNSIGNED_BYTE, Nothing)

        Gl.glReadPixels(0, 0, w, h, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
        Gl.glFinish()

        Gl.glFinish()
        Il.ilSave(Il.IL_PNG, save_path + ".png") ' save to temp
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gdi.SwapBuffers(pb2_hDC)
        Il.ilBindImage(0)
        Il.ilDeleteImage(tId)
        Application.DoEvents()

    End Sub

    Public Sub save_fbx_texture(ByVal id As Integer, ByVal save_path As String)
        If id = -1 Then Return
        frmMain.info_Label.Text = "Exporting : " + save_path + ".png"
        If File.Exists(save_path + ".png") Then ' stop saving exiting FBX textures.. It crashes 3DS Max
            Return
        End If
        frmMain.pb2.Visible = False
        frmMain.pb2.Location = New Point(0, 0)
        frmMain.pb2.BringToFront()
        Application.DoEvents()
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current")
            End
        End If
        Gl.glUseProgram(0)
        'frmMain.gl_stop = True
        'While gl_busy
        'End While
        Dim w, h As Integer
        Gl.glEnable(Gl.GL_TEXTURE_2D)
        Gl.glActiveTexture(Gl.GL_TEXTURE0)
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, id)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, w)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, h)
        Dim p As New Point(0.0!, 0.0!)
        frmMain.pb2.Width = w
        frmMain.pb2.Height = h
        Gl.glViewport(0, 0, w, h)
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, w, -h, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glReadBuffer(Gl.GL_BACK)
        Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
        Gl.glDisable(Gl.GL_CULL_FACE)
        Gl.glDisable(Gl.GL_DEPTH_TEST)
        Gl.glTexEnvi(Gl.GL_TEXTURE_ENV, Gl.GL_TEXTURE_ENV_MODE, Gl.GL_REPLACE)
        Dim e = Gl.glGetError
        Gl.glBegin(Gl.GL_QUADS)

        '  CW...
        '  1 ------ 2
        '  |        |
        '  |        |
        '  4 ------ 3
        '
        Gl.glTexCoord2f(0.0!, 0.0!)
        Gl.glVertex2f(p.X, p.Y)

        Gl.glTexCoord2f(1.0!, 0.0!)
        Gl.glVertex2f(p.X + w, p.Y)

        Gl.glTexCoord2f(1.0!, 1.0!)
        Gl.glVertex2f(p.X + w, p.Y - h)

        Gl.glTexCoord2f(0.0!, 1.0!)
        Gl.glVertex2f(p.X, p.Y - h)
        Gl.glEnd()
        Gl.glUseProgram(0)

        Gl.glFinish()
        Dim tId As Integer = Il.ilGenImage
        Il.ilBindImage(tId)
        Il.ilTexImage(w, h, 0, 4, Il.IL_RGBA, Il.IL_UNSIGNED_BYTE, Nothing)

        Gl.glReadPixels(0, 0, w, h, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, Il.ilGetData())
        Gl.glFinish()

        Gl.glFinish()
        Il.ilSave(Il.IL_PNG, save_path + ".png") ' save to temp
        Gl.glDisable(Gl.GL_TEXTURE_2D)
        Gdi.SwapBuffers(pb2_hDC)
        Il.ilBindImage(0)
        Il.ilDeleteImage(tId)
        Application.DoEvents()

    End Sub

    Public Sub build_textures(ByVal id As Integer)

        Dim diffuse As String = _group(id).color_name
        Dim normal As String = _group(id).normal_name
        Dim metal As String = _group(id).metalGMM_name
        Dim ao_name As String = _group(id).ao_name
        Dim colorIdMap As String = _group(id).colorIDmap
        Dim detail_name As String = _group(id).detail_name
        Dim i As Integer = 0
        For i = 0 To textures.Length - 1
            If textures(i).c_name = diffuse Then
                _group(id).color_name = textures(i).c_name
                _group(id).color_Id = textures(i).c_id
                _group(id).normal_name = textures(i).n_name
                _group(id).normal_Id = textures(i).n_id
                _group(id).metalGMM_name = textures(i).gmm_name
                _group(id).metalGMM_Id = textures(i).gmm_id
                _group(id).ao_name = textures(i).ao_name
                _group(id).ao_id = textures(i).ao_id
                _group(id).colorIDmap = textures(i).colorIdMap
                _group(id).detail_Id = textures(i).detail_id
                _group(id).alphaRef = textures(i).alphaRef
                _group(id).doubleSided = textures(i).doubleSided

                _group(id).texture_id = i
                Return
            End If
        Next

        Dim n_id, c_id, m_id, ao_id, detail_id As Integer

        c_id = get_texture_id(diffuse)
        n_id = get_texture_id(normal)
        m_id = get_texture_id(metal)
        ao_id = get_texture_id(ao_name)
        detail_id = get_texture_id(detail_name)


        i = textures.Length - 1
        ReDim Preserve textures(i + 1)
        textures(i) = New textures_
        textures(i).c_name = diffuse
        textures(i).c_id = c_id

        textures(i).n_name = normal
        textures(i).n_id = n_id

        textures(i).gmm_name = metal
        textures(i).gmm_id = m_id

        textures(i).ao_name = ao_name
        textures(i).ao_id = ao_id

        textures(i).colorIdMap = colorIdMap
        textures(i).detail_name = detail_name
        textures(i).detail_id = detail_id

        textures(i).doubleSided = _group(id).doubleSided
        textures(i).alphaRef = _group(id).alphaRef

        _group(id).texture_id = i

        _group(id).color_Id = c_id
        _group(id).normal_Id = n_id
        _group(id).metalGMM_Id = m_id
        _group(id).ao_id = ao_id
        _group(id).detail_Id = detail_id

    End Sub

    Public Function get_texture_id(name As String) As Integer
        Dim id As Integer
        If name Is Nothing Then name = ""
        Dim ent As Ionic.Zip.ZipEntry = Nothing
        If name = "" Then Return -1
        If My.Settings.res_mods_path.Contains("res_mods") Then
            Dim r_path = My.Settings.res_mods_path + "\" + name.Replace(".dds", "_hd.dds")
            Dim r_pathSD = My.Settings.res_mods_path + "\" + name
            If name.Contains("res_mods") Then
                r_path = name
            End If
            If File.Exists(r_path) Then
                log_text.AppendLine("loaded HD res_mods : " + Path.GetFileName(name))
                Dim raw = File.ReadAllBytes(r_path)
                mStream = New MemoryStream(raw)
                id = get_texture(mStream, name)
                Return id
            End If
            If File.Exists(r_pathSD) Then
                log_text.AppendLine("loaded SD res_mods : " + Path.GetFileName(name))
                Dim raw = File.ReadAllBytes(r_pathSD)
                mStream = New MemoryStream(raw)
                id = get_texture(mStream, name)
                Return id
            End If
        End If
        'No texture found in res_mods so..... try PKG files
        Try 'look in HD packages
            ent = frmMain.packages_HD(current_tank_package)(name.Replace(".dds", "_hd.dds")) ' look in tank package
        Catch ex As Exception
        End Try
        If ent Is Nothing Then
            Try 'look in HD packages
                ent = frmMain.packages_HD_2(current_tank_package)(name.Replace(".dds", "_hd.dds")) ' look in tank package
            Catch ex As Exception
            End Try
        End If
        If ent Is Nothing Then ' look in shared content
            ent = frmMain.packages(11)(name.Replace(".dds", "_hd.dds")) ' look in tank package
        End If
        If ent IsNot Nothing Then
            'it was found as HD abouve
            log_text.AppendLine("loaded HD from PKG : " + Path.GetFileName(name))
            mStream = New MemoryStream
            ent.Extract(mStream)
            id = get_texture(mStream, name) ' get hd texture ID
            Return id
        Else
            'look in current pkg for SD texture
            ent = frmMain.packages(current_tank_package)(name) ' look in tank package
            If ent Is Nothing Then 'if not found in current pkg than look in shared
                If frmMain.packages_2(current_tank_package) IsNot Nothing Then
                    ent = frmMain.packages_2(current_tank_package)(name) ' look in 2nd tank package
                End If
            End If
            If ent Is Nothing Then 'if not found in current pkg than look in shared
                If frmMain.packages(11) IsNot Nothing Then
                    ent = frmMain.packages(11)(name) ' look in 2nd tank package
                End If

            End If

            If ent IsNot Nothing Then ' found SD above
                log_text.AppendLine("loaded SD from PKG : " + Path.GetFileName(name))
                mStream = New MemoryStream
                ent.Extract(mStream)
                id = get_texture(mStream, name) ' get SD texture ID
                Return id
            Else
                'never found the texture period! Log it!
                log_text.AppendLine("Cant find:" + name)
            End If
        End If
        Return 0
    End Function

    Public Function get_texture(ByRef ms As MemoryStream, file_path As String) As Integer
        frmMain.gl_stop = True
        Dim texID As UInt32
        Dim image_id As Integer
        ms.Position = 0
        Dim textIn(ms.Length) As Byte
        ms.Read(textIn, 0, ms.Length)
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError

        Il.ilLoadL(Il.IL_DDS, textIn, textIn.Length)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            Il.ilEnable(Il.IL_FILE_OVERWRITE)
            'Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

            success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            Gl.glGenTextures(1, image_id)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            If largestAnsio > 0 Then
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
            End If
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            Gl.glFinish()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

            If frmFBX.Visible Then
                'setup texture path
                Dim ar = TANK_NAME.Split(":")
                Dim name As String = Path.GetFileName(ar(0))
                FBX_Texture_path = Path.GetDirectoryName(My.Settings.fbx_path) + "\" + name
                If Not File.Exists(FBX_Texture_path) Then

                    'create directory for the textures
                    If Not IO.Directory.Exists(FBX_Texture_path) Then
                        System.IO.Directory.CreateDirectory(FBX_Texture_path)
                    End If
                    Dim abs_name = Path.GetFileNameWithoutExtension(file_path)
                    'we have to save to a temp file.. from devil.. open and lock a new file with the correct name...
                    'save the data from the temp file in to it and finally close the new file....
                    'ALL BECAUSE 3DS MAX Crashes if its using one of PNGs... SUCH BS! Its the file change monitoring.
                    'It tries to load the image as soon as we change it and that makes it corupt!
                    Dim save_path As String = Path.GetDirectoryName(My.Settings.fbx_path)
                    Il.ilSave(Il.IL_PNG, FBX_Texture_path + "\" + abs_name + ".png" + "_temp") ' save to temp
                    Dim ta = File.ReadAllBytes(FBX_Texture_path + "\" + abs_name + ".png" + "_temp") 'read temp to an arry
                    Dim hnd = File.Open(FBX_Texture_path + "\" + abs_name + ".png", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None) ' open and lock a new file
                    hnd.Write(ta, 0, ta.Length) ' save the temp array
                    hnd.Close() ' close the file
                    If File.Exists(FBX_Texture_path + "\" + abs_name + ".png" + "_temp") Then ' delete the temp file
                        File.Delete(FBX_Texture_path + "\" + abs_name + ".png" + "_temp")
                    End If
                End If
            End If

        Else
            MsgBox("Out of memory error at :get texture:", MsgBoxStyle.Critical, "Well Shit!")
            End
        End If
        Il.ilBindImage(0)
        Ilu.iluDeleteImage(texID)
        frmMain.gl_stop = False
        Return image_id
        'ms.Close()
        'ms.Dispose()
        'GC.Collect()
    End Function

    Public Function get_texture_and_bitmap(ByRef ms As MemoryStream, file_path As String, ByRef bmp As Bitmap) As Integer

        Dim texID As UInt32
        Dim image_id As Integer
        ms.Position = 0
        Dim textIn(ms.Length) As Byte
        ms.Read(textIn, 0, ms.Length)
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError

        Il.ilLoadL(Il.IL_DDS, textIn, textIn.Length)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            Il.ilEnable(Il.IL_FILE_OVERWRITE)
            'Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)
            'success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            Gl.glGenTextures(1, image_id)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            If largestAnsio > 0 Then
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
            End If
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            Gl.glFinish()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

            Dim rect As Rectangle = New Rectangle(0, 0, width, height)

            ' Create the bitmap.
            bmp = New System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb)

            Dim bitmapData As BitmapData = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb)

            success = Il.ilConvertImage(Il.IL_BMP, Il.IL_UNSIGNED_BYTE)

            Il.ilCopyPixels(0, 0, 0, width, height, 1, Il.IL_BGR, Il.IL_UNSIGNED_BYTE, bitmapData.Scan0)
            bmp.UnlockBits(bitmapData)


        Else
            MsgBox("Out of memory error at :get texture:", MsgBoxStyle.Critical, "Well Shit!")
            End
        End If
        Il.ilBindImage(0)
        Ilu.iluDeleteImage(texID)
        Return image_id
        'ms.Close()
        'ms.Dispose()
        'GC.Collect()
    End Function

    Public Function get_png(ByVal ms As MemoryStream) As Bitmap
        'Dim s As String = ""
        's = Gl.glGetError
        Dim image_id As Integer = -1
        'Dim app_local As String = Application.StartupPath.ToString

        Dim texID As UInt32
        Dim textIn(ms.Length) As Byte
        ms.Position = 0
        ms.Read(textIn, 0, ms.Length)
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoadL(Il.IL_PNG, textIn, textIn.Length)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            'Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

            ' Create the bitmap.
            Dim Bitmapi = New System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb)
            Dim rect As Rectangle = New Rectangle(0, 0, width, height)

            ' Store the DevIL image data into the bitmap.
            Dim bitmapData As BitmapData = Bitmapi.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

            Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE)
            Il.ilCopyPixels(0, 0, 0, width, height, 1, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE, bitmapData.Scan0)
            Bitmapi.UnlockBits(bitmapData)

            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            'If make_id Then

            '    Gl.glGenTextures(1, image_id)
            '    Gl.glEnable(Gl.GL_TEXTURE_2D)
            '    Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_LINEAR)
            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            '    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            '    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            '                    Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            '                    Il.ilGetData()) '  Texture specification 
            '    Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            '    Il.ilBindImage(0)
            '    'ilu.iludeleteimage(texID)
            '    ReDim Preserve map_texture_ids(index + 1)
            '    map_texture_ids(index) = image_id
            'End If

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            'GC.Collect()
            Return Bitmapi
        Else
            MsgBox("can't find thumb image of tank", MsgBoxStyle.Critical, "oops")
        End If
        Return Nothing
    End Function

    Public Function get_png_id(ByVal ms As MemoryStream) As Integer
        'Dim s As String = ""
        's = Gl.glGetError
        Dim image_id As Integer = -1
        'Dim app_local As String = Application.StartupPath.ToString

        Dim texID As UInt32
        Dim textIn(ms.Length) As Byte
        ms.Position = 0
        ms.Read(textIn, 0, ms.Length)
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoadL(Il.IL_PNG, textIn, textIn.Length)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            Dim e = Gl.glGetError
            'Ilu.iluFlipImage()
            'Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)


            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            'If make_id Then

            Gl.glGenTextures(1, image_id)
            e = Gl.glGetError
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)
            e = Gl.glGetError

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
                            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
                            Il.ilGetData()) '  Texture specification 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            'ilu.iludeleteimage(texID)
            e = Gl.glGetError

            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            'GC.Collect()
            Return image_id
        Else
            MsgBox("can't load MS PNG", MsgBoxStyle.Critical, "oops")
        End If
        Return Nothing
    End Function

    Public Function load_png_file(ByVal fs As String) As Integer
        Dim image_id As Integer = -1

        Dim texID As UInt32
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoad(Il.IL_PNG, fs)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)


            Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE)

            success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            Gl.glGenTextures(1, image_id)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            If largestAnsio > 0 Then
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
            End If
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            Return image_id
        Else
            log_text.AppendLine("Png did not load:" + fs)
        End If
        Return Nothing
    End Function
    Public Function load_dds_file(ByVal fs As String) As Integer
        Dim image_id As Integer = -1

        Dim texID As UInt32
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoad(Il.IL_DDS, fs)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            ' Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)
            'Dim dds_format = Il.ilGetInteger(Il.IL_DXTC_DATA_FORMAT)
            'Debug.WriteLine(dds_format.ToString)

            'Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE)

            'success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE)

            Gl.glGenTextures(1, image_id)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            If largestAnsio > 0 Then
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
            End If
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA8, width, height, 0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 


            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            Gl.glFinish()
            Return image_id
        Else
            log_text.AppendLine("File Missing: " + fs)
        End If
        Return Nothing
    End Function
    Public Function load_jpg_file(ByVal fs As String) As Integer
        Dim image_id As Integer = -1

        Dim texID As UInt32
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoad(Il.IL_JPG, fs)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)


            Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE)

            success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            Gl.glGenTextures(1, image_id)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_NEAREST)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            Gl.glGenerateMipmapEXT(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)
            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            Return image_id
        Else
            log_text.AppendLine("File Missing: " + fs)
        End If
        Return Nothing
    End Function

    Public Function get_texture_from_stream(ByRef ms As MemoryStream) As Integer

        Dim texID As UInt32
        Dim image_id As Integer
        ms.Position = 0
        Dim textIn(ms.Length) As Byte
        ms.Read(textIn, 0, ms.Length)
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError

        Il.ilLoadL(Il.IL_DDS, textIn, textIn.Length)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            Il.ilEnable(Il.IL_FILE_OVERWRITE)
            'Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

            success = Il.ilConvertImage(Il.IL_RGBA, Il.IL_UNSIGNED_BYTE) ' Convert every colour component into unsigned bytes
            'If your image contains alpha channel you can replace IL_RGB with IL_RGBA */
            Gl.glGenTextures(1, image_id)
            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, image_id)
            If largestAnsio > 0 Then
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)
            End If
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_NEAREST_MIPMAP_LINEAR)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE)

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT)
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT)

            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Il.ilGetInteger(Il.IL_IMAGE_BPP), Il.ilGetInteger(Il.IL_IMAGE_WIDTH), _
            Il.ilGetInteger(Il.IL_IMAGE_HEIGHT), 0, Il.ilGetInteger(Il.IL_IMAGE_FORMAT), Gl.GL_UNSIGNED_BYTE, _
            Il.ilGetData()) '  Texture specification 
            Gl.glFinish()
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Else
            MsgBox("Out of memory error at :get texture:", MsgBoxStyle.Critical, "Well Shit!")
            End
        End If
        Il.ilBindImage(0)
        Ilu.iluDeleteImage(texID)
        Return image_id
        'ms.Close()
        'ms.Dispose()
        'GC.Collect()
    End Function

    Public Function get_image(ByVal file As String) As Image
        'Dim s As String = ""
        's = Gl.glGetError
        Dim image_id As Integer = -1
        'Dim app_local As String = Application.StartupPath.ToString

        Dim texID As UInt32
        texID = Ilu.iluGenImage() ' /* Generation of one image name */
        Il.ilBindImage(texID) '; /* Binding of image name */
        Dim success = Il.ilGetError
        Il.ilLoadImage(file)
        success = Il.ilGetError
        If success = Il.IL_NO_ERROR Then
            'Ilu.iluFlipImage()
            'Ilu.iluMirror()
            Dim width As Integer = Il.ilGetInteger(Il.IL_IMAGE_WIDTH)
            Dim height As Integer = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT)

            ' Create the bitmap.
            Dim Bitmapi = New System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb)
            Dim rect As Rectangle = New Rectangle(0, 0, width, height)

            ' Store the DevIL image data into the bitmap.
            Dim bitmapData As BitmapData = Bitmapi.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

            Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE)
            Il.ilCopyPixels(0, 0, 0, width, height, 1, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE, bitmapData.Scan0)
            Bitmapi.UnlockBits(bitmapData)

            Il.ilBindImage(0)
            Ilu.iluDeleteImage(texID)
            GC.Collect()
            Dim istream As New MemoryStream
            Bitmapi.Save(istream, ImageFormat.Png)
            Bitmapi.Dispose()
            Return Image.FromStream(istream)
        Else
            MsgBox("png load error!", MsgBoxStyle.Exclamation, "Oh No!!")
            Return Nothing
        End If
        Return Nothing
    End Function


End Module
