//atlasPBR_fragment.glsl
//Used to light detail models
#version 430 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 blmColor;

uniform sampler2D colorMap; // not on all models
uniform sampler2D normalMap;
uniform sampler2D GMM_Map;
uniform sampler2D detailMap;
uniform samplerCube cubeMap;
uniform sampler2D u_brdfLUT; // loaded resource

uniform vec4 g_detailRejectTiling;
uniform vec4 g_detailInfluences; // Size of Atlas cells X,Y

uniform float A_level; //Ambient level
uniform float S_level; //Specular level
uniform float T_level; //Brightness level

uniform vec3 camPosition;

uniform int alpha_enable;
uniform int alpha_value;
uniform int is_glass;
uniform int has_detail_map;

uniform vec4 u_ScaleFGDSpec; // display switches
uniform vec4 u_ScaleDiffBaseMR; // display switches

in vec2 TC1;// UV coords
in vec2 TC2;// UV2 coords

in vec3 vVertex;
in vec3 Normal;

out vec4 color_out;

float mininput = 0.0;
float maxinput = 1.0;
float minoutput = 0.0;
float maxoutput = 0.95;

const float PI = 3.14159265359;

vec4 correct(in vec4 hdrColor, in float exposure, in float gamma_level){  
    // Exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    // Gamma correction 
    mapped.rgb = pow(mapped.rgb, vec3(1.0 / gamma_level));  
    return vec4 (mapped, 1.0);
}

vec3 getNormalFromMap(in vec3 tangentNormal, in vec2 TexCoords)
    {

    vec3 Q1 = dFdx(vVertex);
    vec3 Q2 = dFdy(vVertex);
    vec2 st1 = dFdx(TexCoords);
    vec2 st2 = dFdy(TexCoords);

    vec3 N = normalize(Normal);
    vec3 T = normalize(Q1*st2.t - Q2*st1.t);
    vec3 B = -normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    return normalize(TBN * tangentNormal);
    }
// PBR Functions ======================================================
vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return max(F0 + (1.0 - F0) * pow(1.0 - min(cosTheta,1.0), 5.0), 0.0);
}
// ----------------------------------------------------------------------------
vec3 fresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness)
{
    return F0 + (max(vec3(1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}   
// ----------------------------------------------------------------------------
float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a      = roughness*roughness;
    float a2     = a*a;
    float NdotH  = max(dot(N, H), 0.0);
    float NdotH2 = NdotH*NdotH;
    
    float num   = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
    return num / denom;
}
// ----------------------------------------------------------------------------
float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r*r) / 8.0;

    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
    return num / denom;
}
// ----------------------------------------------------------------------------
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
    float ggx1  = GeometrySchlickGGX(NdotL, roughness);
    
    return ggx1 * ggx2;
}
// ----------------------------------------------------------------------------
void main(void) {
    float a;
    vec3 bump;
    const float MAX_REFLECTION_LOD = 9.0;
    //============================================
    vec4 color = texture2D(colorMap,TC1);
    vec4 base = color;
    vec4 bumpMap = texture2D(normalMap,TC1);
    vec4 GMM = texture2D(GMM_Map,TC1);
    vec4 detail = texture2D(detailMap,TC1);
    vec4 detail_mix;

    float roughness = 1.0-GMM.r*GMM.b;
    float metallic = GMM.g*GMM.b;
    if (is_glass == 1)
    {
        //gColor += spec_sum * vec4(1.0);
     roughness = 0.25*color.r;
    } 
    float spec_from_color_alpha = color.a;
    //============================================
    if (has_detail_map > 0)
    {
        detail_mix.rgb = mix(color.rgb,vec3(detail.r),g_detailInfluences.x);
        detail_mix.rgb = mix(detail_mix.rgb,vec3(detail.g),g_detailInfluences.y);
        detail_mix.rgb = mix(detail_mix.rgb,vec3(detail.b),g_detailInfluences.z);
        //not sure this is even close!
        //color.rgb /= detail_mix.rgb;
    }
    //============================================
    if (alpha_enable == 1)
    {
    float aRef = float(alpha_value)/255.0;
    if (aRef > bumpMap.r) {
        discard;
    }
    }
    //============================================
    //calc bump from GA map
    a=bumpMap.r;
    vec2 tb = vec2(bumpMap.ga * 2.0 - 1.0);
    bump.xy    = tb.xy;
    bump.z     = clamp(sqrt(1.0 - ((tb.x*tb.x)+(tb.y*tb.y))),-1.0,1.0);
    bump       = normalize(bump);
    bump = getNormalFromMap(bump,TC1);

   //============================================
   // Lighting calculations...
    vec3 albedo = pow(color.rgb,vec3(2.2));
    vec3 N = bump;
    vec3 V = normalize(-vVertex);

    vec3 F0 = vec3(0.04); // metal mateials are darkened.
    F0 = mix(F0, albedo, metallic);
    
    vec3 LightColor = vec3(2.0*T_level);
    vec3 Lo = vec3(0.0);
    vec3 specular;
    vec3 F;
    float G;
    vec3 R;
    vec3 diffuse;
    float NdotL;
for(int i = 0; i < 3; ++i) 
    {
        vec3 L = normalize(gl_LightSource[i].position.xyz - vVertex);
        vec3 H = normalize(V + L);
  
        float distance    = length(gl_LightSource[i].position.xyz - vVertex);
        float attenuation = 1500.0 / (distance * distance); //Shutting atenuation off
        vec3 radiance     = LightColor * attenuation;

        float NDF = DistributionGGX(N, H, roughness);
        G  = GeometrySmith(N, V, L, roughness);

        F  = fresnelSchlick(max(dot(H, V), 0.0), F0);

        vec3 kS = F;
        vec3 kD = vec3(1.0) - kS;
        kD *= 1.0 - metallic;

        vec3 numerator    = NDF * G * F;
        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
        NdotL = max(dot(N, L), 0.0);

    diffuse = NdotL * albedo;

        specular = numerator / max(denominator, 0.001)* S_level*0.75;
        blmColor = vec4(specular,1.0)*1.0;
  
        Lo += ((kD * albedo / PI) + specular) * radiance * NdotL;
    }// light loop

    F  = fresnelSchlickRoughness(max(dot(N, V), 0.0), F0, roughness);
    
    vec3 kS = F;
    vec3 kD = 1.0 - kS;
    kD *= 1.0 - metallic;


    R = reflect(-V,N);
    R.xz *= -1.0;
    vec3 prefilteredColor = pow(textureLod(cubeMap, R,  roughness * MAX_REFLECTION_LOD).rgb,vec3(2.2));    
    vec2 brdf  = texture(u_brdfLUT, vec2(max(dot(N, V), 0.0), roughness)).rg;
    specular = prefilteredColor * (F * brdf.x + brdf.y)*0.5;
  
    vec3 ambient = (kD * diffuse + specular) * A_level*0.20;
    vec3 col = ambient + Lo;
    
    col= col / (col + vec3(1.0));
    col = pow(col, vec3(1.0/2.2)) * 2.5;  
   
    gColor = vec4(col, 0.0);
    vec3 acolor = min(max(gColor.rgb - vec3(mininput), vec3(0.0)) / (vec3(maxinput) - vec3(mininput)), vec3(1.0));
    gColor.rgb = mix(vec3(minoutput), vec3(maxoutput), acolor);

    if (is_glass == 1)
    {
        gColor += vec4(0.0,0.05,0.0,0.0);
        gColor.a = (1.0-GMM.g*0.825);
    } 

} //main