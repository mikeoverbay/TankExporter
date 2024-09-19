//decalCpass_fragment.glsl

#version 330 compatibility
#extension GL_EXT_gpu_shader4 : enable

layout (location = 0) out vec4 gColor;

uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform sampler2D surfaceNormalMap;
uniform sampler2D gmmMap;

uniform sampler2D shadowMap;// produced in depth_fragment

uniform sampler2D depthMap; // copied from depth buffer


uniform sampler2D gNormalMap; // produced in TerrainShader_Fragment
uniform sampler2D fogMap; // produced in TerrainShader_Fragment

uniform samplerCube cubeMap; // loaded resource
uniform sampler2D u_brdfLUT; // loaded resource

uniform int use_shadow; // switch
uniform float alpha_value; // slider value
uniform float color_level; // slider value
uniform vec2 uv_wrap; // wrap
uniform vec3 viewPos; // camera position
uniform vec3 LightPos[3]; // the 3 light positions
uniform mat4 shadowProjection; // light position shadow matrix

uniform vec4 u_ScaleFGDSpec; // display switches
uniform vec4 u_ScaleDiffBaseMR; // display switches

const vec3 tr = vec3 (0.5 ,0.5 , 0.5);
const vec3 bl = vec3(-0.5, -0.5, -0.5);
in mat4 invDecal_mat; // inverse decal matrix
in mat4 matPrjInv; // inverse projection matrix
in mat2 uv_matrix; // texture rotation matrix. Created in vertex section
//in vec4 ShadowCoord;
in vec3 v_Position; // vertex in model view space
in vec4 positionSS; // vertex in screen space 

void clip(vec3 v){
    if (v.x > tr.x || v.x < bl.x ) {
        discard;
    }
    if (v.y > tr.y || v.y < bl.y ) {
        discard;
    }
    if (v.z > tr.z || v.z < bl.z ) {
        discard;
    }
}

vec3 correction(vec3 color_in){
    const float exposure =2.2;
    vec3 mapped = vec3(1.0) - exp(-color_in * exposure);
    return mapped;
}

vec2 postProjToScreen(vec4 position)
{
    vec2 screenPos = position.xy / position.w;
    return 0.5 * (vec2(screenPos.x, screenPos.y) + 1);
}

float alphaGMM;

vec3 getNormal(in vec2 iUV,in vec2 UVn,in vec3 pos,  in float alpha)
{
    // compute derivations of the world position
    vec3 p_dx = dFdx(pos);
    vec3 p_dy = dFdy(pos);
    // compute derivations of the texture coordinate
    vec2 tc_dx = dFdx(iUV);
    vec2 tc_dy = dFdy(iUV);
    // compute initial tangent and bi-tangent
     // get new tangent from a given mesh normal
    vec3 t = normalize( tc_dy.y * p_dx - tc_dx.y * p_dy );
    vec3 b = normalize( tc_dy.x * p_dx - tc_dx.x * p_dy );
    vec3 ng = normalize((texture2D(gNormalMap, UVn).rgb));
    vec3 sng = normalize((texture2D(gNormalMap, UVn).rgb));

    ng = mix(normalize(sng), ng, alpha);
    t = t - ng * dot( t, ng ); // orthonormalization of the tangent vectors
    b = b - ng * dot( b, ng ); // orthonormalization of the binormal vectors to the normal vector 
    b = b - t * dot( b, t ); // orthonormalization of the binormal vectors to the tangent vector
    mat3 tbn = mat3(t, b, ng);
    vec2 tn = texture2D(normalMap, iUV*uv_wrap).ag*2.0-1.0 ;
    vec3 co;
    co.xy = tn.yx;
    co.z = sqrt(1.0 - clamp( ((tn.x*tn.x) + (tn.y*tn.y)) ,-1.0,1.0) );
    co = normalize(co);

    co = tbn * co;
    co.z *= -1.0;

    co = normalize(co);
    return co;
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
const float c_MinRoughness = 0.3;
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
    vec3 specular = specularLight * (pbrInputs.specularColor * brdf.x + brdf.y);
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
    float roughnessSq = 0.4;
    //pbrInputs.alphaRoughness * pbrInputs.alphaRoughness;
    float f = (pbrInputs.NdotH * roughnessSq - pbrInputs.NdotH) * pbrInputs.NdotH + 1.0;
    return roughnessSq / (M_PI * f * f);
}
// end PBR functions ===============================================================

