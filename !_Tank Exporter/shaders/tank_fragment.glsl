// tank_fragment.glsl
// Used to light all tank models
// This shader is mostly based on the code from https://github.com/KhronosGroup/glTF-WebGL-PBR
// Many things had to be changed to get it working.
// The most important change is the texture colors for roughness and metal have to be flipped and biased.
// Im leaving all the comments from the original code in case it helps understand this.
#version 330 compatibility

layout (location = 0) out vec4 gColor;
layout (location = 1) out vec4 blmColor;
uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform sampler2D gmmMap;
uniform sampler2D aoMap;
uniform sampler2D detailMap;
uniform sampler2D camoMap;
uniform sampler2D shadowMap;
uniform samplerCube cubeMap;
uniform sampler2D u_brdfLUT;
uniform int is_GAmap;
uniform int alphaRef;
uniform vec2 detailTiling;
uniform vec2 GMM_Toy;
uniform int use_GMM_Toy;
uniform vec4 tile_vec4;
uniform float detailPower;
uniform int use_camo;
uniform int is_track;
        // used for debuging
uniform vec4 u_ScaleFGDSpec;
uniform vec4 u_ScaleDiffBaseMR;
uniform int use_shadow;
uniform int enableVertexColor;
// if camo is active  1 = yes 0 = no
uniform int use_CM;
// if using the CM mask 1 = yes 0 = no
uniform int exclude_camo;
// if this item has camo  1 = yes 0 = no
uniform vec4 c0;
uniform vec4 c1;
uniform vec4 c2;
uniform vec4 c3;
uniform vec4 armorcolor;
uniform vec4 camo_tiling;
in vec3 p_Camera;
uniform float A_level;
uniform float S_level;
uniform float T_level;
in vec2 TC1;
in vec3 v_Position;
in mat3 TBN;
in vec3 t;
in vec3 b;
in vec3 v_Normal;
in vec3 cubeTC;
in vec3 vertexColor;


// ========================================================
// rextimmy gets full credit for figuring out how mixing works!
vec4 applyCamo(vec4 cc,vec4 camoTex){
    vec4 ac = armorcolor;
    cc   = ac ;
    cc   = mix(cc, c0 , camoTex.r * c0.a );
    cc   = mix(cc, c1 , camoTex.g * c1.a );
    cc   = mix(cc, c2 , camoTex.b * c2.a );
    cc   = mix(cc, c3 , camoTex.a * c3.a );
    return cc;
}

// PBR Functions ======================================================
// Encapsulate the various inputs used by the various functions in the shading equation
// We store values in this struct to simplify the integration of alternative implementations
// of the shading terms, outlined in the Readme.MD Appendix.
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
const float c_MinRoughness = 0.01;
// used for debug.. shows different parts of the lighting

#define MANUAL_SRGB;
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

// Find the normal for this fragment, pulling either from a predefined normal map
// or from the interpolated mesh normal and tangent attributes.
vec3 getNormal()
{
    // Retrieve the tangent space matrix
#ifndef HAS_TANGENTS
    vec3 pos_dx = dFdx(v_Position);
    vec3 pos_dy = dFdy(v_Position);
    vec3 tex_dx = dFdx(vec3(TC1, 0.0));
    vec3 tex_dy = dFdy(vec3(TC1, 0.0));
    vec3 t = (tex_dy.t * pos_dx - tex_dx.t * pos_dy) / (tex_dx.s * tex_dy.t - tex_dy.s * tex_dx.t);

    vec3 ng = normalize(v_Normal);

    t = normalize(t - ng * dot(ng, t));
    vec3 b = normalize(cross(ng, t));
    mat3 tbn = mat3(t, b, ng);

#else // HAS_TANGENTS
    mat3 tbn = v_TBN;
#endif

    vec3 n;
    vec4 tn = texture2D(normalMap, TC1).rgba;
    if (is_GAmap == 1 && use_CM == 0)
    {
        tn.xy = tn.ga*2.0-1.0;
        tn.z =  sqrt(1.0 - clamp( ((tn.x*tn.x) + (tn.y*tn.y)) ,-1.0,1.0) );
        tn.x *= -1.0;
        n = tn.rgb;
    }

    else
    {
        n = texture2D(normalMap, TC1).rgb*2.0-1.0;
    }

    n = normalize(tbn * n);


    return n;
}

