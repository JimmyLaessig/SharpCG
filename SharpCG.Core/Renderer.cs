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
        private Camera camera           = Camera.Main;
        private GLState glState         = new GLState();


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


        public GLState GLState
        {
            get => glState;
            set => glState = value;
        }


        public Camera Camera
        {
            get => camera;
            set => camera = value;
        }


        /// <summary>
        /// Method to execute OpenGL render commands. This will be called once per frame. 
        /// </summary>
        public abstract void RenderGL(); 
    }
}