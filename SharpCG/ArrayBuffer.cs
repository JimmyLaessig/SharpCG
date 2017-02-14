using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;

namespace SharpCG
{
    public class ArrayBuffer<T> : GLComponent where T : struct
    {

        private T[] data;

        private int handle = -1;

        private int stride = 0;

        private BufferTarget target;
       

        public int Length
        {
            get { return data.Length; }
        }


        public static ArrayBuffer<uint> Create(BufferTarget target, uint[] data, int stride = 1)
        {
            ArrayBuffer<uint> buffer = new ArrayBuffer<uint>();
            buffer.data = data;
            buffer.stride = stride;
            buffer.isDirty = true;
            buffer.target = target;
            return buffer;
        }

        public static ArrayBuffer<float> Create(BufferTarget target, float[] data,  int stride)
        {
            ArrayBuffer<float> buffer = new ArrayBuffer<float>();
            buffer.data     = data;
            buffer.stride   = stride;
            buffer.isDirty  = true;
            buffer.target = target;
            return buffer;
        }


        public static ArrayBuffer<float> Create(BufferTarget target, vec2[] data)
        {
            var data1D = new List<float>();
            data.ToList().ForEach(v => data1D.AddRange(v.Values));
            return Create(target, data1D.ToArray(), 2);
        }


        public static ArrayBuffer<float> Create(BufferTarget target, vec3[] data)
        {

            var data1D = new List<float>();
            data.ToList().ForEach(v => data1D.AddRange(v.Values));
            return Create(target, data1D.ToArray(),  3);

        }


        public static ArrayBuffer<float> Create(BufferTarget target, vec4[] data)
        {
            var data1D = new List<float>();
            data.ToList().ForEach(v => data1D.AddRange(v.Values));
            return Create(target, data1D.ToArray(), 4);
        }


        public T[] Data
        {
            get{ return data; }
            set{ data = value;
                isDirty = true;
            }
        }


        public int Handle
        {
            get{ return handle;}       
        }


        public int Stride
        {
            get{ return stride; }
            set{ stride = value;
                isDirty = true;
            }
        }


        public override void InitGL()
        {
            DeInitGL();

            GL.GenBuffers(1, out handle);
            GL.BindBuffer(target, handle);
            var typeSize = System.Runtime.InteropServices.Marshal.SizeOf<T>();
            GL.BufferData(this.target, (IntPtr)(typeSize * data.Length), data, BufferUsageHint.StaticDraw);

            isDirty = false;
        }


        public override void DeInitGL()
        {
            GL.DeleteBuffer(handle);
            handle = -1;
        }


        public void Bind()
        {
            GL.BindBuffer(target, handle);
        }

        
    }
}
