shader_type canvas_item;

uniform float speed : hint_range(0.0, 10.0) = 1.0;
uniform float frequency : hint_range(1.0, 16.0) = 4.0;
uniform float warp_strength : hint_range(0.0, 5.0) = 2.0;
uniform float intensity : hint_range(0.5, 5.0) = 2.0;
uniform float threshold : hint_range(0.0, 1.0) = 0.2;

uniform vec4 color_hot : source_color = vec4(0.922, 0.922, 0.424, 1.0);
uniform vec4 color_mid : source_color = vec4(1.0, 0.5, 0.0, 1.0);
uniform vec4 color_cool : source_color = vec4(1.0, 0.0, 0.0, 1.0);

vec2 gradient(vec2 p) {
	p = mod(p, 7.31);
	float angle = fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453) * TAU;
	return vec2(cos(angle), sin(angle));
}

float perlin_noise(vec2 uv) {
	vec2 i = floor(uv);
	vec2 f = fract(uv);

	vec2 ga = gradient(i);
	vec2 gb = gradient(i + vec2(1, 0));
	vec2 gc = gradient(i + vec2(0, 1));
	vec2 gd = gradient(i + vec2(1, 1));

	float a = dot(ga, f);
	float b = dot(gb, f - vec2(1, 0));
	float c = dot(gc, f - vec2(0, 1));
	float d = dot(gd, f - vec2(1, 1));

	vec2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0); // Cubic interpolation

	float n = mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
	return n;
}

float fbm(vec2 uv) {
	float value = 0.0;
	float amplitude = 0.5;
	float frequency_accum = 1.0;

	for (int i = 0; i < 4; i++) {
		value += perlin_noise(uv * frequency_accum) * amplitude;
		frequency_accum *= 2.0;
		amplitude *= 0.5;
	}

	return value;
}

void fragment() {
	vec2 scrolled = UV + vec2(0.0, TIME * speed);
	
	vec2 warp = vec2(
		fbm(scrolled * frequency), 
		fbm(scrolled * frequency + vec2(5.2, 1.3))
	);
	
	float density = fbm(scrolled * frequency + warp * warp_strength);

	float shape = UV.y * UV.y;
	float fire = clamp(density * intensity + shape - threshold, 0.0, 1.0);
	
	vec3 color = mix(color_cool.rgb, color_mid.rgb, fire);
	color = mix(color, color_hot.rgb, fire * fire);
	
	COLOR = vec4(color, fire);
}
