using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Core;
namespace SharpCG.Effects.Postprocessing
{
    public class FXAARenderer : Renderer
    {
        private Geometry fullscreenQuad;

        public override void OnStart()
        {
            fullscreenQuad = GeometryExtensions.FullscreenQuad;

            base.OnStart();
        }

        public override void RenderGL()
        {
                
        }
    }
}
