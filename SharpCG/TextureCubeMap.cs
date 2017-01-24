using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;

namespace SharpCG
{

    public enum CubeMapFace
    {
        PosX = TextureTarget.TextureCubeMapPositiveX,
        NegX = TextureTarget.TextureCubeMapNegativeX,
        PosY = TextureTarget.TextureCubeMapPositiveY,
        NegY = TextureTarget.TextureCubeMapNegativeY,
        PosZ = TextureTarget.TextureCubeMapPositiveZ,
        NegZ = TextureTarget.TextureCubeMapNegativeZ
    }

    public class TextureCubeMap : Texture
    {
        private int handle;
        private Dictionary<CubeMapFace, Image> images;


        private TextureCubeMap()
        {
            images = new Dictionary<CubeMapFace, Image>();
        }


        public void SetImage(Image image, CubeMapFace face)
        {
            // TODO Assert images have same size
            images[face] = image;
        }


        public override uint Width
        {
            get
            {
                if (images.Count > 0) return images.First().Value.Width;            
                else return 0;                  
            }
        }


        public override uint Height
        {
            get
            {
                if (images.Count > 0) return images.First().Value.Height;
                else return 0;
            }
        }


        public override int Handle
        {
            get{return handle;}
        }


        public override void Bind(TextureUnit unit)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }
      

        public override void DeInitGL()
        {
            if (handle > -1) GL.DeleteTexture(handle);
            handle = -1;
        }


        public override void InitGL()
        {
            DeInitGL();

            GL.GenTextures(1, out handle);
            GL.BindTexture(TextureTarget.TextureCubeMap, handle);

            images.ToList().ForEach(pair =>            
                GL.TexImage2D((TextureTarget)pair.Key, 
                0, 
                PixelInternalFormat.Rgba, 
                (int)pair.Value.Width,
                (int)pair.Value.Width,
                0, 
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, 
                PixelType.UnsignedByte, 
                pair.Value.Data)
            );         

            if (UseMipMaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            TextureMinFilter textureMinFilter = (UseMipMaps) ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear;

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            isDirty = false;
        }


        public override void Dispose()
        {
            DeInitGL();
        }


        public static TextureCubeMap Load(  string posXPath,
                                            string negXPath,
                                            string posYPath,
                                            string negYPath,
                                            string posZPath,
                                            string negZPath,
                                            bool useMipmaps = false)
        {
            TextureCubeMap texture = new TextureCubeMap();

            texture.SetImage(Image.FromFile(posXPath), CubeMapFace.PosX);
            texture.SetImage(Image.FromFile(negXPath), CubeMapFace.NegX);
            texture.SetImage(Image.FromFile(posYPath), CubeMapFace.PosY);
            texture.SetImage(Image.FromFile(negYPath), CubeMapFace.NegY);
            texture.SetImage(Image.FromFile(posZPath), CubeMapFace.PosZ);
            texture.SetImage(Image.FromFile(negZPath), CubeMapFace.NegZ);

            texture.Name        = posXPath;
            texture.isDirty     = true;
            texture.useMipMaps  = useMipmaps;

            return texture;
        }      
    }
}