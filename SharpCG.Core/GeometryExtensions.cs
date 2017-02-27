using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;
using OpenTK.Graphics.OpenGL4;
using SharpCG;

namespace SharpCG.Core
{
    public static class GeometryExtensions
    {
        public struct Triangle
        {
            public vec3 p0, p1, p2;

            public Triangle(vec3 p0, vec3 p1, vec3 p2)
            {
                this.p0 = p0;
                this.p1 = p1;
                this.p2 = p2;
            }                       
        }


        public static Geometry Sphere(vec3 center, double r, double angle)
        {
            var points = new List<vec3>();

            double POLUDNIKOV = 360 / angle;
            double ROVNOBEZIEK = 180 / angle;

            angle = glm.Radians( angle);
                 

            double p2 = POLUDNIKOV / 2;
            double r2 = ROVNOBEZIEK / 2;

            for (double y = -r2; y < r2; ++y)
            {
                double cy = Math.Cos(y * angle);
                double cy1 = Math.Cos((y + 1) * angle);
                double sy = Math.Sin(y * angle);
                double sy1 = Math.Sin((y + 1) * angle);

                for (double i = -p2; i < p2; ++i)
                {
                    double ci = Math.Cos(i * angle);
                    double si = Math.Sin(i * angle);
                    points.Add((vec3)new dvec3(center.x + r * ci * cy, center.y + r * sy, center.z + r * si * cy));
                    points.Add((vec3)new dvec3(center.x + r * ci * cy1, center.y + r * sy1, center.z + r * si * cy1));
                }
            }

            Geometry sphereGeometry = new Geometry();
            sphereGeometry.SetAttribute(DefaultAttributeName.Position, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, points.ToArray()), 0);
            sphereGeometry.PrimitiveType = PrimitiveType.TriangleStrip;
            return sphereGeometry;
        }


        public static Triangle[] Cube
        {
            get
            {
                Triangle[] triangles = new Triangle[12];

                //top
                triangles[0] = new Triangle(new vec3(1, 1, -1), new vec3(-1, 1, -1) ,  new vec3(1, 1, 1));
                triangles[1] = new Triangle(new vec3(-1, 1, 1), new vec3(1, 1, 1), new vec3(-1, 1, -1));
                
                //bottom
                triangles[2] = new Triangle(new vec3(1, -1, 1)    , new vec3(-1, -1, -1), new vec3(1, -1, -1));
                triangles[3] = new Triangle(new vec3(-1, -1, -1)  , new vec3(1, -1, 1)  , new vec3(-1, -1, 1));

                //right
                triangles[4] = new Triangle(new vec3(1, 1, 1)    , new vec3(1, -1, -1)  , new vec3(1, 1, -1));
                triangles[5] = new Triangle(new vec3(1, -1, -1)  , new vec3(1, 1, 1)    , new vec3(1, -1, 1));
                //left
                triangles[6] = new Triangle(new vec3(-1, 1, -1), new vec3(-1, -1, -1), new vec3(-1, 1, 1));
                triangles[7] = new Triangle(new vec3(-1, -1, 1), new vec3(-1, 1, 1) , new vec3(-1, -1, -1));

                //front
                triangles[8] = new Triangle(new vec3(1, 1, 1)    , new vec3(-1, -1, 1)  , new vec3(1, -1, 1));
                triangles[9] = new Triangle(new vec3(-1, -1, 1)  , new vec3(1, 1, 1)    , new vec3(-1, 1, 1));
                //rear
                triangles[10] = new Triangle(new vec3(1, -1, -1), new vec3(-1, -1, -1) ,new vec3(1, 1, -1) );
                triangles[11] = new Triangle(new vec3(-1, 1, -1) , new vec3(1, 1, -1), new vec3(-1, -1, -1));

                return triangles;
            }
        }


        public static Geometry UnitCube
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


