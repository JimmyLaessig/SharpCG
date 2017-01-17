using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG.Base.Scenegraph
{
    public class Texture
    {
        private int handle;
        private Bitmap data;
        private int width;
        private int height;

        public int Width
        {
            get { return width; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Handle
        {
            get{return handle;}
        }

        public Texture()
        {
            handle = -1;
        }

        public void Bind(ref uint unit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + (int)unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public static Texture Load(string path, bool genMipMaps)
        {
            Bitmap bmp = Bitmap.FromFile(path) as Bitmap;
            int handle = -1;

            GL.GenTextures(1, out handle);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            BitmapData data;
            data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            Texture texture = new Texture();

            texture.width   = bmp.Width;
            texture.height  = bmp.Height;
            texture.handle  = handle;
            texture.data    = bmp;
            return texture;
        }
    }
}
