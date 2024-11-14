//noraml drawing fragment shader

#version 120
in vec3 fsColor; // Color from geometry shader

void main()
{
	gl_FragColor = vec4(fsColor, 1.0);
}