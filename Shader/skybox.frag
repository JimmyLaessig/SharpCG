#version 440 core
#extension GL_NV_shadow_samplers_cube : enable

in vec3 texcoords;

uniform samplerCube texCubeMap;


out vec4 vFragColor;

void main(void)
{
	gl_FragDepth	= 1.0;
    vFragColor		= textureCube(texCubeMap, texcoords);

}
