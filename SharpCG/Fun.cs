using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;

namespace SharpCG
{
    public static class Fun
    {
        
        public static float Radians(float degrees)
        {
            return (float) (System.Math.PI * degrees / 180.0);
        }

        public static float Degrees(double radians)
        {
            return (float)(radians * 180.0 / System.Math.PI);
        }

        public static double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(value, max));            
        }

        public static double ClampCircular(double value, double min, double max)
        {
            if (value >= max) value -= max;
            if (value < min) value += max;
            return value;
        }

        public static void Decompose (mat4 matrix, out vec3 translation, out quat rotation, out vec3 scale)
        {
            //Extract the translation
            translation.x = matrix.m03;
            translation.y = matrix.m13;
            translation.z = matrix.m23;

            //Extract row vectors of the matrix
            vec3 row0 = matrix.Row0.xyz;
            vec3 row1 = matrix.Row1.xyz;
            vec3 row2 = matrix.Row2.xyz;

            //Extract the scaling factors
            scale.x = row0.Length;
            scale.y = row1.Length;
            scale.z = row2.Length;

            //Handle negative scaling
            if (matrix.Determinant < 0)
            {
                scale = -scale;
            }

            //Remove scaling from the matrix
            if (scale.x != 0)
            {
                row0 /= scale.x;
            }

            if (scale.y != 0)
            {
                row1 /= scale.y;
            }

            if (scale.z != 0)
            {
                row2 /= scale.z;
            }

            mat3 rotMat = new mat3( row0.x, row0.y, row0.z,
                                    row1.x, row1.y, row1.z,
                                    row2.x, row2.y, row2.z);

            rotation = new quat(rotMat);
        }

    }
}
