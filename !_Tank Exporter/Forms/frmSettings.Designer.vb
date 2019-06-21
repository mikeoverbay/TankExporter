<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSettings
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.FOV_trackbar = New System.Windows.Forms.TrackBar()
        Me.TrackBar1 = New System.Windows.Forms.TrackBar()
        Me.fov_v = New System.Windows.Forms.Label()
        Me.mouse_v = New System.Windows.Forms.Label()
        CType(Me.FOV_trackbar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(6, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(80, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Field of View"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(101, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(84, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Mouse Speed"
        '
        'FOV_trackbar
        '
        Me.FOV_trackbar.Location = New System.Drawing.Point(22, 34)
        Me.FOV_trackbar.Maximum = 90
        Me.FOV_trackbar.Minimum = 10
        Me.FOV_trackbar.Name = "FOV_trackbar"
        Me.FOV_trackbar.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.FOV_trackbar.Size = New System.Drawing.Size(45, 163)
        Me.FOV_trackbar.TabIndex = 2
        Me.FOV_trackbar.TickFrequency = 1000
        Me.FOV_trackbar.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.FOV_trackbar.Value = 10
        '
        'TrackBar1
        '
        Me.TrackBar1.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.TrackBar1.Location = New System.Drawing.Point(120, 34)
        Me.TrackBar1.Maximum = 100
        Me.TrackBar1.Minimum = 10
        Me.TrackBar1.Name = "TrackBar1"
        Me.TrackBar1.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.TrackBar1.Size = New System.Drawing.Size(45, 163)
        Me.TrackBar1.TabIndex = 3
        Me.TrackBar1.TickFrequency = 1000
        Me.TrackBar1.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.TrackBar1.Value = 10
        '
        'fov_v
        '
        Me.fov_v.AutoSize = True
        Me.fov_v.BackColor = System.Drawing.Color.Transparent
        Me.fov_v.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.fov_v.ForeColor = System.Drawing.Color.White
        Me.fov_v.Location = New System.Drawing.Point(21, 200)
        Me.fov_v.Name = "fov_v"
        Me.fov_v.Size = New System.Drawing.Size(45, 13)
        Me.fov_v.TabIndex = 4
        Me.fov_v.Text = "Label3"
        Me.fov_v.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'mouse_v
        '
        Me.mouse_v.AutoSize = True
        Me.mouse_v.BackColor = System.Drawing.Color.Transparent
        Me.mouse_v.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.mouse_v.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.mouse_v.Location = New System.Drawing.Point(119, 200)
        Me.mouse_v.Name = "mouse_v"
        Me.mouse_v.Size = New System.Drawing.Size(45, 13)
        Me.mouse_v.TabIndex = 5
        Me.mouse_v.Text = "Label4"
        Me.mouse_v.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'frmSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.ClientSize = New System.Drawing.Size(188, 222)
        Me.Controls.Add(Me.mouse_v)
        Me.Controls.Add(Me.fov_v)
        Me.Controls.Add(Me.TrackBar1)
        Me.Controls.Add(Me.FOV_trackbar)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Settings"
        Me.TopMost = True
        CType(Me.FOV_trackbar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TrackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents FOV_trackbar As System.Windows.Forms.TrackBar
    Friend WithEvents TrackBar1 As System.Windows.Forms.TrackBar
    Friend WithEvents fov_v As System.Windows.Forms.Label
    Friend WithEvents mouse_v As System.Windows.Forms.Label
End Class
