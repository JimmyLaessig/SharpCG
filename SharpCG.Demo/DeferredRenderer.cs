using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Rendering;
using SharpCG.Base.Scenegraph;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG.Rendering.Deferred

{
    public class DeferredRendererGeometry : Component, IRenderer
    {
        private RenderPass renderPass;

        private Material material;
        private Mesh mesh;


        public override void OnStart()
        {
            mesh = sceneObject.Components.OfType<Mesh>().First();
            material = sceneObject.Components.OfType<Deferred.GeometryPassMaterial>().First() ;
        }

         

        public RenderPass RenderPass
        {
            get { return renderPass; }
        }

        public void Render()
        {
            //Camera camera = Camera.Main;

            //GL.Enable(EnableCap.CullFace);

            ////GL.DepthMask(true);

            //// Uniforms for matrices
            //material.WorldMatrix = sceneObject.Transform.WorldMatrix;
            //material.ProjectionMatrix = camera.ProjectionMatrix;
            //material.ViewMatrix = camera.ViewMatrix;
            //material.WvpMatrix = material.ProjectionMatrix * material.ViewMatrix * material.WorldMatrix;
            //material.NormalMatrix = sceneObject.Transform.NormalMatrix;

            ////material.NormalMappingEnabled = true;

            ////// Set lighting parameters
            ////material.ViewPosition = camera.Transform.Position;

            ////material.LightDirection = new vec3(0, -1, 0);
            ////material.LightColor = new vec3(0.7f, 0.7f, 0.6f);
            ////material.LightAmbientColor = new vec3(0.4f, 0.4f, 0.4f);

            //uint unit = 0;
            //material.Bind(ref unit);

            //mesh.Bind();
            //GL.DrawElements(BeginMode.Triangles, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
            //GL.BindVertexArray(0);

            //material.Shader.release();

            ////GL.Enable(EnableCap.CullFace);
            ////GL.DepthMask(true);
        }
    }



    public class DeferredRendererComposer : Component, IRenderer
    {
        private RenderPass renderPass;


        public RenderPass RenderPass
        {
            get { return renderPass; }
        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
