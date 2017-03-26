using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Core;
using GlmSharp;

namespace SharpCG.Effects
{
    public class PostprocessingMaterial : Material
    {

        private Texture2D depthTexture;
        private Texture2D normalTexture;
        private Texture2D colorTexture;
        private vec2 viewportSize;


        public override void OnStart()
        {
            shader = Shader.Find("postprocessingFXAA");
            base.OnStart();
        }


        [Uniform(Name = "texDepth")]
        public Texture2D DepthTexture
        {
            get => depthTexture;
            set => depthTexture = value;
        }


        [Uniform(Name = "texColor")]
        public Texture2D ColorTexture
        {
            get =>colorTexture;
            set =>colorTexture = value;
        }


        [Uniform(Name = "texNormal")]
        public Texture2D NormalTexture
        {
            get => normalTexture; 
            set => normalTexture = value;
        }
    }
}
