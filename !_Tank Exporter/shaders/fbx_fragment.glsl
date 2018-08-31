//fbx_fragment.glsl
//Used to light all FBX imports
#version 130
uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform sampler2D specularMap;
uniform float A_level;
uniform float S_level;
uniform float T_level;
uniform int t_cnt;
uniform int is_GAmap;
uniform int alphaTest;
uniform int enableVcolor;

in vec2 TC1;
in vec3 n;
in vec3 vVertex;
in vec3 Vcolor;
//in vec3 lightDirection;
in mat3 TBN;

out vec4 color_out;

void main(void) {

//--------------------------------
vec4   cc = vec4(0.0);
vec3   lightDirection;
vec4   detailBump;
vec3   bump;
vec3   bumpD;
vec3   PN_D;
float  alpha;
vec4   color;
float  a;
//float  spec_l = 1.0;
float  specPower = 60.0;
vec4   Ispec1 = vec4(0.0);
vec4   Idiff1 = vec4(0.0);
vec4   sum = vec4(0.0);
//--------------------------------

// Load textures
    vec4 base        = texture2D(colorMap,  TC1.st);
    vec4 bumpMap     = texture2D(normalMap, TC1.st);
    vec4 specMap     = texture2D(specularMap,    TC1.st);
//--------------------------------
    color = base;
    float factor = 1.0;
    a = base.a;

if (is_GAmap == 0 )
    {
        bump = bumpMap.xyz * 2.0 - 1.0;
        bump       = normalize(bump);
        bump.y *= - 1.0;
    } else {
        a=bumpMap.r;
        bumpMap.ag = bumpMap.ag *2.0 - 1.0;
        bump.xy    = bumpMap.ag;
        bump.z     = sqrt(1.0 - dot(bumpMap.ga, bumpMap.ga));
        bump       = normalize(bump);
        bump.y *= - 1.0;
    }

    //AO     = vec4(1.0);
    //GMM = vec2 (1.0);
//==================================

    if (alphaTest == int(1)) { if (a < 0.5) {discard;} }
    vec3 E = normalize(-vVertex);    // we are in Eye Coordinates, so EyePos is (0,0,0)  
    vec3 PN = normalize(TBN * bump); // Get the perturbed normal
    if ((bumpMap.x + bumpMap.y + bumpMap.z )==3.0)
		{
		PN = normalize(n);
		color.rgb *= vec3(0.7);
		}
    vec4 Iamb     = color * A_level ; //calculate Ambient Term:  
    // loop thru lights and calc lighting.
    for (int i = 0 ; i < 3 ; i++)
    {
        vec3 L = normalize(gl_LightSource[i].position.xyz - vVertex);   
        vec3 R = normalize(reflect(-L,PN));  
        //calculate Diffuse Term:  
        Idiff1 = color * max(dot(PN,L), 0.0);//color light level
        Idiff1 = clamp(Idiff1, 0.0, 1.0);     

        // calculate Specular Term:
        Ispec1 = vec4(0.3) * pow(max(dot(R,E),0.0),specPower);
        Ispec1 = clamp(Ispec1, 0.0, 1.0); 

        vec4 IspecMix = clamp(mix(Ispec1,vec4(0.33),specMap.r),0.0,1.0) * 2.0 * S_level;
        sum += clamp(Idiff1 +  IspecMix, 0.0, 1.0);

    } //next light

gl_FragColor = (Iamb + sum) * T_level;   // write mixed Color:  
if (enableVcolor == 1)
{
	gl_FragColor.rgb = (gl_FragColor.rgb * 0.0) + clamp(Vcolor.rgb * 2.0,0.0,1.0);
}
}

