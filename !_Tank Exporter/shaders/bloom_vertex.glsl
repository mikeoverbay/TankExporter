// bloom_vertex.glsl
// Mixes final renderin with bloom texture blm_tex1
// 
#version 130

out vec2 TC1;
void main()
    {
    TC1 = gl_MultiTexCoord0.xy;
    gl_Position = ftransform();
    }