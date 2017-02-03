using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;

namespace SharpCG.Base.Scenegraph
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
            get{ return framebuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);}
        }


        public DirectionalLight Light
        {
            get{return light;}
            set {light = value;}
        }


        public override void RenderGL()
        {
            if (material == null)
                return;

            var shadowCasters = SceneObject.CollectWhere<Mesh>(sceneObject.Runtime.Root, (obj => obj.HasTag("ShadowCaster")));
            GL.Disable(EnableCap.CullFace);

            //GL.DepthMask(true);
            

            shadowCasters.ForEach(mesh => {
                var obj = mesh.SceneObject;

                material.WorldMatrix        = obj.Transform.WorldMatrix;
                material.ProjectionMatrix   = light.LightProjectionMatrix;
                material.ViewMatrix         = light.lightViewMatrix;
                material.WvpMatrix          = material.ProjectionMatrix * material.ViewMatrix * material.WorldMatrix;


                uint unit = 0;
                material.Bind(ref unit);

                mesh.Bind();
                if (mesh.HasIndices)
                {
                    GL.DrawElements(mesh.PrimitiveType, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
                }
                else
                {
                    GL.DrawArrays(mesh.PrimitiveType, 0, mesh.TriangleCount * 3);
                }
                material.Shader.release();
            });            
        }


        public override void DeInitGL()
        {
            base.DeInitGL();
        }


        public int ShadowMapWidth
        {
            get { return width; }
            set { width = value; }
        }


        public int ShadowMaHeight
        {
            get { return height; }
            set { height = value; }
        }
    }
}
