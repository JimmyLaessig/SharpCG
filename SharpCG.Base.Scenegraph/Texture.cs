using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpCG.Base.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG.Base.Scenegraph
{
    public abstract class Texture : GLObject
    {
        protected int handle = -1;
        protected int width;
        protected int height;

        protected bool useMipmaps;

        protected bool isDirty;

        private string name;

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

        public bool IsDirty()
        {
            return isDirty;
        }

        public string Name
        {
            get{ return name; }
            set { name = value; }
        }

        public bool UseMipmaps
        {
            get
            {
                return useMipmaps;
            }

            set
            {
                useMipmaps = value;
            }
        }

        public abstract void Bind(TextureUnit unit);
        public abstract void UpdateGPUResources();
        public abstract void FreeGPUResources();
        public abstract void Dispose();
    }
}
