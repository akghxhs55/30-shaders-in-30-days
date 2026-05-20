shader_type spatial;

render_mode blend_mix, cull_disabled;

uniform vec4 crystal_color : source_color = vec4(0.282, 0.51, 0.718, 1.0);
uniform float fresnel_power : hint_range(0.1, 10.0) = 5.0;
uniform float base_alpha : hint_range(0.0, 1.0) = 0.4;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear;
uniform float refraction_strength : hint_range(0.0, 0.1) = 0.02;

uniform vec4 rim_color : source_color = vec4(0.212, 0.408, 0.506, 1.0);
uniform float rim_power : hint_range(0.1, 10.0) = 3.0;
uniform float rim_intensity : hint_range(0.0, 2.0) = 1.0;

uniform float iridescence_intensity : hint_range(0.0, 1.0) = 0.1;

uniform float specular_strength : hint_range(0.0, 1.0) = 1.0;
uniform float roughness : hint_range(0.0, 1.0) = 0.05;

uniform vec4 inner_color : source_color = vec4(0.18, 0.553, 0.902, 1.0);

void light() {
	float ndotl = max(0.0, dot(NORMAL, LIGHT));
	float sharp = pow(ndotl, 4.0);
	DIFFUSE_LIGHT += sharp * LIGHT_COLOR * 2.0;
}

void fragment() {
	if (!FRONT_FACING) {
		ALBEDO = inner_color.rgb;
		ALPHA = 0.3;
		EMISSION = vec3(0.0);
	}
	else {
		vec2 refraction_offset = NORMAL.xy * refraction_strength;
		float r = texture(screen_texture, SCREEN_UV + refraction_offset * 1.0).r;
		float g = texture(screen_texture, SCREEN_UV + refraction_offset * 1.2).g;
		float b = texture(screen_texture, SCREEN_UV + refraction_offset * 1.4).b;
		vec3 refraction = vec3(r, g, b);
		
		float fresnel = pow(1.0 - max(0.0, dot(NORMAL, VIEW)), fresnel_power);
		ALBEDO = mix(refraction, crystal_color.rgb, fresnel);
		ALPHA = base_alpha + fresnel * (1.0 - base_alpha);

		float rim = pow(1.0 - max(0.0, dot(NORMAL, VIEW)), rim_power);
		EMISSION = rim_color.rgb * rim * rim_intensity;

		float ndotv = dot(NORMAL, VIEW);
		vec3 iridescence = vec3(
			sin(ndotv * 6.0),
			sin(ndotv * 6.0 + 2.0),
			sin(ndotv * 6.0 + 4.0)
		) * 0.5 + 0.5;
		EMISSION += iridescence * iridescence_intensity;

		SPECULAR = specular_strength;
		ROUGHNESS = roughness;
	}

	NORMAL = normalize(cross(dFdx(VERTEX), dFdy(VERTEX)));
}
