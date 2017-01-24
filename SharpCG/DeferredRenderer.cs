using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using GlmSharp;


namespace SharpCG
{
    public enum Stage
    {
        Geometry, Lighting
    }

    public class DeferredRenderer : Renderer
    {
        private Stage stage = Stage.Geometry;


        private GeometryPassMaterial geometryPassMaterial;
        private LightingPassMaterial lightingPassMaterial;

        private Mesh mesh;
        private Light light;

        private Framebuffer gBuffer;


        public override void OnStart()
        {
            switch (stage)
            {
                case Stage.Geometry:
                    mesh = sceneObject.Components.OfType<Mesh>().First();
                    geometryPassMaterial = sceneObject.FindComponent<GeometryPassMaterial>();
                    break;
                case Stage.Lighting:
                    light = sceneObject.FindComponent<Light>();
                    lightingPassMaterial = sceneObject.FindComponent<LightingPassMaterial>();
                    
                    break;
            }
        }


        public Stage Stage
        {
            get{return stage;}
            set{stage = value;}
        }


        public override Framebuffer Framebuffer
        {
            get
            {
                switch (stage)
                {
                    case Stage.Geometry:
                        return gBuffer;

                    case Stage.Lighting:
                        return framebuffer;
                    default:
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


        public Framebuffer GBuffer
        {
            get
            {
                return gBuffer;
            }

            set
            {
                gBuffer = value;
            }
        }


        public override void RenderGL()
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
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            

            // Uniforms for matrices

            var W = sceneObject.Transform.WorldMatrix;
            var V = camera.ViewMatrix;
            var P = camera.ProjectionMatrix;

            geometryPassMaterial.WorldMatrix = W;
            geometryPassMaterial.ViewMatrix = V;
            geometryPassMaterial.ProjectionMatrix = P;
            geometryPassMaterial.WvpMatrix = P * V * W;
            geometryPassMaterial.NormalMatrix = sceneObject.Transform.NormalMatrix;
            geometryPassMaterial.NormalMappingEnabled = true;


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

            GL.Viewport(0, 0, 1024, 768);
            Camera camera = Camera.Main;

            GL.Disable(EnableCap.CullFace);           
            GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);

            // Uniforms for matrices

            var W = sceneObject.Transform.WorldMatrix;
            var V = camera.ViewMatrix;
            var P = camera.ProjectionMatrix;

            if (light.LightType == 0)
            {
                lightingPassMaterial.WorldMatrix = mat4.Identity;
                lightingPassMaterial.ViewMatrix = mat4.Identity;
                lightingPassMaterial.ProjectionMatrix = mat4.Identity;
                lightingPassMaterial.WvpMatrix = mat4.Identity;
                lightingPassMaterial.NormalMatrix = mat3.Identity;
            }


            lightingPassMaterial.WorldPositionTexture = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment0);
            lightingPassMaterial.WorldNormalTexture = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment1);
            lightingPassMaterial.DiffuseAlbedoTexture = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment2);
            lightingPassMaterial.SpecularAlbedoTexture = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment3);


            lightingPassMaterial.LightType = light.LightType;
            lightingPassMaterial.LightPosition = light.Position;
            lightingPassMaterial.LightDirection = light.Direction;
            lightingPassMaterial.LightColor = light.Color;
            lightingPassMaterial.LightAttenuation = light.Attenuation;

            lightingPassMaterial.CameraPosition = camera.Transform.Position;

            uint unit = 0;
            lightingPassMaterial.Bind(ref unit);

            light.LightGeometry.Bind();
            GL.DrawElements(BeginMode.Triangles, light.LightGeometry.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            lightingPassMaterial.Shader.release();

            //GL.Enable(EnableCap.CullFace);
            //GL.DepthMask(true);

            GL.Enable(EnableCap.CullFace);           
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
        }
    }
}