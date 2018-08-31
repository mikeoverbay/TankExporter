//decalCpass_vertex.glsl

#version 330 compatibility 

out mat4 matPrjInv;
out mat4 invDecal_mat;
out mat2 uv_matrix;

out vec4 ShadowCoord;
out vec4 positionSS;
out vec3 v_Position;

uniform mat4 decal_matrix;
uniform float uv_rotate;

void main(void)
{
    gl_Position = decal_matrix * gl_Vertex;
    gl_Position = gl_ModelViewProjectionMatrix * gl_Position;
    positionSS =gl_Position;

    v_Position = vec3(gl_ModelViewMatrix * gl_Vertex);

    matPrjInv = inverse(gl_ModelViewProjectionMatrix);

    invDecal_mat = inverse(decal_matrix);
    uv_matrix = mat2 (
        vec2( cos(uv_rotate), -sin(uv_rotate) ), 
        vec2( sin(uv_rotate),  cos(uv_rotate) )
        );                  
}
