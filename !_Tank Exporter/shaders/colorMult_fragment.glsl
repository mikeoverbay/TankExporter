// ColorMult_fragment.glsl
// multiplies color1 by color2
// 
#version 130
uniform sampler2D color1;
uniform sampler2D color2;

in vec2 TC1;
out vec4 fColor;
void main()
    {
    vec4 color1_ = texture2D(color1, TC1.st);
    vec4 color2_ = texture2D(color2, TC1.st);
    fColor.rgb = color1_.rgb* clamp(color2_.rgb+0.2,0.0,1.0);
    //fColor.rgb = color1_.rgb + color2_.rgb;
    fColor.a = 1.0;
}
