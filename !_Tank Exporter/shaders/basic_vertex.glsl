// basic_vertex.glsl
// writes to depth texture.
// debug
#version 130

out vec2 TC1;
out vec4 v_position;
void main()
    {
    TC1 = gl_MultiTexCoord0.xy;
    gl_Position = ftransform();
    v_position = gl_Position;
	}