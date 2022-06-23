#version 330
//creates all the variables needed to set the lights position and colour
in vec2 v_TexCoord;
in vec3 v_Normal;
in vec3 v_FragPos;
in vec3 v_LightPos[4];
in vec3 v_LightColour[4];
in float v_Time;

vec3 diffuse;
vec3 lightColor[4];
vec3 LightPos[4];

uniform sampler2D s_texture;
uniform vec3 v_diffuse;    // OBJ NEW
uniform float uTime;

out vec4 Color;
vec3 lightDir;

void main()
{
    vec4 lightAmbient = vec4(0.1, 0.1, 0.1, 1.0);
    //loops around the 4 lights created and adds their colour and positions to the diffuse that is then rendered
    for(int i = 0; i < 4; i++)
    {
        LightPos[i] = v_LightPos[i];
        lightColor[i] = v_LightColour[i];
        LightPos[i] = v_LightPos[i];
        
        vec3 norm = normalize(v_Normal);
        lightDir += normalize(LightPos[i] - v_FragPos); 
        float diff = max(dot(norm, lightDir), 0.01);
        diffuse += diff * v_LightColour[i];
        
    }
    Color = lightAmbient + (vec4(v_diffuse, 1) * texture2D(s_texture, v_TexCoord) * vec4(diffuse, 0));  // OBJ CHANGED
}