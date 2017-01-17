using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Assimp;
using GlmSharp;
using System.Drawing;

namespace SharpCG.Base.Scenegraph
{
    public class DefaultAttributeName
    {
        public static string Position   = "Position";
        public static string Normal     = "Normal";
        public static string Tangent    = "Tangent";
        public static string Bitangent  = "Bitangent";
        public static string Color      = "Color";

        public static string Texcoord0  = "Texcoord0";
        public static string Texcoord1  = "Texcoord1";
        public static string Texcoord2  = "Texcoord2";
    }


    public struct AttributeInfo
    {
        public float[] data;
        public int perVertexSize;
        public int location;
        public int VBO;
    }

    public class Mesh : Component
    {      
        
        private Dictionary<string, AttributeInfo> attributes;

        private uint[] indices;

        private int indexVBO = -1;

        private int VAO = -1;

        public Texture texture;

        public Mesh()
        {
            attributes = new Dictionary<string, AttributeInfo>();
            indices = new uint[0];
        }

        public bool HasIndices
        {
            get{return indices.Length > 0;}
        }

        public int Handle
        {
            get
            {
                return VAO;
            }          
        }
     
        public int TriangleCount
        {
            get
            {
                return indices.Length / 3;
            }
        }

        public void Bind()
        {
            // No Attributes
            if (attributes.Count <= 0)
                return;

            // Create VAO
            if (VAO < 0){ CreateVAO();}

            if (VAO >= 0) GL.BindVertexArray(VAO);
        }

        public void CreateVAO()
        {
            // Create VAO for the mesh
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            // Create VBO for the indices if it has indices         
            if (HasIndices)
            {
                GL.GenBuffers(1, out indexVBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexVBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * indices.Length), indices, BufferUsageHint.StaticDraw);
            }


            // Create VBO for each attribute
            foreach(var pair in attributes)
            {
                var info = pair.Value;

                GL.GenBuffers(1, out info.VBO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, info.VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * info.data.Length), info.data, BufferUsageHint.StaticDraw);

