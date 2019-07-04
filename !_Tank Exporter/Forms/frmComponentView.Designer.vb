<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmComponentView
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmComponentView))
        Me.splitter = New System.Windows.Forms.SplitContainer()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.m_g_show_all = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_g_hide_all = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_fbx_hide_all = New System.Windows.Forms.ToolStripMenuItem()
        Me.m_fbx_show_all = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.splitter, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitter.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'splitter
        '
        Me.splitter.BackColor = System.Drawing.SystemColors.Control
        Me.splitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.splitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitter.IsSplitterFixed = True
        Me.splitter.Location = New System.Drawing.Point(0, 0)
        Me.splitter.Name = "splitter"
        '
        'splitter.Panel1
        '
        Me.splitter.Panel1.AutoScroll = True
        Me.splitter.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.splitter.Panel1.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.MDL_bg
        Me.splitter.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        '
        'splitter.Panel2
        '
        Me.splitter.Panel2.AutoScroll = True
        Me.splitter.Panel2.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.splitter.Panel2.BackgroundImage = Global.Tank_Exporter.My.Resources.Resources.FBX_bg
        Me.splitter.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.splitter.Size = New System.Drawing.Size(396, 360)
        Me.splitter.SplitterDistance = 194
        Me.splitter.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_g_show_all, Me.m_g_hide_all, Me.m_fbx_hide_all, Me.m_fbx_show_all})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.MenuStrip1.Size = New System.Drawing.Size(396, 24)
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
        'm_fbx_hide_all
        '
        Me.m_fbx_hide_all.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_fbx_hide_all.Name = "m_fbx_hide_all"
        Me.m_fbx_hide_all.Size = New System.Drawing.Size(73, 20)
        Me.m_fbx_hide_all.Text = "Hide All    "
        '
        'm_fbx_show_all
        '
        Me.m_fbx_show_all.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_fbx_show_all.AutoSize = False
        Me.m_fbx_show_all.Name = "m_fbx_show_all"
        Me.m_fbx_show_all.Size = New System.Drawing.Size(65, 20)
        Me.m_fbx_show_all.Text = "Show All"
        '
        'frmComponentView
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer), CType(CType(33, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(396, 360)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.splitter)
        Me.ForeColor = System.Drawing.Color.White
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmComponentView"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Hide/Show Components"
        Me.TopMost = True
        CType(Me.splitter, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitter.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents splitter As System.Windows.Forms.SplitContainer
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents m_g_show_all As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_g_hide_all As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_fbx_hide_all As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents m_fbx_show_all As System.Windows.Forms.ToolStripMenuItem
End Class
