Imports Aspose.ThreeD.Utilities

Imports System.IO

Module put_STL

    Public Sub ExportBinarySTL()
        ' Open a binary file for writing

        frmMain.SaveFileDialog1.InitialDirectory = My.Settings.GLTF_path

        ' Update filter to handle STL files
        frmMain.SaveFileDialog1.Filter = "STL files (*.stl)|*.stl"

        frmMain.SaveFileDialog1.Title = "Save STL File As..."
        frmMain.SaveFileDialog1.FileName = My.Settings.STL_path

        frmMain.SaveFileDialog1.AddExtension = True
        frmMain.SaveFileDialog1.RestoreDirectory = True

        Dim result = frmMain.SaveFileDialog1.ShowDialog()
        'Bail If the user cancels the dialog
        If result = DialogResult.Cancel Then
            Return ' or Exit Sub/Function, depending on your context
        End If

        Dim sep As Boolean = False
        If MsgBox("Seperate parts = Yes, No = normal pose.", MsgBoxStyle.YesNo, "How do you want it?") = MsgBoxResult.Yes Then
            sep = True
        End If

        Dim out_path = frmMain.SaveFileDialog1.FileName
        My.Settings.STL_path = out_path
        My.Settings.Save()

        ' 2. Write the number of triangles (4 bytes)
        Dim nPrimitives As UInteger
        For i = 1 To object_count
            nPrimitives += _group(i).nPrimitives_
        Next

        If File.Exists(out_path) Then
            File.Delete(out_path)
        End If

        Dim cur_x_pos As Single
        Dim sep_off As Single
        Using fileStream As New FileStream(out_path, FileMode.Create)
            Using writer As New BinaryWriter(fileStream)
                Dim header(79) As Byte
                writer.Write(header)
                writer.Write(nPrimitives)

                For item = 1 To object_count
                    Dim v As FVector3
                    v.X = _object(item).matrix(12)
                    v.Y = _object(item).matrix(13)
                    v.Z = _object(item).matrix(14)

                    Dim part_length = 0 '(_object(item).ix_max - _object(item).ix_min) * -1.0F
                    Dim delta = _object(item).center_x
                    part_length += delta + 3
                    cur_x_pos += part_length

                    If sep Then
                        sep_off = cur_x_pos
                        v = New FVector3
                    End If

                    If _object(item).name.ToLower.Contains("gun") Then
                        v.Z *= -1.0F
                    End If

                    Dim offset = _group(item).startVertex_
                    '====================================================================
                    For i As Integer = 1 To _group(item).nPrimitives_



                        Dim index As uvect3 = _group(item).indices(i)

                        If Not _object(item).name.ToLower.Contains("gun") Then
                            Dim dx = index.v1
                            index.v1 = index.v2
                            index.v2 = dx
                        End If
                        index.v1 -= offset
                        index.v2 -= offset
                        index.v3 -= offset

                        Dim v1 As FVector3
                        v1.X = _group(item).vertices(index.v1).x + v.X + sep_off
                        v1.Y = _group(item).vertices(index.v1).z + v.Z
                        v1.Z = _group(item).vertices(index.v1).y + v.Y

                        Dim v2 As FVector3
                        v2.X = _group(item).vertices(index.v2).x + v.X + sep_off
                        v2.Y = _group(item).vertices(index.v2).z + v.Z
                        v2.Z = _group(item).vertices(index.v2).y + v.Y

                        Dim v3 As FVector3
                        v3.X = _group(item).vertices(index.v3).x + v.X + sep_off
                        v3.Y = _group(item).vertices(index.v3).z + v.Z
                        v3.Z = _group(item).vertices(index.v3).y + v.Y

                        If _object(item).name.ToLower.Contains("gun") Then
                            v1.Y *= -1.0F
                            v2.Y *= -1.0F
                            v3.Y *= -1.0F
                        End If

                        Dim normal As FVector3 = CalculateNormal(v1, v2, v3)

                        ' 5. Write the normal (3 floats)
                        writer.Write(normal.X)
                        writer.Write(normal.Y)
                        writer.Write(normal.Z)

                        ' 6. Write the vertices (each as 3 floats)
                        ' Vertex 1
                        writer.Write(v1.X)
                        writer.Write(v1.Y)
                        writer.Write(v1.Z)

                        ' Vertex 2
                        writer.Write(v2.X)
                        writer.Write(v2.Y)
                        writer.Write(v2.Z)

                        ' Vertex 3
                        writer.Write(v3.X)
                        writer.Write(v3.Y)
                        writer.Write(v3.Z)

                        writer.Write(CShort(0))
                    Next
                Next
            End Using
        End Using
    End Sub

    ' Helper function to calculate the normal for a triangle
    Function CalculateNormal(v1 As Vector3, v2 As Vector3, v3 As Vector3) As Vector3
        Dim u As Vector3 = v2 - v1
        Dim v As Vector3 = v3 - v1

        ' Cross product to get the normal
        Dim normal As New Vector3(
        (u.Y * v.Z) - (u.Z * v.Y),
        (u.Z * v.X) - (u.X * v.Z),
        (u.X * v.Y) - (u.Y * v.X)
    )

        ' Calculate the length of the normal vector
        Dim length As Single = Math.Sqrt(normal.X ^ 2 + normal.Y ^ 2 + normal.Z ^ 2)

        ' Check if the length is greater than 0 to avoid division by zero
        If length > 0 Then
            normal.X /= length
            normal.Y /= length
            normal.Z /= length
        Else
            ' If length is zero, return a default normal vector (e.g., pointing up)
            normal = New Vector3(0, 0, 1)
        End If

        Return normal
    End Function

End Module
