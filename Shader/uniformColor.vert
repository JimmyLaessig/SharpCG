#version 330 core

layout (location = 0) in vec3 vPosition;

uniform mat4 mWVP;

void main() 
{
    gl_Position		= mWVP * vec4(vPosition, 1.0f);   
}


