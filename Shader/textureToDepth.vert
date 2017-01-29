#version 330 core

layout (location = 0) in vec3 vPosition;


uniform mat4 mWVP;


out vec2 vTexcoords;


void main() 
{
	vec4 position	= mWVP * vec4(vPosition, 1.0f);
	
	//position = position * 2.0 - 1.0;
	vTexcoords		= position.xy * 0.5 + 0.5;
    gl_Position		= position;
}