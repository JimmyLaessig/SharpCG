using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

using SharpCG.Core;

namespace SharpCG.Effects
{
    public class TextureToDepthMaterial : Material
    {

        private Texture2D depthTexture;


        public override void OnStart()
        {
            
            shader = Shader.Find("textureToDepth");

            base.OnStart();
        }


        //protected override void InitUniformLocations()
        //{
        //    shader.bind();

        //    uniformLocations["texDepth"] = GL.GetUniformLocation(shader.ProgramHandle, "texDepth");
        //    base.InitUniformLocations();
        //}


        //public override void Bind(ref uint textureUnit)
        //{
        //    shader.bind();

        //    GL.Uniform1(uniformLocations["texDepth"], 0);
        //    depthTexture.Bind(TextureUnit.Texture0);
            

        //    base.Bind(ref textureUnit);
        //}


        public Texture2D DepthTexture
        {
            get{return depthTexture;}
            set{depthTexture = value;}
        }
    }
}
