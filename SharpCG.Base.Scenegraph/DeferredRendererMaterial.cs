using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Rendering;
using SharpCG.Base.Scenegraph;
using GlmSharp;
using OpenTK.Graphics.OpenGL4;


namespace SharpCG.Base.Scenegraph
{

    public class GeometryPassMaterial : Material
    {

        private Texture2D diffuseMapTexture;
        private Texture2D normalMapTexture;
        private Texture2D specularMapTexture;



        private vec3 emissiveAmount     = new vec3(0, 0, 0);
        private vec4 diffuseAmount      = new vec4(1, 1, 1, 1);
        private vec4 specularAmount     = new vec4(1, 1, 1, 128);

        private bool normalMappingEnabled = false;


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
            Shader = Shader.Find("deferredGeometryPass");
           
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


            GL.Uniform1(uniformLocations["texDiffuseMap"], 0);
            diffuseMapTexture.Bind(TextureUnit.Texture0);


            GL.Uniform1(uniformLocations["texNormalMap"], 1);
            normalMapTexture.Bind(TextureUnit.Texture1);


            GL.Uniform1(uniformLocations["texSpecularMap"], 2);
            specularMapTexture.Bind(TextureUnit.Texture2);

        }
    }


    public class LightingPassMaterial : Material
    {

    }
}
