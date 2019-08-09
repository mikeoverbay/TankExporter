#version 130
// blurBchannel_vertex
// blurs only blue channel
out vec2 TexCoords;

void main(void) {

    TexCoords = gl_MultiTexCoord0.xy;
    gl_Position    = ftransform();

}