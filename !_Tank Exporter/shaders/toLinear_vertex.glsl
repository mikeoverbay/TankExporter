// toLinear_vertex.gsls
// used to linearize depth textures to linear colors.
#version 330 compatibility

varying vec2 texCoord;
void main(void)
{ 
        gl_Position = ftransform();     
        texCoord    = gl_MultiTexCoord0.xy;
}