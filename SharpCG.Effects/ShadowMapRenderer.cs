using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;

using SharpCG.Core;


namespace SharpCG.Effects
{
    public class ShadowMapRenderer : Renderer
    {

        private DirectionalLight light = null;
        private int width   = 1000;
        private int height  = 1000;


        private ShadowMapMaterial material;


        public override void OnStart()
        {          
            if(light == null)
            {
                light = sceneObject.FindComponent<DirectionalLight>();
            }
            if (material == null)
            {
                material = sceneObject.AddComponent<ShadowMapMaterial>();
                material.OnStart();
            }  
        }


        public override void InitGL()
        {
            base.InitGL();
        }


        public override void LateInitGL()
        {
            base.LateInitGL();
        }


        public Texture2D DepthTexture
        {
            get=> framebuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);
        }


        public DirectionalLight Light
        {
            get => light;
            set => light = value;
        }


        public override void RenderGL()
        {
            if (material == null)
                return;

            var shadowCasters = SceneObject.CollectWhere<Geometry>(sceneObject.Runtime.Root, (obj => obj.HasTag("ShadowCaster")));
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);
           
            GL.PolygonOffset(1.1f, 4.0f);
       

            shadowCasters.ForEach(mesh => 
            {
                var obj = mesh.SceneObject;

                material.WorldMatrix        = obj.Transform.WorldMatrix;
                material.ProjectionMatrix   = light.ProjectionMatrix;
                material.ViewMatrix         = light.ViewMatrix;
                material.WvpMatrix          = material.ProjectionMatrix * material.ViewMatrix * material.WorldMatrix;

               
                material.Bind();

                mesh.Bind();
                if (mesh.HasIndices)
                {
                    GL.DrawElements(mesh.PrimitiveType, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    GL.DrawArrays(mesh.PrimitiveType, 0, mesh.TriangleCount * 3);
                }
                material.Shader.Release();
            });


            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.PolygonOffset(0.0f, 0.0f);
        }


        public override void DeInitGL()
        {
            base.DeInitGL();
        }


        public int ShadowMapWidth
        {
            get => width; 
            set => width = value;
        }


        public int ShadowMaHeight
        {
            get => height; 
            set => height = value; 
        }
    }
}
