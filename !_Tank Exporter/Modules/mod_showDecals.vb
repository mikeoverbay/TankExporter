Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Forms

Public Class CustomListView
    Inherits ListView

    Public Sub New()
        MyBase.New()
        Me.SetStyle(ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

    End Sub


End Class
Public Class showDecals
    Inherits Form

    Private Shared instance As showDecals = Nothing
    Public listView As New CustomListView
    Private pictureBox As PictureBox
    Private pathLabel As Label
    Private acceptBut As Button
    Private cancelBut As Button
    Private preloadedImages As Dictionary(Of String, Bitmap)
    Public AppCalled As Boolean = False
    ' Singleton pattern to ensure only one instance of the form
    Public Shared Function GetInstance() As showDecals
        If instance Is Nothing OrElse instance.IsDisposed Then
            instance = New showDecals()
        End If
        Return instance
    End Function

    Private Sub New()
        preloadedImages = New Dictionary(Of String, Bitmap)()
        InitializeCustomComponents()
        AddHandler Me.Shown, AddressOf Form_Shown
        AddHandler Me.FormClosing, AddressOf showDecals_FormClosing
    End Sub

    Private Sub InitializeCustomComponents()
        Me.Width = 600 ' Make the form 3 times wider
        Me.Height = 800 ' Make the form 3 times wider
        Me.TopMost = True ' Keep the form on top

        With listView
            .View = View.LargeIcon
            .Dock = DockStyle.Left
            .Width = 200
            .BackColor = Color.FromArgb(255, 255, 255) ' Dark background color
            .ForeColor = Color.FromArgb(0, 0, 0) ' Light text color
        End With
        listView.Sorting = SortOrder.None  ' Ensure no sorting is applied

        AddHandler listView.SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged
        AddHandler listView.Paint, AddressOf ListView_Paint

        pictureBox = New PictureBox With {
            .Dock = DockStyle.Fill,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .BackColor = Color.FromArgb(30, 30, 30) ' Dark background color
        }

        pathLabel = New Label With {
            .Dock = DockStyle.Bottom,
            .Height = 30,
            .TextAlign = ContentAlignment.MiddleLeft,
            .BackColor = Color.FromArgb(45, 45, 48), ' Dark background color
            .ForeColor = Color.FromArgb(220, 220, 220) ' Light text color
        }

        acceptBut = New Button With {
            .Text = "Accept",
            .Dock = DockStyle.Bottom,
            .Height = 30
        }
        AddHandler acceptBut.Click, AddressOf acceptBut_Click

        cancelBut = New Button With {
            .Text = "Cancel",
            .Dock = DockStyle.Bottom,
            .Height = 30
        }
        AddHandler cancelBut.Click, AddressOf cancelBut_Click

        Me.Controls.Add(listView)
        Me.Controls.Add(pictureBox)
        Me.Controls.Add(pathLabel)
        Me.Controls.Add(acceptBut)
        Me.Controls.Add(cancelBut)

        ShowLoadingPopupAndLoadImages() ' Replace with your actual path
    End Sub

    Private Sub ShowLoadingPopupAndLoadImages()
        Dim loadingForm As New LoadingForm()
        loadingForm.Show()
        Application.DoEvents() ' Ensure the loading form is displayed

        LoadDdsImages()

        loadingForm.Close()
    End Sub

    Private Sub LoadDdsImages()
        updateEvent.Reset()
        listView.SuspendLayout()
        For Each item In decal_textures
            If item.full_path.ToLower.Contains("am.dds") Then
                If item.colorMap_Id = 0 Then
                    item.colorMap_Id = load_dds_file(item.full_path)
                End If
                If item.colorMap_Id > 0 Then

                    Dim bm = ConvertTextureToBitmap(item.colorMap_Id)
                    If bm Is Nothing Then
                        Continue For
                    End If
                    Dim vl_item = New ListViewItem With {
                   .Text = item.colorMap_name,
                   .Tag = item.full_path, ' Store the file path in the Tag property
                   .ImageIndex = listView.Items.Count
               }
                    listView.Items.Add(vl_item)
                    If listView.LargeImageList Is Nothing Then
                        listView.LargeImageList = New ImageList() With {
                            .ImageSize = New Size(110, 110) ' Set the icon size to 90x90
                        }
                    End If
                    If vl_item IsNot Nothing Then
                        preloadedImages(item.full_path) = bm

                        listView.LargeImageList.Images.Add(bm)
                    End If
                End If
            End If
        Next
        listView.ResumeLayout()
        updateEvent.Set()

    End Sub
    Private Function ConvertTextureToBitmap(textureId As Integer) As Bitmap
        ' Bind the texture
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureId)

        ' Get the width of the texture
        Dim width As Integer
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, width)

        ' Get the height of the texture
        Dim height As Integer
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, height)

        ' Allocate memory for the pixel data
        Dim pixelData(width * height * 4 - 1) As Byte ' 4 bytes per pixel (RGBA)

        ' Read the pixel data from the texture
        Gl.glGetTexImage(Gl.GL_TEXTURE_2D, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, pixelData)

        ' Create a Bitmap from the pixel data
        Dim bitmap = Nothing
        Try
            bitmap = New System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb)

        Catch ex As Exception
            Return Nothing
        End Try
        ' Lock the bitmap's bits
        Dim bmpData As BitmapData = bitmap.LockBits(New Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bitmap.PixelFormat)

        ' Copy the pixel data into the bitmap
        System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, bmpData.Scan0, pixelData.Length)

        ' Unlock the bits
        bitmap.UnlockBits(bmpData)

        ' Unbind the texture
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)

        Return bitmap
    End Function


    Private Sub ListView_SelectedIndexChanged(sender As Object, e As EventArgs)

        Try
            ' Clear the current selection and select the desired item

            If listView.SelectedItems.Count > 0 Then

                Dim selectedItem = listView.SelectedItems(0)
                pictureBox.Image = preloadedImages(selectedItem.Tag.ToString())
                pathLabel.Text = selectedItem.Tag.ToString() ' Display the image path
                cur_selected_decal = selectedItem.ImageIndex ' Store the selected image ID

                ' Invalidate the ListView to trigger a repaint
                listView.Invalidate()
            End If
        Finally
        End Try
    End Sub
    Private Sub ListView_Paint(sender As Object, e As PaintEventArgs)
        ' Temporarily remove the SelectedIndexChanged event handler
        RemoveHandler listView.SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged
        'Call the base class's OnPaint method to ensure regular drawing logic is executed
        MyBase.OnPaint(e)
        Application.DoEvents()
        'listView.SelectedItems.Clear()
        'listView.Items(cur_selected_decal).Selected = True

        ' Draw each item
        For Each item As ListViewItem In listView.Items
            Dim itemRect = listView.GetItemRect(item.Index)
            Dim imageRect = New Rectangle(itemRect.Left, itemRect.Top, 80, 80) ' Adjust the size and position as needed

            ' Draw the image
            If listView.LargeImageList IsNot Nothing AndAlso item.ImageIndex >= 0 AndAlso item.ImageIndex < listView.LargeImageList.Images.Count Then
                e.Graphics.DrawImage(listView.LargeImageList.Images(item.ImageIndex), imageRect)
            End If

            ' Draw the index ID and tag string name
            Dim indexText As String = item.Index.ToString("D3") ' Format index as three digits
            Dim textRect As New Rectangle(itemRect.Left, itemRect.Bottom - 50, itemRect.Width, 40) ' Adjust the position as needed

            Using textBrush As New SolidBrush(Color.Black)
                e.Graphics.DrawString($"{indexText}. {item.Text.ToString()}", listView.Font, textBrush, textRect)
            End Using

            ' Draw the rectangle
            If item.Selected Then
                ' Draw a red rectangle around the selected item
                Using redPen As New Pen(Color.Red, 2)
                    e.Graphics.DrawRectangle(redPen, itemRect.Left, itemRect.Top, itemRect.Width, itemRect.Height)
                End Using
            Else
                ' Draw a grey rectangle around the other items
                Using greyPen As New Pen(Color.Gray, 1)
                    e.Graphics.DrawRectangle(greyPen, itemRect.Left, itemRect.Top, itemRect.Width, itemRect.Height)
                End Using
            End If
        Next
        If Not AppCalled Then
            listView.SelectedItems.Clear()
            For Each item As ListViewItem In listView.Items
                ' Check if the item's Tag matches the desired tag name
                If item.Tag IsNot Nothing AndAlso item.Tag.ToString().Contains(frmMain.d_texture_name.Text) Then
                    ' Select the item
                    item.Selected = True
                    ' Ensure the item is visible
                    item.EnsureVisible()

                    ' Exit the loop as we found the item
                    Exit For
                End If
            Next
            Dim selectedItem = listView.SelectedItems(0)
            pictureBox.Image = preloadedImages(selectedItem.Tag.ToString())
            pathLabel.Text = selectedItem.Tag.ToString() ' Display the image path
            AppCalled = True
            listView.Invalidate()
        End If
        ' Re-add the SelectedIndexChanged event handler
        AddHandler listView.SelectedIndexChanged, AddressOf ListView_SelectedIndexChanged
        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub
    Private Sub acceptBut_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.OK

    End Sub

    Private Sub cancelBut_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Hide()
    End Sub

    ' Method to show the form as a dialog and return the selected image ID
    Public Function ShowDialogAndGetImageId() As Integer
        If Me.ShowDialog() = DialogResult.OK Then
            Return cur_selected_decal
        Else
            Return -1
        End If
    End Function

    ' Method to select and display the image based on the provided ID
    Private Sub SelectAndDisplayImage(imageId As Integer)
        If imageId >= 0 AndAlso imageId < listView.Items.Count Then
            listView.Items(imageId).Selected = True
            listView.EnsureVisible(imageId)
            pictureBox.Image = preloadedImages(listView.Items(imageId).Tag.ToString())
            pathLabel.Text = listView.Items(imageId).Tag.ToString()
            cur_selected_decal = imageId
        Else
            Console.WriteLine($"Invalid imageId: {imageId}")
        End If
    End Sub

    ' Method to update the selected image based on the provided ID
    Public Sub UpdateSelectedImage(imageId As Integer)
        SelectAndDisplayImage(imageId)
    End Sub

    ' Event handler for the Shown event
    Private Sub Form_Shown(sender As Object, e As EventArgs)

    End Sub

    ' Event handler for the FormClosed event

    Private Sub showDecals_FormClosing(sender As Object, e As FormClosingEventArgs)
        ' Cancel the close operation and hide the form instead
        'e.Cancel = True
        'Me.Hide()
    End Sub
End Class

' Usage example:
' Dim decalsForm As showDecals = showDecals.GetInstance(current_decal)
' Dim selectedImageId As Integer = decalsForm.ShowDialogAndGetImageId()
' If selectedImageId <> -1 Then
'     ' Use the selected image ID
' End If
Public Class LoadingForm
    Inherits Form

    Private loadingLabel As Label

    Public Sub New()
        Me.Text = "Loading"
        Me.Size = New Size(300, 100)
        Me.StartPosition = FormStartPosition.CenterScreen

        loadingLabel = New Label With {
            .Text = "Loading images, please wait...",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleCenter
        }

        Me.Controls.Add(loadingLabel)
    End Sub
End Class
