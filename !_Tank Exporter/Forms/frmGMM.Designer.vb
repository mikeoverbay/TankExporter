<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmGMM
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmGMM))
        Me.red_value_tb = New System.Windows.Forms.TextBox()
        Me.red_slider = New System.Windows.Forms.TrackBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.blue_slider = New System.Windows.Forms.TrackBar()
        Me.blue_value_tb = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        CType(Me.red_slider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.blue_slider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'red_value_tb
        '
        Me.red_value_tb.BackColor = System.Drawing.Color.DimGray
        Me.red_value_tb.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.red_value_tb.ForeColor = System.Drawing.Color.White
        Me.red_value_tb.Location = New System.Drawing.Point(12, 6)
        Me.red_value_tb.Name = "red_value_tb"
        Me.red_value_tb.Size = New System.Drawing.Size(46, 13)
        Me.red_value_tb.TabIndex = 1
        Me.red_value_tb.Text = "0"
        Me.red_value_tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'red_slider
        '
        Me.red_slider.Location = New System.Drawing.Point(12, 25)
        Me.red_slider.Maximum = 78
        Me.red_slider.Name = "red_slider"
        Me.red_slider.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.red_slider.Size = New System.Drawing.Size(45, 207)
        Me.red_slider.TabIndex = 2
        Me.red_slider.TickFrequency = 2500
        Me.red_slider.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.red_slider.Value = 30
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(4, 235)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Roughness"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(72, 235)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Metalness"
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'blue_slider
        '
        Me.blue_slider.Location = New System.Drawing.Point(78, 25)
        Me.blue_slider.Maximum = 50
        Me.blue_slider.Name = "blue_slider"
        Me.blue_slider.Orientation = System.Windows.Forms.Orientation.Vertical
        Me.blue_slider.Size = New System.Drawing.Size(45, 207)
        Me.blue_slider.TabIndex = 5
        Me.blue_slider.TickFrequency = 2512
        Me.blue_slider.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.blue_slider.Value = 30
        '
        'blue_value_tb
        '
        Me.blue_value_tb.BackColor = System.Drawing.Color.DimGray
        Me.blue_value_tb.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.blue_value_tb.ForeColor = System.Drawing.Color.White
        Me.blue_value_tb.Location = New System.Drawing.Point(78, 6)
        Me.blue_value_tb.Name = "blue_value_tb"
        Me.blue_value_tb.Size = New System.Drawing.Size(46, 13)
        Me.blue_value_tb.TabIndex = 4
        Me.blue_value_tb.Text = "0"
        Me.blue_value_tb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(4, 252)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(127, 65)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "NOTE:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Roughness is inverted in" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "the shader." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Higher values means less" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "roughness" & _
    "!"
        '
        'frmGMM
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.DimGray
        Me.ClientSize = New System.Drawing.Size(142, 322)
        Me.ControlBox = False
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.blue_slider)
        Me.Controls.Add(Me.blue_value_tb)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.red_slider)
        Me.Controls.Add(Me.red_value_tb)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmGMM"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "GMM Toy"
        Me.TopMost = True
        CType(Me.red_slider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.blue_slider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents red_value_tb As System.Windows.Forms.TextBox
    Friend WithEvents red_slider As System.Windows.Forms.TrackBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents blue_slider As System.Windows.Forms.TrackBar
    Friend WithEvents blue_value_tb As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class
