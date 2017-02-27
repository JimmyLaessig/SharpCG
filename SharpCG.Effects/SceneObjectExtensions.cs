using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Core;

namespace SharpCG.Effects
{
    public static class SceneObjectExtensions
    {

        public static SceneObject CopyDepthBuffer(Framebuffer from, Framebuffer target, RenderPass renderPass)
        {
            SceneObject obj = new SceneObject("Depth Texture Copy");
            obj.AddComponent<Geometry>(GeometryExtensions.FullscreenQuad);

            var mat             = obj.AddComponent<TextureToDepthMaterial>();
            mat.DepthTexture    = from.GetRenderTarget(FramebufferAttachment.DepthAttachment);

            var renderer = obj.AddComponent<TextureToDepthRenderer>();
            renderer.Framebuffer    = target;
            renderer.RenderPass     = renderPass;
            return obj;
        }
    }
}
