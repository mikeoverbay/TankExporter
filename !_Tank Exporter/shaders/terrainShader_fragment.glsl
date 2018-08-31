#version 330 compatibility
//terrainShader_fragment.glsl
//Used to light terrain
layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gFog;

uniform sampler2D colorMap;
uniform sampler2D shadowMap;
uniform sampler2D depthMap;
uniform sampler2D normalMap;
uniform sampler2D gradientLU; // non-linear value look up
uniform sampler2D noise; // clouds

uniform vec3 camPosition;
uniform int use_shadow;
uniform float shift;// fog animation
in vec3 v_Position;
in vec3 v_Normal;
in vec3 w_Normal;

in vec2 TC1;
in vec4 ShadowCoord;
in vec3 vertex;

vec4 ShadowCoordPostW;
vec2 moments ;
const vec2 uv_Scale = vec2(20.0);
const float PI = 3.1415927;

vec3 correction(vec3 color_in){
    const float exposure = 1.6;
    vec3 mapped = vec3(1.0) - exp(-color_in * exposure);
    return mapped;
    }

float chebyshevUpperBound( float distance)
{
    // make sure we are actually on the depth texture!
    if (ShadowCoordPostW.x >0.95) return 1.0;
    if (ShadowCoordPostW.x <0.0) return 1.0;
    if (ShadowCoordPostW.y >0.85) return 1.0;
    if (ShadowCoordPostW.y <0.1) return 1.0;

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
    p_max = max(p_max,0.2);
    return p_max ;
}


vec3 getNormal()
{
    // Retrieve the tangent space matrix
    vec3 pos_dx = dFdx(v_Position);
    vec3 pos_dy = dFdy(v_Position);
    vec3 tex_dx = dFdx(vec3(TC1/20.0, 0.0));
    vec3 tex_dy = dFdy(vec3(TC1/20.00, 0.0));
    vec3 t = (tex_dy.t * pos_dx - tex_dx.t * pos_dy) / (tex_dx.s * tex_dy.t - tex_dy.s * tex_dx.t);
    vec3 ng = normalize(v_Normal);

    t = normalize(t - ng * dot(ng, t));
    vec3 b = normalize(cross(ng, t));
    mat3 tbn = mat3(t, b, ng);
    vec3 n = ng;
    n = texture2D(normalMap, TC1*uv_Scale).rgb*2.0-1.0;
    n.x*=-1.0;
    n = normalize(tbn * n);
    return n;
}

//===========================================================
void main(void){
vec2 time = vec2(float(shift));
    vec3 ambient;
    float falloff = 1.0;
    float shadow = 1.0;
    float zone = 120.0;
    float z_start = 60.0;
    float curve = 0.0;
    float alpha = 1.0;
    //=========================================================
    float dist = length(vertex.xz);
if (dist > 150.0) {
    float h = 1.0-vertex.y/30.0;
    alpha = sin(h * (PI/2));

    }
    if (dist > z_start){
        float d = (dist - z_start) / zone;
        curve = texture2D(gradientLU,vec2(1.0-d-0.01,0.5)).r;
        if (zone + z_start < dist+1.0)  curve = 1.0-texture2D(gradientLU,vec2(0.0,0.5)).r;
        }
    //=========================================================
    float y_fog = 0.0;
    float y_fog2 = 0.0;
    float range = 3.0;
    float y = vertex.y;
    if (y < 0.0){
    if (y > -range) y_fog = abs(y/range);
    y_fog2 = y_fog*.5;
    }
    y_fog *= texture2D(noise,(TC1*15)+time).r*1.5;
    y_fog = clamp(y_fog+y_fog2,0.0 , 1.0)*0.9;
    
    //=========================================================

   if (use_shadow == 1){
    ShadowCoordPostW = ShadowCoord / ShadowCoord.w;
    // Depth was scaled up in the depth writer so we scale it up here too.
    // This fixes precision issues.
    shadow = chebyshevUpperBound(ShadowCoordPostW.z*5000.0);
    }
    vec3 color = texture2D(colorMap,TC1*uv_Scale).rgb;
    vec3 n = getNormal();// normal at surface point
    vec3 v = normalize(camPosition-v_Position);// Vector from surface point to camera
    vec3 r = normalize(reflect(-v,n));

    vec3 sum = vec3(0.0);
    float NdotL;
vec3 spec;
    for (int i = 0; i<1; i++){
        vec3 u_LightDirection = gl_LightSource[i].position.xyz;
        vec3 l = normalize(u_LightDirection - v_Position);// Vector from surface point to light
        
        spec = vec3(1.0) * pow(max(dot(r,l),0.0),4.0) * falloff * 0.030;
        float len = length(v_Position - u_LightDirection);
        float d = 220;
        if (len < d) { falloff = 1.0-(len/d); }
        NdotL = clamp(dot(n, l), 0.0, 1.0);
        ambient = (color.rgb - (color.rgb * vec3(NdotL))) * falloff * 0.45;//adjust ambient with distance
        sum +=  (color.rgb* shadow) * NdotL * 1.1;
        sum += spec;
    }
    ;//sum *= vec3(shadow);
    gColor.rgb = correction(sum + ambient + ( vec3(y_fog) * vec3(0.5,0.5,0.45) ) );
   
    gColor.rgb *= (vec3(1.0-curve));
    //==========================================================================
    // FOG calculation... using distance from camera and height on map.
    // It's a more natural height based fog than plastering the screen with it.
    float z = length(vertex);
    float height = sin(1.0-( (vertex.y+3.0)/50.0)*(3.1415/2.0));
    const float LOG2 = 1.442695;


    float density = (gl_Fog.density * height) * 0.03;
    float fogFactor = exp2(-density * density * z * z * LOG2);
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    y_fog *= 0.3;
    vec4 fog_color = vec4(0.35)+vec4(vec3(y_fog),1.0);
    //fog.r = moving fog.
    // fog.b = exp fog;
     gFog.rgb = vec3(y_fog);
     gFog.rgb *= (vec3(1.0-curve));

    //gFog.rgb = mix(fog_color.rgb, gFog.rgb, fogFactor );
    gFog.a = 1.0;
    gColor = mix(fog_color, gColor, fogFactor );
    gColor.a = alpha;

    gNormal.rgb = (w_Normal *0.5)+0.5;// we need this for the decal shader!!
    gNormal.a = 1.0;    
    //gColor.rgb = (gColor.rgb * vec3(0.0025)) + vec3(gFog);

    }