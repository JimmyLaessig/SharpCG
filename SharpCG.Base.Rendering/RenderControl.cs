using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics;

namespace SharpCG.Base.Rendering
{ 

    public class RenderObject
    {
        public RenderObject(Renderable renderable, string name)
        {
            this.renderable = renderable;
            this.name       = name;
        }

        public long id;
        public string name;
        public Renderable renderable;
    }

    public class RenderControl
    {    

        private Rectangle viewport;
        private Color4 clearColor;
        private double clearDepth;
        private int clearStencil;

        public bool ClearColorFlag;
        public bool ClearDepthFlag;
        public bool ClearStencilFlag;

        public bool isDirty;


        public List<RenderObject> renderObjects = new List<RenderObject>();

        public Rectangle Viewport
        {
            get {return viewport;}
            set { isDirty = true; viewport = value;}
        }

        public Color4 ClearColor
        {
            get {return clearColor;}
            set { isDirty = true; clearColor = value;}
        }

        public double ClearDepth
        {
            get { return clearDepth;}
            set { isDirty = true; clearDepth = value;}
        }

        public int ClearStencil
        {
            get {return clearStencil;}
            set{isDirty = true;clearStencil = value;}
        }
    }
}
