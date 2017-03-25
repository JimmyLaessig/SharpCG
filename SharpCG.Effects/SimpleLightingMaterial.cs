using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using OpenTK;
using GlmSharp;

using SharpCG.Core;

namespace SharpCG.Effects
{
    /// <summary>
    /// 
    /// </summary>
    public class SimpleLightingMaterial : Material
    {
        private Texture2D diffuseMapTexture;
        private Texture2D normalMapTexture;
        private Texture2D specularMapTexture;

        private dvec4 diffuseAmount = new vec4(1, 1, 1, 1);
        private dvec3 emissiveAmount = new dvec3(0, 0, 0);
        private dvec4 specularAmount = new vec4(1, 1, 1, 128);

        private dvec3 viewPosition = new dvec3(0, 0, 0);
        private dvec3 lightColor = new dvec3(1, 1, 1);
        private dvec3 lightAmbientColor = new dvec3(1, 1, 1);
        private dvec3 lightDirection = new dvec3(1, 0, 0);


        private bool normalMappingEnabled = false;


        public override void OnStart()
        {
            Shader = Shader.Find("simpleLighting");

            base.OnStart();
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


        public dvec3 ViewPosition
        {
            get => viewPosition;
            set => viewPosition = value;
        }


        public dvec3 LightColor
        {
            get => lightColor;
            set => lightColor = value;
        }


        public dvec3 LightAmbientColor
        {
            get => lightAmbientColor;
            set => lightAmbientColor = value;
        }


        public dvec3 LightDirection
        {
            get => lightDirection;
            set => lightDirection = value;
        }      
    }
}