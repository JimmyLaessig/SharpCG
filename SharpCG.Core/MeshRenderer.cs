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
            if(material == null)          
                material = sceneObject.FindComponent<Material>();           
        }


        public override void RenderGL()
        {
            if (material == null)
                return;

           
            // Default Uniforms for matrices
            if (Camera == null)
            {          
                material.ProjectionMatrix   = dmat4.Identity;
                material.ViewMatrix         = dmat4.Identity;              
            }
            else
            {
                material.ProjectionMatrix   = Camera.ProjectionMatrix;
                material.ViewMatrix         = Camera.ViewMatrix;
            }
            
            material.ViewportSize = (framebuffer == null) ? new vec2(this.sceneObject.Runtime.Width, this.sceneObject.Runtime.Height) : material.ViewportSize = new vec2(framebuffer.Width, framebuffer.Height);
 
            material.WorldMatrix    = sceneObject.Transform.WorldMatrix;
            material.NormalMatrix   = sceneObject.Transform.NormalMatrix;
            material.WvpMatrix      = material.ProjectionMatrix * material.ViewMatrix * material.WorldMatrix;


            GLState.Bind();
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
        }
    }
}
