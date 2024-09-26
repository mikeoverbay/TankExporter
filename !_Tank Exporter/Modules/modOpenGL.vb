Imports System.Windows
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Net
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Web
Imports Tao.OpenGl
Imports Tao.Platform.Windows
Imports Tao.FreeGlut
Imports Tao.FreeGlut.Glut
Imports Microsoft.VisualBasic.Strings
Imports System.Math
Imports System.Object
Imports System.Threading
Imports System.Data
Imports Tao.DevIl
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports System.Collections.Generic

Module modOpenGL
    Public pb1_hDC As System.IntPtr
    Public pb1_hRC As System.IntPtr
    Public pb2_hDC As System.IntPtr
    Public pb2_hRC As System.IntPtr
    Public pb3_hDC As System.IntPtr
    Public pb3_hRC As System.IntPtr
    Public pb4_hDC As System.IntPtr ' New for the fourth context
    Public pb4_hRC As System.IntPtr ' New for the fourth context
    Public position0() As Single = {2.843F, 10.0F, 9.596F, 1.0F}
    Public position1() As Single = {-5.0F, 8.0F, -5.0F, 1.0F}
    Public position2() As Single = {5.0F, 12.0F, 0.0F, -5.0F}

    Public W_position0() As Single = {5.0F, 10.0F, 5.0F, 1.0F}
    Public W_position1() As Single = {-5.0F, 8.0F, -5.0F, 1.0F}
    Public W_position2() As Single = {5.0F, 10.0F, -5.0F, 1.0F}

    Public Sub EnableOpenGL()
        position0(0) = W_position0(0)
        position0(1) = W_position0(1)
        position0(2) = W_position0(2)

        position1(0) = W_position1(0)
        position1(1) = W_position1(1)
        position1(2) = W_position1(2)

        position2(0) = W_position2(0)
        position2(1) = W_position2(1)
        position2(2) = W_position2(2)

        frmMain.pb2.Visible = False
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()

        pb1_hDC = User.GetDC(frmMain.pb1.Handle)
        pb2_hDC = User.GetDC(frmMain.pb2.Handle)
        pb3_hDC = User.GetDC(frmMain.PB3.Handle)
        pb4_hDC = User.GetDC(frmPickDecal.pb4.Handle) ' Get the device context of the form

        frmMain.Controls.Add(frmMain.pb2)
        Application.DoEvents()
        Application.DoEvents()
        Application.DoEvents()

        frmMain.pb2.Location = frmMain.pb1.Location
        Dim pfd As Gdi.PIXELFORMATDESCRIPTOR
        Dim PixelFormat As Integer

        ' ZeroMemory(pfd, Len(pfd))
        pfd.nSize = Len(pfd)
        pfd.nVersion = 1
        pfd.dwFlags = Gdi.PFD_DRAW_TO_WINDOW Or Gdi.PFD_SUPPORT_OPENGL Or Gdi.PFD_DOUBLEBUFFER Or Gdi.PFD_GENERIC_ACCELERATED
        pfd.iPixelType = Gdi.PFD_TYPE_RGBA
        pfd.cColorBits = 32
        pfd.cDepthBits = 24
        pfd.cStencilBits = 8
        pfd.cAlphaBits = 8
        pfd.iLayerType = Gdi.PFD_MAIN_PLANE

        ' Set pixel formats
        PixelFormat = Gdi.ChoosePixelFormat(pb1_hDC, pfd)
        If PixelFormat = 0 Then
            MessageBox.Show("Unable to retrieve pixel format")
            Return
        End If

        '================================================================1
        If Not (Gdi.SetPixelFormat(pb1_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format")
            Return
        End If
        pb1_hRC = Wgl.wglCreateContext(pb1_hDC)
        If pb1_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context")
            Return
        End If
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 1")
            Return
        End If

        '================================================================2
        If Not (Gdi.SetPixelFormat(pb2_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format 2")
            Return
        End If
        pb2_hRC = Wgl.wglCreateContext(pb2_hDC)
        If pb2_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context 2")
            Return
        End If
        If Not (Wgl.wglMakeCurrent(pb2_hDC, pb2_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 2")
            Return
        End If

        '================================================================3
        If Not (Gdi.SetPixelFormat(pb3_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format 3")
            Return
        End If
        pb3_hRC = Wgl.wglCreateContext(pb3_hDC)
        If pb3_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context 3")
            Return
        End If
        If Not (Wgl.wglMakeCurrent(pb3_hDC, pb3_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 3")
            Return
        End If

        '================================================================4

        If Not (Gdi.SetPixelFormat(pb4_hDC, PixelFormat, pfd)) Then
            MessageBox.Show("Unable to set pixel format 4")
            Return
        End If
        pb4_hRC = Wgl.wglCreateContext(pb4_hDC)
        If pb4_hRC.ToInt32 = 0 Then
            MessageBox.Show("Unable to get rendering context 4")
            Return
        End If
        If Not (Wgl.wglMakeCurrent(pb4_hDC, pb4_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 4")
            Return
        End If

        '================================================================
        ' Share resources among contexts
        Wgl.wglShareLists(pb1_hRC, pb2_hRC)
        Wgl.wglShareLists(pb1_hRC, pb3_hRC)
        Wgl.wglShareLists(pb1_hRC, pb4_hRC) ' Share with the fourth context

        ' Go back to context 1
        If Not (Wgl.wglMakeCurrent(pb1_hDC, pb1_hRC)) Then
            MessageBox.Show("Unable to make rendering context current 1")
            Return
        End If

        Glut.glutInit()
        Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, largestAnsio)

        Glut.glutInitDisplayMode(GLUT_RGBA Or GLUT_DOUBLE)
        Gl.glViewport(0, 0, frmMain.pb1.Width, frmMain.pb1.Height)

        Gl.glClearColor(0.0F, 0.0F, 0.0F, 1.0F)
        Gl.glEnable(Gl.GL_COLOR_MATERIAL)
        Gl.glEnable(Gl.GL_LIGHT0)
        Gl.glEnable(Gl.GL_LIGHTING)

        gl_set_lights()
        'build_shaders()
        Dim pa = Wgl.wglGetProcAddress("wglGetExtensionsStringEXT")

        'If Wgl.wglSwapIntervalEXT(0) Then
        'End If
    End Sub



    Public Sub DisableOpenGL()
        G_Buffer.shut_down()
        'BlurShadowFBO.shut_down()
        shadow_fbo.shadow_fbo_shut_down()
        worker_fbo.shutdown_worker_fbo()

        Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero)
        Wgl.wglDeleteContext(pb1_hRC)

    End Sub
    Private far_Clip As Single = 1000.0
    Public Sub ResizeGL(ByRef w As Integer, ByRef h As Integer)
        Gl.glViewport(0, 0, w, h)

    End Sub
    Public Sub glutPrintSmall(ByVal x As Single, ByVal y As Single,
ByVal text As String, ByVal r As Single, ByVal g As Single, ByVal b As Single, ByVal a As Single)

        Try
            If text.Length = 0 Then Exit Sub
        Catch ex As Exception
            Return
        End Try
        Dim blending As Boolean = False
        If Gl.glIsEnabled(Gl.GL_BLEND) Then blending = True
        Gl.glEnable(Gl.GL_BLEND)
        Gl.glColor3f(r, g, b)
        Gl.glRasterPos2f(x, y)
        For Each I In text

            Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_HELVETICA_10, Asc(I))

        Next
        If Not blending Then Gl.glDisable(Gl.GL_BLEND)
    End Sub
    Public Sub glutPrint(ByVal x As Single, ByVal y As Single,
ByVal text As String, ByVal r As Single, ByVal g As Single, ByVal b As Single, ByVal a As Single)
        ' Split the text by lines
        Dim lines As String() = text.Split(New String() {Environment.NewLine}, StringSplitOptions.None)

        ' Set the color for text rendering
        Gl.glColor4f(r, g, b, a)
        Dim blending As Boolean = False

        ' Iterate through each line and render it
        For Each line As String In lines
            ' Render the text line by line
            ' RenderText(x, y, line) ' Assume RenderText is a method that renders the text at the given position

            ' Move to the next line position
            y -= 15 ' Adjust this value based on your line height requirements

            Try
            If text.Length = 0 Then Exit Sub
        Catch ex As Exception
            Return
        End Try
            If Gl.glIsEnabled(Gl.GL_BLEND) Then blending = True
            Gl.glEnable(Gl.GL_BLEND)
        Gl.glColor3f(r, g, b)
        Gl.glRasterPos2f(x, y)
            For Each I In line

                Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_8_BY_13, Asc(I))

            Next
        Next
        If Not blending Then Gl.glDisable(Gl.GL_BLEND)
    End Sub
    Public Sub glutPrintBox(ByVal x As Single, ByVal y As Single,
ByVal text As String, ByVal r As Single, ByVal g As Single, ByVal b As Single, ByVal a As Single)

        ' Split the text by lines
        Dim lines As String() = text.Split(New String() {Environment.NewLine}, StringSplitOptions.None)

        ' Set the color for text rendering
        Gl.glColor4f(r, g, b, a)
        Dim blending As Boolean = False

        ' Iterate through each line and render it
        For Each line As String In lines
            ' Render the text line by line
            ' RenderText(x, y, line) ' Assume RenderText is a method that renders the text at the given position

            ' Move to the next line position
            y -= 15 ' Adjust this value based on your line height requirements
            Try
                If text.Length = 0 Then Exit Sub
            Catch ex As Exception
                Return
            End Try
            Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_FILL)
            If Gl.glIsEnabled(Gl.GL_BLEND) Then blending = True
            Gl.glEnable(Gl.GL_BLEND)
            Gl.glColor4f(0, 0, 0, 0.25)
            Gl.glBegin(Gl.GL_QUADS)
            Dim L1 = line.Length * 8
            Dim l2 = 7
            Gl.glVertex2f(x - 2, y - l2 + 2)
            Gl.glVertex2f(x + L1 + 2, y - l2 + 2)
            Gl.glVertex2f(x + L1 + 2, y + l2 + 5)
            Gl.glVertex2f(x - 2, y + l2 + 5)
            Gl.glEnd()
            Gl.glColor3f(r, g, b)
            Gl.glRasterPos2f(x, y)
            For Each I In line

                Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_8_BY_13, Asc(I))

            Next
        Next
        If Not blending Then Gl.glDisable(Gl.GL_BLEND)
    End Sub

    Public Sub ViewOrtho()
        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity() 'Reset The Matrix
        Gl.glOrtho(0, frmMain.pb1.Width, -frmMain.pb1.Height, 0, -200.0, 100.0) 'Select Ortho Mode
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview Matrix
        Gl.glLoadIdentity() 'Reset The Matrix
    End Sub
    Public Sub ViewPerspective(w, h)
        ' Set Up A Perspective View

        Gl.glMatrixMode(Gl.GL_PROJECTION) 'Select Projection
        Gl.glLoadIdentity()

        Glu.gluPerspective(FOV, CSng(w / h), 0.1F, far_Clip)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glDepthMask(Gl.GL_TRUE)
        Gl.glDepthRange(0.0, 1.0)
        Gl.glMatrixMode(Gl.GL_MODELVIEW)    'Select Modelview
        Gl.glLoadIdentity() 'Reset The Matrix
    End Sub

    Public Sub gl_set_lights()
        'lighting

        'Debug.WriteLine("GL Error A:" + Gl.glGetError().ToString)
        ''Gl.glEnable(Gl.GL_SMOOTH)
        ''Gl.glShadeModel(Gl.GL_SMOOTH)
        'Debug.WriteLine("GL Error B:" + Gl.glGetError().ToString)
        Dim global_ambient() As Single = {0.2F, 0.2F, 0.2F, 1.0F}

        Dim specular0() As Single = {0.5F, 0.5F, 0.5F, 1.0F}
        Dim emission0() As Single = {0.0F, 0.0F, 0.0F, 1.0F}
        Dim ambient0() As Single = {0.3F, 0.3F, 0.3F, 1.0F}
        Dim diffuseLight0() As Single = {0.5, 0.5, 0.5, 1.0F}

        Dim specular1() As Single = {0.5F, 0.5F, 0.5F, 1.0F}
        Dim emission1() As Single = {0.0F, 0.0F, 0.0F, 1.0F}
        Dim ambient1() As Single = {0.3F, 0.3F, 0.3F, 1.0F}
        Dim diffuseLight1() As Single = {0.5, 0.5, 0.5, 1.0F}

        Dim specular2() As Single = {0.5F, 0.5F, 0.5F, 1.0F}
        Dim emission2() As Single = {0.0F, 0.0F, 0.0F, 1.0F}
        Dim ambient2() As Single = {0.3F, 0.3F, 0.3F, 1.0F}
        Dim diffuseLight2() As Single = {0.5, 0.5, 0.5, 1.0F}

        Dim specReflection0() As Single = {0.6F, 0.6F, 0.6F, 1.0F}
        Dim specReflection1() As Single = {0.6F, 0.6F, 0.6F, 1.0F}
        Dim specReflection2() As Single = {0.6F, 0.6F, 0.6F, 1.0F}

        Dim mcolor() As Single = {0.2F, 0.2F, 0.2F, 1.0F}
        'Gl.glEnable(Gl.GL_SMOOTH)
        Gl.glShadeModel(Gl.GL_SMOOTH)

        Gl.glEnable(Gl.GL_LIGHT0)
        Gl.glEnable(Gl.GL_LIGHT1)
        Gl.glEnable(Gl.GL_LIGHT2)
        Gl.glEnable(Gl.GL_LIGHTING)

        'light 0
        Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, global_ambient)

        'Dim position3() As Single = {-400.0F, 100.0F, -400.0F, 1.0F}
        'Dim position4() As Single = {-400.0F, 100.0F, 400.0F, 1.0F}

        ' light 1

        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_POSITION, position0)
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPECULAR, specular0)
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_EMISSION, emission0)
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_DIFFUSE, diffuseLight0)
        Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_AMBIENT, ambient0)

        Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_POSITION, position1)
        Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_SPECULAR, specular1)
        Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_EMISSION, emission1)
        Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_DIFFUSE, diffuseLight1)
        Gl.glLightfv(Gl.GL_LIGHT1, Gl.GL_AMBIENT, ambient1)

        Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_POSITION, position2)
        Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_SPECULAR, specular2)
        Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_EMISSION, emission2)
        Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_DIFFUSE, diffuseLight2)
        Gl.glLightfv(Gl.GL_LIGHT2, Gl.GL_AMBIENT, ambient2)


        Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_AMBIENT, global_ambient)


        Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL)

        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT_AND_DIFFUSE, mcolor)
        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, specReflection0)
        Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, diffuseLight0)
        Gl.glColorMaterial(Gl.GL_FRONT, Gl.GL_SPECULAR Or Gl.GL_AMBIENT_AND_DIFFUSE)


        Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, 100)
        Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST)
        Gl.glEnable(Gl.GL_COLOR_MATERIAL)
        Gl.glLightModeli(Gl.GL_LIGHT_MODEL_TWO_SIDE, Gl.GL_FALSE)

        'Gl.glFrontFace(Gl.GL_CCW)
        Gl.glClearDepth(1.0F)
        Gl.glEnable(Gl.GL_DEPTH_TEST)
        Gl.glLightModelfv(Gl.GL_LIGHT_MODEL_LOCAL_VIEWER, 0.0F)
        Gl.glEnable(Gl.GL_NORMALIZE)


    End Sub

    Public Sub make_xy_grid()
        grid = Gl.glGenLists(1)
        Gl.glNewList(grid, Gl.GL_COMPILE)
        frmMain.draw_XZ_grid()
        Gl.glEndList()
    End Sub

End Module
