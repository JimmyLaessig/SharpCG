using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SharpCG.Core
{
    public abstract class Renderer : GLComponent
    {
        protected Framebuffer framebuffer;
        protected RenderPass renderPass = RenderPass.Main;

        private GLState glState;

        public virtual Framebuffer Framebuffer
        {
            get => framebuffer; 
            set => framebuffer = value;
        }


        public RenderPass RenderPass
        {
            get => renderPass; 
            set => renderPass = value; 
        }


        protected GLState GlState
        {
            get => glState;
            set => glState = value;
        }


        /// <summary>
        /// Method to execute OpenGL render commands. This will be called once per frame. 
        /// </summary>
        public abstract void RenderGL(); 
    }
}