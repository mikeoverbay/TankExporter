#Region "imports"
Imports System.IO
Imports System.String
Imports System.Text
Imports FastColoredTextBoxNS
Imports System.Linq
Imports System.Diagnostics
Imports System.Drawing
Imports System.Collections.Generic
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text.RegularExpressions
Imports System.Drawing.Drawing2D
#End Region
Public Class frmEditFrag
#Region "variables"

    Private f_app_path As String
    Private v_app_path As String
    Private g_app_path As String
    Private shader_index As Integer
    Private Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal _
 hwnd As Long, ByVal wMsg As Long, ByVal wParam As Long, _
 lParam As Object) As Long
    Const EM_SETTABSTOPS = &HCB
    Private focused_form As New Control
    Dim TealStyle As TextStyle = New TextStyle(Brushes.LightBlue, Nothing, FontStyle.Regular)
    Dim BoldStyle As TextStyle = New TextStyle(Nothing, Nothing, FontStyle.Bold Or FontStyle.Underline)
    Dim GrayStyle As TextStyle = New TextStyle(Brushes.Gray, Nothing, FontStyle.Regular)
    Dim PowderBlueStyle As TextStyle = New TextStyle(Brushes.PowderBlue, Nothing, FontStyle.Regular)
    Dim GreenStyle As TextStyle = New TextStyle(Brushes.Green, Nothing, FontStyle.Italic)
    Dim BrownStyle As TextStyle = New TextStyle(Brushes.Brown, Nothing, FontStyle.Italic)
    Dim MaroonStyle As TextStyle = New TextStyle(Brushes.Maroon, Nothing, FontStyle.Regular)
    Dim GLSLstyle As TextStyle = New TextStyle(Brushes.LightGreen, Nothing, FontStyle.Regular)

    Dim SameWordsStyle As MarkerStyle = New MarkerStyle(New SolidBrush(Color.FromArgb(40, Color.Gray)))
