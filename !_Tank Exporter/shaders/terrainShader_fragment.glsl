#version 330 compatibility
//terrainShader_fragment.glsl
//Used to light terrain
layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gFog;
layout (location = 3) out vec4 surface_normal;

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
in float l_dist; // used to clip the shadow map
const vec2 uv_Scale = vec2(20.0);
const float PI = 3.1415927;

vec3 correction(vec3 color_in){
    const float exposure = 1.6;
    vec3 mapped = vec3(1.0) - exp(-color_in * exposure);
    return mapped;
    }



vec3 getNormal()
{
    // Retrieve the tangent space matrix
    vec3 pos_dx = dFdx(v_Position);
    vec3 pos_dy = dFdy(v_Position);
    vec3 tex_dx = dFdx(vec3(TC1*uv_Scale, 0.0));
    vec3 tex_dy = dFdy(vec3(TC1*uv_Scale, 0.0));
    vec3 t = (tex_dy.t * pos_dx - tex_dx.t * pos_dy) / (tex_dx.s * tex_dy.t - tex_dy.s * tex_dx.t);
    vec3 ng = normalize(v_Normal);

    t = normalize(t - ng * dot(ng, t));
    vec3 b = normalize(cross(ng, t));
    mat3 tbn = mat3(t, b, ng);
    vec3 n = ng;
    n = texture(normalMap, TC1*uv_Scale, 1.0).rgb*2.0-1.0;
    n.x*=-1.0;
    n = normalize(tbn * n);
    return n;
}

//===========================================================
void main(void){
vec2 time = vec2(float(shift));
    vec3 ambient;
    float falloff = 0.0;
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
    y_fog *= texture2D(noise,(TC1*5)+time).r*1.5;
    y_fog = clamp(y_fog+y_fog2,0.0 , 1.0)*0.9;
    
    //=========================================================
    float shadow=1.0;
     if (use_shadow == 1 && l_dist < 12.0) shadow = texelFetch(shadowMap, ivec2(gl_FragCoord.xy), 0).r;

   vec3 color = texture2D(colorMap,TC1*uv_Scale).rgb*shadow;
    vec3 n = getNormal();// normal at surface point
    vec3 v = normalize(camPosition-v_Position);// Vector from surface point to camera
    vec3 r = normalize(reflect(-v,n));

    vec3 sum = vec3(0.0);
    float NdotL;
vec3 spec;
    for (int i = 0; i<2; i++){
        vec3 u_LightDirection = gl_LightSource[0].position.xyz+ vec3(0.0 , i*25.0,0.0);
        vec3 l = normalize(u_LightDirection - v_Position);// Vector from surface point to light
        
        float len = length(v_Position - u_LightDirection);
        float d = 5000;
        if (len < d) { falloff = 1.0-(len/d); }

        spec = vec3(1.0) * pow(max(dot(r,l),0.0),4.0) * falloff * 0.030;
        NdotL = clamp(dot(n, l), 0.0, 1.0);
        ambient = (color.rgb - (color.rgb * vec3(NdotL))) * falloff * 0.045;//adjust ambient with distance
        sum +=  (color.rgb) * NdotL ;
        sum += spec;
    }
    ;//sum *= vec3(shadow);
    gColor.rgb = correction(sum + ambient + ( vec3(y_fog) * vec3(0.5,0.5,0.45) ) )*0.8;
   
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
    gColor = mix(fog_color * falloff , gColor, fogFactor );


    gColor.a = alpha;

    gNormal.xyz = w_Normal;// we need this for the decal shader!!
    gNormal.a = 1.0;
	surface_normal.xyz = v_Normal;
     gNormal.a = 1.0;
   //gColor.rgb = (gColor.rgb * vec3(0.0025)) + vec3(gFog);

    }