shader_type canvas_item;

uniform int outline_mode : hint_enum("Four Directional", "Eight Directional", "Radial");

// Radial parameters
uniform int sample_count : hint_range(1, 64) = 16;

uniform int placement : hint_enum("Outer", "Inner", "Both");

uniform float thickness : hint_range(0.0, 40.0) = 10.0;
uniform vec4 outline_color : source_color = vec4(1.0);

void fragment() {
	vec4 original = texture(TEXTURE, UV);
	float alpha_mask = step(0.5, original.a);
	
	float max_neighbor_alpha = 0.0;
	float min_neighbor_alpha = 1.0;
	switch (outline_mode) {
		case 0: // Four Directional
			vec2 offsets[4] = {
				vec2(0, 1), vec2(0, -1), vec2(1, 0), vec2(-1, 0)
			};
			for (int i = 0; i < 4; i++) {
				vec2 offset_uv = UV + offsets[i] * thickness * TEXTURE_PIXEL_SIZE;
				max_neighbor_alpha = max(max_neighbor_alpha, texture(TEXTURE, offset_uv).a);
				min_neighbor_alpha = min(min_neighbor_alpha, texture(TEXTURE, offset_uv).a);
			}
			break;
			
		case 1: // Eight Directional
			vec2 offsets[8] = {
				vec2(0, 1), vec2(0, -1), vec2(1, 0), vec2(-1, 0),
				vec2(1, 1)/sqrt(2.0), vec2(1, -1)/sqrt(2.0), vec2(-1, 1)/sqrt(2.0), vec2(-1, -1)/sqrt(2.0)
			};
			for (int i = 0; i < 8; i++) {
				vec2 offset_uv = UV + offsets[i] * thickness * TEXTURE_PIXEL_SIZE;
				max_neighbor_alpha = max(max_neighbor_alpha, texture(TEXTURE, offset_uv).a);
				min_neighbor_alpha = min(min_neighbor_alpha, texture(TEXTURE, offset_uv).a);
			}
			break;
			
		case 2: // Radial
			for (int i = 0; i < sample_count; i++) {
				float angle = float(i) / float(sample_count) * TAU;
				vec2 offset_uv = UV + vec2(cos(angle), sin(angle)) * thickness * TEXTURE_PIXEL_SIZE;
				max_neighbor_alpha = max(max_neighbor_alpha, texture(TEXTURE, offset_uv).a);
				min_neighbor_alpha = min(min_neighbor_alpha, texture(TEXTURE, offset_uv).a);
			}
			break;
	}
	
	float outline_mask = 0.0;
	switch (placement) {
		case 0: // Outer
			outline_mask = (1.0 - alpha_mask) * max_neighbor_alpha;
			break;
			
		case 1: // Inner
			outline_mask = alpha_mask * (1.0 - min_neighbor_alpha);
			break;
			
		case 2: // Both
			outline_mask = max((1.0 - alpha_mask) * max_neighbor_alpha, alpha_mask * (1.0 - min_neighbor_alpha));
			break;
	}
	
	COLOR = mix(original, outline_color, outline_mask * outline_color.a);
	COLOR.a = max(original.a, outline_mask * outline_color.a);
}
