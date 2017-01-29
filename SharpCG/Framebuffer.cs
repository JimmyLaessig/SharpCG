using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

using GlmSharp;

namespace SharpCG
{

    public class RenderTarget
    {
        public Texture2D Texture;
        public vec4 ClearColor;
        public FramebufferAttachment attachment;
    }


    public class Framebuffer : GLComponent
    {

        private int handle;

        private List<RenderTarget> attachments = new List<RenderTarget>();

        private RenderTarget depthBuffer;

        private int renderBufferHandle;


        
        public int Width
        {
            get
            {
                if (attachments.Count <= 0)
                    return 0;
                return (int)attachments.First().Texture.Width;
            }
        }


        public int Height
        {
            get
            {
                if (attachments.Count <= 0)
                    return 0;
                return (int)attachments.First().Texture.Height;
            }
        }


        public void AddRenderBuffer()
        {

            //if (depthBuffer != null)
            //{
            //    Console.WriteLine("Already depth texture created!");
            //    return;
            //}

            int width = Width;
            int height = Height;

            if (width <= 0 || height <= 0)
                return;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);

            GL.GenRenderbuffers(1, out renderBufferHandle);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, renderBufferHandle);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderBufferHandle);

        }



        public void AddRenderTarget(Texture2D texture, FramebufferAttachment attachment, vec4 clearColor)
        {
            // TODO CHECK THAT TEXTURES HAVE SAME SIZE

            var target          = new RenderTarget();
            target.Texture      = texture;
            target.ClearColor   = clearColor;
            target.attachment = attachment;

            if (attachment == FramebufferAttachment.DepthAttachment || attachment == FramebufferAttachment.DepthStencilAttachment)
                depthBuffer = target;
            else
                attachments.Add(target);

            isDirty = true;         
        }


        public Texture2D GetRenderTarget(FramebufferAttachment attachment)
        {

            if (attachment == FramebufferAttachment.DepthAttachment || attachment == FramebufferAttachment.DepthStencilAttachment)
                return depthBuffer.Texture;

            var ls = attachments.Where(a => a.attachment == attachment);

            if (ls.Count() > 0)
                return ls.First().Texture;

            return null;
             
        }



        public void Clear()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);

            attachments.ForEach(ct => 
            {
                GL.DrawBuffer((DrawBufferMode)ct.attachment);              
                var c = ct.ClearColor;
                GL.ClearColor(c.r, c.g, c.b, c.a);
                GL.Clear(ClearBufferMask.ColorBufferBit);
            });
            GL.ClearDepth(1.0);
            GL.ClearStencil(0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }



        public void BindForWriting()
        {
            if (attachments.Count > 0)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.handle);

                var buffers = attachments.ConvertAll(a =>(DrawBuffersEnum) a.attachment).ToArray();
                GL.DrawBuffers(buffers.Length, buffers);                
                
                int width = (int)attachments.First().Texture.Width;
                int height = (int)attachments.First().Texture.Height;
                GL.Viewport(0, 0, width, height);
            }
        }

        public void BindForReading()
        {

        }


        public override void LateInitGL()
        {
            DeInitGL();

            GL.GenFramebuffers(1, out handle);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);

            attachments.ToList().ForEach(ct =>
            {
                ct.Texture.Bind(TextureUnit.Texture0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, ct.attachment, TextureTarget.Texture2D, ct.Texture.Handle, 0);
            }
            );

            if (depthBuffer != null)
            {
                depthBuffer.Texture.Bind(TextureUnit.Texture0);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, depthBuffer.attachment, TextureTarget.Texture2D, depthBuffer.Texture.Handle, 0);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            isDirty = false;
        }        



        public override void DeInitGL()
        {
            GL.DeleteFramebuffer(handle);
            GL.DeleteRenderbuffer(renderBufferHandle);     
        }
    }
}
