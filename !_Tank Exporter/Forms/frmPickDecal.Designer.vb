<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPickDecal
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPickDecal))
        Me.pb4 = New System.Windows.Forms.PictureBox()
        Me.cancel_selection_bt = New System.Windows.Forms.Button()
        CType(Me.pb4, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pb4
        '
        Me.pb4.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.pb4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pb4.Location = New System.Drawing.Point(0, 0)
        Me.pb4.Name = "pb4"
        Me.pb4.Size = New System.Drawing.Size(580, 450)
        Me.pb4.TabIndex = 0
        Me.pb4.TabStop = False
        '
        'cancel_selection_bt
        '
        Me.cancel_selection_bt.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cancel_selection_bt.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.cancel_selection_bt.ForeColor = System.Drawing.Color.White
        Me.cancel_selection_bt.Location = New System.Drawing.Point(493, 415)
        Me.cancel_selection_bt.Name = "cancel_selection_bt"
        Me.cancel_selection_bt.Size = New System.Drawing.Size(75, 23)
        Me.cancel_selection_bt.TabIndex = 1
        Me.cancel_selection_bt.Text = "Cancel"
        Me.cancel_selection_bt.UseVisualStyleBackColor = True
        '
        'frmPickDecal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(580, 450)
        Me.Controls.Add(Me.cancel_selection_bt)
        Me.Controls.Add(Me.pb4)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmPickDecal"
        Me.Opacity = 0R
        Me.Text = "Decal Picker"
        Me.TopMost = True
        CType(Me.pb4, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pb4 As PictureBox
    Friend WithEvents cancel_selection_bt As Button
End Class
