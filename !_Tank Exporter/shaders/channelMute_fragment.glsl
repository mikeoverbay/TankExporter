// ChannelMute_vertex.glsl
// Masks out color channels
//
#version 130

uniform sampler2D colorMap;
uniform int mask;

in vec2 TC1;
out vec4 fragColor;
void main()
    {
    vec4 color = texture2D(colorMap,TC1).rgba;
    float r = float(mask & 1) ;
    float g = float((mask & 2) / 2);
    float b = float((mask & 4) / 4);
    float a = float((mask & 8) / 8);

    vec4 MASK = vec4(r,g,b,a);

    if (r+g+b == 0.0)
        {
            if (a == 1.0 )
            {
            color.rgb = vec3(color.a);
            a=0.0;
            MASK = vec4(1.0);
            }
        }
    
    fragColor = color * MASK; // Mask the textures color channels
    if (a==0.0) fragColor.a = 1.0; // dont allow 100% transparent!
    }