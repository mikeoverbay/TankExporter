<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmShaderDebugSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmShaderDebugSettings))
        Me.ScaleFGDSpec = New System.Windows.Forms.GroupBox()
        Me.specContrib = New System.Windows.Forms.RadioButton()
        Me.D = New System.Windows.Forms.RadioButton()
        Me.G = New System.Windows.Forms.RadioButton()
        Me.F = New System.Windows.Forms.RadioButton()
        Me.a_normal = New System.Windows.Forms.RadioButton()
        Me.ScaleDiffBaseMR = New System.Windows.Forms.GroupBox()
        Me.Roughness = New System.Windows.Forms.RadioButton()
        Me.Metallic = New System.Windows.Forms.RadioButton()
        Me.baseColor = New System.Windows.Forms.RadioButton()
        Me.diffuseContrib = New System.Windows.Forms.RadioButton()
        Me.b_normal = New System.Windows.Forms.RadioButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ScaleFGDSpec.SuspendLayout()
        Me.ScaleDiffBaseMR.SuspendLayout()
        Me.SuspendLayout()
        '
        'ScaleFGDSpec
        '
        Me.ScaleFGDSpec.Controls.Add(Me.specContrib)
        Me.ScaleFGDSpec.Controls.Add(Me.D)
        Me.ScaleFGDSpec.Controls.Add(Me.G)
        Me.ScaleFGDSpec.Controls.Add(Me.F)
        Me.ScaleFGDSpec.Controls.Add(Me.a_normal)
        Me.ScaleFGDSpec.ForeColor = System.Drawing.Color.White
        Me.ScaleFGDSpec.Location = New System.Drawing.Point(12, 73)
        Me.ScaleFGDSpec.Name = "ScaleFGDSpec"
        Me.ScaleFGDSpec.Size = New System.Drawing.Size(106, 150)
        Me.ScaleFGDSpec.TabIndex = 0
        Me.ScaleFGDSpec.TabStop = False
        Me.ScaleFGDSpec.Text = "ScaleFGDSpec"
        '
        'specContrib
        '
        Me.specContrib.AutoSize = True
        Me.specContrib.Location = New System.Drawing.Point(7, 122)
        Me.specContrib.Name = "specContrib"
        Me.specContrib.Size = New System.Drawing.Size(81, 17)
        Me.specContrib.TabIndex = 4
        Me.specContrib.Tag = "8"
        Me.specContrib.Text = "specContrib"
        Me.specContrib.UseVisualStyleBackColor = True
        '
        'D
        '
        Me.D.AutoSize = True
        Me.D.Location = New System.Drawing.Point(7, 99)
        Me.D.Name = "D"
        Me.D.Size = New System.Drawing.Size(33, 17)
        Me.D.TabIndex = 3
        Me.D.Tag = "4"
        Me.D.Text = "D"
        Me.D.UseVisualStyleBackColor = True
        '
        'G
        '
        Me.G.AutoSize = True
        Me.G.Location = New System.Drawing.Point(7, 76)
        Me.G.Name = "G"
        Me.G.Size = New System.Drawing.Size(33, 17)
        Me.G.TabIndex = 2
        Me.G.Tag = "2"
        Me.G.Text = "G"
        Me.G.UseVisualStyleBackColor = True
        '
        'F
        '
        Me.F.AutoSize = True
        Me.F.Location = New System.Drawing.Point(7, 53)
        Me.F.Name = "F"
        Me.F.Size = New System.Drawing.Size(31, 17)
        Me.F.TabIndex = 1
        Me.F.Tag = "1"
        Me.F.Text = "F"
        Me.F.UseVisualStyleBackColor = True
        '
        'a_normal
        '
        Me.a_normal.AutoSize = True
        Me.a_normal.Checked = True
        Me.a_normal.Location = New System.Drawing.Point(7, 20)
        Me.a_normal.Name = "a_normal"
        Me.a_normal.Size = New System.Drawing.Size(58, 17)
        Me.a_normal.TabIndex = 0
        Me.a_normal.TabStop = True
        Me.a_normal.Text = "Normal"
        Me.a_normal.UseVisualStyleBackColor = True
        '
        'ScaleDiffBaseMR
        '
        Me.ScaleDiffBaseMR.Controls.Add(Me.Roughness)
        Me.ScaleDiffBaseMR.Controls.Add(Me.Metallic)
        Me.ScaleDiffBaseMR.Controls.Add(Me.baseColor)
        Me.ScaleDiffBaseMR.Controls.Add(Me.diffuseContrib)
        Me.ScaleDiffBaseMR.Controls.Add(Me.b_normal)
        Me.ScaleDiffBaseMR.ForeColor = System.Drawing.Color.White
        Me.ScaleDiffBaseMR.Location = New System.Drawing.Point(124, 73)
        Me.ScaleDiffBaseMR.Name = "ScaleDiffBaseMR"
        Me.ScaleDiffBaseMR.Size = New System.Drawing.Size(106, 150)
        Me.ScaleDiffBaseMR.TabIndex = 1
        Me.ScaleDiffBaseMR.TabStop = False
        Me.ScaleDiffBaseMR.Text = "ScaleDiffBaseMR"
        '
        'Roughness
        '
        Me.Roughness.AutoSize = True
        Me.Roughness.Location = New System.Drawing.Point(6, 122)
        Me.Roughness.Name = "Roughness"
        Me.Roughness.Size = New System.Drawing.Size(79, 17)
        Me.Roughness.TabIndex = 5
        Me.Roughness.Tag = "8"
        Me.Roughness.Text = "Roughness"
        Me.Roughness.UseVisualStyleBackColor = True
        '
        'Metallic
        '
        Me.Metallic.AutoSize = True
        Me.Metallic.Location = New System.Drawing.Point(6, 99)
        Me.Metallic.Name = "Metallic"
        Me.Metallic.Size = New System.Drawing.Size(61, 17)
        Me.Metallic.TabIndex = 4
        Me.Metallic.Tag = "4"
        Me.Metallic.Text = "Metallic"
        Me.Metallic.UseVisualStyleBackColor = True
        '
        'baseColor
        '
        Me.baseColor.AutoSize = True
        Me.baseColor.Location = New System.Drawing.Point(6, 76)
        Me.baseColor.Name = "baseColor"
        Me.baseColor.Size = New System.Drawing.Size(72, 17)
        Me.baseColor.TabIndex = 3
        Me.baseColor.Tag = "2"
        Me.baseColor.Text = "baseColor"
        Me.baseColor.UseVisualStyleBackColor = True
        '
        'diffuseContrib
        '
        Me.diffuseContrib.AutoSize = True
        Me.diffuseContrib.Location = New System.Drawing.Point(6, 53)
        Me.diffuseContrib.Name = "diffuseContrib"
        Me.diffuseContrib.Size = New System.Drawing.Size(89, 17)
        Me.diffuseContrib.TabIndex = 2
        Me.diffuseContrib.Tag = "1"
        Me.diffuseContrib.Text = "diffuseContrib"
        Me.diffuseContrib.UseVisualStyleBackColor = True
        '
        'b_normal
        '
        Me.b_normal.AutoSize = True
        Me.b_normal.Checked = True
        Me.b_normal.Location = New System.Drawing.Point(6, 20)
        Me.b_normal.Name = "b_normal"
        Me.b_normal.Size = New System.Drawing.Size(58, 17)
        Me.b_normal.TabIndex = 1
        Me.b_normal.TabStop = True
        Me.b_normal.Text = "Normal"
        Me.b_normal.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(14, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(218, 52)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "These are used to show different" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "sections of the tank_shader program." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Please re" & _
    "fer to the bottom of the" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "shader to understand more." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'frmShaderDebugSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(244, 235)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ScaleDiffBaseMR)
        Me.Controls.Add(Me.ScaleFGDSpec)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmShaderDebugSettings"
        Me.Text = "Tank Shader Debug Settings"
        Me.TopMost = True
        Me.ScaleFGDSpec.ResumeLayout(False)
        Me.ScaleFGDSpec.PerformLayout()
        Me.ScaleDiffBaseMR.ResumeLayout(False)
        Me.ScaleDiffBaseMR.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ScaleFGDSpec As System.Windows.Forms.GroupBox
    Friend WithEvents ScaleDiffBaseMR As System.Windows.Forms.GroupBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents specContrib As System.Windows.Forms.RadioButton
    Friend WithEvents D As System.Windows.Forms.RadioButton
    Friend WithEvents G As System.Windows.Forms.RadioButton
    Friend WithEvents F As System.Windows.Forms.RadioButton
    Friend WithEvents a_normal As System.Windows.Forms.RadioButton
    Friend WithEvents Roughness As System.Windows.Forms.RadioButton
    Friend WithEvents Metallic As System.Windows.Forms.RadioButton
    Friend WithEvents baseColor As System.Windows.Forms.RadioButton
    Friend WithEvents diffuseContrib As System.Windows.Forms.RadioButton
    Friend WithEvents b_normal As System.Windows.Forms.RadioButton
End Class
