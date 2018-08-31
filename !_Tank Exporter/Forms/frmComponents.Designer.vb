<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmComponents
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmComponents))
        Me.tv_guns = New System.Windows.Forms.TreeView()
        Me.tv_turrets = New System.Windows.Forms.TreeView()
        Me.continue_bt = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'tv_guns
        '
        Me.tv_guns.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.tv_guns.CheckBoxes = True
        Me.tv_guns.ForeColor = System.Drawing.Color.White
        Me.tv_guns.Location = New System.Drawing.Point(12, 24)
        Me.tv_guns.Name = "tv_guns"
        Me.tv_guns.Size = New System.Drawing.Size(136, 140)
        Me.tv_guns.TabIndex = 0
        '
        'tv_turrets
        '
        Me.tv_turrets.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.tv_turrets.CheckBoxes = True
        Me.tv_turrets.ForeColor = System.Drawing.Color.White
        Me.tv_turrets.Location = New System.Drawing.Point(12, 183)
        Me.tv_turrets.Name = "tv_turrets"
        Me.tv_turrets.Size = New System.Drawing.Size(136, 144)
        Me.tv_turrets.TabIndex = 1
        '
        'continue_bt
        '
        Me.continue_bt.Location = New System.Drawing.Point(44, 333)
        Me.continue_bt.Name = "continue_bt"
        Me.continue_bt.Size = New System.Drawing.Size(75, 23)
        Me.continue_bt.TabIndex = 2
        Me.continue_bt.Text = "Continue"
        Me.continue_bt.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(12, 167)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Found Turrets"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(12, 8)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(65, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Found Guns"
        '
        'frmComponents
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(164, 363)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.continue_bt)
        Me.Controls.Add(Me.tv_turrets)
        Me.Controls.Add(Me.tv_guns)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmComponents"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Components"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tv_guns As System.Windows.Forms.TreeView
    Friend WithEvents tv_turrets As System.Windows.Forms.TreeView
    Friend WithEvents continue_bt As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
End Class
