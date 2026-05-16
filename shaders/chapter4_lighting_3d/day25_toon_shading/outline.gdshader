shader_type spatial;

render_mode cull_front, unshaded;

uniform float outline_thickness : hint_range(0.0, 0.3) = 0.03;
uniform vec3 outline_color : source_color = vec3(0.0);

void vertex() {
	vec4 clip_pos = PROJECTION_MATRIX * MODELVIEW_MATRIX * vec4(VERTEX, 1.0);
	vec4 clip_normal = PROJECTION_MATRIX * MODELVIEW_MATRIX * vec4(NORMAL, 0.0);

	float aspect = VIEWPORT_SIZE.x / VIEWPORT_SIZE.y;
	vec2 offset = normalize(clip_normal.xy) * outline_thickness * clip_pos.w;
	offset.x /= aspect;

	clip_pos.xy += offset;
	POSITION = clip_pos;
}

void fragment() {
	ALBEDO = outline_color.rgb;
}
