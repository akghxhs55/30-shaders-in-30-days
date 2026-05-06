shader_type canvas_item;

uniform int cell_mode : hint_enum("Closest", "Second", "Border") = 0;
uniform float frequency : hint_range(0.01, 32.0) = 4.0;
uniform float seed : hint_range(0.0, 7.31) = 0.0;

vec2 random2(vec2 p) {
	p = mod(p + seed, 289.0);
	return fract(vec2(
		sin(dot(p, vec2(127.1, 311.7))), 
		sin(dot(p, vec2(269.5, 183.3)))
	) * 43758.5453);
}

void fragment() {
	vec2 p = UV * frequency;
	
	vec2 i = floor(p);
	vec2 f = fract(p);
	
	float f1 = 10.0;
	float f2 = 10.0;
	for (int x = -1; x <= 1; x++) {
		for (int y = -1; y <= 1; y++) {
			vec2 neighbor = i + vec2(float(x), float(y));
			vec2 point = neighbor + random2(neighbor);
			float dist = length(point - p);
			
			if (dist < f1) {
				f2 = f1;
				f1 = dist;
			} else if (dist < f2) {
				f2 = dist;
			}
		}
	}
	
	float value = 0.0;
	switch (cell_mode) {
		case 0: // Closest
			value = f1 * sqrt(2.0);
			break;
		case 1: // Second
			value = f2 / 1.2;
			break;
		case 2: // Border
			value = (f2 - f1) * sqrt(2.0);
			break;
	}
	
	COLOR.rgb = vec3(value);
}
