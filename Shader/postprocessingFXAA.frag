#version 330 core

in vec2 vTexcoords;

// GBuffer uniforms
uniform sampler2D texColor;
uniform sampler2D texDepth;


out vec4 vFragColor;



void main()
{
	vFragColor = texture(texColor, vTexcoords);
}