Imports System
Imports System.Math
Imports System.Collections.Generic
Imports System.Collections
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Xml
Imports Microsoft.VisualBasic
Module vis_main
    Public xmldataset As New DataSet
    Public xml_name As String
    Public PackedFileName As String = ""
    Public ReadOnly sver As String = "0.5"
    Public ReadOnly stitle As String = "WoT Mod Tools "
    Public PS As New Packed_Section()
    Public PF As New Primitive_File()
    Public xDoc As New XmlDocument

    Public ReadOnly Binary_Header As Int32 = &H42A14E65



    Private Function FormatXml(ByVal sUnformattedXml As String) As String
        'load unformatted xml into a dom
        Dim ts As String = sUnformattedXml.Replace("><", ">" + vbCrLf + "<")
        sUnformattedXml = ts
        Dim xd As New XmlDocument()
        xd.LoadXml(sUnformattedXml)

        'will hold formatted xml

        Dim sb As New StringBuilder()

        'pumps the formatted xml into the StringBuilder above

        Dim sw As New StringWriter(sb)

        'does the formatting

        Dim xtw As XmlTextWriter = Nothing

        Try
            'point the xtw at the StringWriter

            xtw = New XmlTextWriter(sw)

            'we want the output formatted

            xtw.Formatting = Formatting.Indented

            'get the dom to dump its contents into the xtw 

            xd.WriteTo(xtw)
        Catch
        Finally
            'clean up even if error

            If xtw IsNot Nothing Then
                xtw.Close()
            End If
        End Try

        'return the formatted xml
        'sb.Replace("><", ">" + vbCrLf + "<")
        'Dim rder As TextReader = New StringReader(sb.ToString)
        'Dim xxd As XDocument = XDocument.Load(rder)

        Return sb.ToString()
    End Function

    Public Sub DecodePackedFile(ByVal reader As BinaryReader)
        reader.ReadSByte()
        Dim dictionary As List(Of String) = PS.readDictionary(reader)
        'PackedFileName = PackedFileName.Replace("_back", "")
        Dim xmlroot As XmlNode = xDoc.CreateNode(XmlNodeType.Element, PackedFileName, "")
        xDoc.OuterXml.Replace("><", ">" + vbCrLf + "<")


        PS.readElement(reader, xmlroot, xDoc, dictionary)
        Dim xml_string As String = xmlroot.InnerXml

        Dim fileS As New MemoryStream
        Dim fbw As New BinaryWriter(fileS)
        fileS.Position = 0

        xDoc.AppendChild(xmlroot)
        Dim Id = xmlroot.Name + "/gameplayTypes"
        Dim node As XmlElement = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("assault")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("assault2")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("domination")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("fallout")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("fallout2")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        Id = xmlroot.Name + "/trees"
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            node.ParentNode.RemoveChild(node)
        End If
        Id = xmlroot.Name + "/fallingAtoms"
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            node.ParentNode.RemoveChild(node)
        End If
        Id = xmlroot.Name + "/turret0/armor"
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            node.ParentNode.RemoveChild(node)
        End If

        If xDoc.InnerXml.Contains("customization") Then
            Id = xmlroot.Name + "/inscriptions"
            node = xDoc.SelectSingleNode(Id)
