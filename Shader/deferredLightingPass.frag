#version 330 core

in vec2 vTexcoords;

// GBuffer uniforms
uniform sampler2D texWorldNormal;
uniform sampler2D texDiffuseAlbedo;
uniform sampler2D texSpecularAlbedo;
uniform sampler2D texDepth;


// Light uniforms

uniform int iLightType;
// 0 ... Ambient
// 1 ... Directional
// 2 ... Point
// 3 ... Spot
uniform vec3 vLightPosition;
uniform vec3 vLightDirection;
uniform vec3 vLightColor;
uniform vec3 vLightAttenuation;
uniform mat4 mLightBiasVP;


// ShadowMap uniforms
uniform bool bHasShadowMap;
uniform int iShadowMapSize;
uniform sampler2D texShadowMap;


// Camera uniforms
uniform mat4 mProj;
uniform mat4 mInvViewProj;
uniform vec3 vCameraPosition;


out vec4 vFragColor;


float calcAttenuation(float constant, float linear, float quadratic, float d)
{
    return 1.0 / (constant + linear * d + quadratic * d * d);
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


#define EPSILON 0.0001
float calcVisiblity(vec3 vWorldPosition)
{
	vec2 poission[4] = vec2[](
	  vec2(-0.94201624 ,-0.39906216),
	  vec2( 0.94558609 ,-0.76890725),
	  vec2(-0.09418410 ,-0.92938870),
	  vec2( 0.34495938 , 0.29387760)
	);


	if(bHasShadowMap)
	{
		vec4 shadowPosition = mLightBiasVP * vec4(vWorldPosition, 1.0);
		vec3 lightCoords = shadowPosition.xyz / shadowPosition.w;
		float depth = texture (texShadowMap, lightCoords.xy).r;

		float visibility = (depth > lightCoords.z - EPSILON) ? 1.0 : 0.0;
		
		for (int i = 0; i < 4; i++)
		{
			if(texture(texShadowMap, lightCoords.xy + poission[i] / iShadowMapSize).r > lightCoords.z - EPSILON)
			{
				visibility +=1;
			}
		}

		return visibility / 5;
	}
	return 1.0f;
}


void main()
{
	vec4 normal	= texture(texWorldNormal, vTexcoords);		

	float depth = texture(texDepth, vTexcoords).x;
	

	// Extract GBuffer Information
	vec3 vWorldNormal		= normalize(normal.xyz); 	
    vec3 vWorldPosition		= calcWorldPosition(vec3(vTexcoords, depth) * 2.0 - 1.0);    
	vec4 vDiffuseAlbedo		= texture(texDiffuseAlbedo, vTexcoords);
    vec4 vSpecularAlbedo	= texture(texSpecularAlbedo, vTexcoords);

	// Ambient Light
	if(iLightType == 0)
	{
		if(normal.w <= 0)
		{
			vFragColor = vDiffuseAlbedo;
			return;
		}
		vFragColor.rgb	= vDiffuseAlbedo.rgb * vLightColor;

		vFragColor.a = 1;
		
	}	
	// Directional Light (with ShadowMapping)
	else if(iLightType == 1)
	{		
		if(normal.w <= 0)
		{
			vFragColor = vec4(0);		
			return;
		}
		
		vec3 N = vWorldNormal;
		vec3 V = normalize( vCameraPosition - vWorldPosition);
		vec3 L = normalize(-vLightDirection);	
		vec3 H = normalize( V + L );
		vec3 R = normalize(-reflect (L, N));
		
		
		vec3 Id	= vDiffuseAlbedo.rgb * saturate(dot(N,L));		
		vec3 Is = vSpecularAlbedo.rgb * pow(max(0.0, dot(R, V)),  vSpecularAlbedo.a);
		float visibility = calcVisiblity(vWorldPosition);

		vFragColor.rgb	= ( Id + Is) * vLightColor * visibility;
		vFragColor.a	= 1.0f;		
			
	}
	// Point light
	else if(iLightType == 2)
	{
		if(normal.w <= 0)
		{
			vFragColor = vec4(0);		
			return;
		}

		vec3 N = vWorldNormal;

		vec3 V = normalize( vCameraPosition - vWorldPosition);
		vec3 L = normalize( vLightPosition - vWorldPosition);			
		vec3 H = normalize( V + L );
		vec3 R = normalize(-reflect (L, N));		
		
		float d = length(vLightPosition - vWorldPosition);
		
		vec3 Id	= vDiffuseAlbedo.rgb * saturate(dot(N,L));		
		vec3 Is = vSpecularAlbedo.rgb * pow(max(0.0, dot(R, V)),  vSpecularAlbedo.a);
		//vec3 Is = vSpecularAlbedo.rgb * pow(max(0.0, dot(N, H)),  vSpecularAlbedo.a);
		float att = calcAttenuation(vLightAttenuation.x, vLightAttenuation.y ,vLightAttenuation.z, d);
		vFragColor.rgb	= ( Id + Is) * vLightColor * att;
		//vFragColor.rgb = vec3(1, 0, 0);
		vFragColor.a	= 1.0f;				
	}

	//vFragColor = vec4(0.5, 0.0, 0.0, 1.0);
}