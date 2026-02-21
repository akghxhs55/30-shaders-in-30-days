shader_type canvas_item;

uniform int animation_mode : hint_enum(
	"Scrolling",
	"Pulsing Circle",
	"Rotation",
	"Color Cycle",
	"Ping Pong"
) = 0;

uniform float speed : hint_range(0.0, 5.0) = 1.0;

// Ping Pong
uniform int transition : hint_enum(
	"Linear",
	"Quadratic",
	"Cubic",
	"Sine",
	"Back"
) = 0;

uniform int easing : hint_enum(
	"Ease-In",
	"Ease-Out",
	"Ease-In-Out",
	"Ease-Out-In"
) = 0;

const vec4 color_a = vec4(0.0, 0.0, 0.0, 1.0);
const vec4 color_b = vec4(1.0, 1.0, 1.0, 1.0);

float apply_transition(float t) {
	float result = t;
	switch (transition) {
		case 0: // Linear
			result = t;
			break;
		case 1: // Quad
			result = t * t;
			break;
		case 2: // Cubic
			result = t * t * t;
			break;
		case 3: // Sine
			result = 1.0 - cos(t * PI / 2.0);
			break;
		case 4: // Back
			result = t * t * (2.70158 * t - 1.70158);
			break;
	}
	
	return result;
}

float apply_easing(float t) {
	float result = t;
	switch (easing) {
		case 0: // EaseIn
			result = apply_transition(t);
			break;
		case 1: // EaseOut
			result = 1.0 - apply_transition(1.0 - t);
			break;
		case 2: // EaseInOut
			if (t < 0.5) {
				result = apply_transition(t * 2.0) * 0.5;
			} else {
				result = 1.0 - apply_transition((1.0 - t) * 2.0) * 0.5;
			}
			break;
		case 3: // EaseOutIn
			if (t < 0.5) {
				result = (1.0 - apply_transition(1.0 - t * 2.0)) * 0.5;
			} else {
				result = 0.5 + apply_transition((t - 0.5) * 2.0) * 0.5;
			}
			break;
	}
	
	return result;
}

void fragment() {
	vec4 color = vec4(0.0);
	
	vec2 uv = UV - vec2(0.5);
	switch (animation_mode) {
		case 0: // Scrolling
			float coord = UV.y + TIME * speed;
			
			float stripe = step(0.5, fract(coord * 4.0));
			color = mix(color_a, color_b, stripe);
			break;
			
		case 1: // Pulsing Circle
			float sdf = length(uv) - 0.3 * (1.0 + 0.3 * sin(TIME * speed));
			
			float mask = smoothstep(0.01, 0.0, sdf);
			color = mix(color_a, color_b, mask);
			break;
			
		case 2: // Rotation
			float angle = TIME * speed;
			float c = cos(angle);
			float s = sin(angle);
			mat2 rotation_mat = mat2(vec2(c, s), vec2(-s, c));
			vec2 rotated_uv = rotation_mat * uv;
			
			float rotation = step(0.5, fract(rotated_uv.x * 4.0));
			color = mix(color_a, color_b, rotation);
			break;
			
		case 3: // Color Cycle
			color = vec4(0.5 + 0.5 * cos(TIME * speed + uv.xyx + vec3(0.0, 2.094, 4.189)), 1.0);
			break;
			
		case 4: // Ping Pong
			float t = fract(TIME * speed);
			float triangle = abs(t * 2.0 - 1.0);
			float eased = apply_easing(triangle);

			float x_offset = mix(-0.3, 0.3, eased);
			float sdf = length(uv - vec2(x_offset, 0.0)) - 0.05;

			float mask = smoothstep(0.01, 0.0, sdf);
			color = mix(color_a, color_b, mask);
			break;
	}
	
	COLOR = color;
}
