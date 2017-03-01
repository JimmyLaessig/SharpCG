#version 330 core


in vec2 pTexcoords;
in vec4 pColor;

uniform bool bHasDiffuseMap;
uniform sampler2D texDiffuseMap;

layout (location = 0) out vec4 vFragColor;


void main()
{
	vFragColor = (bHasDiffuseMap) ? texture(texDiffuseMap, pTexcoords) : pColor;	
	//vFragColor = vec4(1, 0, 0, 1);
}