using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using GlmSharp;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG
{
    public static class MaterialExtensions
    {

        public static SimpleLightingMaterial Create(Assimp.Material aiMaterial, string directory)
        {
            
            SimpleLightingMaterial material = new SimpleLightingMaterial();

            if (aiMaterial.HasColorDiffuse)
            {
                material.DiffuseAmount = new vec4(aiMaterial.ColorDiffuse.R, aiMaterial.ColorDiffuse.G, aiMaterial.ColorDiffuse.B, aiMaterial.ColorDiffuse.A);
            }
            if (aiMaterial.HasOpacity)
            {
                material.DiffuseAmount = new vec4(material.DiffuseAmount.rgb, aiMaterial.Opacity);
            }
            if (aiMaterial.HasShininess)
            {
                material.SpecularExponent = aiMaterial.Shininess;
            }
            if (aiMaterial.HasTextureDiffuse)
            {
                material.DiffuseMapTexture = Texture2D.Load(directory + "/" + aiMaterial.TextureDiffuse.FilePath, true);
            }
            if (aiMaterial.HasTextureNormal)
            {
                material.NormalMapTexture = Texture2D.Load(directory + "/" + aiMaterial.TextureNormal.FilePath, true);
            }
            if (aiMaterial.HasTextureSpecular)
            {
                material.SpecularMapTexture = Texture2D.Load(directory + "/" + aiMaterial.TextureSpecular.FilePath, true);
            }

            return material;
        }

        public static GeometryPassMaterial Create2(Assimp.Material aiMaterial, string directory)
        {
            GeometryPassMaterial material = new GeometryPassMaterial();

            if (aiMaterial.HasColorDiffuse)
            {
                material.DiffuseAmount = new vec4(aiMaterial.ColorDiffuse.R, aiMaterial.ColorDiffuse.G, aiMaterial.ColorDiffuse.B, aiMaterial.ColorDiffuse.A);
            }
            if (aiMaterial.HasOpacity)
            {
                material.DiffuseAmount = new vec4(material.DiffuseAmount.rgb, aiMaterial.Opacity);
            }
            if (aiMaterial.HasShininess)
            {
                material.SpecularExponent = aiMaterial.Shininess;
            }
            if (aiMaterial.HasTextureDiffuse)
            {
                material.DiffuseMapTexture = Texture2D.Load(directory + "/" + aiMaterial.TextureDiffuse.FilePath, true);
            }
            if (aiMaterial.HasTextureNormal)
            {
                material.NormalMapTexture = Texture2D.Load(directory + "/" + aiMaterial.TextureNormal.FilePath, true);
            }
            if (aiMaterial.HasTextureSpecular)
            {
                material.SpecularMapTexture = Texture2D.Load(directory + "/" + aiMaterial.TextureSpecular.FilePath, true);
            }

            return material;
        }

    }
}
