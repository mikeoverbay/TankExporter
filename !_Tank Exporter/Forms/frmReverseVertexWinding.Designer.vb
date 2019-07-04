<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmReverseVertexWinding
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmReverseVertexWinding))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.m_g_show_all = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_g_hide_all = New System.Windows.Forms.ToolStripMenuItem()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_g_show_all, Me.m_g_hide_all})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.MenuStrip1.Size = New System.Drawing.Size(208, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'm_g_show_all
        '
        Me.m_g_show_all.AutoSize = False
        Me.m_g_show_all.Name = "m_g_show_all"
        Me.m_g_show_all.Size = New System.Drawing.Size(65, 20)
        Me.m_g_show_all.Text = "Show All"
        '
        'm_g_hide_all
        '
        Me.m_g_hide_all.AutoSize = False
        Me.m_g_hide_all.Name = "m_g_hide_all"
        Me.m_g_hide_all.Size = New System.Drawing.Size(61, 20)
        Me.m_g_hide_all.Text = "Hide All"
        '
        'Panel1
        '
        Me.Panel1.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.FBX_bg
        Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 24)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(208, 359)
        Me.Panel1.TabIndex = 1
        '
        'frmReverseVertexWinding
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(208, 383)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmReverseVertexWinding"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Check to Flip Winding Order"
        Me.TopMost = True
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents m_g_show_all As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_g_hide_all As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
End Class
