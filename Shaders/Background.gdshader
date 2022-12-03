shader_type canvas_item;
uniform float ROTATION = 0.0;
uniform float ASPECT = 1.7;

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

void fragment() {
	mat2 rotation = mat2(vec2(sin(-ROTATION), -cos(-ROTATION)), vec2(cos(-ROTATION), sin(-ROTATION)));
	vec2 coords = (UV - vec2(0.5)) * vec2(ASPECT, 1) * rotation + vec2(0.5);

	COLOR.rgb = vec3(pow(perlin_noise(coords * 100.0), 1000));
}
