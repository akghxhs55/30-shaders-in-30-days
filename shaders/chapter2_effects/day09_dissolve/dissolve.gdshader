shader_type canvas_item;

uniform int dissolve_mode : hint_enum("Noise", "Directional", "Radial");

// Noise parameters
uniform sampler2D noise_texture;

// Directional parameters
uniform float angle : hint_range(0.0, 6.283) = 0.0;

uniform int edge_mode : hint_enum("Hard", "Soft", "Burn");

// Soft parameters
uniform float softness : hint_range(0.0, 1.0) = 0.5;

// Burn parameters
uniform float burn_width : hint_range(0.0, 1.0) = 0.1;

uniform float threshold : hint_range(0.0, 1.0) = 0.0;

void fragment() {
	vec4 original = texture(TEXTURE, UV);
	
	float map_value = 0.0;
	switch (dissolve_mode) {
		case 0: // Noise
			map_value = texture(noise_texture, UV).r;
			break;
			
		case 1: // Directional
			map_value = dot(UV - 0.5, vec2(cos(angle), sin(angle))) + 0.5;
			break;
			
		case 2: // Radial
			map_value = length(UV - 0.5) * sqrt(2.0);
			break;
	}
	
	float alpha = 1.0;
	switch (edge_mode) {
		case 0: // Hard
			alpha = step(threshold, map_value);
			break;
			
		case 1: // Soft
			float t = threshold * (1.0 + softness);
			alpha = smoothstep(t - softness, t, map_value);
			break;
			
		case 2: // Burn
			float t = mix(-burn_width * 2.0, 1.0, threshold);
			alpha = smoothstep(t, t + burn_width, map_value);
			float fill = smoothstep(t + burn_width, t + burn_width * 2.0, map_value);
			original.rgb = mix(vec3(1.0), original.rgb, fill);
			break;
	}
	
	COLOR.rgb = original.rgb;
	COLOR.a = original.a * alpha;
}
