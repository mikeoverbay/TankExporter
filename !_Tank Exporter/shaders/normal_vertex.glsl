varying vec3 normal;      
 
void main(void)
{
    gl_Position = gl_Vertex;
    normal = gl_Normal;
    gl_FrontColor = gl_Color;
}