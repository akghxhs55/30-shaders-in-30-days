shader_type spatial;

uniform int rim_type : hint_enum("View Only", "Backlit") = 0;

uniform vec4 rim_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float rim_power : hint_range(0.0, 16.0) = 4.0;
uniform float rim_intensity : hint_range(0.0, 1.0) = 1.0;

void fragment() {
	ALBEDO = vec3(0.063, 0.408, 0.89);
	
	if (rim_type == 0) {
		float ndotv = max(0.0, dot(NORMAL, VIEW));
		float rim = pow(1.0 - ndotv, rim_power);
		EMISSION = rim_color.rgb * rim * rim_intensity;
	}
}

void light() {
	float ndotl = max(0.0, dot(NORMAL, LIGHT));
	vec3 diffuse = ndotl * LIGHT_COLOR;
	DIFFUSE_LIGHT += diffuse;

	if (rim_type == 1) {
		float backlight = 1.0 - ndotl;
		float ndotv = max(0.0, dot(NORMAL, VIEW));
		float rim = backlight * pow(1.0 - ndotv, rim_power);
		DIFFUSE_LIGHT += rim_color.rgb * rim * rim_intensity;
	}
}
