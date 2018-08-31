#version 130
// gausian_vertex
// blure shader
uniform int horizontal;
out vec2 TexCoords;
out float x1,x2,x3,x5,x6,x7;
out float y1,y2,y3,y5,y6,y7;

void main(void) {
    const float scale = 1.0/4096.0;
    vec2 ScaleU;
    if (horizontal == 1)
    {
    ScaleU = vec2 (scale,0.0);
    }
    else
    {
    ScaleU = vec2 (0.0,scale);
    }

    x1 = -3.0*ScaleU.x;
    x2 = -2.0*ScaleU.x;
    x3 = -1.0*ScaleU.x;

    x5 =  1.0*ScaleU.x;
    x6 =  2.0*ScaleU.x;
    x7 =  3.0*ScaleU.x;

    y1 = -3.0*ScaleU.y;
    y2 = -2.0*ScaleU.y;
    y3 = -1.0*ScaleU.y;

    y5 =  1.0*ScaleU.y;
    y6 =  2.0*ScaleU.y;
    y7 =  3.0*ScaleU.y;

    TexCoords = gl_MultiTexCoord0.xy;
    gl_Position    = ftransform();

}