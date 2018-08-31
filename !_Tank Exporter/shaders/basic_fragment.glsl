// basic_fragment.glsl
// writes to depth texture.
// debug
#version 130

uniform sampler2D normalMap;

uniform int alphaTest;
uniform int alphaRef;
in vec4 v_position;
in vec2 TC1;
out vec4 fColor;

void main()
    {

    // figure out if we need to discard this.
    if (alphaTest == 1){
        float a = texture2D(normalMap, TC1.st).r;
        float aRef = float(alphaRef)/255.0;
        if (aRef > a) {
            discard;
        }
    }

	fColor.rgb = texture2D(normalMap, TC1.st).rgb;
	fColor.a = 1.0;
    }