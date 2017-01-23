using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Rendering;
using OpenTK.Graphics.OpenGL4;

using GlmSharp;
namespace SharpCG.Base.Rendering
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


        private List<RenderTarget> attachments;

        private RenderTarget depthBuffer;


        public Framebuffer()
        {
            attachments = new List<RenderTarget>();
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
                attachments.Add(target);

            isDirty = true;
           
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


        public Texture2D GetTexture(FramebufferAttachment attachement)
        {
            return attachments.First(ct => ct.attachment == attachement).Texture;
        }


        public List<Texture2D> GetTextures()
        {
            return attachments.ConvertAll(ct => ct.Texture);
        }


        public void InitGL()
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


        public void DeInitGL()
        {
            GL.DeleteFramebuffer(handle);

            attachments.ForEach(ct => ct.Texture.DeInitGL());            
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
