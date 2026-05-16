shader_type spatial;

uniform int diffuse_steps : hint_range(1, 10) = 4;
uniform float specular_threshold : hint_range(0.0, 1.0) = 0.5;

void light() {
	float ndotl = max(0.0, dot(NORMAL, LIGHT));
	float toon = floor(ndotl * float(diffuse_steps)) / float(diffuse_steps);
	vec3 diffuse = toon * LIGHT_COLOR;
	
	DIFFUSE_LIGHT += diffuse;
	
	vec3 h = normalize(LIGHT + VIEW);
	float ndoth = max(0.0, dot(NORMAL, h));
	float spec = pow(ndoth, 16.0);
	float toon_spec = step(specular_threshold, spec);
	vec3 specular = toon_spec * LIGHT_COLOR;
	
	SPECULAR_LIGHT += specular;
}

void fragment() {
	ALBEDO = vec3(0.063, 0.408, 0.89);
}
