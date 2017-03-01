using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;


namespace SharpCG.Core
{

    public class AssimpMaterial : Material
    {
        private Texture2D diffuseMapTexture;
        private Texture2D specularMapTexture;
        private Texture2D normalMapTexture;
        private Texture2D displacementMapTexture;
        private Texture2D bumpMapTexture;

        private dvec3 emissiveAmount;
        private dvec4 diffuseAmount;
        private dvec4 specularAmount;


        public override void OnStart()
        {          
            shader = Shader.Find("simpleTextured");
            base.OnStart();
        }

        [Uniform(Name = "bHasDiffuseMap")]
        public bool HasDiffuseMap
        {
            get { return (diffuseMapTexture != null); }
        }

        [Uniform(Name = "bHasNormalMap")]
        public bool HasNormapMap
        {
            get { return (normalMapTexture != null); }
        }

        [Uniform(Name = "bHasSpecularMap")]
        public bool HasSpecularMap
        {
            get { return (SpecularMapTexture != null); }
        }

        [Uniform(Name = "bHasDisplacementMap")]
        public bool HasDisplacementMap
        {
            get { return (displacementMapTexture != null); }
        }


        public bool IsTransparent
        {
            get { return (diffuseAmount.a >= 1.0); }
        }

        [Uniform(Name = "texDiffuseMap")]
        public Texture2D DiffuseMapTexture
        {
            get { return diffuseMapTexture; }
            set { diffuseMapTexture = value; }
        }

        [Uniform(Name = "texNormalMap")]
        public Texture2D NormalMapTexture
        {
            get { return normalMapTexture; }
            set { normalMapTexture = value; }
        }

        [Uniform(Name = "texSpecularMap")]
        public Texture2D SpecularMapTexture
        {
            get { return specularMapTexture; }
            set { specularMapTexture = value; }
        }

        [Uniform(Name = "texDisplacementMap")]
        public Texture2D DisplacementMapTexture
        {
            get { return displacementMapTexture; }
            set { displacementMapTexture = value; }
        }

        [Uniform(Name = "vMaterialDiffuse")]
        public dvec4 DiffuseAmount
        {
            get { return diffuseAmount; }
            set { diffuseAmount = value; }
        }

        [Uniform(Name = "vMaterialSpecular")]
        public dvec4 SpecularAmount
        {
            get { return specularAmount; }
            set { specularAmount = value; }
        }

        [Uniform(Name = "vMaterialEmissive")]
        public dvec3 EmissiveAmount
        {
            get { return emissiveAmount; }
            set { emissiveAmount = value; }
        }


        [Uniform(Name = "texBumpMap")]
        public Texture2D BumpMapTexture
        {
            get{ return bumpMapTexture;}
            set{bumpMapTexture = value;}
        }


        //protected override void InitUniformLocations()
        //{
        //    uniformLocations["bHasDiffuseMap"]  = GL.GetUniformLocation(Shader.ProgramHandle, "bHasDiffuseMap");
        //    uniformLocations["bHasSpecularMap"] = GL.GetUniformLocation(Shader.ProgramHandle, "bHasSpecularMap");
        //    uniformLocations["bHasNormalMap"]   = GL.GetUniformLocation(Shader.ProgramHandle, "bHasNormalMap");
        //    uniformLocations["bHasBumpMap"]     = GL.GetUniformLocation(Shader.ProgramHandle, "bHasBumpMap");

        //    uniformLocations["texDiffuseMap"]   = GL.GetUniformLocation(Shader.ProgramHandle, "texDiffuseMap");
        //    uniformLocations["texNormalMap"]    = GL.GetUniformLocation(Shader.ProgramHandle, "texNormalMap");
        //    uniformLocations["texSpecularMap"]  = GL.GetUniformLocation(Shader.ProgramHandle, "texSpecularMap");
        //    uniformLocations["texBumpMap"]      = GL.GetUniformLocation(Shader.ProgramHandle, "texBumpMap");

        //    uniformLocations["vMaterialDiffuse"]    = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialDiffuse");
        //    uniformLocations["vMaterialEmissive"]   = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialEmissive");
        //    uniformLocations["vMaterialSpecular"]   = GL.GetUniformLocation(Shader.ProgramHandle, "vMaterialSpecular");

        //    base.InitUniformLocations();
        //}


        //public override void Bind(ref uint textureUnit)
        //{
        //    base.Bind(ref textureUnit);

        //    GL.Uniform1(uniformLocations["bHasDiffuseMap"], HasDiffuseMap ? 1 : 0);
        //    GL.Uniform1(uniformLocations["bHasNormalMap"], HasNormapMap ? 1 : 0);
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
}