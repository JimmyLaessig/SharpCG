using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

using SharpCG.Core;

namespace SharpCG.Effects
{
    public class SkyboxMaterial : Material
    {

        private TextureCubeMap cubeMapTexture;


        [Uniform(Name = "texCubeMap")]
        public TextureCubeMap CubeMapTexture
        {
            get{return cubeMapTexture;}
            set{cubeMapTexture = value;}
        }

        public override void OnStart()
        {
            this.shader = Shader.Find("skybox");
            base.OnStart();
        }
       
    }
}
