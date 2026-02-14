shader_type canvas_item;

uniform int mode : hint_enum(
	"RGB Visualization",
	"Horizontal Gradient (U Only)",
	"Vertical Gradient (V Only)",
	"Centered UV",
	"Tiling",
	"Radial"
) = 0;

uniform int tile_repeats : hint_range(1, 16) = 4;

void fragment() {
	switch (mode) {
		case 0: // RGB Visualization
			COLOR = vec4(UV, 0.0, 1.0);
			break;
		
		case 1: // Horizontal Gradient (U Only)
			COLOR = vec4(vec3(UV.x), 1.0);
			break;
		
		case 2: // Vertical Gradient (V Only)
			COLOR = vec4(vec3(UV.y), 1.0);
			break;
		
		case 3: // Centered UV
			COLOR = vec4(UV - vec2(0.5), 0.0, 1.0);
			break;
		
		case 4: // Tiling
			COLOR = vec4(fract(UV * float(tile_repeats)), 0.0, 1.0);
			break;
		
		case 5: // Radial
			float dist = length(UV - vec2(0.5));
			COLOR = vec4(vec3(dist), 1.0);
			break;
	}
}
