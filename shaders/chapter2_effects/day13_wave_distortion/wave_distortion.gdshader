shader_type canvas_item;

uniform int direction : hint_enum("Horizontal", "Vertical", "Both");

uniform float frequency : hint_range(0.1, 20.0) = 3.0;
uniform float amplitude : hint_range(0.0, 100.0) = 10.0;
uniform float speed : hint_range(0.0, 10.0) = 1.0;

void fragment() {
    float t = TIME * speed;
	
	float wave_x = sin(UV.y * frequency * TAU + t) * amplitude * TEXTURE_PIXEL_SIZE.x;
	float wave_y = sin(UV.x * frequency * TAU + t) * amplitude * TEXTURE_PIXEL_SIZE.y;

	vec2 offset = vec2(0.0);
	switch (direction) {
		case 0: // Horizontal
			offset = vec2(wave_x, 0.0);
			break;
			
		case 1: // Vertical
			offset = vec2(0.0, wave_y);
			break;
			
		case 2: // Both
			offset = vec2(wave_x, wave_y);
			break;
	}
	
	vec2 distorted_uv = UV + offset;
	COLOR = texture(TEXTURE, distorted_uv);
}
