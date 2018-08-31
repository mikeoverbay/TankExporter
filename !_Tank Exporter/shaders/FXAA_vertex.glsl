#version 130
// FXAA_vertex
// final mix shader
out vec2 TexCoords;

void main(void) {

    TexCoords = gl_MultiTexCoord0.xy;
	gl_Position    = ftransform();

}