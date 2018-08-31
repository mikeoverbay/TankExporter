// depth_fragment.glsl
// writes to depth texture.
//
#version 130

uniform sampler2D normalMap;

uniform int alphaTest;
uniform int alphaRef;
in vec4 v_position;
in vec2 TC1;
out vec4 fColor;

void main()
    {

    float d = v_position.z / v_position.w ;

    // figure out if we need to discard this.
    if (alphaTest == 1){
        float a = texture2D(normalMap, TC1.st).r;
        float aRef = float(alphaRef)/255.0;
        if (aRef > a) {
            discard;
        }
    }

    d = d* 0.5 + 0.5;
    
    float d2 = d * d;

    // Adjusting moments (this is sort of bias per pixel) using derivative
    float dx = dFdx(d);
    float dy = dFdy(d);
    d2 += 0.25*(dx*dx+dy*dy) ;  
    fColor = vec4( d * 5000.0, d2 * 5000.0, 0.0, 1.0 );
    }