#version 330 core

in vec3 pWorldPosition;
in vec3 pWorldNormal;
in vec3 pWorldTangent;
in vec3 pWorldBitangent;
in vec2 pTexcoords;


uniform bool bHasDiffuseMap		= true;
uniform bool bHasNormalMap		= true;
uniform bool bHasSpecularMap	= true;


uniform sampler2D texDiffuseMap;
uniform sampler2D texNormalMap;
uniform sampler2D texSpecularMap;


uniform vec3 vMaterialEmissive;
uniform vec4 vMaterialDiffuse;
uniform vec4 vMaterialSpecular;

layout (location = 0) out vec4 gDiffuseAlbedo;
layout (location = 1) out vec4 gSpecularAlbedo;
layout (location = 2) out vec4 gWorldPosition;
layout (location = 3) out vec4 gWorldNormal;



//--------------------------------------------------------------------------------------
// Normal mapping
//--------------------------------------------------------------------------------------
// This function returns the normal of the perturbed surface in world coordinates.
// The input struct contains tangent (t1), bitangent (t2) and normal (n) of the
// unperturbed surface in world coordinates. The perturbed normal in tangent space
// can be read from texNormalMap.
// The RGB values in this texture need to be normalized from (0, +1) to (-1, +1).
vec3 calcNormal(vec3 normal, vec3 tangent, vec3 bitangent)
{
	if(bHasNormalMap)
	{	
		vec3 perturbedNormal	= normalize(texture(texNormalMap, pTexcoords).xyz * 2.0 -1.0);
		mat3 mTBN				= mat3(tangent, bitangent, normal);
		return normalize(mTBN * perturbedNormal);
	}
	return normal;	
}


void main()
{
    gWorldPosition.xyz = pWorldPosition;
	gWorldPosition.w = 1;

	// Calculate pertubed normal  
	gWorldNormal.xyz = calcNormal(	normalize(pWorldNormal), 
									normalize(pWorldTangent), 
									normalize(pWorldBitangent));
	gWorldNormal.w = 1;		
	
	// Get diffuse color from texture
	vec4 vDiffuseColor	= (bHasDiffuseMap)	? texture(texDiffuseMap, pTexcoords) : vec4(1);
	// Get specular color from texture
	vec4 vSpecularColor = (bHasSpecularMap) ? texture(texSpecularMap, pTexcoords) : vec4(1);

	gDiffuseAlbedo		= vDiffuseColor * vMaterialDiffuse;
	gSpecularAlbedo		= vec4 (vSpecularColor.rgb * vMaterialSpecular.rgb, vMaterialSpecular.a);

}