shader_type canvas_item;

uniform float wave_speed : hint_range(0.0, 10.0) = 1.5;
uniform float frequency : hint_range(0.5, 32.0) = 8.0;

uniform vec3 color_deep : source_color = vec3(0.118, 0.545, 0.651);
uniform vec3 color_caustic : source_color = vec3(0.357, 0.769, 0.871);
uniform float caustic_brightness : hint_range(0.0, 5.0) = 0.4;

vec2 random2(vec2 p) {
	p = mod(p, 289.0);
	return fract(vec2(
		sin(dot(p, vec2(127.1, 311.7))),
		sin(dot(p, vec2(269.5, 183.3)))
	) * 43758.5453);
}

vec2 gradient(vec2 p) {
	p = mod(p, 7.31);
	float angle = fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453) * TAU;
	return vec2(cos(angle), sin(angle));
}

float range(float value, float min_val, float max_val, float bias) {
	float remapped = smoothstep(min_val, max_val, value);
	return pow(remapped, log(0.5) / log(bias));
}

float cellular_f1(vec2 p) {
	vec2 i = floor(p);
	float f1 = 10.0;
	for (int x = -1; x <= 1; x++) {
		for (int y = -1; y <= 1; y++) {
			vec2 neighbor = i + vec2(float(x), float(y));
			vec2 r = random2(neighbor);
			vec2 point = neighbor + r;
			float dist = length(point - p);
			f1 = min(f1, dist);
		}
	}
	return f1;
}

float fbm_cellular(vec2 p) {
	float value = 0.0;
	float amplitude = 0.5;
	float freq = 1.0;

	for (int i = 0; i < 8; i++) {
		vec2 offset = vec2(float(i) * 1.7321, float(i) * 3.1415);
		value += cellular_f1(p * freq + offset) * amplitude;
		freq *= 3.0;
		amplitude *= 0.5;
	}

	return value;
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

	vec2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);

	float n = mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
	return n;
}

float fbm_perlin(vec2 uv) {
	float value = 0.0;
	float amplitude = 0.5;
	float frequency_accum = 1.0;

	for (int i = 0; i < 8; i++) {
		value += perlin_noise(uv * frequency_accum) * amplitude;
		frequency_accum *= 2.0;
		amplitude *= 0.5;
	}

	return value;
}

void fragment() {
	vec2 distortion = vec2(
		fbm_perlin(UV * frequency * 0.5 + TIME * 0.25),
		fbm_perlin(UV * frequency * 0.5 + TIME * 0.25 + vec2(5.2, 1.3))
	) * 0.3;

	float shallow = fbm_cellular((UV + distortion) * frequency + TIME * wave_speed * vec2(0.3, 0.2));
	float caustic_near = range(shallow, 0.6, 1.0, 0.15);

	float deep = fbm_cellular((UV + distortion * 0.5) * frequency * 0.5 + TIME * wave_speed * 0.4 * vec2(0.2, 0.3));
	float caustic_far = range(deep, 0.6, 1.0, 0.15) * 0.4;

	vec3 color = color_deep.rgb;
	color += caustic_near * color_caustic.rgb * caustic_brightness;
	color += caustic_far * color_caustic.rgb * caustic_brightness;
	
	COLOR = vec4(color, 1.0);
}