using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Rendering;
using SharpCG.Base.Scenegraph;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG.Base.Scenegraph
{

    public enum Stage
    {
        Geometry, Lighting
    }

    public class DeferredRenderer : Component, IRenderer
    {
        private Stage stage = Stage.Geometry;

        private RenderPass renderPass;

        private GeometryPassMaterial geometryPassMaterial;
        private LightingPassMaterial lightingPassMaterial;

        private Mesh mesh;

        private Framebuffer gBuffer;
        private Framebuffer framebuffer;

        public override void OnStart()
        {
            switch(stage)
            {
                case Stage.Geometry:
                    mesh = sceneObject.Components.OfType<Mesh>().First();
                    geometryPassMaterial = sceneObject.FindComponent<GeometryPassMaterial>();
                    break;
                case Stage.Lighting:
                    // TODO FIND LIGHT ATTACHED TO THIS SCENEOBJECT
                    lightingPassMaterial = sceneObject.FindComponent<LightingPassMaterial>();
                    break;
            }

        }



        public RenderPass RenderPass
        {
            get { return renderPass; }
            set { renderPass = value; }
        }

        public Stage Stage
        {
            get
            {
                return stage;
            }

            set
            {
                stage = value;
            }
        }

        public Framebuffer Framebuffer
        {
            get
            {            
                switch (stage)
                {
                    case Stage.Geometry:
                        return gBuffer;
                        
                    case Stage.Lighting:
                        return framebuffer;
                    default :
                        return null;                      
                }               
            }
            set
            {
                switch (stage)
                {
                    case Stage.Geometry:
                        gBuffer = value;
                        break;
                    case Stage.Lighting:
                        framebuffer = value;
                        break;
                    default:
                        break;
                }
            }
        }


        public void Render()
        {
            switch (stage)
            {
                case Stage.Geometry:
                    RenderGeometry();
                    break;
                case Stage.Lighting:
                    RenderLight();
                    break;
            }
        }


        private void RenderGeometry()
        {
            Camera camera = Camera.Main;
            GL.Enable(EnableCap.CullFace);

            //GL.DepthMask(true);

            // Uniforms for matrices

            var W = sceneObject.Transform.WorldMatrix;
            var V = camera.ViewMatrix;
            var P = camera.ProjectionMatrix;

            geometryPassMaterial.WorldMatrix            = W;
            geometryPassMaterial.ViewMatrix             = V;
            geometryPassMaterial.ProjectionMatrix       = P;
            geometryPassMaterial.WvpMatrix              = P * V * W;
            geometryPassMaterial.NormalMatrix           = sceneObject.Transform.NormalMatrix;
            geometryPassMaterial.NormalMappingEnabled   = true;


            uint unit = 0;
            geometryPassMaterial.Bind(ref unit);

            mesh.Bind();
            GL.DrawElements(BeginMode.Triangles, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            geometryPassMaterial.Shader.release();

            //GL.Enable(EnableCap.CullFace);
            //GL.DepthMask(true);
        }


        private void RenderLight()
        {

        }
    }
}
