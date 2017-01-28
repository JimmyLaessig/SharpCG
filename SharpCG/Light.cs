﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using GlmSharp;


namespace SharpCG
{



    public abstract class Light : Component
    {
        protected vec3 color;

        public abstract vec3 Direction
        {
            get;
        }

        public abstract int LightType
        {
            get;
        }
        public abstract vec3 Position
        {
            get;
        }


        public abstract vec3 Attenuation
        {
            get;
        }

        public virtual vec3 Color
        {
            get { return color; }
            set { color = value; }
        }

        public abstract Mesh LightGeometry
        {
            get;
        }
    }
}