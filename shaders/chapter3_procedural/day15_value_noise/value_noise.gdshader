shader_type canvas_item;

uniform int interpolation : hint_enum("Linear", "Smoothstep", "Cubic") = 1;
uniform float frequency : hint_range(1.0, 64.0) = 4.0;
uniform float seed : hint_range(0.0, 7.31) = 0.0;

float random(vec2 p) {
	p = mod(p + seed, 7.31);
	return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

float value_noise(vec2 uv) {
	vec2 i = floor(uv);
	vec2 f = fract(uv);

	float a = random(i);
	float b = random(i + vec2(1, 0));
	float c = random(i + vec2(0, 1));
	float d = random(i + vec2(1, 1));

	vec2 u = vec2(0.0);
	switch (interpolation) {
		case 0: // Linear
			u = f;
			break;

		case 1: // Smoothstep
			u = smoothstep(vec2(0.0), vec2(1.0), f);
			break;

		case 2: // Cubic
			u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
			break;
	}

	return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

void fragment() {
	COLOR.rgb = vec3(value_noise(UV * frequency));
}
