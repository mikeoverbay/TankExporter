<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditVisual
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditVisual))
        Me.RichTextBox1 = New System.Windows.Forms.RichTextBox()
        Me.CMS = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.m_copy = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_cut = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_paste = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.m_file = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_c_xml = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_h_xml = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_t_xml = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_g_xml = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_up = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_down = New System.Windows.Forms.ToolStripMenuItem()
        Me.CMS.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'RichTextBox1
        '
        Me.RichTextBox1.BackColor = System.Drawing.Color.Black
        Me.RichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.RichTextBox1.ContextMenuStrip = Me.CMS
        Me.RichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RichTextBox1.Font = New System.Drawing.Font("Lucida Console", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RichTextBox1.ForeColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.RichTextBox1.Location = New System.Drawing.Point(0, 24)
        Me.RichTextBox1.Name = "RichTextBox1"
        Me.RichTextBox1.ShowSelectionMargin = True
        Me.RichTextBox1.Size = New System.Drawing.Size(545, 340)
        Me.RichTextBox1.TabIndex = 0
        Me.RichTextBox1.Text = ""
        Me.RichTextBox1.WordWrap = False
        '
        'CMS
        '
        Me.CMS.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_copy, Me.m_cut, Me.m_paste})
        Me.CMS.Name = "CMS"
        Me.CMS.Size = New System.Drawing.Size(103, 70)
        '
        'm_copy
        '
        Me.m_copy.Image = Global.Tank_Exporter.My.Resources.Resources.Copy_6524
        Me.m_copy.Name = "m_copy"
        Me.m_copy.Size = New System.Drawing.Size(102, 22)
        Me.m_copy.Text = "Copy"
        '
        'm_cut
        '
        Me.m_cut.Image = Global.Tank_Exporter.My.Resources.Resources.Cut_6523
        Me.m_cut.Name = "m_cut"
        Me.m_cut.Size = New System.Drawing.Size(102, 22)
        Me.m_cut.Text = "Cut"
        '
        'm_paste
        '
        Me.m_paste.Image = Global.Tank_Exporter.My.Resources.Resources.Paste_6520
        Me.m_paste.Name = "m_paste"
        Me.m_paste.Size = New System.Drawing.Size(102, 22)
        Me.m_paste.Text = "Paste"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_file, Me.m_c_xml, Me.m_h_xml, Me.m_t_xml, Me.m_g_xml, Me.m_up, Me.m_down})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(545, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'm_file
        '
        Me.m_file.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1})
        Me.m_file.Name = "m_file"
        Me.m_file.Size = New System.Drawing.Size(37, 20)
        Me.m_file.Text = "File"
        Me.m_file.Visible = False
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(98, 22)
        Me.ToolStripMenuItem1.Text = "Save"
        '
        'm_c_xml
        '
        Me.m_c_xml.Name = "m_c_xml"
        Me.m_c_xml.Size = New System.Drawing.Size(58, 20)
        Me.m_c_xml.Text = "Chassis"
        '
        'm_h_xml
        '
        Me.m_h_xml.Name = "m_h_xml"
        Me.m_h_xml.Size = New System.Drawing.Size(41, 20)
        Me.m_h_xml.Text = "Hull"
        '
        'm_t_xml
        '
        Me.m_t_xml.Name = "m_t_xml"
        Me.m_t_xml.Size = New System.Drawing.Size(51, 20)
        Me.m_t_xml.Text = "Turret"
        '
        'm_g_xml
        '
        Me.m_g_xml.Name = "m_g_xml"
        Me.m_g_xml.Size = New System.Drawing.Size(41, 20)
        Me.m_g_xml.Text = "Gun"
        '
        'm_up
        '
        Me.m_up.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_up.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_up.Image = Global.Tank_Exporter.My.Resources.Resources.control_270
        Me.m_up.Name = "m_up"
        Me.m_up.Size = New System.Drawing.Size(28, 20)
        Me.m_up.Text = "ToolStripMenuItem2"
        '
        'm_down
        '
        Me.m_down.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_down.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_down.Image = Global.Tank_Exporter.My.Resources.Resources.control_090
        Me.m_down.Name = "m_down"
        Me.m_down.Size = New System.Drawing.Size(28, 20)
        Me.m_down.Text = "ToolStripMenuItem2"
        '
        'frmEditVisual
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(545, 364)
        Me.Controls.Add(Me.RichTextBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmEditVisual"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Visual Viewer"
        Me.TopMost = True
        Me.CMS.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents RichTextBox1 As System.Windows.Forms.RichTextBox
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents m_c_xml As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_h_xml As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_t_xml As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_g_xml As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_file As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CMS As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents m_copy As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_cut As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_paste As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_down As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_up As System.Windows.Forms.ToolStripMenuItem
End Class
