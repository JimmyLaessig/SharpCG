using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG
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
                mesh.Indices    = ArrayBuffer<uint>.Create(OpenTK.Graphics.OpenGL4.BufferTarget.ElementArrayBuffer, indices);               
                mesh.SetAttribute(DefaultAttributeName.Position,  ArrayBuffer<uint>.Create(BufferTarget.ArrayBuffer, positions, 3), 0);

                return mesh;
            }
        }


        public static Mesh FullscreenQuad
        {
            get {
                float[] positions = {
                    -1.0f, -1.0f, 0.0f,
                     1.0f, -1.0f, 0.0f, 
                     1.0f,  1.0f, 0.0f, 
                    -1.0f,  1.0f, 0.0f, 
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
                mesh.Indices = ArrayBuffer<uint>.Create(OpenTK.Graphics.OpenGL4.BufferTarget.ElementArrayBuffer, indices);
                mesh.SetAttribute(DefaultAttributeName.Position, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, positions, 3), 0);
                mesh.SetAttribute(DefaultAttributeName.Texcoord0, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, uvs, 2), 1);
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
            mesh.Indices = ArrayBuffer<uint>.Create(OpenTK.Graphics.OpenGL4.BufferTarget.ElementArrayBuffer, indices);
            mesh.SetAttribute(DefaultAttributeName.Position, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, positions, 3), 0);
            mesh.SetAttribute(DefaultAttributeName.Texcoord0, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, uvs, 2), 0);
            return mesh;
        }

        public enum Materials
        {
            SimpleLighting, Deferred
        }

        public static SceneObject Load(string path, Materials materialType)
        {
            Assimp.AssimpContext assimp = new Assimp.AssimpContext();
            Assimp.Scene scene = assimp.ImportFile(path,
                                                            Assimp.PostProcessSteps.Triangulate
                                                            | Assimp.PostProcessSteps.JoinIdenticalVertices
                                                            | Assimp.PostProcessSteps.CalculateTangentSpace);
            string directory = path.Substring(0, path.LastIndexOf('/'));
            return Traverse(scene, scene.RootNode, directory, materialType);
        }


        private static SceneObject Traverse(Assimp.Scene scene, Assimp.Node node, string directory, Materials materialType)
        {
            SceneObject obj = new SceneObject();
            // Decompose Transform
            Assimp.Vector3D t; Assimp.Vector3D s; Assimp.Quaternion r;
            node.Transform.Decompose(out s, out r, out t);

            // Add Transform to scene object
            obj.Transform.Position = new vec3(t.X, t.Y, t.Z);
            obj.Transform.Scale = new vec3(s.X, s.Y, s.Z);
            obj.Transform.Rotation = new quat(r.X, r.Y, r.Z, r.W);


           
            // Create Mesh Component if it has some (For now, assume each node has max one mesh)
            if (node.HasMeshes)
            {
                var mesh = obj.AddComponent<Mesh>();
                var aiMesh = scene.Meshes[node.MeshIndices[0]];
                ConvertMesh(aiMesh, mesh);
                obj.Name = aiMesh.Name;
                Assimp.Material aiMaterial = scene.Materials[aiMesh.MaterialIndex];
                if (materialType == Materials.SimpleLighting)
                {
                    obj.AddComponent(MaterialExtensions.Create(aiMaterial, directory));
                }
                else if (materialType == Materials.Deferred)
                {
                    obj.AddComponent(MaterialExtensions.Create2(aiMaterial, directory));
                }
               
            }


            // Traverse children
            if (node.HasChildren)
            {
                var children = node.Children.ToList().ConvertAll(child => Traverse(scene, child, directory, materialType)).Where(c => c != null).ToList();
                children.ForEach(c => obj.AddChild(c));
            }


            // Remove unwanted (empty) scene objects
            if (!node.HasMeshes && !node.HasChildren)
            {
                obj = null;
            }


            // Return composed scene object
            return obj;
        }


        private static void ConvertMesh(Assimp.Mesh aiMesh, Mesh mesh)
        {
            uint[] indices = aiMesh.GetUnsignedIndices();
            int numIndices = indices.Length;
            int numVertices = aiMesh.VertexCount;
            
            float[] positions   = new float[numVertices * 3];
            float[] colors      = new float[numVertices * 4];
            float[] normals     = new float[numVertices * 3];
            float[] tangents    = new float[numVertices * 3];
            float[] bitangents  = new float[numVertices * 3];
            float[] texcoords0  = new float[numVertices * 2];

            // Read vertex information
            if (aiMesh.HasVertices)
            {
                int i = 0;
                foreach (var vertex in aiMesh.Vertices)
                {
                    positions[i++] = vertex.X;
                    positions[i++] = vertex.Y;
                    positions[i++] = vertex.Z;
                }
            }

            // Read normal information
            if (aiMesh.HasNormals)
            {
                Console.WriteLine("Loading Normals...");
                var i = 0;
                foreach (var normal in aiMesh.Normals)
                {
                    normals[i++] = normal.X;
                    normals[i++] = normal.Y;
                    normals[i++] = normal.Z;
                }
            }

            // Read Tangents and Bitangents
            if (aiMesh.HasTangentBasis)
            {
                Console.WriteLine("Loading Tangents and Bitangents...");
                int i = 0;
                foreach (var tangent in aiMesh.Tangents)
                {
                    tangents[i++] = tangent.X;
                    tangents[i++] = tangent.Y;
                    tangents[i++] = tangent.Z;
                }

                i = 0;
                foreach (var bitangent in aiMesh.Tangents)
                {
                    bitangents[i++] = bitangent.X;
                    bitangents[i++] = bitangent.Y;
                    bitangents[i++] = bitangent.Z;
                }
            }

            // Read Texture coordinates           
            if (aiMesh.HasTextureCoords(0))
            {
                Console.WriteLine("Loading Texture coordinates...");
                var i = 0;
                foreach (var uv in aiMesh.TextureCoordinateChannels[0])
                {
                    texcoords0[i++] = uv.X;
                    texcoords0[i++] = uv.Y;
                }
            }

            // Read color information               
            if (aiMesh.HasVertexColors(0))
            {
                Console.WriteLine("Loading Colors...");
                int i = 0;
                foreach (var color in aiMesh.VertexColorChannels[0])
                {
                    colors[i + 0] = color.R;
                    colors[i + 1] = color.G;
                    colors[i + 2] = color.B;
                    colors[i + 3] = color.A;
                }
            }

            // Compose mesh structure
            mesh.Name = aiMesh.Name;
            mesh.Indices = ArrayBuffer<uint>.Create(BufferTarget.ElementArrayBuffer, indices);

            // ArrayBuffer.Create()
            mesh.SetAttribute(DefaultAttributeName.Position,    ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, positions, 3), 0);
            mesh.SetAttribute(DefaultAttributeName.Normal,      ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, normals, 3), 1);
            mesh.SetAttribute(DefaultAttributeName.Tangent,     ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, tangents, 3), 2);
            mesh.SetAttribute(DefaultAttributeName.Bitangent,   ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, bitangents, 3), 3);
            mesh.SetAttribute(DefaultAttributeName.Color,       ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, colors, 4), 4);
            mesh.SetAttribute(DefaultAttributeName.Texcoord0,   ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, texcoords0, 2), 5);

        }
    }
}
