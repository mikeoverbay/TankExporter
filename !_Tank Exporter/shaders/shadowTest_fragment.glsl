// Shadow map rendering
// This is not used yet.
#version 130

uniform sampler2D shadowMap;
in vec4 ShadowCoord;
vec4 ShadowCoordPostW;
vec2 moments ;
float chebyshevUpperBound( float distance)
{
    moments = texture2D(shadowMap,ShadowCoordPostW.xy).rg;

    // Surface is fully lit. as the current fragment is before the light occluder
    if (distance <= moments.x)
        return 1.0 ;

    // The fragment is either in shadow or penumbra.
    // We now use chebyshev's upperBound to check
    // How likely this pixel is to be lit (p_max)
    float variance = moments.y - (moments.x*moments.x);
    variance = max(variance,0.5);

    float d = distance - moments.x;
    float p_max =  smoothstep(0.1, 0.18, variance / (variance + d*d));
    //float p_max =   variance / (variance + d*d);
    p_max = max(p_max,0.15);
    return p_max ;
}


void main()
{   
    ShadowCoordPostW = ShadowCoord / ShadowCoord.w;
    // Depth was scaled up in the depth writer so we scale it up here too.
    // This fixes precision issues.
    float shadow = chebyshevUpperBound(ShadowCoordPostW.z*5000.0);

    
    gl_FragColor.r  =  shadow;
  
}