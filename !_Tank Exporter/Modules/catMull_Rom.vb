Imports System

Module catMull_Rom
    Public catmullrom As New CatmullRomSpline


    'Interpolation between points with a Catmull-Rom spline
    Public Class CatmullRomSpline
 

        'Display a spline between 2 points derived with the Catmull-Rom spline algorithm
        Public Sub GetCatmullRomSpline1(ByVal pos As Integer)
            'The 4 points we need to form a spline between p1 and p2
            Dim p0 As SlimDX.Vector3 = tracks(ClampListPos(pos - 1)).position
            Dim p1 As SlimDX.Vector3 = tracks(pos).position
            Dim p2 As SlimDX.Vector3 = tracks(ClampListPos(pos + 1)).position
            Dim p3 As SlimDX.Vector3 = tracks(ClampListPos(pos + 2)).position

            'The start position of the line
            Dim lastPos As SlimDX.Vector3 = p1

            'The spline's resolution
            'Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
            Dim resolution As Single = 0.0005F

            'How many times should we loop?
            Dim loops As Integer = Math.Floor(1.0F / resolution)
            For i As Integer = 1 To loops + 1
                'Which t position are we at?
                Dim t As Single = i * resolution

                'Find the coordinate between the end points with a Catmull-Rom spline
                Dim newPos As SlimDX.Vector3 = GetCatmullRomPosition(t, p0, p1, p2, p3)

                'Draw this line segment
                ' Gizmos.DrawLine(lastPos, newPos)
                'Gl.glBegin(Gl.GL_LINES)
                'Gl.glVertex3f(lastPos.X, lastPos.Y, lastPos.Z)
                'Gl.glVertex3f(newPos.X, newPos.Y, newPos.Z)
                'Gl.glEnd()
                Dim V = lastPos - newPos
                running += V.Length
                If Z_Flipped Then
                    If running >= track_length + segment_length_adjusted Then
                        path_data1(path_pointer1) = New path_data_
                        track_length += segment_length_adjusted
                        path_data1(path_pointer1).pos1 = newPos
                        path_pointer1 += 1
                    End If
                Else
                    If running >= track_length + segment_length_adjusted Then
                        path_data1(path_pointer1) = New path_data_
                        track_length += segment_length_adjusted
                        path_data1(path_pointer1).pos1 = newPos
                        path_pointer1 += 1
                    End If

                End If
                'Save this pos so we can draw the next line segment
                lastPos = newPos
            Next
        End Sub
        Public Sub GetCatmullRomSpline2(ByVal pos As Integer)
            'The 4 points we need to form a spline between p1 and p2
            Dim p0 As SlimDX.Vector3 = tracks(ClampListPos(pos - 1)).position
            Dim p1 As SlimDX.Vector3 = tracks(pos).position
            Dim p2 As SlimDX.Vector3 = tracks(ClampListPos(pos + 1)).position
            Dim p3 As SlimDX.Vector3 = tracks(ClampListPos(pos + 2)).position

            'The start position of the line
            Dim lastPos As SlimDX.Vector3 = p1

            'The spline's resolution
            'Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
            Dim resolution As Single = 0.0005F

            'How many times should we loop?
            Dim loops As Integer = Math.Floor(1.0F / resolution)
            For i As Integer = 1 To loops + 1
                'Which t position are we at?
                Dim t As Single = i * resolution

                'Find the coordinate between the end points with a Catmull-Rom spline
                Dim newPos As SlimDX.Vector3 = GetCatmullRomPosition(t, p0, p1, p2, p3)

                'Draw this line segment
                ' Gizmos.DrawLine(lastPos, newPos)
                'Gl.glBegin(Gl.GL_LINES)
                'Gl.glVertex3f(lastPos.X, lastPos.Y, lastPos.Z)
                'Gl.glVertex3f(newPos.X, newPos.Y, newPos.Z)
                'Gl.glEnd()
                Dim V = lastPos - newPos
                running += V.Length
                If Z_Flipped Then
                    If running >= track_length + track_info.segment_offset2 Then
                        path_data2(path_pointer2) = New path_data_
                        track_length += segment_length_adjusted
                        path_data2(path_pointer2).pos1 = newPos
                        path_pointer2 += 1
                    End If
                Else
                    If running >= track_length + track_info.segment_offset2 Then
                        path_data2(path_pointer2) = New path_data_
                        track_length += segment_length_adjusted
                        path_data2(path_pointer2).pos1 = newPos
                        path_pointer2 += 1
                    End If

                End If
                'Save this pos so we can draw the next line segment
                lastPos = newPos
            Next
        End Sub
        Public Sub CatmullRomSpline_get_length(ByVal pos As Integer)
            'The 4 points we need to form a spline between p1 and p2
            Dim p0 As SlimDX.Vector3 = tracks(ClampListPos(pos - 1)).position
            Dim p1 As SlimDX.Vector3 = tracks(pos).position
            Dim p2 As SlimDX.Vector3 = tracks(ClampListPos(pos + 1)).position
            Dim p3 As SlimDX.Vector3 = tracks(ClampListPos(pos + 2)).position

            'The start position of the line
            Dim lastPos As SlimDX.Vector3 = p1

            'The spline's resolution
            'Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
            Dim resolution As Single = 0.0005F

            'How many times should we loop?
            Dim loops As Integer = Math.Floor(1.0F / resolution)
            For i As Integer = 1 To loops + 1
                'Which t position are we at?
                Dim t As Single = i * resolution

                'Find the coordinate between the end points with a Catmull-Rom spline
                Dim newPos As SlimDX.Vector3 = GetCatmullRomPosition(t, p0, p1, p2, p3)

                'Draw this line segment
                ' Gizmos.DrawLine(lastPos, newPos)
                Dim V = lastPos - newPos
                running += V.Length

                lastPos = newPos
            Next
        End Sub
        Public Sub draw_spline()
            For i = 0 To tracks.Length - 1
                DisplayCatmullRomSpline(i)
            Next
        End Sub

        Public Sub DisplayCatmullRomSpline(ByVal pos As Integer)
            'The 4 points we need to form a spline between p1 and p2
            Dim p0 As SlimDX.Vector3 = tracks(ClampListPos(pos - 1)).position
            Dim p1 As SlimDX.Vector3 = tracks(pos).position
            Dim p2 As SlimDX.Vector3 = tracks(ClampListPos(pos + 1)).position
            Dim p3 As SlimDX.Vector3 = tracks(ClampListPos(pos + 2)).position

            'The start position of the line
            Dim lastPos As SlimDX.Vector3 = p1

            'The spline's resolution
            'Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
            Dim resolution As Single = 0.01F

            'How many times should we loop?
            Dim loops As Integer = Math.Floor(1.0F / resolution)
            For i As Integer = 1 To loops + 1
                'Which t position are we at?
                Dim t As Single = i * resolution

                'Find the coordinate between the end points with a Catmull-Rom spline
                Dim newPos As SlimDX.Vector3 = GetCatmullRomPosition(t, p0, p1, p2, p3)
                If running = 0.0 Then
                    Gl.glPushMatrix()
                    Gl.glTranslatef(newPos.X, newPos.Y, newPos.Z)
                    Glut.glutSolidSphere(0.03, 20, 20)
                    Gl.glPopMatrix()
                End If
                Gl.glBegin(Gl.GL_LINES)
                Gl.glVertex3f(lastPos.X, lastPos.Y, lastPos.Z)
                Gl.glVertex3f(newPos.X, newPos.Y, newPos.Z)
                Gl.glEnd()

                'Draw this line segment
                ' Gizmos.DrawLine(lastPos, newPos)
                Dim V = lastPos - newPos
                running += V.Length

                lastPos = newPos
            Next
        End Sub

        'Clamp the list positions to allow looping
        Private Function ClampListPos(pos As Integer) As Integer
            If pos < 0 Then
                pos = tracks.Length - 1
            End If

            If pos > tracks.Length Then
                pos = 1
            ElseIf pos > tracks.Length - 1 Then
                pos = 0
            End If

            Return pos
        End Function

        'Returns a position between 4 SlimDX.Vector3 with Catmull-Rom spline algorithm
        'http://www.iquilezles.org/www/articles/minispline/minispline.htm
        Private Function GetCatmullRomPosition(t As Single, p0 As SlimDX.Vector3, p1 As SlimDX.Vector3, p2 As SlimDX.Vector3, p3 As SlimDX.Vector3) As SlimDX.Vector3
            'The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Dim a As SlimDX.Vector3 = 2.0F * p1
            Dim b As SlimDX.Vector3 = p2 - p0
            Dim c As SlimDX.Vector3 = 2.0F * p0 - 5.0F * p1 + 4.0F * p2 - p3
            Dim d As SlimDX.Vector3 = -p0 + 3.0F * p1 - 3.0F * p2 + p3

            'The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Dim pos As SlimDX.Vector3 = 0.5F * (a + (b * t) + (c * t * t) + (d * t * t * t))

            Return pos
        End Function
    End Class

    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================

End Module
