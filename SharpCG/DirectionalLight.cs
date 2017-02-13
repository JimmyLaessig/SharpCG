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
        private Mesh fullscreenQuad;

        private ShadowMapRenderer shadowMappingTechnique;
        private vec3 direction;
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
            get{ return vec3.Zero; }
            set{ }
        }


        public override vec3 Direction
        {
            get{ 
                return direction;
            }
            set{
                direction = value;    
            }
        }


        public override Mesh LightGeometry
        {
            get{ return fullscreenQuad; }
        }


        public override int LightType
        {
            get{ return 1; }
        }


        public override vec3 Position
        {
            get { return vec3.Zero; }
            set { }
        }


        public override mat4 ProjectionMatrix
        {           
            get { return mat4.Ortho(-10, 10, -10, 10, 0.01f, 1000f); }
        }


        public override mat4 ViewMatrix
        {
            get {
                vec3 up = vec3.UnitY;
                if (Math.Abs(vec3.Dot(direction, up)) > 0.9999f)
                    up = vec3.UnitZ;

                return mat4.LookAt(-direction, vec3.Zero, up);
                }
        }


        public bool HasShadowMap
        {
            get { return shadowMappingTechnique != null; }
        }


        public override Texture ShadowMap
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
