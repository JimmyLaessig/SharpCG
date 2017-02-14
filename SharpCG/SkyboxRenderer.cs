using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;



using GlmSharp;


namespace SharpCG
{
    public class SkyboxRenderer : Renderer
    {

        private Geometry mesh;
        private SkyboxMaterial material;

       

        public override void OnStart()
        {
            mesh = (Geometry)sceneObject.Components.Find(c => c is Geometry);
            material = (SkyboxMaterial)sceneObject.Components.Find(c => c is SkyboxMaterial);
        }

        public override void RenderGL()
        {
            Camera camera = Camera.Main;

            GL.Disable(EnableCap.CullFace);
            GL.DepthMask(false);
            

            mat4 W = mat4.Translate(camera.Transform.Position);
            mat4 V = camera.ViewMatrix;
            mat4 P = camera.ProjectionMatrix;


            material.WorldMatrix = W;
            material.ViewMatrix = V;
            material.ProjectionMatrix = P;
            material.WvpMatrix = P * V * W;


            uint unit = 0;
            material.Bind(ref unit);

            mesh.Bind();
            GL.DrawElements(BeginMode.Triangles, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
           

            GL.Enable(EnableCap.CullFace);
            GL.DepthMask(true);

        }

        public RenderPass GetRenderPass()
        {
            return renderPass;
        }
    }
}

