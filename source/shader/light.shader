shader_type canvas_item;

uniform vec4 light_color = vec4(1.0,1.0,1.0,1.0);//光照颜色
uniform float light_intensity = 0.5f;//光照强度

void fragment()
{
	if(AT_LIGHT_PASS)
	{
		COLOR = vec4(texture(TEXTURE,UV));
	}
	else
	{
		COLOR = texture(TEXTURE,UV) * vec4(light_color.rgb * light_intensity,1.0);
	}
}

void light() {
	
}