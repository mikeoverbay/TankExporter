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
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Tao.DevIl
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic
Imports Ionic.Zip
Imports System.Drawing.Imaging

#End Region


Module WOT_Dev_interface
    Public in_tags As String
    Public in_tiers As String
    Public in_shortnames As String
    Public in_nations As String
    Public in_types As String
    'Public in_turrets As String
    Public tank_list(1) As tankitem_
    Public TankDataTable As New DataTable
    Public Structure tankitem_
        Public path As String
        Public tier As String
        Public short_name As String
        Public nation As String
        Public type As String
        'Public turrets As String
    End Structure
    Public Sub get_tank_info_from_temp_folder()

        frmMain.info_Label.Text = "Loading API data from temp folder..."

        in_shortnames = File.ReadAllText(Temp_Storage + "\in_shortnames.txt")
        in_tiers = File.ReadAllText(Temp_Storage + "\in_tiers.txt")
        in_nations = File.ReadAllText(Temp_Storage + "\in_nations.txt")
        in_tags = File.ReadAllText(Temp_Storage + "\in_tags.txt")
        in_types = File.ReadAllText(Temp_Storage + "\in_types.txt")

        build_look_table()

        'frmMain.info_Label.Text = "Building Tank Table..."
        'Application.DoEvents()

        TankDataTable.Columns.Add("tag")
        TankDataTable.Columns.Add("shortname")
        TankDataTable.Columns.Add("tier")
        TankDataTable.Columns.Add("nation")
        TankDataTable.Columns.Add("type")
        For Each q In tank_list
            If q.type IsNot Nothing Then

                Dim r = TankDataTable.NewRow
                r("tag") = q.path
                r("shortname") = q.short_name
                r("tier") = q.tier
                r("nation") = q.nation

                q.type = q.type.Replace("heavyTank", "Heavy")
                q.type = q.type.Replace("lightTank", "Light")
                q.type = q.type.Replace("mediumTank", "Medium")
                q.type = q.type.Replace("AT-SPG", "Destoryer")
                q.type = q.type.Replace("SPG", "Artillary")
                r("type") = q.type
                TankDataTable.Rows.Add(r)
                Dim outs As String = q.nation + ";" + q.tier + ";" + q.path + ";" + q.short_name + ";" + q.type + vbCrLf
                alltanks.Append(outs)
            End If

        Next


    End Sub

    Public Sub get_tank_names()
        '================================================================
        Dim application_id As String = "3e19101959e40173881a2f33e9bbe62c"
        '================================================================


        frmMain.info_Label.Text = "Connecting to WoT Api Server"
        Application.DoEvents()
        Dim client As New WebClient
        Dim tags As String = "https://api.worldoftanks.com/wot/encyclopedia/vehicles/?application_id=" + application_id + "&fields=tag"
        Dim tiers As String = "https://api.worldoftanks.com/wot/encyclopedia/vehicles/?application_id=" + application_id + "&fields=tier"
        Dim shortnames As String = "https://api.worldoftanks.com/wot/encyclopedia/vehicles/?application_id=" + application_id + "&fields=short_name"
        Dim nations As String = "https://api.worldoftanks.com/wot/encyclopedia/vehicles/?application_id=" + application_id + "&fields=nation"
        Dim types As String = "https://api.worldoftanks.com/wot/encyclopedia/vehicles/?application_id=" + application_id + "&fields=type"
        'Dim turrets As String = "https://api.worldoftanks.com/wot/encyclopedia/vehicles/?application_id=" + application_id + "&fields=turrets"
        ' change region if not NA
        Dim r_string = My.Settings.region_selection.ToLower
        If Not My.Settings.region_selection = "NA" Then
            tags = tags.Replace(".com", "." + r_string)
            tiers = tiers.Replace(".com", "." + r_string)
            shortnames = shortnames.Replace(".com", "." + r_string)
            nations = nations.Replace(".com", "." + r_string)
            types = types.Replace(".com", "." + r_string)
        End If
        Dim reader As New StreamReader(client.OpenRead(tags))
        frmMain.info_Label.Text = "Getting Tags..."
        Application.DoEvents()

        in_tags = reader.ReadToEnd
        'save to temp_folder
        File.WriteAllText(Temp_Storage + "\in_tags.txt", in_tags)

        frmMain.info_Label.Text = "Getting tiers..."
        Application.DoEvents()
        reader = New StreamReader(client.OpenRead(tiers))
        in_tiers = reader.ReadToEnd
        'save to temp_folder
        File.WriteAllText(Temp_Storage + "\in_tiers.txt", in_tiers)

        frmMain.info_Label.Text = "Getting shortnames..."
        Application.DoEvents()
        reader = New StreamReader(client.OpenRead(shortnames))
        in_shortnames = reader.ReadToEnd
        'save to temp_folder
        File.WriteAllText(Temp_Storage + "\in_shortnames.txt", in_shortnames)

        frmMain.info_Label.Text = "Getting nations..."
        Application.DoEvents()
        reader = New StreamReader(client.OpenRead(nations))
        in_nations = reader.ReadToEnd
        'save to temp_folder
        File.WriteAllText(Temp_Storage + "\in_nations.txt", in_nations)

        frmMain.info_Label.Text = "Getting Types..."
        Application.DoEvents()
        reader = New StreamReader(client.OpenRead(types))
        in_types = reader.ReadToEnd
        'save to temp_folder
        File.WriteAllText(Temp_Storage + "\in_types.txt", in_types)

        'frmMain.info_Label.Text = "Getting Turrets..."
        'Application.DoEvents()
        'reader = New StreamReader(client.OpenRead(turrets))
        'in_turrets = reader.ReadToEnd
        ''save to temp_folder
        'File.WriteAllText(Temp_Storage + "\in_turrets.txt", in_types)

        reader.Dispose()


        build_look_table()

        'frmMain.info_Label.Text = "Building Tank Table..."
        'Application.DoEvents()
        TankDataTable = New DataTable
        TankDataTable.Columns.Add("tag")
        TankDataTable.Columns.Add("shortname")
        TankDataTable.Columns.Add("tier")
        TankDataTable.Columns.Add("nation")
        TankDataTable.Columns.Add("type")
        'TankDataTable.Columns.Add("turrets")
        For Each q In tank_list
            If q.type IsNot Nothing Then
                Dim r = TankDataTable.NewRow
                r("tag") = q.path
                r("shortname") = q.short_name
                r("tier") = q.tier
                r("nation") = q.nation
                q.type = q.type.Replace("heavyTank", "Heavy")
                q.type = q.type.Replace("lightTank", "Light")
                q.type = q.type.Replace("mediumTank", "Medium")
                q.type = q.type.Replace("AT-SPG", "Destoryer")
                q.type = q.type.Replace("SPG", "Artillary")
                r("type") = q.type
                TankDataTable.Rows.Add(r)
                Dim outs As String = q.nation + ";" + q.tier + ";" + q.path + ";" + q.short_name + ";" + q.type + vbCrLf
                alltanks.Append(outs)
            End If
        Next

    End Sub

    Public Sub build_look_table()
        Dim tags() As String
        Dim tiers() As String
        Dim shortnames() As String
        Dim nations() As String
        Dim types() As String
        'Dim turrets() As String
        tags = clean_file(in_tags)
        tiers = clean_file(in_tiers)
        shortnames = clean_file(in_shortnames)
        nations = clean_file(in_nations)
        types = clean_file(in_types)
        'turrets = clean_file(in_turrets)
        ReDim tank_list(tags.Length)
        For i = 0 To tags.Length - 1
            tank_list(i) = New tankitem_
            tank_list(i).path = tags(i)
            tank_list(i).tier = tiers(i)
            tank_list(i).short_name = shortnames(i)
            tank_list(i).nation = nations(i)
            tank_list(i).type = types(i)
            'tank_list(i).turrets = turrets(i)
        Next
    End Sub
    Public Function clean_file(ByRef st As String) As Array
        Dim ts As String = st.Replace("data" + """" + ":", "!")
        ts = ts.Replace("8,8", "8.8") ' causes issues with spitting the data
        ts = ts.Replace("}}", "")
        Dim ar = ts.Split("!")
        ts = ar(1)
        ar = ts.Split(",")
        Dim cnt As Integer = 0
        For Each a In ar
            Dim sa = a.Split(":")
            Dim s = sa(2).Replace("}", "")
            s = s.Replace("""", "")
            ar(cnt) = s
            cnt += 1
        Next
        Return ar
    End Function

End Module
