#version 430 compatibility

layout(location = 0) out vec4 gColor;
layout(location = 1) out vec4 blmColor;

uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform sampler2D GMM_Map;
uniform sampler2D detailMap;
uniform samplerCube cubeMap;
uniform sampler2D u_brdfLUT;

uniform vec4 g_detailRejectTiling;
uniform vec4 g_detailInfluences;

uniform float A_level;
uniform float S_level;
uniform float T_level;

uniform vec3 camPosition;

uniform int alpha_enable;
uniform int alpha_value;
uniform int is_glass;
uniform int has_detail_map;

uniform vec4 u_ScaleFGDSpec;
uniform vec4 u_ScaleDiffBaseMR;

in vec2 TC1;
in vec2 TC2;
in mat3 TBN;
in vec3 vVertex;
in vec3 Normal;

out vec4 color_out;

const float PI = 3.14159265359;
const float MAX_REFLECTION_LOD = 9.0;

vec3 F; // Global Fresnel term
vec3 kD; // Global diffuse term

vec4 correct(in vec4 hdrColor, in float exposure, in float gamma_level) {
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    mapped.rgb = pow(mapped.rgb, vec3(1.0 / gamma_level));
    return vec4(mapped, hdrColor.a);
}

vec3 fresnelSchlick(float cosTheta, vec3 F0) {
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

vec3 fresnelSchlickRoughness(float cosTheta, vec3 F0, float roughness) {
    return F0 + (max(vec3(1.0 - roughness), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}

float DistributionGGX(vec3 N, vec3 H, float roughness) {
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float num = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness) {
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;
    return num / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness) {
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

void main(void) {
    vec4 color = texture(colorMap, TC1);
    vec4 bumpMap = texture(normalMap, TC1);
    vec4 GMM = texture(GMM_Map, TC1);
    vec4 detail = texture(detailMap, TC1);

    float roughness = 1.1 - GMM.r;
    if (GMM.b > 0.03) roughness = 1.0 - GMM.b;
    float metallic = GMM.g;
    if (is_glass == 1) roughness = 0.25 * color.r;

    if (has_detail_map > 0) {
        vec3 detail_mix = mix(color.rgb, vec3(detail.r), g_detailInfluences.x);
        detail_mix = mix(detail_mix, vec3(detail.g), g_detailInfluences.y);
        detail_mix = mix(detail_mix, vec3(detail.b), g_detailInfluences.z);
    }

    if (alpha_enable == 1) {
        float aRef = float(alpha_value) / 255.0;
        if (aRef > bumpMap.r) discard;
    }

    vec2 tb = vec2(bumpMap.ga * 2.0 - 1.0);
    vec3 bump = normalize(vec3(tb, sqrt(1.0 - dot(tb, tb))));

    vec3 albedo = pow(color.rgb, vec3(2.2));
    vec3 N = normalize(TBN * bump);
    vec3 V = normalize(-vVertex);

    vec3 F0 = mix(vec3(0.04), albedo, metallic);
    vec3 LightColor = vec3(2.0 * T_level);
    vec3 Lo = vec3(0.0);

    for (int i = 0; i < 3; ++i) {
        vec3 L = normalize(gl_LightSource[i].position.xyz - vVertex);
        vec3 H = normalize(V + L);

        float distance = length(gl_LightSource[i].position.xyz - vVertex);
        float attenuation = 1500.0 / (distance * distance);
        vec3 radiance = LightColor * attenuation;

        float NDF = DistributionGGX(N, H, roughness);
        float G = GeometrySmith(N, V, L, roughness);
        F = fresnelSchlick(max(dot(H, V), 0.0), F0);

        vec3 kS = F;
        kD = vec3(1.0) - kS;
        kD *= 1.0 - metallic;

        vec3 numerator = NDF * G * F;
        float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
        float NdotL = max(dot(N, L), 0.0);

        vec3 diffuse = NdotL * albedo + albedo * 0.05;
        vec3 specular = numerator / max(denominator, 0.001) * S_level;

        blmColor = vec4(specular, 1.0);
        Lo += ((kD * albedo / PI) + specular) * radiance * NdotL;
    }

    vec3 R = reflect(-V, N);
    R.xz *= -1.0;
    vec3 prefilteredColor = pow(textureCubeLod(cubeMap, R, roughness * MAX_REFLECTION_LOD).rgb, vec3(2.2));
    vec2 brdf = texture(u_brdfLUT, vec2(max(dot(N, V), 0.0), roughness)).rg;
    vec3 specular = prefilteredColor * (F * brdf.x + brdf.y) * (1.0 - roughness);

    vec3 ambient = (kD * albedo + specular) * A_level;
    vec3 col = ambient + Lo;

    col = col / (col + vec3(1.0));
    col = pow(col, vec3(1.0 / 2.2)) * 1.1;

    gColor = vec4(col, 0.0);
    gColor.rgb = mix(vec3(0.0), vec3(0.9), min(max(gColor.rgb, vec3(0.0)), vec3(1.0)));

    if (is_glass == 1) {
        gColor += vec4(0.0, 0.01, 0.0, 0.0);
        gColor.a = 1.0 - GMM.g * 0.825;
    }

    gColor = correct(gColor, 1.8, 0.9) * 1.25;
}
