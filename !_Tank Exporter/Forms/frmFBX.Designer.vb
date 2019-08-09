<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmFBX
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmFBX))
        Me.flip_u = New System.Windows.Forms.CheckBox()
        Me.export_textures = New System.Windows.Forms.CheckBox()
        Me.flip_v = New System.Windows.Forms.CheckBox()
        Me.Start_Export_btn = New System.Windows.Forms.Button()
        Me.Cancel_bnt = New System.Windows.Forms.Button()
        Me.export_as_binary_cb = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.blender_cb = New System.Windows.Forms.CheckBox()
        Me.texture_per_model_cb = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.convert_normal_maps_cb = New System.Windows.Forms.CheckBox()
        Me.flip_normal_cb = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'flip_u
        '
        Me.flip_u.AutoSize = True
        Me.flip_u.BackColor = System.Drawing.Color.Transparent
        Me.flip_u.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.flip_u.ForeColor = System.Drawing.Color.White
        Me.flip_u.Location = New System.Drawing.Point(12, 74)
        Me.flip_u.Name = "flip_u"
        Me.flip_u.Size = New System.Drawing.Size(59, 17)
        Me.flip_u.TabIndex = 2
        Me.flip_u.Text = "Flip U"
        Me.flip_u.UseVisualStyleBackColor = False
        '
        'export_textures
        '
        Me.export_textures.AutoSize = True
        Me.export_textures.BackColor = System.Drawing.Color.Transparent
        Me.export_textures.Checked = True
        Me.export_textures.CheckState = System.Windows.Forms.CheckState.Checked
        Me.export_textures.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.export_textures.ForeColor = System.Drawing.Color.White
        Me.export_textures.Location = New System.Drawing.Point(12, 166)
        Me.export_textures.Name = "export_textures"
        Me.export_textures.Size = New System.Drawing.Size(115, 17)
        Me.export_textures.TabIndex = 1
        Me.export_textures.Text = "Export Textures"
        Me.export_textures.UseVisualStyleBackColor = False
        '
        'flip_v
        '
        Me.flip_v.AutoSize = True
        Me.flip_v.BackColor = System.Drawing.Color.Transparent
        Me.flip_v.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.flip_v.ForeColor = System.Drawing.Color.White
        Me.flip_v.Location = New System.Drawing.Point(12, 97)
        Me.flip_v.Name = "flip_v"
        Me.flip_v.Size = New System.Drawing.Size(58, 17)
        Me.flip_v.TabIndex = 0
        Me.flip_v.Text = "Flip V"
        Me.flip_v.UseVisualStyleBackColor = False
        '
        'Start_Export_btn
        '
        Me.Start_Export_btn.ForeColor = System.Drawing.Color.Black
        Me.Start_Export_btn.Location = New System.Drawing.Point(108, 346)
        Me.Start_Export_btn.Name = "Start_Export_btn"
        Me.Start_Export_btn.Size = New System.Drawing.Size(75, 23)
        Me.Start_Export_btn.TabIndex = 1
        Me.Start_Export_btn.Text = "Start Export"
        Me.Start_Export_btn.UseVisualStyleBackColor = True
        '
        'Cancel_bnt
        '
        Me.Cancel_bnt.ForeColor = System.Drawing.Color.Black
        Me.Cancel_bnt.Location = New System.Drawing.Point(12, 346)
        Me.Cancel_bnt.Name = "Cancel_bnt"
        Me.Cancel_bnt.Size = New System.Drawing.Size(75, 23)
        Me.Cancel_bnt.TabIndex = 2
        Me.Cancel_bnt.Text = "Cancel"
        Me.Cancel_bnt.UseVisualStyleBackColor = True
        '
        'export_as_binary_cb
        '
        Me.export_as_binary_cb.AutoSize = True
        Me.export_as_binary_cb.BackColor = System.Drawing.Color.Transparent
        Me.export_as_binary_cb.Checked = True
        Me.export_as_binary_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.export_as_binary_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.export_as_binary_cb.ForeColor = System.Drawing.Color.White
        Me.export_as_binary_cb.Location = New System.Drawing.Point(12, 189)
        Me.export_as_binary_cb.Name = "export_as_binary_cb"
        Me.export_as_binary_cb.Size = New System.Drawing.Size(150, 17)
        Me.export_as_binary_cb.TabIndex = 4
        Me.export_as_binary_cb.Text = "Create Binary FBX file"
        Me.export_as_binary_cb.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(243, 36)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "This exporter uses Fbx Sdk 2009.1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "To convert to other versions, Find" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "AutoDesk's" & _
    " FBX Converter"
        '
        'blender_cb
        '
        Me.blender_cb.AutoSize = True
        Me.blender_cb.BackColor = System.Drawing.Color.Transparent
        Me.blender_cb.Checked = Global.Tank_Exporter.My.MySettings.Default.blender_compatible
        Me.blender_cb.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.Tank_Exporter.My.MySettings.Default, "blender_compatible", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.blender_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.blender_cb.ForeColor = System.Drawing.Color.Yellow
        Me.blender_cb.Location = New System.Drawing.Point(12, 212)
        Me.blender_cb.Name = "blender_cb"
        Me.blender_cb.Size = New System.Drawing.Size(162, 17)
        Me.blender_cb.TabIndex = 6
        Me.blender_cb.Text = "Blender Compatible FBX"
        Me.blender_cb.UseVisualStyleBackColor = False
        '
        'texture_per_model_cb
        '
        Me.texture_per_model_cb.AutoSize = True
        Me.texture_per_model_cb.BackColor = System.Drawing.Color.Transparent
        Me.texture_per_model_cb.Checked = Global.Tank_Exporter.My.MySettings.Default.blender_compatible
        Me.texture_per_model_cb.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.Tank_Exporter.My.MySettings.Default, "blender_compatible", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.texture_per_model_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.texture_per_model_cb.ForeColor = System.Drawing.Color.Yellow
        Me.texture_per_model_cb.Location = New System.Drawing.Point(12, 235)
        Me.texture_per_model_cb.Name = "texture_per_model_cb"
        Me.texture_per_model_cb.Size = New System.Drawing.Size(159, 17)
        Me.texture_per_model_cb.TabIndex = 7
        Me.texture_per_model_cb.Text = "Split Textures by Model"
        Me.texture_per_model_cb.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.BackColor = System.Drawing.Color.Transparent
        Me.Label2.Font = New System.Drawing.Font("Lucida Console", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Label2.Location = New System.Drawing.Point(9, 266)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(278, 72)
        Me.Label2.TabIndex = 8
        Me.Label2.Text = resources.GetString("Label2.Text")
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'convert_normal_maps_cb
        '
        Me.convert_normal_maps_cb.AutoSize = True
        Me.convert_normal_maps_cb.BackColor = System.Drawing.Color.Transparent
        Me.convert_normal_maps_cb.Checked = True
        Me.convert_normal_maps_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.convert_normal_maps_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.convert_normal_maps_cb.ForeColor = System.Drawing.Color.White
        Me.convert_normal_maps_cb.Location = New System.Drawing.Point(12, 120)
        Me.convert_normal_maps_cb.Name = "convert_normal_maps_cb"
        Me.convert_normal_maps_cb.Size = New System.Drawing.Size(147, 17)
        Me.convert_normal_maps_cb.TabIndex = 9
        Me.convert_normal_maps_cb.Text = "Convert Normal Maps"
        Me.convert_normal_maps_cb.UseVisualStyleBackColor = False
        '
        'flip_normal_cb
        '
        Me.flip_normal_cb.AutoSize = True
        Me.flip_normal_cb.BackColor = System.Drawing.Color.Transparent
        Me.flip_normal_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.flip_normal_cb.ForeColor = System.Drawing.Color.White
        Me.flip_normal_cb.Location = New System.Drawing.Point(12, 143)
        Me.flip_normal_cb.Name = "flip_normal_cb"
        Me.flip_normal_cb.Size = New System.Drawing.Size(101, 17)
        Me.flip_normal_cb.TabIndex = 10
        Me.flip_normal_cb.Text = "Flip Y Normal"
        Me.flip_normal_cb.UseVisualStyleBackColor = False
        '
        'frmFBX
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.upton_BnW
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(305, 381)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.flip_normal_cb)
        Me.Controls.Add(Me.convert_normal_maps_cb)
        Me.Controls.Add(Me.texture_per_model_cb)
        Me.Controls.Add(Me.blender_cb)
        Me.Controls.Add(Me.flip_u)
        Me.Controls.Add(Me.export_textures)
        Me.Controls.Add(Me.flip_v)
        Me.Controls.Add(Me.export_as_binary_cb)
        Me.Controls.Add(Me.Cancel_bnt)
        Me.Controls.Add(Me.Start_Export_btn)
        Me.DoubleBuffered = True
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmFBX"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "FBX Xporter"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents flip_u As System.Windows.Forms.CheckBox
    Friend WithEvents export_textures As System.Windows.Forms.CheckBox
    Friend WithEvents flip_v As System.Windows.Forms.CheckBox
    Friend WithEvents Start_Export_btn As System.Windows.Forms.Button
    Friend WithEvents Cancel_bnt As System.Windows.Forms.Button
    Friend WithEvents export_as_binary_cb As System.Windows.Forms.CheckBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents blender_cb As System.Windows.Forms.CheckBox
    Friend WithEvents texture_per_model_cb As System.Windows.Forms.CheckBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents convert_normal_maps_cb As System.Windows.Forms.CheckBox
    Friend WithEvents flip_normal_cb As System.Windows.Forms.CheckBox
End Class