void main(){
    float shadow = 1.0;
    float falloff = 0.0;
vec3 lightColor[3];
    lightColor[0] = vec3 (0.6,0.0,0.0);
    lightColor[1] = vec3 (0.0,0.6,0.0);
    lightColor[2] = vec3 (0.0,0.0,0.8);
    lightColor[0] = vec3 (0.5);
    lightColor[1] = vec3 (0.5);
    lightColor[2] = vec3 (0.5);

 //=========================================================
// Calculate UVs
    vec2 UV = postProjToScreen(positionSS);

    // Sample the Depth from the Depthsampler
    float Depth = texture2D(depthMap, UV).x * 2.0 - 1.0;

    // Calculate World Position by recreating it out of the coordinates and depth-sample
    vec4 ScreenPosition;
    ScreenPosition.xy = UV * 2.0 - 1.0;
    ScreenPosition.z = Depth;
    ScreenPosition.w = 1.0;

    // Transform position from screen space to world space
    vec4 WorldPosition = matPrjInv * ScreenPosition;
    WorldPosition.xyz /= WorldPosition.w;
    WorldPosition.w = 1.0;

    // Transform to decal original and size
    vec3 world = vec3(WorldPosition); // This is world space!

    float l_dist = sqrt(world.x * world.x + world.z * world.z); // Used to clip the shadow map

    WorldPosition = invDecal_mat * WorldPosition;
    clip(WorldPosition.xyz);

    /*==================================================*/
    //Get texture UVs
    //get 1/2 pixel
    WorldPosition.xy += 0.5;
    WorldPosition.xy *= -1.0;
    vec2 UV_ = (WorldPosition.xy );
   
    UV_ = uv_matrix * clamp(UV_, -1.0 , 0.0);
    //=========================================================
    vec4 color = texture2D(colorMap,  UV_.st*uv_wrap);
    color.rgb *= color_level;
    vec3 GMM   = texture2D(gmmMap,    UV_.st*uv_wrap).rgb;
    alphaGMM = texture2D(gmmMap,    UV_.st*uv_wrap).a;
    vec2 AO = vec2(0.75);

    //=========================================================
    float perceptualRoughness = 0.2;
    float metallic = 1.0;
    // Roughness is stored in the 'g' channel, metallic is stored in the 'b' channel.
    // This layout intentionally reserves the 'r' channel for (optional) occlusion map data
    //vec4 mrSample = vec4(1.0 , 1.0-GMM.r, GMM.g ,1.0);
    vec4 mrSample = vec4(1.0 , GMM.g* alphaGMM, 1.0-GMM.r ,1.0);
    // setup correct loctaions
    //mrSample = vec4(AO.r , pow(GMM.r/1.0,2.0), pow(GMM.g/1.0,1.0) ,1.0);
    // setup correct loctaions
    perceptualRoughness = mrSample.g;
    metallic = max(mrSample.b,0.1) * metallic;


    perceptualRoughness = clamp(perceptualRoughness, c_MinRoughness, 1.0);
    metallic = clamp(metallic, 0.0, 1.0);
    // Roughness is authored as perceptual roughness; as is convention,
    // convert to material roughness by squaring the perceptual roughness [2].
    float alphaRoughness = perceptualRoughness * perceptualRoughness;
    // The albedo may be defined from a base texture or a flat color
    if (use_shadow == 1 && l_dist < 12.0) shadow = texelFetch(shadowMap, ivec2(gl_FragCoord.xy), 0).r;
    vec4 baseColor = color*shadow;

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


    vec4 sum = vec4(0.0);
    vec3 sSpec;
	// color.a controls mix of terrain face normal and decals texture normal;
    vec3 n = getNormal(UV_, UV, world, color.a);
    vec3 v = normalize(viewPos - world);// Vector from surface point to camera

for (int i = 0; i < 2; i++){
        vec3 ll = LightPos[0].xyz + vec3(0.0,i*25.0,0.0)*5.0;

        float len = length(world - ll);
        float d = 120;
        if (len < d) { falloff = 1.0-(len/d); }

        vec3 l = normalize(ll - world);// Vector from surface point to light
        vec3 h = normalize(l+v);// Half vector between both l and v
        vec3 reflection = normalize(reflect(-v, n));
        vec3 R = normalize(reflect(-v,n));

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
        vec3 u_LightColor = lightColor[i];
        // Calculation of analytical lighting contribution
    vec3 diffuseContrib = (1.0 - F) * diffuse(pbrInputs);
    vec3 specContrib = F * G * D / (4.0 * NdotL * NdotV);

    sSpec = u_LightColor * pow(max(dot(R,l),0.0),20.0) * 0.000;

        // Obtain final intensity as reflectance (BRDF) scaled by the energy of the light (cosine law)
    vec3 colorMix =  NdotL * u_LightColor * ((sSpec + diffuseContrib) + (specContrib * mrSample.g*1.0))*5.0*falloff;
    colorMix += NdotV * getIBLContribution(pbrInputs, n, R) * perceptualRoughness *falloff;
    vec3 ambient = diffuseContrib.rgb *0.25;
    colorMix += (ambient + (ambient*NdotL));
       // Calculate lighting contribution from image based lighting source (IBL)
    #define USE_IBL;
    #ifdef USE_IBL
    #endif

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

    sum.rgb += colorMix.rgb *1.0;
    //sum.rgb += vec3(sSpec) * lightColor[i] + colorMix.rgb *0.0001;
    //sum.rgb += (gColor.rgb*vec3(0.00001)) + vec3(sSpec) * 1.0;
    //sum.rgb += (gColor.rgb*vec3(0.00001)) + n + nn  * 0.1;
    //sum.rgb = (gColor.rgb*vec3(0.00001)) + normalize(nn)  ;
    //sum.rgb += (gColor.rgb*vec3(0.00001)) + normalize(n)  ;
    
}// i loop

    //=========================================================
    vec4 groundFog = texture2D(fogMap,vec2(UV.x,UV.y));
    float z = length(world);
    float height = sin(1.0-( (world.y+3.0)/50.0)*(3.1415/2.0));
    const float LOG2 = 1.442695;
    vec4 fog_color = vec4(vec3(0.35),1.0);

    //the moving ground fog
    float density = (gl_Fog.density * height) * 0.03;
    float fogFactor = exp2(-density * density * z * z * LOG2);
    fogFactor = clamp(fogFactor, 0.0, 1.0);
    gColor.rgb = sum.rgb+groundFog.xyz;
    gColor.rgb = mix(fog_color.rgb, gColor.rgb, fogFactor );
    
    gColor.a = color.a * alpha_value;
    //gColor.rgb = (gColor.rgb * vec3(0.0000001)) + vec3(vec2(1.0-UV_.x,1.0-UV_.y),0.0);
    
    }

