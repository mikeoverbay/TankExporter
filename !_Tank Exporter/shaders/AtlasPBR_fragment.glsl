//fbx_fragment.glsl
//Used to light all FBX imports
#version 130
uniform sampler2D ATLAS_AM_Map; // not on all models
uniform sampler2D ATLAS_GBMT_Map;
uniform sampler2D ATLAS_MAO_Map;

uniform sampler2D AM_Map; // not on all models
uniform sampler2D GBMT_Map;
uniform sampler2D MAO_Map;

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


uniform vec2 UV_tiling; //how mant times the texture repetes

uniform vec2 image_size;
uniform int alpha_enable;
uniform int alpha_value;

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
    float metal;
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
    //============================================
    //============================================
    // Calculate UVs based on indexes ============
    // dont touch these!!!!
    float scaleX = 1.0 / atlas_sizes.x;
    float scaleY = 1.0 / atlas_sizes.y;
    vec2 tc = TC1/round(UV_tiling);

    float uox = 1.0/(image_size.x * 0.0625);
    float uoy = 1.0/(image_size.y * 0.0625);
    float usx = 0.875;//1.0-uox*2.0;//1.0-(uox*1.0))/atlas_sizes.x;
    float usy = 0.875;
    vec2 offset = vec2(uox/atlas_sizes.x,uoy/atlas_sizes.y);
    //============================================
    //============================================

    float index=atlas_indexes.x;
    tile.y = floor(index/atlas_sizes.x);
    tile.x = index - tile.y * atlas_sizes.x;
    UV1.x = ((fract(TC1.x)*scaleX*usx)+tile.x*scaleX);
    UV1.y = ((fract(TC1.y)*scaleY*usy)+tile.y*scaleY);
    UV1 += offset;

    index=atlas_indexes.y;
    tile.y = floor(index/atlas_sizes.x);
    tile.x = index - tile.y * atlas_sizes.x;
    UV2.x = ((fract(TC1.x)*scaleX*usx)+tile.x*scaleX);
    UV2.y = ((fract(TC1.y)*scaleY*usy)+tile.y*scaleY);
    UV2+= offset;

    index=atlas_indexes.z;
    tile.y = floor(index/atlas_sizes.x);
    tile.x = index - tile.y * atlas_sizes.x;
    UV3.x = ((fract(TC1.x)*scaleX*usx)+tile.x*scaleX);
    UV3.y = ((fract(TC1.y)*scaleY*usy)+tile.y*scaleY);
    UV3+= offset;

    //this UV is used for blend.
    scaleX = 1.0 / atlas_sizes.z;
    scaleY = 1.0 / atlas_sizes.w;

    index=atlas_indexes.w;
    tile.y = floor(index/atlas_sizes.z);
    tile.x = index - tile.y * atlas_sizes.z;


    UV4.x = (fract(TC2.x)*scaleX)+tile.x*scaleX;
    UV4.y = (fract(TC2.y)*scaleY)+tile.y*scaleY;

    UV4_T.x = (fract(tc.x)*scaleX)+tile.x;
    UV4_T.y = (fract(tc.y)*scaleX)+tile.y;

    // ===========================================================
    // Load Textures... even if they don't exist.