                Geometry mesh   = new Geometry();
                mesh.Name       = "Cube";
                mesh.Enabled    = true;
                mesh.Indices    = ArrayBuffer<uint>.Create(OpenTK.Graphics.OpenGL4.BufferTarget.ElementArrayBuffer, indices);               
                mesh.SetAttribute(DefaultAttributeName.Position,  ArrayBuffer<uint>.Create(BufferTarget.ArrayBuffer, positions, 3), 0);

                return mesh;
            }
        }


        public static Geometry FullscreenQuad
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

                Geometry mesh       = new Geometry();
                mesh.Name       = "Quad";
                mesh.Enabled    = true;
                mesh.Indices = ArrayBuffer<uint>.Create(OpenTK.Graphics.OpenGL4.BufferTarget.ElementArrayBuffer, indices);
                mesh.SetAttribute(DefaultAttributeName.Position, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, positions, 3), 0);
                mesh.SetAttribute(DefaultAttributeName.Texcoord0, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, uvs, 2), 1);
                return mesh;
            }
        }


        public static Geometry Plane(vec2 dim)
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

            Geometry mesh       = new Geometry();
            mesh.Name       = "Plane";
            mesh.Enabled    = true;
            mesh.Indices = ArrayBuffer<uint>.Create(OpenTK.Graphics.OpenGL4.BufferTarget.ElementArrayBuffer, indices);
            mesh.SetAttribute(DefaultAttributeName.Position, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, positions, 3), 0);
            mesh.SetAttribute(DefaultAttributeName.Texcoord0, ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, uvs, 2), 0);
            return mesh;
        }


        public static SceneObject Load(string path)
        {           
            Assimp.AssimpContext assimp = new Assimp.AssimpContext();
            
            Assimp.Scene scene = assimp.ImportFile(path,    Assimp.PostProcessSteps.Triangulate |
                                                            Assimp.PostProcessSteps.JoinIdenticalVertices |
                                                            Assimp.PostProcessSteps.CalculateTangentSpace |
                                                            0                                                          
                                                            );
            string directory = path.Substring(0, path.LastIndexOf('/'));
            var obj = Traverse(scene, scene.RootNode, directory);

            scene.Clear();
            
            return obj;
        }


        private static SceneObject Traverse(Assimp.Scene scene, Assimp.Node node, string path)
        {
            var name = node.Name;
            Assimp.Vector3D t; Assimp.Vector3D s; Assimp.Quaternion r;
            node.Transform.Decompose(out s, out r, out t);
                        
            SceneObject obj         = new SceneObject();
            obj.Name                = node.Name;
            // Add Transform to scene object
            obj.Transform.WorldPosition = new vec3(t.X, t.Y, t.Z);
            obj.Transform.WorldScale    = new vec3(s.X, s.Y, s.Z);
            obj.Transform.WorldRotation = new quat(r.X, r.Y, r.Z, r.W);
            //obj.Transform.WorldRotation = quat.Identity;
            obj.Transform.ToIdentity();
            // Create Meshes
            node.MeshIndices.ForEach(i => 
            {
                var aiMesh      = scene.Meshes[i];
                obj.Name        = aiMesh.Name;
                var mesh        = obj.AddComponent<Geometry>();
                var material    = obj.AddComponent<AssimpMaterial>();
               
                AssimpConverter.Convert(aiMesh, ref mesh);
                AssimpConverter.Convert(scene.Materials[aiMesh.MaterialIndex], path, ref material);

                mesh.Material = material;
            });

            // Remove unwanted (empty) scene objects
            if (!node.HasMeshes && !node.HasChildren)
            {
                obj = null;
            }

            // Traverse children
            if (node.HasChildren)
            {
                var children = node.Children.ToList().ConvertAll(child => Traverse(scene, child, path)).Where(c => c != null);
                children.ToList().ForEach(c => obj.AddChild(c));               
            }          

            // Return composed scene object
            return obj;
        }

       
    }
}
