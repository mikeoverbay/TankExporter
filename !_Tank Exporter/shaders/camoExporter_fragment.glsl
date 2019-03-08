#version 130
//camoExporter_fragment.glsl

uniform sampler2D camo_Map;
uniform sampler2D AO_Map;
uniform sampler2D AM_Map;
uniform vec4 tile;
uniform vec4 camo_tile;
in vec2 TC1;

void main(void){

vec2 sTC = TC1 * camo_tile.xy;
sTC *= tile.xy;
sTC = sTC + camo_tile.zw;
sTC = sTC + tile.zw;

float alpha = texture2D(AO_Map,TC1).a;

vec4 camo_color = texture2D(camo_Map,sTC).rgba;

vec4 base = texture2D(AM_Map,TC1).rgba;

gl_FragColor = mix(base, camo_color, alpha);


}