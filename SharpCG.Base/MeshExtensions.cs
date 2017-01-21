using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Scenegraph;

using GlmSharp;

namespace SharpCG.Base
{
    public static class MeshExtensions
    {
        public static Mesh UnitCube
        {
            get {
                float[] positions = {
                // front
                -0.5f, -0.5f,  0.5f,
                 0.5f, -0.5f,  0.5f,
                 0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f,
                // back
                -0.5f, -0.5f, -0.5f,
                 0.5f, -0.5f, -0.5f,
                 0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f,
              };

                uint[] indices = {    
                // front
                0, 1, 2,
                2, 3, 0,
		        // top
		        1, 5, 6,
                6, 2, 1,
		        // back
		        7, 6, 5,
                5, 4, 7,
		        // bottom
		        4, 0, 3,
                3, 7, 4,
		        // left
		        4, 5, 1,
                1, 0, 4,
		        // right
		        3, 2, 6,
                6, 7, 3, };


                Mesh mesh       = new Mesh();
                mesh.Name       = "Cube";
                mesh.Enabled    = true;
                mesh.Indices    = indices;               
                mesh.SetAttribute(DefaultAttributeName.Position, positions, 3, 0);

                return mesh;
            }
        }

        public static Mesh FullscreenQuad
        {
            get {
                float[] positions = {
                    -1.0f, -1.0f, 1.0f,
                     1.0f, -1.0f, 1.0f,
                     1.0f,  1.0f, 1.0f,
                    -1.0f,  1.0f, 1.0f,
                };               
                   
                uint[] indices = {
                    0, 1, 2,
                    0, 2 ,3,
                };

                float[] uvs = {
                    0.0f, 0.0f,
                    1.0f, 0.0f,
                    1.0f, 1.0f,
                    0.0f, 1.0f,
                };

                Mesh mesh       = new Mesh();
                mesh.Name       = "Quad";
                mesh.Enabled    = true;
                mesh.Indices    = indices;
                mesh.SetAttribute(DefaultAttributeName.Position, positions, 3, 0);
                mesh.SetAttribute(DefaultAttributeName.Texcoord0, uvs, 2, 2);
                return mesh;
            }
        }

        public static Mesh Plane(vec2 dim)
        {
            float[] positions = {
                dim.x * -0.5f, 0.0f, dim.y * -0.5f,
                dim.x *  0.5f, 0.0f, dim.y * -0.5f,
                dim.x *  0.5f, 0.0f, dim.y *  0.5f,
                dim.x * -0.5f, 0.0f, dim.y *  0.5f,
              };

            uint[] indices = {
                0, 1, 2,
                0, 2 ,3,
            };

            float[] uvs = {
                0.0f, 0.0f,
                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
            };

            Mesh mesh       = new Mesh();
            mesh.Name       = "Plane";
            mesh.Enabled    = true;
            mesh.Indices    = indices;
            mesh.SetAttribute(DefaultAttributeName.Position, positions, 3, 0);
            mesh.SetAttribute(DefaultAttributeName.Texcoord0, uvs, 2, 2);
            return mesh;
        }
    }
}
