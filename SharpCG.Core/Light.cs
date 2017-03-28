using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using GlmSharp;



namespace SharpCG.Core
{
    public abstract class LightMaterial : Material
    {
        private Light light;


        [Uniform(Name = "vLightDirection")]
        public dvec3 LightDirection
        {
            get => light.Direction;
        }


        [Uniform(Name = "vLightPosition")]
        public dvec3 LightPosition
        {
            get => light.Position;
        }


        [Uniform(Name = "vLightAttenuation")]
        public dvec3 LightAttenuation
        {
            get => light.Attenuation;
        }


        [Uniform(Name = "vLightColor")]
        public dvec3 LightColor
        {
            get => light.Color;
        }


        [Uniform(Name = "iLightType")]
        public int LightType
        {
            get => light.LightType;
        }



        public Light Light { get => light; set => light = value; }
    }

    public abstract class Light : Component
    {
        protected dvec3 color;

        private LightMaterial material;


        public abstract dvec3 Direction
        {
            get;
        }       


        public abstract int LightType
        {
            get;
        }


        public abstract dmat4 ViewMatrix
        {
            get;
        }


        public abstract dmat4 ProjectionMatrix
        {
            get;
        }


        public abstract dvec3 Position
        {
            get;
        }


        public abstract dvec3 Attenuation
        {
            get;
            set;
        }


        public virtual dvec3 Color
        {
            get => color; 
            set => color = value; 
        }


        public virtual Texture ShadowMap
        {
            get => null;         
        }


        public abstract Geometry LightGeometry
        {
            get;
        }


        public LightMaterial Material
        {
            get => material;
            set => material = value;
        }
    }
}
