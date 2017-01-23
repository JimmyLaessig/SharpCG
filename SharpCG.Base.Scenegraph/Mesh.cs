using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

using GlmSharp;
using System.Drawing;
using SharpCG.Base.Rendering;

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


    struct AttributeInfo<T> where T : struct
    {
        public ArrayBuffer<T> buffer;
        public int location;
    }

    public class Mesh : Component , GLObject
    {      
        
        private Dictionary<string, AttributeInfo<float>> attributes;

        private ArrayBuffer<uint> indices;

        private int VAO = -1;

        private PrimitiveType primitiveType = PrimitiveType.Triangles;

        private bool isDirty;


        public Mesh()
        {
            allMeshes.Add(this);          
            attributes = new Dictionary<string, AttributeInfo<float>>();
            
        }


        public bool HasIndices
        {
            get{return indices != null;}
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
                if (indices != null)
                {
                    return indices.Data.Length / 3;
                }
                if (attributes.Values.Count > 0)
                {
                    var buffer = attributes.Values.First().buffer;
                    return buffer.Length / buffer.Stride;
                }
                else
                {
                    return 0;
                }
            }
        }
   


        public void Bind()
        {            
            if (VAO >= 0) GL.BindVertexArray(VAO);
        }


        public void SetAttribute(string name, ArrayBuffer<float> buffer, int location)
        {
            // Remove old attribute if exists
            if(attributes.ContainsKey(name))
            {
                RemoveAttribute(name);
            }
            // Add new Attribute
            {
                var info = new AttributeInfo<float>();
                info.buffer     = buffer;
                info.location   = location;

                attributes[name]    = info;
                isDirty             = true;
            }
        }


        public void RemoveAttribute(string name)
        {
            attributes[name].buffer.DeInitGL();          
            attributes.Remove(name);
            isDirty = true;
        }


        public ArrayBuffer<uint> Indices
        {
            get { return indices; }
            set
            {
                indices = value;
                isDirty = true;
            }
        }


        public bool IsDirty
        {
            get{return isDirty;}
        }

      
        public void InitGL()
        {

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            if (indices != null) indices.InitGL();
            


            //if (HasIndices)
            //    indices.Bind();

            // Bind each Attribute Array and set Pointer
            attributes.Values.ToList().ForEach(info =>

            {
                info.buffer.InitGL();

                var buffer = info.buffer;
                var location = info.location;

                buffer.Bind();
                // Link to the given location with the given per vertex size
                GL.EnableVertexAttribArray(location);
                GL.VertexAttribPointer(location, buffer.Stride, VertexAttribPointerType.Float, false, 0, 0);

            });

            GL.BindVertexArray(0);

            isDirty = false;
        }

        public void AfterUpdateGPUResources()
        {
            //string s = this.name;
            ////private string n = ;

            //// Create VAO for the mesh
            

            //// Bind IndexArray if it exists
            //if (HasIndices)
            //    indices.Bind();

            //// Bind each Attribute Array and set Pointer
            //attributes.Values.ToList().ForEach(info => 
         
            //{
            //    var buffer      = info.buffer;
            //    var location    = info.location;

            //    buffer.Bind();             
            //    // Link to the given location with the given per vertex size
            //    GL.EnableVertexAttribArray(location);
            //    GL.VertexAttribPointer(location, buffer.Stride, VertexAttribPointerType.Float, false, 0, 0);

            //});

            //isDirty = false;

        }


        public void DeInitGL()
        {
            // Delete Buffers from GPU and clear collection
           //A
           // foreach (at)
           // {
           //     var info = pair.Value;
           //     GL.DeleteBuffer(info.VBO);
           //     info.VBO = -1;
           // }


           // // Delete Index Buffer if exists
           // if (indexVBO != -1)
           // {
           //     GL.DeleteBuffer(indexVBO);
           //     indexVBO = -1;
           // }

           // // Delete Vertex Array Obect from GPU and clear handle
           // GL.DeleteVertexArray(VAO);
           // VAO = -1;

           // isDirty = false;
        }


        public override void Dispose()
        {
            DeInitGL();
            attributes.Clear();
            isDirty = false;
        }
        

        public PrimitiveType PrimitiveType
        {
            get{return primitiveType; }
            set{ primitiveType = value;}
        }


        private static List<Mesh> allMeshes = new List<Mesh>();

        public static List<Mesh> All
        {
            get { return allMeshes; }
        }
    }
}
