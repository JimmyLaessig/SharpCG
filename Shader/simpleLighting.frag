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


uniform vec4 vMaterialDiffuse;
uniform vec4 vMaterialSpecular;
uniform vec3 vMaterialEmissive;


uniform vec3 vLightDirection;
uniform vec3 vLightColor;
uniform vec3 vLightAmbient;
uniform vec3 vViewPosition;

uniform bool bNormalMappingEnabled = true;

layout (location = 0) out vec4 fragColor;



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
	vec4 vDiffuseColor	= (bHasDiffuseMap) ? texture(texDiffuseMap, pTexcoords) : vec4(1);
	// Get specular color from texture
	vec4 vSpecularColor = (bHasSpecularMap) ? texture(texSpecularMap, pTexcoords): vec4(1, 1, 1, 128);

	// calculate Phong lighting parameter
	vec3 L = normalize(-vLightDirection);
	vec3 V = normalize(vViewPosition - pWorldPosition);
	vec3 H = normalize(V + L);

	// Calculate Directional Lighting
	vec3 emissiveColor	= vMaterialEmissive;
	vec3 ambientColor	= vDiffuseColor.rgb	 * vLightAmbient; 
	vec3 diffuseColor	= vDiffuseColor.rgb	 * vLightColor	 * vMaterialDiffuse.rgb	* clamp(dot(L, N), 0.0, 1.0);		
	vec3 specularColor	= vSpecularColor.rgb * vLightColor	 * vMaterialSpecular.rgb * pow (clamp (dot (N, H), 0.0, 1.0), vMaterialSpecular.a);
	
	
	fragColor.rgb = diffuseColor + emissiveColor + ambientColor + specularColor;	
	fragColor.a = vDiffuseColor.a;
	
}