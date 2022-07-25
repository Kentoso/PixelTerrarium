shader_type canvas_item;

uniform sampler2D palette : hint_black;
uniform sampler2D gameMap : hint_black;
uniform float mapSize;

void fragment() {
	vec2 coords = UV * mapSize;
	float mapVal = texelFetch(gameMap, ivec2(coords), 0).r;
	mapVal *= 255f;
	vec4 textureValue = texelFetch(palette, ivec2(int(mapVal), 0), 0);
	COLOR = textureValue;
}