#version 330 core

in vec2 TexUV;

struct Light {
    vec3 position;
    vec3 color;
    float linear;
    float quadratic;
	vec2 near_far;
    float radius;
};

uniform sampler2D gWorldPosition;
uniform sampler2D gWorldNormal;
uniform sampler2D gDiffuse;
uniform sampler2D gAmbient;

uniform sampler2D colorShadowMap;
uniform mat4 lightViewProjectionMatrix;

uniform vec3 eyePosition;

uniform bool ssaoEnabled;
uniform bool omniDirShadowsEnabled;
uniform bool colorShadowsEnabled;

uniform float bloomHighpassFilterFactor = 0.90f;

// Light Mode describing what light components should light the scene: 
// 0: Ambient + Diffuse + Specular
// 1: Ambient + Diffuse
// 2: Ambient Only
uniform int lightMode = 0;

#define AMBIENT_LIGHT 0.1f
#define NUM_LIGHTS 7

uniform Light lights[NUM_LIGHTS];
uniform samplerCube shadowMaps[NUM_LIGHTS];

layout(location = 0) out vec4 fragColor;
layout(location = 1) out vec4 brightColor;


vec3 ambientComponent()
{
	if(ssaoEnabled == true)
		return vec3(texture(gAmbient, TexUV).x) * AMBIENT_LIGHT;
	return vec3(AMBIENT_LIGHT);
}


float diffuseComponent(vec3 L, vec3 N)
{
    return clamp(dot(L, N), 0.0, 1.0);
}


float specularComponent(vec3 lightDirection, vec3 viewDirection, vec3 normal, float shininess)
{
	if(dot(normal, lightDirection) < 0.0)
		return 0.0;

	return pow(max(0.0, dot(reflect(-lightDirection, normal), viewDirection)), shininess);
}


float attenuation(float linear, float quadratic, float distance)
{
    return 1.0 / (1.0 + linear * distance + quadratic * distance * distance);
}


bool inShadow(vec3 position, vec3 lightPosition, samplerCube  shadowCubeMap, vec2 near_far)
{	 
	// Fragment is not in shadow if the shadows aren't enabled (obviously)
	if(!omniDirShadowsEnabled)
		return false;

	// calculate vector from surface point to light position
	// (both positions are given in world space)
	vec3 cubemap_lookup_vec = position - lightPosition;

	// read depth value from cubemap shadow map
	float minDist = texture(shadowCubeMap, normalize(cubemap_lookup_vec)).r;

	// WS “dist-to-lightsource” for current fragment
	float currentDist = length(cubemap_lookup_vec);

	// undo previous [0;1]-mapping of distance to light source
	minDist = minDist * near_far.y;
	float bias = 0.03;

	return (minDist  + bias < currentDist);
}


vec4 bloomHighPassFilter(vec4 color)
{
	// Do a Highpass Filter for Bloom Effect
	// Convert to grayscale using NTSC conversion weights
    float brightness = dot(color.rgb, vec3(0.299, 0.587, 0.114));
	return (brightness > bloomHighpassFilterFactor) ?  color : vec4(0);	
}


void main()
{
    vec3 worldPosition = texture(gWorldPosition, TexUV).xyz;
    vec3 worldNormal = normalize(texture(gWorldNormal, TexUV).xyz);
    vec4 color =  texture(gDiffuse, TexUV);
    

	vec3 ambientColor = (worldNormal == vec3(0)) ? vec3(1) : ambientComponent();
    vec3 diffuseColor = vec3(0);
    vec3 specularColor = vec3(0);

	// Calculate lighting of fragment in View Space in order to unify procedure and minimalize overhead
	// The lighting of a fragment consists of the ambient color, which influenced by Screen Space Ambient Occlusion, 
	// diffuse color which is calculated with the dot product of the normal with the light direction and
	// the specular color (hightlight color)
	// Furthermore for each light source we determine if the fragment lies in the shadow and needs not to be shaded at all. 
	// In this case only the diffuse color will affect lighting of the fragment. 
	
	bool shadow = false;

	for(int i = 0; i < NUM_LIGHTS; i++)
	{
		vec3 color = lights[i].color;
		vec3 lightPosition = lights[i].position;
		float linear = lights[i].linear;
		float quadratic = lights[i].quadratic;
		vec2 near_far = lights[i].near_far;
		float radius = lights[i].radius;	
		
		// Determine if fragment is in shadow
		if(inShadow(worldPosition, (vec4(lightPosition, 1)).xyz, shadowMaps[i], near_far))
			continue;
		
		vec3 viewDirection = normalize(eyePosition - worldPosition);
		vec3 lightDirection = lightPosition - worldPosition;
		
		 if(length(lightDirection) > radius)
			continue;       
	
		// Attenuation
		float attenuation = attenuation(linear, quadratic, length(lightDirection));		
		
		lightDirection = normalize(lightDirection);
       
		// Sum diffuse component
		 if(lightMode == 0 || lightMode == 1)		
			diffuseColor += color * diffuseComponent(lightDirection, worldNormal) * attenuation;
		   
		// Sum specular component			
		if(lightMode == 0)				
			specularColor += color * specularComponent(lightDirection, viewDirection, worldNormal, 16) * 0.8f * attenuation;				
	}
	
	//hier ist der Colorpart

	if(colorShadowsEnabled)
	{
		vec4 posLigthSpace = lightViewProjectionMatrix * vec4(worldPosition,1);
		vec4 colorshadowvalue = texture(colorShadowMap,	posLigthSpace.xy);
		if(colorshadowvalue[0] != colorshadowvalue[1] || colorshadowvalue[0] != colorshadowvalue[2] || colorshadowvalue[1] != colorshadowvalue[2] )
		{
			if(posLigthSpace.z  >  colorshadowvalue.z)
				diffuseColor += colorshadowvalue.xyz;
		}
	}
	//bis hier	

	color.rgb =  (ambientColor + diffuseColor + specularColor) * color.rgb;
	brightColor = bloomHighPassFilter(color);
	//color.rgb = ambientColor / AMBIENT_LIGHT; // Uncomment to test SSAO
	fragColor = color;
	
}