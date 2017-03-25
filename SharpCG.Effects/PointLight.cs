using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

using SharpCG.Core;

namespace SharpCG.Effects
{
    public class PointLight : Light
    {
        private dvec3 attenuation;
        private Geometry sphere;

        public override void OnStart()
        {
            sphere = GeometryExtensions.Sphere(vec3.Zero, 1, 20);
            //sphere = GeometryExtensions.FullscreenQuad;
            base.OnStart();
        }

        public override dvec3 Attenuation
        {
            get => attenuation; 
            set
            {
                attenuation = value;
                sceneObject.Transform.WorldScale = new dvec3(EstimateRadius(attenuation));
            }
            
        }

        public override dvec3 Direction
        {
            get => dvec3.Zero;
            set { }
        }

        public override Geometry LightGeometry
        {
            get => sphere;
        }


        public override int LightType
        {
            get => 2;
        }


        public override dvec3 Position
        {
            get => this.sceneObject.Transform.WorldPosition; 
            set => this.sceneObject.Transform.WorldPosition = value; 
        }


        public override dmat4 ProjectionMatrix
        {
            get => dmat4.Identity;
        }


        public override dmat4 ViewMatrix
        {
            get => this.sceneObject.Transform.WorldMatrix.Inverse;
        }


        private static double EstimateRadius(dvec3 attenuation, double tolerance = 0.01)
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

            // TODO ARTR2016_A3 Task 1c: SOLVED
            // Estimate radius of point lights for proper scaling of the geometry.
            // This method can be reused for the implementation of spot lights.

            //float att = 1.0f / ( vLightAtt.x + vLightAtt.y * d + vLightAtt.z * d * d );
            // Solve Attenuation formula against d using the 'größe Lösungsformel' for quadratic equations
            //a = attenuation.z;                       // (quatratic attenuation)
            //b = attenuation.y;                       // (linear attenuation)
            //c = attenuation.x - 1.0f / tolerance;    // (constant attenuation - 1/att);

            //double d1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
            //double d2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);

            //return Math.Max(d1, d2);


        }
    }
}
