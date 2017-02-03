using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

using System.Runtime.InteropServices;
using SharpCG;

namespace SharpCG
{

    public class Texture2D : Texture
    {
        private Image image;
        private int handle = -1;


        private PixelInternalFormat internalFormat  = PixelInternalFormat.Rgba;
        private PixelFormat format                  = PixelFormat.Bgra;
        private PixelType type                      = PixelType.UnsignedByte;



        public Image Image
        {
            get { return image; }
            set
            {
                image   = value;
                isDirty = true;
            }
        }


        public override int Width
        {
            get{ return image.Width; }
        }


        public override int Height
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
             
        public override void InitGL()
        {
            DeInitGL();          

            GL.GenTextures(1, out handle);
            GL.BindTexture(TextureTarget.Texture2D, handle);

          
            GL.TexImage2D(TextureTarget.Texture2D, 0, internalFormat, Width, Height, 0, format, type, image.Data);
            var error = GL.GetError();

            if (format == PixelFormat.DepthComponent || format == PixelFormat.DepthStencil)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)TextureCompareMode.CompareRefToTexture);
                //  GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)DepthFunction.Lequal);
            }
            else
            {
                if (UseMipMaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

                TextureMinFilter textureMinFilter = (UseMipMaps) ? TextureMinFilter.LinearMipmapLinear : TextureMinFilter.Linear;
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            isDirty = false;
        }


        public override void DeInitGL()
        {
            if(handle > -1) GL.DeleteTexture(handle);           
        }


        public override void Dispose()
        {
            DeInitGL();
        }


        public static Texture2D Empty(int width, int height, bool useMipMaps = false)
        {
            Texture2D texture = new Texture2D();
            texture.Image       = new Image(width, height);
            texture.isDirty     = true;
            texture.Name        = "empty";
            texture.useMipMaps  = useMipMaps;
            texture.isDirty     = true;

            texture.internalFormat  = PixelInternalFormat.Rgba;
            texture.format          = PixelFormat.Rgba;
            texture.type            = PixelType.UnsignedByte;

        //private PixelInternalFormat internalFormat = PixelInternalFormat.Rgba;
        //private PixelFormat format = PixelFormat.Bgra;
        //private PixelType type = PixelType.UnsignedByte;


            return texture;
        }


        public static Texture2D Depth(int width, int height, bool useStencil = false)
        {
            Texture2D texture = new Texture2D();
            texture.Image = new Image(width, height);
            texture.isDirty = true;

            texture.internalFormat  = (useStencil) ? PixelInternalFormat.Depth24Stencil8 : PixelInternalFormat.DepthComponent;
            texture.format          = (useStencil) ? PixelFormat.DepthStencil : PixelFormat.DepthComponent;
            texture.type            = PixelType.Float;

            texture.Name = "Depth Texture";


            texture.useMipMaps = false;

            return texture;
        }

        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static Texture2D Load(string path, bool useMipMaps = true)
        {
            if (!textures.ContainsKey(path))
            {
                Texture2D texture = new Texture2D();
                texture.Image = Image.FromFile(path);
                texture.useMipMaps = useMipMaps;
                texture.Name = path;
                texture.isDirty = true;

                textures[path] = texture;
            }

            return textures[path];
        }        
    }
}
