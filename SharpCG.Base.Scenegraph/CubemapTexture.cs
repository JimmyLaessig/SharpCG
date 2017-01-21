using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;

namespace SharpCG.Base.Scenegraph
{
    public class CubeMapTexture : Texture
    {
        private Bitmap posX;
        private Bitmap negX;
        private Bitmap posY;
        private Bitmap negY;
        private Bitmap posZ;
        private Bitmap negZ;



        public override void Bind(TextureUnit unit)
        {
            //GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.TextureCubeMap, Handle);
        }
      
        public override void FreeGPUResources()
        {
            if (handle > -1) GL.DeleteTexture(handle);
            handle = -1;
        }

        public override void UpdateGPUResources()
        {
            FreeGPUResources();

            GL.GenTextures(1, out handle);
            GL.BindTexture(TextureTarget.TextureCubeMap, handle);

            BitmapData data;

            data = posX.LockBits(new Rectangle(0, 0, posX.Width, posX.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            posX.UnlockBits(data);

            data = negX.LockBits(new Rectangle(0, 0, negX.Width, negX.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeX, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            negX.UnlockBits(data);

            data = posY.LockBits(new Rectangle(0, 0, posY.Width, posY.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveY, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            posY.UnlockBits(data);

            data = negY.LockBits(new Rectangle(0, 0, negY.Width, negY.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeY, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            negY.UnlockBits(data);

            data = posZ.LockBits(new Rectangle(0, 0, posZ.Width, posZ.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveZ, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            posZ.UnlockBits(data);

            data = negZ.LockBits(new Rectangle(0, 0, negZ.Width, negZ.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeZ, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            negZ.UnlockBits(data);

            if (useMipmaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            TextureMinFilter textureMinFilter = (useMipmaps) ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear;

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            isDirty = false;
        }

        public override void Dispose()
        {
            FreeGPUResources();
        }


        public static CubeMapTexture Load(  string posXPath,
                                            string negXPath,
                                            string posYPath,
                                            string negYPath,
                                            string posZPath,
                                            string negZPath,
                                            bool useMipmaps = false)
        {


            Bitmap posX = Bitmap.FromFile(posXPath) as Bitmap;
            Bitmap negX = Bitmap.FromFile(negXPath) as Bitmap;
            Bitmap posY = Bitmap.FromFile(posYPath) as Bitmap;
            Bitmap negY = Bitmap.FromFile(negYPath) as Bitmap;
            Bitmap posZ = Bitmap.FromFile(posZPath) as Bitmap;
            Bitmap negZ = Bitmap.FromFile(negZPath) as Bitmap;


            CubeMapTexture texture = new CubeMapTexture();

            texture.width = posX.Width;
            texture.height = posX.Height;

            texture.posX = posX;
            texture.negX = negX;
            texture.posY = posY;
            texture.negY = negY;
            texture.posZ = posZ;
            texture.negZ = negZ;

            texture.Name = posXPath;
            texture.isDirty = true;
            texture.useMipmaps = useMipmaps;
            return texture;
        }
    }
}
