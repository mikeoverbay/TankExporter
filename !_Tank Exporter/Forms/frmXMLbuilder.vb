﻿Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Threading
Imports Ionic.Zip

Public Class frmXMLbuilder
    Public thelist(1) As list_

    Public Structure list_
        Public entry_length As Integer
        Public file_name As String
        Public pkg_name As String
        Public maxLod As String
    End Structure

    Public game_path As String
    Public list_size As Integer
    Public pkg_cnt As Integer
    Public total_found As Integer
    Public duplcate_count As Integer
    Public temp_path As String
    Public used_pkg_cnt As Integer
    Public DATA_TABLE As New DataTable("items")

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles go_btn.Click

        Dim pkgFolder As String = My.Settings.game_path + "\res\packages\"
        Dim st As New Stopwatch
        st.Restart()
        total_found = 0
        duplcate_count = 0
        used_pkg_cnt = 0

        go_btn.Enabled = False '<-- dont let a reenter occur

        Dim iPath = My.Settings.game_path
        Dim f_info = Directory.GetFiles(pkgFolder)

        Dim PKGS(250) As String
        Dim p_cnt As Integer = 0
        Dim f_cnt As Integer = 0
        Dim cnt As Integer = 0
        list_size = 0
        ReDim Preserve thelist(300000)
        For i = 0 To thelist.Length - 1
            thelist(i) = New list_
            'ReDim thelist(i).file_name(200000)
        Next
        'get all the packages
        For Each m In f_info
            If m.Contains(".pkg") And Not StartsWithDigit(m) Then ' remove the last part to search maps
                PKGS(cnt) = m
                cnt += 1
            End If
        Next
        Dim searchPatterns As String() = {
    "vehicles/american/", "vehicles/british/", "vehicles/chinese/", "vehicles/czech/",
    "vehicles/french/", "vehicles/german/", "vehicles/itialy/", "vehicles/japan/",
    "vehicles/poland/", "vehicles/russian/", "vehicles/sweden/"}

        'Dim searchPatterns As String() = {"gui/maps/icons/vehicle/"}

        For i = 0 To cnt - 1
            pkg_tb.Text = PKGS(i)
            Application.DoEvents()
            Using z As New Ionic.Zip.ZipFile(PKGS(i))
                For k = 0 To searchPatterns.Length - 1

                    For Each item In z
                        If Not item.IsDirectory Then

                            Dim fileName As String = item.FileName
                            If fileName.Contains(searchPatterns(k)) _
                            And Not ((fileName.Contains("vehicle/420x307/") OrElse fileName.Contains("vehicle/contour/") _
                            OrElse fileName.Contains("vehicle/small/") OrElse fileName.Contains("lod1") _
                            OrElse fileName.Contains("lod2")) OrElse fileName.Contains("lod3") _
                            OrElse fileName.Contains("lod3") OrElse fileName.Contains("lod4") _
                            OrElse fileName.Contains(".anim_") _
                            OrElse fileName.Contains(".vt") OrElse fileName.Contains(".prefab") _
                            OrElse fileName.Contains("lod5") OrElse fileName.Contains("lod6")) Then
                                seach_add(item, PKGS(i))
                                pkg_cnt += 1
                            End If
                        End If
                    Next
                Next
            End Using
            Application.DoEvents()
            GC.Collect()
            GC.WaitForFullGCComplete()
        Next

        'For cp = 0 To pkg_cnt - 1
        '    ReDim Preserve thelist(cp).file_name(thelist(cp).entry_length - 1)
        'Next
        Dim seconds = st.ElapsedMilliseconds / 1000
        unique.Text = "I found " & total_found.ToString() & " In " & seconds.ToString() & " seconds."

        'We have the list so lets create the XML file
        Dim fb As New StringBuilder
        fb.AppendLine("<?xml version=""1.0"" standalone=""yes""?>")
        fb.AppendLine("<FileList>")
        For i = 0 To pkg_cnt
            fb.AppendLine("<items>")
            fb.AppendLine("<filename>" + thelist(i).file_name + "</filename>")
            fb.AppendLine("<package>" + thelist(i).pkg_name + "</package>")
            fb.AppendLine("</items>")
        Next
        fb.AppendLine("</FileList>")
        IO.File.WriteAllText(Application.StartupPath + "\resources\XMLdata\TheItemList.xml", fb.ToString)
        'File.WriteAllText("C:\!_Tank Exporter\!_Tank Exporter\resources\XMLdata\TheItemListIcons.xml", fb.ToString)
        'Process.Start(temp_path + "TheItemList.xml")
        go_btn.Enabled = True
skip_searching:
        DATA_TABLE.Clear()
        DATA_TABLE.Columns.Add("filename", GetType(String))
        DATA_TABLE.Columns.Add("package", GetType(String))
        DATA_TABLE.ReadXml(Application.StartupPath + "\resources\XMLdata\TheItemList.xml")
        go_btn.Enabled = True
    End Sub

    Private Sub seach_add(ByRef item As ZipEntry, ByRef pkg_name As String)
        'Check if this item has already been added to our list. Return if found.
        'Check if the pkg_name is even in our list.. If not, add it!
        'removing local path of the pkg_names .. The XML file is too big!!
        pkg_name = Path.GetFileName(pkg_name)
        Dim fname = item.FileName.Replace("\", "/")
        Dim cp As Integer = 0

        'The pgk_name is not on the list so add it.
        thelist(pkg_cnt).pkg_name = pkg_name
        'its a newly added list so it has no file_names. Go ahead and add this one.
        thelist(pkg_cnt).file_name = fname
        thelist(pkg_cnt).entry_length += 1

        pkg_tb.Text = pkg_name
        file_tb.Text = fname
        total_found += 1
        If cp > used_pkg_cnt Then used_pkg_cnt = cp
        Application.DoEvents() 'so we don't lock up this app
    End Sub


    Function StartsWithDigit(ByVal filename As String) As Boolean
        If String.IsNullOrEmpty(filename) Then
            Return False
        End If

        Dim firstChar As Char = Path.GetFileName(filename)(0)
        Return Char.IsDigit(firstChar)
    End Function

    Private Sub frmXMLbuilder_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim nonInvariantCulture As CultureInfo = New CultureInfo("en-US")
        nonInvariantCulture.NumberFormat.NumberDecimalSeparator = "."
        Thread.CurrentThread.CurrentCulture = nonInvariantCulture

    End Sub
End Class