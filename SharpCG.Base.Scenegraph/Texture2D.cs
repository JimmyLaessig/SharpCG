using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace SharpCG.Base.Scenegraph
{
    public class Texture2D : Texture
    {
        private Bitmap bitmap;

        public Bitmap Data
        {
            get { return bitmap; }
            set
            {
                bitmap = value;
                isDirty = true;
            }
        }

        public override void Bind(TextureUnit unit)
        {           
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
             
        public override void UpdateGPUResources()
        {
            FreeGPUResources();          

            GL.GenTextures(1, out handle);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            BitmapData data;
            data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);


            if (useMipmaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            TextureMinFilter textureMinFilter = (useMipmaps) ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear;
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            isDirty = false;
        }

        public override void FreeGPUResources()
        {
            if(handle > -1) GL.DeleteTexture(handle);           
        }

        public override void Dispose()
        {
            FreeGPUResources();
        }


        public static Texture2D Load(string path, bool genMipMaps = true)
        {
            Bitmap bmp = Bitmap.FromFile(path) as Bitmap;

            Texture2D texture = new Texture2D();

            texture.width = bmp.Width;
            texture.height = bmp.Height;
            texture.bitmap = bmp;
            texture.isDirty = true;
            texture.useMipmaps = genMipMaps;
            texture.Name = path;
            return texture;
        }        
    }
}
