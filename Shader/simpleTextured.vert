#version 330 core

layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec3 vTangent;
layout (location = 3) in vec3 vBitangent;
layout (location = 4) in vec4 vColor;
layout (location = 5) in vec2 vTexcoords;


uniform mat4 mWVP;


out vec2 pTexcoords;


void main() 
{
    gl_Position		= mWVP * vec4(vPosition, 1.0f);   
	pTexcoords		= vTexcoords;	
}