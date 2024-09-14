Module mod_FindTankParts

    Public xmlPartList As DataTable
    Public Function Load_partList() As Boolean
        Try

            Dim xmlFilePath As String = Application.StartupPath + "/resources/XMLdata/TheItemList.xml"

            ' Create a DataSet
            Dim dataSet As New DataSet()

            ' Load the XML file into the DataSet
            dataSet.ReadXml(xmlFilePath)

            ' Assuming the XML file contains a single DataTable
            xmlPartList = dataSet.Tables(0)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    Public Function Find_entry(ByVal name As String) As String
        Debug.WriteLine("=======" + name)
        name = name.Replace("\", "/")
        If xmlPartList IsNot Nothing Then
            For Each row As DataRow In xmlPartList.Rows
                If row("filename").ToString().ToLower = name.ToLower Then
                    Return row("package").ToString()
                End If
            Next
        End If
        Return String.Empty
    End Function
End Module
