Imports System.IO
Imports System.Text
Module shader_loader
    Public shader_list As New shader_list_
    Public Class shader_list_
        Public AtlasPBR_shader As Integer
        Public basic_shader As Integer
        Public backDepth_shader As Integer
        Public bloom_shader As Integer
        Public blurBchannel_shader As Integer
        Public blurR_shader As Integer
        Public camoExporter_shader As Integer
        Public channelMute_shader As Integer
        Public colorMult_shader As Integer
        Public convertNormalMap_shader As Integer
        Public cube_shader As Integer
        Public decalsCpass_shader As Integer
        Public depth_shader As Integer
        Public dome_shader As Integer
        Public fbx_shader As Integer
        Public FXAA_shader As Integer
        Public gaussian_shader As Integer
        Public gDetail_shader As Integer
        Public mixer_shader As Integer
        Public normal_shader As Integer
        Public r2mono_shader As Integer
        Public shadowTest_shader As Integer
        Public tank_shader As Integer
        Public terrainShader_shader As Integer
        Public textureBuilder_shader As Integer
        Public textureNormalBuilder_shader As Integer
        Public toLinear_shader As Integer
    End Class

#Region "variables"
    Public shaders As shaders__
    Public Structure shaders__
        Public shader() As shaders_
        Public Function f(ByVal name As String) As Integer
            For Each s In shader
                If s.shader_name = name Then
                    Return s.shader_id
                End If
            Next
            Return 0
        End Function
    End Structure
    Public Structure shaders_
        Public fragment As String
        Public vertex As String
        Public geo As String
        Public shader_name As String
        Public shader_id As Integer
        Public has_geo As Boolean
        Public Sub set_call_id(ByVal id As Integer)
            Try
                CallByName(shader_list, Me.shader_name, CallType.Set, Me.shader_id)
            Catch ex As Exception
                MsgBox("missing member from shader_list:" + Me.shader_name, MsgBoxStyle.Exclamation, "Oops!")
                End
            End Try
        End Sub
    End Structure

