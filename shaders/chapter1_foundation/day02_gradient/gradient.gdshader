shader_type canvas_item;

uniform int shape : hint_enum(
	"Linear",
	"Radial",
	"Angular"
) = 0;

uniform int interpolation : hint_enum(
	"Raw",
	"Smoothstep",
	"Step",
	"Custom"
) = 0;

uniform int color_mapping : hint_enum(
	"TwoColor",
	"MultiStop"
) = 0;

// Interpolation Parameters
uniform float edge0 : hint_range(0.0, 1.0) = 0.3;
uniform float edge1 : hint_range(0.0, 1.0) = 0.7;
uniform float threshold : hint_range(0.0, 1.0) = 0.5;
uniform sampler2D custom_curve;

// TwoColor Parameters
uniform vec4 color_a : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform vec4 color_b : source_color = vec4(1.0, 1.0, 1.0, 1.0);

// MultiStop Parameters
uniform int color_count : hint_range(2, 8) = 3;
uniform vec4 stop_colors[8] : source_color;

void fragment() {
	// Shape : Calculate t
	float t = 0.0;
	switch (shape) {
		case 0: // Linear
			t = UV.x;
			break;
		case 1: // Radial
			float dist = distance(UV, vec2(0.5));
			t = clamp(dist * 2.0, 0.0, 1.0);
			break;
		case 2: // Angular
			vec2 dir = UV - vec2(0.5);
			float angle = atan(dir.y, dir.x);
			t = (angle + PI) / TAU;
			break;
	}
	
	
	// Interpolation : Calculate f
	float f = 0.0;
	switch (interpolation) {
		case 0: // Raw
			f = t;
			break;
		case 1: // Smoothstep
			f = smoothstep(edge0, edge1, t);
			break;
		case 2: // Step
			f = step(threshold, t);
			break;
		case 3: // Custom
			f = texture(custom_curve, vec2(t, 0.0)).r;
			break;
	}
	
	// Color Mapping : Calculate final color
	vec4 final_color = vec4(0.0);
	switch (color_mapping) {
		case 0: // TwoColor
			final_color = mix(color_a, color_b, f);
			break;
		case 1: // MultiStop
			float segment = f * float(color_count - 1);
			int segment_index = min(int(floor(segment)), color_count - 2);
			float local_t = segment - float(segment_index);
			final_color = mix(stop_colors[segment_index], stop_colors[segment_index + 1], local_t);
			break;
	}
	
	COLOR = final_color;
}
