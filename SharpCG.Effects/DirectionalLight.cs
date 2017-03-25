using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;


using SharpCG.Core;

namespace SharpCG.Effects
{
    public class DirectionalLight : Light
    {        
        private Geometry fullscreenQuad;

        private ShadowMapRenderer shadowMappingTechnique;
        private dvec3 direction;
        public override void OnStart()
        {
            fullscreenQuad = GeometryExtensions.FullscreenQuad;

            if(shadowMappingTechnique == null)
            {
                shadowMappingTechnique = sceneObject.FindComponent<ShadowMapRenderer>();
            }
        }


        public override dvec3 Attenuation
        {
            get => dvec3.Zero;
            set {}
        }


        public override dvec3 Direction
        {
            get => direction;
            set => direction = value;
        }


        public override Geometry LightGeometry
        {
            get => fullscreenQuad;
        }


        public override int LightType
        {
            get => 1; 
        }


        public override dvec3 Position
        {
            get => dvec3.Zero; 
            set { }
        }


        public override dmat4 ProjectionMatrix
        {           
            get => dmat4.Ortho(-10, 10, -10, 10, 0.01f, 1000f); 
        }


        public override dmat4 ViewMatrix
        {
            get
            {
                dvec3 up = dvec3.UnitY;
                if (Math.Abs(dvec3.Dot(direction, up)) > 0.9999f)
                    up = dvec3.UnitZ;

                return dmat4.LookAt(-direction, dvec3.Zero, up);
            }
        }


        public bool HasShadowMap
        {
            get => shadowMappingTechnique != null;
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
