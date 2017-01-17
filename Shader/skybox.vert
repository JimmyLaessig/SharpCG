#version 440 core

layout (location = 0) in vec3 position;

uniform mat4 wvpMatrix;

out vec3 texcoords;

void main(void)
{
    gl_Position = wvpMatrix * vec4(position, 1);
    texcoords = normalize(position);
}
