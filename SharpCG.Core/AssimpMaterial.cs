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
            get => (diffuseMapTexture != null);
        }


        [Uniform(Name = "bHasNormalMap")]
        public bool HasNormapMap
        {
            get => (normalMapTexture != null); 
        }


        [Uniform(Name = "bHasSpecularMap")]
        public bool HasSpecularMap
        {
            get => (SpecularMapTexture != null); 
        }


        [Uniform(Name = "bHasDisplacementMap")]
        public bool HasDisplacementMap
        {
            get => (displacementMapTexture != null); 
        }


        public bool IsTransparent
        {
            get => (diffuseAmount.a >= 1.0); 
        }


        [Uniform(Name = "texDiffuseMap")]
        public Texture2D DiffuseMapTexture
        {
            get => diffuseMapTexture;
            set => diffuseMapTexture = value; 
        }


        [Uniform(Name = "texNormalMap")]
        public Texture2D NormalMapTexture
        {
            get => normalMapTexture;
            set => normalMapTexture = value; 
        }


        [Uniform(Name = "texSpecularMap")]
        public Texture2D SpecularMapTexture
        {
            get => specularMapTexture;
            set => specularMapTexture = value; 
        }


        [Uniform(Name = "texDisplacementMap")]
        public Texture2D DisplacementMapTexture
        {
            get => displacementMapTexture;
            set => displacementMapTexture = value; 
        }


        [Uniform(Name = "vMaterialDiffuse")]
        public dvec4 DiffuseAmount
        {
            get => diffuseAmount;
            set => diffuseAmount = value; 
        }


        [Uniform(Name = "vMaterialSpecular")]
        public dvec4 SpecularAmount
        {
            get => specularAmount;
            set => specularAmount = value; 
        }


        [Uniform(Name = "vMaterialEmissive")]
        public dvec3 EmissiveAmount
        {
            get => emissiveAmount;
            set => emissiveAmount = value; 
        }


        [Uniform(Name = "texBumpMap")]
        public Texture2D BumpMapTexture
        {
            get => bumpMapTexture;
            set => bumpMapTexture = value;
        }        
    }   
}