#End Region

    Private Sub frmEditFrag_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        'If MsgBox("Save Shader?", MsgBoxStyle.YesNo, "Save?") = MsgBoxResult.Yes Then
        '	File.WriteAllText(v_app_path, vert_tb.Text)
        'End If
    End Sub

    Private Sub frmEditFrag_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TabControl1.Width = Me.ClientSize.Width
        TabControl1.Height = Me.ClientSize.Height - CB1.Height - 5
        recompile_bt.Location = New Point(recompile_bt.Location.X, TabControl1.Height + 3)
        search_btn.Location = New Point(search_btn.Location.X, TabControl1.Height + 3)


        vert_tb.AcceptsTab = True
        geo_tb.AcceptsTab = True
        frag_tb.AcceptsTab = True

        For i = 0 To shaders.shader.Length - 1
            CB1.Items.Add(shaders.shader(i).shader_name)
        Next

        recompile_bt.Enabled = False
        Me.Text = "Shader Editor:"
    End Sub

    Private Sub recompile_bt_Click(sender As Object, e As EventArgs) Handles recompile_bt.Click
        recompile_bt.Enabled = False
        File.WriteAllText(v_app_path, vert_tb.Text)
        File.WriteAllText(f_app_path, frag_tb.Text)
        If shaders.shader(shader_index).has_geo Then
            File.WriteAllText(g_app_path, geo_tb.Text)
        End If

        gl_busy = True  'disable rendering

        Dim fs As String
        Dim vs As String
        Dim gs As String

        With shaders.shader(shader_index)
            vs = .vertex
            fs = .fragment
            gs = .geo
            Dim id = assemble_shader(vs, gs, fs, .shader_id, .shader_name, .has_geo)
            .set_call_id(id)
            .shader_id = id
        End With
        set_shader_variables() ' update uniform addresses
        gl_busy = False
        reset_focus()
        recompile_bt.Enabled = True

    End Sub

    Private Sub CB1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CB1.SelectedIndexChanged
        Dim shader As String = CB1.Items(CB1.SelectedIndex)
        Me.Text = "Shader Editor: " + shader
        shader_index = CB1.SelectedIndex
        f_app_path = shaders.shader(shader_index).fragment
        v_app_path = shaders.shader(shader_index).vertex
        g_app_path = shaders.shader(shader_index).geo

        vert_tb.Text = File.ReadAllText(v_app_path)
        frag_tb.Text = File.ReadAllText(f_app_path)
        If shaders.shader(shader_index).has_geo Then
            geo_tb.Enabled = True
            geo_tb.Text = File.ReadAllText(g_app_path)
        Else
            geo_tb.Text = "NO GEO PROGRAM"
            geo_tb.Enabled = False
        End If
        recompile_bt.Enabled = True
    End Sub


    Private Sub reset_focus()
        If focused_form IsNot Nothing Then
            focused_form.Focus()
        End If
    End Sub


    Private Sub CSharpSyntaxHighlight(ByRef sender As FastColoredTextBox, e As TextChangedEventArgs)
        e.ChangedRange.SetFoldingMarkers("", "")
        sender.LeftBracket = "("c
        sender.RightBracket = ")"c
        sender.LeftBracket2 = ControlChars.NullChar
        sender.RightBracket2 = ControlChars.NullChar
        'clear style of changed range
        e.ChangedRange.ClearStyle(TealStyle, BoldStyle, GrayStyle, PowderBlueStyle, GreenStyle, BrownStyle)

        'string highlighting
        e.ChangedRange.SetStyle(BrownStyle, """""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'")
        'comment highlighting
        e.ChangedRange.SetStyle(GreenStyle, "//.*$", RegexOptions.Multiline)
        e.ChangedRange.SetStyle(GreenStyle, "(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline)
        e.ChangedRange.SetStyle(GreenStyle, "(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline Or RegexOptions.RightToLeft)
        'number highlighting
        e.ChangedRange.SetStyle(PowderBlueStyle, "\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b")
        'attribute highlighting
        e.ChangedRange.SetStyle(GrayStyle, "^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline)
        'class name highlighting
        e.ChangedRange.SetStyle(BoldStyle, "\b(class|struct|enum|interface)\s+(?<range>\w+?)\b")
        'keyword highlighting
        e.ChangedRange.SetStyle(TealStyle, "\b(mat3|mat4|vec2|vec3|vec4|abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b")
        'GLSL keyword highlighting
        e.ChangedRange.SetStyle(GLSLstyle, "\b(gl_FrontColor|uniform|varying|attribute|gl_Vertex|gl_NormalMatrix|gl_ModelViewMatrix|" _
                                        + "gl_ModelViewProjectionMatrix|gl_Position|ftransform|mix|max|min|dfdx|dfdy|gl_FragColor|" _
                                        + "gl_MultiTexCoord0|gl_MultiTexCoord1|gl_MultiTexCoord2|gl_MultiTexCoord3|gl_MultiTexCoord4|" _
                                        + "reflact|fract|smoothstep|step|normalize|dot|cross|gl_Normal|pow|gl_LightSource|" _
                                        + "gl_FrontMaterial|clamp|reflect|gl_Fog|gl_FragCoord|discard\b)")

        'clear folding markers
        e.ChangedRange.ClearFoldingMarkers()

        'set folding markers
        e.ChangedRange.SetFoldingMarkers("{", "}")
        'allow to collapse brackets block
        e.ChangedRange.SetFoldingMarkers("#region\b", "#endregion\b")
        'allow to collapse #region blocks
        e.ChangedRange.SetFoldingMarkers("/\*", "\*/")
        'allow to collapse comment block


    End Sub

    Private Sub search_btn_Click(sender As Object, e As EventArgs) Handles search_btn.Click
        Dim s As String = ""
        Dim tab = TabControl1.SelectedIndex
        Select Case tab
            Case 0
                If vert_tb.SelectedText.Length > 0 Then
                    s = vert_tb.SelectedText
                End If
            Case 1
                If frag_tb.SelectedText.Length > 0 Then
                    s = frag_tb.SelectedText.ToString
                End If
            Case 2
                If geo_tb.SelectedText.Length > 0 Then
                    s = geo_tb.SelectedText.ToString
                End If
        End Select

        If s.Length = 0 Then Return
        'www.opengl.org%2Fsdk%2Fdocs%2Fman%2Fhtml%2Fclamp.xhtml
        Dim s2 As String = "https://www.google.com/?gws_rd=ssl#q=" + s

        System.Diagnostics.Process.Start(s2)
        reset_focus()
    End Sub

    Private Sub vert_tb_GotFocus(sender As Object, e As EventArgs) Handles vert_tb.GotFocus
        focused_form = vert_tb
    End Sub


    Private Sub vert_tb_TextChanged(sender As Object, e As TextChangedEventArgs) Handles vert_tb.TextChanged
        CSharpSyntaxHighlight(vert_tb, e) 'custom highlighting
    End Sub

    Private Sub frag_tb_GotFocus(sender As Object, e As EventArgs) Handles frag_tb.GotFocus
        focused_form = frag_tb
    End Sub

    Private Sub frag_tb_TextChanged(sender As Object, e As TextChangedEventArgs) Handles frag_tb.TextChanged
        CSharpSyntaxHighlight(frag_tb, e) 'custom highlighting
    End Sub

    Private Sub geo_tb_GotFocus(sender As Object, e As EventArgs) Handles geo_tb.GotFocus
        focused_form = geo_tb
    End Sub

    Private Sub geo_tb_TextChanged(sender As Object, e As TextChangedEventArgs) Handles geo_tb.TextChanged
        CSharpSyntaxHighlight(geo_tb, e) 'custom highlighting
    End Sub
End Class