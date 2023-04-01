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
    <Extension()>
    Public Function Split(ByVal input As String,
                          ByVal ParamArray delimiter As String()) As String()
        Return input.Split(delimiter, StringSplitOptions.None)
        Dim a(0) As String
        a(0) = input
        Return a
    End Function


    Public visualXML_str As String
    Public myPath As String


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
    Public Function get_actual_primitive_name() As String
        Try
            Dim s = TheXML_String.Replace("  ", "")
            Dim delim As String = "<primitivesName>"
            Dim s_sections = s.Split(delim)
            delim = "</primitivesName>"
            Dim s_2 = s_sections(1).Split(delim)
            Return s_2(0)
        Catch ex As Exception
            Return ""

        End Try

    End Function
    Public Sub praseVisualXml()
        'define our colors




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