remove_more:
            If node IsNot Nothing Then
                node.ParentNode.RemoveChild(node)
            End If
            node = xDoc.SelectSingleNode(Id)
            If node IsNot Nothing Then
                GoTo remove_more
            End If

        End If
        fileS.Position = 0
        xDoc.Save(fileS)

        '' xDoc.Save(sfd.FileName);
        Dim fbr As New BinaryReader(fileS)
        fileS.Position = 0
        TheXML_String = fbr.ReadChars(fileS.Length)
        TheXML_String = PrettyPrint(TheXML_String)

        Id = xmlroot.Name + "/hull/armor"
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Return
        End If

        fileS.Position = 0
        Try
            xmldataset.ReadXml(fileS)

        Catch ex As Exception
            MsgBox("Please report this bug." + ex.Message, MsgBoxStyle.Exclamation, "packed XML file Error...")
        End Try
        'Dim sw As New StringWriter
        'Dim xtw As New XmlTextWriter(sw)
        'xtw.Formatting = Formatting.Indented
        'xtw.IndentChar = vbTab
        'xtw.Indentation = 1
        'data_set.WriteTo(xtw)
        'xtw.Flush()
        fbr.Close()
        fbw.Close()
        fileS.Close()
        fileS.Dispose()

        'Dim f As String = File.ReadAllText("C:\wot_temp\InnerXml.xml")
        'f = f.Replace(vbCrLf, vbLf)

        'muted .. to slow
        'File.WriteAllText("C:\wot_temp\InnerXml.xml", f)
        'frmMain.WebBrowser1.DocumentStream = TransformXML(f, My.Resources.xml_format)
    End Sub
    Private Function fix_bad_tags(xmlString As String)
        'box all primitive tags.. Dont think there will ever be over 30 :-)
        For i = 0 To 30
            Dim ast = xmlString.Replace("<primitiveGroup>" + i.ToString, "<primitiveGroup>" + ControlChars.CrLf.ToCharArray() + "<PG_ID>" + i.ToString + "</PG_ID>")
            xmlString = ast
        Next
        Return xmlString
    End Function
    Private Function PrettyPrint(XML As [String]) As [String]
        Dim Result As [String] = ""
        XML = fix_bad_tags(XML)
        'another hack to fix WG's bad xml
        XML = XML.Replace("<_", "<G_")
        XML = XML.Replace("</_", "</G_")
        Dim MS As New MemoryStream()

        Dim xmlsettings As New XmlWriterSettings
        xmlsettings.Indent = True
        xmlsettings.NewLineOnAttributes = True
        xmlsettings.Encoding = Encoding.UTF8
        xmlsettings.OmitXmlDeclaration = True
        xmlsettings.CheckCharacters = True
        xmlsettings.CloseOutput = True
        Dim W = XmlWriter.Create(MS, xmlsettings)
        Dim D As New XmlDocument()
        Try
            ' Load the XmlDocument with the XML.
            D.LoadXml(XML)

            ' Write the XML into a formatting XmlTextWriter
            D.WriteContentTo(W)
            W.Flush()
            MS.Flush()

            ' Have to rewind the MemoryStream in order to read
            ' its contents.
            MS.Position = 0

            ' Read MemoryStream contents into a StreamReader.
            Dim SR As New StreamReader(MS)

            ' Extract the text from the StreamReader.
            Dim FormattedXML As [String] = SR.ReadToEnd()
            'MS.Close()
            'W.Close()

            Result = FormattedXML.Replace("<G_", "<_")
            Result = Result.Replace("</G_", "</_")

        Catch generatedExceptionName As XmlException
        End Try


        Return Result
    End Function

    Public Sub DecodePackedFile_2(ByVal reader As BinaryReader)
        reader.ReadSByte()
        Dim dictionary As List(Of String) = PS.readDictionary(reader)
        'PackedFileName = PackedFileName.Replace("_back", "")
        Dim xmlroot As XmlNode = xDoc.CreateNode(XmlNodeType.Element, PackedFileName, "")
        xDoc.OuterXml.Replace("><", ">" + vbCrLf + "<")


        PS.readElement(reader, xmlroot, xDoc, dictionary)
        Dim xml_string As String = xmlroot.InnerXml

        Dim fileS As New MemoryStream
        Dim fbw As New BinaryWriter(fileS)
        fileS.Position = 0

        xDoc.AppendChild(xmlroot)
        Dim Id = xmlroot.Name + "/gameplayTypes"
        Dim node As XmlElement = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("assault")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("assault2")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("domination")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("fallout")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Dim n2 = node.SelectSingleNode("fallout2")
            If n2 IsNot Nothing Then
                node.RemoveChild(n2)
            End If
        End If
        Id = xmlroot.Name + "/trees"
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            node.ParentNode.RemoveChild(node)
        End If
        Id = xmlroot.Name + "/fallingAtoms"
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            node.ParentNode.RemoveChild(node)
        End If
        Id = xmlroot.Name + "/turret0/armor"
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            node.ParentNode.RemoveChild(node)
        End If

        If xDoc.InnerXml.Contains("customization") Then
            Id = xmlroot.Name + "/inscriptions"
            node = xDoc.SelectSingleNode(Id)
