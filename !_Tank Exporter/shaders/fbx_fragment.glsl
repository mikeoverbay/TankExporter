#version 130

uniform sampler2D colorMap;  // Base color map
uniform sampler2D normalMap; // Normal map
uniform vec3 lightColor[3];  // Array for the colors of 3 lights
uniform vec3 lightPos[3];    // Array for the positions of 3 lights
uniform float S_level;       // Specular level
uniform float A_level;       // Ambient level

in vec2 TC1;                 // Texture coordinates
in vec3 vVertex;             // Vertex position in world space
in mat3 TBN;                 // Tangent, Bitangent, Normal matrix (TBN)

out vec4 color_out;          // Final output color

void main(void) {
    // Sample the normal map and flip the Y component
    if (texture2D(normalMap, TC1.st).a < 0.5)discard;
    vec3 bump = texture2D(normalMap, TC1.st).xyz * 2.0 - 1.0;
    bump.y *= -1.0;  // Flip the Y component of the normal
    vec3 perturbedNormal = normalize(TBN * bump);
    vec4 baseColor = texture2D(colorMap, TC1.st);

    vec3 finalColor = vec3(0.0);

    // Loop over the 3 lights
    for (int i = 0; i < 3; i++) {
        vec3 lightDir = normalize(lightPos[i] - vVertex);
        vec3 viewDir = normalize(-vVertex); // Assuming the view direction is from the camera at the origin

        // Diffuse lighting
        float diff = max(dot(perturbedNormal, lightDir), 0.0);
        vec3 diffuseColor = diff * lightColor[i];

        // Specular lighting
        vec3 reflectDir = reflect(-lightDir, perturbedNormal);
        float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32.0); // Specular exponent (shininess)
        vec3 specularColor = S_level * spec * lightColor[i] * 4.0;

        finalColor += (diffuseColor + specularColor) * baseColor.rgb * 0.6;
    }

    // Ambient term
    vec3 ambientColor = baseColor.rgb * A_level;

    // Final color output
    color_out = vec4(finalColor + ambientColor, baseColor.a); // Preserve alpha from the base color
}
