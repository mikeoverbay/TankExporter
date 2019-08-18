#version 130
//AtlasPBR_vertex.glsl
//Used to light primitive models

out vec2 TC1;
out vec2 TC2;

out vec3 vVertex;
out vec3 Normal;
void main(void) {

    TC1 = gl_MultiTexCoord0.xy;
    TC2 = gl_MultiTexCoord4.xy;

    vec3 n = normalize(gl_NormalMatrix * gl_Normal);
    Normal = n;

    vVertex = vec3(gl_ModelViewMatrix * gl_Vertex);
    gl_Position    = ftransform();

}