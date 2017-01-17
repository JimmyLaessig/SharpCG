using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCG
{
    static class Fun
    {
        
        public static float Radians(float degrees)
        {
            return (float)System.Math.PI * degrees / 180.0f;
        }

        public static float Degrees(float radians)
        {
            return radians * 180.0f / (float)System.Math.PI;
        }

        public static float Clamp(float value, float min, float max)
        {
          return Math.Max(min, Math.Min(value, max));
        }

        public static float ClampCircular(float value, float min, float max)
        {
            if (value >= max) value -= max;
            if (value < min) value += max;
            return value;
        }

    }
}
