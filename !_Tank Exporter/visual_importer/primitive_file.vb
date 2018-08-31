Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Xml

Public Class Primitive_File

    Public Sub ReadPrimitives(ByVal reader As BinaryReader, ByVal element As XmlNode, ByVal xDoc As XmlDocument)
        Dim len As Integer = CInt(reader.BaseStream.Length)
        Dim data As Integer = CInt(reader.BaseStream.Position) + 4

        reader.BaseStream.Position = (CInt(reader.BaseStream.Position) + len - 4)
        Dim indexLen As Integer = reader.ReadInt32()
        Dim offset As Integer = len - (indexLen + 4)
        reader.BaseStream.Position = offset
        Dim oldDataLen As Long = 4
        While offset < (len - 4)
            Dim entryDataLen As Integer = 0
            Dim entryNameLen As Integer = 0
            For i As Integer = 0 To (len - CInt(reader.BaseStream.Position)) - 1
                If reader.ReadByte() <> &H0 Then
                    reader.BaseStream.Position = CInt(reader.BaseStream.Position) - 1
                    entryDataLen = reader.ReadInt32()
                    Exit For
                End If
            Next

            For i As Integer = 0 To (len - CInt(reader.BaseStream.Position)) - 1
                If reader.ReadByte() <> &H0 Then
                    reader.BaseStream.Position = CInt(reader.BaseStream.Position) - 1
                    entryNameLen = reader.ReadInt32()
                    Exit For
                End If
            Next

            Dim entryStr As New String(reader.ReadChars(entryNameLen), 0, entryNameLen)

            Dim XentryStr As XmlNode = xDoc.CreateElement("primitive")
            Dim attr As XmlAttribute = xDoc.CreateAttribute("id")
            attr.InnerText = entryStr
            XentryStr.Attributes.Append(attr)

            Dim XentryDataPos As XmlNode = xDoc.CreateElement("position")
            XentryDataPos.InnerText = Convert.ToString(oldDataLen)
            Dim XentryDataLen As XmlNode = xDoc.CreateElement("length")
            XentryDataLen.InnerText = Convert.ToString(entryDataLen)
            oldDataLen += (entryDataLen + 3) And (Not 3L)

            XentryStr.AppendChild(XentryDataPos)
            XentryStr.AppendChild(XentryDataLen)

            offset = CInt(reader.BaseStream.Position) + entryNameLen
            element.AppendChild(XentryStr)
        End While
    End Sub
End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik, @toddanglin
'Facebook: facebook.com/telerik
'=======================================================

