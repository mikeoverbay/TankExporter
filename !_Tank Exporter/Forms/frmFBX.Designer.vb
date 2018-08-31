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
        Me.flip_u = New System.Windows.Forms.CheckBox()
        Me.export_textures = New System.Windows.Forms.CheckBox()
        Me.flip_v = New System.Windows.Forms.CheckBox()
        Me.Start_Export_btn = New System.Windows.Forms.Button()
        Me.Cancel_bnt = New System.Windows.Forms.Button()
        Me.export_as_binary_cb = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'flip_u
        '
        Me.flip_u.AutoSize = True
        Me.flip_u.Location = New System.Drawing.Point(12, 87)
        Me.flip_u.Name = "flip_u"
        Me.flip_u.Size = New System.Drawing.Size(53, 17)
        Me.flip_u.TabIndex = 2
        Me.flip_u.Text = "Flip U"
        Me.flip_u.UseVisualStyleBackColor = True
        '
        'export_textures
        '
        Me.export_textures.AutoSize = True
        Me.export_textures.Checked = True
        Me.export_textures.CheckState = System.Windows.Forms.CheckState.Checked
        Me.export_textures.Location = New System.Drawing.Point(12, 133)
        Me.export_textures.Name = "export_textures"
        Me.export_textures.Size = New System.Drawing.Size(100, 17)
        Me.export_textures.TabIndex = 1
        Me.export_textures.Text = "Export Textures"
        Me.export_textures.UseVisualStyleBackColor = True
        '
        'flip_v
        '
        Me.flip_v.AutoSize = True
        Me.flip_v.Location = New System.Drawing.Point(12, 110)
        Me.flip_v.Name = "flip_v"
        Me.flip_v.Size = New System.Drawing.Size(52, 17)
        Me.flip_v.TabIndex = 0
        Me.flip_v.Text = "Flip V"
        Me.flip_v.UseVisualStyleBackColor = True
        '
        'Start_Export_btn
        '
        Me.Start_Export_btn.ForeColor = System.Drawing.Color.Black
        Me.Start_Export_btn.Location = New System.Drawing.Point(98, 192)
        Me.Start_Export_btn.Name = "Start_Export_btn"
        Me.Start_Export_btn.Size = New System.Drawing.Size(75, 23)
        Me.Start_Export_btn.TabIndex = 1
        Me.Start_Export_btn.Text = "Start Export"
        Me.Start_Export_btn.UseVisualStyleBackColor = True
        '
        'Cancel_bnt
        '
        Me.Cancel_bnt.ForeColor = System.Drawing.Color.Black
        Me.Cancel_bnt.Location = New System.Drawing.Point(12, 192)
        Me.Cancel_bnt.Name = "Cancel_bnt"
        Me.Cancel_bnt.Size = New System.Drawing.Size(75, 23)
        Me.Cancel_bnt.TabIndex = 2
        Me.Cancel_bnt.Text = "Cancel"
        Me.Cancel_bnt.UseVisualStyleBackColor = True
        '
        'export_as_binary_cb
        '
        Me.export_as_binary_cb.AutoSize = True
        Me.export_as_binary_cb.Checked = True
        Me.export_as_binary_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.export_as_binary_cb.Location = New System.Drawing.Point(12, 162)
        Me.export_as_binary_cb.Name = "export_as_binary_cb"
        Me.export_as_binary_cb.Size = New System.Drawing.Size(128, 17)
        Me.export_as_binary_cb.TabIndex = 4
        Me.export_as_binary_cb.Text = "Create Binary FBX file"
        Me.export_as_binary_cb.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(128, Byte), Integer))
        Me.Label1.Location = New System.Drawing.Point(9, 32)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(171, 39)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "This exporter uses Fbx Sdk 2009.1" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "To convert to other versions, Find" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "AutoDesk's" & _
    " FBX Converter"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.Yellow
        Me.Label2.Location = New System.Drawing.Point(9, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "This is a test"
        Me.Label2.Visible = False
        '
        'frmFBX
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(185, 232)
        Me.Controls.Add(Me.flip_u)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.export_textures)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.flip_v)
        Me.Controls.Add(Me.export_as_binary_cb)
        Me.Controls.Add(Me.Cancel_bnt)
        Me.Controls.Add(Me.Start_Export_btn)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
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
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
