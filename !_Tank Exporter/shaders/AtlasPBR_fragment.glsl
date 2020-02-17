//atlasPBR_fragment.glsl
//Used to light primitive models
#version 330 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 blmColor;

uniform sampler2D ATLAS_AM_Map; // not on all models
uniform sampler2D ATLAS_GBMT_Map;
uniform sampler2D ATLAS_MAO_Map;

uniform sampler2D AM_Map; // not on all models
uniform sampler2D GBMT_Map;
uniform sampler2D MAO_Map;

uniform samplerCube cubeMap; // loaded resource
uniform sampler2D u_brdfLUT; // loaded resource

uniform sampler2D ATLAS_BLEND_MAP;
uniform sampler2D ATLAS_DIRT_MAP;

uniform sampler2D colorMap; // not on all models
uniform sampler2D colorMap2;
uniform sampler2D normalMap;
uniform sampler2D GMM_map;

uniform vec4 g_tile0Tint; // tints. Not all have these
uniform vec4 g_tile1Tint;
uniform vec4 g_tile2Tint;
uniform vec4 g_dirtColor;
uniform vec4 dirt_params;

uniform vec3 camPosition;

uniform int IS_ATLAS; // set if this uses atlas textures
uniform int use_UV2; // set if this model has a UV2 texture
uniform int use_normapMAP; // normalMap in visual
uniform int is_ANM_Map; // _ANN normapMap in visual

uniform vec4 atlas_TILE; // Not used
uniform vec4 atlas_sizes; // Size of Atlas cells X,Y
uniform vec4 atlas_indexes; // cell selection in Atlas X,Y,Z and A

uniform float A_level; //Ambient level
uniform float S_level; //Specular level
uniform float T_level; //Brightness level

uniform vec4 u_ScaleFGDSpec; // display switches
uniform vec4 u_ScaleDiffBaseMR; // display switches


uniform vec2 UV_tiling; //how many times the texture repetes

uniform vec2 image_size;
uniform int alpha_enable;
uniform int alpha_value;

in vec2 TC1;// UV coords
in vec2 TC2;// UV2 coords

in mat3 TBN;
in vec3 vVertex;

//in mat3 TBN;
in vec3 Normal;
float mininput = 0.0;
float maxinput = 1.0;
float minoutput = 0.0;
float maxoutput = 0.9;

const float PI = 3.14159265359;

float mip_map_level(in vec2 texture_coordinate)
{
    vec2  dx_vtc        = dFdx(texture_coordinate);
    vec2  dy_vtc        = dFdy(texture_coordinate);
    float delta_max_sqr = max(dot(dx_vtc, dx_vtc), dot(dy_vtc, dy_vtc));
    
    return 0.5 * log2(delta_max_sqr); 
    return 5.0;
}

vec4 correct(in vec4 hdrColor, in float exposure, in float gamma_level){  
    // Exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    // Gamma correction 
    mapped.rgb = pow(mapped.rgb, vec3(1.0 / gamma_level));  
    return vec4 (mapped, hdrColor.a);
}

