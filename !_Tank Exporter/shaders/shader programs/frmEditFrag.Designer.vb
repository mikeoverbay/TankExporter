<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmEditFrag
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEditFrag))
        Me.recompile_bt = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.vert_tb = New FastColoredTextBoxNS.FastColoredTextBox()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.frag_tb = New FastColoredTextBoxNS.FastColoredTextBox()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.geo_tb = New FastColoredTextBoxNS.FastColoredTextBox()
        Me.CB1 = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.search_btn = New System.Windows.Forms.Button()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.vert_tb, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage2.SuspendLayout()
        CType(Me.frag_tb, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage3.SuspendLayout()
        CType(Me.geo_tb, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'recompile_bt
        '
        Me.recompile_bt.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.recompile_bt.Location = New System.Drawing.Point(577, 415)
        Me.recompile_bt.Name = "recompile_bt"
        Me.recompile_bt.Size = New System.Drawing.Size(75, 23)
        Me.recompile_bt.TabIndex = 0
        Me.recompile_bt.Text = "Recompile"
        Me.recompile_bt.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Location = New System.Drawing.Point(1, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.Padding = New System.Drawing.Point(3, 3)
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(663, 413)
        Me.TabControl1.TabIndex = 2
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.vert_tb)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(655, 387)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Vertex Program"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'vert_tb
        '
        Me.vert_tb.AutoCompleteBracketsList = New Char() {Global.Microsoft.VisualBasic.ChrW(40), Global.Microsoft.VisualBasic.ChrW(41), Global.Microsoft.VisualBasic.ChrW(123), Global.Microsoft.VisualBasic.ChrW(125), Global.Microsoft.VisualBasic.ChrW(91), Global.Microsoft.VisualBasic.ChrW(93), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(39), Global.Microsoft.VisualBasic.ChrW(39)}
        Me.vert_tb.AutoIndent = False
        Me.vert_tb.AutoIndentChars = False
        Me.vert_tb.AutoIndentExistingLines = False
        Me.vert_tb.AutoScrollMinSize = New System.Drawing.Size(27, 14)
        Me.vert_tb.BackBrush = Nothing
        Me.vert_tb.BackColor = System.Drawing.Color.Black
        Me.vert_tb.CaretColor = System.Drawing.Color.WhiteSmoke
        Me.vert_tb.CharHeight = 14
        Me.vert_tb.CharWidth = 8
        Me.vert_tb.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.vert_tb.DisabledColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer))
        Me.vert_tb.Dock = System.Windows.Forms.DockStyle.Fill
        Me.vert_tb.ForeColor = System.Drawing.Color.White
        Me.vert_tb.IndentBackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.vert_tb.IsReplaceMode = False
        Me.vert_tb.LineNumberColor = System.Drawing.Color.Cyan
        Me.vert_tb.Location = New System.Drawing.Point(3, 3)
        Me.vert_tb.Name = "vert_tb"
        Me.vert_tb.Paddings = New System.Windows.Forms.Padding(0)
        Me.vert_tb.SelectionColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.vert_tb.ServiceColors = CType(resources.GetObject("vert_tb.ServiceColors"), FastColoredTextBoxNS.ServiceColors)
        Me.vert_tb.ShowFoldingLines = True
        Me.vert_tb.Size = New System.Drawing.Size(649, 381)
        Me.vert_tb.TabIndex = 0
        Me.vert_tb.Zoom = 100
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.frag_tb)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(655, 387)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Fragment Program"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'frag_tb
        '
        Me.frag_tb.AutoCompleteBracketsList = New Char() {Global.Microsoft.VisualBasic.ChrW(40), Global.Microsoft.VisualBasic.ChrW(41), Global.Microsoft.VisualBasic.ChrW(123), Global.Microsoft.VisualBasic.ChrW(125), Global.Microsoft.VisualBasic.ChrW(91), Global.Microsoft.VisualBasic.ChrW(93), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(39), Global.Microsoft.VisualBasic.ChrW(39)}
        Me.frag_tb.AutoIndent = False
        Me.frag_tb.AutoIndentChars = False
        Me.frag_tb.AutoIndentExistingLines = False
        Me.frag_tb.AutoScrollMinSize = New System.Drawing.Size(2, 14)
        Me.frag_tb.BackBrush = Nothing
        Me.frag_tb.BackColor = System.Drawing.Color.Black
        Me.frag_tb.CaretColor = System.Drawing.Color.WhiteSmoke
        Me.frag_tb.CharHeight = 14
        Me.frag_tb.CharWidth = 8
        Me.frag_tb.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.frag_tb.DisabledColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer))
        Me.frag_tb.Dock = System.Windows.Forms.DockStyle.Fill
        Me.frag_tb.ForeColor = System.Drawing.Color.White
        Me.frag_tb.IndentBackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.frag_tb.IsReplaceMode = False
        Me.frag_tb.LineNumberColor = System.Drawing.Color.Cyan
        Me.frag_tb.Location = New System.Drawing.Point(3, 3)
        Me.frag_tb.Name = "frag_tb"
        Me.frag_tb.Paddings = New System.Windows.Forms.Padding(0)
        Me.frag_tb.SelectionColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.frag_tb.ServiceColors = CType(resources.GetObject("frag_tb.ServiceColors"), FastColoredTextBoxNS.ServiceColors)
        Me.frag_tb.Size = New System.Drawing.Size(649, 381)
        Me.frag_tb.TabIndex = 0
        Me.frag_tb.Zoom = 100
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.geo_tb)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(655, 387)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Geometry Program"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'geo_tb
        '
        Me.geo_tb.AutoCompleteBracketsList = New Char() {Global.Microsoft.VisualBasic.ChrW(40), Global.Microsoft.VisualBasic.ChrW(41), Global.Microsoft.VisualBasic.ChrW(123), Global.Microsoft.VisualBasic.ChrW(125), Global.Microsoft.VisualBasic.ChrW(91), Global.Microsoft.VisualBasic.ChrW(93), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(34), Global.Microsoft.VisualBasic.ChrW(39), Global.Microsoft.VisualBasic.ChrW(39)}
        Me.geo_tb.AutoIndent = False
        Me.geo_tb.AutoIndentChars = False
        Me.geo_tb.AutoIndentExistingLines = False
        Me.geo_tb.AutoScrollMinSize = New System.Drawing.Size(2, 14)
        Me.geo_tb.BackBrush = Nothing
        Me.geo_tb.BackColor = System.Drawing.Color.Black
        Me.geo_tb.CaretColor = System.Drawing.Color.WhiteSmoke
        Me.geo_tb.CharHeight = 14
        Me.geo_tb.CharWidth = 8
        Me.geo_tb.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.geo_tb.DisabledColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer), CType(CType(180, Byte), Integer))
        Me.geo_tb.Dock = System.Windows.Forms.DockStyle.Fill
        Me.geo_tb.ForeColor = System.Drawing.Color.White
        Me.geo_tb.IndentBackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.geo_tb.IsReplaceMode = False
        Me.geo_tb.LineNumberColor = System.Drawing.Color.Cyan
        Me.geo_tb.Location = New System.Drawing.Point(3, 3)
        Me.geo_tb.Name = "geo_tb"
        Me.geo_tb.Paddings = New System.Windows.Forms.Padding(0)
        Me.geo_tb.SelectionColor = System.Drawing.Color.FromArgb(CType(CType(60, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(255, Byte), Integer))
        Me.geo_tb.ServiceColors = CType(resources.GetObject("geo_tb.ServiceColors"), FastColoredTextBoxNS.ServiceColors)
        Me.geo_tb.Size = New System.Drawing.Size(649, 381)
        Me.geo_tb.TabIndex = 0
        Me.geo_tb.Zoom = 100
        '
        'CB1
        '
        Me.CB1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CB1.BackColor = System.Drawing.Color.Black
        Me.CB1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.CB1.ForeColor = System.Drawing.Color.White
        Me.CB1.FormattingEnabled = True
        Me.CB1.Location = New System.Drawing.Point(96, 417)
        Me.CB1.Name = "CB1"
        Me.CB1.Size = New System.Drawing.Size(153, 21)
        Me.CB1.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(13, 420)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(74, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Select Shader"
        '
        'search_btn
        '
        Me.search_btn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.search_btn.Location = New System.Drawing.Point(477, 415)
        Me.search_btn.Name = "search_btn"
        Me.search_btn.Size = New System.Drawing.Size(66, 23)
        Me.search_btn.TabIndex = 5
        Me.search_btn.Text = "Search"
        Me.search_btn.UseVisualStyleBackColor = True
        '
        'frmEditFrag
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(664, 442)
        Me.Controls.Add(Me.search_btn)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.CB1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.recompile_bt)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmEditFrag"
        Me.Text = "Shader Program Editor"
        Me.TopMost = True
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        CType(Me.vert_tb, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage2.ResumeLayout(False)
        CType(Me.frag_tb, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage3.ResumeLayout(False)
        CType(Me.geo_tb, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents recompile_bt As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents CB1 As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents search_btn As System.Windows.Forms.Button
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents vert_tb As FastColoredTextBoxNS.FastColoredTextBox
    Friend WithEvents frag_tb As FastColoredTextBoxNS.FastColoredTextBox
    Friend WithEvents geo_tb As FastColoredTextBoxNS.FastColoredTextBox
End Class
