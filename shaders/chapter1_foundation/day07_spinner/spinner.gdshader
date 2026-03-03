shader_type canvas_item;

uniform int mode : hint_enum("Arc Spinner", "Dots Spinner", "Pulsing Ring");

uniform float speed : hint_range(0.0, 5.0) = 1.0;

// Arc Spinner Parameters
const float arc_radius = 0.3;
const float arc_thickness = 0.05;

// Dots Spinner Parameters
const int dot_count = 12;
const float dot_distance = 0.3;
const float space_angle = TAU / float(dot_count);
const float dot_min_radius = 0.01;
const float dot_max_radius = 0.05;
const float dot_inward_offset = 0.05;

// Pulsing Ring Parameters
const float ring_duty_cycle = 0.6;
const float ring_min_radius = 0.05;
const float ring_max_radius = 0.3;
const float ring_thickness = 0.03;

float sdf_arc(vec2 p, float radius, float thickness, float half_arc_angle) {
	float angle = atan(p.y, p.x);
	
	if (abs(angle) < half_arc_angle) {
		return abs(length(p) - radius) - thickness;
	} else {
		vec2 edge_point = vec2(cos(half_arc_angle), sign(p.y) * sin(half_arc_angle)) * radius;
		return length(p - edge_point) - thickness;
	}
}

float calculate_ring_mask(vec2 p, float t) {
	float r = mix(ring_min_radius, ring_max_radius, 1.0 - pow(1.0 - t, 2.0));
	float sdf = abs(length(p) - r) - ring_thickness;
	float alpha = smoothstep(0.0, 0.3, t) * (1.0 - t);

	return smoothstep(fwidth(sdf), -fwidth(sdf), sdf) * alpha;
}

void fragment() {
	vec2 uv = UV - vec2(0.5);
	
	vec4 color = vec4(1.0);
	switch (mode) {
		case 0: // Arc Spinner
			float t = TIME * speed * 5.0;
			
			float angle = -t;
			float c = cos(angle), s = sin(angle);
			vec2 rotated_uv = mat2(vec2(c, s), vec2(-s, c)) * uv;
			
			float half_arc_angle = mix(PI * 0.2, PI * 0.8, 0.5 + 0.5 * sin(t * 0.5));
			float sdf = sdf_arc(rotated_uv, arc_radius, arc_thickness, half_arc_angle);
			
			color.a = smoothstep(fwidth(sdf), -fwidth(sdf), sdf);
			break;
			
		case 1: // Dots Spinner
			float t = TIME * speed * 5.0;
			
			float pixel_angle = atan(uv.y, uv.x);
			float index = mod(round(pixel_angle / space_angle), float(dot_count));
			float active_index = mod(t / space_angle, float(dot_count));

			float diff = mod(active_index - index + float(dot_count), float(dot_count));
			float intensity = 1.0 - diff / float(dot_count);
			float dot_r = mix(dot_min_radius, dot_max_radius, intensity);
			
			float dot_angle = index * space_angle;
			float dot_dist = dot_distance + mix(-dot_inward_offset, 0.0, intensity);
			vec2 dot_pos = vec2(cos(dot_angle), sin(dot_angle)) * dot_dist;
			
			float sdf = length(uv - dot_pos) - dot_r;
			
			color.a = smoothstep(0.003, -0.003, sdf) * intensity;
			break;
			
		case 2: // Pulsing Ring
			float t = TIME * speed * 0.5;
			
			float t0 = min(fract(t) / ring_duty_cycle, 1.0);
			float t1 = min(fract(t + 0.5) / ring_duty_cycle, 1.0);
			
			float mask0 = calculate_ring_mask(uv, t0);
			float mask1 = calculate_ring_mask(uv, t1);

			color.a = mask0 + mask1;
			break;
	}
	
	COLOR = color;
}
