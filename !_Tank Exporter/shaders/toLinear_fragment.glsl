// toLinear_fragment.gsls
// used to linearize depth textures to linear colors.
#version 330 compatibility

uniform sampler2D depthMap;
varying vec2 texCoord;

float mininput = 0.2;
float maxinput = 0.8;
float minoutput = 0.0;
float maxoutput = 0.8;

float linearDepth(float depthSample)
{
    float f = 25.0;
    float n = 1.0;
    
    //depthSample = 2.0 * depthSample - 1.0;
    //float zLinear = 2.0 * zNear * zFar / (zFar + zNear - depthSample * (zFar - zNear));
    return  (2.0 * n) / (f + n - depthSample * (f - n));
}

void main(){

    float r = linearDepth(texture2D(depthMap, texCoord).r/1000.0);
    gl_FragColor = vec4(vec3( r*3.0), 1.0);
    vec3 acolor = min(max(gl_FragColor.rgb - vec3(mininput), vec3(0.0)) / (vec3(maxinput) - vec3(mininput)), vec3(1.0));
    gl_FragColor.rgb = mix(vec3(minoutput), vec3(maxoutput), acolor);

}