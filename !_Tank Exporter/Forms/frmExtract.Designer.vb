<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmExtract
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmExtract))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.no_models = New System.Windows.Forms.RadioButton()
        Me.lod_0_only = New System.Windows.Forms.RadioButton()
        Me.all_lods_rb = New System.Windows.Forms.RadioButton()
        Me.TT = New System.Windows.Forms.ToolTip(Me.components)
        Me.create_work_area_cb = New System.Windows.Forms.CheckBox()
        Me.m_customization = New System.Windows.Forms.CheckBox()
        Me.ext_chassis = New System.Windows.Forms.CheckBox()
        Me.ext_hull = New System.Windows.Forms.CheckBox()
        Me.ext_turret = New System.Windows.Forms.CheckBox()
        Me.ext_gun = New System.Windows.Forms.CheckBox()
        Me.extract_btn = New System.Windows.Forms.Button()
        Me.cancel_btn = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.no_models)
        Me.GroupBox1.Controls.Add(Me.lod_0_only)
        Me.GroupBox1.Controls.Add(Me.all_lods_rb)
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(131, 93)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "LODs"
        '
        'no_models
        '
        Me.no_models.AutoSize = True
        Me.no_models.Location = New System.Drawing.Point(15, 65)
        Me.no_models.Name = "no_models"
        Me.no_models.Size = New System.Drawing.Size(112, 17)
        Me.no_models.TabIndex = 2
        Me.no_models.Text = "Extract No Models"
        Me.no_models.UseVisualStyleBackColor = True
        '
        'lod_0_only
        '
        Me.lod_0_only.AutoSize = True
        Me.lod_0_only.Checked = True
        Me.lod_0_only.Location = New System.Drawing.Point(15, 42)
        Me.lod_0_only.Name = "lod_0_only"
        Me.lod_0_only.Size = New System.Drawing.Size(104, 17)
        Me.lod_0_only.TabIndex = 1
        Me.lod_0_only.TabStop = True
        Me.lod_0_only.Text = "LOD ZERO Only"
        Me.lod_0_only.UseVisualStyleBackColor = True
        '
        'all_lods_rb
        '
        Me.all_lods_rb.AutoSize = True
        Me.all_lods_rb.Location = New System.Drawing.Point(15, 19)
        Me.all_lods_rb.Name = "all_lods_rb"
        Me.all_lods_rb.Size = New System.Drawing.Size(111, 17)
        Me.all_lods_rb.TabIndex = 0
        Me.all_lods_rb.Text = "All available LODs"
        Me.all_lods_rb.UseVisualStyleBackColor = True
        '
        'create_work_area_cb
        '
        Me.create_work_area_cb.AutoSize = True
        Me.create_work_area_cb.ForeColor = System.Drawing.Color.Red
        Me.create_work_area_cb.Location = New System.Drawing.Point(27, 209)
        Me.create_work_area_cb.Name = "create_work_area_cb"
        Me.create_work_area_cb.Size = New System.Drawing.Size(111, 17)
        Me.create_work_area_cb.TabIndex = 7
        Me.create_work_area_cb.Text = "Create Work Area"
        Me.TT.SetToolTip(Me.create_work_area_cb, "Creates a fold called Work Area in the tanks root folder." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "It copies the AM map t" & _
        "o this folder as a PNG for editing.")
        Me.create_work_area_cb.UseVisualStyleBackColor = True
        '
        'm_customization
        '
        Me.m_customization.AutoSize = True
        Me.m_customization.ForeColor = System.Drawing.Color.Red
        Me.m_customization.Location = New System.Drawing.Point(27, 232)
        Me.m_customization.Name = "m_customization"
        Me.m_customization.Size = New System.Drawing.Size(109, 17)
        Me.m_customization.TabIndex = 8
        Me.m_customization.Text = "Customization.xml"
        Me.TT.SetToolTip(Me.m_customization, "Extracts customization.xml if checked.")
        Me.m_customization.UseVisualStyleBackColor = True
        '
        'ext_chassis
        '
        Me.ext_chassis.AutoSize = True
        Me.ext_chassis.Checked = True
        Me.ext_chassis.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ext_chassis.Location = New System.Drawing.Point(27, 117)
        Me.ext_chassis.Name = "ext_chassis"
        Me.ext_chassis.Size = New System.Drawing.Size(88, 17)
        Me.ext_chassis.TabIndex = 1
        Me.ext_chassis.Text = "Chassis Data"
        Me.ext_chassis.UseVisualStyleBackColor = True
        '
        'ext_hull
        '
        Me.ext_hull.AutoSize = True
        Me.ext_hull.Checked = True
        Me.ext_hull.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ext_hull.Location = New System.Drawing.Point(27, 140)
        Me.ext_hull.Name = "ext_hull"
        Me.ext_hull.Size = New System.Drawing.Size(70, 17)
        Me.ext_hull.TabIndex = 2
        Me.ext_hull.Text = "Hull Data"
        Me.ext_hull.UseVisualStyleBackColor = True
        '
        'ext_turret
        '
        Me.ext_turret.AutoSize = True
        Me.ext_turret.Checked = True
        Me.ext_turret.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ext_turret.Location = New System.Drawing.Point(27, 163)
        Me.ext_turret.Name = "ext_turret"
        Me.ext_turret.Size = New System.Drawing.Size(78, 17)
        Me.ext_turret.TabIndex = 3
        Me.ext_turret.Text = "Turret data"
        Me.ext_turret.UseVisualStyleBackColor = True
        '
        'ext_gun
        '
        Me.ext_gun.AutoSize = True
        Me.ext_gun.Checked = True
        Me.ext_gun.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ext_gun.Location = New System.Drawing.Point(27, 186)
        Me.ext_gun.Name = "ext_gun"
        Me.ext_gun.Size = New System.Drawing.Size(70, 17)
        Me.ext_gun.TabIndex = 4
        Me.ext_gun.Text = "Gun data"
        Me.ext_gun.UseVisualStyleBackColor = True
        '
        'extract_btn
        '
        Me.extract_btn.ForeColor = System.Drawing.Color.Black
        Me.extract_btn.Location = New System.Drawing.Point(12, 254)
        Me.extract_btn.Name = "extract_btn"
        Me.extract_btn.Size = New System.Drawing.Size(53, 23)
        Me.extract_btn.TabIndex = 5
        Me.extract_btn.Text = "Extract"
        Me.extract_btn.UseVisualStyleBackColor = True
        '
        'cancel_btn
        '
        Me.cancel_btn.ForeColor = System.Drawing.Color.Black
        Me.cancel_btn.Location = New System.Drawing.Point(92, 254)
        Me.cancel_btn.Name = "cancel_btn"
        Me.cancel_btn.Size = New System.Drawing.Size(52, 23)
        Me.cancel_btn.TabIndex = 6
        Me.cancel_btn.Text = "Cancel"
        Me.cancel_btn.UseVisualStyleBackColor = True
        '
        'frmExtract
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(156, 286)
        Me.Controls.Add(Me.m_customization)
        Me.Controls.Add(Me.create_work_area_cb)
        Me.Controls.Add(Me.cancel_btn)
        Me.Controls.Add(Me.extract_btn)
        Me.Controls.Add(Me.ext_gun)
        Me.Controls.Add(Me.ext_turret)
        Me.Controls.Add(Me.ext_hull)
        Me.Controls.Add(Me.ext_chassis)
        Me.Controls.Add(Me.GroupBox1)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmExtract"
        Me.ShowInTaskbar = False
        Me.Text = "Extraction Settings"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lod_0_only As System.Windows.Forms.RadioButton
    Friend WithEvents all_lods_rb As System.Windows.Forms.RadioButton
    Friend WithEvents TT As System.Windows.Forms.ToolTip
    Friend WithEvents no_models As System.Windows.Forms.RadioButton
    Friend WithEvents ext_chassis As System.Windows.Forms.CheckBox
    Friend WithEvents ext_hull As System.Windows.Forms.CheckBox
    Friend WithEvents ext_turret As System.Windows.Forms.CheckBox
    Friend WithEvents ext_gun As System.Windows.Forms.CheckBox
    Friend WithEvents extract_btn As System.Windows.Forms.Button
    Friend WithEvents cancel_btn As System.Windows.Forms.Button
    Friend WithEvents create_work_area_cb As System.Windows.Forms.CheckBox
    Friend WithEvents m_customization As System.Windows.Forms.CheckBox
End Class
