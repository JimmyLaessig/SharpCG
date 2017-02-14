#version 330 core

uniform vec4 vColor;

layout (location = 0) out vec4 vFragColor;

void main()
{
	vFragColor = vColor;
	vFragColor = vec4(1, 0, 0, 1);	
}