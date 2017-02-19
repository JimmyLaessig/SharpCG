using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;

namespace SharpCG.Base.Scenegraph
{
    public class MatrixExtensions
    {

        /// <summary>
        /// Returns the trafo that transforms from the coordinate system
        /// specified by the basis into the world coordinate system.
        /// </summary>
        public static mat4 FromBasis(vec3 xAxis, vec3 yAxis, vec3 zAxis, vec3 orign)
        {
            return new mat4(
                            xAxis.x, yAxis.x, zAxis.x, orign.x,
                            xAxis.y, yAxis.y, zAxis.y, orign.y,
                            xAxis.z, yAxis.z, zAxis.z, orign.z,
                            0, 0, 0, 1);
        }

        public static quat FromMat3(mat3 m, double epsilon = (double)1e-6)
        {
            var t = 1 + m.m00 + m.m11 + m.m22;

            if (t > epsilon)
            {
                float s = (float)Math.Sqrt(t) * 2;
                float x = (m.m21 - m.m12) / s;
                float y = (m.m02 - m.m20) / s;
                float z = (m.m10 - m.m01) / s;
                float w = s / 4;
                return new quat(w, x, y, z).Normalized;
            }
            else
            {
                if (m.m00 > m.m11 && m.m00 > m.m22)
                {
                    float s = (float)Math.Sqrt(1 + m.m00 - m.m11 - m.m22) * 2;
                    float x = s / 4;
                    float y = (m.m01 + m.m10) / s;
                    float z = (m.m02 + m.m20) / s;
                    float w = (m.m21 - m.m12) / s;
                    return new quat(w, x, y, z).Normalized;
                }
                else if (m.m11 > m.m22)
                {
                    float s = (float)Math.Sqrt(1 + m.m11 - m.m00 - m.m22) * 2;
                    float x = (m.m01 + m.m10) / s;
                    float y = s / 4;
                    float z = (m.m12 + m.m21) / s;
                    float w = (m.m20 - m.m02) / s;
                    return new quat(w, x, y, z).Normalized;
                }
                else
                {
                    float s = (float)Math.Sqrt(1 + m.m22 - m.m00 - m.m11) * 2;
                    float x = (m.m20 + m.m02) / s;
                    float y = (m.m12 + m.m21) / s;
                    float z = s / 4;
                    float w = (m.m01 - m.m10) / s;
                    return new quat(w, x, y, z).Normalized;
                }
            }
        }


        public static mat4 GetOrthoNormalOrientation(mat4 matrix)
        {
            var x = matrix.Column0.xyz.Normalized; 
            var y = matrix.Column1.xyz.Normalized; 
            var z = matrix.Column2.xyz.Normalized; 

            y = vec3.Cross(z, x).Normalized;
            z = vec3.Cross(x, y).Normalized;

            return FromBasis(x, y, z, vec3.Zero);
        }


        public static void Decompose(out vec3 position, out vec3 scale, out quat rotation, mat4 matrix)
        {
            position = matrix.Column3.xyz;

            if (matrix.Determinant < 1e-6)
            {
                rotation = quat.Identity;
            }
            else
            {
                var m = new mat3(matrix.Column0.xyz, matrix.Column1.xyz, matrix.Column2.xyz);
                //rotation = G
            }

            //scale = trafo.GetScaleVector();

            // if matrix is left-handed there must be some negative scale
            // since rotation remains the x-axis, the y-axis must be flipped
            //if (trafo.Forward.Det < 0)
            //    scale.Y = -scale.Y;

            scale = new vec3(matrix.Column0.xyz.Length, matrix.Column1.xyz.Length, matrix.Column2.xyz.Length);
            rotation = new quat(1);
        }
    }
}
