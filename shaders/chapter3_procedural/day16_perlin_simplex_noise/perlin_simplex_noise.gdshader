shader_type canvas_item;

uniform int noise_type : hint_enum("Perlin", "Simplex") = 0;
uniform float frequency : hint_range(1.0, 64.0) = 4.0;
uniform float seed : hint_range(0.0, 7.31) = 0.0;

vec2 gradient(vec2 p) {
	p = mod(p + seed, 7.31);
	float angle = fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453) * TAU;
	return vec2(cos(angle), sin(angle));
}

float perlin_noise(vec2 uv) {
	vec2 i = floor(uv);
	vec2 f = fract(uv);

	vec2 ga = gradient(i);
	vec2 gb = gradient(i + vec2(1, 0));
	vec2 gc = gradient(i + vec2(0, 1));
	vec2 gd = gradient(i + vec2(1, 1));

	float a = dot(ga, f);
	float b = dot(gb, f - vec2(1, 0));
	float c = dot(gc, f - vec2(0, 1));
	float d = dot(gd, f - vec2(1, 1));
	
	vec2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0); // Cubic interpolation
	
	float n = mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
	return n * 0.5 + 0.5; // Normalize to [0, 1]
}

float simplex_noise(vec2 uv) {
	const float F = 0.3660254037; // (sqrt(3) - 1) / 2
	const float G = 0.2113248654; // (3 - sqrt(3)) / 6
	
	vec2 skewed = uv + dot(uv, vec2(F));
	vec2 i = floor(skewed);
	
	vec2 unskewed = i - dot(i, vec2(G));
	vec2 d0 = uv - unskewed;

	vec2 i1 = d0.x > d0.y ? vec2(1.0, 0.0) : vec2(0.0, 1.0);
	
	vec2 d1 = d0 - i1 + vec2(G);
	vec2 d2 = d0 - vec2(1.0) + 2.0 * vec2(G);
	
	vec3 t = max(0.5 - vec3(dot(d0, d0), dot(d1, d1), dot(d2, d2)), 0.0);
	t = t * t * t * t;
	
	float n = t.x * dot(gradient(i), d0) + t.y * dot(gradient(i + i1), d1) + t.z * dot(gradient(i + vec2(1.0)), d2);
	return n * 35.0 + 0.5; // Normalize to [0, 1]
}

void fragment() {
	float noise_value = 0.0;
	switch (noise_type) {
		case 0: // Perlin
			noise_value = perlin_noise(UV * frequency);
			break;

		case 1: // Simplex
			noise_value = simplex_noise(UV * frequency);
			break;
	}

	COLOR.rgb = vec3(noise_value);
}
