<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFullScreen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.fs_render_box = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        'fs_render_box
        '
        Me.fs_render_box.BackColor = System.Drawing.Color.DimGray
        Me.fs_render_box.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.fs_render_box.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.fs_render_box.Dock = System.Windows.Forms.DockStyle.Fill
        Me.fs_render_box.Location = New System.Drawing.Point(0, 0)
        Me.fs_render_box.Name = "fs_render_box"
        Me.fs_render_box.Size = New System.Drawing.Size(800, 450)
        Me.fs_render_box.TabIndex = 0
        Me.fs_render_box.Text = ""
        '
        'frmFullScreen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.fs_render_box)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "frmFullScreen"
        Me.Opacity = 0R
        Me.Text = "frmFullScreen"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents fs_render_box As RichTextBox
End Class
