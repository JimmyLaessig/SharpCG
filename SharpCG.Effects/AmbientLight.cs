using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;


using SharpCG.Core;

namespace SharpCG.Effects
{
    public class AmbientLight : Light
    {
        private Geometry fullscreenQuad;

        public override void OnStart()
        {
            fullscreenQuad = GeometryExtensions.FullscreenQuad;

        }

        public override dvec3 Attenuation
        {
            get => dvec3.Zero;
            set { }
        }


        public override int LightType
        {
            get => 0;
        }

        

        public override dvec3 Direction
        {
            get => dvec3.Zero;
            set { }
        }


        public override Geometry LightGeometry
        {
            get => fullscreenQuad;
        }


        public override dvec3 Position
        {
            get => dvec3.Zero;
            set { }
        }


        public override dmat4 ViewMatrix
        {
            get => dmat4.Identity;        
        }


        public override dmat4 ProjectionMatrix
        {
            get => dmat4.Identity;
        }
    }
}
