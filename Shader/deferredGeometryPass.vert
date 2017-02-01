#version 330 core

layout (location = 0) in vec3 vPosition;
layout (location = 1) in vec3 vNormal;
layout (location = 2) in vec3 vTangent;
layout (location = 3) in vec3 vBitangent;
layout (location = 4) in vec4 vColor;
layout (location = 5) in vec2 vTexcoords;


uniform mat4 mWorld;
uniform mat4 mView;
uniform mat4 mProj;
uniform mat4 mWVP;
uniform mat3 mNormal;


out vec3 pWorldPosition;
out vec3 pWorldNormal;
out vec3 pWorldTangent;
out vec3 pWorldBitangent;
out vec2 pTexcoords;

void main() 
{
	gl_Position		= (mWVP		* vec4(vPosition, 1.0f));   
	pWorldPosition	= (mWorld	* vec4(vPosition, 1.0f)).xyz;
    pWorldNormal	= (mNormal * vNormal);   
    pWorldTangent	= (mNormal * vTangent);
    pWorldBitangent	= (mNormal * vBitangent);
	pTexcoords		= vTexcoords;	  
}
