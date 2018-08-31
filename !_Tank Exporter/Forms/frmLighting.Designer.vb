<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLighting
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmLighting))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.total_slider = New System.Windows.Forms.TrackBar()
        Me.specular_slider = New System.Windows.Forms.TrackBar()
        Me.ambient_slider = New System.Windows.Forms.TrackBar()
        CType(Me.total_slider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.specular_slider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ambient_slider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(20, 11)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Ambient"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(20, 83)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(56, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Brightness"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(20, 47)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(49, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "Specular"
        '
        'total_slider
        '
        Me.total_slider.AutoSize = False
        Me.total_slider.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Tank_Exporter.My.MySettings.Default, "total_value", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.total_slider.Location = New System.Drawing.Point(12, 83)
        Me.total_slider.Maximum = 100
        Me.total_slider.Name = "total_slider"
        Me.total_slider.Size = New System.Drawing.Size(212, 30)
        Me.total_slider.TabIndex = 3
        Me.total_slider.TickFrequency = 10
        Me.total_slider.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.total_slider.Value = Global.Tank_Exporter.My.MySettings.Default.total_value
        '
        'specular_slider
        '
        Me.specular_slider.AutoSize = False
        Me.specular_slider.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Tank_Exporter.My.MySettings.Default, "sepcular_value", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.specular_slider.Location = New System.Drawing.Point(12, 47)
        Me.specular_slider.Maximum = 100
        Me.specular_slider.Name = "specular_slider"
        Me.specular_slider.Size = New System.Drawing.Size(212, 30)
        Me.specular_slider.TabIndex = 2
        Me.specular_slider.TickFrequency = 10
        Me.specular_slider.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.specular_slider.Value = Global.Tank_Exporter.My.MySettings.Default.sepcular_value
        '
        'ambient_slider
        '
        Me.ambient_slider.AutoSize = False
        Me.ambient_slider.DataBindings.Add(New System.Windows.Forms.Binding("Value", Global.Tank_Exporter.My.MySettings.Default, "ambient_value", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ambient_slider.Location = New System.Drawing.Point(12, 11)
        Me.ambient_slider.Maximum = 100
        Me.ambient_slider.Name = "ambient_slider"
        Me.ambient_slider.Size = New System.Drawing.Size(212, 30)
        Me.ambient_slider.TabIndex = 0
        Me.ambient_slider.TickFrequency = 10
        Me.ambient_slider.TickStyle = System.Windows.Forms.TickStyle.Both
        Me.ambient_slider.Value = Global.Tank_Exporter.My.MySettings.Default.ambient_value
        '
        'frmLighting
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(241, 129)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.total_slider)
        Me.Controls.Add(Me.specular_slider)
        Me.Controls.Add(Me.ambient_slider)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmLighting"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Lighting"
        Me.TopMost = True
        CType(Me.total_slider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.specular_slider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ambient_slider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ambient_slider As System.Windows.Forms.TrackBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents specular_slider As System.Windows.Forms.TrackBar
    Friend WithEvents total_slider As System.Windows.Forms.TrackBar
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
End Class
