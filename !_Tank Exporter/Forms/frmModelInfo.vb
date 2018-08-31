Imports System.IO

Public Class frmModelInfo
    Const EM_SETTABSTOPS As Integer = &HCB
    Declare Function SendMessageA Lib "user32" (ByVal TBHandle As IntPtr, _
                                               ByVal EM_SETTABSTOPS As Integer, _
                                               ByVal wParam As Integer, _
                                               ByRef lParam As Integer) As Boolean



    Private Sub frmModelInfo_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim TabStop As Integer = 32 'Tab times 4
        Try
            SendMessageA(infotb.Handle, EM_SETTABSTOPS, 1, TabStop)

        Catch ex As Exception

        End Try
        infotb.WordWrap = False
        Dim strings(50) As String
        Dim n As Integer = 0
        If FBX_LOADED Then
            strings(n) = pad_string("FBX Models") + pad_string("------------------") + pad_string("------------------") + pad_string2("------------") + pad_string2("-------------")
            n += 1
            For i = 1 To fbxgrp.Length - 1
                Dim name = fbxgrp(i).name.Split(":")
                strings(n) = pad_string(Path.GetFileNameWithoutExtension(name(0)))
                strings(n) += pad_string("nVertices: " + fbxgrp(i).nVertices_.ToString)
                strings(n) += pad_string("nPrmitives: " + fbxgrp(i).nPrimitives_.ToString)
                strings(n) += pad_string2("Has UV2: " + fbxgrp(i).has_uv2.ToString)
                strings(n) += pad_string2("Has Colour: " + fbxgrp(i).has_color.ToString)
                n += 1
            Next
            strings(n) = ""
            n += 1
        End If

        If MODEL_LOADED Then
            strings(n) += pad_string("Packaged Models") + pad_string("------------------") + pad_string("------------------") + pad_string2("------------") + pad_string2("-------------")
            n += 1
            For i = 1 To object_count
                Dim name = _group(i).name.Split(":")
                strings(n) = pad_string(Path.GetFileNameWithoutExtension(name(0)))
                strings(n) += pad_string("nVertices: " + _group(i).nVertices_.ToString)
                strings(n) += pad_string("nPrmitives: " + _group(i).nPrimitives_.ToString)
                strings(n) += pad_string2("Has UV2: " + _group(i).has_uv2.ToString)
                strings(n) += pad_string2("Has Colour: " + _group(i).has_color.ToString)
                n += 1
            Next
        End If
        infotb.Text = ""
        For i = 0 To 30
            If strings(i) IsNot Nothing Then
                infotb.Text += strings(i) + vbCrLf
            End If
        Next
        infotb.Select(0, 0)

        Application.DoEvents()
    End Sub
    Private Function pad_string(ByVal s As String) As String
        Return s.PadRight(18, " ")
    End Function
    Private Function pad_string2(ByVal s As String) As String
        Return s.PadRight(12, " ")
    End Function
End Class