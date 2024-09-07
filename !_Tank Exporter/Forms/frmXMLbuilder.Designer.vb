<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmXMLbuilder
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmXMLbuilder))
        Me.go_btn = New System.Windows.Forms.Button()
        Me.file_tb = New System.Windows.Forms.TextBox()
        Me.pkg_tb = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.unique = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'go_btn
        '
        Me.go_btn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.go_btn.Location = New System.Drawing.Point(215, 3)
        Me.go_btn.Name = "go_btn"
        Me.go_btn.Size = New System.Drawing.Size(75, 23)
        Me.go_btn.TabIndex = 0
        Me.go_btn.Text = "Update lookup XML"
        Me.go_btn.UseVisualStyleBackColor = True
        '
        'file_tb
        '
        Me.file_tb.Location = New System.Drawing.Point(15, 89)
        Me.file_tb.Name = "file_tb"
        Me.file_tb.Size = New System.Drawing.Size(494, 20)
        Me.file_tb.TabIndex = 1
        '
        'pkg_tb
        '
        Me.pkg_tb.Location = New System.Drawing.Point(15, 32)
        Me.pkg_tb.Name = "pkg_tb"
        Me.pkg_tb.Size = New System.Drawing.Size(494, 20)
        Me.pkg_tb.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.Label1.Location = New System.Drawing.Point(12, 66)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(57, 20)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Label1"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.SystemColors.ControlLight
        Me.Label2.Location = New System.Drawing.Point(11, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(57, 20)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Label2"
        '
        'unique
        '
        Me.unique.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.unique.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.unique.Location = New System.Drawing.Point(13, 112)
        Me.unique.Name = "unique"
        Me.unique.Size = New System.Drawing.Size(496, 23)
        Me.unique.TabIndex = 5
        Me.unique.Tag = ""
        '
        'frmXMLbuilder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(33, 33, 33)
        Me.ClientSize = New System.Drawing.Size(521, 138)
        Me.Controls.Add(Me.unique)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.pkg_tb)
        Me.Controls.Add(Me.file_tb)
        Me.Controls.Add(Me.go_btn)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmXMLbuilder"
        Me.Text = "frmXMLbuilder"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents go_btn As Button
    Friend WithEvents file_tb As TextBox
    Friend WithEvents pkg_tb As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents unique As Label
End Class
