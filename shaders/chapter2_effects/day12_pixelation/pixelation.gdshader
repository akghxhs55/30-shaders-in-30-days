shader_type canvas_item;

uniform int pixelation_mode : hint_enum("Snap", "Average", "Dot");
uniform int pixel_count : hint_range(1, 512) = 64;

// Average mode parameters
uniform int sample_count : hint_range(1, 16) = 4;

// Dot mode parameters
uniform float dot_size : hint_range(0.0, 1.0) = 0.5;

void fragment() {
    vec4 original = texture(TEXTURE, UV);
	
	vec4 color = original;
	switch (pixelation_mode) {
		case 0: // Snap
			vec2 snapped = (floor(UV * float(pixel_count)) + 0.5) / float(pixel_count);
			color = texture(TEXTURE, snapped);
			break;
			
		case 1: // Average
			vec2 cell = floor(UV * float(pixel_count)) / float(pixel_count);
			vec2 cell_size = vec2(1.0) / float(pixel_count);
			
			vec4 sum = vec4(0.0);
			for (int x = 0; x < sample_count; x++) {
				for (int y = 0; y < sample_count; y++) {
					vec2 offset = (vec2(float(x), float(y)) + 0.5) / float(sample_count) * cell_size;
					sum += texture(TEXTURE, cell + offset);
				}
			}
			color = sum / float(sample_count * sample_count);
			break;
			
		case 2: // Dot
			vec2 cell_uv = fract(UV * float(pixel_count)) * 2.0 - 1.0;
			vec2 snapped = (floor(UV * float(pixel_count)) + 0.5) / float(pixel_count);
			vec4 cell_color = texture(TEXTURE, snapped);
			
			float dist = length(cell_uv);
			float mask = step(dist, dot_size);
			
			color = vec4(cell_color.rgb, cell_color.a * mask);
			break;
	}
	
	COLOR = color;
}
