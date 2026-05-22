shader_type spatial;
render_mode ambient_light_disabled;

uniform vec3 color_bottom1 : source_color = vec3(0.008, 0.4, 0.702);
uniform vec3 color_middle1 : source_color = vec3(0.537, 0.702, 0.929);
uniform vec3 color_top1 : source_color = vec3(0.875, 0.906, 1.0);
uniform vec3 color_bottom2 : source_color = vec3(0.0, 0.157, 0.31);
uniform vec3 color_middle2 : source_color = vec3(0.02, 0.133, 0.341);
uniform vec3 color_top2 : source_color = vec3(0.11, 0.208, 0.38);

uniform float specular_power : hint_range(1.0, 128.0) = 32.0;
uniform float specular_intensity : hint_range(0.0, 2.0) = 0.15;
uniform float rim_power : hint_range(0.1, 8.0) = 6.0;
uniform float rim_intensity : hint_range(0.0, 2.0) = 0.5;
uniform vec4 rim_color : source_color = vec4(0.522, 0.753, 0.871, 1.0);

uniform sampler3D noise_texture : repeat_enable;

varying vec3 local_pos;

float noise(vec3 p) {
	return texture(noise_texture, p).r;
}

float fbm(vec3 p) {
	const int octaves = 6;
	const float lacunarity = 2.0;
	const float gain = 0.5;

	float value = 0.0;
	float amplitude = 1.0 - gain;
	float frequency = 1.0;
	for (int i = 0; i < octaves; i++) {
		value += amplitude * noise(p * frequency);
		frequency *= lacunarity;
		amplitude *= gain;
	}

	return value;
}

float cloud(vec3 p, float time_scale, float warp) {
	vec3 q = vec3(
		fbm(p + vec3(TIME * time_scale * 2.0, 0.0, 0.25)),
		fbm(p + vec3(1.3, 1.7, 0.0)),
		fbm(p + vec3(2.7, 0.5, 0.8))
	);
	vec3 r = vec3(
		fbm(p + q + vec3(TIME * time_scale * 0.5, 0.0, 0.0)),
		fbm(p + q + vec3(4.1, 2.3, 0.0)),
		fbm(p + q + vec3(0.0, 3.7, 1.5))
	);
	return fbm(p + warp * r + vec3(0.0, 0.0, TIME * time_scale * 0.1));
}

vec3 apply_color(float v, vec3 col_bot, vec3 col_mid, vec3 col_top) {
	float x = v * 2.0 - 1.0;
	vec3 color = mix(col_mid, col_top, clamp(x, 0.0, 1.0));
	return mix(color, col_bot, clamp(-x, 0.0, 1.0));
}

void vertex() {
	local_pos = VERTEX;
}

void fragment() {
	vec3 p = local_pos;
	
	float v1 = cloud(p * 0.6, 0.01, 0.4);
	float v2 = cloud(p * 0.35, 0.004, 0.5);

	vec3 color1 = apply_color(v1, color_bottom1, color_middle1, color_top1);
	vec3 color2 = apply_color(v2, color_bottom2, color_middle2, color_top2);

	vec3 color = mix(color2, color1, smoothstep(0.1, 0.8, v1));

	ALBEDO = color;
}

void light() {
	float ndotl = max(0.0, dot(NORMAL, LIGHT));
    float sharp_diffuse = pow(ndotl, 3.0);
    DIFFUSE_LIGHT += sharp_diffuse * LIGHT_COLOR;

	vec3 h = normalize(LIGHT + VIEW);
	float spec = pow(max(0.0, dot(NORMAL, h)), specular_power);
	SPECULAR_LIGHT += spec * LIGHT_COLOR * specular_intensity;

	float backlight = 1.0 - max(0.0, dot(NORMAL, LIGHT));
	float rim = pow(1.0 - max(0.0, dot(NORMAL, VIEW)), rim_power);
	DIFFUSE_LIGHT += rim_color.rgb * backlight * rim * rim_intensity;
}
