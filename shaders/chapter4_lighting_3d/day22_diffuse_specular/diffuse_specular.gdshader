shader_type spatial;

uniform int shading_mode : hint_enum("Phong", "Blinn-Phong") = 0;
uniform float diffuse_intensity : hint_range(0.0, 1.0) = 1.0;
uniform float specular_intensity : hint_range(0.0, 1.0) = 1.0;

uniform float specular_power : hint_range(0.0, 128.0) = 32.0;

uniform vec3 color : source_color = vec3(0.063, 0.408, 0.89);

void light() {
    float ndotl = max(0.0, dot(NORMAL, LIGHT));
	vec3 diffuse = ndotl * LIGHT_COLOR;
	
	float spec = 0.0;
	switch (shading_mode) {
		case 0: { // Phong 
			vec3 reflect_dir = reflect(-LIGHT, NORMAL);
			spec = pow(max(0.0, dot(reflect_dir, VIEW)), specular_power);
			break;
		}
		case 1: { // Blinn-Phong
			vec3 half_vector = normalize(LIGHT + VIEW);
			spec = pow(max(0.0, dot(NORMAL, half_vector)), specular_power);
			break;
		}
	}
	vec3 specular = spec * LIGHT_COLOR;
	
	DIFFUSE_LIGHT += diffuse * diffuse_intensity;
	SPECULAR_LIGHT += specular * specular_intensity;
}

void fragment() {
    ALBEDO = color.rgb;
}
