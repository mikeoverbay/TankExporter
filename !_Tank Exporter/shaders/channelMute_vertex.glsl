// ChannelMute_vertex.glsl
// Masks out color channels
// 
#version 130

out vec2 TC1;
void main()
    {
    TC1 = gl_MultiTexCoord0.xy;
    gl_Position = ftransform();
    }