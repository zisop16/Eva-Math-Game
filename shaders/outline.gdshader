shader_type canvas_item;

uniform vec4 outline_color : source_color = vec4(1.0);
uniform int pixel_scan_width : hint_range(0, 5) = 1;
uniform float outline_alpha_threshold : hint_range(0, 1, .001) = .1;
instance uniform bool toggled = true;

bool isOutline(vec2 uv, vec2 texture_pixel_size, sampler2D tex) {
	vec4 center_pixel = texture(tex, uv);
	if (center_pixel.a > outline_alpha_threshold) {
		return false;
	}

	for (int xOffset = -pixel_scan_width; xOffset <= pixel_scan_width; xOffset++) {
		for (int yOffset = -pixel_scan_width; yOffset <= pixel_scan_width; yOffset++) {
			if (xOffset == yOffset && xOffset == 0) {
				continue;
			}
			vec4 curr_pixel = texture(tex, uv + texture_pixel_size * vec2(float(xOffset), float(yOffset)));
			if (curr_pixel.a > outline_alpha_threshold) {
				return true;
			}
		}
	}
	
	return false;
}

void fragment() {
	vec2 uv = UV;
	if (toggled && isOutline(uv, TEXTURE_PIXEL_SIZE, TEXTURE)) {
		COLOR = outline_color;
	} else {
		COLOR = texture(TEXTURE, uv);
	}
}