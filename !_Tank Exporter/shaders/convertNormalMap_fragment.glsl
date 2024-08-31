// convertNormalMap_fragment
// Converts Bigworld AG normalmaps in to RGB maps.

#version 130

uniform sampler2D map;

uniform int convert;
uniform int flip_y;
uniform int alpha_enabled;

in vec2 TC1;
out vec4 color_out;

void main(void){

vec4 m = texture2D(map,TC1);
    vec4 tn = m;
    tn.xy = tn.ga*2.0-1.0;
    tn.z =  sqrt(1.0 - clamp( ((tn.x*tn.x) + (tn.y*tn.y)) ,-1.0,1.0) );

    if (flip_y == 1 ) { tn.y *=-1.0; }

	if (convert == 1)
	{
		color_out.rgb =  tn.rgb*0.5+0.5;

		if (alpha_enabled == 1) {color_out.a = m.r;}
		if (alpha_enabled == 0) {color_out.a = 1.0;}

	}
	else
	{
		color_out = m;
		color_out.a = 1.0;
	}
}