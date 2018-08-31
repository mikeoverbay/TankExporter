#version 130
//tank_vertex.glsl
//Used to light all models

uniform mat4 shadowProjection;

out vec4 ShadowCoord;
out vec3 vVertex;

out vec3 v_Position;
out vec2 TC1;
out mat3 TBN;

out vec3 t;
out vec3 b;
out vec3 v_Normal;
out vec3 vertexColor;
void main(void) {

    TC1 = gl_MultiTexCoord0.xy;

    ShadowCoord= shadowProjection * gl_Vertex;

    vec3 n = normalize(gl_NormalMatrix * gl_Normal);
    vec3 t = normalize(gl_NormalMatrix * gl_MultiTexCoord1.xyz);
    vec3 b = normalize(gl_NormalMatrix * gl_MultiTexCoord2.xyz);
	vertexColor = gl_MultiTexCoord3.xyz;
    v_Normal = n;
    float invmax = inversesqrt(max(dot(t, t), dot(b, b)));
    TBN = mat3(t * invmax, b * invmax, n * invmax);

    v_Position = vec3(gl_ModelViewMatrix * gl_Vertex);
    gl_Position    = ftransform();

}