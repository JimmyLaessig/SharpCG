using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;

namespace SharpCG.Base.Rendering
{
    public class ArrayBuffer<T> : GLObject where T : struct
    {
        private bool isDirty = false;

        private T[] data;

        private int handle = -1;

        private int stride = 0;

        private BufferTarget target;
       
        private ArrayBuffer()
        {
            //if(T is float)
            //    allFloatBuffers.Add(this);   
            //if(data is float[])
            //    allFloatBuffers.Add(this);
        }

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
            // TODO quicker?
            var data1D = data.Select(v => v.Values).Cast<byte>().Cast<float>().ToArray();

            return Create(target, data1D,  2);
        }


        public static ArrayBuffer<float> Create(BufferTarget target, vec3[] data)
        {

            //var data1D = new float[data.Length * 3];
           // System.Buffer.BlockCopy(data, 0, data1D, 0, data.Length * 3);
            var data1D = data.Select(v => v.Values).Cast<byte>().Cast<float>().ToArray();

            return Create(target, data1D,  3);

        }


        public static ArrayBuffer<float> Create(BufferTarget target, vec4[] data)
        {
            var data1D = data.Select(v => v.Values).Cast<byte>().Cast<float>().ToArray();
            return Create(target,  data1D,  4);
        }





        public bool IsDirty
        {
            get{ return isDirty; }
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


        public void Dispose()
        {
            DeInitGL();
        }


        public void DeInitGL()
        {
            GL.DeleteBuffer(handle);
            handle = -1;
        }


        public void Bind()
        {
            GL.BindBuffer(target, handle);
        }


        public void InitGL()
        {
            //if(target == BufferTarget.ElementArrayBuffer)
            //{
            //    Console.WriteLine("");
            //}
            DeInitGL();

            GL.GenBuffers(1, out handle);
            GL.BindBuffer(target, handle);
            var typeSize = System.Runtime.InteropServices.Marshal.SizeOf<T>();
            GL.BufferData(this.target, (IntPtr)(typeSize * data.Length), data, BufferUsageHint.StaticDraw);

            isDirty = false;
        }


        public void AfterUpdateGPUResources(){}

    }
}
