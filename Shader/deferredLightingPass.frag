#version 330 core

in vec2 vTexcoords;


uniform sampler2D texWorldPosition;
uniform sampler2D texWorldNormal;
uniform sampler2D texDiffuseAlbedo;
uniform sampler2D texSpecularAlbedo;


uniform vec3 vCameraPosition;

uniform vec3 vLightDirection;
uniform vec3 vLightColor;
uniform vec3 vLightAttenuation;


uniform int lightType = 0;	
// 0 ... Ambient
// 1 ... Directional
// 2 ... Point
// 3 ... Spot


out vec4 fragColor;



float attenuation(float linear, float quadratic, float distance)
{
    return 1.0 / (1.0 + linear * distance + quadratic * distance * distance);
}



void main()
{
	vec4 normal			= texture(texWorldNormal, vTexcoords);	
	float hasGeometry	= normal.w;

	// Extract GBuffer Information
	vec3 vWorldNormal	= normalize(normal.xyz); 
    vec3 vWorldPosition = texture(texWorldPosition, vTexcoords).xyz;
    vec4 vDiffuseColor	= texture(texDiffuseAlbedo, vTexcoords);
    vec4 vSpecularColor = texture(texSpecularAlbedo, vTexcoords);

	

	
	fragColor = vDiffuseColor;
	
}