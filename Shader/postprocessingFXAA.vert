#version 330 core

layout (location = 0) in vec3 vPosition;


uniform float FXAA_SUBPIX_SHIFT = 1.0/4.0;
uniform vec2 vViewportSize = vec2(1024, 768);

out vec4 vTexcoords;

void main(void)
{
	gl_Position = vec4(vPosition, 1);

	vec2 invViewportSize	= 1.0 / vViewportSize;
	vTexcoords.xy			= vPosition.xy * 0.5 + 0.5;
	vTexcoords.zw			= (vPosition.xy * 0.5 + 0.5) - (invViewportSize * (0.5 + FXAA_SUBPIX_SHIFT));
}