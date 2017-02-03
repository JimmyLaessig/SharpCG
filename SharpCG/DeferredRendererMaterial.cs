using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using OpenTK.Graphics.OpenGL4;


namespace SharpCG
{

    public class GeometryPassMaterial : Material
    {

        private Texture2D diffuseMapTexture;
        private Texture2D normalMapTexture;
        private Texture2D specularMapTexture;



        private vec3 emissiveAmount     = new vec3(0, 0, 0);
        private vec4 diffuseAmount      = new vec4(1, 1, 1, 1);
        private vec4 specularAmount     = new vec4(1, 1, 1, 128);

        private bool normalMappingEnabled = true;

        
        public override void OnStart()
        {
            Shader = Shader.Find("deferredGeometryPass");

            base.OnStart();
        }

        public bool HasDiffuseMap
        {
            get { return (diffuseMapTexture != null); }
        }


        public bool HasNormapMap
        {
            get { return (normalMapTexture != null); }
        }


        public bool HasSpecularMap
        {
            get { return (SpecularMapTexture != null); }
        }


        public Texture2D DiffuseMapTexture
        {
            get { return diffuseMapTexture; }
            set { diffuseMapTexture = value; }
        }


        public Texture2D NormalMapTexture
        {
            get { return normalMapTexture; }
            set { normalMapTexture = value; }
        }


        public Texture2D SpecularMapTexture
        {
            get { return specularMapTexture; }
            set { specularMapTexture = value; }
        }


        public vec4 DiffuseAmount
        {
            get { return diffuseAmount; }
            set { diffuseAmount = value; }
        }


        public vec3 SpecularAmount
        {
            get { return specularAmount.rgb; }
            set { specularAmount.rgb = value; }
        }


        public vec3 EmissiveAmount
        {
            get { return emissiveAmount; }
            set { emissiveAmount = value; }
        }


        public float SpecularExponent
        {
            get { return specularAmount.a; }
            set { specularAmount.a = value; }
        }


        public bool NormalMappingEnabled
        {
            get { return normalMappingEnabled; }
            set { normalMappingEnabled = value; }
        }


        protected override void InitUniformLocations()
        {                     
            uniformLocations["bHasDiffuseMap"]      = GL.GetUniformLocation(Shader.ProgramHandle, "bHasDiffuseMap");          
            uniformLocations["bHasSpecularMap"]     = GL.GetUniformLocation(Shader.ProgramHandle, "bHasSpecularMap");
            uniformLocations["bHasNormalMap"]       = GL.GetUniformLocation(Shader.ProgramHandle, "bHasNormalMap");

            uniformLocations["texDiffuseMap"]       = GL.GetUniformLocation(Shader.ProgramHandle, "texDiffuseMap");
            uniformLocations["texNormalMap"]        = GL.GetUniformLocation(Shader.ProgramHandle, "texNormalMap");
            uniformLocations["texSpecularMap"]      = GL.GetUniformLocation(Shader.ProgramHandle, "texSpecularMap");

            uniformLocations["vMaterialDiffuse"]    = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialDiffuse");
            uniformLocations["vMaterialEmissive"]   = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialEmissive");
            uniformLocations["vMaterialSpecular"]   = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialSpecular");

            base.InitUniformLocations();
        }


        public override void Bind(ref uint textureUnit)
        {
            base.Bind(ref textureUnit);

            GL.Uniform1(uniformLocations["bHasDiffuseMap"], HasDiffuseMap ? 1 : 0);
            GL.Uniform1(uniformLocations["bHasNormalMap"], (HasNormapMap && normalMappingEnabled) ? 1 : 0);
            GL.Uniform1(uniformLocations["bHasSpecularMap"], HasSpecularMap ? 1 : 0);

            GL.Uniform3(uniformLocations["vMaterialEmissive"], 1, emissiveAmount.Values);
            GL.Uniform4(uniformLocations["vMaterialDiffuse"], 1, diffuseAmount.Values);      
            GL.Uniform4(uniformLocations["vMaterialSpecular"], 1, specularAmount.Values);

            if (HasDiffuseMap)
            {
                GL.Uniform1(uniformLocations["texDiffuseMap"], 0);
                diffuseMapTexture.Bind(TextureUnit.Texture0);
            }
            if (HasSpecularMap)
            {
                GL.Uniform1(uniformLocations["texSpecularMap"], 1);
                specularMapTexture.Bind(TextureUnit.Texture1);
            }
            if (HasNormapMap)
            {
                GL.Uniform1(uniformLocations["texNormalMap"], 2);
                normalMapTexture.Bind(TextureUnit.Texture2);
            }

        }
    }


    public class LightingPassMaterial : Material
    {

        private Texture2D diffuseAlbedoTexture;
        private Texture2D specularAlbedoTexture;
        private Texture2D worldNormalTexture;

        private Texture2D depthTexture;

