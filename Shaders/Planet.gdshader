shader_type canvas_item;
uniform float PIXEL_SCALE = 8000.;
uniform float BORDER_SIZE = 100;
uniform vec4 COLOR_1 : hint_color = vec4(0.390625, 0.29296875, 0.25, 1.);
uniform vec4 COLOR_2 : hint_color = vec4(0.31640625, 0.22265625, 0.1796875, 1.);
uniform vec4 OUTLINE_COLOR_1 : hint_color  = vec4(0.2734375, 0.27734375, 0.140625, 1.);
uniform vec4 OUTLINE_COLOR_2 : hint_color = vec4(0.32421875, 0.3515625, 0.13671875, 1.);

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
		vec2 pixelatedUV = floor(UV * PIXEL_SCALE) / PIXEL_SCALE;
	
		COLOR.rgb = mix(
			mix(
				COLOR_1.rgb,
				COLOR_2.rgb,
				clamp(1.0, 0.0, (
					(pow(distanceFromCenter, 2) + pow(distanceFromCenter, 20))
					* (0.2 + 0.8 * perlin_noise(pixelatedUV * 50.0))
					* (0.5 + 0.5 * noise(pixelatedUV))
				))
			),
			mix(
				OUTLINE_COLOR_1.rgb,
				OUTLINE_COLOR_2.rgb,
				noise(pixelatedUV)
			),
			step(0.4, clamp(1.0, 0.0, (
				pow(distanceFromCenter, BORDER_SIZE)
				* (0.6 + 0.4 * perlin_noise(pixelatedUV * 50.0))
				* (0.8 + 0.2 * noise(pixelatedUV))
			)))
		);
	}
}
