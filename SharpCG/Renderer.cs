using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SharpCG
{
    public abstract class Renderer : GLComponent
    {
        protected Framebuffer framebuffer;
        protected RenderPass renderPass;


        public virtual Framebuffer Framebuffer
        {
            get { return framebuffer; } 
            set { framebuffer = value; }
        }


        public RenderPass RenderPass
        {
            get { return renderPass; }
            set { renderPass = value; }
        }


        /// <summary>
        /// Method to execute OpenGL render commands. This will be called once per frame. 
        /// </summary>
        public abstract void RenderGL();

    
    }
}