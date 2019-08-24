// toLinear_fragment.gsls
// used to linearize depth textures to linear colors.
#version 330 compatibility

uniform sampler2D depthMap;
varying vec2 texCoord;

float mininput = 0.4;
float maxinput = 0.9;
float minoutput = 0.0;
float maxoutput = 0.9;

float linearDepth(float depthSample)
{
    float f = 20.0;
    float n = 7.0;
    
    //depthSample = 2.0 * depthSample - 1.0;
    //float zLinear = 2.0 * zNear * zFar / (zFar + zNear - depthSample * (zFar - zNear));
    return  (2.0 * n) / (f + n - depthSample * (f - n));
}

void main(){

    float r = linearDepth(texture2D(depthMap, texCoord).r/250.0);
    gl_FragColor = vec4(vec3( r*1.0), 1.0);
    vec3 acolor = min(max(gl_FragColor.rgb - vec3(mininput), vec3(0.0)) / (vec3(maxinput) - vec3(mininput)), vec3(1.0));
    gl_FragColor.rgb = mix(vec3(minoutput), vec3(maxoutput), acolor);

}