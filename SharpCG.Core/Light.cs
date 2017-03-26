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
        private dvec3 lightDirection;
        private dvec3 lightPosition;
        private dvec3 lightAttenuation;
        private dvec3 lightColor;
        private int lightType;
                

        [Uniform(Name = "vLightDirection")]
        public dvec3 LightDirection { get => lightDirection; set => lightDirection = value; }


        [Uniform(Name = "vLightPosition")]
        public dvec3 LightPosition { get => lightPosition; set => lightPosition = value; }


        [Uniform(Name = "vLightColor")]
        public dvec3 LightAttenuation { get => lightAttenuation; set => lightAttenuation = value; }


        [Uniform(Name = "vLightAttenuation")]
        public dvec3 LightColor { get => lightColor; set => lightColor = value; }


        [Uniform(Name = "lightType")]
        public int LightType { get => lightType; set => lightType = value; }
    }

    public abstract class Light : Component
    {
        protected dvec3 color;

        private LightMaterial material;

        public abstract dvec3 Direction
        {
            get;
            set;
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
            set;
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


        public override void Update(double deltaTime)
        {
            material.LightColor         = color;
            material.LightDirection     = this.sceneObject.Transform.Forward;
            material.LightPosition      = this.sceneObject.Transform.Position;
            material.LightAttenuation   = this.Attenuation;
            material.LightType          = this.LightType;
        }
    }
}
