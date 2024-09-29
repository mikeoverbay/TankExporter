Imports System.ComponentModel
Imports System.Runtime.InteropServices

Public Class frmPickDecal
    Private decals_loaded As Boolean = False
    Public Structure cell_
        Public posY As Single
        Public posX As Single
        Public W As Single
        Public H As Single
        Public tex_id As Integer
        Public selected As Boolean
        Public index As UInt16
        ' Make the draw method public

    End Structure
    Public Y_delta As Single = 0
    '=========================================================================================================
    '===== program code below
    '=========================================================================================================
    Sub DrawCenteredImage(textureID As Integer)
        Dim imageWidth As Integer = 480
        Dim imageHeight As Integer = 480
        Dim clientWidth = Me.ClientSize.Width
        Dim clientHeight = Me.ClientSize.Height

        ' Calculate the top-left corner of the image
        Dim posX As Single = 120.0 ' Center X and shift left by 100
        Dim posY As Single = (clientHeight / 2) - (imageHeight / 2) ' Center Y

        ' Enable 2D texture
        Gl.glEnable(Gl.GL_TEXTURE_2D)

        ' Bind the texture to be drawn
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureID)

        ' Draw the image as a quad with texture mapping
        Gl.glBegin(Gl.GL_QUADS)
        ' Bottom-left corner
        Gl.glTexCoord2f(0.0F, 0.0F)
        Gl.glVertex2f(posX, -posY)

        ' Bottom-right corner
        Gl.glTexCoord2f(1.0F, 0.0F)
        Gl.glVertex2f(posX + imageWidth, -posY)

        ' Top-right corner
        Gl.glTexCoord2f(1.0F, 1.0F)
        Gl.glVertex2f(posX + imageWidth, -posY + -imageHeight)

        ' Top-left corner
        Gl.glTexCoord2f(0.0F, 1.0F)
        Gl.glVertex2f(posX, -posY + -imageHeight)
        Gl.glEnd()

        ' Disable texture if no longer needed
        Gl.glDisable(Gl.GL_TEXTURE_2D)
    End Sub

    Public Sub draw(ByRef c As frmPickDecal.cell_)
        If c.tex_id <> 0 Then

            Gl.glEnable(Gl.GL_BLEND)
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA)
            Gl.glColor4f(1.0, 1.0, 1.0, 1.0)

            ' Draw with texture
            ' Reset to solid (filled) rendering
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)

            Gl.glEnable(Gl.GL_TEXTURE_2D)
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, c.tex_id)

            Gl.glBegin(Gl.GL_QUADS)
            ' Bottom-left corner
            Gl.glTexCoord2f(0.0F, 0.0F)
            Gl.glVertex2f(c.posX, c.posY - Y_delta)

            ' Bottom-right corner
            Gl.glTexCoord2f(1.0F, 0.0F)
            Gl.glVertex2f(c.posX + c.W, c.posY - Y_delta)

            ' Top-right corner
            Gl.glTexCoord2f(1.0F, 1.0F)
            Gl.glVertex2f(c.posX + c.W, c.posY - c.H - Y_delta)

            ' Top-left corner
            Gl.glTexCoord2f(0.0F, 1.0F)
            Gl.glVertex2f(c.posX, c.posY - c.H - Y_delta)
            Gl.glEnd()

            Gl.glDisable(Gl.GL_TEXTURE_2D)

            If c.selected Then
                DrawCenteredImage(c.tex_id)
                Gl.glColor4f(1.0, 0.0, 0.0, 1.0) ' for boarder
            Else
                Gl.glColor4f(0.0, 1.0, 0.0, 1.0)

            End If
            Gl.glDisable(Gl.GL_BLEND)

            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE) ' Set polygon mode to render lines (outlines only)

            Gl.glBegin(Gl.GL_QUADS)
            ' Bottom-left corner
            Gl.glTexCoord2f(0.0F, 0.0F)
            Gl.glVertex2f(c.posX, c.posY - Y_delta)

            ' Bottom-right corner
            Gl.glTexCoord2f(1.0F, 0.0F)
            Gl.glVertex2f(c.posX + c.W, c.posY - Y_delta)

            ' Top-right corner
            Gl.glTexCoord2f(1.0F, 1.0F)
            Gl.glVertex2f(c.posX + c.W, c.posY - c.H - Y_delta)

            ' Top-left corner
            Gl.glTexCoord2f(0.0F, 1.0F)
            Gl.glVertex2f(c.posX, c.posY - c.H - Y_delta)
            Gl.glEnd()
            DrawText(c.posX + c.W + 10, c.posY - (c.H * 0.5) - Y_delta, c.index.ToString, 0.0F, 1.0F, 0.0F)

            Gl.glColor4f(1.0, 1.0, 1.0, 1.0)

        End If
    End Sub

    Private cell_list() As cell_


    Private Sub frmPickDecal_Load(sender As Object, e As EventArgs) Handles Me.Load
    End Sub

    Public Sub loadcells()
        If decals_loaded Then Return
        Dim old_size = Me.Size
        Me.Size = New Size(250, 75)
        Me.StartPosition = FormStartPosition.CenterParent
        pb4.Hide()
        cancel_selection_bt.Visible = False
        Dim loadingLabel = New Label With {
            .Text = "Loading images, please wait...",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleCenter,
            .ForeColor = Color.White,
            .BackColor = Me.BackColor
        }
        Me.FormBorderStyle = FormBorderStyle.FixedToolWindow
        Me.Controls.Add(loadingLabel)

        Me.Show()
        'Application.DoEvents()

        If decals_loaded Then Return
        Dim H As Single = 80
        Dim W As Single = 80
        ' Me.Location = Parent.Location
        updateEvent.Reset()
        Threading.Thread.Sleep(100)

        ReDim Preserve cell_list(decal_textures.Count - 1)
        For i = 0 To decal_textures.Count - 1
            load_this_Decal(i)
            cell_list(i) = New cell_
            cell_list(i).tex_id = decal_textures(i).colorMap_Id
            cell_list(i).posX = 5
            cell_list(i).W = W
            cell_list(i).H = H
            cell_list(i).posY = i * (-H + -10)

            cell_list(i).index = i

            If Not current_decal_data_pnt = -1 Then
                If i = decal_matrix_list(current_decal_data_pnt).DecalIndex Then
                    cell_list(i).selected = True
                Else
                    cell_list(i).selected = False
                End If
            End If


            loadingLabel.Text = "Loading image: [" + i.ToString("D3") + "]  , please wait..."
            Application.DoEvents()
        Next
        decals_loaded = True
        Me.Controls.Remove(loadingLabel)
        Me.Size = old_size
        Me.FormBorderStyle = FormBorderStyle.SizableToolWindow
        pb4.Show()
        cancel_selection_bt.Visible = True
        updateEvent.Set()
    End Sub
    Private Sub frmPickDecal_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel() = True
        Me.Hide()
    End Sub
    Public Sub SetupOpenGL()


        ' Make the rendering context current
        Wgl.wglMakeCurrent(pb4_hDC, pb4_hRC)

        ' Setup 2D projection
        Gl.glViewport(0, 0, Me.ClientSize.Width, Me.ClientSize.Height)
        Gl.glMatrixMode(Gl.GL_PROJECTION)
        Gl.glLoadIdentity()
        Gl.glOrtho(0, Me.ClientSize.Width, -Me.ClientSize.Height, 0, -1, 1) ' Set up 2D orthographic projection
        Gl.glMatrixMode(Gl.GL_MODELVIEW)
        Gl.glLoadIdentity()

        ' Clear the background
        Gl.glClearColor(0.6F, 0.0F, 0.0F, 1.0F)
    End Sub

    Private Sub frmPickDecal_ClientSizeChanged(sender As Object, e As EventArgs) Handles Me.ClientSizeChanged
        'If Not _Started Then Return
        'SetupOpenGL()
        'DrawText(5, -8, "Hello, OpenGL!", 0.0F, 1.0F, 0.0F) ' Example text
    End Sub

    Private Sub frmPickDecal_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'If Not _Started Then Return
        'SetupOpenGL()
        'loadcells()
    End Sub
    Public Sub DrawText(ByVal x As Integer, ByVal y As Integer, ByVal text As String, ByVal r As Single, ByVal g As Single, ByVal b As Single)
        Gl.glColor3f(r, g, b) ' Set text color
        glutPrint(x, y, text, r, g, b, 1.0) ' Call your existing text drawing method
    End Sub
    Private old_texture As Integer
    Public Sub set_selecion(ByVal cellIndex As Integer)
        If Not decals_loaded Then Return
        If cell_list Is Nothing Then Return
        If cell_list.Length = 0 Then Return
        old_texture = cellIndex
        If cellIndex > -1 Then

            For i = 0 To cell_list.Length - 1
                cell_list(i).selected = False
            Next
            cell_list(cellIndex).selected = True
            Y_delta = (current_decal_data_pnt * -90) + 8

            Dim cp = cell_list(cellIndex).posY
            Dim sb = -pb4.Height / 2
            Y_delta = cp - sb



        End If

    End Sub
    '================================================================================================
    Private Sub HandleCellClick(cellIndex As Integer)
        ' Logic for what happens when a cell is clicked
        For i = 0 To cell_list.Length - 1
            cell_list(i).selected = False
        Next
        cell_list(cellIndex).selected = True
        frmMain.selected_texture_changed(cellIndex)

    End Sub

    Public Sub Draw_Decal_Cells()
        Wgl.wglMakeCurrent(pb4_hDC, pb4_hRC)
        ' Get the client size of the OpenGL window
        SetupOpenGL()

        Gl.glClearColor(0.2F, 0.0F, 0.0F, 1.0F)
        Gl.glClear(Gl.GL_COLOR_BUFFER_BIT)

        For i As Integer = 0 To cell_list.Length - 1
            ' Draw the cell with the current color
            draw(cell_list(i))

        Next
        If current_decal_data_pnt > -1 Then
            DrawText(130, -10, "Selected: " + decal_matrix_list(current_decal_data_pnt).DecalTexture, 0.0F, 1.0F, 0.0F)

        End If
        Gl.glColor4f(1.0, 1.0, 1.0, 1.0)
        ' Swap the buffers to display the square
        Wgl.wglSwapBuffers(pb4_hDC)
        'DrawCells()
        ' Assuming pb4_hDC is the correct DC for the context
    End Sub


    Private Sub pb4_MouseEnter(sender As Object, e As EventArgs) Handles pb4.MouseEnter

    End Sub

    Private Sub pb4_MouseLeave(sender As Object, e As EventArgs) Handles pb4.MouseLeave

    End Sub

    Dim m_down As Boolean = False
    Dim mouse_last_pos As vec2
    Dim scaling_factor As Single = 8.0
    Private Sub pb4_MouseDown(sender As Object, e As MouseEventArgs) Handles pb4.MouseDown
        m_down = True

        mouse_last_pos.x = e.Location.X
        mouse_last_pos.y = e.Location.Y

    End Sub

    Private Sub pb4_MouseUp(sender As Object, e As MouseEventArgs) Handles pb4.MouseUp
        m_down = False
    End Sub

    Private Sub pb4_MouseClick(sender As Object, e As MouseEventArgs) Handles pb4.MouseClick
        Dim mouseX As Single = e.X
        Dim mouseY As Single = e.Y
        m_down = True

        For i As Integer = 0 To cell_list.Length - 1
            If mouseX >= cell_list(i).posX AndAlso mouseX <= (cell_list(i).posX + cell_list(i).W) AndAlso
               mouseY >= -cell_list(i).posY + Y_delta AndAlso mouseY <= (-cell_list(i).posY + cell_list(i).H + Y_delta) Then
                ' Cell clicked: handle the click
                HandleCellClick(i)
                Exit For
            End If
        Next
    End Sub

    Private Sub pb4_MouseMove(sender As Object, e As MouseEventArgs) Handles pb4.MouseMove
        If m_down Then


            Dim current_mouse_pos As New vec2
            current_mouse_pos.x = e.Location.X
            current_mouse_pos.y = e.Location.Y

            ' Calculate Y-axis movement
            Dim y_movement As Single = current_mouse_pos.y - mouse_last_pos.y

            ' Apply scaling factor to Y movement
            Y_delta += y_movement * scaling_factor

            If Y_delta >= 8 And Math.Sign(y_movement) = 1 Then
                Y_delta = 8
                mouse_last_pos = current_mouse_pos
                Return
            End If
            If Y_delta <= cell_list.Length * -90 + Me.ClientSize.Height And Math.Sign(y_movement) = -1 Then
                Y_delta = cell_list.Length * -90 + Me.ClientSize.Height
                mouse_last_pos = current_mouse_pos
                Return
            End If

            ' Update last position
            mouse_last_pos = current_mouse_pos
        End If
    End Sub

    Private Sub pb4_MouseWheel(sender As Object, e As MouseEventArgs) Handles pb4.MouseWheel
        ' Use the same logic for Y-axis movement
        Dim scrollAmount As Single = 10.0F ' Adjust scroll sensitivity

        ' Calculate movement based on mouse wheel delta
        Dim y_movement As Single = (e.Delta / SystemInformation.MouseWheelScrollDelta) * scrollAmount

        ' Apply scaling factor to Y movement
        Y_delta += y_movement * scaling_factor * 3

        ' Clamp Y_delta to the limits set in your MouseMove event
        If Y_delta >= 8 And Math.Sign(y_movement) = 1 Then
            Y_delta = 8
            Return
        End If

        If Y_delta <= cell_list.Length * -90 + Me.ClientSize.Height And Math.Sign(y_movement) = -1 Then
            Y_delta = cell_list.Length * -90 + Me.ClientSize.Height
            Return
        End If

    End Sub

    Private Sub cancel_selection_bt_Click(sender As Object, e As EventArgs) Handles cancel_selection_bt.Click
        For i = 0 To cell_list.Length - 1
            cell_list(i).selected = False
        Next
        cell_list(old_texture).selected = True
        frmMain.selected_texture_changed(old_texture)
    End Sub
End Class

