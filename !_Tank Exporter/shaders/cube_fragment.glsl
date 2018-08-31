#version 130
 
in vec3 texCoord;
out vec4 fragColor;
uniform samplerCube cubeMap;
 
void main (void) {
    fragColor = texture(cubeMap, texCoord);
}