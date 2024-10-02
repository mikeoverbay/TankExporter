#Region "imports"
Imports System.Windows
Imports System.Windows.Forms
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
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Skill.FbxSDK
Imports Skill.FbxSDK.IO
Imports cttools
Imports System.Data.Common
Imports System.Reflection



#End Region



Module modToLists
    Public temp_list As Integer
    'Public comp As comp_
    Public Structure comp_

        Public vertices() As vertice_
        Public indices() As Integer
        Public vert_cnt As Integer
        Public indi_cnt As Integer
        Public nPrimitives As Integer
    End Structure


    Public Function compact_primitive(ByVal fbx_id As Integer, ByRef comp As comp_) As comp_
        Try

            comp = New comp_
            ReDim comp.vertices(fbxgrp(fbx_id).vertices.Length)
            ReDim comp.indices((fbxgrp(fbx_id).indices.Length) * 3)
            If fbxgrp(fbx_id).has_uv2 = 1 Then
                ReDim fbx_uv2s(fbxgrp(fbx_id).vertices.Length)
                uv2_total_count = 0
                For i As Integer = 0 To fbxgrp(fbx_id).vertices.Length - 1
                    fbx_uv2s(uv2_total_count) = New uv_
                    fbx_uv2s(uv2_total_count).u = fbxgrp(fbx_id).vertices(i).u2
                    fbx_uv2s(uv2_total_count).v = fbxgrp(fbx_id).vertices(i).v2
                    uv2_total_count += 1
                    comp.vertices(i) = New vertice_
                    fbxgrp(fbx_id).vertices(i).found = False
                Next
                ReDim Preserve fbx_uv2s(uv2_total_count - 1)
            End If

            Dim i_cnt As Integer
            Dim indx As Integer
            Dim v_cnt As Integer = 0

            comp.indi_cnt = (fbxgrp(fbx_id).indices.Length) * 3
            comp.nPrimitives = fbxgrp(fbx_id).nPrimitives_
            comp.vert_cnt = fbxgrp(fbx_id).nVertices_
            comp.vertices = fbxgrp(fbx_id).vertices
            For i = 0 To comp.indi_cnt - 1 Step 3
                comp.indices(i + 0) = fbxgrp(fbx_id).indices(v_cnt).v1
                comp.indices(i + 1) = fbxgrp(fbx_id).indices(v_cnt).v2
                comp.indices(i + 2) = fbxgrp(fbx_id).indices(v_cnt).v3
                v_cnt += 1
            Next

            Return comp

            frmMain.PG1.Value = 0
            frmMain.PG1.Maximum = fbxgrp(fbx_id).nPrimitives_ * 3 - 1
            For i As Integer = 0 To fbxgrp(fbx_id).nPrimitives_
                frmMain.PG1.Value = i
                Dim id As Integer = get_vert(fbxgrp(fbx_id).vertices(i), indx, v_cnt, comp)
                If id > -1 Then
                    comp.indices(i_cnt) = indx
                    i_cnt += 1
                Else
                    fbxgrp(fbx_id).vertices(i).found = True
                    comp.vertices(indx) = fbxgrp(fbx_id).vertices(i)
                    comp.vertices(indx).found = True
                    comp.indices(i_cnt) = indx
                    i_cnt += 1
                    v_cnt += 1

                End If
            Next
            ReDim Preserve comp.vertices(v_cnt - 1)
            ReDim Preserve comp.indices(i_cnt - 1)
            comp.indi_cnt = i_cnt
            comp.vert_cnt = v_cnt
            comp.nPrimitives = CInt(i_cnt / 3)
            'make_temp_list(comp) test function to make sure its correct
        Catch ex As Exception

        End Try
        Return comp
    End Function
    Private Sub make_temp_list(ByRef comp As comp_)
        If temp_list > 0 Then
            Gl.glDeleteLists(temp_list, 1)
        End If
        temp_list = Gl.glGenLists(1)
        Gl.glNewList(temp_list, Gl.GL_COMPILE)

        Gl.glBegin(Gl.GL_TRIANGLES)
        For i = 0 To comp.indi_cnt - 1
            Dim p = comp.indices(i)
            Dim x = comp.vertices(p).x
            Dim y = comp.vertices(p).y
            Dim z = comp.vertices(p).z
            Dim nx = comp.vertices(p).nx
            Dim ny = comp.vertices(p).ny
            Dim nz = comp.vertices(p).nz
            Gl.glNormal3f(nx, ny, nz)
            Gl.glVertex3f(x, y, z)
        Next

        Gl.glEnd()
        Gl.glEndList()
    End Sub
    Private Function get_vert(v1 As vertice_, ByRef indx As Integer, ByVal v_cnt As Integer, ByRef comp As comp_) As Integer
        For i = 0 To v_cnt + 1
            If v1.x = comp.vertices(i).x Then
                If v1.y = comp.vertices(i).y Then
                    If v1.z = comp.vertices(i).z Then
                        If v1.u = comp.vertices(i).u Then
                            If v1.v = comp.vertices(i).v Then
                                indx = i
                                Return i
                            End If
                        End If
                    End If
                End If
            End If
            If Not comp.vertices(i).found Then
                indx = i
                Return -1
            End If
        Next
        Return &HFFFF
    End Function

End Module
