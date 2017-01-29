using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK.Graphics.OpenGL4;


namespace SharpCG
{
    public abstract class Texture : GLComponent
    {     

        protected bool useMipMaps;      


        public abstract int Width
        {
            get;
        }


        public abstract int Height
        {
            get;
        }


        public abstract int Handle
        {
            get;
        }

        
        public abstract void Bind(TextureUnit unit);
       

        public virtual bool UseMipMaps
        {
            get { return useMipMaps; }
        }


        //private static List<Texture> allTextures = new List<Texture>();


        //public static List<Texture> All
        //{
        //    get { return allTextures; }
        //}
    }
}
