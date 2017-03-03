using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCG.Demo
{
    public class DeferredRendererExample
    {
        //static void Main(string[] args)
        //{
            // Copy depth texture to back buffer
            //{
                //var obj = SceneObjectExtensions.CopyDepthBuffer(gBuffer, null, beforeLighting);
                //window.AddSceneObject(obj);
            //}
            // Add Ambient Light
            //{
                //SceneObject lightObject = new SceneObject();
                //lightObject.Name = "Ambient Light";
                //var light = lightObject.AddComponent<AmbientLight>();
                //light.Color = new dvec3(0.25f, 0.25f, 0.25f);
                //var material = lightObject.AddComponent<LightingPassMaterial>();

                //var renderer = lightObject.AddComponent<DeferredMeshRenderer>();
                //renderer.RenderPass = lightingPass;
                //renderer.Stage = Stage.Lighting;
                //renderer.LightingPassMaterial = material;
                //renderer.GBuffer = gBuffer;

                //window.AddSceneObject(lightObject);
            //}


            // Add Directional Light
            //{
                //SceneObject lightObject = new SceneObject();
                //lightObject.Name = "Directional Light";

                //var light = lightObject.AddComponent<DirectionalLight>();

                //light.Color = new dvec3(1);
                //light.Direction = new dvec3(0f, -1f, 0f);
                //var renderer = lightObject.AddComponent<DeferredMeshRenderer>();
                //renderer.RenderPass = lightingPass;
                //renderer.Stage = Stage.Lighting;
                //renderer.GBuffer = gBuffer;

                //var material = lightObject.AddComponent<LightingPassMaterial>();
                //renderer.LightingPassMaterial = material;


                //// Shadows
                //var framebuffer = new Framebuffer();
                //framebuffer.AddRenderTarget(Texture2D.Depth(2000, 2000), FramebufferAttachment.DepthAttachment, vec4.Ones);
                //window.RenderControl.AddImmediateGLEvent(shadowPass, (() => framebuffer.Clear()));


                //var shadows = lightObject.AddComponent<ShadowMapRenderer>();
                //shadows.RenderPass = shadowPass;
                //shadows.Framebuffer = framebuffer;

                //window.AddSceneObject(lightObject);
            //}

            // Add Point Light
            //{
                //SceneObject pointLight = new SceneObject();
                //pointLight.Name = "Point Light";

                //var light = pointLight.AddComponent<PointLight>();

                //light.Color         = new dvec3(1, 0, 0);
                //var scale1 = light.SceneObject.Transform.WorldScale;
                //light.Attenuation   = new dvec3(1.0f, 0.7f, 1.8f);

                //var scale2 = light.SceneObject.Transform.WorldScale;
                //light.SceneObject.Transform.WorldScale = light.SceneObject.Transform.WorldScale * 2.0f;
                //var scale3 = light.SceneObject.Transform.WorldScale;
                //light.Position      = new dvec3(0, 0.5f, 0);

                //var renderer        = pointLight.AddComponent<DeferredMeshRenderer>();
                //renderer.RenderPass = lightingPass;
                //renderer.Stage      = Stage.Lighting;
                //renderer.GBuffer    = gBuffer;

                //var material = pointLight.AddComponent<LightingPassMaterial>();
                //renderer.LightingPassMaterial = material;

                //window.AddSceneObject(pointLight);
           // }

            //RenderPass geometryPass = RenderPass.Main;

            //RenderPass beforeGeometry = RenderPass.Before(geometryPass, "before Geometry");

            //RenderPass shadowPass = RenderPass.After(geometryPass, "shadowPass");

            //RenderPass lightingPass = RenderPass.After(shadowPass, "LightingPass");
            //RenderPass beforeLighting = RenderPass.Before(lightingPass, "before lighting");
            //RenderPass afterLighting = RenderPass.After(lightingPass, "aftér lighting");

            //var gBuffer = Framebuffer.GBuffer;
            //window.RenderControl.AddImmediateGLEvent(beforeGeometry, (() => gBuffer.Clear()));
        //}
    }
}
