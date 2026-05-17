shader_type spatial;

uniform sampler2D environment_map : filter_linear, hint_default_black;

void fragment() {
	vec3 reflection = reflect(-VIEW, NORMAL);
	vec3 world_reflection = normalize((INV_VIEW_MATRIX * vec4(reflection, 0.0)).xyz);
	
	vec2 panorama_uv = vec2(
	atan(world_reflection.z, world_reflection.x) / TAU + 0.5,
		asin(world_reflection.y) / PI + 0.5
	);
	
	vec3 env_color = texture(environment_map, panorama_uv).rgb;
	
	float ndotv = max(0.0, dot(NORMAL, VIEW));
	float fresnel = pow(1.0 - ndotv, 5.0);
	
	ALBEDO = mix(vec3(0.6), env_color, fresnel);
}
