﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace SharpCG
{
    public class PointLight : Light
    {
        private vec3 attenuation;
        private Geometry sphere;

        public override void OnStart()
        {
            sphere = GeometryExtensions.Sphere(vec3.Zero, 1, 20);
            base.OnStart();
        }

        public override vec3 Attenuation
        {
            get{ return attenuation; }
            set{ attenuation = value;
                sceneObject.Transform.WorldScale = new vec3((float)EstimateRadius(attenuation));
            }
        }

        public override vec3 Direction
        {
            get{ return vec3.Zero; }
            set{}
        }

        public override Geometry LightGeometry
        {
            get{return sphere;}
        }


        public override int LightType
        {
            get{return 2;}
        }


        public override vec3 Position
        {
            get{ return this.sceneObject.Transform.WorldPosition; }
            set{ this.sceneObject.Transform.WorldPosition = value; }
        }


        public override mat4 ProjectionMatrix
        {
            get{return mat4.Identity;}
        }


        public override mat4 ViewMatrix
        {
            get{ return this.sceneObject.Transform.WorldMatrix.Inverse; }
        }


        private static double EstimateRadius(vec3 attenuation, double tolerance = 0.01)
        {
            double a = attenuation.z;
            double b = attenuation.y;
            double c = attenuation.x;
            if ((a == 0) && (b == 0))
            {
                return 10.0f; // there is no attenuation -> pick an arbitrary radius
            }
            else if (a == 0)
            {
                return (1.0 / tolerance); // no quadratic attenuation
            }
            else
            {
                double discriminant = b * b - 4.0f * a * (c - 1.0f / tolerance);
                return (-b + Math.Sqrt(discriminant) / (2.0f * a));
            }
        }
    }
}
