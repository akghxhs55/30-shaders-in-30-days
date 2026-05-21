shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear;

uniform bool use_barrel_distortion = false;
uniform float barrel_strength : hint_range(-0.2, 1.0) = 0.1;

uniform bool use_chromatic_aberration = false;
uniform float chromatic_aberration_strength : hint_range(0.0, 0.1) = 0.005;

uniform bool use_vignette = false;
uniform float vignette_strength : hint_range(0.0, 2.0) = 1.0;
uniform float vignette_radius : hint_range(0.0, 1.0) = 0.5;

uniform bool use_film_grain = false;
uniform float grain_strength : hint_range(0.0, 0.1) = 0.03;

float random(vec2 uv) {
	return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment() {
	vec2 uv = SCREEN_UV;
	
	if (use_barrel_distortion) {
		vec2 centered_uv = uv * 2.0 - 1.0;
		float r2 = dot(centered_uv, centered_uv);
		centered_uv *= (1.0 + r2 * barrel_strength);
		uv = centered_uv * 0.5 + 0.5;
	}
	
	vec3 color = texture(screen_texture, uv).rgb;
	
	if (use_chromatic_aberration) {
		vec2 dir = uv - 0.5;
		float r = texture(screen_texture, uv + dir * chromatic_aberration_strength).r;
		float g = texture(screen_texture, uv).g;
		float b = texture(screen_texture, uv - dir * chromatic_aberration_strength).b;
		color = vec3(r, g, b);
	}
	
	if (use_vignette) {
		vec2 centered_uv = uv - 0.5;
		float dist2 = dot(centered_uv, centered_uv) * 2.0;
		float vignette = 1.0 - smoothstep(vignette_radius, vignette_radius + 0.5, dist2 * vignette_strength);
		color *= vignette;
	}
	
	if (use_film_grain) {
		float grain = random(uv + fract(TIME)) * 2.0 - 1.0;
		color += grain * grain_strength;
	}
	
	COLOR.rgb = color;
}
