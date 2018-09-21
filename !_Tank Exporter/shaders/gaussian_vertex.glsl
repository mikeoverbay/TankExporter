#version 130
// gausian_vertex
// blure shader
out vec2 TexCoords;

void main(void) {

    TexCoords = gl_MultiTexCoord0.xy;
    gl_Position    = ftransform();

}