#End Region


    Public Sub make_shaders()
        'I'm tierd of all the work every time I add a shader.
        'So... Im going to automate the process.. Hey.. its a computer for fucks sake!
        'This works by figuring out what files are in the shaders folder.. paring them up and creating the shader.
        'Names are important! Only one "_" is allowed in the names as a delimiter. _vertex, _geo and _fragment.
        Dim f_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*fragment.glsl")
        Dim v_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*vertex.glsl")
        Dim g_list() As String = IO.Directory.GetFiles(Application.StartupPath + "\shaders\", "*geo.glsl")
        Array.Sort(f_list)
        Array.Sort(v_list)
        Array.Sort(g_list)
        ReDim shaders.shader(f_list.Length - 1)
        With shaders

            For i = 0 To f_list.Length - 1
                .shader(i) = New shaders_
                With .shader(i)
                    Dim fn As String = Path.GetFileNameWithoutExtension(f_list(i))
                    Dim ar = fn.Split("_")
                    .shader_name = ar(0) + "_shader"
                    .fragment = f_list(i)
                    .vertex = v_list(i)
                    .geo = ""
                    For Each g In g_list ' find matching geo if there is one.. usually there wont be
                        If Path.GetFileNameWithoutExtension(g).Contains(ar(0)) Then
                            .geo = g
                            .has_geo = True ' found a matching geo so we need to set this true
                        End If
                    Next
                    .shader_id = -1
                    .set_call_id(-1)
                End With
            Next

        End With
        Dim fs As String
        Dim vs As String
        Dim gs As String

        For i = 0 To shaders.shader.Length - 1
            With shaders.shader(i)
                vs = .vertex
                fs = .fragment
                gs = .geo
                Dim id = assemble_shader(vs, gs, fs, .shader_id, .shader_name, .has_geo)
                .set_call_id(id)
                .shader_id = id
            End With
        Next

    End Sub
    Public Function assemble_shader(v As String, g As String, f As String, ByRef shader As Integer, ByRef name As String, ByRef has_geo As Boolean) As Integer
        frmShaderEditor.TopMost = False
        Dim vs(1) As String
        Dim gs(1) As String
        Dim fs(1) As String
        Dim vertexObject As Integer
        Dim geoObject As Integer
        Dim fragmentObject As Integer
        Dim status_code As Integer
        Dim info As New StringBuilder
        info.Length = 8192
        Dim info_l As Integer

        If shader > 0 Then
            Gl.glUseProgram(0)
            Gl.glDeleteProgram(shader)
            Gl.glGetProgramiv(shader, Gl.GL_DELETE_STATUS, status_code)
            Gl.glFinish()
        End If

        Dim e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        'have a hard time with files remaining open.. hope this fixes it! (yep.. it did)
        Using vs_s As New StreamReader(v)
            vs(0) = vs_s.ReadToEnd
            vs_s.Close()
            vs_s.Dispose()
            vs(0) = clean_shader(vs(0)) ' remove non_ascii characters
        End Using
        Using fs_s As New StreamReader(f)
            fs(0) = fs_s.ReadToEnd
            fs_s.Close()
            fs_s.Dispose()
            fs(0) = clean_shader(fs(0))
        End Using
        If has_geo Then
            Using gs_s As New StreamReader(g)
                gs(0) = gs_s.ReadToEnd
                gs_s.Close()
                gs_s.Dispose()
                gs(0) = clean_shader(gs(0))
            End Using
        End If


        vertexObject = Gl.glCreateShader(Gl.GL_VERTEX_SHADER)
        fragmentObject = Gl.glCreateShader(Gl.GL_FRAGMENT_SHADER)
        '--------------------------------------------------------------------
        shader = Gl.glCreateProgram()

        ' Compile vertex shader
        Gl.glShaderSource(vertexObject, 1, vs, vs(0).Length)
        Gl.glCompileShader(vertexObject)
        Gl.glGetShaderInfoLog(vertexObject, 8192, info_l, info)
        Gl.glGetShaderiv(vertexObject, Gl.GL_COMPILE_STATUS, status_code)
        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteShader(vertexObject)
            gl_error(name + "_vertex didn't compile!" + vbCrLf + info.ToString)

        End If

        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        If has_geo Then
            'geo
            geoObject = Gl.glCreateShader(Gl.GL_GEOMETRY_SHADER_EXT)
            Gl.glShaderSource(geoObject, 1, gs, gs(0).Length)
            Gl.glCompileShader(geoObject)
            Gl.glGetShaderInfoLog(geoObject, 8192, info_l, info)
            Gl.glGetShaderiv(geoObject, Gl.GL_COMPILE_STATUS, status_code)
            If Not status_code = Gl.GL_TRUE Then
                Gl.glDeleteShader(geoObject)
                gl_error(name + "_geo didn't compile!" + vbCrLf + info.ToString)

            End If
            e = Gl.glGetError
            If e <> 0 Then
                Dim s = Glu.gluErrorString(e)
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If
            If name.Contains("raytrace") Then

                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_INPUT_TYPE_EXT, Gl.GL_TRIANGLES)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_OUTPUT_TYPE_EXT, Gl.GL_LINE_STRIP)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 6)
            End If
            If name.Contains("normal") Then
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_INPUT_TYPE_EXT, Gl.GL_TRIANGLES)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_OUTPUT_TYPE_EXT, Gl.GL_LINE_STRIP)
                Gl.glProgramParameteriEXT(shader, Gl.GL_GEOMETRY_VERTICES_OUT_EXT, 18)
            End If

            e = Gl.glGetError
            If e <> 0 Then
                Dim s = Glu.gluErrorString(e)
                Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
                MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
            End If

        End If

        ' Compile fragment shader

        Gl.glShaderSource(fragmentObject, 1, fs, fs(0).Length)
        Gl.glCompileShader(fragmentObject)
        Gl.glGetShaderInfoLog(fragmentObject, 8192, info_l, info)
        Gl.glGetShaderiv(fragmentObject, Gl.GL_COMPILE_STATUS, status_code)

        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteShader(fragmentObject)
            gl_error(name + "_fragment didn't compile!" + vbCrLf + info.ToString)

        End If
        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        'attach shader objects
        Gl.glAttachShader(shader, fragmentObject)
        If has_geo Then
            Gl.glAttachShader(shader, geoObject)
        End If
        Gl.glAttachShader(shader, vertexObject)

        'link program
        Gl.glLinkProgram(shader)

        ' detach shader objects
        Gl.glDetachShader(shader, fragmentObject)
        If has_geo Then
            Gl.glDetachShader(shader, geoObject)
        End If
        Gl.glDetachShader(shader, vertexObject)

        e = Gl.glGetError
        If e <> 0 Then
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If

        Gl.glGetShaderiv(shader, Gl.GL_LINK_STATUS, status_code)

        If Not status_code = Gl.GL_TRUE Then
            Gl.glDeleteProgram(shader)
            gl_error(name + " did not link!" + vbCrLf + info.ToString)

        End If

        'delete shader objects
        Gl.glDeleteShader(fragmentObject)
        Gl.glGetShaderiv(fragmentObject, Gl.GL_DELETE_STATUS, status_code)
        If has_geo Then
            Gl.glDeleteShader(geoObject)
            Gl.glGetShaderiv(geoObject, Gl.GL_DELETE_STATUS, status_code)
        End If
        Gl.glDeleteShader(vertexObject)
        Gl.glGetShaderiv(vertexObject, Gl.GL_DELETE_STATUS, status_code)
        e = Gl.glGetError
        If e <> 0 Then
            'aways throws a error after deletion even though the status shows them as deleted.. ????
            Dim s = Glu.gluErrorString(e)
            Dim ms As String = System.Reflection.MethodBase.GetCurrentMethod().Name
            'MsgBox("Function: " + ms + vbCrLf + "Error! " + s, MsgBoxStyle.Exclamation, "OpenGL Issue")
        End If
        vs(0) = Nothing
        fs(0) = Nothing
        If has_geo Then
            gs(0) = Nothing
        End If
        GC.Collect()
        GC.WaitForFullGCComplete()

        frmShaderEditor.TopMost = True

        Return shader
    End Function

    Private Function clean_shader(ByRef s As String) As String
        'removes any non-ascii characters
        Dim encoder = Encoding.UTF8
        Dim ar = encoder.GetBytes(s)
        s = encoder.GetString(ar)
        Return s
    End Function
    Public Sub gl_error(s As String)
        s = s.Replace(vbLf, vbCrLf)
        s.Replace("0(", vbCrLf + "(")
        frmShaderError.Show()
        frmShaderError.er_tb.Text += s
    End Sub

    '==============================================================================================================
    'tank_shader
    Public tank_colorMap, tank_normalMap, tank_is_GAmap, tank_alphaRef, tank_viewPosition As Integer
    Public tank_GMM, tank_AO, tank_detailMap, tank_detailTiling, tank_detailPower As Integer
    Public tank_camo, tank_use_camo, tank_tile_vec4 As Integer
    Public tank_c0, tank_c1, tank_c2, tank_c3, tank_armorcolor, tank_camo_tiling, tank_exclude_camo As Integer
    Public tank_use_CM, tank_Camera As Integer
    Public tank_ambient, tank_specular, tank_total, tank_cubeMap, tank_LUT, tank_shadowMap As Integer
    Public tank_a_group, tank_b_group, tank_lightMatrix, tank_use_shadow As Integer
    Public tank_colorEnable, tank_is_Track As Integer
    Public tank_GMM_Toy_value, tank_use_GMM_Toy As Integer
    Private Sub set_tank_shader_variables()
        tank_a_group = Gl.glGetUniformLocation(shader_list.tank_shader, "u_ScaleFGDSpec")
        tank_b_group = Gl.glGetUniformLocation(shader_list.tank_shader, "u_ScaleDiffBaseMR")
        tank_colorMap = Gl.glGetUniformLocation(shader_list.tank_shader, "colorMap")
        tank_normalMap = Gl.glGetUniformLocation(shader_list.tank_shader, "normalMap")
        tank_shadowMap = Gl.glGetUniformLocation(shader_list.tank_shader, "shadowMap")
        tank_cubeMap = Gl.glGetUniformLocation(shader_list.tank_shader, "cubeMap")
        tank_LUT = Gl.glGetUniformLocation(shader_list.tank_shader, "u_brdfLUT")
        tank_GMM = Gl.glGetUniformLocation(shader_list.tank_shader, "gmmMap")
        tank_AO = Gl.glGetUniformLocation(shader_list.tank_shader, "aoMap")
        tank_detailMap = Gl.glGetUniformLocation(shader_list.tank_shader, "detailMap")
        tank_camo = Gl.glGetUniformLocation(shader_list.tank_shader, "camoMap")
        tank_is_GAmap = Gl.glGetUniformLocation(shader_list.tank_shader, "is_GAmap")
        tank_alphaRef = Gl.glGetUniformLocation(shader_list.tank_shader, "alphaRef")
        tank_detailTiling = Gl.glGetUniformLocation(shader_list.tank_shader, "detailTiling")
        tank_detailPower = Gl.glGetUniformLocation(shader_list.tank_shader, "detailPower")
        tank_use_camo = Gl.glGetUniformLocation(shader_list.tank_shader, "use_camo")
        tank_tile_vec4 = Gl.glGetUniformLocation(shader_list.tank_shader, "tile_vec4")
        tank_Camera = Gl.glGetUniformLocation(shader_list.tank_shader, "u_Camera")
        tank_c0 = Gl.glGetUniformLocation(shader_list.tank_shader, "c0")
        tank_c1 = Gl.glGetUniformLocation(shader_list.tank_shader, "c1")
        tank_c2 = Gl.glGetUniformLocation(shader_list.tank_shader, "c2")
        tank_c3 = Gl.glGetUniformLocation(shader_list.tank_shader, "c3")
        tank_armorcolor = Gl.glGetUniformLocation(shader_list.tank_shader, "armorcolor")
        tank_camo_tiling = Gl.glGetUniformLocation(shader_list.tank_shader, "camo_tiling")
        tank_exclude_camo = Gl.glGetUniformLocation(shader_list.tank_shader, "exclude_camo")
        tank_use_CM = Gl.glGetUniformLocation(shader_list.tank_shader, "use_CM")
        tank_ambient = Gl.glGetUniformLocation(shader_list.tank_shader, "A_level")
        tank_specular = Gl.glGetUniformLocation(shader_list.tank_shader, "S_level")
        tank_total = Gl.glGetUniformLocation(shader_list.tank_shader, "T_level")
        tank_lightMatrix = Gl.glGetUniformLocation(shader_list.tank_shader, "shadowProjection")
        tank_use_shadow = Gl.glGetUniformLocation(shader_list.tank_shader, "use_shadow")
        tank_colorEnable = Gl.glGetUniformLocation(shader_list.tank_shader, "enableVertexColor")
        tank_is_Track = Gl.glGetUniformLocation(shader_list.tank_shader, "is_track")
        tank_use_GMM_Toy = Gl.glGetUniformLocation(shader_list.tank_shader, "use_GMM_Toy")
        tank_GMM_Toy_value = Gl.glGetUniformLocation(shader_list.tank_shader, "GMM_Toy")

    End Sub

    '==============================================================================================================
    Public fbx_ambient, fbx_specular, fbx_level As Integer
    Public fbx_colorMap, fbx_specularMap, fbx_normalMap, fbx_is_GAmap, fbx_bumped As Integer
    Public fbx_alphatest, fbx_enableVcolor As Integer
    Private Sub set_fbx_shader_variables()
        fbx_ambient = Gl.glGetUniformLocation(shader_list.fbx_shader, "A_level")
        fbx_specular = Gl.glGetUniformLocation(shader_list.fbx_shader, "S_level")
        fbx_level = Gl.glGetUniformLocation(shader_list.fbx_shader, "T_level")
        fbx_colorMap = Gl.glGetUniformLocation(shader_list.fbx_shader, "colorMap")
        fbx_normalMap = Gl.glGetUniformLocation(shader_list.fbx_shader, "normalMap")
        fbx_specularMap = Gl.glGetUniformLocation(shader_list.fbx_shader, "specularMap")
        fbx_is_GAmap = Gl.glGetUniformLocation(shader_list.fbx_shader, "is_GAmap")
        fbx_bumped = Gl.glGetUniformLocation(shader_list.fbx_shader, "bumped")
        fbx_alphatest = Gl.glGetUniformLocation(shader_list.fbx_shader, "alphaTest")
        fbx_enableVcolor = Gl.glGetUniformLocation(shader_list.fbx_shader, "enableVcolor")
    End Sub

    '==============================================================================================================
    Public normal_shader_mode As Integer
    Public normal_shader_mode_id As Integer
    Private Sub set_normal_shader_variables()
        normal_shader_mode_id = Gl.glGetUniformLocation(shader_list.normal_shader, "mode")
    End Sub

    '==============================================================================================================
    Public mix_camoMap, mix_c0, mix_c1, mix_c2, mix_c3, mix_armorColor
    Private Sub set_mixer_shader_variables()
        mix_camoMap = Gl.glGetUniformLocation(shader_list.mixer_shader, "camoMap")
        mix_c0 = Gl.glGetUniformLocation(shader_list.mixer_shader, "c0")
        mix_c1 = Gl.glGetUniformLocation(shader_list.mixer_shader, "c1")
        mix_c2 = Gl.glGetUniformLocation(shader_list.mixer_shader, "c2")
        mix_c3 = Gl.glGetUniformLocation(shader_list.mixer_shader, "c3")
        mix_armorColor = Gl.glGetUniformLocation(shader_list.mixer_shader, "armorColor")
    End Sub

    '==============================================================================================================
    Public gaus_image, gaus_switch As Integer
    Public Sub set_gaussian_variables()
        gaus_image = Gl.glGetUniformLocation(shader_list.gaussian_shader, "image")
        gaus_switch = Gl.glGetUniformLocation(shader_list.gaussian_shader, "horizontal")
    End Sub

    '==============================================================================================================
    Public blurR_image, blurR_switch As Integer
    Public Sub set_blurR_variables()
        blurR_image = Gl.glGetUniformLocation(shader_list.blurR_shader, "image")
        blurR_switch = Gl.glGetUniformLocation(shader_list.blurR_shader, "horizontal")
    End Sub

    '==============================================================================================================
    Public blurB_image, blurB_switch As Integer
    Public Sub set_blurbchannel_variables()
        blurB_image = Gl.glGetUniformLocation(shader_list.blurBchannel_shader, "image")
        blurB_switch = Gl.glGetUniformLocation(shader_list.blurBchannel_shader, "horizontal")
    End Sub

    '==============================================================================================================
    Public FXAA_color, FXAA_screenSize As Integer
    Public Sub set_FXAA_variables()
        FXAA_color = Gl.glGetUniformLocation(shader_list.FXAA_shader, "colorMap")
        FXAA_screenSize = Gl.glGetUniformLocation(shader_list.FXAA_shader, "viewportSize")

    End Sub

    '==============================================================================================================
    Public cube_cubeMap As Integer
    Public Sub set_cube_variables()
        cube_cubeMap = Gl.glGetUniformLocation(shader_list.cube_shader, "cubeMap")
    End Sub

    '==============================================================================================================

    Public toLinear_depthMap As Integer
    '==============================================================================================================
    Public Sub set_toLinear_variables()
        toLinear_depthMap = Gl.glGetUniformLocation(shader_list.toLinear_shader, "depthMap")

    End Sub

    Public shadowTest_depthMap, shadowTest_normalMap, shadowTest_shadowProjection As Integer
    Public shadowTest_light_pos, shadowTest_alphaTest, shadowTest_alphaRef As Integer
    '==============================================================================================================
    Public Sub set_shadowTest_variables()
        shadowTest_depthMap = Gl.glGetUniformLocation(shader_list.shadowTest_shader, "shadowMap")
        shadowTest_normalMap = Gl.glGetUniformLocation(shader_list.shadowTest_shader, "normalMap")
        shadowTest_shadowProjection = Gl.glGetUniformLocation(shader_list.shadowTest_shader, "shadowProjection")
        shadowTest_alphaRef = Gl.glGetUniformLocation(shader_list.shadowTest_shader, "alphaRef")
        shadowTest_alphaTest = Gl.glGetUniformLocation(shader_list.shadowTest_shader, "alphaTest")
    End Sub

    Public terrain_depthMap, terrain_shadowProjection, terrain_textureMap, _
        terrain_normalMap, terrain_use_shadow, terrain_gradient, terrain_noise, terain_animation, terrain_camPosition As Integer
    '==============================================================================================================
    Public Sub set_terrain_variables()
        terrain_textureMap = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "colorMap")
        terrain_normalMap = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "normalMap")
        terrain_depthMap = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "shadowMap")
        terrain_shadowProjection = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "shadowProjection")
        terrain_use_shadow = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "use_shadow")
        terrain_gradient = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "gradientLU")
        terrain_noise = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "noise")
        terain_animation = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "shift")
        terrain_camPosition = Gl.glGetUniformLocation(shader_list.terrainShader_shader, "camPosition")
    End Sub

    Public depth_alphaTest, depth_alphaRef, depth_normalMap As Integer
    '==============================================================================================================
    Public Sub set_depth_variables()
        depth_alphaRef = Gl.glGetUniformLocation(shader_list.depth_shader, "alphaRef")
        depth_alphaTest = Gl.glGetUniformLocation(shader_list.depth_shader, "alphaTest")
        depth_normalMap = Gl.glGetUniformLocation(shader_list.depth_shader, "normalMap")
    End Sub

    Public Backdepth_alphaTest, Backdepth_alphaRef, Backdepth_normalMap As Integer
    '==============================================================================================================
    Public Sub set_Backdepth_variables()
        Backdepth_alphaRef = Gl.glGetUniformLocation(shader_list.backDepth_shader, "alphaRef")
        Backdepth_alphaTest = Gl.glGetUniformLocation(shader_list.backDepth_shader, "alphaTest")
        Backdepth_normalMap = Gl.glGetUniformLocation(shader_list.backDepth_shader, "normalMap")
    End Sub

    Public basic_alphaTest, basic_alphaRef, basic_normalMap As Integer
    '==============================================================================================================
    Public Sub set_basic_variables()
        basic_alphaRef = Gl.glGetUniformLocation(shader_list.depth_shader, "alphaRef")
        basic_alphaTest = Gl.glGetUniformLocation(shader_list.depth_shader, "alphaTest")
        basic_normalMap = Gl.glGetUniformLocation(shader_list.depth_shader, "normalMap")
    End Sub

    Public r2mono_shadow As Integer
    '==============================================================================================================
    Public Sub set_r2mono_variables()
        r2mono_shadow = Gl.glGetUniformLocation(shader_list.r2mono_shader, "shadow")
    End Sub

    Public decalC_colorMap, decalC_normalMap, decalC_surfaceNormalMap, decalC_shadowMap, decalC_decal_matrix As Integer
    Public decalC_depthMap, decalC_alpha, decalC_level, decalC_UVwrap, decalC_uv_rotate As Integer
    Public decalC_shadowProj, decalC_use_shadow, decalC_fog, decalC_gNormalMap As Integer
    Public decalC_camLocation, decalC_lightPosition, decalC_GMM, decalC_cube, decalC_brdf As Integer
    Public decalC_a_group, decalC_b_group As Integer
    '==============================================================================================================
    Public Sub set_decalsNpass_variables()
        decalC_depthMap = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "depthMap")
        decalC_colorMap = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "colorMap")
        decalC_shadowMap = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "shadowMap")
        decalC_normalMap = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "normalMap")
        decalC_surfaceNormalMap = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "surfaceNormalMap")

        decalC_gNormalMap = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "gNormalMap")
        decalC_GMM = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "gmmMap")
        decalC_cube = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "cubeMap")
        decalC_brdf = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "u_brdfLUT")
        decalC_a_group = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "u_ScaleFGDSpec")
        decalC_b_group = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "u_ScaleDiffBaseMR")

        decalC_fog = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "fogMap")
        decalC_uv_rotate = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "uv_rotate")

        decalC_decal_matrix = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "decal_matrix")
        decalC_shadowProj = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "shadowProjection")
        decalC_alpha = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "alpha_value")
        decalC_level = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "color_level")
        decalC_UVwrap = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "uv_wrap")
        decalC_use_shadow = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "use_shadow")
        decalC_camLocation = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "viewPos")
        decalC_lightPosition = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "LightPos")
    End Sub

    Public dome_colorMap, dome_noise, dome_LU, dome_shift, dome_camPosition As Integer

    '==============================================================================================================
    Public Sub set_dome_variables()
        dome_colorMap = Gl.glGetUniformLocation(shader_list.dome_shader, "colorMap")
    End Sub

    Public bloom_gcolor, bloom_blm_tex1, bloom_enableBloom, bloom_transparent As Integer
    '==============================================================================================================
    Public Sub set_bloom_variables()
        bloom_gcolor = Gl.glGetUniformLocation(shader_list.bloom_shader, "gColor")
        bloom_blm_tex1 = Gl.glGetUniformLocation(shader_list.bloom_shader, "blm_tex1")
        bloom_enableBloom = Gl.glGetUniformLocation(shader_list.bloom_shader, "show_bloom")
        bloom_transparent = Gl.glGetUniformLocation(shader_list.bloom_shader, "transparent")

    End Sub

    Public colorMult_color1, colorMult_color2 As Integer
    '==============================================================================================================
    Public Sub set_colorMult_variables()
        colorMult_color1 = Gl.glGetUniformLocation(shader_list.colorMult_shader, "color1")
        colorMult_color2 = Gl.glGetUniformLocation(shader_list.colorMult_shader, "color2")
    End Sub

    Public chMute_channels, chMute_texture As Integer
    '==============================================================================================================
    Public Sub set_channelMute_variables()
        chMute_channels = Gl.glGetUniformLocation(shader_list.channelMute_shader, "mask")
        chMute_texture = Gl.glGetUniformLocation(shader_list.channelMute_shader, "colorMap")
    End Sub

    Public CE_camo_Map, CE_AO_Map, CE_AM_Map, CE_tile, CE_camo_tile As Integer
    '==============================================================================================================
    Public Sub set_camoExporter_variables()
        CE_camo_Map = Gl.glGetUniformLocation(shader_list.camoExporter_shader, "camo_Map")
        CE_AO_Map = Gl.glGetUniformLocation(shader_list.camoExporter_shader, "AO_Map")
        CE_AM_Map = Gl.glGetUniformLocation(shader_list.camoExporter_shader, "AM_Map")
        CE_tile = Gl.glGetUniformLocation(shader_list.camoExporter_shader, "tile")
        CE_camo_tile = Gl.glGetUniformLocation(shader_list.camoExporter_shader, "camo_tile")
    End Sub

    Public atlasPBR_atlas_TILE, atlasPBR_sizes, atlasPBR_g_tile0Tint, atlasPBR_g_tile1Tint, atlasPBR_g_tile2Tint As Integer
    Public atlasPBR_g_dirtColor, atlasPBR_use_normapMap, atlasPBR_UVrepete, atlasPBR_dirtParams As Integer
    Public atlasPBR_colorMap, atlasPBR_colorMap2, atlasPBR_normalMap, atlasPBR_Tex_Size As Integer
    Public atlasPBR_atlas_AM_map, atlasPBR_atlas_GBMT_map, atlasPBR_atlas_MAO_map, atlasPBR_BLEND_map, atlasPBR_DIRT_map As Integer
    Public atlasPBR_image_size, atlasPBR_a_group, atlasPBR_b_group, atlasPBR_cube, atlasPBR_brdf As Integer
    Public atlasPBR_IS_ATLAS, atlasPBR_USE_UV2, atlasPBR_ambient, atlasPBR_specular, atlasPBR_brightness As Integer
    Public atlasPBR_INDEXES, atlasPBR_is_ANM, atlasPBR_GMM_Map, atlasPBR_alpha_enable, atlasPBR_alpha_value As Integer
    Public atlasPBR_camPos As Integer
    '==============================================================================================================
    Public Sub set_AtlasPBR_shader_variables()
        atlasPBR_atlas_AM_map = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "ATLAS_AM_Map")
        atlasPBR_atlas_GBMT_map = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "ATLAS_GBMT_Map")
        atlasPBR_atlas_MAO_map = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "ATLAS_MAO_Map")

        atlasPBR_BLEND_map = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "ATLAS_BLEND_MAP")
        atlasPBR_DIRT_map = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "ATLAS_DIRT_MAP")

        atlasPBR_colorMap = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "colorMap")
        atlasPBR_colorMap2 = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "colorMap2")
        atlasPBR_normalMap = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "normalMap")
        atlasPBR_GMM_Map = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "GMM_map")

        atlasPBR_cube = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "cubeMap")
        atlasPBR_brdf = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "u_brdfLUT")

        atlasPBR_atlas_TILE = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "atlas_TILE")
        atlasPBR_sizes = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "atlas_sizes")
        atlasPBR_INDEXES = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "atlas_indexes")

        atlasPBR_g_tile0Tint = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "g_tile0Tint")
        atlasPBR_g_tile1Tint = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "g_tile1Tint")
        atlasPBR_g_tile2Tint = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "g_tile2Tint")
        atlasPBR_g_dirtColor = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "g_dirtColor")

        atlasPBR_IS_ATLAS = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "IS_ATLAS")
        atlasPBR_USE_UV2 = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "use_UV2")

        atlasPBR_ambient = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "A_level")
        atlasPBR_specular = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "S_level")
        atlasPBR_brightness = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "T_level")
        atlasPBR_use_normapMap = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "use_normapMAP")
        atlasPBR_is_ANM = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "is_ANM_Map")
        atlasPBR_UVrepete = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "UV_tiling")
        atlasPBR_Tex_Size = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "tex_size")
        atlasPBR_alpha_enable = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "alpha_enable")
        atlasPBR_alpha_value = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "alpha_value")
        atlasPBR_image_size = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "image_size")
        atlasPBR_dirtParams = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "dirt_params")

        atlasPBR_a_group = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "u_ScaleFGDSpec")
        atlasPBR_b_group = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "u_ScaleDiffBaseMR")
        atlasPBR_camPos = Gl.glGetUniformLocation(shader_list.AtlasPBR_shader, "camPosition")

    End Sub

    Public gDetail_colorMap, gDetail_normalMap, gDetail_GMM_Map, gDetail_detailMap, gDetail_inf, g_Detail_reject_tiling As Integer
    Public gDetail_A_level, gDetail_S_level, gDetail_T_level, gDetail_cubeMap, gDetail_has_Detail As Integer
    Public gDetail_alpha_enabled, gDetail_alpha_value, gDetail_dirtParams, gDetail_is_glass As Integer
    Public gDetail_a_group, gDetail_b_group, gDetail_brdf, gDetail_camPos As Integer
    '==============================================================================================================
    Public Sub set_gDetail_shader_variables()
        gDetail_colorMap = Gl.glGetUniformLocation(shader_list.gDetail_shader, "colorMap")
        gDetail_normalMap = Gl.glGetUniformLocation(shader_list.gDetail_shader, "normalMap")
        gDetail_GMM_Map = Gl.glGetUniformLocation(shader_list.gDetail_shader, "GMM_Map")
        gDetail_detailMap = Gl.glGetUniformLocation(shader_list.gDetail_shader, "detailMap")
        gDetail_cubeMap = Gl.glGetUniformLocation(shader_list.gDetail_shader, "cubeMap")
        gDetail_brdf = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "u_brdfLUT")

        gDetail_camPos = Gl.glGetUniformLocation(shader_list.decalsCpass_shader, "camPosition")

        g_Detail_reject_tiling = Gl.glGetUniformLocation(shader_list.gDetail_shader, "g_detailRejectTiling")
        gDetail_inf = Gl.glGetUniformLocation(shader_list.gDetail_shader, "g_detailInfluences")
        g_Detail_reject_tiling = Gl.glGetUniformLocation(shader_list.gDetail_shader, "g_detailRejectTiling")

        gDetail_A_level = Gl.glGetUniformLocation(shader_list.gDetail_shader, "A_level")
        gDetail_S_level = Gl.glGetUniformLocation(shader_list.gDetail_shader, "S_level")
        gDetail_T_level = Gl.glGetUniformLocation(shader_list.gDetail_shader, "T_level")

        gDetail_alpha_enabled = Gl.glGetUniformLocation(shader_list.gDetail_shader, "alpha_enable")
        gDetail_alpha_value = Gl.glGetUniformLocation(shader_list.gDetail_shader, "alpha_value")
        gDetail_is_glass = Gl.glGetUniformLocation(shader_list.gDetail_shader, "is_glass")
        gDetail_has_Detail = Gl.glGetUniformLocation(shader_list.gDetail_shader, "has_detail_map")

        gDetail_a_group = Gl.glGetUniformLocation(shader_list.gDetail_shader, "u_ScaleFGDSpec")
        gDetail_b_group = Gl.glGetUniformLocation(shader_list.gDetail_shader, "u_ScaleDiffBaseMR")

    End Sub

    Public textureBuilder_atlasAM, textureBuilder_atlasBlend, textureBuilder_atlasDirt, textureBuilder_tint0, textureBuilder_tint1, textureBuilder_tint2, textureBuilder_dirtColor As Integer
    Public textureBuilder_repeat, textureBuilder_atlasSize, textureBuilder_indexes As Integer
    '==============================================================================================================
    Public Sub set_textureBuilder_variables()
        textureBuilder_atlasAM = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "atlasMap")
        textureBuilder_atlasBlend = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "atlasBlend")
        textureBuilder_atlasDirt = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "atlasDirt")

        textureBuilder_tint0 = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "g_tile0Tint")
        textureBuilder_tint1 = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "g_tile1Tint")
        textureBuilder_tint2 = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "g_tile2Tint")
        textureBuilder_dirtColor = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "g_dirtColor")

        textureBuilder_repeat = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "UV_tiling")
        textureBuilder_atlasSize = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "atlas_sizes")
        textureBuilder_indexes = Gl.glGetUniformLocation(shader_list.textureBuilder_shader, "atlas_indexes")

    End Sub
    Public textureNormalBuilder_atlasAM, textureNormalBuilder_atlasBlend As Integer
    Public textureNormalBuilder_repeat, textureNormalBuilder_atlasSize, textureNormalBuilder_indexes As Integer
    Public textureNormalBuilder_convert As Integer
    '==============================================================================================================
    Public Sub set_textureNormalBuilder_variables()
        textureNormalBuilder_atlasAM = Gl.glGetUniformLocation(shader_list.textureNormalBuilder_shader, "atlasMap")
        textureNormalBuilder_atlasBlend = Gl.glGetUniformLocation(shader_list.textureNormalBuilder_shader, "atlasBlend")

        textureNormalBuilder_repeat = Gl.glGetUniformLocation(shader_list.textureNormalBuilder_shader, "UV_tiling")
        textureNormalBuilder_atlasSize = Gl.glGetUniformLocation(shader_list.textureNormalBuilder_shader, "atlas_sizes")
        textureNormalBuilder_indexes = Gl.glGetUniformLocation(shader_list.textureNormalBuilder_shader, "atlas_indexes")

        textureNormalBuilder_convert = Gl.glGetUniformLocation(shader_list.textureNormalBuilder_shader, "convert")

    End Sub

    Public convertMap_map, convertMap_flip_y, convertMap_convert, convertMap_alpha_enabled As Integer
    '==============================================================================================================
    Public Sub set_convertNormalMap_variables()
        convertMap_map = Gl.glGetUniformLocation(shader_list.convertNormalMap_shader, "map")
        convertMap_convert = Gl.glGetUniformLocation(shader_list.convertNormalMap_shader, "convert")
        convertMap_flip_y = Gl.glGetUniformLocation(shader_list.convertNormalMap_shader, "flip_y")
        convertMap_alpha_enabled = Gl.glGetUniformLocation(shader_list.convertNormalMap_shader, "alpha_enabled")

    End Sub

    Public Sub set_shader_variables()

        set_textureBuilder_variables()
        set_textureNormalBuilder_variables()
        set_convertNormalMap_variables()
        set_gDetail_shader_variables()
        set_AtlasPBR_shader_variables()
        set_tank_shader_variables()
        set_normal_shader_variables()
        set_mixer_shader_variables()
        set_fbx_shader_variables()
        set_gaussian_variables()
        set_FXAA_variables()
        set_cube_variables()
        set_toLinear_variables()
        set_shadowTest_variables()
        set_terrain_variables()
        set_depth_variables()
        set_basic_variables()
        set_r2mono_variables()
        set_decalsNpass_variables()
        set_dome_variables()
        set_bloom_variables()
        set_colorMult_variables()
        set_channelMute_variables()
        set_camoExporter_variables()
        set_blurbchannel_variables()
        set_blurR_variables()
        Return
    End Sub
    '==============================================================================================================

End Module
