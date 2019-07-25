#version 130
//AtlasPBR_vertex.glsl
//Used to light all models

out vec2 TC1;
out vec2 TC2;

out mat3 TBN;

out vec3 vNormal;

out vec3 vVertex;

void main(void) {

    TC1 = gl_MultiTexCoord0.xy;
    TC2 = gl_MultiTexCoord4.xy;

    vec3 n = normalize(gl_NormalMatrix * gl_Normal);
    vec3 t = normalize(gl_NormalMatrix * gl_MultiTexCoord1.xyz);
    vec3 b = normalize(gl_NormalMatrix * gl_MultiTexCoord2.xyz);

    vNormal = n;
    float invmax = inversesqrt(max(dot(t, t), dot(b, b)));
    TBN = mat3(t * invmax, b * invmax, n * invmax);

    vVertex = vec3(gl_ModelViewMatrix * gl_Vertex);
    gl_Position    = ftransform();

}