shader_type canvas_item;

uniform float flash_amount : hint_range(0.0, 1.0) = 0.0;
uniform vec4 flash_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
	vec4 original = texture(TEXTURE, UV);
	COLOR.rgb = mix(original, flash_color, flash_amount).rgb;
	COLOR.a = original.a;
}
