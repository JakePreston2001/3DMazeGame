#version 330

in vec2 v_TexCoord;
uniform sampler2D s_texture;
uniform vec3 v_diffuse;    // OBJ NEW

out vec4 Color;
//a fragment shader that uses no lights, giving a solid texture all over with no shadows
void main()
{
    Color = vec4(v_diffuse, 1) * texture2D(s_texture, v_TexCoord);
}