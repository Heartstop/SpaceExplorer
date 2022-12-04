shader_type canvas_item;
uniform float ROTATION = 0.0;

const float PI = 3.141592653589793;
const float TAU = 6.283185307179586;

float rand(vec2 cord) {
	cord = mod(cord, 10000.0);
	return fract(sin(dot(cord.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

float noise(vec2 uv) {
	return rand(uv);
}

float perlin_noise(vec2 coord) {
	vec2 i = floor(coord);
	vec2 f = fract(coord);
	float rad2 = 6.283;
	float tl = rand(i) * rad2;
	float tr = rand(i + vec2(1.0, 0.0)) * rad2;
	float bl = rand(i + vec2(0.0, 1.0)) * rad2;
	float br = rand(i + vec2(1.0, 1.0)) * rad2;
	
	vec2 tlvec = vec2(-sin(tl), cos(tl));
	vec2 trvec = vec2(-sin(tr), cos(tr));
	vec2 blvec = vec2(-sin(bl), cos(bl));
	vec2 brvec = vec2(-sin(br), cos(br));
	
	float tldot = dot(tlvec, f);
	float trdot = dot(trvec, f - vec2(1.0, 0.0));
	float bldot = dot(blvec, f - vec2(0.0, 1.0));
	float brdot = dot(brvec, f - vec2(1.0, 1.0));
	
	vec2 cubic = f * f * (3.0 - 2.0 * f);
	
	float topmix = mix(tldot, trdot, cubic.x);
	float botmix = mix(bldot, brdot, cubic.x);
	float wholemix = mix(topmix, botmix, cubic.y) + 0.5;
	
	return clamp(wholemix, 0, 1);
}

vec3 star(vec2 coord) {
	return vec3(pow(0.00000000000000001, perlin_noise(coord * 250.0)));
}

void fragment() {
	float aspect = SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
	mat2 rotation = mat2(vec2(sin(-ROTATION), -cos(-ROTATION)), vec2(cos(-ROTATION), sin(-ROTATION)));
	vec2 coord = (UV - vec2(0.5)) * vec2(aspect, 1) * rotation + vec2(0.5);

	COLOR.rgb =
		max(
			vec3(0.07421875, 0.1015625, 0.12109375),
			(
				  star(coord + SCREEN_PIXEL_SIZE * vec2(-0.5, -0.5))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2( 0.0, -0.5))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2( 0.5, -0.5))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2(-0.5,  0.0))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2( 0.0,  0.0))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2( 0.0,  0.0))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2( 0.5,  0.0))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2(-0.5,  0.5))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2( 0.0,  0.5))
				+ star(coord + SCREEN_PIXEL_SIZE * vec2( 0.5,  0.5))
			) * 0.1
		);
}
