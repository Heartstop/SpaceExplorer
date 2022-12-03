shader_type canvas_item;
uniform int cell_amount = 8;
uniform sampler2D noise_pattern;
uniform vec2 period = vec2(3, 3);

void fragment(){
	if(distance(UV, vec2(0.5, 0.5)) > 1.0/2.0)
	{
		COLOR.rgb = vec3(0,0,0);
		COLOR.a = 0.0;
	}
	else
	{
		vec4 text = texture(noise_pattern, UV);
		text = text * 0.3 + 0.3;
		COLOR = text;
	}
}