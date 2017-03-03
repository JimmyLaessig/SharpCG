using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using OpenTK.Graphics.OpenGL4;


namespace SharpCG.Core
{
    public class MeshRenderer : Renderer
    {

        private Geometry mesh;
        private Material material;


        public override void OnStart()
        {
            if (mesh == null)
                mesh = sceneObject.FindComponent<Geometry>();

            if(mesh != null)
                material = mesh.Material;
        }


        public override void RenderGL()
        {
            if (material == null)
                return;

            Camera camera = Camera.Main;

            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Disable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.Src1Alpha, BlendingFactorDest.OneMinusSrc1Alpha);

            // Default Uniforms for matrices
            material.WorldMatrix        = sceneObject.Transform.WorldMatrix;
            material.ProjectionMatrix   = camera.ProjectionMatrix;
            material.ViewMatrix         = camera.ViewMatrix;
            material.WvpMatrix          = material.ProjectionMatrix * material.ViewMatrix * material.WorldMatrix;
            material.NormalMatrix       = sceneObject.Transform.NormalMatrix;

            
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
            //GL.BindVertexArray(0);

            material.Shader.release();
        }
    }
}
