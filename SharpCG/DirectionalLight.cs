using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

using SharpCG.Base.Scenegraph;

namespace SharpCG
{
    public class DirectionalLight : Light
    {
        private vec3 direction = new vec3(1, -1, 0);

        private Mesh fullscreenQuad;


        private ShadowMapRenderer shadowMappingTechnique;

        public override void OnStart()
        {
            fullscreenQuad = MeshExtensions.FullscreenQuad;

            if(shadowMappingTechnique == null)
            {
                shadowMappingTechnique = sceneObject.FindComponent<ShadowMapRenderer>();
            }
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


        public mat4 LightProjectionMatrix
        {
            get { return mat4.Ortho(-10f, 10f, -10f, 10f, 0.1f, 1000f); }
        }


        public mat4 lightViewMatrix
        {
            get { return mat4.LookAt(direction, vec3.Zero, vec3.UnitY); }
        }


        public bool HasShadowMap
        {
            get { return shadowMappingTechnique != null; }
        }


        public Texture2D ShadowMap
        {
            get
            {
                if (shadowMappingTechnique != null)
                    return shadowMappingTechnique.DepthTexture;
                return null;
            }
        }
    }
}
