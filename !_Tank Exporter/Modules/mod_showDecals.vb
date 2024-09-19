Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports Tao.DevIl
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
    Public myListView As New CustomListView
    Private myPictureBox As PictureBox
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
        Me.FormBorderStyle = FormBorderStyle.SizableToolWindow
        Me.Width = 600
        Me.Height = CInt(Width * 0.816)
        Me.TopMost = True ' Keep the form on top
        formAspectRatio = 0.816

        With myListView
            .View = View.LargeIcon
            .Dock = DockStyle.Left
            .Width = 200
            .BackColor = Color.FromArgb(255, 255, 255) ' Dark background color
            .ForeColor = Color.FromArgb(0, 0, 0) ' Light text color
        End With
        myListView.Sorting = SortOrder.None  ' Ensure no sorting is applied

        AddHandler myListView.SelectedIndexChanged, AddressOf myListView_SelectedIndexChanged
        AddHandler myListView.Paint, AddressOf myListView_Paint
        AddHandler Me.SizeChanged, AddressOf Form_SizeChanged

        myPictureBox = New PictureBox With {
            .Dock = DockStyle.None,
            .Location = New Point(myListView.Width, 0),
            .Size = New Size(Me.ClientSize.Width - myListView.Width, Me.ClientSize.Height - 90),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .BackColor = Color.FromArgb(64, 64, 64) ' Dark background color
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

        Me.Controls.Add(myListView)
        Me.Controls.Add(myPictureBox)
        Me.Controls.Add(pathLabel)
        Me.Controls.Add(acceptBut)
        Me.Controls.Add(cancelBut)

        ' Anchor the PictureBox to all sides after everything is initialized
        myPictureBox.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right

        ShowLoadingPopupAndLoadImages() ' Replace with your actual path
    End Sub

    Private Sub ShowLoadingPopupAndLoadImages()
        Dim loadingForm As New LoadingForm()
        loadingForm.Show()
        Application.DoEvents() ' Ensure the loading form is displayed

        LoadDdsImages()

        loadingForm.Close()

    End Sub
    ' Handle form resizing while maintaining aspect ratio
    ' Handle form resizing while maintaining aspect ratio
    Private Sub Form_SizeChanged(sender As Object, e As EventArgs)
        ' Temporarily remove the SizeChanged handler to avoid recursion issues
        RemoveHandler Me.SizeChanged, AddressOf Form_SizeChanged

        ' Calculate the new aspect ratio
        Dim newAspectRatio As Double = Me.Width / Me.Height

        ' Adjust dimensions to maintain the correct aspect ratio
        If newAspectRatio > formAspectRatio Then
            ' Width is too large relative to height; adjust width
            Me.Width = CInt(Me.Height / formAspectRatio)
        ElseIf newAspectRatio < formAspectRatio Then
            ' Height is too large relative to width; adjust height
            Me.Height = CInt(Me.Width * formAspectRatio)
        End If

        ' Re-add the SizeChanged handler
        AddHandler Me.SizeChanged, AddressOf Form_SizeChanged
    End Sub
    Private Sub LoadDdsImages()
        updateEvent.Reset()
        myListView.SuspendLayout()
        For Each item In decal_textures
            If item.full_path.ToLower.Contains("am.dds") Then
                If item.colorMap_Id = 0 Then
                    item.colorMap_Id = load_dds_file(item.full_path)
                End If
                If item.colorMap_Id > 0 Then

                    Dim bm = ConvertAndRescaleTexture(item.colorMap_Id)
                    If bm Is Nothing Then
                        Continue For
                    End If
                    Dim vl_item = New ListViewItem With {
                   .Text = item.colorMap_name,
                   .Tag = item.full_path, ' Store the file path in the Tag property
                   .ImageIndex = myListView.Items.Count
               }
                    myListView.Items.Add(vl_item)
                    If myListView.LargeImageList Is Nothing Then
                        myListView.LargeImageList = New ImageList() With {
                            .ImageSize = New Size(110, 110) ' Set the icon size to 90x90
                        }
                    End If
                    If vl_item IsNot Nothing Then
                        preloadedImages(item.full_path) = bm

                        myListView.LargeImageList.Images.Add(bm)
                    End If
                End If
            End If
        Next
        myListView.ResumeLayout()
        updateEvent.Set()

    End Sub
    Public Function ConvertAndRescaleTexture(textureId As Integer, Optional scaleFactor As Double = 0.25) As Bitmap
        ' Step 1: Bind the OpenGL texture
        Gl.glBindTexture(Gl.GL_TEXTURE_2D, textureId)

        ' Step 2: Retrieve the width and height of the OpenGL texture
        Dim width As Integer
        Dim height As Integer
        Dim depth As Integer = 1 ' For 2D textures, depth is 1

        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_WIDTH, width)
        Gl.glGetTexLevelParameteriv(Gl.GL_TEXTURE_2D, 0, Gl.GL_TEXTURE_HEIGHT, height)

        ' Step 3: Generate an iLL image ID and bind it
        Dim texID As UInt32 = Ilu.iluGenImage()   ' Generate an iLL image ID
        Il.ilBindImage(texID)

        ' Step 4: Allocate memory for the texture data
        Dim textureDataSize As Integer = width * height * 4 ' 4 bytes per pixel (RGBA)
        Dim textureData As IntPtr = Marshal.AllocHGlobal(textureDataSize)

        Try
            ' Step 5: Retrieve the texture data from OpenGL
            Gl.glGetTexImage(Gl.GL_TEXTURE_2D, 0, Gl.GL_BGRA, Gl.GL_UNSIGNED_BYTE, textureData)

            ' Step 6: Load the texture data into iLL
            Il.ilTexImage(width, height, depth, 4, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE, textureData)

            ' Step 7: Apply scaling in iLL
            Dim newWidth As Integer = CInt(width * scaleFactor)
            Dim newHeight As Integer = CInt(height * scaleFactor)
            Ilu.iluScale(newWidth, newHeight, 1) ' Scale the image

            ' Step 8: Allocate memory for the rescaled pixel data
            Dim scaledPixelDataSize As Integer = newWidth * newHeight * 4 ' 4 bytes per pixel (RGBA)
            Dim scaledPixelData As IntPtr = Marshal.AllocHGlobal(scaledPixelDataSize)

            Try
                ' Get the rescaled pixel data from iLL
                Il.ilCopyPixels(0, 0, 0, newWidth, newHeight, 1, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE, scaledPixelData)

                ' Step 9: Create a Bitmap from the scaled pixel data
                Dim bitmap As New System.Drawing.Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb)
                Dim rect As New Rectangle(0, 0, newWidth, newHeight)
                Dim bitmapData As BitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

                ' Convert iLL image format to match the Bitmap
                Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE)

                ' Copy the data from IntPtr to a managed byte array
                Dim pixelData As Byte() = New Byte(scaledPixelDataSize - 1) {}
                Marshal.Copy(scaledPixelData, pixelData, 0, scaledPixelDataSize)

                ' Copy the data to the Bitmap
                System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, bitmapData.Scan0, pixelData.Length)
                bitmap.UnlockBits(bitmapData)

                Return bitmap
            Finally
                ' Clean up scaledPixelData
                Marshal.FreeHGlobal(scaledPixelData)
            End Try
        Finally
            ' Clean up textureData
            Marshal.FreeHGlobal(textureData)
            ' Unbind the iLL image and OpenGL texture
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, 0)    ' Unbind OpenGL texture
            Il.ilBindImage(0)                        ' Unbind iLL image
        End Try
    End Function


    Private Sub myListView_SelectedIndexChanged(sender As Object, e As EventArgs)

        Try
            ' Clear the current selection and select the desired item

            If myListView.SelectedItems.Count > 0 Then

                Dim selectedItem = myListView.SelectedItems(0)
                myPictureBox.Image = preloadedImages(selectedItem.Tag.ToString())
                pathLabel.Text = selectedItem.Tag.ToString() ' Display the image path
                cur_selected_decal = selectedItem.ImageIndex ' Store the selected image ID

                ' Invalidate the myListView to trigger a repaint
                myListView.Invalidate()
            End If
        Finally
        End Try
    End Sub
    Private Sub myListView_Paint(sender As Object, e As PaintEventArgs)
        ' Temporarily remove the SelectedIndexChanged event handler
        RemoveHandler myListView.SelectedIndexChanged, AddressOf myListView_SelectedIndexChanged
        ' Call the base class's OnPaint method to ensure regular drawing logic is executed
        MyBase.OnPaint(e)
        Application.DoEvents()

        ' Draw each item
        For Each item As ListViewItem In myListView.Items
            Dim itemRect = myListView.GetItemRect(item.Index)
            Dim imageSize As New Size(120, 120) ' Target size of the image container
            Dim imageRect As Rectangle ' Final destination rectangle for the image

            ' Draw the image with proper aspect ratio
            If myListView.LargeImageList IsNot Nothing AndAlso item.ImageIndex >= 0 AndAlso item.ImageIndex < myListView.LargeImageList.Images.Count Then
                Dim img = myListView.LargeImageList.Images(item.ImageIndex)

                ' Calculate the aspect ratio of the original image
                Dim aspectRatio As Single = img.Width / img.Height
                Dim containerAspectRatio As Single = imageSize.Width / imageSize.Height

                ' Adjust the image rectangle to maintain the correct aspect ratio
                If aspectRatio > containerAspectRatio Then
                    ' Image is wider, fit to width
                    Dim scaledHeight As Integer = CInt(imageSize.Width / aspectRatio)
                    imageRect = New Rectangle(itemRect.Left, itemRect.Top + (imageSize.Height - scaledHeight) \ 2, imageSize.Width, scaledHeight)
                Else
                    ' Image is taller, fit to height
                    Dim scaledWidth As Integer = CInt(imageSize.Height * aspectRatio)
                    imageRect = New Rectangle(itemRect.Left + (imageSize.Width - scaledWidth) \ 2, itemRect.Top, scaledWidth, imageSize.Height)
                End If

                ' Draw the image in the calculated rectangle
                e.Graphics.DrawImage(img, imageRect)
            End If

            ' Add the 3-place padded ID number in the bottom right corner
            Dim idText As String = item.Index.ToString("D3") ' 3-place padded ID number
            Dim textFont As New Font("Arial", 8, FontStyle.Bold)
            Dim textColor As Color = Color.Black
            Dim textBrush As New SolidBrush(textColor)
            Dim textPosition As New Point(itemRect.Right - 30, itemRect.Bottom - 20) ' Adjust position to bottom right

            e.Graphics.DrawString(idText, textFont, textBrush, textPosition)

            ' Draw the rectangle for selection
            If item.Selected Then
                ' Draw a red rectangle around the selected item
                Using redPen As New Pen(Color.Red, 2)
                    e.Graphics.DrawRectangle(redPen, itemRect.Left, itemRect.Top, itemRect.Width, itemRect.Height)
                End Using
            Else
                ' Draw a grey rectangle around other items
                Using greyPen As New Pen(Color.Gray, 1)
                    e.Graphics.DrawRectangle(greyPen, itemRect.Left, itemRect.Top, itemRect.Width, itemRect.Height)
                End Using
            End If
        Next

        ' Logic for selecting the correct image
        If Not AppCalled Then
            myListView.SelectedItems.Clear()
            For Each item As ListViewItem In myListView.Items
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

            Try
                Dim selectedItem = myListView.SelectedItems(0)
                myPictureBox.Image = preloadedImages(selectedItem.Tag.ToString())
                pathLabel.Text = selectedItem.Tag.ToString() ' Display the image path
                AppCalled = True
                myListView.Invalidate()
            Catch ex As Exception
            End Try
        End If

        ' Re-add the SelectedIndexChanged event handler
        AddHandler myListView.SelectedIndexChanged, AddressOf myListView_SelectedIndexChanged
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
        If imageId >= 0 AndAlso imageId < myListView.Items.Count Then
            myListView.Items(imageId).Selected = True
            myListView.EnsureVisible(imageId)
            myPictureBox.Image = preloadedImages(myListView.Items(imageId).Tag.ToString())
            pathLabel.Text = myListView.Items(imageId).Tag.ToString()
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
        Me.Size = New Size(250, 75)
        Me.StartPosition = FormStartPosition.CenterScreen

        loadingLabel = New Label With {
            .Text = "Loading images, please wait...",
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.FormBorderStyle = FormBorderStyle.FixedToolWindow
        Me.Controls.Add(loadingLabel)
    End Sub
End Class
