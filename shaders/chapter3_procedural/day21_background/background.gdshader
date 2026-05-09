shader_type canvas_item;

uniform vec3 nebula_color1 : source_color = vec3(0.5, 0.2, 0.8);
uniform vec3 nebula_color2 : source_color = vec3(0.145, 0.604, 0.588);
uniform vec3 nebula_color3 : source_color = vec3(1.0, 0.3, 0.2);

uniform float nebula_brightness : hint_range(0.0, 2.0) = 1.3;
uniform float star_occlusion : hint_range(0.0, 5.0) = 2.0;

const vec2 texture_pixel_size = vec2(1.0) / vec2(512.0, 512.0);

vec2 gradient(vec2 p) {
	float angle = fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453) * TAU;
	return vec2(cos(angle), sin(angle));
}

float perlin(vec2 uv) {
	vec2 i = floor(uv); vec2 f = fract(uv);
	vec2 ga = gradient(i);
	vec2 gb = gradient(i + vec2(1, 0));
	vec2 gc = gradient(i + vec2(0, 1));
	vec2 gd = gradient(i + vec2(1, 1));
	float a = dot(ga, f), b = dot(gb, f - vec2(1, 0));
	float c = dot(gc, f - vec2(0, 1)), d = dot(gd, f - vec2(1, 1));
	vec2 u = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
	return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 uv) {
	float v = 0.0, a = 0.5;
	for (int i = 0; i < 6; i++) {
		v += perlin(uv) * a;
		uv *= 2.0; a *= 0.5;
	}
	return v * 0.5 + 0.5;
}

vec3 star_layer(vec2 uv, float density, float angle, float seed) {
	float c = cos(angle), s = sin(angle);
	vec2 rotated = mat2(vec2(c, -s), vec2(s, c)) * uv;

	vec2 grid = floor(rotated * density);

	float r1 = fract(sin(dot(grid + seed, vec2(127.1, 311.7))) * 43758.5453);
	float r2 = fract(sin(dot(grid + seed, vec2(269.5, 183.3))) * 43758.5453);
	float r3 = fract(sin(dot(grid + seed, vec2(419.2, 371.9))) * 43758.5453);
	float r4 = fract(sin(dot(grid + seed, vec2(631.7, 523.1))) * 43758.5453);

	if (r1 < 0.97) return vec3(0.0);

	vec2 cell_uv = fract(rotated * density) - 0.5;
	float dist = length(cell_uv);

	float pixel_radius = texture_pixel_size.x * density * 2.0;
	float size = 0.05 + r2 * 0.15;

	float display_size;
	float area_scale;
	if (size < pixel_radius) {
		display_size = pixel_radius;
		area_scale = (size / pixel_radius) * (size / pixel_radius);
	} else {
		display_size = size;
		area_scale = 1.0;
	}

	float brightness = (0.4 + r3 * 0.6) * area_scale;
	float glow = exp(-dist * dist / (display_size * display_size * 0.5));

	vec3 tints[3] = {
		vec3(0.9, 0.95, 1.0),
		vec3(1.0, 1.0, 0.9),
		vec3(1.0, 0.95, 0.85)
	};
	vec3 color = tints[int(r4 * 3.0)];

	return color * brightness * glow;
}

vec3 nebula_layer(vec2 uv, float scale, vec2 offset, vec3 color, float falloff) {
	vec2 warp = vec2(
		fbm(uv * scale * 0.5 + offset),
		fbm(uv * scale * 0.5 + offset + vec2(5.2, 1.3))
	) * 0.4;

	float density = fbm(uv * scale + offset + warp);

	density = pow(max(0.0, density - 0.3) / 0.7, falloff);

	return color * density;
}

void fragment() {
	vec3 stars = vec3(0.0);
	stars = max(stars, star_layer(UV + TIME * 0.008, 50.0, 0.0, 0.0));
	stars = max(stars, star_layer(UV + TIME * 0.006, 60.0, 0.785, 1.7));
	stars = max(stars, star_layer(UV + TIME * 0.004, 70.0, 0.393, 3.3));
	stars = max(stars, star_layer(UV + TIME * 0.002, 80.0, 1.178, 5.1));

	vec3 nebula = vec3(0.0);
	nebula += nebula_layer(UV + TIME * 0.005, 2.5, vec2(0.0, 0.0), nebula_color1, 2.0);
	nebula += nebula_layer(UV + TIME * 0.0075, 3.5, vec2(3.7, 1.9), nebula_color2, 2.5);
	nebula += nebula_layer(UV + TIME * 0.01, 2.0, vec2(7.3, 4.2), nebula_color3, 3.0);

	nebula = clamp(nebula * nebula_brightness, 0.0, 1.0);
	float nebula_density = dot(nebula, vec3(0.299, 0.587, 0.114));
	float star_visibility = 1.0 - clamp(nebula_density * star_occlusion, 0.0, 1.0);

	COLOR = vec4(nebula + stars * star_visibility, 1.0);
}