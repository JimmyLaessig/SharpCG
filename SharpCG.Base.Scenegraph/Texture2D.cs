using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;

using System.Runtime.InteropServices;
using SharpCG;

namespace SharpCG.Base.Scenegraph
{

    public class Texture2D : Texture
    {
        private Image image;
        private int handle;


        public Image Image
        {
            get { return image; }
            set
            {
                image   = value;
                isDirty = true;
            }
        }


        public override uint Width
        {
            get{ return image.Width; }
        }


        public override uint Height
        {
            get{ return image.Height; }
        }


        public override int Handle
        {
            get{return handle;}
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

          
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)Width, (int)Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, image.Data);

            if (UseMipMaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            TextureMinFilter textureMinFilter = (UseMipMaps) ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear;
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


        public static Texture2D Empty(uint width, uint height, bool useMipMaps = false)
        {
            Texture2D texture = new Texture2D();
            texture.Image       = new Image(width, height);
            texture.isDirty     = true;
            texture.Name        = "empty";
            texture.useMipMaps  = useMipMaps;
            texture.isDirty     = true;
            return texture;
        }


        public static Texture2D Load(string path, bool useMipMaps = true)
        {

            Texture2D texture = new Texture2D();
            texture.Image       = Image.FromFile(path);
            texture.useMipMaps  = useMipMaps;
            texture.Name        = path;
            texture.isDirty     = true;

            return texture;
        }        
    }
}
