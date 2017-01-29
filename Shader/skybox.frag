#version 440 core
#extension GL_NV_shadow_samplers_cube : enable

in vec3 texcoords;

uniform samplerCube texCubeMap;

layout (location = 0) out vec4 vDiffuseAlbdeo;
//layout (location = 1) out vec4 vSpecularAlbedo;
//layout (location = 2) out vec4 vWorldPosition;
//layout (location = 3) out vec4 vWorldNormal;

void main(void)
{

	gl_FragDepth = 1.0;
    vDiffuseAlbdeo	= textureCube(texCubeMap, texcoords);
	vDiffuseAlbdeo.a = 1;
	//vSpecularAlbedo	= vec4(0);
	//vWorldPosition	= vec4(0);
	//vWorldNormal	= vec4(0);
}
