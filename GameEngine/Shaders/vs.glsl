#version 330

layout (location = 0) in vec3 a_Position;
layout (location = 1) in vec2 a_TexCoord;
layout (location = 2) in vec3 a_Normal;
layout (location = 3) in float a_Time;
layout (location = 4) in float a_Time2;

uniform mat4 ModelViewProjMat;
uniform mat4 ModelMat;
uniform vec3 LightPosition[4];
uniform vec3 LightColour[4];

out vec2 v_TexCoord;
out vec3 v_Normal;
out vec3 v_FragPos;
out vec3 v_LightPos[4];
out vec3 v_LightColour[4];
out float v_Time;

void main()
{
    //sets the variables going out to the fragment shader and loops around the light positions and colours getting the correct lights
    gl_Position = ModelViewProjMat * vec4(a_Position, 1.0);
    v_FragPos = vec3(ModelMat * vec4(a_Position, 1.0));
    v_TexCoord = a_TexCoord;
    v_Normal = a_Normal;
    v_Time = a_Time2;
    for(int i = 0; i < 4; i++)
    {
        v_LightPos[i] = LightPosition[i];
        v_LightColour[i] = LightColour[i];
    }
}