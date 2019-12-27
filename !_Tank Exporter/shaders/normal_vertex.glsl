// normal drawing vertex shader
#version 330 compatibility
out vData
{
    vec3 normal;
    vec4 vert;
}vertex;

void main()
{
    vertex.normal = gl_Normal;
    vertex.vert = gl_Vertex;
}