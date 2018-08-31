// r2mono_vertex.glsl
// renders R channel as gray scale.
// debug
#version 130

out vec4 v_position;
out vec2 TC1;
void main()
    {
    TC1 = gl_MultiTexCoord0.xy;
    gl_Position = ftransform();
    v_position = gl_Position;
    }