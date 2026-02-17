shader_type canvas_item;

// Edge Mode
uniform int edge_mode : hint_enum("Hard", "Soft", "Auto AA") = 0;
uniform float softness : hint_range(0.0, 0.2) = 0.05; // Soft

// Glow
uniform bool glow_enabled = false;
uniform float glow_falloff : hint_range(0.0, 32.0) = 16.0;
uniform vec4 glow_color : source_color = vec4(0.0, 0.655, 1.0, 1.0);

// Shadow
uniform bool shadow_enabled = false;
uniform vec2 shadow_offset = vec2(0.02, 0.02);
uniform float shadow_softness : hint_range(0.0, 0.2) = 0.05;
uniform vec4 shadow_color : source_color = vec4(0.0, 0.0, 0.0, 0.5);

// Circle
uniform float radius : hint_range(0.0, 0.5) = 0.3;

// Colors
uniform vec4 shape_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);
uniform vec4 background_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);

void fragment() {
	vec2 uv = UV - vec2(0.5);
	float sdf = length(uv) - radius;
	
	float mask = 1.0;
	switch (edge_mode) {
		case 0: // Hard
			mask = step(0.0, -sdf);
			break;
		case 1: // Soft
			mask = smoothstep(softness, -softness, sdf);
			break;
		case 2: // Auto AA
			float pixel_size = fwidth(sdf);
			mask = smoothstep(pixel_size, -pixel_size, sdf);
			break;
	}
	
	float glow = 0.0;
	if (glow_enabled) {
		glow = clamp(exp(-sdf * glow_falloff), 0.0, 1.0);
	}
	
	float shadow_mask = 0.0;
	if (shadow_enabled) {
		float shadow_sdf = length(uv - shadow_offset) - radius;
		shadow_mask = smoothstep(shadow_softness, -shadow_softness, shadow_sdf);
	}
	
	vec4 color = background_color;
	
	if (shadow_enabled) {
		color = mix(color, shadow_color, shadow_mask);
	}
	
	if (glow_enabled) {
		color = mix(color, glow_color, glow);
	}
	
	color = mix(color, shape_color, mask);
	
	COLOR = color;
}
