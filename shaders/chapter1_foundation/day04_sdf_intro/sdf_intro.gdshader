shader_type canvas_item;


// Shape A
uniform int shape_a : hint_enum("Circle", "Rectangle", "Rounded Rectangle", "Ring", "None") = 0;
uniform vec2 position_a = vec2(-0.15, 0.0);
uniform float radius_a : hint_range(0.0, 0.5) = 0.2;           // Circle, Ring
uniform vec2 size_a = vec2(0.3, 0.2);                          // Rectangle, RoundedRect
uniform float corner_radius_a : hint_range(0.0, 0.2) = 0.05;   // RoundedRect
uniform float ring_thickness_a : hint_range(0.0, 0.2) = 0.05;  // Ring

// Shape B
uniform int shape_b : hint_enum("Circle", "Rectangle", "Rounded Rectangle", "Ring", "None") = 0;
uniform vec2 position_b = vec2(0.15, 0.0);
uniform float radius_b : hint_range(0.0, 0.5) = 0.2;           // Circle, Ring
uniform vec2 size_b = vec2(0.3, 0.2);                          // Rectangle, RoundedRect
uniform float corner_radius_b : hint_range(0.0, 0.2) = 0.05;   // RoundedRect
uniform float ring_thickness_b : hint_range(0.0, 0.2) = 0.05;  // Ring

// Operation
uniform int operation : hint_enum("Union", "Intersection", "Subtraction") = 0;

// Visualization
uniform int visualization : hint_enum("Solid", "Distance", "Stepped") = 0;
uniform float band_frequency : hint_range(1.0, 32.0) = 16.0;   // Stepped

// Colors
uniform vec4 shape_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform vec4 background_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);

float sdf_circle(vec2 p, float radius) {
	return length(p) - radius;
}

float sdf_rectangle(vec2 p, vec2 size) {
	vec2 d = abs(p) - size;
	return length(max(d, vec2(0.0))) + min(max(d.x, d.y), 0.0);
}

float sdf_rounded_rectangle(vec2 p, vec2 size, float corner_radius) {
	return sdf_rectangle(p, size - vec2(corner_radius)) - corner_radius;
}

float sdf_ring(vec2 p, float radius, float thickness) {
	return abs(sdf_circle(p, radius)) - thickness;
}

float calculate_sdf(int shape_type, vec2 p, float radius, vec2 size, float corner_radius, float ring_thickness) {
	float result = 1.0;
	
	switch (shape_type) {
		case 0: // Circle
			result = sdf_circle(p, radius);
			break;
		case 1: // Rectangle
			result = sdf_rectangle(p, size);
			break;
		case 2: // Rounded Rectangle
			result = sdf_rounded_rectangle(p, size, corner_radius);
			break;
		case 3: // Ring
			result = sdf_ring(p, radius, ring_thickness);
			break;
	}

	return result;
}

float combine(float sdf_a, float sdf_b) {
	float result = 1.0;
	
	switch (operation) {
		case 0: // Union
			result = min(sdf_a, sdf_b);
			break;
		case 1: // Intersection
			result = max(sdf_a, sdf_b);
			break;
		case 2: // Subtraction
			result = max(sdf_a, -sdf_b);
			break;
	}

	return result;
}

vec4 visualize(float sdf) {
	vec4 result = background_color;
	
	switch (visualization) {
		case 0: // Solid
			result = sdf < 0.0 ? shape_color : background_color;
			break;
		case 1: // Distance
			float intensity = clamp(abs(sdf) * 2.0, 0.0, 1.0);
			if (sdf < 0.0) {
				result = mix(shape_color, background_color, intensity);
			} else {
				result = mix(background_color, shape_color, intensity);
			}
			break;
		case 2: // Stepped
			float bands = abs(floor(sdf * band_frequency));
			result = mix(background_color, shape_color, mod(bands, 2.0));
			break;
	}

	return result;
}

void fragment() {
	vec2 uv = UV - vec2(0.5);

	float sdf_a = calculate_sdf(shape_a, uv - position_a, radius_a, size_a, corner_radius_a, ring_thickness_a);
	float sdf_b = calculate_sdf(shape_b, uv - position_b, radius_b, size_b, corner_radius_b, ring_thickness_b);

	float sdf = combine(sdf_a, sdf_b);

	COLOR = visualize(sdf);
}
