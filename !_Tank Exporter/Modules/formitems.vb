
Module formitems
    Public just As Integer
    Public tanklist As New mytextbox

    Public Class mytreeview
        Inherits TreeView

        Protected Overrides Sub OnDrawNode(e As DrawTreeNodeEventArgs)
            Dim font = e.Node.NodeFont
            Dim fore = e.Node.ForeColor

            If (e.State And TreeNodeStates.Hot) = TreeNodeStates.Hot Then
                e.Graphics.FillRectangle(New SolidBrush(Color.DarkGray), e.Bounds)
            Else

                e.Graphics.FillRectangle(New SolidBrush(Color.DimGray), e.Bounds)

            End If
            TextRenderer.DrawText(e.Graphics, e.Node.Text, font, e.Bounds, fore, TextFormatFlags.GlyphOverhangPadding)

            MyBase.OnDrawNode(e)
        End Sub
        Protected Overrides Sub OnCreateControl()
            SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            MyBase.OnCreateControl()
        End Sub
       
    End Class


    Public Class mytextbox
        Inherits TextBox
        Public Sub New()
            'SetStyle(ControlStyles.UserPaint, True)
        End Sub
        Protected Overrides Sub OnPaint(e As PaintEventArgs)
            MyBase.OnPaint(e)
        End Sub

        Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
            'MyBase.OnPaintBackground(pevent)
        End Sub
    End Class
End Module
