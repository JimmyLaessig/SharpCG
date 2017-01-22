#version 330 core

in vec3 pWorldPosition;
in vec3 pWorldNormal;
in vec3 pWorldTangent;
in vec3 pWorldBitangent;
in vec2 pTexcoords;


uniform bool bHasDiffuseMap;
uniform bool bHasNormalMap;
uniform bool bHasSpecularMap;


uniform sampler2D texDiffuseMap;
uniform sampler2D texNormalMap;
uniform sampler2D texSpecularMap;


uniform vec3 vMaterialDiffuse;
uniform vec3 vMaterialSpecular;
uniform vec3 vMaterialEmissive;
uniform vec3 vMaterialAmbient;
uniform float fMaterialShininess;

uniform bool bNormalMappingEnabled = true;

layout (location = 0) out vec4 gWorldPosition;
layout (location = 1) out vec4 gWorldNormal;
layout (location = 2) out vec4 gDiffuseAlbedo;
layout (location = 3) out vec4 gSpecularAlbedo;


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
	if(bHasNormalMap && bNormalMappingEnabled)
	{	
		vec3 perturbedNormal = normalize(texture(texNormalMap, pTexcoords).xyz * 2.0 -1.0);
		mat3 mTBN = mat3(tangent, bitangent, normal);
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

	gDiffuseAlbedo.rgb = vDiffuseColor.rgb * vMaterialDiffuse;
	gDiffuseAlbedo.a = vDiffuseColor.a;
	gSpecularAlbedo.rgb = vSpecularColor.rgb * vMaterialSpecular;
	gSpecularAlbedo.a = fMaterialShininess;		
}