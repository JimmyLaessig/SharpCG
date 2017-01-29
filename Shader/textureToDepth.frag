#version 440 core


in vec2 vTexcoords;

uniform sampler2D texDepth;



void main(void)
{
	float z = texture(texDepth, vTexcoords).r;	
	gl_FragDepth	=z;

}
