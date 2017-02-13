#version 330 core

in vec2 vTexcoords;


uniform sampler2D texWorldNormal;
uniform sampler2D texDiffuseAlbedo;
uniform sampler2D texSpecularAlbedo;
uniform sampler2D texDepth;

uniform vec3 vCameraPosition;

uniform int iLightType = 0;	
// 0 ... Ambient
// 1 ... Directional
// 2 ... Point
// 3 ... Spot

uniform vec3 vLightPosition;
uniform vec3 vLightDirection;
uniform vec3 vLightColor;
uniform vec3 vLightAttenuation;

uniform sampler2D texShadowMap;
uniform bool bHasShadowMap = false;

uniform mat4 mLightBiasVP;

uniform mat4 mProj;
uniform mat4 mInvViewProj;

out vec4 fragColor;


float attenuation(float linear, float quadratic, float distance)
{
    return 1.0 / (1.0 + linear * distance + quadratic * distance * distance);
}


float saturate(float v)
{
	return clamp (v, 0.0, 1.0);
}


vec3 calcWorldPosition(vec3 ndc)
{
	vec4 clipPos;
	clipPos.w		= mProj[3][2] / (ndc.z - (mProj[2][2] / mProj[2][3]));
	clipPos.xyz		= ndc * clipPos.w;
	vec4 worldPos	= mInvViewProj * clipPos;
	
	//return ndc.xyz;
	return worldPos.xyz;
}
#define EPSILON 0.1

float calcVisiblity(vec3 vWorldPosition)
{
	if(bHasShadowMap)
	{
		vec4 shadowPosition = mLightBiasVP * vec4(vWorldPosition, 1.0);
		vec3 lightCoords = shadowPosition.xyz / shadowPosition.w;
		float depth = texture (texShadowMap, lightCoords.xy).r;

		return (depth < lightCoords.z ) ? 0.0 : 1.0;

	}
	return 1.0f;
}



//float calcVisiblity(vec3 vWorldPosition)
//{
//	if(bHasShadowMap)
//	{
//		vec4 shadowPosition = mLightBiasVP * vec4(vWorldPosition, 1.0);
//		if (texture (texShadowMap, shadowPosition.xy).r < shadowPosition.z + EPSILON)
//			return 0.0f;
//		return 1.0f;

//	}
//	return 0.0f;
//}


void main()
{
	vec4 normal	= texture(texWorldNormal, vTexcoords);	
	
	

	float depth = texture(texDepth, vTexcoords).x;
	

	// Extract GBuffer Information
	vec3 vWorldNormal	= normalize(normal.xyz); 	
    vec3 vWorldPosition = calcWorldPosition(vec3(vTexcoords, depth) * 2.0 - 1.0);    	
	vec4 vDiffuseAlbedo	= texture(texDiffuseAlbedo, vTexcoords);
    vec4 vSpecularAlbedo= texture(texSpecularAlbedo, vTexcoords);

	
	if(iLightType == 0)
	{
		fragColor.rgb = vDiffuseAlbedo.rgb * vLightColor;
		fragColor.a = 1;

		if(normal.w <= 0)
		{
			fragColor = vDiffuseAlbedo;		
			return;
		}
	}

	if(iLightType == 1)
	{		
		if(normal.w <= 0)
		{
			fragColor = vec4(0);		
			return;
		}
		
		vec3 N = vWorldNormal;

		//vec3 N = vec3(0, 1, 0);
		vec3 V = normalize( vCameraPosition - vWorldPosition);

		vec3 L = normalize(-vLightDirection);	
		

		vec3 H = normalize( V + L );

		vec3 R = normalize(-reflect (L, N));
		
		
		
		vec3 Id	= vDiffuseAlbedo.rgb * saturate(dot(N,L));		
		vec3 Is = vSpecularAlbedo.rgb * pow(max(0.0, dot(R, V)),  512f);
		float visibility = calcVisiblity(vWorldPosition);

		fragColor.rgb	= ( Id + Is) * vLightColor * visibility;
		fragColor.a		= 1.0f;		
		
	}
}