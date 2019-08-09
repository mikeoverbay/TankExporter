#version 130
// textureBuilder_vertex
// used to build export texture from atlas sets
uniform sampler2D atlasMap; // not on all models
uniform sampler2D atlasBlend; // not on all models
uniform sampler2D atlasDirt;
uniform vec4 atlas_sizes; // Size of Atlas cells X,Y and Z,W
uniform vec4 atlas_indexes; // cell selection in Atlas X,Y,Z and A

uniform vec2 image_size;

uniform vec4 g_tile0Tint; // tints. Not all have these
uniform vec4 g_tile1Tint;
uniform vec4 g_tile2Tint;
uniform vec4 g_dirtColor;
uniform vec2 UV_tiling; //how many times the texture repetes

in vec2 TC1;// UV coords
in vec2 TC2;

out vec4 color_out;

vec4 correct(in vec4 hdrColor, in float exposure, in float gamma_level){  
    // Exposure tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor.rgb * exposure);
    // Gamma correction 
    mapped.rgb = pow(mapped.rgb, vec3(1.0 / gamma_level));  
    return vec4 (mapped, hdrColor.a);
}

void main(void){

    vec2 UV1;
    vec2 UV2;
    vec2 UV3;
    vec2 UV4;
    vec2 tile;
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

    //============================================
    float uox = 0.0625;
    float uoy = 0.0625;

    float usx = 0.875;
    float usy = 0.875;
    vec2 offset = vec2(uox/At_size.x, uoy/At_size.y);

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

    offset = vec2(uox/At_size.z, uoy/At_size.w);
    index = atlas_indexes.w;
    tile.y = floor(index/At_size.z);
    tile.x = index - tile.y * At_size.z;

    UV4.x = (fract(TC2.x)*scaleX)+tile.x*scaleX;
    UV4.y = (fract(TC2.y)*scaleY)+tile.y*scaleY;
    //============================================
    //============================================
    

    vec4 BLEND = texture2DLod(atlasBlend,UV4,0.0);
    //============================================
    vec4 colorAM_1 = texture2DLod(atlasMap,UV1,0.0) * g_tile0Tint;
    vec4 colorAM_2 = texture2DLod(atlasMap,UV2,0.0) * g_tile1Tint;
    vec4 colorAM_3 = texture2DLod(atlasMap,UV3,0.0) * g_tile2Tint;

    vec4 DIRT = texture2DLod(atlasDirt,UV4,0.0);
    DIRT.rgb *= g_dirtColor.rgb;
    //============================================
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

          color_out.rgb =  colorAM.rgb;// * vec3(0.6) + (BLEND.rgb*BLEND.a*0.5);
          color_out.a   =  1.0;

 }