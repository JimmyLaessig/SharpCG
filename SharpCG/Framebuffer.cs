using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

using GlmSharp;

namespace SharpCG
{

    public struct RenderTarget
    {
        public Texture2D Texture;
        public vec4 ClearColor;
        public FramebufferAttachment attachment;
    }


    public class Framebuffer : GLComponent
    {

        private int handle;

        private List<RenderTarget> attachments;

        private RenderTarget depthBuffer;

        private int renderBufferHandle;


        public Framebuffer() : base()
        {
            attachments = new List<RenderTarget>();
            isDirty     = false;
        }
        
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

            if (attachment == FramebufferAttachment.Depth)
                depthBuffer = target;
            else
                attachments.Add(target);

            isDirty = true;
           
        }


        public Texture2D GetRenderTarget(FramebufferAttachment attachment)
        {
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

            //glBindFramebuffer(GL_FRAMEBUFFER, gBuffer);

        }

        public void BindForReading()
        {

        }


        public override void InitGL()
        {
            DeInitGL();

            GL.GenFramebuffers(1, out handle);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);


            attachments.ForEach(ct => 
                ct.Texture.InitGL()
            );


            attachments.ToList().ForEach(ct =>
            {
                ct.Texture.Bind(TextureUnit.Texture10);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, ct.attachment, TextureTarget.Texture2D, ct.Texture.Handle, 0);
            }
            );

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
