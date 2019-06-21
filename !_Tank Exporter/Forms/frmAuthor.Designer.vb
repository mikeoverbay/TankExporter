<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAuthor
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.mod_name_tb = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.human_readable_tb = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.description_tb = New System.Windows.Forms.TextBox()
        Me.cancel_btn = New System.Windows.Forms.Button()
        Me.ok_btn = New System.Windows.Forms.Button()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.version_tb = New System.Windows.Forms.TextBox()
        Me.creator_tb = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(12, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Creator's Name"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Location = New System.Drawing.Point(242, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Technical Id"
        '
        'mod_name_tb
        '
        Me.mod_name_tb.BackColor = System.Drawing.Color.DimGray
        Me.mod_name_tb.ForeColor = System.Drawing.Color.White
        Me.mod_name_tb.Location = New System.Drawing.Point(245, 39)
        Me.mod_name_tb.Name = "mod_name_tb"
        Me.mod_name_tb.Size = New System.Drawing.Size(245, 20)
        Me.mod_name_tb.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Location = New System.Drawing.Point(12, 83)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(121, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Human Readable Name"
        '
        'human_readable_tb
        '
        Me.human_readable_tb.BackColor = System.Drawing.Color.DimGray
        Me.human_readable_tb.ForeColor = System.Drawing.Color.White
        Me.human_readable_tb.Location = New System.Drawing.Point(15, 99)
        Me.human_readable_tb.Name = "human_readable_tb"
        Me.human_readable_tb.Size = New System.Drawing.Size(205, 20)
        Me.human_readable_tb.TabIndex = 2
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Location = New System.Drawing.Point(242, 83)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(84, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Mod Description"
        '
        'description_tb
        '
        Me.description_tb.BackColor = System.Drawing.Color.DimGray
        Me.description_tb.ForeColor = System.Drawing.Color.White
        Me.description_tb.Location = New System.Drawing.Point(245, 99)
        Me.description_tb.Name = "description_tb"
        Me.description_tb.Size = New System.Drawing.Size(245, 20)
        Me.description_tb.TabIndex = 3
        '
        'cancel_btn
        '
        Me.cancel_btn.Location = New System.Drawing.Point(313, 140)
        Me.cancel_btn.Name = "cancel_btn"
        Me.cancel_btn.Size = New System.Drawing.Size(75, 23)
        Me.cancel_btn.TabIndex = 6
        Me.cancel_btn.Text = "Cancel"
        Me.cancel_btn.UseVisualStyleBackColor = True
        '
        'ok_btn
        '
        Me.ok_btn.Location = New System.Drawing.Point(415, 140)
        Me.ok_btn.Name = "ok_btn"
        Me.ok_btn.Size = New System.Drawing.Size(75, 23)
        Me.ok_btn.TabIndex = 5
        Me.ok_btn.Text = "OK"
        Me.ok_btn.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(110, 145)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(42, 13)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Version"
        '
        'version_tb
        '
        Me.version_tb.BackColor = System.Drawing.Color.DimGray
        Me.version_tb.ForeColor = System.Drawing.Color.White
        Me.version_tb.Location = New System.Drawing.Point(158, 142)
        Me.version_tb.Name = "version_tb"
        Me.version_tb.Size = New System.Drawing.Size(62, 20)
        Me.version_tb.TabIndex = 4
        '
        'creator_tb
        '
        Me.creator_tb.BackColor = System.Drawing.Color.DimGray
        Me.creator_tb.ForeColor = System.Drawing.Color.White
        Me.creator_tb.Location = New System.Drawing.Point(15, 39)
        Me.creator_tb.Name = "creator_tb"
        Me.creator_tb.Size = New System.Drawing.Size(205, 20)
        Me.creator_tb.TabIndex = 0
        '
        'frmAuthor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(515, 175)
        Me.ControlBox = False
        Me.Controls.Add(Me.version_tb)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.ok_btn)
        Me.Controls.Add(Me.cancel_btn)
        Me.Controls.Add(Me.description_tb)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.human_readable_tb)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.mod_name_tb)
        Me.Controls.Add(Me.creator_tb)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "frmAuthor"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Creators Form"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents creator_tb As System.Windows.Forms.TextBox
    Friend WithEvents mod_name_tb As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents human_readable_tb As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents description_tb As System.Windows.Forms.TextBox
    Friend WithEvents cancel_btn As System.Windows.Forms.Button
    Friend WithEvents ok_btn As System.Windows.Forms.Button
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents version_tb As System.Windows.Forms.TextBox
End Class
