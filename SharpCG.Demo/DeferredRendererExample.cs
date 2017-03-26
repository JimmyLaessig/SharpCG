using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using SharpCG;
using GlmSharp;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SharpCG.Core;
using SharpCG.Effects;


namespace SharpCG.Demo
{
    public class DeferredRendererExample
    {
        static void Main(string[] args)
        {
            // Create window
            Window window = Window.CreateSimpleWindow(1024, 768);
            window.Runtime.ClearColor = OpenTK.Graphics.Color4.Black;
            Shader.InitializeShaders();



            var geometryPass    = RenderPass.Main;
            var shadowPass      = RenderPass.After(geometryPass, "Shadows");
            var lightingPass    = RenderPass.After(geometryPass, "Lighting");
            var preLightingPass = RenderPass.Before(lightingPass, "beforeLighting");


            // Create GBuffer
            var gBuffer = Framebuffer.GBuffer;
            // Clear GBuffer before geometryPass
            window.Runtime.AddRenderEvent(geometryPass, (() => gBuffer.Clear()));

            {
                SceneObject plane = GeometryExtensions.Load("Assets/plane/plane.fbx");
                plane.Name = "plane";

                //plane.Transform.Scale = new dvec3(5);
                plane = Templates.Templates.CreateDeferred(plane, gBuffer, geometryPass);

                window.AddSceneObject(plane);
            }
            {
                SceneObject trooper = GeometryExtensions.Load("Assets/stormtrooper/stormtrooper.fbx");
                trooper.Name    = "stormtrooper";
                trooper         = Templates.Templates.CreateDeferred(trooper, gBuffer, geometryPass);    
                window.AddSceneObject(trooper);
            }


            // Copy depth texture to back buffer
            {
                window.AddSceneObject(Templates.Templates.RenderDepthTexture(gBuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment), null, preLightingPass));
            }

            //Add Ambient Light
            {
                //SceneObject lightObject = new SceneObject();
                //lightObject.Name        = "Ambient Light";
                //var light               = lightObject.AddComponent<AmbientLight>();
                //light.Color             = new dvec3(0.25f, 0.25f, 0.25f);
                //var renderer            = lightObject.AddComponent<MeshRenderer>();
                //var material            = lightObject.AddComponent<DeferredLightMaterial>();
                //material.DiffuseAlbedoTexture   = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment0);
                //material.SpecularAlbedoTexture  = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment1);
                //material.WorldNormalTexture     = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment2);
                //material.DepthTexture           = gBuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);
                
                //renderer.Material       = material;

                //renderer.RenderPass     = lightingPass;
                
                
                ////renderer.Framebuffer    = gBuffer;

                //window.AddSceneObject(lightObject);
            }


            //Add Directional Light
            {
                //SceneObject lightObject = new SceneObject();
                //lightObject.Name = "Directional Light";

                //var light = lightObject.AddComponent<DirectionalLight>();

                //light.Color = new dvec3(1);
                //light.Direction = new dvec3(0f, -1f, 0f);
                //var renderer = lightObject.AddComponent<DeferredMeshRenderer>();
                //renderer.RenderPass = lightingPass;
                //renderer.Stage = Stage.Lighting;
                //renderer.GBuffer = gBuffer;

                //var material = lightObject.AddComponent<DeferredLightMaterial>();
                //renderer.LightingPassMaterial = material;


                //// Shadows
                //var framebuffer = new Framebuffer();
                //framebuffer.AddRenderTarget(Texture2D.Depth(2000, 2000), FramebufferAttachment.DepthAttachment, vec4.Ones);
                //window.RenderControl.AddImmediateGLEvent(shadowPass, (() => framebuffer.Clear()));


                //var shadows = lightObject.AddComponent<ShadowMapRenderer>();
                //shadows.RenderPass = shadowPass;
                //shadows.Framebuffer = framebuffer;

                //window.AddSceneObject(lightObject);
            }

            //Add Point Light
            {
                //SceneObject pointLight = new SceneObject();
                //pointLight.Name = "Point Light";

                //var light = pointLight.AddComponent<PointLight>();

                //light.Color = new dvec3(1, 0, 0);
                //var scale1 = light.SceneObject.Transform.WorldScale;
                //light.Attenuation = new dvec3(1.0f, 0.7f, 1.8f);

                //var scale2 = light.SceneObject.Transform.WorldScale;
                //light.SceneObject.Transform.WorldScale = light.SceneObject.Transform.WorldScale * 2.0f;
                //var scale3 = light.SceneObject.Transform.WorldScale;
                //light.Position = new dvec3(0, 0.5f, 0);

                //var renderer = pointLight.AddComponent<DeferredMeshRenderer>();
                //renderer.RenderPass = lightingPass;
                //renderer.Stage = Stage.Lighting;
                //renderer.GBuffer = gBuffer;

                //var material = pointLight.AddComponent<DeferredLightMaterial>();
                //renderer.LightingPassMaterial = material;

                //window.AddSceneObject(pointLight);
            }

            window.Run();
        }
    }
}
