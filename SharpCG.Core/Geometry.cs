using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

using GlmSharp;
using System.Drawing;


namespace SharpCG.Core
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


    public class Geometry : GLComponent
    {      
        
        private Dictionary<string, AttributeInfo<float>> attributes = new Dictionary<string, AttributeInfo<float>>();

        private ArrayBuffer<uint> indices;

        private int VAO = -1;

        private PrimitiveType primitiveType = PrimitiveType.Triangles;

        private Material material;


        public bool HasIndices
        {
            get => indices != null;
        }


        public int Handle
        {
            get => VAO;        
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
                    if (attributes.ContainsKey(DefaultAttributeName.Position))
                    {
                        var buffer = attributes[DefaultAttributeName.Position].buffer;
                        return buffer.Length / buffer.Stride / 3;
                    }
                    return 0;                  
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
            get => indices; 
            set
            {
                indices = value;
                isDirty = true;
            }
        }

      
        public override void LateInitGL()
        {
            DeInitGL();
            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);

            // Bind IndexBuffer if exists
            if (indices != null) 
                indices.Bind();

            // Bind each Attribute Array and set Pointer
            attributes.Values.ToList().ForEach(info =>
            {
                var buffer      = info.buffer;
                var location    = info.location;

                buffer.Bind();
                // Link to the given location with the given per vertex size
                GL.EnableVertexAttribArray(location);
                GL.VertexAttribPointer(location, buffer.Stride, VertexAttribPointerType.Float, false, 0, 0);
            });

            // Unbind VAO
            GL.BindVertexArray(0);

            isDirty = false;
        }
        

        public override void DeInitGL()
        {
            GL.DeleteVertexArray(VAO);
            isDirty = false;          
        }       


        public PrimitiveType PrimitiveType
        {
            get => primitiveType; 
            set => primitiveType = value;
        }


        public Material Material
        {
            get => material;
            set => material = value;
        }
    }
}
