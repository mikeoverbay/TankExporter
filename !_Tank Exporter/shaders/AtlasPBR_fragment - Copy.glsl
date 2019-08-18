//atlasPBR_fragment.glsl
//Used to light primitive models
#version 430 compatibility

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
uniform int use_UV2; // set if this model has a UV2 channel
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

in vec3 vVertex;

in mat3 TBN;

float mininput = 0.0;
float maxinput = 1.0;
float minoutput = 0.0;
float maxoutput = 0.9;

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

// PBR Functions ======================================================
// Encapsulate the various inputs used by the various functions in the shading equation
// We store values in this struct to simplify the integration of alternative implementations
// of the shading terms, outlined in the Readme.MD Appendix.
// See the Tank Fragment shader for more info about this PBS implementation.

struct PBRInfo
{
    float NdotL;
    // cos angle between normal and light direction
    float NdotV;
    // cos angle between normal and view direction
    float NdotH;
    // cos angle between normal and half vector
    float LdotH;
    // cos angle between light direction and half vector
    float VdotH;
    // cos angle between view direction and half vector
    float perceptualRoughness;
    // roughness value, as authored by the model creator (input to shader)
    float metalness;
    // metallic value at the surface
    vec3 reflectance0;
    // full reflectance color (normal incidence angle)
    vec3 reflectance90;
    // reflectance color at grazing angle
    float alphaRoughness;
    // roughness mapped to a more linear change in the roughness (proposed by [2])
    vec3 diffuseColor;
    // color contribution from diffuse lighting
    vec3 specularColor;
    // color contribution from specular lighting
};
const float M_PI = 3.141592653589793;
const float c_MinRoughness = 0.13;
// used for debug.. shows different parts of the lighting

#define MANUAL_SRGB;
#define SRGB_FAST_APPROXIMATION;
vec4 SRGBtoLINEAR(vec4 srgbIn)
{
    #ifdef MANUAL_SRGB
    #ifdef SRGB_FAST_APPROXIMATION
    vec3 linOut = pow(srgbIn.xyz,vec3(2.2));
    #else //SRGB_FAST_APPROXIMATION
    vec3 bLess = step(vec3(0.04045),srgbIn.xyz);
    vec3 linOut = mix( srgbIn.xyz/vec3(12.92), pow((srgbIn.xyz+vec3(0.055))/vec3(1.055),vec3(2.4)), bLess );
    #endif //SRGB_FAST_APPROXIMATION
    return vec4(linOut,srgbIn.w);
    ;
    #else //MANUAL_SRGB
    return srgbIn;
    #endif //MANUAL_SRGB
}

vec3 getIBLContribution(PBRInfo pbrInputs, vec3 n, vec3 reflection)
{
    float mipCount = 9.0;
    // resolution of 512x512
    float lod = ((1.0-pbrInputs.perceptualRoughness) * mipCount);
    // retrieve a scale and bias to F0. See [1], Figure 3
    vec3 brdf = SRGBtoLINEAR(texture2D(u_brdfLUT, vec2(pbrInputs.NdotV*0.1, (1.0 - pbrInputs.perceptualRoughness)*0.1))).rgb;
    vec3 diffuseLight = SRGBtoLINEAR(textureCubeLod(cubeMap, n, 7)).rgb;
    reflection.xz *= -1.0;
    // like so many other things, DirectX to OpenDL causes axis issues.
    vec3 specularLight = SRGBtoLINEAR(textureCubeLod(cubeMap, reflection, lod)).rgb;
    vec3 diffuse = diffuseLight * pbrInputs.diffuseColor;
    vec3 specular = specularLight * (pbrInputs.specularColor * brdf.x + brdf.y) * S_level;
    // For presentation, this allows us to disable IBL terms
    return diffuse + specular;
}

vec3 diffuse(PBRInfo pbrInputs)
{
    return pbrInputs.diffuseColor /M_PI;
}

vec3 specularReflection(PBRInfo pbrInputs)
{
    return pbrInputs.reflectance0 + (pbrInputs.reflectance90 - pbrInputs.reflectance0) * pow(clamp(1.0 - pbrInputs.VdotH, 0.0, 1.0), 1.5);
}

