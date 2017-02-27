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
            get{return dvec3.Zero;}
            set { }
        }


        public override int LightType
        {
            get{return 0;}
        }

        

        public override dvec3 Direction
        {
            get{return dvec3.Zero;}
            set { }
        }


        public override Geometry LightGeometry
        {
            get{return fullscreenQuad;}
        }


        public override dvec3 Position
        {
            get {return dvec3.Zero;}
            set { }
        }

        public override dmat4 ViewMatrix
        {
            get
            {
                return dmat4.Identity;
            }
        }

        public override dmat4 ProjectionMatrix
        {
            get
            {
                return dmat4.Identity;
            }
        }
    }
}
