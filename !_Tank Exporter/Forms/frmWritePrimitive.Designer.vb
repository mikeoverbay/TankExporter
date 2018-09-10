<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmWritePrimitive
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmWritePrimitive))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cew_cb = New System.Windows.Forms.CheckBox()
        Me.hew_cb = New System.Windows.Forms.CheckBox()
        Me.tew_cb = New System.Windows.Forms.CheckBox()
        Me.gew_cb = New System.Windows.Forms.CheckBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.flipWindingOrder_cb = New System.Windows.Forms.CheckBox()
        Me.hide_tracks_cb = New System.Windows.Forms.CheckBox()
        Me.copy_lods_cb = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(99, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Modified Groups"
        '
        'cew_cb
        '
        Me.cew_cb.AutoSize = True
        Me.cew_cb.BackColor = System.Drawing.Color.Transparent
        Me.cew_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cew_cb.ForeColor = System.Drawing.Color.Black
        Me.cew_cb.Location = New System.Drawing.Point(15, 34)
        Me.cew_cb.Name = "cew_cb"
        Me.cew_cb.Size = New System.Drawing.Size(69, 17)
        Me.cew_cb.TabIndex = 1
        Me.cew_cb.Text = "Chassis"
        Me.cew_cb.UseVisualStyleBackColor = False
        '
        'hew_cb
        '
        Me.hew_cb.AutoSize = True
        Me.hew_cb.BackColor = System.Drawing.Color.Transparent
        Me.hew_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.hew_cb.ForeColor = System.Drawing.Color.Black
        Me.hew_cb.Location = New System.Drawing.Point(15, 57)
        Me.hew_cb.Name = "hew_cb"
        Me.hew_cb.Size = New System.Drawing.Size(48, 17)
        Me.hew_cb.TabIndex = 2
        Me.hew_cb.Text = "Hull"
        Me.hew_cb.UseVisualStyleBackColor = False
        '
        'tew_cb
        '
        Me.tew_cb.AutoSize = True
        Me.tew_cb.BackColor = System.Drawing.Color.Transparent
        Me.tew_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tew_cb.ForeColor = System.Drawing.Color.Black
        Me.tew_cb.Location = New System.Drawing.Point(15, 80)
        Me.tew_cb.Name = "tew_cb"
        Me.tew_cb.Size = New System.Drawing.Size(60, 17)
        Me.tew_cb.TabIndex = 3
        Me.tew_cb.Text = "Turret"
        Me.tew_cb.UseVisualStyleBackColor = False
        '
        'gew_cb
        '
        Me.gew_cb.AutoSize = True
        Me.gew_cb.BackColor = System.Drawing.Color.Transparent
        Me.gew_cb.Enabled = False
        Me.gew_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.gew_cb.ForeColor = System.Drawing.Color.Black
        Me.gew_cb.Location = New System.Drawing.Point(15, 103)
        Me.gew_cb.Name = "gew_cb"
        Me.gew_cb.Size = New System.Drawing.Size(49, 17)
        Me.gew_cb.TabIndex = 4
        Me.gew_cb.Text = "Gun"
        Me.gew_cb.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.ForeColor = System.Drawing.Color.Black
        Me.Button1.Location = New System.Drawing.Point(180, 189)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "Write File(s)"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'flipWindingOrder_cb
        '
        Me.flipWindingOrder_cb.AutoSize = True
        Me.flipWindingOrder_cb.BackColor = System.Drawing.Color.Transparent
        Me.flipWindingOrder_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.flipWindingOrder_cb.ForeColor = System.Drawing.Color.Black
        Me.flipWindingOrder_cb.Location = New System.Drawing.Point(15, 135)
        Me.flipWindingOrder_cb.Name = "flipWindingOrder_cb"
        Me.flipWindingOrder_cb.Size = New System.Drawing.Size(131, 17)
        Me.flipWindingOrder_cb.TabIndex = 6
        Me.flipWindingOrder_cb.Text = "Flip Winding Order"
        Me.flipWindingOrder_cb.UseVisualStyleBackColor = False
        '
        'hide_tracks_cb
        '
        Me.hide_tracks_cb.AutoSize = True
        Me.hide_tracks_cb.BackColor = System.Drawing.Color.Transparent
        Me.hide_tracks_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.hide_tracks_cb.ForeColor = System.Drawing.Color.White
        Me.hide_tracks_cb.Location = New System.Drawing.Point(15, 172)
        Me.hide_tracks_cb.Name = "hide_tracks_cb"
        Me.hide_tracks_cb.Size = New System.Drawing.Size(160, 17)
        Me.hide_tracks_cb.TabIndex = 7
        Me.hide_tracks_cb.Text = "Hide Tracks on Chassis"
        Me.hide_tracks_cb.UseVisualStyleBackColor = False
        '
        'copy_lods_cb
        '
        Me.copy_lods_cb.AutoSize = True
        Me.copy_lods_cb.BackColor = System.Drawing.Color.Transparent
        Me.copy_lods_cb.Checked = True
        Me.copy_lods_cb.CheckState = System.Windows.Forms.CheckState.Checked
        Me.copy_lods_cb.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.copy_lods_cb.ForeColor = System.Drawing.Color.White
        Me.copy_lods_cb.Location = New System.Drawing.Point(15, 195)
        Me.copy_lods_cb.Name = "copy_lods_cb"
        Me.copy_lods_cb.Size = New System.Drawing.Size(126, 17)
        Me.copy_lods_cb.TabIndex = 8
        Me.copy_lods_cb.Text = "Copy to LODs 1-4"
        Me.copy_lods_cb.UseVisualStyleBackColor = False
        '
        'frmWritePrimitive
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.upton
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ClientSize = New System.Drawing.Size(256, 214)
        Me.Controls.Add(Me.copy_lods_cb)
        Me.Controls.Add(Me.hide_tracks_cb)
        Me.Controls.Add(Me.flipWindingOrder_cb)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.gew_cb)
        Me.Controls.Add(Me.tew_cb)
        Me.Controls.Add(Me.hew_cb)
        Me.Controls.Add(Me.cew_cb)
        Me.Controls.Add(Me.Label1)
        Me.ForeColor = System.Drawing.Color.Gainsboro
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmWritePrimitive"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Primitive Writer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cew_cb As System.Windows.Forms.CheckBox
    Friend WithEvents hew_cb As System.Windows.Forms.CheckBox
    Friend WithEvents tew_cb As System.Windows.Forms.CheckBox
    Friend WithEvents gew_cb As System.Windows.Forms.CheckBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Friend WithEvents flipWindingOrder_cb As System.Windows.Forms.CheckBox
    Friend WithEvents hide_tracks_cb As System.Windows.Forms.CheckBox
    Friend WithEvents copy_lods_cb As System.Windows.Forms.CheckBox
End Class