                // Link to the given location with the given per vertex size
                GL.EnableVertexAttribArray(info.location);
                GL.VertexAttribPointer(info.location, info.perVertexSize, VertexAttribPointerType.Float, false, 0, 0);    
                
            }
        }

        public void SetAttribute(string name, float[] data, int perVertexSize, int location)
        {
            // Remove old attribute if exists
            if(attributes.ContainsKey(name))
            {
                RemoveAttribute(name);
            }
            // Add new Attribute
            {
                var info            = new AttributeInfo();
                info.data           = data;
                info.perVertexSize  = perVertexSize;
                info.location       = location;
                info.VBO            = -1;
                attributes[name]    = info;
            }
        }

        public uint[] Indices
        {
            set{indices = value;}
        }
        
        public void SetAttribute(string name, vec2[] data, int location)
        {
            // TODO quicker?
            var data2 = data.Select(v => v.Values).Cast<byte>().Cast<float>().ToArray();

            SetAttribute(name, data2, 2, location);
        }

        public void SetAttribute(string name, vec3[] data, int location)
        {
            var data2 = data.Select(v => v.Values).Cast<byte>().Cast<float>().ToArray();

            SetAttribute(name, data2, 3, location);
        }

        public void SetAttribute(string name, vec4[] data, int location)
        {
            var data2 = data.Select(v => v.Values).Cast<byte>().Cast<float>().ToArray();

            SetAttribute(name, data2, 4, location);
        }

        public void RemoveAttribute(string name)
        {
            GL.DeleteBuffer(attributes[name].VBO);
            attributes.Remove(name);            
        }

        public void DisposeHandle()
        {
            // Delete Buffers from GPU and clear collection

            foreach(var pair in attributes)
            {
                var info = pair.Value;
                GL.DeleteBuffer(info.VBO);
                info.VBO = -1;
            }


           
            // Delete Index Buffer if exists
            if(indexVBO != -1)
            {
                GL.DeleteBuffer(indexVBO);
                indexVBO = -1;
            }

            // Delete Vertex Array Obect from GPU and clear handle
            GL.DeleteVertexArray(VAO);
            VAO = -1;
        }

        public override void Dispose()
        {
            DisposeHandle();
            attributes.Clear();           
        }

        public static SceneObject Load(string path)
        {
            Assimp.AssimpContext assimp = new AssimpContext();
            Assimp.Scene scene          = assimp.ImportFile(path, 
                                                              PostProcessSteps.Triangulate 
                                                            | PostProcessSteps.JoinIdenticalVertices 
                                                            | PostProcessSteps.CalculateTangentSpace);
            string directory = path.Substring(0, path.LastIndexOf('/'));
            return Traverse(scene, scene.RootNode, directory);
        }

        private static SceneObject Traverse(Assimp.Scene scene, Assimp.Node node, string directory)
        {           
            SceneObject obj = new SceneObject();
            // Decompose Transform
            Vector3D t; Vector3D s; Quaternion r;
            node.Transform.Decompose(out s, out r, out t);

            // Add Transform to scene object
            obj.Transform.Position = new vec3(t.X, t.Y, t.Z);
            obj.Transform.Scale = new vec3(s.X, s.Y, s.Z);
            obj.Transform.Rotation = new quat(r.X, r.Y, r.Z, r.W);

            // Traverse children
            if (node.HasChildren)
            {
                var children = node.Children.ToList().ConvertAll(child => Traverse(scene, child, directory)).Where(c => c != null);
                obj.Children.AddRange(children);
            }

            // Create Mesh Component if it has some (For now, assume each node has max one mesh)
            if (node.HasMeshes)
            {
                var mesh = obj.AddComponent<Mesh>();
                var aiMesh = scene.Meshes[node.MeshIndices[0]];
                mesh.CreateMesh(aiMesh);
                
                Assimp.Material aiMaterial = scene.Materials[aiMesh.MaterialIndex];

                var m = obj.AddComponent<SimpleLightingMaterial>();
                
                if (aiMaterial.HasColorDiffuse)
                {
                   // m.MaterialColor = new Vector4(aiMaterial.ColorDiffuse.R, aiMaterial.ColorDiffuse.G, aiMaterial.ColorDiffuse.B, 1);
                }
                if (aiMaterial.HasOpacity)
                {

                    //mesh.material.diffuseAmount.W = aiMaterial.Opacity;
                }
                if (aiMaterial.HasShininess)
                {
                    m.SpecularExponent = aiMaterial.Shininess;
                }
                if (aiMaterial.HasTextureDiffuse)
                {
                    m.DiffuseMapTexture = Texture.Load(directory + "/" + aiMaterial.TextureDiffuse.FilePath, true);      
                }
                if (aiMaterial.HasTextureNormal)
                {
                    m.NormalMapTexture = Texture.Load(directory + "/" + aiMaterial.TextureNormal.FilePath, true);
                }
                if (aiMaterial.HasTextureSpecular)
                {
                    m.SpecularMapTexture = Texture.Load(directory + "/" + aiMaterial.TextureSpecular.FilePath, true);
                }
                //if(aiMaterial.HasTexture)
                //    if (aiMaterial.HasTextureDisplacement)
                //    {
                //        //     Console.WriteLine("Displacement");
                //    }
                //    if (aiMaterial.HasTextureHeight)
                //    {
                //        //    Console.WriteLine("Height");
                //    }
                //}
                //return meshes;
            }
            // Remove unwanted (empty) scene objects
            if (!node.HasMeshes && !node.HasChildren)
            {
                obj = null;
            }
               
             
            // Return composed scene object
            return obj;
        }

        private void CreateMesh(Assimp.Mesh aiMesh)
        {                     
            uint[] indices  = aiMesh.GetUnsignedIndices();
            int numIndices  = indices.Length;
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
            this.Name       = aiMesh.Name;
            this.indices    = indices;
            this.SetAttribute(DefaultAttributeName.Position , positions , 3 , 0);
            this.SetAttribute(DefaultAttributeName.Normal   , normals   , 3 , 1);
            this.SetAttribute(DefaultAttributeName.Tangent  , tangents  , 3 , 2);
            this.SetAttribute(DefaultAttributeName.Bitangent, bitangents, 3 , 3);
            this.SetAttribute(DefaultAttributeName.Color    , colors    , 4 , 4);
            this.SetAttribute(DefaultAttributeName.Texcoord0, texcoords0, 2 , 5);
           
            }

        

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public static Mesh Find(string key)
        //{
        //    foreach (Mesh mesh in Meshes)
        //    {
        //        if (mesh.name == key)
        //            return mesh;
        //    }
        //    return null;
        //}




    }
}
