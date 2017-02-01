using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;


namespace SharpCG
{
    public class DirectionalLight : Light
    {
        private vec3 direction = new vec3(1, -1, 0);

        private Mesh fullscreenQuad;


        public override void OnStart()
        {
            fullscreenQuad = MeshExtensions.FullscreenQuad;
        }


        public override vec3 Attenuation
        {
            get{return vec3.Zero;}
            set { }
        }


        public override vec3 Direction
        {
            get{ return direction;}
            set{ direction = value;}
        }


        public override Mesh LightGeometry
        {
            get{ return fullscreenQuad; }
        }


        public override int LightType
        {
            get{return 1;}
        }

        public override vec3 Position
        {
            get {return vec3.Zero;}
            set { }
        }
    }
}