remove_more:
            If node IsNot Nothing Then
                node.ParentNode.RemoveChild(node)
            End If
            node = xDoc.SelectSingleNode(Id)
            If node IsNot Nothing Then
                GoTo remove_more
            End If

        End If
        fileS.Position = 0
        xDoc.Save(fileS)

        '' xDoc.Save(sfd.FileName);
        Dim fbr As New BinaryReader(fileS)
        fileS.Position = 0
        TheXML_String = fbr.ReadChars(fileS.Length)

        Id = xmlroot.Name + "/hull/armor"
        node = xDoc.SelectSingleNode(Id)
        If node IsNot Nothing Then
            Return
        End If

        Dim da_arry = TheXML_String.Split(New String() {"<primitiveGroup>0<material>"}, StringSplitOptions.None)
        Dim ts As String = ""
        If da_arry.Length > 2 Then
            For i = 0 To da_arry.Length - 2
                ts += da_arry(i) + "<primitiveGroup>" + i.ToString + "<material>"
            Next
            TheXML_String = ts + da_arry(da_arry.Length - 1)
        End If
        fileS.Position = 0
        'Try
        '    xmldataset.ReadXml(fileS)

        'Catch ex As Exception
        '    MsgBox("Please report this bug." + ex.Message, MsgBoxStyle.Exclamation, "packed XML file Error...")
        'End Try
        'Dim sw As New StringWriter
        'Dim xtw As New XmlTextWriter(sw)
        'xtw.Formatting = Formatting.Indented
        'xtw.IndentChar = vbTab
        'xtw.Indentation = 1
        'data_set.WriteTo(xtw)
        'xtw.Flush()
        fbr.Close()
        fbw.Close()
        fileS.Close()
        fileS.Dispose()

        'Dim f As String = File.ReadAllText("C:\wot_temp\InnerXml.xml")
        'f = f.Replace(vbCrLf, vbLf)

        'muted .. to slow
        'File.WriteAllText("C:\wot_temp\InnerXml.xml", f)
        'frmMain.WebBrowser1.DocumentStream = TransformXML(f, My.Resources.xml_format)
    End Sub

    Public Sub ReadPrimitiveFile(ByVal file As String)
        Dim F As New FileStream(file, FileMode.Open, FileAccess.Read)
        Dim reader As New BinaryReader(F)

        Dim ptiComment As XmlComment = xDoc.CreateComment("DO NOT SAVE THIS FILE! THIS CODE IS JUST FOR INFORMATION PUPORSES!")

        Dim xmlprimitives As XmlNode = xDoc.CreateNode(XmlNodeType.Element, "primitives", "")

        PF.ReadPrimitives(reader, xmlprimitives, xDoc)

        xDoc.AppendChild(ptiComment)
        xDoc.AppendChild(xmlprimitives)
        'frmMain.TxTOut.AppendText(FormatXml(xDoc.OuterXml))

        'muted.. to slow
        'IO.File.WriteAllText("C:\wot_temp\xml_temp.xml", FormatXml(xDoc.OuterXml))
        'frmMain.WebBrowser1.DocumentStream = TransformXML(FormatXml(xDoc.OuterXml), My.Resources.xml_format)

    End Sub

    Public TheXML_String As String = ""
    Public Function openXml_stream(ByVal f As MemoryStream, ByVal PackedFileName_in As String) As Boolean
        xDoc = New XmlDocument
        f.Position = 0
        xmldataset.Clear()
        While xmldataset.Tables.Count > 0
            xmldataset.Reset()
        End While
        PackedFileName = "map_" & PackedFileName_in.ToLower()
        Dim reader As New BinaryReader(f)
        Dim head As Int32 = reader.ReadInt32()
        If head = Packed_Section.Packed_Header Then
            DecodePackedFile(reader)
        ElseIf head = Binary_Header Then
        Else
            Return False

        End If
        reader.Close()
        Return True
    End Function
    Public Function openXml_stream_2(ByVal f As MemoryStream, ByVal PackedFileName_in As String) As Boolean
        xDoc = New XmlDocument
        f.Position = 0
        xmldataset.Clear()
        While xmldataset.Tables.Count > 0
            xmldataset.Reset()
        End While
        PackedFileName = "map_" & PackedFileName_in.ToLower()
        Dim reader As New BinaryReader(f)
        Dim head As Int32 = reader.ReadInt32()
        If head = Packed_Section.Packed_Header Then
            DecodePackedFile_2(reader)
        ElseIf head = Binary_Header Then
        Else
            If Not PackedFileName.Contains(".xml") Then
                PackedFileName &= ".xml"
            End If
        End If
        reader.Close()
        Return True
    End Function

    'Public Function this_is_uncrushed(ByRef modID As UInt32, ByRef peice As Integer) As Boolean
    '    'redoing this.. old way sucks!!!
    '    Dim primStart As Integer = InStr(TheXML_String, "tiveGroup>" + peice.ToString)
    '    'Dim spec_pos As Integer
    '    'Dim norm_pos As Integer
    '    If primStart = 0 Then
    '        Return False
    '    End If
    '    Dim di_ As Integer = InStr(primStart, TheXML_String, "<identifier>") + "<identifier>".Length
    '    Dim di_end As Integer = InStr(di_, TheXML_String, "</identifier>")
    '    Dim material As String = Mid(TheXML_String, di_, di_end - di_)
    '    Dim fn = Models.Model_list(modID).ToLower
    '    'If InStr(fn, "env205_Boats02") > 0 Then
    '    '    Stop
    '    'End If
    '    fn = fn.Replace("lod1", "lod0")
    '    fn = fn.Replace("lod2", "lod0")
    '    fn = fn.Replace("lod3", "lod0")
    '    If fn.Contains("mle040_") And peice = 1 Then
    '        Return False
    '    End If
    '    If fn.Contains("mle008_") And peice = 2 Then
    '        Return False
    '    End If
    '    If fn.Contains("env053_") And peice = 0 Then
    '        Return False
    '    End If
    '    'If fn.Contains("bldAM_008") Then
    '    '    Stop
    '    'End If
    '    'If material.Contains("n_stone") Then
    '    '    Return False
    '    'End If
    '    'If material.Contains("n_wood") Then
    '    '    Return False
    '    'End If
    '    'If material.Contains("n_metal") Then
    '    '    Return False
    '    'End If
    '    'If material.Contains("partN_") Then
    '    '    Return False
    '    'End If

    '    If material.Contains("s_wall") Then
    '        Return True
    '    End If
    '    If material.Contains("s_ramp") Then
    '        Return True
    '    End If
    '    If material.Contains("s_n") Then
    '        Return False
    '    End If
    '    If material.Contains("d_wo") Then
    '        Return True
    '    End If
    '    If material.Contains("d_me") Then
    '        Return True
    '    End If
    '    If material.Contains("d_sto") Then
    '        Return True
    '    End If
    '    fn = fn.Replace("_processed", "")
    '    If dest_buildings.filename.Contains(fn) Then
    '        Dim indx = dest_buildings.filename.IndexOf(fn)
    '        If material.ToLower.Contains(dest_buildings.matName(indx)) Then
    '            Return False
    '        End If
    '        Return True
    '    End If
    '    'Dim outs As String = ""
    '    'For k = 0 To dest_buildings.filename.Count - 1
    '    '    outs += dest_buildings.filename(k)
    '    '    outs += " mat: "
    '    '    outs += dest_buildings.matName(k) + vbCrLf
    '    'Next
    '    'File.WriteAllText("C:\dest_buildings.txt", outs)
    '    Return False
    'End Function


    '    Public Function get_textures_and_names(ByVal mod_id As Integer, ByVal currentP As Integer, ByVal primNum As Integer, ByRef has_uv2 As Boolean) As Boolean
    '        'the old method sucks so.. im redoing it!
    '        'If map = 63 And mod_id = 18 Then
    '        '	Stop
    '        'End If
    '        Models.models(mod_id).componets(currentP).alpha_only = False
    '        ' Models.models(mod_id).componets(currentP).multi_textured = False
    '        Dim primStart As Integer = InStr(TheXML_String, "primitiveGroup>" & primNum.ToString)
    '        Dim primStart2 As Integer = InStr(primStart + 5, TheXML_String, "primitiveGroup>")
    '        If primStart2 = 0 Then primStart2 = TheXML_String.Length
    '        Dim diff_pos As Integer
    '        'Dim spec_pos As Integer
    '        'Dim norm_pos As Integer
    '        If primStart = 0 Then primStart += 1
    '        diff_pos = InStr(primStart, TheXML_String, "diffuseMap<")
    '        If diff_pos = 0 Then
    '            'No diffuseMap name was found. This means this primitiveGroup
    '            'is probable something we dont want or need.. :)
    '            Return False
    '        End If
    '        If diff_pos > 0 Then
    '            If diff_pos > primStart2 Then
    '                'diff_pos has locked on to the next primitiveGroups diffuseMap
    '                'because this group has NONE.. this means its a collision box
    '                'and we dont want to waste our time on these... :)
    '                GoTo clouds

    '            End If
    '            Dim tex1_pos = InStr(diff_pos, TheXML_String, "<Texture>") + "<texture>".Length
    '            Dim tex1_Epos = InStr(tex1_pos, TheXML_String, "</Texture>")
    '            Dim newS As String = ""
    '            newS = Mid(TheXML_String, tex1_pos, tex1_Epos - tex1_pos)
    '            Models.models(mod_id).componets(currentP).color_name = newS
    '            'Debug.Write(newS & vbCrLf)
    '            Models.models(mod_id).componets(currentP).color_id = -1
    '            If newS.Contains("clouds.dds") Then
    '                Return True
    '            End If
    '        End If
    '        '----------------------------------------------------------------------------------------
    'clouds:
    '        diff_pos = InStr(primStart, TheXML_String, "clouds<")
    '        If diff_pos > 0 Then
    '            If diff_pos > primStart2 Then
    '                'diff_pos has locked on to the next primitiveGroups diffuseMap
    '                'because this group has NONE.. this means its a collision box
    '                'and we dont want to waste our time on these... :)
    '                Return False
    '            End If
    '            Dim tex1_pos = InStr(diff_pos, TheXML_String, "<Texture>") + "<texture>".Length
    '            Dim tex1_Epos = InStr(tex1_pos, TheXML_String, "</Texture>")
    '            Dim newS As String = ""
    '            newS = Mid(TheXML_String, tex1_pos, tex1_Epos - tex1_pos)
    '            Models.models(mod_id).componets(currentP).color_name = newS
    '            'Debug.Write(newS & vbCrLf)
    '            Models.models(mod_id).componets(currentP).color_id = -1
    '            Return True
    '        End If
    '        If Models.models(mod_id).componets(currentP).multi_textured Then
    '            Models.models(mod_id).componets(currentP).color2_name = "none"
    '            diff_pos = InStr(primStart, TheXML_String, "diffuseMap2<")
    '            If diff_pos > primStart2 Then
    '                'diff_pos has locked on to the next primitiveGroups diffuseMap
    '                'because this group has NONE.. this means its a collision box
    '                'and we dont want to waste our time on these... :)
    '                Models.models(mod_id).componets(currentP).multi_textured = False
    '                'Return True

    '            End If
    '            If diff_pos > 0 Then
    '                Dim tex1_pos = InStr(diff_pos, TheXML_String, "<Texture>") + "<texture>".Length
    '                Dim tex1_Epos = InStr(tex1_pos, TheXML_String, "</Texture>")
    '                Dim newS As String = ""
    '                newS = Mid(TheXML_String, tex1_pos, tex1_Epos - tex1_pos)
    '                Models.models(mod_id).componets(currentP).color2_name = newS
    '                Models.models(mod_id).componets(currentP).color2_Id = -1
    '                'Debug.Write(newS & vbCrLf)
    '            Else
    '                has_uv2 = False
    '                Models.models(mod_id).componets(currentP).multi_textured = False
    '            End If
    '        End If
    '        'saving this shit incase we want to use it to find the bump-normals later..
    '        'Not likely.. it will slow the rendering to a crawl!
    '        Models.models(mod_id).componets(currentP).bumped = False    ' this stops loading NormalMaps

    '        ' if we dont want bump mapped models.. lets not waste time loading them!!!
    '        model_bump_loaded = True
    '        diff_pos = InStr(primStart, TheXML_String, "normalMap<")
    '        If diff_pos > 0 Then
    '            If diff_pos > primStart2 Then
    '                'diff_pos has locked on to the next primitiveGroups diffuseMap
    '                'because this group has NONE.. this means its a collision box
    '                'and we dont want to waste our time on these... :)
    '                Models.models(mod_id).componets(currentP).bumped = False
    '                Return True
    '            End If
    '            Dim tex1_pos = InStr(diff_pos, TheXML_String, "<Texture>") + "<texture>".Length
    '            Dim tex1_Epos = InStr(tex1_pos, TheXML_String, "</Texture>")
    '            Dim newS As String = ""
    '            newS = Mid(TheXML_String, tex1_pos, tex1_Epos - tex1_pos)
    '            'Dim ar = Models.models(mod_id).componets(currentP).color_name
    '            Models.models(mod_id).componets(currentP).normal_name = newS
    '            'Debug.Write(newS & vbCrLf)
    '            Models.models(mod_id).componets(currentP).normal_Id = -1
    '            Models.models(mod_id).componets(currentP).bumped = True
    '        End If
    '        diff_pos = InStr(primStart, TheXML_String, "alphaReference<")
    '        If diff_pos > 0 Then
    '            If diff_pos > primStart2 Then
    '                'diff_pos has locked on to the next primitiveGroups diffuseMap
    '                'because this group has NONE.. this means its a collision box
    '                'and we dont want to waste our time on these... :)
    '                Models.models(mod_id).componets(currentP).alphaRef = 0
    '                Return True
    '            End If
    '            Dim tex1_pos = InStr(diff_pos, TheXML_String, "<Int>") + "<Int>".Length
    '            Dim tex1_Epos = InStr(tex1_pos, TheXML_String, "</Int>")
    '            Dim newS As String = ""
    '            newS = Mid(TheXML_String, tex1_pos, tex1_Epos - tex1_pos)
    '            'Dim ar = Models.models(mod_id).componets(currentP).color_name
    '            Dim ref = Convert.ToInt32(newS)

    '            Models.models(mod_id).componets(currentP).alphaRef = ref

    '        End If
    '        diff_pos = InStr(primStart, TheXML_String, "alphaTestEnable<")
    '        If diff_pos > 0 Then
    '            If diff_pos > primStart2 Then
    '                'diff_pos has locked on to the next primitiveGroups diffuseMap
    '                'because this group has NONE.. this means its a collision box
    '                'and we dont want to waste our time on these... :)
    '                Models.models(mod_id).componets(currentP).alphaRef = 0
    '                Return True
    '            End If
    '            Dim tex1_pos = InStr(diff_pos, TheXML_String, "<Bool>") + "<Bool>".Length
    '            Dim tex1_Epos = InStr(tex1_pos, TheXML_String, "</Bool>")
    '            Dim newS As String = ""
    '            newS = Mid(TheXML_String, tex1_pos, tex1_Epos - tex1_pos)
    '            'Dim ar = Models.models(mod_id).componets(currentP).color_name
    '            Dim ref As Integer = 0
    '            If newS = "true" Then
    '                ref = 1
    '            End If
    '            Models.models(mod_id).componets(currentP).alphaTestEnable = ref


    '        End If

    '        Return True 'have a valid texture name.. Whaaaahooooooo!!!


    '    End Function


    Private Function normalize(ByVal v As vect3) As vect3
        Dim ul As Single = Sqrt((v.x ^ 2) + (v.y ^ 2) + (v.z ^ 2))
        If ul < 0.0000001 Then Return v
        v.x /= ul
        v.y /= ul
        v.z /= ul
        Return v
    End Function

#Region "This is not used and If'ef out"
#If 1 Then

#End If
#End Region
	Public Function TransformXML(ByVal xmlString As String, ByVal xlsString As String) As MemoryStream
		Dim memStream As MemoryStream = Nothing
		Try
			' Create a xml-document from the sent-in xml-string
			Dim xmlDoc As New XmlDocument
			xmlDoc.LoadXml(xmlString)

			' Load the xls into another document
			Dim xslDoc As New XmlDocument
			xslDoc.LoadXml(xlsString)

			' Create a transformation
			Dim trans As New System.Xml.Xsl.XslCompiledTransform
			trans.Load(xslDoc)

			' Create a memory stream for output
			memStream = New MemoryStream()

			' Do the transformation according to the XSLT and save the result in our memory stream
			trans.Transform(xmlDoc, Nothing, memStream)
			memStream.Position = 0
		Catch ex As Exception
			Throw ex
		End Try

		Return memStream
	End Function


End Module


