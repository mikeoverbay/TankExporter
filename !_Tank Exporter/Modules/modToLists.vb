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
        ' Fix 5: removed the outer Try/Catch that silently swallowed every exception
        ' with an empty handler.  Exceptions now propagate naturally to the caller so
        ' failures are visible instead of producing a silently corrupted comp_.

        comp = New comp_

        ' Fix 6: use the authoritative nPrimitives_ / nVertices_ counts instead of
        ' deriving counts from .Length on the indices/vertices arrays.  .Length is
        ' fragile — if either array is ever over-allocated by one slot the counts
        ' would be wrong.  Local aliases fc/vc are used throughout for clarity.
        Dim fc As Integer = fbxgrp(fbx_id).nPrimitives_
        Dim vc As Integer = fbxgrp(fbx_id).nVertices_

        comp.indi_cnt = fc * 3
        comp.nPrimitives = fc
        comp.vert_cnt = vc

        ' Fix 1 & 2: comp.vertices is now a deep copy — each slot is a NEW vertice_
        ' instance, fully independent of fbxgrp and _group.
        '
        ' Previously: comp.vertices = fbxgrp(fbx_id).vertices was a reference alias
        ' (both pointed at the same backing array), so any write through comp would
        ' silently corrupt fbxgrp.
        '
        ' Fields are sourced from two places:
        '   fbxgrp  — geometry the artist may have changed in the 3D app
        '             (x/y/z, nx/ny/nz, u, bone indices/weights)
        '   _group  — packed binary fields the FBX format cannot carry
        '             (n, t, bn) and raw v (fbxgrp.v = 1-raw_v from the export flip)
        '
        ' v is un-flipped here (1 - fbxgrp.v) so comp.vertices.v = raw game-file
        ' value, ready to write directly.  The writer no longer needs to touch
        ' _group at all — all fields come from comp.
        ReDim comp.vertices(vc - 1)
        For i As Integer = 0 To vc - 1
            comp.vertices(i) = New vertice_
            comp.vertices(i).x = fbxgrp(fbx_id).vertices(i).x
            comp.vertices(i).y = fbxgrp(fbx_id).vertices(i).y
            comp.vertices(i).z = fbxgrp(fbx_id).vertices(i).z
            comp.vertices(i).nx = fbxgrp(fbx_id).vertices(i).nx
            comp.vertices(i).ny = fbxgrp(fbx_id).vertices(i).ny
            comp.vertices(i).nz = fbxgrp(fbx_id).vertices(i).nz
            comp.vertices(i).u = fbxgrp(fbx_id).vertices(i).u
            ' Fix 2: do NOT flip v here.  fbx_reader.dll already reverses the
            ' exporter's (1-v) transform: it delivers v = raw game value directly.
            ' The previous "1.0F - fbxgrp.v" was a double-flip that produced (1-v_raw),
            ' causing the writer to store the wrong UV and corrupting all UV0 v coords.
            comp.vertices(i).v = fbxgrp(fbx_id).vertices(i).v   ' v_raw — ready to write as-is
            comp.vertices(i).u2 = fbxgrp(fbx_id).vertices(i).u2
            comp.vertices(i).v2 = fbxgrp(fbx_id).vertices(i).v2
            comp.vertices(i).index_1 = fbxgrp(fbx_id).vertices(i).index_1
            comp.vertices(i).index_2 = fbxgrp(fbx_id).vertices(i).index_2
            comp.vertices(i).index_3 = fbxgrp(fbx_id).vertices(i).index_3
            comp.vertices(i).index_4 = fbxgrp(fbx_id).vertices(i).index_4
            comp.vertices(i).weight_1 = fbxgrp(fbx_id).vertices(i).weight_1
            comp.vertices(i).weight_2 = fbxgrp(fbx_id).vertices(i).weight_2
            comp.vertices(i).weight_3 = fbxgrp(fbx_id).vertices(i).weight_3
            comp.vertices(i).weight_4 = fbxgrp(fbx_id).vertices(i).weight_4
            ' packed fields lost in FBX round-trip — carry forward from original load
            comp.vertices(i).n = _group(fbx_id).vertices(i).n
            comp.vertices(i).t = _group(fbx_id).vertices(i).t
            comp.vertices(i).bn = _group(fbx_id).vertices(i).bn
        Next

        ' Fix 2 & 3: UV2 extraction now uses vc (authoritative count) as loop bound.
        ' Removed two dead statements from the old UV2 block:
        '   comp.vertices(i) = New vertice_  — wrote into the array that was about to
        '                                       be thrown away by the reference assign.
        '   fbxgrp(fbx_id).vertices(i).found = False — side-effect on global fbxgrp
        '                                       state, unrelated to building comp_.
        If fbxgrp(fbx_id).has_uv2 = 1 Then
            ReDim fbx_uv2s(vc - 1)
            uv2_total_count = 0
            For i As Integer = 0 To vc - 1
                fbx_uv2s(uv2_total_count) = New uv_
                fbx_uv2s(uv2_total_count).u = fbxgrp(fbx_id).vertices(i).u2
                fbx_uv2s(uv2_total_count).v = fbxgrp(fbx_id).vertices(i).v2
                uv2_total_count += 1
            Next
        End If

        ' Fix 2: comp.indices sized exactly indi_cnt.  Previously ReDim'd as
        ' (indices.Length * 3) which created one extra unused trailing slot.
        ReDim comp.indices(comp.indi_cnt - 1)
        Dim v_cnt As Integer = 0
        For i As Integer = 0 To comp.indi_cnt - 1 Step 3
            comp.indices(i + 0) = fbxgrp(fbx_id).indices(v_cnt).v1
            comp.indices(i + 1) = fbxgrp(fbx_id).indices(v_cnt).v2
            comp.indices(i + 2) = fbxgrp(fbx_id).indices(v_cnt).v3
            v_cnt += 1
        Next

        ' Fix 4: removed the unreachable vertex-deduplication block that previously
        ' followed an early "Return comp" statement.  That entire For loop
        ' (get_vert calls, comp.vertices/indices rebuilding, ReDim Preserve) was
        ' dead code — it could never execute.

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
