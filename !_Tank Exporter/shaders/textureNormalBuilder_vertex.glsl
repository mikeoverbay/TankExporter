#version 130
// textureBuilder_vertex
// used to build export texture from atlas sets
out vec2 TC1;
out vec2 TC2;
void main(void) {

    TC1 = gl_MultiTexCoord0.xy;
    TC2 = gl_MultiTexCoord1.xy;
 
    gl_Position    = gl_ModelViewProjectionMatrix * gl_Vertex;

}