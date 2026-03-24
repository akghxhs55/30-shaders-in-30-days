shader_type canvas_item;

uniform int swap_mode : hint_enum("Direct", "Palette");

// Direct mode parameters
uniform vec4 source_colors[8] : source_color;
uniform vec4 target_colors[8] : source_color;
uniform int color_count : hint_range(1, 8) = 1;
uniform float tolerance : hint_range(0.0, 1.0) = 0.1;

// Palette mode parameters
uniform sampler2D index_texture : filter_nearest;
uniform sampler2D palette_texture : hint_default_white;

void fragment() {
	vec4 original = texture(TEXTURE, UV);
	
	vec4 color = original;
	switch (swap_mode) {
		case 0: // Direct
			for (int i = 0; i < color_count; i++) {
				float dist = distance(original.rgb, source_colors[i].rgb);
				float t = step(dist, tolerance);
				color = mix(color, vec4(target_colors[i].rgb, original.a), t);
			}
			break;
			
		case 1: // Palette
			vec4 index_color = texture(index_texture, UV);
			vec4 swapped_color = texture(palette_texture, vec2(index_color.r, 0.5));
			color = vec4(swapped_color.rgb, index_color.a);
			break;
	}
	
	COLOR = color;
}
