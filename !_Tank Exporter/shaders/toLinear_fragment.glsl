// toLinear_fragment.gsls
// used to linearize depth textures to linear colors.
#version 330 compatibility

uniform sampler2D depthMap;
varying vec2 texCoord;


float linearDepth(float depthSample)
{
    float f = 30.0;
    float n = 5.0;
    
    //depthSample = 2.0 * depthSample - 1.0;
    //float zLinear = 2.0 * zNear * zFar / (zFar + zNear - depthSample * (zFar - zNear));
    return  (2.0 * n) / (f + n - depthSample * (f - n));
}

void main(){

    float r = linearDepth(texture2D(depthMap, texCoord).r/10000.0);
    gl_FragColor = vec4(vec3(pow( r,6.0))*200.0, 1.0);

}