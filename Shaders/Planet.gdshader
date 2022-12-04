shader_type canvas_item;
uniform float SCALE = 3;
uniform vec3 BASE_COLOR = vec3(.85, .6, .77);
uniform vec3 OUTLINE_COLOR = vec3(0.86, .8, .9);
uniform vec3 COLOR_1 = vec3(.7, .48, .64);
uniform vec3 COLOR_2 = vec3(.6, .34, .54);

float rand(vec2 uv) {
	return fract(sin(dot(uv.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

float noise(vec2 uv)
{
	return rand(uv);
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
	float distanceFromCenter = distance(UV, vec2(0.5, 0.5));
	if(distanceFromCenter > 0.5)
	{
		COLOR.rgba = vec4(0, 0, 0, 0);
	}
	else
	{
		vec2 pixelatedUV = floor(UV * 400.0 * 20.0) / 400.0 / 20.0;

		float generatedNoise = (
			0.6 * perlin_noise(pixelatedUV * 100.0)
			+ 0.4 * noise(pixelatedUV) * 0.3
		) * (
			min(1.0, max(0.0, - (distanceFromCenter - 0.5) / 0.01))
		);
		
		vec3 color = BASE_COLOR;
		
		if (generatedNoise > .1)
			color = COLOR_1;
				
		if (generatedNoise > .3)
			color = COLOR_2;
	
		COLOR.rgb = color;
	}
}
