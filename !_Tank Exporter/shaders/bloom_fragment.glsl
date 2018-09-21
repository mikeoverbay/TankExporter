// bloom_fragment.glsl
// Mixes final renderin with bloom texture blm_tex1
// 
#version 130

uniform sampler2D gColor;
uniform sampler2D blm_tex1;
uniform int show_bloom;
uniform int transparent;
in vec2 TC1;
out vec4 fColor;
void main()
    {
    vec4 color = texture2D(gColor, TC1.st);
    vec4 bloom = texture2D(blm_tex1, TC1.st);
    if (show_bloom == 1)
    {
        fColor.rgb = color.rgb * vec3(0.0001) + clamp(bloom.rgb *4.0, 0.0, 0.6);
    }

    else
    {
        fColor.rgb = color.rgb + clamp(bloom.rgb *2.5, 0.0, 0.6);
    }
    fColor.a = 1.0;

	if (transparent == 1)
	{
		fColor.a = color.a;
	}
	}
