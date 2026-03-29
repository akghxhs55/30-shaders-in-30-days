shader_type canvas_item;

// Pixelation parameters
uniform float pixelation_size : hint_range(1.0, 64.0) = 1.0;

// Frozen parameters
uniform float frozen_progress : hint_range(0.0, 1.0) = 0.0;

// Burned parameters
uniform float burned_progress : hint_range(0.0, 1.0) = 0.0;

// Flash parameters
uniform float resurrect_flash_amount : hint_range(0.0, 1.0) = 0.0;
uniform float hit_flash_amount : hint_range(0.0, 1.0) = 0.0;

// Dissolve parameters
uniform sampler2D noise_texture;
uniform float threshold : hint_range(0.0, 1.0) = 0.0;

vec2 apply_burned_distortion(vec2 uv, vec2 pixel_size) {
	float heat_x = sin(uv.y * 20.0 + TIME * 5.0) * burned_progress * 2.0 * pixel_size.x;
	float heat_y = sin(uv.x * 15.0 + TIME * 4.0) * burned_progress * 1.0 * pixel_size.y;
	
	return uv + vec2(heat_x, heat_y);
}

vec2 apply_pixelation(vec2 uv, vec2 texture_size) {
	vec2 pixelated_uv = (floor(uv * texture_size / pixelation_size) * pixelation_size) / texture_size;
	return pixelated_uv;
}

vec4 apply_flash(vec4 color) {
	const vec4 resurrect_color = vec4(0.0, 0.8, 0.2, 1.0);
	const vec4 hit_color = vec4(1.0, 1.0, 1.0, 1.0);
	
	color.rgb = mix(color, resurrect_color, resurrect_flash_amount).rgb;
	color.rgb = mix(color, hit_color, hit_flash_amount).rgb;
	
	return color;
}

vec4 apply_frozen(vec2 uv, vec4 color) {
	// Tint
	const vec3 frozen_tint = vec3(0.5, 0.8, 1.0);

	float luminance = dot(color.rgb, vec3(0.299, 0.587, 0.114));
	vec3 frozen_color = frozen_tint * luminance * 2.0;
	
	float frozen_value = smoothstep(frozen_progress, frozen_progress - 0.3, 1.0 - uv.y);
	
	color.rgb = mix(color.rgb, frozen_color, frozen_value);
	
	// Shimmer
	float cycle = fract(TIME / 3.0);
	float t = cycle / 0.15;
	
	float diagonal = (uv.x + uv.y) / 2.0;
	float band = t - diagonal;

	float shimmer = smoothstep(0.0, 0.05, band) * smoothstep(0.1, 0.05, band) * step(t, 1.0);
	
	color.rgb += shimmer * frozen_value;
	
	return color;
}

vec4 apply_burned(vec2 uv, vec4 color, sampler2D texture, vec2 pixel_size) {
	// Tint
	const vec3 burned_tint = vec3(1.0, 0.5, 0.0);
	
	float luminance = dot(color.rgb, vec3(0.299, 0.587, 0.114));
	vec3 burned_color = burned_tint * luminance * 2.0;
	
	color.rgb = mix(color.rgb, burned_color, burned_progress);
	
	// Glow
	float glow_strength = 0.0;
	for (int r = 0; r < 5; r++) {
		float radius = pow(2.0, float(r)); // 1, 2, 4, 8, 16
		float ring_alpha = 0.0;
		for (int i = 0; i < 8; i++) {
			float angle = float(i) / 8.0 * TAU;
			vec2 offset = vec2(cos(angle), sin(angle)) * pixel_size * radius;
			ring_alpha = max(ring_alpha, texture(texture, uv + offset).a);
		}
		glow_strength = max(glow_strength, ring_alpha * (1.0 - float(r) / 5.0));
	}

	float alpha_mask = step(0.5, texture(texture, uv).a);
	float glow = (1.0 - alpha_mask) * glow_strength;
	color.rgb += vec3(1.0, 0.4, 0.0) * glow * burned_progress;
	color.a = max(color.a, glow * burned_progress);
	
	return color;
}

vec4 apply_dissolve(vec2 uv, vec4 color) {
	float map_value = texture(noise_texture, uv).r;
	float alpha = step(threshold, map_value);
	return vec4(color.rgb, color.a * alpha);
}

void fragment() {
	vec2 uv = apply_burned_distortion(UV, TEXTURE_PIXEL_SIZE);
	uv = apply_pixelation(uv, 1.0 / TEXTURE_PIXEL_SIZE);
	
	vec4 color = texture(TEXTURE, uv);
	color = apply_frozen(uv, color);
	color = apply_burned(uv, color, TEXTURE, TEXTURE_PIXEL_SIZE);
	color = apply_flash(color);
	color = apply_dissolve(uv, color);
	
	COLOR = color;
}
