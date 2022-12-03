shader_type canvas_item;
uniform int cell_amount = 8;
uniform sampler2D noise_pattern;
uniform vec2 period = vec2(3, 3);

float rand(vec2 co){
    return fract(sin(dot(co.xy ,vec2(12.9898,78.233))) * 43758.5453);
}

void fragment(){
	if(distance(UV, vec2(0.5, 0.5)) > 1.0/2.0)
	{
		COLOR.rgb = vec3(0,0,0);
		COLOR.a = 0.0;
	}
	else
	{
		COLOR.rgb = vec3(rand(UV*200.0) * 0.3 + 0.5)
	}
}