using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using OpenTK.Graphics.OpenGL4;

using SharpCG.Core;

namespace SharpCG.Effects
{

    public class DeferredGeometryMaterial : Material
    {

        private Texture2D diffuseMapTexture;
        private Texture2D specularMapTexture;
        private Texture2D normalMapTexture;


        private dvec3 emissiveAmount     = new dvec3(0, 0, 0);
        private dvec4 diffuseAmount      = new dvec4(1, 1, 1, 1);
        private dvec4 specularAmount     = new dvec4(1, 1, 1, 128);

        private bool normalMappingEnabled = true;

        
        public override void OnStart()
        {
            Shader = Shader.Find("deferredGeometryPass");

            base.OnStart();
        }


        public void ConvertFrom(AssimpMaterial material)
        {
            diffuseMapTexture   = material.DiffuseMapTexture;
            specularMapTexture  = material.SpecularMapTexture;
            normalMapTexture    = material.NormalMapTexture;

            emissiveAmount  = material.EmissiveAmount;
            diffuseAmount   = material.DiffuseAmount;
            specularAmount  = material.SpecularAmount;

        }

        public bool HasDiffuseMap
        {
            get => (diffuseMapTexture != null);
        }


        public bool HasNormapMap
        {
            get => (normalMapTexture != null); 
        }


        public bool HasSpecularMap
        {
            get => (SpecularMapTexture != null); 
        }


        public Texture2D DiffuseMapTexture
        {
            get => diffuseMapTexture; 
            set => diffuseMapTexture = value; 
        }


        public Texture2D NormalMapTexture
        {
            get => normalMapTexture; 
            set => normalMapTexture = value; 
        }


        public Texture2D SpecularMapTexture
        {
            get => specularMapTexture; 
            set => specularMapTexture = value; 
        }


        public dvec4 DiffuseAmount
        { 
            get => diffuseAmount; 
            set => diffuseAmount = value; 
        }


        public dvec3 SpecularAmount
        {
            get => specularAmount.rgb; 
            set => specularAmount.rgb = value; 
        }


        public dvec3 EmissiveAmount
        {
            get => emissiveAmount; 
            set => emissiveAmount = value; 
        }


        public double SpecularExponent
        {
            get => specularAmount.a; 
            set => specularAmount.a = value; 
        }


        public bool NormalMappingEnabled
        {
            get => normalMappingEnabled; 
            set => normalMappingEnabled = value; 
        }


        //protected override void InitUniformLocations()
        //{                     
        //    uniformLocations["bHasDiffuseMap"]      = GL.GetUniformLocation(Shader.ProgramHandle, "bHasDiffuseMap");          
        //    uniformLocations["bHasSpecularMap"]     = GL.GetUniformLocation(Shader.ProgramHandle, "bHasSpecularMap");
        //    uniformLocations["bHasNormalMap"]       = GL.GetUniformLocation(Shader.ProgramHandle, "bHasNormalMap");

        //    uniformLocations["texDiffuseMap"]       = GL.GetUniformLocation(Shader.ProgramHandle, "texDiffuseMap");
        //    uniformLocations["texNormalMap"]        = GL.GetUniformLocation(Shader.ProgramHandle, "texNormalMap");
        //    uniformLocations["texSpecularMap"]      = GL.GetUniformLocation(Shader.ProgramHandle, "texSpecularMap");

        //    uniformLocations["vMaterialDiffuse"]    = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialDiffuse");
        //    uniformLocations["vMaterialEmissive"]   = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialEmissive");
        //    uniformLocations["vMaterialSpecular"]   = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialSpecular");

        //    base.InitUniformLocations();
        //}


        //public override void Bind(ref uint textureUnit)
        //{
        //    base.Bind(ref textureUnit);

        //    GL.Uniform1(uniformLocations["bHasDiffuseMap"], HasDiffuseMap ? 1 : 0);
        //    GL.Uniform1(uniformLocations["bHasNormalMap"], (HasNormapMap && normalMappingEnabled) ? 1 : 0);
        //    GL.Uniform1(uniformLocations["bHasSpecularMap"], HasSpecularMap ? 1 : 0);

