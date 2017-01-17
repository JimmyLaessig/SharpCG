#version 440 core

in vec3 texcoords;

uniform samplerCube cubeMap;

out vec4 fragColor;

void main(void)
{
    fragColor = textureCube(cubeMap, texcoords);
	
}
