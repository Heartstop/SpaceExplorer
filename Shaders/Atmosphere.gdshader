shader_type canvas_item;
uniform float PIXEL_SCALE = 750.0;

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

	if (distanceFromCenter > 1.0)
	{
		COLOR.rgba = vec4(0, 0, 0, 0);
	}
	else
	{
		vec2 pixelatedUV = floor(UV * PIXEL_SCALE) / PIXEL_SCALE;
		float pixelatedDistanceFromCenter = (1.0 - 2.0 * distance(pixelatedUV, vec2(0.5, 0.5))) * 4.5 - 0.5;

		float density =
			pixelatedDistanceFromCenter
			- perlin_noise(pixelatedUV * 10.0) * 0.2
			- perlin_noise(pixelatedUV * 30.0) * 0.1;
		COLOR.rgba =
			vec4(
				mix(
					vec3(0.19f, 0.23f, 0.26f),
					vec3(0.07f, 0.1f, 0.12f),
					0.5
				),
				// + (step(1.0 - density, 0.0)) * 1.0
				+ (step(1.0 - density, 0.1)) * 0.09
				+ (step(1.0 - density, 0.2)) * 0.09
				+ (step(1.0 - density, 0.3)) * 0.09
				+ (step(1.0 - density, 0.4)) * 0.09
				+ (step(1.0 - density, 0.5)) * 0.09
				+ (step(1.0 - density, 0.6)) * 0.09
				+ (step(1.0 - density, 0.7)) * 0.09
				+ (step(1.0 - density, 0.8)) * 0.09
				+ (step(1.0 - density, 0.9)) * 0.09
				+ (step(1.0 - density, 1.0)) * 0.09
			);
	}
}