        //    GL.Uniform3(uniformLocations["vMaterialEmissive"], 1, emissiveAmount.Values);
        //    GL.Uniform4(uniformLocations["vMaterialDiffuse"], 1, diffuseAmount.Values);      
        //    GL.Uniform4(uniformLocations["vMaterialSpecular"], 1, specularAmount.Values);

        //    if (HasDiffuseMap)
        //    {
        //        GL.Uniform1(uniformLocations["texDiffuseMap"], 0);
        //        diffuseMapTexture.Bind(TextureUnit.Texture0);
        //    }
        //    if (HasSpecularMap)
        //    {
        //        GL.Uniform1(uniformLocations["texSpecularMap"], 1);
        //        specularMapTexture.Bind(TextureUnit.Texture1);
        //    }
        //    if (HasNormapMap)
        //    {
        //        GL.Uniform1(uniformLocations["texNormalMap"], 2);
        //        normalMapTexture.Bind(TextureUnit.Texture2);
        //    }
        //}
    }


    public class DeferredLightMaterial : Material
    {
        // GBuffer
        private Texture2D diffuseAlbedoTexture;
        private Texture2D specularAlbedoTexture;
        private Texture2D worldNormalTexture;
        private Texture2D depthTexture;

        // CameraParams
        private dvec3 cameraPosition;
        private dmat4 inverseViewProjectionMatrix;

        // Light Params
        private dvec3 lightDirection;
        private dvec3 lightPosition;
        private dvec3 lightColor;
        private dvec3 lightAttenuation;
        private int lightType;
        
        // ShadowMappingParams
        private dmat4 lightViewProjectionMatrix;
        private Texture shadowMapTexture;

        public override void OnStart()
        {
            Shader = Shader.Find("deferredLightingPass");
            base.OnStart();
        }


        public Texture2D WorldNormalTexture
        {
            get => worldNormalTexture; 
            set => worldNormalTexture = value;
        }


        public Texture2D DiffuseAlbedoTexture
        {
            get => diffuseAlbedoTexture;
            set => diffuseAlbedoTexture = value;
        }


        public Texture2D SpecularAlbedoTexture
        {
            get => specularAlbedoTexture;
            set => specularAlbedoTexture = value;
        }


        public dvec3 LightDirection
        {
            get => lightDirection;
            set => lightDirection = value;
        }


        public dvec3 LightPosition
        {
            get => lightPosition;
            set => lightPosition = value;
        }


        public bool HasShadowMapTexture
        {
            get => shadowMapTexture != null; 
        }


        public dvec3 LightColor
        {
            get => lightColor; 
            set => lightColor = value;
        }


        public dvec3 LightAttenuation
        {
            get =>lightAttenuation;
            set =>lightAttenuation = value;
        }


        public int LightType
        {
            get => lightType;
            set => lightType = value;
        }


        public dmat4 InverseViewProjectionMatrix
        {
            get => inverseViewProjectionMatrix;
            set => inverseViewProjectionMatrix = value;
        }


        public Texture2D DepthTexture
        {
            get => depthTexture;
            set => depthTexture = value;
        }


        public dmat4 LightViewProjectionMatrix
        {
            get => lightViewProjectionMatrix;
            set => lightViewProjectionMatrix = value;
        }


        public Texture ShadowMapTexture
        {
            get => shadowMapTexture;
            set  => shadowMapTexture = value;
        }
        

        //protected override void InitUniformLocations()
        //{
        //    base.InitUniformLocations();

           
        //    uniformLocations["texDiffuseAlbedo"]    = GL.GetUniformLocation(Shader.ProgramHandle, "texDiffuseAlbedo");
        //    uniformLocations["texSpecularAlbedo"]   = GL.GetUniformLocation(Shader.ProgramHandle, "texSpecularAlbedo");
        //    uniformLocations["texWorldNormal"]      = GL.GetUniformLocation(Shader.ProgramHandle, "texWorldNormal");
        //    uniformLocations["texWorldPosition"]    = GL.GetUniformLocation(Shader.ProgramHandle, "texWorldPosition");
        //    uniformLocations["texDepth"]            = GL.GetUniformLocation(Shader.ProgramHandle, "texDepth");

            
        //    uniformLocations["mInvViewProj"]        = GL.GetUniformLocation(Shader.ProgramHandle, "mInvViewProj");
        //    uniformLocations["vCameraPosition"]     = GL.GetUniformLocation(Shader.ProgramHandle, "vCameraPosition");
        //    uniformLocations["vViewportSize"]       = GL.GetUniformLocation(Shader.ProgramHandle, "vViewportSize");

