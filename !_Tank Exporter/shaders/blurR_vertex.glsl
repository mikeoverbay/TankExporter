#version 130
// blurR_vertex
// blurs only red channel
out vec2 TexCoords;

void main(void) {

    TexCoords = gl_MultiTexCoord0.xy;
    gl_Position    = ftransform();

}