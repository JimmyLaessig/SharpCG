using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Scenegraph;
using SharpCG.Base.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG.Demo
{
    class SkyboxMaterial : Material
    {

        private CubeMapTexture cubeMapTexture;

        public CubeMapTexture CubeMapTexture
        {
            get{return cubeMapTexture;}
            set{cubeMapTexture = value;}
        }


        public override void Bind(ref uint textureUnit)
        {
           // base.Bind(ref textureUnit);

            shader.bind();

            GL.UniformMatrix4(uniformLocations["mWVP"], 1, false, this.WvpMatrix.Values1D);
            GL.Uniform1(uniformLocations["texCubeMap"], 0);
            cubeMapTexture.Bind(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0);
        }

        public override List<Texture> GetMaterialTextures()
        {
            var ls = base.GetMaterialTextures();
            if(cubeMapTexture != null) ls.Add(cubeMapTexture);
            return ls;
        }    

        protected override void InitUniformLocations()
        {
            Shader = Shader.Find("skybox");

            base.InitUniformLocations();
            uniformLocations["texCubeMap"] = GL.GetUniformLocation(Shader.ProgramHandle, "texCubeMap");      
        }
    }
}
