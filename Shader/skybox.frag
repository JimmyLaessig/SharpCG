#version 440 core
#extension GL_NV_shadow_samplers_cube : enable

in vec3 texcoords;

uniform samplerCube texCubeMap;

out vec4 fragColor;

void main(void)
{
    fragColor = textureCube(texCubeMap, texcoords);
}
