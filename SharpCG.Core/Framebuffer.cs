using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

using GlmSharp;

namespace SharpCG.Core
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

        private List<RenderTarget> colorAttachments = new List<RenderTarget>();

        private RenderTarget depthBuffer;

        private int renderBufferHandle;

		public int Handle 
		{
			get => handle;
		}

        public int Width
        {
            get
            {
                if (depthBuffer != null)
                    return depthBuffer.Texture.Width;

                if (colorAttachments.Count > 0)
                    return colorAttachments.First().Texture.Width;

                return 0;
            }
        }


        public int Height
        {
            get
            {
                if (depthBuffer != null)
                    return depthBuffer.Texture.Height;

                if (colorAttachments.Count > 0)
                    return colorAttachments.First().Texture.Height;

                return 0;
            }
        }


        public void AddRenderBuffer()
        {

            if (depthBuffer != null)
            {
                Console.WriteLine("Already depth texture created!");
                return;
            }

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
            // TODO CHECK IF TEXTURES HAVE SAME SIZE

            var target = new RenderTarget()
            {
                Texture = texture,
                ClearColor = clearColor,
                attachment = attachment
            };
            if (attachment == FramebufferAttachment.DepthAttachment || attachment == FramebufferAttachment.DepthStencilAttachment)
                depthBuffer = target;
            else
                colorAttachments.Add(target);

            isDirty = true;
        }


        public Texture2D GetRenderTarget(FramebufferAttachment attachment)
        {

            if (attachment == FramebufferAttachment.DepthAttachment || attachment == FramebufferAttachment.DepthStencilAttachment)
                return depthBuffer.Texture;

            var renderTarget = colorAttachments.FindLast(a => a.attachment == attachment);
            return renderTarget?.Texture;
        }


        public void Clear()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
            GL.Viewport(0, 0, Width, Height);
            colorAttachments.ForEach(renderTarget =>
            {
                GL.DrawBuffer((DrawBufferMode)renderTarget.attachment);
                var c = renderTarget.ClearColor;
                GL.ClearColor(c.r, c.g, c.b, c.a);
                GL.Clear(ClearBufferMask.ColorBufferBit);
            });

            GL.ClearDepth(1.0);
            GL.ClearStencil(0);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


        public void BindForWriting()
        {

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, this.handle);

            if (colorAttachments.Count > 0)
            {
                var buffers = colorAttachments.ConvertAll(a => (DrawBuffersEnum)a.attachment).ToArray();
                GL.DrawBuffers(buffers.Length, buffers);
            }
            else
            {
                GL.ReadBuffer(ReadBufferMode.None);
                GL.DrawBuffer(DrawBufferMode.None);
            }

            GL.Viewport(0, 0, Width, Height);
        }



        public override void LateInitGL()
        {
            DeInitGL();

            GL.GenFramebuffers(1, out handle);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);

            colorAttachments.ToList().ForEach(ct =>
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


        public static Framebuffer GBuffer(Window window)
        {
            
                var gBuffer = new Framebuffer();

                // TODO CHANGE THIS
                int width   = window.Width;
                int height  = window.Height;

                gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment0, new vec4(1));   // Diffuse
                gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment1, new vec4(0));   // Specular
                gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment2, new vec4(0));   // Normals
                gBuffer.AddRenderTarget(Texture2D.Depth(width, height), FramebufferAttachment.DepthAttachment, new vec4(1));    // Depth               

            return gBuffer;
           
        }
    }
}