        //    uniformLocations["vLightDirection"]     = GL.GetUniformLocation(Shader.ProgramHandle, "vLightDirection");
        //    uniformLocations["vLightPosition"]      = GL.GetUniformLocation(Shader.ProgramHandle, "vLightPosition");

        //    uniformLocations["vLightColor"]         = GL.GetUniformLocation(Shader.ProgramHandle, "vLightColor");
        //    uniformLocations["vLightAttenuation"]   = GL.GetUniformLocation(Shader.ProgramHandle, "vLightAttenuation");
        //    uniformLocations["iLightType"]          = GL.GetUniformLocation(Shader.ProgramHandle, "iLightType");

        //    uniformLocations["mLightBiasVP"]        = GL.GetUniformLocation(Shader.ProgramHandle, "mLightBiasVP");
        //    uniformLocations["bHasShadowMap"]       = GL.GetUniformLocation(Shader.ProgramHandle, "bHasShadowMap");
        //    uniformLocations["texShadowMap"]        = GL.GetUniformLocation(Shader.ProgramHandle, "texShadowMap");
        //    uniformLocations["iShadowMapSize"]      = GL.GetUniformLocation(Shader.ProgramHandle, "iShadowMapSize");



        //}


        //public override void Bind(ref uint textureUnit)
        //{
            
        //    Shader.bind();

        //    GL.Uniform1(uniformLocations["texDiffuseAlbedo"], 0);
        //    diffuseAlbedoTexture.Bind(TextureUnit.Texture0);

        //    GL.Uniform1(uniformLocations["texSpecularAlbedo"], 1);
        //    specularAlbedoTexture.Bind(TextureUnit.Texture1);

        //    GL.Uniform1(uniformLocations["texWorldNormal"], 2);
        //    worldNormalTexture.Bind(TextureUnit.Texture2);

        //    GL.Uniform1(uniformLocations["texDepth"], 3);
        //    depthTexture.Bind(TextureUnit.Texture3);

        //    GL.Uniform3(uniformLocations["vCameraPosition"], 1, cameraPosition.Values);

        //    GL.UniformMatrix4(uniformLocations["mInvViewProj"], 1, false, inverseViewProjectionMatrix.Values1D);

        //    GL.Uniform1(uniformLocations["iLightType"], lightType);

        //    GL.Uniform3(uniformLocations["vLightDirection"], 1, lightDirection.Values);
        //    GL.Uniform3(uniformLocations["vLightPosition"], 1, lightPosition.Values);            
        //    GL.Uniform3(uniformLocations["vLightColor"], 1, lightColor.Values);
        //    GL.Uniform3(uniformLocations["vLightAttenuation"], 1, lightAttenuation.Values);


        //    GL.Uniform1(uniformLocations["bHasShadowMap"], (HasShadowMapTexture) ? 1 : 0);

        //    if (HasShadowMapTexture)
        //    {
        //        GL.Uniform1(uniformLocations["texShadowMap"], 5);
        //        shadowMapTexture.Bind(TextureUnit.Texture5);

        //        GL.Uniform1(uniformLocations["iShadowMapSize"], ShadowMapTexture.Height);

        //        var biasMatrix = new dmat4(
        //            0.5f, 0.0f, 0.0f, 0.0f,
        //            0.0f, 0.5f, 0.0f, 0.0f,
        //            0.0f, 0.0f, 0.5f, 0.0f,
        //            0.5f, 0.5f, 0.5f, 1.0f);

        //        var biasLightVP = biasMatrix * lightViewProjectionMatrix;
        //        GL.UniformMatrix4(uniformLocations["mLightBiasVP"], 1, false, biasLightVP.Values1D);
        //    }

        //    base.Bind(ref textureUnit);
        //}
    }
}
