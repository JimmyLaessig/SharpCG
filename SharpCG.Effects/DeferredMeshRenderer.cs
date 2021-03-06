﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using OpenTK.Graphics.OpenGL4;
//using GlmSharp;
//using SharpCG.Core;

//namespace SharpCG.Effects
//{
//    public enum Stage
//    {
//        Geometry, Lighting
//    }
    

//    public class DeferredMeshRenderer : Renderer
//    {
//        private Stage stage = Stage.Geometry;


//        private DeferredGeometryMaterial geometryPassMaterial;
//        private DeferredLightMaterial lightingPassMaterial;


//        private Geometry mesh;
//        private Light light;
          

//        private Framebuffer gBuffer;


//        public override void OnStart()
//        {
//            switch (stage)
//            {
//                case Stage.Geometry:
//                    if (mesh == null)                   mesh                    = sceneObject.Components.OfType<Geometry>().First();                  
//                    if(geometryPassMaterial == null)    geometryPassMaterial    = sceneObject.FindComponent<DeferredGeometryMaterial>();                    
//                    break;
//                case Stage.Lighting:
//                    if (light == null)                  light                   = sceneObject.FindComponent<Light>();
//                    if (lightingPassMaterial == null)   lightingPassMaterial    = sceneObject.FindComponent<DeferredLightMaterial>();                   
//                    break;
//            }
//        }


//        public override Framebuffer Framebuffer
//        {
//            get
//            {
//                if (stage == Stage.Geometry)
//                    return gBuffer;
//                else
//                    return framebuffer;
//            }
//            set
//            {
//                if (stage == Stage.Geometry)
//                    gBuffer = value;
//                else
//                    framebuffer = value;
//            }
//        }


//        public Stage Stage
//        {
//            =>return stage;}
//            =>stage = value;}
//        }    


//        public Geometry Mesh
//        {
//            =>return mesh; }
//            => mesh = value;}
//        }


//        public Light Light
//        {
//            =>return light; }
//            =>light = value;}
//        }


//        public DeferredGeometryMaterial GeometryPassMaterial
//        {
//            get {return geometryPassMaterial;}
//            set {geometryPassMaterial = value;}
//        }


//        public DeferredLightMaterial LightingPassMaterial
//        {
//            get {return lightingPassMaterial;}
//            set {lightingPassMaterial = value; }
//        }


//        public Framebuffer GBuffer
//        {
//            =>return gBuffer;}
//            =>gBuffer = value;}
//        }


//        public override void RenderGL()
//        {
//            switch (stage)
//            {
//                case Stage.Geometry:
//                    RenderGeometry();
//                    break;
//                case Stage.Lighting:
//                    RenderLight();
//                    break;
//            }
//        }


//        private void RenderGeometry()
//        {
//            Camera camera = Camera.Main;

//            GL.Enable(EnableCap.CullFace);
//            GL.CullFace(CullFaceMode.Back);      
//            GL.Enable(EnableCap.DepthTest);
           
//            GL.DepthMask(true);

//            GL.Disable(EnableCap.Blend);
//            //GL.BlendFunc(BlendingFactorSrc.Src1Alpha, BlendingFactorDest.OneMinusSrc1Alpha);

//            // Uniforms for matrices


//            var W = sceneObject.Transform.WorldMatrix;
//            var V = camera.ViewMatrix;
//            var P = camera.ProjectionMatrix;
            
//            geometryPassMaterial.WorldMatrix        = W;
//            geometryPassMaterial.ViewMatrix         = V;
//            geometryPassMaterial.ProjectionMatrix   = P;
//            geometryPassMaterial.WvpMatrix          = P * V * W;
//            geometryPassMaterial.NormalMatrix       = sceneObject.Transform.NormalMatrix;                      

            
//            geometryPassMaterial.Bind();

//            mesh.Bind();
//            GL.DrawElements(BeginMode.Triangles, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
//            //GL.BindVertexArray(0);

//            //geometryPassMaterial.Shader.release();

//            //GL.Enable(EnableCap.CullFace);
//            //GL.DepthMask(true);
//        }


//        private void RenderLight()
//        {

//            GL.Viewport(0, 0, 1024, 768);
//            Camera camera = Camera.Main;


//            GL.Disable(EnableCap.CullFace);           
//            GL.Enable(EnableCap.DepthTest);
//            GL.DepthMask(false);
            

//            GL.Enable(EnableCap.Blend);
//            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

//            // Uniforms for matrices

//            var W = sceneObject.Transform.WorldMatrix;
//            var V = camera.ViewMatrix;
//            var P = camera.ProjectionMatrix;
                      
//            lightingPassMaterial.WorldMatrix        = W;
//            lightingPassMaterial.ViewMatrix         = V;
//            lightingPassMaterial.ProjectionMatrix   = P;

//            if (light.LightType == 0 || light.LightType == 1 )
//            {
//                lightingPassMaterial.WvpMatrix = dmat4.Identity;
//            }
//            else
//            {
//                GL.Enable(EnableCap.CullFace);
//                lightingPassMaterial.WvpMatrix = P * V * W;
//            }

//            lightingPassMaterial.NormalMatrix           = dmat3.Identity;          
        
//            lightingPassMaterial.DiffuseAlbedoTexture   = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment0);
//            lightingPassMaterial.SpecularAlbedoTexture  = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment1);
//            lightingPassMaterial.WorldNormalTexture     = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment2);
//            lightingPassMaterial.DepthTexture           = gBuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);


//            lightingPassMaterial.LightType          = light.LightType;
//            lightingPassMaterial.LightPosition      = light.Position;
//            lightingPassMaterial.LightDirection     = light.Direction;
//            lightingPassMaterial.LightColor         = light.Color;
//            lightingPassMaterial.LightAttenuation   = light.Attenuation;
            

//            lightingPassMaterial.InverseViewProjectionMatrix    = V.Inverse * P.Inverse;
//            lightingPassMaterial.CameraPosition                 = camera.Transform.WorldPosition;

//            if (light.ShadowMap != null)
//            {
//                lightingPassMaterial.ShadowMapTexture = light.ShadowMap;
//                lightingPassMaterial.LightViewProjectionMatrix = light.ProjectionMatrix * Light.ViewMatrix;
//            }

            
//            lightingPassMaterial.Bind();

//            light.LightGeometry.Bind();


//            if (light.LightGeometry.HasIndices)
//            {
//                GL.DrawElements(light.LightGeometry.PrimitiveType, light.LightGeometry.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
//            }
//            else
//            {
//                GL.DrawArrays(light.LightGeometry.PrimitiveType, 0, light.LightGeometry.TriangleCount * 3);
//            }
//            GL.BindVertexArray(0);

//            lightingPassMaterial.Shader.release();



//            GL.Enable(EnableCap.CullFace);           
//            GL.Enable(EnableCap.DepthTest);
//            GL.DepthMask(true);
//        }
//    }
//}