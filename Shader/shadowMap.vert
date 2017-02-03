#version 330 core

layout (location = 0) in vec3 vPosition;



uniform mat4 mWorld;
uniform mat4 mView;
uniform mat4 mProj;
uniform mat4 mWVP;
uniform mat3 mNormal;

void main() 
{
    gl_Position		= mWVP * vec4(vPosition, 1.0f);   
}