// Used for shadow matrix lookup and translation
// This is not used yet.
#version 130
out vec4 ShadowCoord;

uniform mat4 shadowProjection;

out vec3 vVertex;
void main()
{
    ShadowCoord= shadowProjection * gl_Vertex;
    gl_Position = ftransform();
}