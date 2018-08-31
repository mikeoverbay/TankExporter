// r2mono_fragment.glsl
// renders R channel as gray scale.
// debug
#version 130

uniform sampler2D shadow;

in vec4 v_position;
in vec2 TC1;

void main(){

vec3 shade = vec3(texture2D(shadow,TC1).r);
if (shade.r == 0.0) shade.b = 0.4;
gl_FragColor.rgb = shade;
gl_FragColor.a = 1.0;

}