float geometricOcclusion(PBRInfo pbrInputs)
{
    float NdotL = pbrInputs.NdotL;
    float NdotV = pbrInputs.NdotV;
    float r = 0.3;
    //pbrInputs.alphaRoughness;
    float attenuationL = 2.0 * NdotL / (NdotL + sqrt(r * r + (1.0 - r * r) * (NdotL * NdotL)));
    float attenuationV = 2.0 * NdotV / (NdotV + sqrt(r * r + (1.0 - r * r) * (NdotV * NdotV)));
    return attenuationL * attenuationV;
}


float microfacetDistribution(PBRInfo pbrInputs)
{
    float roughnessSq = 0.05;
    //pbrInputs.alphaRoughness * pbrInputs.alphaRoughness;
    float f = (pbrInputs.NdotH * roughnessSq - pbrInputs.NdotH) * pbrInputs.NdotH + 1.0;
    return roughnessSq / (M_PI * f * f);
}
// end PBR functions ===============================================================


void main(void) {
    float specPower = 10.0;
    float metal;
    float gloss;
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
    float spec_from_color_alpha;
    //============================================
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
    vec4 GMM =          texture2D(GMM_map,UV1);
    if (alpha_enable == 1)
    {
    float aRef = float(alpha_value)/255.0;
    if (aRef > bumpMap.r) {
        discard;
    }
    }
    //============================================
    // Some 40 plus hours of trial and error to get this working.
    // The mix texture has to be compressed down/squished.
    BLEND.r = smoothstep(BLEND.r*colorAM_1.a,0.00,0.09);
    BLEND.g = smoothstep(BLEND.g*colorAM_2.a,0.00,0.25);
    BLEND.b = smoothstep(BLEND.b,0.00,0.6);// uncertain still... but this value seems to work well
    BLEND = correct(BLEND,4.0,0.8);

    //============================================
     vec4 colorAM = colorAM_3; //colorAM_1;// - (colorAM_1 * BLEND.r);
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
        gloss = GBMT.r;
        metal = MAO.r;
    }
    else
    {
        metal = GMM.g;
        gloss = GMM.r;
    }

    a=bumpMap.r;
    vec2 tb = vec2(bumpMap.ga * 2.0 - 1.0);
    bump.xy    = tb.xy;
    bump.z     = clamp(sqrt(1.0 - ((tb.x*tb.x)+(tb.y*tb.y))),-1.0,1.0);
    bump       = normalize(bump);
    //============================================

    
    if (IS_ATLAS == 1)
        {        
            color = colorAM * MAO.g;
            spec_from_color_alpha = colorAM.a;
        }
        else
        {
            colorAM.rgb = basic_color.rgb*0.65;
            color = colorAM;
            //UV2 textures are not used on most models other than the blend mapping.
            //if (use_UV2 == 1) color.rgb = basic_color.rgb * basic_color2.rgb;
        }

    spec_from_color_alpha = colorAM.a*-1.0;//spec is in color alpha channel
    vec4 color_out = color;
    vec4 base = color;
    //============================================
    float perceptualRoughness = 0.3;
    float metallic = 0.25;
    // Roughness is stored in the 'g' channel, metallic is stored in the 'b' channel.
    // This layout intentionally reserves the 'r' channel for (optional) occlusion map data
 
    vec4 mrSample = vec4(1.0 ,gloss, metal ,1.0);

    perceptualRoughness = mrSample.g;
    metallic = max(mrSample.b,0.2) * metallic;


    perceptualRoughness = clamp(perceptualRoughness, c_MinRoughness, 1.0);
    metallic = clamp(metallic, 0.0, 1.0);
    // Roughness is authored as perceptual roughness; as is convention,
    // convert to material roughness by squaring the perceptual roughness [2].
    float alphaRoughness = perceptualRoughness * perceptualRoughness;
    // The albedo may be defined from a base texture or a flat color
    color = SRGBtoLINEAR(color);
    vec4 baseColor = color;

    vec3 f0 = vec3(0.04);
    vec3 diffuseColor = baseColor.rgb * (vec3(1.0) - f0);
    //diffuseColor *= 1.0 - metallic;
    vec3 specularColor = mix(f0, baseColor.rgb, metallic);
    // Compute reflectance.
    float reflectance = 0.1;//max(max(specularColor.r, specularColor.g), specularColor.b);
    if (gl_FrontFacing) reflectance =0.0;
    
    // For typical incident reflectance range (between 4% to 100%)
    // set the grazing reflectance to 100% for typical fresnel effect.
    // For very low reflectance range on highly diffuse objects (below 4%),
    // incrementally reduce grazing reflecance to 0%.
    float reflectance90 = clamp(reflectance * 25.0, 0.0, 1.0);
    vec3 specularEnvironmentR0 = specularColor.rgb;
    vec3 specularEnvironmentR90 = vec3(1.0, 1.0, 1.0) * reflectance90;

    vec3 PN = normalize(TBN * bump); // Get the perturbed normal
    vec3 sSpec;
    //============================================
    vec3 u_LightColor = vec3(0.75);
    //============================================
