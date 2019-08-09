//atlasPBR_fragment.glsl
//Used to light primitive models
#version 130
uniform sampler2D colorMap; // not on all models
uniform sampler2D normalMap;
uniform sampler2D GMM_Map;
uniform sampler2D detailMap;
uniform samplerCube cubeMap;

uniform vec4 g_detailRejectTiling;
uniform vec4 g_detailInfluences; // Size of Atlas cells X,Y

uniform float A_level; //Ambient level
uniform float S_level; //Specular level
uniform float T_level; //Brightness level


uniform int alpha_enable;
uniform int alpha_value;
uniform int is_glass;
uniform int has_detail_map;

in vec2 TC1;// UV coords
in vec2 TC2;// UV2 coords

in vec3 vVertex;
in vec3 vNormal;
in vec4 vColor;
float b_size = 512.0;
in mat3 TBN;

out vec4 color_out;

vec4 correct(in vec4 hdrColor, in float exposure, in float gamma_level){  
    // Exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    // Gamma correction 
    mapped.rgb = pow(mapped.rgb, vec3(1.0 / gamma_level));  
    return vec4 (mapped, 1.0);
}

void main(void) {
    float specPower = 10.0;
    float specular = 0.5;
    float a;
    vec4 Ispec1 = vec4(0.0);
    vec4 Ispec1HP = vec4(0.0);
    vec4 Idiff1 = vec4(0.0);
    vec4 sum = vec4(0.0);
    vec4 spec_sum = vec4(0.0);
    vec3 bump;
    //============================================
    vec4 color = texture2D(colorMap,TC1);
    vec4 bumpMap = texture2D(normalMap,TC1);
    vec4 GMM = texture2D(GMM_Map,TC1);
    vec4 detail = texture2D(detailMap,TC1);
    vec4 detail_mix;
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
    bump.z     = sqrt(1.0 - dot(tb.xy, bumpMap.xy));
    bump       = normalize(bump);
    //bump.y *= - 1.0;

    //=================================================================
    //metalness factor
    color.rgb = mix(color.rgb,vec3(0.04),1.0-GMM.g);
    
    //=================================================================

    vec3 E = normalize(-vVertex);    // we are in Eye Coordinates, so EyePos is (0,0,0)  
    vec3 PN = normalize(TBN * bump); // Get the perturbed normal

    vec4 Iamb = color * A_level*0.5 ; //calculate Ambient Term:  
    // loop thru lights and calc lighting.
    vec3 norm = normalize(vNormal);
  
    for (int i = 0 ; i < 1 ; i++)
        {
        vec3 Lpos = gl_LightSource[i].position.xyz;
        vec3 L = normalize(Lpos - vVertex);   
        vec3 R = normalize(reflect(-L,PN));  
        //calculate Diffuse Term:  
        Idiff1 = color * pow(clamp(dot(PN,L), 0.0, 1.0),1.0)*0.25;//color light level

        // calculate Specular Term:
        Ispec1 = vec4(0.2) * clamp(pow(dot(R,E),specPower/GMM.r),0.0,1.0) * S_level;
        Ispec1HP = vec4(0.6,0.6,0.6,1.0) * vec4(clamp(pow(dot(R,E),200.0),0.0,1.0) * S_level*(1.0-GMM.g));
        spec_sum += Ispec1HP;
        sum += clamp(Idiff1 +  Ispec1, 0.0, 1.0);

       } //next light
       color_out = correct((Iamb + sum)*1.5 * T_level,6.5,1.0);
        
    if (is_glass == 1)
    {

        vec3 R = reflect(E, norm);
        vec4 cube = vec4(textureCubeLod(cubeMap, -R,1).rgb, 0.0);
        color_out += vec4(0.0,0.05,0.0,0.0);
        color_out.rgb += cube.rgb* GMM.g;
        color_out += spec_sum * vec4(6.0);
        color_out.a = color.a*(1.0-GMM.g);
    } 
    //debug
    //color_out.rgb += DIRT.rgb * T_level;//(Iamb + sum) * T_level;   // write mixed Color:  
    //color_out.rgb = (color_out.rgb * 0.0005) + colorAM_1.rgb  * T_level;   // write mixed Color:  

    //=================================================================

} //main