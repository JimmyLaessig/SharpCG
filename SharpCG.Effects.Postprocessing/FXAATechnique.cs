using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Core;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;

namespace SharpCG.Effects.Postprocessing
{
    public class FXAATechnique : Renderer
    {
        private PostprocessingMaterial material;

        private Texture2D depthTexture;

        private Texture2D colorTexture;

        private Geometry fullscreenQuad;


        public Texture2D DepthTexture
        {
            get{return depthTexture;}
            set{depthTexture = value;}
        }


        public Texture2D ColorTexture
        {
            get{return colorTexture;}
            set{colorTexture = value;}
        }


        public PostprocessingMaterial Material
        {
            get{return material;}
            set{material = value;}
        }


        public override void OnStart()
        {
            fullscreenQuad = GeometryExtensions.FullscreenQuad;

            material = sceneObject.FindComponent<PostprocessingMaterial>();
            base.OnStart();
        }


        public override void RenderGL()
        {
            GL.Viewport(0, 0, 1024, 768);
            Camera camera = Camera.Main;


            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

            // Uniforms for matrices

            var W = sceneObject.Transform.WorldMatrix;
            var V = camera.ViewMatrix;
            var P = camera.ProjectionMatrix;


            material.NormalMatrix       = dmat3.Identity;
            material.WorldMatrix        = dmat4.Identity;
            material.ViewMatrix         = Camera.Main.ViewMatrix;
            material.ProjectionMatrix   = Camera.Main.ProjectionMatrix;
            material.WvpMatrix          = dmat4.Identity;
            material.ViewportSize       = new vec2(1024, 768);
          
            material.ColorTexture = colorTexture;
            material.DepthTexture = depthTexture; 


            material.Bind();

            fullscreenQuad.Bind();


            if (fullscreenQuad.HasIndices)
            {
                GL.DrawElements(fullscreenQuad.PrimitiveType, fullscreenQuad.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                GL.DrawArrays(fullscreenQuad.PrimitiveType, 0, fullscreenQuad.TriangleCount * 3);
            }
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
        }
    }
}