// Calculation of the lighting contribution from an optional Image Based Light source.
// Precomputed Environment Maps are required uniform inputs and are computed as outlined in [1].
// See our README.md on Environment Maps [3] for additional discussion.
vec3 getIBLContribution(PBRInfo pbrInputs, vec3 n, vec3 reflection)
{
    float mipCount = 9.0; // resolution of 512x512
    float lod = ((1.0-pbrInputs.perceptualRoughness) * mipCount);
    // retrieve a scale and bias to F0. See [1], Figure 3
    vec3 brdf = SRGBtoLINEAR(texture2D(u_brdfLUT, vec2(pbrInputs.NdotV*0.1, (1.0 - pbrInputs.perceptualRoughness)*0.1))).rgb;
    vec3 diffuseLight = SRGBtoLINEAR(textureCubeLod(cubeMap, n, 7)).rgb;
     reflection.xyz *= -1.0;// like so many other things, DirectX to OpenGL causes axis issues.
    vec3 specularLight = SRGBtoLINEAR(textureCubeLod(cubeMap, reflection, lod)).rgb;

    vec3 diffuse = diffuseLight * pbrInputs.diffuseColor;
    vec3 specular = specularLight * (pbrInputs.specularColor * brdf.x + brdf.y);
    // For presentation, this allows us to disable IBL terms
    diffuse *= S_level;
    specular *= S_level;
    return diffuse + specular;
}

// Basic Lambertian diffuse
// Implementation from Lambert's Photometria https://archive.org/details/lambertsphotome00lambgoog
// See also [1], Equation 1
vec3 diffuse(PBRInfo pbrInputs)
{
    return pbrInputs.diffuseColor / M_PI;
}

// The following equation models the Fresnel reflectance term of the spec equation (aka F())
// Implementation of fresnel from [4], Equation 15
vec3 specularReflection(PBRInfo pbrInputs)
{
    return pbrInputs.reflectance0 + (pbrInputs.reflectance90 - pbrInputs.reflectance0) * pow(clamp(1.0 - pbrInputs.VdotH, 0.0, 1.0), 2.0);
}

// This calculates the specular geometric attenuation (aka G()),
// where rougher material will reflect less light back to the viewer.
// This implementation is based on [1] Equation 4, and we adopt their modifications to
// alphaRoughness as input as originally proposed in [2].
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


// The following equation(s) model the distribution of microfacet normals across the area being drawn (aka D())
// Implementation from "Average Irregularity Representation of a Roughened Surface for Ray Reflection" by T. S. Trowbridge, and K. P. Reitz
// Follows the distribution function recommended in the SIGGRAPH 2013 course notes from EPIC Games [1], Equation 3.
float microfacetDistribution(PBRInfo pbrInputs)
{
    float roughnessSq = 0.05;
    //pbrInputs.alphaRoughness * pbrInputs.alphaRoughness;
    float f = (pbrInputs.NdotH * roughnessSq - pbrInputs.NdotH) * pbrInputs.NdotH + 1.0;
    return roughnessSq / (M_PI * f * f);
}

// end PBR functions ===============================================================


