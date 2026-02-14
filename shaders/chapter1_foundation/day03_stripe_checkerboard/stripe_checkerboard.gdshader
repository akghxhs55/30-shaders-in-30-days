shader_type canvas_item;

uniform int pattern : hint_enum(
	"Stripes",
	"Checkerboard",
	"Grid",
	"Dots"
) = 0;

uniform float frequency : hint_range(1.0, 32.0) = 8.0;

// Stripes Parameters
uniform int direction : hint_enum(
	"Horizontal",
	"Vertical",
	"Diagonal"
) = 0;

uniform float duty_cycle : hint_range(0.0, 1.0) = 0.5;

// Grid Parameters
uniform float line_thickness : hint_range(0.0, 1.0) = 0.1;

// Dots Parameters
uniform float dot_radius : hint_range(0.0, 0.5) = 0.2;

// Color
uniform vec4 color_a : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform vec4 color_b : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
	float pattern_value = 0.0;
	
    switch (pattern) {
		case 0: // Stripes
			float coord = 0.0;
			switch (direction) {
				case 0: // Horizontal
					coord = UV.y;
					break;
				case 1: // Vertical
					coord = UV.x;
					break;
				case 2: // Diagonal
					coord = (UV.x + UV.y) * 0.5;
					break;
			}
			pattern_value = step(duty_cycle, fract(coord * frequency));
			break;
			
		case 1: // Checkerboard
			float cx = floor(UV.x * frequency);
			float cy = floor(UV.y * frequency);
			pattern_value = mod(cx + cy, 2.0);
			break;
		
		case 2: // Grid
			vec2 cell = fract(UV * frequency);
			float line_x = step(cell.x, line_thickness * 0.5) + step(1.0 - line_thickness * 0.5, cell.x);
			float line_y = step(cell.y, line_thickness * 0.5) + step(1.0 - line_thickness * 0.5, cell.y);
			pattern_value = max(line_x, line_y);
			break;
		
		case 3: // Dots
			vec2 cell = fract(UV * frequency);
			float dist = distance(cell, vec2(0.5));
			pattern_value = step(dist, dot_radius);
			break;
	}
	
	COLOR = mix(color_a, color_b, pattern_value);
}
