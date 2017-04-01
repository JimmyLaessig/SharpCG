#version 330 core

layout (location = 0) in vec3 vPosition;


uniform mat4 mWVP;

uniform int iLightType;
// 0 ... Ambient
// 1 ... Directional
// 2 ... Point
// 3 ... Spot


out vec2 vTexcoords;


void main() 
{
	vec4 position;

	if(iLightType == 0 || iLightType == 1 )
	{
		position = vec4(vPosition, 1.0f);
	}
	else
	{
		position	= mWVP * vec4(vPosition, 1.0f);
	}
	vTexcoords		= (position.xy / position.w) * 0.5 + 0.5;
    gl_Position		= position;
}