#version 120
#extension GL_EXT_geometry_shader4 : enable         
 
//uniform float uNormalsLength;      
uniform int mode;
varying in vec3 normal[];      
void main()
{
   
 if (mode == 1) {
 vec4 sumV;
 vec4 sumN;
 sumV = (gl_PositionIn[0] + gl_PositionIn[1] + gl_PositionIn[2]) / 3.0;
 sumN.xyz = (normal[0].xyz + normal[1].xyz + normal[2].xyz) / 3.0;
 sumN.w = 0.0;
        gl_Position = gl_ModelViewProjectionMatrix * sumV;
        EmitVertex();      
        gl_Position = gl_ModelViewProjectionMatrix * (sumV + (sumN * 0.03));
        EmitVertex();      
 }
 else
 {
    for(int i = 0; i < gl_VerticesIn; ++i)
    {
        gl_Position = gl_ModelViewProjectionMatrix * gl_PositionIn[i];
        EmitVertex();      
 
        gl_Position = gl_ModelViewProjectionMatrix * (gl_PositionIn[i] + (vec4(normal[i], 0) * 0.03));
        EmitVertex();      
 
        EndPrimitive();
    }
}
}