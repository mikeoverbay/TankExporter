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
        xDoc.OuterXml.Replace("  ", "")
        xDoc.OuterXml.Replace("> ", ">")
        xDoc.OuterXml.Replace(">" + vbTab, ">")
        xDoc.OuterXml.Replace(" <", "<")
        xDoc.OuterXml.Replace(vbTab + "<", "<")


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
        fbr.Close()
        fbw.Close()
        fileS.Close()
        fileS.Dispose()
    End Sub
    Private Function fix_bad_tags(ByVal xmlString As String) As String
        'box all primitive tags.. Dont think there will ever be over 30 :-)
        For i = 0 To 30
            Dim ast = xmlString.Replace("<primitiveGroup>" + i.ToString, "<primitiveGroup>" + ControlChars.CrLf.ToCharArray() + "<PG_ID>" + i.ToString + "</PG_ID>")
            xmlString = ast
        Next
        Return xmlString
    End Function
    Public Function PrettyPrint(XML As [String]) As [String]
        Dim Result As [String] = ""
        'another hack to fix WG's bad xml
        XML = XML.Replace("<_", "<G_")
        XML = XML.Replace("</_", "</G_")
        XML = XML.Replace("><", ">" + vbCrLf + "<")
        XML = XML.Replace("  ", "")
        XML = XML.Replace("> ", ">")
        'XML = XML.Replace(">" + vbTab, ">")
        'XML = XML.Replace(vbTab, "")
        'XML = XML.Replace(" <", "<")
        'XML = XML.Replace(vbTab + "<", "<")
        XML = XML.Replace(vbCrLf + "</property>", "</property>")
        XML = fix_bad_tags(XML)
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

    Public Function get_bw_xml(p) As String
        Dim buf = File.ReadAllBytes(p)
        Dim mstream = New MemoryStream(buf)
        If openXml_stream(mstream, Path.GetFileName(p)) Then
            mstream.Dispose()
            buf = Nothing
            GC.Collect()
            Return TheXML_String
        End If
        mstream.Dispose()
        GC.Collect()
        TheXML_String = File.ReadAllText(p)
        Return TheXML_String
    End Function




End Module


