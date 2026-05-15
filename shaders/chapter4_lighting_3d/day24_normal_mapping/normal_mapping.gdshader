shader_type spatial;

uniform bool use_albedo_map = true;
uniform bool use_normal_map = true;

uniform sampler2D albedo_map : source_color, hint_default_white;
uniform sampler2D normal_map : hint_normal;

void fragment() {
	if (use_albedo_map) {
		ALBEDO = texture(albedo_map, UV).rgb;
	}
	
	if (use_normal_map) {
		vec3 n = texture(normal_map, UV).rgb * 2.0 - 1.0;
		mat3 TBN = mat3(TANGENT, BINORMAL, -NORMAL);
		NORMAL = normalize(TBN * n);
		// NORMAL_MAP = texture(normal_map, UV).rgb;
	}
}