vec3 getNormalFromMap(in vec3 tangentNormal)
    {
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
    float specPower = 10.0;
    float metallic;
    float roughness;
    float a;
    vec4 Ispec1 = vec4(0.0);
    vec4 Idiff1 = vec4(0.0);
    vec4 sum = vec4(0.0);
    vec4 color;
    vec3 bump;
    vec2 UV1;
    vec2 UV2;
    vec2 UV3;
    vec2 UV4;
    vec2 UV4_T;
    vec2 tile;
    const float MAX_REFLECTION_LOD = 9.0;
    //============================================
    // Calculate UVs based on indexes ============
    // dont touch these!!!!
    vec4 At_size = atlas_sizes;
    //hack to fix missing atlas sizes in visual.
    //Not sure this is good for all Models!
    if (At_size.x + At_size.y + At_size.z + At_size.w == 0.0)
        { 
        At_size.x = 4.0;
        At_size.y = 4.0;
        At_size.z = 8.0;
        At_size.w = 4.0;
        }
    vec2 tc = TC1/round(UV_tiling);

    //============================================
    float uox = 0.0625;
    float uoy = 0.0625;

    float usx = 0.875;
    float usy = 0.875;
    vec2 hpix = vec2(0.5/image_size.x,0.5/image_size.x);// / At_size.xy;
    vec2 offset = vec2(uox/At_size.x, uoy/At_size.y) + hpix;

    //============================================
    //common scale for UV1, UV2 and UV3
    vec2 UVs;
    float scaleX = 1.0 / At_size.x;
    float scaleY = 1.0 / At_size.y;
    UVs.x = fract(TC1.x)*scaleX*usx; 
    UVs.y = fract(TC1.y)*scaleY*usy;
    
    //============================================

    float index = atlas_indexes.x;
    tile.y = floor(index/At_size.x);
    tile.x = index - tile.y * At_size.x;
    UV1.x = UVs.x + offset.x + tile.x * scaleX;
    UV1.y = UVs.y + offset.y + tile.y * scaleY;

    index = atlas_indexes.y;
    tile.y = floor(index/At_size.x);
    tile.x = index - tile.y * At_size.x;
    UV2.x = UVs.x + offset.x + tile.x * scaleX;
    UV2.y = UVs.y + offset.y + tile.y * scaleY;

    index = atlas_indexes.z;
    tile.y = floor(index/At_size.x);
    tile.x = index - tile.y * At_size.x;
    UV3.x = UVs.x + offset.x + tile.x * scaleX;
    UV3.y = UVs.y + offset.y + tile.y * scaleY;

    //UV4 is used for blend.
    scaleX = 1.0 / At_size.z;
    scaleY = 1.0 / At_size.w;

    index = atlas_indexes.w;
    tile.y = floor(index/At_size.z);
    tile.x = index - tile.y * At_size.z;

    offset = vec2(uox/At_size.z, uoy/At_size.w);

    UV4.x = (fract(TC2.x)*scaleX)+tile.x*scaleX;
    UV4.y = (fract(TC2.y)*scaleY)+tile.y*scaleY;
    UV4_T.x = (fract(tc.x)*scaleX)+tile.x;
    UV4_T.y = (fract(tc.y)*scaleX)+tile.y;
    //============================================
    //============================================
    vec2 dirt_scale = vec2(dirt_params.y,dirt_params.z);
    float dirt_blend = dirt_params.x;
    vec4 BLEND;
        if (IS_ATLAS == 1)
        {
            BLEND = texture2D(ATLAS_BLEND_MAP,UV4);
        }
        else
        {
            BLEND = texture2D(ATLAS_BLEND_MAP,UV4_T);
        }

    float mip = mip_map_level(TC1*image_size)*0.5;

    vec4 colorAM_1 = texture2DLod(ATLAS_AM_Map,UV1,mip) * g_tile0Tint;
    vec4 GBMT_1 =    texture2DLod(ATLAS_GBMT_Map,UV1,mip);
    vec4 MAO_1 =     texture2DLod(ATLAS_MAO_Map,UV1,mip);

    vec4 colorAM_2 = texture2DLod(ATLAS_AM_Map,UV2,mip) * g_tile1Tint;
    vec4 GBMT_2 =    texture2DLod(ATLAS_GBMT_Map,UV2,mip);
    vec4 MAO_2 =     texture2DLod(ATLAS_MAO_Map,UV2,mip);

    vec4 colorAM_3 = texture2DLod(ATLAS_AM_Map,UV3,mip) * g_tile2Tint;
    vec4 GBMT_3 =    texture2DLod(ATLAS_GBMT_Map,UV3,mip);
    vec4 MAO_3 =     texture2DLod(ATLAS_MAO_Map,UV3,mip);
   
    vec4 DIRT = texture2DLod(ATLAS_DIRT_MAP,UV4,mip);
    DIRT.rgb *= g_dirtColor.rgb;
    
    //non-atlas textures
    vec4 basic_color =  texture2D(colorMap,TC1);
    vec4 basic_color2 = texture2D(colorMap2,TC2);
    vec4 bumpMap =      texture2D(normalMap,TC1);
    vec4 GMM =          texture2D(GMM_map,TC1);
    //============================================
    // Some 40 plus hours of trial and error to get this working.
    // The mix texture has to be compressed down/squished.
    BLEND.r = smoothstep(BLEND.r*colorAM_1.a,0.00,0.09);
    BLEND.g = smoothstep(BLEND.g*colorAM_2.a,0.00,0.25);
    BLEND.b = smoothstep(BLEND.b,0.00,0.6);// uncertain still... but this value seems to work well
    BLEND = correct(BLEND,4.0,0.8);
    //============================================
     vec4 colorAM = colorAM_3;
          colorAM = mix(colorAM,colorAM_1, BLEND.r);
          colorAM = mix(colorAM,colorAM_2, BLEND.g);
          
          colorAM = mix(colorAM,DIRT, BLEND.b);
          colorAM *= BLEND.a;

    vec4 GBMT = GBMT_3;
         GBMT = mix(GBMT, GBMT_1, BLEND.r);
         GBMT = mix(GBMT, GBMT_2, BLEND.g);
  
    vec4 MAO = MAO_3;
         MAO = mix(MAO, MAO_1, BLEND.r);
         MAO = mix(MAO, MAO_2, BLEND.g);
    //============================================
    if (use_normapMAP == 0)
    {
        bumpMap = GBMT;
        roughness = 1.1-GBMT.r;
        if (GBMT.b > 0.03 ) roughness = 1.0-GBMT.b;
        
        metallic = MAO.r;
    }
    else
    {   //these are backwards and blue is a mask!
        roughness = 1.0-GMM.r;
        if (GMM.b > 0.03 ) roughness = 1.0-GMM.b;
        metallic = GMM.g;
    }
    vec2 tb = vec2(bumpMap.ga * 2.0 - 1.0);
    bump.xy    = tb.xy;
    bump.z     = clamp(sqrt(1.0 - ((tb.x*tb.x)+(tb.y*tb.y))),-1.0,1.0);
    bump       = normalize(bump);
    //============================================
    if (IS_ATLAS == 1){        
            color = colorAM * MAO.g;
        }
        else
        {
        colorAM.rgb = basic_color.rgb*0.65;
        color = colorAM;
        //UV2 textures are not used on most models other than the blend mapping.
        //Some do have a 2nd texture that needs to be used.
        if (use_UV2 == 1) {
            color.rgb = basic_color.rgb * basic_color2.rgb;
            }
        }
    a=bumpMap.r;
    if (alpha_enable == 1)
        {
        colorAM *= a; // reduces black out lines
        float aRef = float(alpha_value)/255.0;
        if (aRef > a) {
            discard;
            }
        }
//metallic *= a;
//roughness *= 1.0*a;
    //============================================
    // Lighting calculations...
    vec3 albedo = pow(colorAM.rgb,vec3(2.2));
    vec3 N = normalize(TBN * bump);
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
    vec3 LP = gl_LightSource[i].position.xyz * vec3(1.0,0.5,1.0);
    vec3 L = normalize(LP - vVertex);
    vec3 H = normalize(V + L);
  
    float distance    = length(LP - vVertex);
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


    specular = numerator / max(denominator, 0.001)* S_level * 1.0;
    blmColor = vec4(specular,1.0)*(1.0-roughness);
    diffuse += NdotL * albedo+albedo*0.05;
  
    Lo += ((kD * albedo / PI) + specular) * radiance * NdotL;
    }

    F  = fresnelSchlickRoughness(max(dot(N, V), 0.0), F0, roughness);
    
    vec3 kS = F;
    vec3 kD = 1.0 - kS;
    kD *= 1.0 - metallic;


    R = reflect(-V,N);
    R.xz *= -1.0;
    vec3 prefilteredColor = pow(textureLod(cubeMap, R,  roughness * MAX_REFLECTION_LOD).rgb,vec3(2.2));    
    vec2 brdf  = texture(u_brdfLUT, vec2(max(dot(N, V), 0.0), roughness)).rg;
    specular = prefilteredColor * (F * brdf.x + brdf.y)*(1.0-roughness);
  
    vec3 ambient = (kD * diffuse + specular) * A_level*1.0;
    vec3 col = ambient + Lo;
    
    col= col / (col + vec3(1.0));
    col = pow(col, vec3(1.0/2.2)) * 1.1;  
   
    gColor = vec4(col, 1.0);
    vec3 acolor = min(max(gColor.rgb - vec3(mininput), vec3(0.0)) / (vec3(maxinput) - vec3(mininput)), vec3(1.0));
    gColor.rgb = mix(vec3(minoutput), vec3(maxoutput), acolor);
    gColor = correct(gColor,1.8,0.9);
    //gColor.rgb = gColor.rgb * 0.001 + bump;
}//main