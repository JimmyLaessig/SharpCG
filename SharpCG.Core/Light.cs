using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using GlmSharp;



namespace SharpCG.Core
{

    public abstract class Light : Component
    {
        protected dvec3 color;

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
    }
}
