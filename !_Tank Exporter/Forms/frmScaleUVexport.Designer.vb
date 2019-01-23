<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmScaleUVexport
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmScaleUVexport))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.full_scale_cb = New System.Windows.Forms.RadioButton()
        Me.half_scale_cb = New System.Windows.Forms.RadioButton()
        Me.double_scale_cb = New System.Windows.Forms.RadioButton()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox1.Controls.Add(Me.double_scale_cb)
        Me.GroupBox1.Controls.Add(Me.half_scale_cb)
        Me.GroupBox1.Controls.Add(Me.full_scale_cb)
        Me.GroupBox1.ForeColor = System.Drawing.Color.White
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(98, 98)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Scale"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(23, 116)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Write Image"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'full_scale_cb
        '
        Me.full_scale_cb.AutoSize = True
        Me.full_scale_cb.Checked = True
        Me.full_scale_cb.Location = New System.Drawing.Point(11, 20)
        Me.full_scale_cb.Name = "full_scale_cb"
        Me.full_scale_cb.Size = New System.Drawing.Size(70, 17)
        Me.full_scale_cb.TabIndex = 0
        Me.full_scale_cb.TabStop = True
        Me.full_scale_cb.Text = "1.0 Scale"
        Me.full_scale_cb.UseVisualStyleBackColor = True
        '
        'half_scale_cb
        '
        Me.half_scale_cb.AutoSize = True
        Me.half_scale_cb.Location = New System.Drawing.Point(11, 44)
        Me.half_scale_cb.Name = "half_scale_cb"
        Me.half_scale_cb.Size = New System.Drawing.Size(70, 17)
        Me.half_scale_cb.TabIndex = 1
        Me.half_scale_cb.Text = "0.5 Scale"
        Me.half_scale_cb.UseVisualStyleBackColor = True
        '
        'double_scale_cb
        '
        Me.double_scale_cb.AutoSize = True
        Me.double_scale_cb.Location = New System.Drawing.Point(11, 68)
        Me.double_scale_cb.Name = "double_scale_cb"
        Me.double_scale_cb.Size = New System.Drawing.Size(70, 17)
        Me.double_scale_cb.TabIndex = 2
        Me.double_scale_cb.Text = "2.0 Scale"
        Me.double_scale_cb.UseVisualStyleBackColor = True
        '
        'frmScaleUVexport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(122, 155)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmScaleUVexport"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Scale UV image"
        Me.TopMost = True
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents double_scale_cb As System.Windows.Forms.RadioButton
    Friend WithEvents half_scale_cb As System.Windows.Forms.RadioButton
    Friend WithEvents full_scale_cb As System.Windows.Forms.RadioButton
End Class
