shader_type canvas_item;
uniform float SCALE = 1;
uniform vec3 BASE_COLOR = vec3(.85, .6, .77);
uniform vec3 OUTLINE_COLOR = vec3(0.86, .8, .9);
uniform vec3 COLOR_1 = vec3(.7, .48, .64);
uniform vec3 COLOR_2 = vec3(.6, .34, .54);

float rand(vec2 uv) {
	return fract(sin(dot(uv.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

float noise(vec2 uv)
{
	return rand(uv * 50.0);
}

float perlin_noise(vec2 coord)
{
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

void fragment(){
	float distanceFromCenter = 2.0 * distance(UV, vec2(0.5, 0.5));

	if(distanceFromCenter > 1.0)
	{
		COLOR.rgba = vec4(0, 0, 0, 0);
	}
	else
	{
		vec2 pixelatedUV = floor(UV * 8000.0) / 8000.0;
	
		COLOR.rgb = mix(
			mix(
				vec3(0.390625, 0.29296875, 0.25),
				vec3(0.31640625, 0.22265625, 0.1796875),
				clamp(1.0, 0.0, (
					(pow(distanceFromCenter, 2) + pow(distanceFromCenter, 20))
					* (0.2 + 0.8 * perlin_noise(pixelatedUV * 50.0))
					* (0.5 + 0.5 * noise(pixelatedUV))
				))
			),
			mix(
				vec3(0.2734375, 0.27734375, 0.140625),
				vec3(0.32421875, 0.3515625, 0.13671875),
				noise(pixelatedUV)
			),
			step(0.4, clamp(1.0, 0.0, (
				pow(distanceFromCenter, 100)
				* (0.6 + 0.4 * perlin_noise(pixelatedUV * 50.0))
				* (0.8 + 0.2 * noise(pixelatedUV))
			)))
		);
	}
}
