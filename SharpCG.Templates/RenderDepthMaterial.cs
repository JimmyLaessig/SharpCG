using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

using SharpCG.Core;


namespace SharpCG.Templates
{

    public class RenderDepthMaterial : Material
    {

        private Texture2D depthTexture;


        public override void OnStart()
        {

            shader = Shader.Find("textureToDepth");

            base.OnStart();
        }


        [Uniform(Name = "texDepth")]
        public Texture2D DepthTexture
        {
            get => depthTexture;
            set => depthTexture = value;
        }
    }
}
