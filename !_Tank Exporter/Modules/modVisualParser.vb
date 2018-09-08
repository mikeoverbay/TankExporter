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
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports System.Drawing.Imaging
Imports System.Globalization
Imports Skill.FbxSDK

#End Region


Module modVisualParser
    <Extension()> _
    Public Function Split(ByVal input As String, _
                          ByVal ParamArray delimiter As String()) As String()
        Return input.Split(delimiter, StringSplitOptions.None)
    End Function
    Public boneMarker As Integer
    Public boneMarker2 As Integer
    Public boneMarker3 As Integer
    Public boneMarker4 As Integer

    Public visualXML_str As String
    Public myPath As String

    Public v_boneGroups(1) As v_boneGroups_
    Public fbx_boneGroups(1) As v_boneGroups_
    Public Structure v_boneGroups_
        Public groupName As String
        Public node_list() As String
        Public nodeCnt As Integer
        Public node_matrices() As mat_
        Public models() As Vmodel_
        Public isTrack As Boolean
    End Structure
    Public Structure mat_
        Public mat() As Double
    End Structure
    Public Structure Vmodel_
        Public type As Integer
        Public color As vColor_
        Public displayId As Integer
        Public scale As FbxVector4
        Public rotation As FbxVector4
        Public translation As FbxVector4
        Public removed As Boolean
        Public found As Boolean
        Public newItem As Boolean
    End Structure

    'Public Structure vec3
    '    Public x, y, z As Single
    'End Structure
    Public Structure vColor_
        Public r, g, b, a As Single
    End Structure
    'there are these different types
    '1 V_
    '2 WD_
    '3 W_
    '4 Track
    '5 Chassis
    Public vc0, vc1, vc2, vc3, vc4, vc5 As vColor_

    Private Function set_vc_color(ByVal c As Color) As vColor_
        Dim co As New vColor_
        co.r = CSng(c.R / 255)
        co.g = CSng(c.G / 255)
        co.b = CSng(c.B / 255)
        co.a = 1.0
        Return co
    End Function

    Public Sub praseVisualXml()
        'define our colors
        vc1 = set_vc_color(Color.Maroon)
        vc2 = set_vc_color(Color.Green)
        vc3 = set_vc_color(Color.Navy)
        vc4 = set_vc_color(Color.Olive)
        vc5 = set_vc_color(Color.Purple)
        vc0 = set_vc_color(Color.Gray)


        Dim s = TheXML_String.Replace("  ", "")
        'first,clean up the string
        s = s.Replace("</renderSet>", "")
        s = s.Replace("</geometry>", "")

        Dim delim As String = "<renderSet>"
        Dim s_sections = s.Split(delim)
        'resize v_boneGroups
        ReDim v_boneGroups(s_sections.Length - 2)
        For i = 0 To s_sections.Length - 2
            v_boneGroups(i) = New v_boneGroups_
            Dim suba = s_sections(i + 1).Split("<geometry>")
            Dim subb = suba(0).Split(vbLf)
            v_boneGroups(i).groupName = trim_string(suba(1))
            If v_boneGroups(i).groupName.ToLower.Contains("tra") Then
                v_boneGroups(i).isTrack = True
            Else
                v_boneGroups(i).isTrack = False
            End If
            Dim cnt As Integer = 0
            For k = 0 To subb.Length - 1
                If subb(k).Length > 0 Then
                    If Not subb(k).ToLower.Contains("true") Then
                        If Not subb(k).Length < 3 Then
                            cnt += 1
                        End If

                    End If
                End If
            Next
            ReDim v_boneGroups(i).node_list(cnt - 1)
            ReDim v_boneGroups(i).models(cnt - 1)
            ReDim v_boneGroups(i).node_matrices(cnt - 1)
            cnt = 0
            For k = 0 To subb.Length - 1
                If subb(k).Length > 0 Then
                    If Not subb(k).ToLower.Contains("true") Then
                        If Not subb(k).Length < 3 Then
                            v_boneGroups(i).node_list(cnt) = trim_string(subb(k))
                            v_boneGroups(i).nodeCnt = cnt + 1
                            v_boneGroups(i).node_matrices(cnt) = New mat_
                            ReDim v_boneGroups(i).node_matrices(cnt).mat(15)
                            get_type_and_color(v_boneGroups(i), cnt) 'figures out what type of entry this is
                            find_visual_matrix(s_sections(0), v_boneGroups(i), cnt) 'find and get the matrix for this entry
                            cnt += 1
                        End If
                    End If
                End If
            Next



        Next
    End Sub
    Private Sub find_visual_matrix(ByRef vs As String, ByRef v_r As v_boneGroups_, ByRef i As Integer)
        'Find and get the matrix for this entry
        Dim s_inx As Integer = 0
        Dim da = vs.Split(vbLf)
        For z = 0 To da.Length - 1
            If da(z).ToLower.Contains(v_r.node_list(i).ToLower) Then
                v_r.node_matrices(i) = get_vr_matrix(da, z)
                Return
            End If

        Next


    End Sub
    Private Function get_vr_matrix(ByRef da() As String, ByRef idx As Integer) As mat_
        Dim mat As New mat_
        Dim v1 = get_vec3_from_string(da(idx + 2))
        Dim v2 = get_vec3_from_string(da(idx + 3))
        Dim v3 = get_vec3_from_string(da(idx + 4))
        Dim v4 = get_vec3_from_string(da(idx + 5))
        ReDim mat.mat(15)
        mat.mat(0) = v1.x
        mat.mat(1) = v1.y
        mat.mat(2) = v1.z
        mat.mat(3) = 0.0

        mat.mat(4) = v2.x
        mat.mat(5) = v2.y
        mat.mat(6) = v2.z
        mat.mat(7) = 0.0

        mat.mat(8) = v3.x
        mat.mat(9) = v3.y
        mat.mat(10) = -v3.z
        mat.mat(11) = 0.0

        mat.mat(12) = v4.x
        mat.mat(13) = -v4.y
        mat.mat(14) = -v4.z
        mat.mat(15) = 1.0


        Return mat
    End Function
    Public Sub get_type_and_color(ByRef v_r As v_boneGroups_, ByRef i As Integer)
        'Figures out what type of entry this is
        Dim s = v_r.node_list(i).Substring(0, 2)
        v_r.models(i).displayId = boneMarker
        Select Case s.ToLower
            Case "v_"
                v_r.models(i).type = 0
                v_r.models(i).color = vc4
                v_r.models(i).displayId = boneMarker3
                Return
            Case "wd"
                v_r.models(i).type = 1
                v_r.models(i).color = vc1
                Return
            Case "w_"
                v_r.models(i).type = 2
                v_r.models(i).color = vc2
                Return
            Case "tr"
                v_r.models(i).type = 3
                v_r.models(i).color = vc3
            Case "ta"
                v_r.models(i).type = 5
                v_r.models(i).color = vc5
                v_r.models(i).displayId = boneMarker4
                Return
        End Select
        If Not v_r.isTrack Then
            v_r.models(i).displayId = boneMarker2
            v_r.models(i).type = 4
            v_r.models(i).color = vc0
        End If

    End Sub

    Private Function trim_string(ByRef s As String)
        Dim a = s.Split(">")
        Dim b = a(1).Split("<")
        s = b(0).Trim
        Return s
    End Function

    Private Function get_vec3_from_string(ByRef s As String) As vec3
        Dim ss = trim_string(s)
        Dim a = ss.Split(" ")
        Dim v As New vec3
        v.x = Convert.ToSingle(a(0))
        v.y = Convert.ToSingle(a(1))
        v.z = Convert.ToSingle(a(2))
        Return v
    End Function


End Module
