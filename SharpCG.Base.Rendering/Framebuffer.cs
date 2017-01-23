using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Rendering;
using OpenTK.Graphics.OpenGL4;

using GlmSharp;
namespace SharpCG.Base.Scenegraph
{

    public struct RenderTarget
    {
        public Texture2D Texture;
        public vec4 ClearColor;
        public FramebufferAttachment attachment;

    }

    public class Framebuffer : GLObject
    {
        private bool isDirty;

        private int handle;


        private List<RenderTarget> colorTargets;

        private RenderTarget depthBuffer;


        public Framebuffer()
        {
            colorTargets = new List<RenderTarget>();
        }


        public bool IsDirty
        {
            get{return IsDirty;}
        }


        public void AddRenderTarget(Texture2D texture, FramebufferAttachment attachment, vec4 clearColor)
        {
            // TODO CHECK THAT TEXTURES HAVE SAME SIZE

            var target          = new RenderTarget();
            target.Texture      = texture;
            target.ClearColor   = clearColor;
            target.attachment = attachment;

            if (attachment == FramebufferAttachment.Depth)
                depthBuffer = target;
            else
                colorTargets.Add(target);
           
        }


        public void Clear()
        {
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
            //var error = GL.GetError();
            //var drawBuffers = colorTargets.ConvertAll(ct => ct.attachment).Cast<DrawBuffersEnum>().ToArray();              
            //GL.DrawBuffers(drawBuffers.Length, drawBuffers);
            //error = GL.GetError();

            colorTargets.ForEach(ct => 
            {
                GL.DrawBuffer((DrawBufferMode)ct.attachment);              
                var c = ct.ClearColor;
                GL.ClearColor(c.r, c.g, c.b, c.a);
                GL.Clear(ClearBufferMask.ColorBufferBit);
            });
           
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


        public void BindForWriting()
        {

        }


        public Texture2D GetTexture(FramebufferAttachment attachement)
        {
            return colorTargets.First(ct => ct.attachment == attachement).Texture;
        }


        public List<Texture2D> GetTextures()
        {
            return colorTargets.ConvertAll(ct => ct.Texture);
        }


        public void InitGL()
        {
            DeInitGL();

            GL.GenFramebuffers(1, out handle);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);


            colorTargets.ForEach(ct => 
                ct.Texture.InitGL()
            );


            colorTargets.ToList().ForEach(ct =>
            {
                ct.Texture.Bind(TextureUnit.Texture10);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, ct.attachment, TextureTarget.Texture2D, ct.Texture.Handle, 0);
            }
            );

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }        


        public void DeInitGL()
        {
            GL.DeleteFramebuffer(handle);

            colorTargets.ForEach(ct => ct.Texture.DeInitGL());            
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void AfterUpdateGPUResources()
        {
            throw new NotImplementedException();
        }
    }
}