vec4 BLEND;
        if (IS_ATLAS == 1)
        {
            BLEND = texture2D(ATLAS_BLEND_MAP,UV4);
        }
        else
        {
            BLEND = texture2D(ATLAS_BLEND_MAP,UV4_T);
        }

    
    vec4 colorAM_1 = texture2D(ATLAS_AM_Map,fract(UV1))* g_tile0Tint;
    vec4 GBMT_1 = texture2D(ATLAS_GBMT_Map,fract(UV1));
    vec4 MAO_1 = texture2D(ATLAS_MAO_Map,fract(UV1));

    vec4 colorAM_2 = texture2D(ATLAS_AM_Map,fract(UV2))* g_tile1Tint;
    vec4 GBMT_2 = texture2D(ATLAS_GBMT_Map,fract(UV2));
    vec4 MAO_2 = texture2D(ATLAS_MAO_Map,fract(UV2));

    vec4 colorAM_3 = texture2D(ATLAS_AM_Map,fract(UV3))* g_tile2Tint;
    vec4 GBMT_3 = texture2D(ATLAS_GBMT_Map,fract(UV3));
    vec4 MAO_3 = texture2D(ATLAS_MAO_Map,fract(UV3));
   
    vec4 DIRT = texture2D(ATLAS_DIRT_MAP,fract(UV4))* g_dirtColor;
    //DIRT = mix(DIRT, g_dirtColor, g_dirtColor.a);

    vec4 basic_color = texture2D(colorMap,TC1);
    vec4 basic_color2 = texture2D(colorMap2,TC2);
    vec4 bumpMap = texture2D(normalMap,TC1);
    vec4 GMM = texture2D(GMM_map,UV1);
    if (alpha_enable == 1)
    {
    float aRef = float(alpha_value)/255.0;
    if (aRef > bumpMap.r) {
        discard;
    }
    }
    // ===========================================================
     vec4 colorAM;// = DIRT;
          colorAM = mix(colorAM_2,colorAM_1,BLEND.r);
          colorAM = mix(colorAM,colorAM_3,BLEND.g);
          colorAM = mix(colorAM,DIRT,BLEND.b);
          colorAM *= BLEND.a;

    vec4 GBMT = vec4(0.0);
         GBMT = mix(GBMT_2,GBMT_1,BLEND.r);
         GBMT = mix(GBMT,GBMT_3,BLEND.g);

    vec4 MAO = mix(MAO_2,MAO_2,BLEND.r);
         MAO = mix(MAO  ,MAO_3,BLEND.g);

    //=================================================================
    metal = MAO.r;
    if (use_normapMAP == 0 && is_ANM_Map ==0)
    {
        bumpMap = GBMT; //not using normalMap texture?\
        GMM.r = GBMT.r;
        metal = 0.2;
        specular = GBMT.r;
    }
    else
    {
        //bumpMap = bumpMap;
        //specular = GMM.r;
    }
    //if (is_ANM_Map == 1) specular = GMM.r; // USING ANM and not
    
    if ( false )
    {
        bump = bumpMap.xyz * 2.0 - 1.0;
        bump = normalize(bump);
        //bump.y *= - 1.0;
    } else {
        a=bumpMap.r;
        vec2 tb = vec2(bumpMap.ga * 2.0 - 1.0);
        bump.xy    = tb.xy;
        bump.z     = sqrt(1.0 - dot(tb.xy, bumpMap.xy));
        bump       = normalize(bump);
        //bump.y *= - 1.0;
    }
    //=================================================================
    //specular = 0.25;

    //=================================================================
    //colorAM.rgb = mix(colorAM.rgb,vec3(0.04),metal);
    //vec4 test = noise* (1.0-BLEND2.r);
    
    if (IS_ATLAS == 1)
        {
        
        color = colorAM * MAO.g*2.0;
        //color = colorAM;
        }
        else
        {
        colorAM.rgb = basic_color.rgb;
        //if (use_UV2 == 1) colorAM.rgb = basic_color.rgb * basic_color2.rgb;
        color = colorAM;
        }

    //=================================================================
    //vec4 CP = view_mat * vec4(camPosition,0.0);
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
        Ispec1 = vec4(0.2) * clamp(pow(dot(R,E),specPower/specular),0.0,1.0) * S_level;
       
        sum += clamp(Idiff1 +  Ispec1, 0.0, 1.0);

       } //next light

       color_out = correct((Iamb + sum)*1.5 * T_level,1.9,1.3);   // write mixed Color:  
       //color_out.rgb += DIRT.rgb * T_level;//(Iamb + sum) * T_level;   // write mixed Color:  
       //color_out.rgb = (color_out.rgb * 0.0005) + colorAM_1.rgb  * T_level;   // write mixed Color:  

    //=================================================================

}//main