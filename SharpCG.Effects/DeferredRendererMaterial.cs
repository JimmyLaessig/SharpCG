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


    public class DeferredLightMaterial : LightMaterial
    {

        // GBuffer
        private Texture2D diffuseAlbedoTexture;
        private Texture2D specularAlbedoTexture;
        private Texture2D worldNormalTexture;
        private Texture2D depthTexture;

        // CameraParams
        private dmat4 inverseViewProjectionMatrix;


        // ShadowMappingParams
        private Texture shadowMapTexture;


        public override void OnStart()
        {
            Shader = Shader.Find("deferredLightingPass");
            base.OnStart();
        }

        #region GBuffer

        [Uniform(Name = "texWorldNormal")] 
        public Texture2D WorldNormalTexture
        {
            get => worldNormalTexture; 
            set => worldNormalTexture = value;
        }


        [Uniform(Name = "texDiffuseAlbedo")] 
        public Texture2D DiffuseAlbedoTexture
        {
            get => diffuseAlbedoTexture;
            set => diffuseAlbedoTexture = value;
        }


        [Uniform(Name = "texSpecularAlbedo")]
        public Texture2D SpecularAlbedoTexture
        {
            get => specularAlbedoTexture;
            set => specularAlbedoTexture = value;
        }


        [Uniform(Name = "texDepth")]
        public Texture2D DepthTexture
        {
            get => depthTexture;
            set => depthTexture = value;
        }


        [Uniform(Name = "mInvViewProj")]
        public dmat4 InverseViewProjectionMatrix
        {
            get => inverseViewProjectionMatrix;
            set => inverseViewProjectionMatrix = value;
        }

        #endregion


        #region ShadowMapping

        [Uniform(Name = "mLightBiasVP")]
        public dmat4 LightBiasViewProjectionMatrix
        {
            get
            {     
                var biasMatrix = new dmat4(
                    0.5f, 0.0f, 0.0f, 0.0f,
                    0.0f, 0.5f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.5f, 0.0f,
                    0.5f, 0.5f, 0.5f, 1.0f);

                return biasMatrix * Light.ProjectionMatrix * Light.ViewMatrix;
            }
        }


        [Uniform(Name = "texShadowMap")]
        public Texture ShadowMapTexture
        {
            get => shadowMapTexture;
            set  => shadowMapTexture = value;
        }


        [Uniform(Name = "bHasShadowMap")]
        public bool HasShadowMapTexture
        {
            get => shadowMapTexture != null;
        }
        #endregion
    }
}
