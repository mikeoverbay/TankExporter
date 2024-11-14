//normal drawing geo shader
#version 330 compatibility

layout(triangles) in;
layout(line_strip) out;
layout(max_vertices = 6) out;
out vec3 fsColor; // Color to fragment shader

#extension GL_EXT_geometry_shader4 : enable

uniform int mode;
in vData
{
	vec3 normal;
	vec4 vert;
}vertices[];

void main()
{
	// Define hard-coded colors: red, green, blue
	vec3 colors[4];
	colors[0] = vec3(1.0, 0.0, 0.0); // Red
	colors[1] = vec3(0.0, 1.0, 0.0); // Green
	colors[2] = vec3(0.0, 0.0, 1.0); // Blue
	colors[3] = vec3(1.0, 1.0, 1.0); // white

	vec4 sumV;
	vec4 sumN;

	if (mode == 1) {
		fsColor = colors[3]; // Assign red, green, blue in cycle

		sumV = vec4(vertices[0].vert + vertices[1].vert + vertices[2].vert);
		sumN.xyz = vec3(vertices[0].normal.xyz + vertices[1].normal.xyz + vertices[2].normal.xyz) / vec3(3.0, 3.0, 3.0);
		sumN = vec4(sumN.xyz, 0.0);
		gl_Position = gl_ModelViewProjectionMatrix * sumV;
		EmitVertex();
		gl_Position = gl_ModelViewProjectionMatrix * (sumV + (sumN * 0.1));
		EmitVertex();
		EndPrimitive();

	}
	else
	{
		for (int i = 0; i < gl_in.length(); ++i)

		{
			// Use colors based on the index modulo the array length
			fsColor = colors[i % 3]; // Assign red, green, blue in cycle
			gl_Position = gl_ModelViewProjectionMatrix * vertices[i].vert;
			EmitVertex();

			gl_Position = gl_ModelViewProjectionMatrix * (vertices[i].vert + (vec4(vertices[i].normal, 0) * 0.05));
			EmitVertex();

			EndPrimitive();
		}
	}
}