// ShadowTest_vertex
// Use to make the shaodw mask

#version 130
out vec4 ShadowCoord;
out vec2 TC1;
uniform mat4 shadowProjection;

out vec3 vVertex;
out vec3 norm;
void main()
{
	TC1 = gl_MultiTexCoord0.xy;
    ShadowCoord= shadowProjection * gl_Vertex;
    norm = gl_NormalMatrix * gl_Normal;
    vVertex = gl_Vertex.xyz;
    gl_Position = ftransform();
}