void main(void) {
    vec3 lightColors[3];
    // orbiting light
    lightColors[0] = vec3(1.0);
    // Sun direction light
    lightColors[1] = vec3(1.0, 1.0, 1.0);
    //top light
    lightColors[2] = vec3(1.0);

//--------------------------------
vec4   cc = vec4(0.0);
    vec3   lightDirection;
    vec4   detailBump;
    vec3   bump;
    float  alpha;
    vec4   color;
    float  a;
    vec3   sumC = vec3(0.0);
    //--------------------------------


// setup tiling values
    vec2 ctc = TC1 * camo_tiling.xy;
    // from scripts/vehicle/nation/customiztion.xml file 
    ctc.xy *= tile_vec4.xy;
    // from scripts/vehicle/nation/tank.xml file
    ctc.xy += camo_tiling.zw;
    ctc.xy += tile_vec4.zw;
    //--------------------------------

// Load textures
    vec2 flipTC = TC1;
    flipTC.y *= -1.0;;
    vec4 camoTexture = texture2D(camoMap,   ctc.st );
    vec4 detail      = texture2D(detailMap, TC1.st * detailTiling);
    vec4 AO          = texture2D(aoMap,     TC1.st);
    vec4 base        = texture2D(colorMap,  TC1.st);
    vec4 bumpMap     = texture2D(normalMap, TC1.st);
    vec3 GMM         = texture2D(gmmMap,    TC1.st).rgb;
    if (use_GMM_Toy ==1){ 
        GMM.rg = GMM_Toy.xy;
        }
    //--------------------------------
    color.rgb  = base.rgb;
    a = base.a;
    //==================================

    if (is_GAmap == 1 && use_CM == 0)
        {
        if (exclude_camo == 0)
        {
            // This is everyting but the chassis
            if (use_camo > 0 )
            {
                cc    = applyCamo(cc, camoTexture);
                color.rgb = mix(color.rgb, cc.rgb,  AO.a*1.5); // big boost to camo mix
            }
            color.rgb *= AO.g;
            base.rgb  = color.rgb;
        }else{
            if (is_track == 1 ){
            // If we are here, we are on the track treads
            // AO is stored in the tracks color channel's alpha!
            AO = vec4(base.a);
            color.rgb *= AO.g;
            base = color;
            }else{
            // If we land here, we are on chassis
            color.rgb *= AO.g; // This makes the tracks look like crap!
            base.rgb  = color.rgb;
            }// end AO.g
        }

    }else{
        // If we land here, a SD tank was loaded.
        // If there is no GMM map, there is no detail map
        // Some values have to be hard coded to handle them
        // being misssing.
        bump   = normalize(bumpMap.rgb*2.0 - 1.0);
        bump.y *= -1.0;
        if (use_camo > 0 && exclude_camo == 0)
        {
            cc    = applyCamo(cc, camoTexture);
            color = mix(color, cc, AO.b * cc.a);
        }            
        
        base.rgb  = color.rgb;
        GMM.g     = 0.4;
        GMM.r     = 0.5;
    } //end is_GAmap
    
    float aRef = float(alphaRef)/255.0;
    if (aRef > bumpMap.r) {
        discard;
    }
    color = SRGBtoLINEAR(color);
   //=============================================================================================
   // 3rd try at PBR lighting

    // Metallic and Roughness material properties are packed together
    // In glTF, these factors can be specified by fixed scalar values
    // or from a metallic-roughness map
    float perceptualRoughness = 0.2;
    float metallic = 1.5;
    // Roughness is stored in the 'g' channel, metallic is stored in the 'b' channel.
    // This layout intentionally reserves the 'r' channel for (optional) occlusion map data
    vec4 mrSample = vec4(1.0 , GMM.r, GMM.g ,1.0);
    // setup correct loctaions
    mrSample = vec4(AO.r , pow(GMM.r/0.8,7.0), pow(GMM.g/0.5,5.0) ,1.0);
    // setup correct loctaions
    perceptualRoughness = mrSample.g;
    metallic = mrSample.b * metallic;
    perceptualRoughness = clamp(perceptualRoughness, c_MinRoughness, 1.0);
    metallic = clamp(metallic, 0.0, 1.0);
    // Roughness is authored as perceptual roughness; as is convention,
    // convert to material roughness by squaring the perceptual roughness [2].
    float alphaRoughness = perceptualRoughness * perceptualRoughness;
    // The albedo may be defined from a base texture or a flat color
    vec4 baseColor = color;

    vec3 f0 = vec3(0.04);
    vec3 diffuseColor = baseColor.rgb * (vec3(1.0) - f0);
    diffuseColor *= 1.0 - metallic;
    vec3 specularColor = mix(f0, baseColor.rgb, metallic);
    // Compute reflectance.
    float reflectance = max(max(specularColor.r, specularColor.g), specularColor.b);
    // For typical incident reflectance range (between 4% to 100%) set the grazing reflectance to 100% for typical fresnel effect.
    // For very low reflectance range on highly diffuse objects (below 4%), incrementally reduce grazing reflecance to 0%.
    float reflectance90 = clamp(reflectance * 25.0, 0.0, 1.0);
    vec3 specularEnvironmentR0 = specularColor.rgb;
    vec3 specularEnvironmentR90 = vec3(1.0, 1.0, 1.0) * reflectance90;
    float shadow =1.0;
    if (use_shadow == 1) shadow = texelFetch(shadowMap, ivec2(gl_FragCoord.xy), 0).r;
    base.rgb *= shadow;
    diffuseColor.rgb *= shadow;

    vec3 sum = vec3(0.0);
    float scrach = pow((detail.r * detail.g )/0.4,2.0)* metallic;

for (int i = 0; i < 3; i++){
        vec3 u_LightDirection = gl_LightSource[i].position.xyz;
        vec3 n = getNormal();// normal at surface point
        vec3 v = normalize(p_Camera - v_Position);// Vector from surface point to camera
        vec3 l = normalize(u_LightDirection - v_Position);// Vector from surface point to light
        vec3 h = normalize(l+v);// Half vector between both l and v
        vec3 reflection = normalize(reflect(-v, n));
        vec3 R = normalize(reflect(v,-v_Normal));
        float NdotL = clamp(dot(n, l), 0.001, 1.0);
        float NdotV = abs(dot(n, v)) + 0.001;
        float NdotH = clamp(dot(n, h), 0.0, 1.0);
        float LdotH = clamp(dot(l, h), 0.0, 1.0);
        float VdotH = clamp(dot(v, h), 0.0, 1.0);

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
    vec3 F = specularReflection(pbrInputs);
        float G = geometricOcclusion(pbrInputs);
        float D = microfacetDistribution(pbrInputs)* GMM.r;
        vec3 u_LightColor = lightColors[i];
        // Calculation of analytical lighting contribution
    vec3 diffuseContrib = (1.0 - F) * diffuse(pbrInputs);
    vec3 specContrib = F * G * D / (4.0 * NdotL * NdotV);
    vec3 sSpec = vec3(1.0) * pow(max(dot(reflection,l),0.0),10.0) * S_level * scrach * shadow;
        // Obtain final intensity as reflectance (BRDF) scaled by the energy of the light (cosine law)
    vec3 colorMix =  NdotL * u_LightColor *  ((sSpec + diffuseContrib* shadow) + (specContrib*S_level*mrSample.g*6.0))*5.0;

    blmColor.rgb +=  NdotL *(specContrib*S_level*mrSample.g)*colorMix.rgb*base.rgb*6.0*GMM.r;
    colorMix += NdotV * getIBLContribution(pbrInputs, n, R)
                *perceptualRoughness * float(is_GAmap);
    vec3 ambient = diffuseContrib.rgb * A_level*0.25;
    colorMix += (ambient + (ambient*NdotL));

    // This section uses mix to override final color for reference app visualization
    // of various parameters in the lighting equation. Great for Debuging!
    colorMix = mix(colorMix, F, u_ScaleFGDSpec.x);
    colorMix = mix(colorMix, vec3(G)*0.5, u_ScaleFGDSpec.y);
    colorMix = mix(colorMix, vec3(D), u_ScaleFGDSpec.z);
    colorMix = mix(colorMix, specContrib, u_ScaleFGDSpec.w);

    colorMix = mix(colorMix, diffuseContrib, u_ScaleDiffBaseMR.x);
    colorMix = mix(colorMix, baseColor.rgb, u_ScaleDiffBaseMR.y);
    colorMix = mix(colorMix, vec3(metallic), u_ScaleDiffBaseMR.z);
    colorMix = mix(colorMix, vec3(perceptualRoughness), u_ScaleDiffBaseMR.w);

    gColor += vec4(pow(colorMix,vec3(1.0/2.2)), 1.0) * T_level;


    if (enableVertexColor == 1)
        {
        gColor.rgb = (gColor.rgb *0.0) + clamp((vertexColor*2.0),0.0,1.0);
        }
    
}// i loop
    blmColor.a = 1.0;
}

