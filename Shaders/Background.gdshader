shader_type canvas_item;
uniform float ROTATION = 0.0;

const float PI = 3.141592653589793;
const float TAU = 6.283185307179586;

float rand(vec2 cord) {
	cord = mod(cord, 10000.0);
	return fract(sin(dot(cord.xy, vec2(12.9898,78.233))) * 43758.5453);
}

float noise(vec2 uv) {
	return rand(uv * 1000.0);
}

void fragment() {
	float aspect = SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
	mat2 rotation = mat2(vec2(sin(-ROTATION), -cos(-ROTATION)), vec2(cos(-ROTATION), sin(-ROTATION)));
	vec2 coord = (UV - vec2(0.5)) * vec2(aspect, 1) * rotation + vec2(0.5);

	vec2 pixelatedCoord = floor(coord * 600.0) / 600.0;
	vec2 subArea = floor(coord * 120.0) / 120.0;
	vec2 subAreaCoord = (pixelatedCoord - subArea) * 120.0;

	float starX = floor(noise(subArea) * 100.0) / 100.0;
	float starY = floor(noise(-subArea) * 100.0) / 100.0;

	COLOR.rgb = vec3(0.07421875, 0.1015625, 0.12109375);

	if (distance(subAreaCoord, vec2(starX, starY)) < 0.1)
		COLOR.rgb += vec3(pow(noise(subArea * 2.0), 12));
}
