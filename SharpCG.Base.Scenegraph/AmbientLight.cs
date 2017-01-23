using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using SharpCG.Base;

namespace SharpCG.Base.Scenegraph
{
    public class AmbientLight : Light
    {
        private Mesh fullscreenQuad;

        public override void OnStart()
        {
            fullscreenQuad = MeshExtensions.FullscreenQuad;

        }

        public override vec3 Attenuation
        {
            get{return vec3.Zero;}
        }


        public override int LightType
        {
            get{return 0;}
        }


        public override vec3 Direction
        {
            get{return vec3.Zero;}
        }


        public override Mesh LightGeometry
        {
            get{return fullscreenQuad;}
        }


        public override vec3 Position
        {
            get {return vec3.Zero;}
        }
    }
}
