#version 130
//terrainShader_vertex.glsl
//Used to light terrain


out vec3 v_Position;
out vec3 v_Normal;
out vec3 w_Normal;
out vec2 TC1;
out vec3 vertex;
out float l_dist; // used to clip the shadow map
void main(void){

    TC1 = gl_MultiTexCoord0.xy * 4.0;
    vertex = gl_Vertex.xyz;
    v_Normal = gl_NormalMatrix * gl_Normal;
    w_Normal = gl_Normal;
    v_Position = vec3(gl_ModelViewMatrix * gl_Vertex);

    l_dist = sqrt(gl_Vertex.x*gl_Vertex.x + gl_Vertex.z*gl_Vertex.z);
    gl_Position    = ftransform();

}