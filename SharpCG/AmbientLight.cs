using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;


namespace SharpCG
{
    public class AmbientLight : Light
    {
        private Geometry fullscreenQuad;

        public override void OnStart()
        {
            fullscreenQuad = GeometryExtensions.FullscreenQuad;

        }

        public override vec3 Attenuation
        {
            get{return vec3.Zero;}
            set { }
        }


        public override int LightType
        {
            get{return 0;}
        }

        

        public override vec3 Direction
        {
            get{return vec3.Zero;}
            set { }
        }


        public override Geometry LightGeometry
        {
            get{return fullscreenQuad;}
        }


        public override vec3 Position
        {
            get {return vec3.Zero;}
            set { }
        }

        public override mat4 ViewMatrix
        {
            get
            {
                return mat4.Identity;
            }
        }

        public override mat4 ProjectionMatrix
        {
            get
            {
                return mat4.Identity;
            }
        }
    }
}
