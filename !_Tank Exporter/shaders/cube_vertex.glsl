#version 130
 
out vec3 texCoord;
 
void main() {
    gl_Position    = ftransform();
    texCoord = gl_Vertex.xyz;
    texCoord.x *= -1.0;
}