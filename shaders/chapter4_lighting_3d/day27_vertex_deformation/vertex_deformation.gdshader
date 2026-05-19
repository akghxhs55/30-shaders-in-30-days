shader_type spatial;

uniform sampler2D noise_texture : filter_linear, repeat_enable;

uniform float amplitude : hint_range(0.0, 1.0) = 0.2;
uniform float frequency : hint_range(0.0, 10.0) = 1.0;
uniform float scroll_speed : hint_range(0.0, 2.0) = 0.5;
uniform bool recalculate_normals = true;

void vertex() {
	vec2 uv = UV * frequency + TIME * scroll_speed;
	float noise = textureLod(noise_texture, uv, 0.0).r * 2.0 - 1.0;
	VERTEX += NORMAL * noise * amplitude;

	if (recalculate_normals) {
		float eps = 0.01;
		float n0 = noise;
		float nx = textureLod(noise_texture, uv + vec2(eps, 0.0), 0.0).r * 2.0 - 1.0;
		float ny = textureLod(noise_texture, uv + vec2(0.0, eps), 0.0).r * 2.0 - 1.0;
		NORMAL = normalize(NORMAL + vec3(n0 - nx, n0 - ny, 1.0) * amplitude);
	}
}

void fragment() {
	ALBEDO = vec3(0.063, 0.408, 0.89);
}
