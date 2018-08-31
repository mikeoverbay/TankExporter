// mixer_fragment.glsl
#version 150
uniform sampler2D camoMap;
uniform vec4 c0;
uniform vec4 c1;
uniform vec4 c2;
uniform vec4 c3;

uniform vec4 armorColor;
in vec2 TC1;


out vec4 color_out;
void main(void) {

    vec4 camoTexture = texture2D(camoMap, TC1.st );
    vec4 cc = vec4(0.0);
    cc = armorColor ;
    cc = mix(cc, c0 , camoTexture.r * c0.a);
    cc = mix(cc, c1 , camoTexture.g * c1.a);
    cc = mix(cc, c2 , camoTexture.b * c2.a);
    cc = mix(cc, c3 , camoTexture.a * c3.a);
                
    gl_FragColor = cc;

}

