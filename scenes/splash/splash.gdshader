shader_type canvas_item;

uniform sampler2D base_tex;
uniform sampler2D over_tex;

void fragment() {
    vec4 base = texture(base_tex, UV);
    vec4 over = texture(over_tex, UV);
	
    if (base.a == 0.0f) { // if base transparent - result transparent
        COLOR = vec4(0.0, 0.0, 0.0, 0.0);
    } else if (over.a == 0.0f) { // if overlay is transparent - draw base color
        COLOR = base;
    } else { // if overlay is not transparent - draw overlay color
        COLOR = over;
    }
}