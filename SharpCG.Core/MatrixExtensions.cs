using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;


namespace SharpCG.Core
{ 
    public static class MatrixExtensions
    {

        public static mat4 ToMat4 (this dmat4 matrix)
        {
            var c0 = (vec4)matrix.Column0;
            var c1 = (vec4)matrix.Column1;
            var c2 = (vec4)matrix.Column2;
            var c3 = (vec4)matrix.Column3;

            return new mat4(c0, c1, c2, c3);
        }


        public static mat3 ToMat3(this dmat3 matrix)
        {
            var c0 = (vec3)matrix.Column0;
            var c1 = (vec3)matrix.Column1;
            var c2 = (vec3)matrix.Column2;
            return new mat3(c0, c1, c2);
        }

        public static mat2 ToMat2(this dmat2 matrix)
        {
            var c0 = (vec2)matrix.Column0;
            var c1 = (vec2)matrix.Column1;
            return new mat2(c0, c1);
        }


        public static double[] ToArray(this dmat2[] matrices)
        {
            var values = new List<double>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));
            
            return values.ToArray();
        }


        public static double[] ToArray(this dmat3[] matrices)
        {
            var values = new List<double>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));

            return values.ToArray();
        }


        public static double[] ToArray(this dmat4[] matrices)
        {
            var values = new List<double>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));

            return values.ToArray();
        }


        public static float[] ToFloatArray(this dmat2[] matrices)
        {
            var values = new List<double>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));

            return values.ConvertAll(x => (float)x).ToArray();
        }


        public static float[] ToFloatArray(this dmat3[] matrices)
        {
            var values = new List<double>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));

            return values.ConvertAll(x => (float)x).ToArray();
        }


        public static float[] ToFloatArray(this dmat4[] matrices)
        {
            var values = new List<double>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));

            return values.ConvertAll(x => (float)x).ToArray();
        }


        public static float[] ToArray(this mat2[] matrices)
        {
            var values = new List<float>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));

            return values.ToArray();
        }

        public static float[] ToArray(this mat3[] matrices)
        {
            var values = new List<float>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));

            return values.ToArray();
        }

        public static float[] ToArray(this mat4[] matrices)
        {
            var values = new List<float>();
            matrices.ToList().ForEach(m => values.AddRange(m.Values1D));

            return values.ToArray();
        }

        public static dvec3 GetTranslation(this dmat4 matrix)
        {
            return new dvec3(matrix.Column3.xyz);
        }


        public static dvec3 GetScale(this dmat4 matrix)
        {
             return new dvec3(matrix.Column0.xyz.Length, matrix.Column1.xyz.Length, matrix.Column2.xyz.Length);
        }


        /// <summary>
        /// Builds an ortho-normal orientation transformation form the given transform.
        /// Scale and Translation will be removed and basis vectors will be ortho-normalized.
        /// NOTE: The X-Axis is untouched and Y/Z are forced to a normal-angle.
        /// </summary>
        public static dmat4 GetOrthoNormalOrientation(this dmat4 matrix)
        {
            var x = matrix.Column0.xyz.Normalized; // TransformDir(V3d.XAxis)
            var y = matrix.Column1.xyz.Normalized; // TransformDir(V3d.YAxis)
            var z = matrix.Column2.xyz.Normalized; // TransformDir(V3d.ZAxis)

            y = dvec3.Cross(z, x).Normalized;
            z = dvec3.Cross(x, y).Normalized;

            return new dmat4(x, y, z);
        }



        public static void Decompose(this dmat4 matrix, out dvec3 position, out dquat rotation, out dvec3 scale)
        {
            position    = matrix.GetTranslation();

            var rt      = matrix.GetOrthoNormalOrientation();
            rotation    = rt.ToQuaternion;
            scale       = matrix.GetScale();                          
        }
    }
}