        private vec3 cameraPosition;


        private vec3 lightDirection;
        private vec3 lightPosition;
        private vec3 lightColor;
        private vec3 lightAttenuation;

        private int lightType;

        private mat4 inverseViewProjectionMatrix;

        public override void OnStart()
        {
            Shader = Shader.Find("deferredLightingPass");
            base.OnStart();
        }


        public Texture2D WorldNormalTexture
        {
            get{ return worldNormalTexture; }
            set{worldNormalTexture = value;}
        }


        public Texture2D DiffuseAlbedoTexture
        {
            get{return diffuseAlbedoTexture;}
            set{diffuseAlbedoTexture = value;}
        }


        public Texture2D SpecularAlbedoTexture
        {
            get{return specularAlbedoTexture;}
            set{ specularAlbedoTexture = value;}
        }


        public vec3 CameraPosition
        {
            get { return cameraPosition; }

            set{cameraPosition = value;}
        }


        public vec3 LightDirection
        {
            get{ return lightDirection;}
            set{lightDirection = value;}
        }


        public vec3 LightPosition
        {
            get{return lightPosition;}
            set{lightPosition = value;}
        }


        public vec3 LightColor
        {
            get{return lightColor; }

            set{ lightColor = value;}
        }


        public vec3 LightAttenuation
        {
            get{return lightAttenuation;}
            set{lightAttenuation = value;}
        }


        public int LightType
        {
            get{ return lightType;}
            set{lightType = value;}
        }


        public mat4 InverseViewProjectionMatrix
        {
            get{return inverseViewProjectionMatrix;}
            set{inverseViewProjectionMatrix = value;}
        }


        public Texture2D DepthTexture
        {
            get{return depthTexture;}
            set{ depthTexture = value;}
        }


        protected override void InitUniformLocations()
        {
            base.InitUniformLocations();

            uniformLocations["texWorldNormal"]      = GL.GetUniformLocation(Shader.ProgramHandle, "texWorldNormal");
            uniformLocations["texDiffuseAlbedo"]    = GL.GetUniformLocation(Shader.ProgramHandle, "texDiffuseAlbedo");
            uniformLocations["texSpecularAlbedo"]   = GL.GetUniformLocation(Shader.ProgramHandle, "texSpecularAlbedo");
            uniformLocations["texDepth"]            = GL.GetUniformLocation(Shader.ProgramHandle, "texDepth");

            
            uniformLocations["mInvViewProj"]        = GL.GetUniformLocation(Shader.ProgramHandle, "mInvViewProj");
            uniformLocations["vCameraPosition"]     = GL.GetUniformLocation(Shader.ProgramHandle, "vCameraPosition");
            uniformLocations["vViewportSize"]       = GL.GetUniformLocation(Shader.ProgramHandle, "vViewportSize");

            uniformLocations["vLightDirection"]     = GL.GetUniformLocation(Shader.ProgramHandle, "vLightDirection");
            uniformLocations["vLightPosition"]      = GL.GetUniformLocation(Shader.ProgramHandle, "vLightPosition");
            uniformLocations["vLightColor"]         = GL.GetUniformLocation(Shader.ProgramHandle, "vLightColor");
            uniformLocations["vLightAttenuation"]   = GL.GetUniformLocation(Shader.ProgramHandle, "vLightAttenuation");
            uniformLocations["iLightType"]          = GL.GetUniformLocation(Shader.ProgramHandle, "iLightType");

        }


        public override void Bind(ref uint textureUnit)
        {
            
            Shader.bind();


            GL.Uniform1(uniformLocations["texDiffuseAlbedo"], 0);
            diffuseAlbedoTexture.Bind(TextureUnit.Texture0);

            GL.Uniform1(uniformLocations["texSpecularAlbedo"], 1);
            specularAlbedoTexture.Bind(TextureUnit.Texture1);

            GL.Uniform1(uniformLocations["texWorldNormal"], 2);
            worldNormalTexture.Bind(TextureUnit.Texture2);

            GL.Uniform1(uniformLocations["texDepth"], 3);
            depthTexture.Bind(TextureUnit.Texture3);

            GL.Uniform3(uniformLocations["vCameraPosition"], 1, cameraPosition.Values);

            GL.UniformMatrix4(uniformLocations["mInvViewProj"], 1, false, inverseViewProjectionMatrix.Values1D);

            GL.Uniform1(uniformLocations["iLightType"], lightType);

            GL.Uniform3(uniformLocations["vLightDirection"], 1, lightDirection.Values);
            GL.Uniform3(uniformLocations["vLightPosition"], 1, lightPosition.Values);            
            GL.Uniform3(uniformLocations["vLightColor"], 1, lightColor.Values);
            GL.Uniform3(uniformLocations["vLightAttenuation"], 1, lightAttenuation.Values);

            base.Bind(ref textureUnit);
        }
    }
}
