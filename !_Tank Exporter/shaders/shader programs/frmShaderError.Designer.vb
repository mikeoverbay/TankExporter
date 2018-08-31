<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmShaderError
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
        Me.er_tb = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'er_tb
        '
        Me.er_tb.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.er_tb.Dock = System.Windows.Forms.DockStyle.Fill
        Me.er_tb.ForeColor = System.Drawing.SystemColors.Info
        Me.er_tb.Location = New System.Drawing.Point(0, 0)
        Me.er_tb.Multiline = True
        Me.er_tb.Name = "er_tb"
        Me.er_tb.Size = New System.Drawing.Size(575, 262)
        Me.er_tb.TabIndex = 0
        '
        'frmShaderError
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(575, 262)
        Me.Controls.Add(Me.er_tb)
        Me.Name = "frmShaderError"
        Me.Text = "frmShaderError"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents er_tb As System.Windows.Forms.TextBox
End Class
