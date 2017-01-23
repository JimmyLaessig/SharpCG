using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SharpCG.Base.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG.Base.Rendering
{
    public abstract class Texture : GLObject
    {     

        protected bool isDirty;
        protected bool useMipMaps;
        protected string name;

        protected Texture()
        {
            allTextures.Add(this);
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public abstract uint Width
        {
            get;
        }


        public abstract uint Height
        {
            get;
        }


        public abstract int Handle
        {
            get;
        }

        
        public abstract void Bind(TextureUnit unit);


        public abstract void InitGL();


        public virtual void AfterUpdateGPUResources() {}

        public abstract void DeInitGL();


        public abstract void Dispose();


        public virtual bool UseMipMaps
        {
            get { return useMipMaps; }
        }


        public virtual bool IsDirty
        {
            get { return isDirty; }
        }


        private static List<Texture> allTextures = new List<Texture>();


        public static List<Texture> All
        {
            get { return allTextures; }
        }
    }
}
