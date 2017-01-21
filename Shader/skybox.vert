#version 440 core


layout (location = 0) in vec3 vPosition;

uniform mat4 mWVP;

out vec3 texcoords;

void main(void)
{
    gl_Position = mWVP * vec4(vPosition, 1);
    texcoords = normalize(vPosition);
}
