#version 330 compatibility
//dome_fragment.glsl
//Used to light dome
layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;

uniform sampler2D colorMap;

in vec3 v_Normal;
in vec2 TC1;

//===========================================================
vec3 correction(vec3 color_in){
    const float exposure = 1.6;
    vec3 mapped = vec3(1.0) - exp(-color_in * exposure);
    return mapped;
    }

vec3 getNormal()
{
    vec3 ng = normalize(v_Normal);
    gNormal = vec4(ng,1.0);
    return ng;
}
//===========================================================

void main(void){

    vec3 color = texture2D(colorMap,TC1).rgb;
    gColor.rgb = correction(color);

    gColor.a = 1.0;

    }