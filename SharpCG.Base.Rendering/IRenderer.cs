using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Rendering;

namespace SharpCG.Base.Rendering
{
    public interface IRenderer
    {

        Framebuffer Framebuffer
        {
            get;     
        }

        RenderPass RenderPass
        {
            get;
        }
        
        void Render();
    }
}