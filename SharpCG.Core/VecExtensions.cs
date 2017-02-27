using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
namespace SharpCG.Core
{
    public static class VecExtensions
    {


        public static dvec3 ClampCircular(this dvec3 vec, double maxX = 0, double maxY = 0, double maxZ = 0)
        {
            var TWO_PI = Math.PI * 2;


            vec = vec % TWO_PI;

            //fmodf can return negative values, but this will make them all positive
            if (vec.x < 0)      vec.x += TWO_PI;
            if (vec.x > TWO_PI) vec.x -= TWO_PI;

            if (vec.y < 0)      vec.y += TWO_PI;
            if (vec.y > TWO_PI) vec.y -= TWO_PI;

            if (vec.z < 0)      vec.z += TWO_PI;
            if (vec.z > TWO_PI) vec.z -= TWO_PI;


            if (maxX > 0)   vec.x = glm.Clamp(vec.x, -maxX, maxX);
            if (maxY > 0)   vec.y = glm.Clamp(vec.y, -maxY, maxY);        
            if (maxZ > 0)   vec.z = glm.Clamp(vec.z, -maxZ, maxZ);          

            return vec;
        }
    }
}
