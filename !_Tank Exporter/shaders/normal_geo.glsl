//normal drawing geo shader
#version 330 compatibility

layout (triangles) in;
layout (line_strip) out;
layout (max_vertices = 6) out;

#extension GL_EXT_geometry_shader4 : enable         

uniform int mode;
in vData
{
    vec3 normal;
    vec4 vert;
}vertices[];

void main()
{
 vec4 sumV;
 vec4 sumN;
   
 if (mode == 1) {
 sumV = vec4(vertices[0].vert + vertices[1].vert + vertices[2].vert);
 sumN.xyz = vec3(vertices[0].normal.xyz + vertices[1].normal.xyz + vertices[2].normal.xyz) / vec3(3.0,3.0,3.0);
 sumN = vec4(sumN.xyz,0.0);
        gl_Position = gl_ModelViewProjectionMatrix * sumV;
        EmitVertex();      
        gl_Position = gl_ModelViewProjectionMatrix * (sumV + (sumN * 0.05));
        EmitVertex();
        EndPrimitive();
     
  }
  else
  {
    for(int i = 0; i < gl_in.length(); ++i)
    {
        gl_Position = gl_ModelViewProjectionMatrix * vertices[i].vert;
        EmitVertex();      
 
        gl_Position = gl_ModelViewProjectionMatrix * (vertices[i].vert + (vec4(vertices[i].normal, 0) * 0.05));
        EmitVertex();      
 
        EndPrimitive();
    }
  }
}