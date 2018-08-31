#version 130
//dome_vertex.glsl
//Used to light dome

out vec3 v_Position;
out vec3 v_Normal;
out vec2 TC1;

void main(void){

    TC1 = gl_MultiTexCoord0.xy;
    v_Normal = gl_NormalMatrix * gl_Normal;
    gl_Position    = ftransform();

}