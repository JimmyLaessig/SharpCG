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


uniform vec3 vLightDirection;
uniform vec3 vLightColor;
uniform vec3 vLightAmbient;
uniform vec3 vViewPosition;

uniform bool bNormalMappingEnabled = true;

layout (location = 0) out vec4 fragColor;



//--------------------------------------------------------------------------------------
// Phong Lighting Reflection Model
//--------------------------------------------------------------------------------------
// Returns the sum of the diffuse and specular terms in the Phong reflection model
// The specular and diffuse reflection constants for the currently loaded material (k_d and k_s) as well
// as other material properties are defined in Material.fx.
vec3 calcBlinnPhongLighting( vec3 vLightColor, vec3 vDiffuseColor, vec3 vSpecularColor, vec3 N, vec3 L, vec3 H)
{
	vec3 Id = vDiffuseColor * clamp( dot(N,L), 0.0, 1.0 );
	vec3 Is = vSpecularColor * pow( clamp(dot(N,H), 0.0, 1.0), fMaterialShininess );
    return  ( Id + Is) * vLightColor;
}




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
	
	// Calculate pertubed normal  
	vec3 N = calcNormal(normalize(pWorldNormal), normalize(pWorldTangent), normalize(pWorldBitangent));
	
	// Get diffuse color from texture
	vec4 vDiffuseColor	= vec4(vMaterialDiffuse, 1);
	if (bHasDiffuseMap) 
	{
		 vDiffuseColor *= texture(texDiffuseMap, pTexcoords);
	}

	// Get specular color from texture
	vec4 vSpecularColor = vec4(vMaterialSpecular, 1);
	if (bHasSpecularMap)
	{
		vSpecularColor *= texture(texSpecularMap, pTexcoords);
	}

	// Calculate Directional Lighting
	vec3 L = normalize(-vLightDirection);
	vec3 V = normalize(vViewPosition - pWorldPosition);
	vec3 H = normalize(V + L);


	fragColor.rgb	= vMaterialEmissive;
	fragColor.rgb	+= vMaterialAmbient * vLightAmbient * vDiffuseColor.rgb;
	fragColor.rgb	+= calcBlinnPhongLighting( vLightColor, vDiffuseColor.rgb, vSpecularColor.rgb, N, L, H);
	fragColor.a		= vDiffuseColor.a;
	
	//fragColor = vSpecularColor;
	//fragColor = vec4(1,0,0,1);
	//fragColor.xyz = N;
}