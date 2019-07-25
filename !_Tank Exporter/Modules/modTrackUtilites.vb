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


Module modTrackUtilites
    Public Function find_and_tag_if_missing(ByVal i As Integer, ByVal j As Integer) As Boolean
        Dim s = v_boneGroups(i).node_list(j)
        Dim kcnt = fbx_boneGroups(i).nodeCnt
        fbx_boneGroups(i).isTrack = v_boneGroups(i).isTrack
        For k = 0 To kcnt - 1
            If fbx_boneGroups(i).node_list(k) Is Nothing Then
                fbx_boneGroups(i).node_list(k) = "V_BlendBone"
            End If
            Dim ss = fbx_boneGroups(i).node_list(k)

            If ss.ToLower = s.ToLower Then
                Return True
            End If
        Next
        'if false, there is a new entry name in the list
        'Debug.WriteLine(s + " " + i.ToString)
        Return False


    End Function

End Module
