using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace SharpCG.Core
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

        
        public abstract void Bind(OpenTK.Graphics.OpenGL4.TextureUnit unit);
       

        public virtual bool UseMipMaps
        {
            get => useMipMaps;
        }
    }
}