vec3 ndl;
    for (int i = 0; i < 3; i++){
        vec3 ll = gl_LightSource[i].position.xyz;

        vec3 n = PN;// normal at surface point

        vec3 v = normalize(camPosition-vVertex);// Vector from surface point to camera
        vec3 l = normalize(ll - vVertex);// Vector from surface point to light
        vec3 h = normalize(l+v);// Half vector between both l and v
        vec3 reflection = normalize(reflect(-v, n));
        vec3 R = normalize(reflect(-v,n));

        float NdotL = clamp(dot(n, l), 0.0, 1.0);
        float NdotV = abs(dot(n, v))+0.0;
        float NdotH = clamp(dot(n, h), 0.0, 1.0);
        float LdotH = clamp(dot(l, h), 0.0, 1.0);
        float VdotH = clamp(dot(v, h), 0.0, 1.0);
        ndl+= NdotL * NdotV;
        PBRInfo pbrInputs = PBRInfo(
            NdotL,
            NdotV,
            NdotH,
            LdotH,
            VdotH,
            perceptualRoughness,
            metallic,
            specularEnvironmentR0,
            specularEnvironmentR90,
            alphaRoughness,
            diffuseColor,
            specularColor
        );
    // Calculate the shading terms for the microfacet specular shading model
    vec3 F = specularReflection(pbrInputs)*1.0;
    float G = geometricOcclusion(pbrInputs);
    float D = microfacetDistribution(pbrInputs);
    // Calculation of analytical lighting contribution
    vec3 diffuseContrib = (1.0- F) * diffuse(pbrInputs);
    vec3 specContrib = F * G * D * (4.0 * NdotL * NdotV)* S_level;
    vec3 sSpec = vec3(1.0) * pow(max(dot(reflection,l),0.0),1.0) * S_level;
    // Obtain final intensity as reflectance (BRDF) scaled by the energy of the light (cosine law)
    vec3 colorMix =  (NdotL * u_LightColor *  (diffuseContrib*15.0)
                     + ((sSpec + specContrib *spec_from_color_alpha) * S_level * mrSample.g)*1.0);

    blmColor.rgb +=  NdotL *(specContrib*S_level*mrSample.g)*colorMix.rgb*base.rgb*2.0*gloss;

    colorMix += NdotV * getIBLContribution(pbrInputs, n, R) * perceptualRoughness*5.0;
    //ambient
    vec3 ambient = diffuseContrib.rgb * A_level;

    colorMix += (ambient + (ambient))*2.0;

    // This section uses mix to override final color for reference app visualization
    // of various parameters in the lighting equation. Great for Debuging!
    colorMix = mix(colorMix, F, u_ScaleFGDSpec.x);
    colorMix = mix(colorMix, vec3(G), u_ScaleFGDSpec.y);
    colorMix = mix(colorMix, vec3(D), u_ScaleFGDSpec.z);
    colorMix = mix(colorMix, specContrib, u_ScaleFGDSpec.w);

    colorMix = mix(colorMix, diffuseContrib, u_ScaleDiffBaseMR.x);
    colorMix = mix(colorMix, baseColor.rgb, u_ScaleDiffBaseMR.y);
    colorMix = mix(colorMix, vec3(metallic), u_ScaleDiffBaseMR.z);
    colorMix = mix(colorMix, vec3(perceptualRoughness), u_ScaleDiffBaseMR.w);

    sum.rgb += colorMix.rgb;
    
}// i loop
    gColor = correct(sum*T_level,2.5,0.9)*1.0;  
    vec3 acolor = min(max(gColor.rgb - vec3(mininput), vec3(0.0)) / (vec3(maxinput) - vec3(mininput)), vec3(1.0));
    gColor.rgb = mix(vec3(minoutput), vec3(maxoutput), acolor);
    //gColor.a = sum.a;
    //gColor.rgb = (vec3(ndl) * vec3(0.999 * T_level)) + ( gColor.rgb * vec3(1.0)*(1.0- T_level)) ;
}//main