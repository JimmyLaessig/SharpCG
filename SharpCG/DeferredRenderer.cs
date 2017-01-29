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


        private static Framebuffer gBuffer;


        public static Framebuffer GBuffer
        {
            get
            {
                if (gBuffer == null)
                {
                    gBuffer = new Framebuffer();

                    // TODO CHANGE THIS
                    int width = 1024;
                    int height = 758;

                    gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment0, new vec4(0));   // Diffuse
                    gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment1, new vec4(0));   // Specular
                    gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment2, new vec4(0));   // Position
                    gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment3, new vec4(0));   // Normals
                    gBuffer.AddRenderTarget(Texture2D.Depth(width, height), FramebufferAttachment.DepthAttachment, new vec4(1));    // Depth
                }
                return gBuffer;
            }
            set { gBuffer = value; }
        }


        public override void OnStart()
        {
            switch (stage)
            {
                case Stage.Geometry:
                    if (mesh == null)                   mesh = sceneObject.Components.OfType<Mesh>().First();                  
                    if(geometryPassMaterial == null)    geometryPassMaterial = sceneObject.FindComponent<GeometryPassMaterial>();
                    break;
                case Stage.Lighting:
                    if (light == null)                  light = sceneObject.FindComponent<Light>();
                    if (lightingPassMaterial == null)   lightingPassMaterial = sceneObject.FindComponent<LightingPassMaterial>();                   
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
                        return GBuffer;

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
                        GBuffer = value;
                        break;
                    case Stage.Lighting:
                        framebuffer = value;
                        break;
                    default:
                        break;
                }
            }
        }


        public Mesh Mesh
        {
            get{return mesh; }
            set{ mesh = value;}
        }


        public Light Light
        {
            get{return light; }
            set{light = value;}
        }


        public GeometryPassMaterial GeometryPassMaterial
        {
            get {return geometryPassMaterial;}
            set {geometryPassMaterial = value;}
        }


        public LightingPassMaterial LightingPassMaterial
        {
            get {return lightingPassMaterial;}
            set {lightingPassMaterial = value; }
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

            GL.Disable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.Src1Alpha, BlendingFactorDest.OneMinusSrc1Alpha);

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
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(false);


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

            // Uniforms for matrices

            var W = sceneObject.Transform.WorldMatrix;
            var V = camera.ViewMatrix;
            var P = camera.ProjectionMatrix;

            if (light.LightType == 0)
            {
                lightingPassMaterial.WorldMatrix        = mat4.Identity;
                lightingPassMaterial.ViewMatrix         = mat4.Identity;
                lightingPassMaterial.ProjectionMatrix   = mat4.Identity;
                lightingPassMaterial.WvpMatrix          = mat4.Identity;
                lightingPassMaterial.NormalMatrix       = mat3.Identity;
            }         
        
            lightingPassMaterial.DiffuseAlbedoTexture   = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment0);
            lightingPassMaterial.SpecularAlbedoTexture  = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment1);
            lightingPassMaterial.WorldPositionTexture   = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment2);
            lightingPassMaterial.WorldNormalTexture     = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment3);
          


            lightingPassMaterial.LightType          = light.LightType;
            lightingPassMaterial.LightPosition      = light.Position;
            lightingPassMaterial.LightDirection     = light.Direction;
            lightingPassMaterial.LightColor         = light.Color;
            lightingPassMaterial.LightAttenuation   = light.Attenuation;

            lightingPassMaterial.CameraPosition     = camera.Transform.Position;

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