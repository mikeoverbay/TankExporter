// ShadowTest_fragment
// Use to make the shaodw mask
#version 130

uniform sampler2D shadowMap;
uniform sampler2D normalMap;
uniform int alphaTest;
uniform int alphaRef;
uniform vec3 light_pos;

in vec2 TC1;
in vec4 ShadowCoord;
in vec3 vVertex;
vec4 ShadowCoordPostW;
vec2 moments;
in vec3 norm;
float bias;
float chebyshevUpperBound( float distance)
{
    // this clips off the depth map edge artifact.
    if (ShadowCoordPostW.x >0.997) return 1.0;
    if (ShadowCoordPostW.x <0.003) return 1.0;
    if (ShadowCoordPostW.y >0.997) return 1.0;
    if (ShadowCoordPostW.y <0.003) return 1.0;
   
    moments = texture2D(shadowMap,ShadowCoordPostW.xy).rg;

    // Surface is fully lit. as the current fragment is before the light occluder
    if (distance <= moments.x)
        return 1.0 ;

    // The fragment is either in shadow or penumbra.
    // We now use chebyshev's upperBound to check
    // How likely this pixel is to be lit (p_max)
    float variance = moments.y - (moments.x*moments.x);
    variance = max(variance,bias);

    float d = distance - moments.x;
    float p_max =  smoothstep(0.08, 0.18   , variance / (variance + d*d));
    //float p_max =   variance / (variance + d*d);
    p_max = max(p_max,0.0);
    return p_max ;
}


void main()
{   
    if (alphaTest == 1){
        float a = texture2D(normalMap, TC1.st).r;
        float aRef = float(alphaRef)/255.0;
        if (aRef > a) {
            gl_FragColor.r = 1.0;
            discard;
        }
    }
    vec3 N = normalize(norm);
    vec3 L = normalize(light_pos.xyz);
    float cosTheta =    (max(dot(N,L),-1.0)*0.985) +.05;
    bias = tan(acos(cosTheta)); // cosTheta is dot( n,l ), clamped between 0 and 1

    ShadowCoordPostW = ShadowCoord / ShadowCoord.w;
    // Depth was scaled up in the depth writer so we scale it up here too.
    // This fixes precision issues.
    float shadow = chebyshevUpperBound(ShadowCoordPostW.z*250.0);

    float t = clamp(bias,-0.65,0.65);
    float ca = 3.1;
    if ( abs(bias) > ca){
    //if (shadow >0.0) shadow =  round((ca/bias));
    }

     if (length(vVertex) > 12.0) shadow = 1.0;
    gl_FragColor.r  =  max(abs(shadow)+0.4,0.1);



  
}