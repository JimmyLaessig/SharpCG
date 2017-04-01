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


            var geometryPass = RenderPass.Main;
            var shadowPass = RenderPass.After(geometryPass, "Shadows");
            var lightingPass = RenderPass.After(geometryPass, "Lighting");
            var preLightingPass = RenderPass.Before(lightingPass, "beforeLighting");


            {
                // Create Camera
                Camera.Main.SetProjectionMatrix(60.0, (double)window.Width / (double)window.Height, 0.1, 100000.0);
                Camera.Main.Transform.Position = new dvec3(0.0, 2.5, 5);
                Camera.Main.Transform.Rotation = dquat.FromAxisAngle(glm.Radians(-30.0), dvec3.UnitX);
                Camera.Main.Transform.LookAt(new dvec3(10,10, 10), dvec3.Zero, dvec3.UnitY);
                Camera.Main.SceneObject.AddComponent<CameraController>();
                window.AddSceneObject(Camera.Main.SceneObject);
            }


            //Create GBuffer
            var gBuffer = Templates.Templates.GBuffer(window.Width, window.Height);
            //Clear GBuffer before geometryPass
            window.Runtime.AddRenderEvent(geometryPass, (() => gBuffer.Clear()));
            {
                SceneObject skybox = Templates.Templates.SkyBox(TextureCubeMap.Load("Assets/skybox/skybox_left2048.png",
                                                                                    "Assets/skybox/skybox_right2048.png",
                                                                                    "Assets/skybox/skybox_top2048.png",
                                                                                    "Assets/skybox/skybox_bottom2048.png",
                                                                                    "Assets/skybox/skybox_front2048.png",
                                                                                    "Assets/skybox/skybox_back2048.png"), 
                                                                                    gBuffer, 
                                                                                    geometryPass);
                window.AddSceneObject(skybox);
            }
            { 
                //SceneObject plane = GeometryExtensions.Load("Assets/plane/plane.fbx");               
                //plane.Name = "plane";                                          

                //plane.Transform.Scale = new dvec3(2);
                //plane = Templates.Templates.CreateDeferred(plane, gBuffer, geometryPass);

                //window.AddSceneObject(plane);
            }

            {
                //SceneObject trooper = GeometryExtensions.Load("Assets/stormtrooper/stormtrooper.fbx");
                //trooper.Name = "stormtrooper";
                //trooper = Templates.Templates.CreateDeferred(trooper, gBuffer, geometryPass);
                //window.AddSceneObject(trooper);
            }

            {
                SceneObject box1 = GeometryExtensions.Load("Assets/sponza/sponza.obj");
                //SceneObject box1 = GeometryExtensions.Load("Assets/container/container.fbx");
                box1.Name = "box1";
                box1 = Templates.Templates.CreateDeferred(box1, gBuffer, geometryPass);
                box1.Transform.WorldPosition = new dvec3(0.5, 0.25, 0.0);
                window.AddSceneObject(box1);
            }
            {
                //SceneObject box1 = GeometryExtensions.Load("Assets/container/container.fbx");
                //box1.Name = "box1";
                //box1 = Templates.Templates.CreateDeferred(box1, gBuffer, geometryPass);
                //box1.Transform.WorldPosition = new dvec3(-0.5, 0.25, 0.0);
                //window.AddSceneObject(box1);
            }




            // Copy depth texture to back buffer
            {
              // window.AddSceneObject(Templates.Templates.RenderDepthTexture(gBuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment), null, preLightingPass));
            }

            //Add Ambient Light
            {
                SceneObject lightObject = Templates.Templates.AmbientLight(new dvec3(0.5), gBuffer, lightingPass);
                lightObject.Name = "Ambient Light";
                window.AddSceneObject(lightObject);
            }

            //Add Directional Light
            {
                //SceneObject light1 = Templates.Templates.DirectionalLight(new dvec3(1.0, 1.0, 1.0), new dvec3(0.0, -1.0, -1.0), gBuffer, lightingPass);
                //light1.Name = "Directional Light";
                //// Shadows
                ////var framebuffer = new Framebuffer();
                ////framebuffer.AddRenderTarget(Texture2D.Depth(2000, 2000), FramebufferAttachment.DepthAttachment, vec4.Ones);
                ////window.RenderControl.AddImmediateGLEvent(shadowPass, (() => framebuffer.Clear()));


                ////var shadows = lightObject.AddComponent<ShadowMapRenderer>();
                ////shadows.RenderPass = shadowPass;
                ////shadows.Framebuffer = framebuffer;

                //window.AddSceneObject(light1);
            }

            //Add Point Light
            {
                //SceneObject pointLight = Templates.Templates.PointLight(new dvec3(0, 0.25, 0), new dvec3(1.0, 0.0, 0.0), new dvec3(1.0, 0.7, 1.8) * 5.0, gBuffer, lightingPass);
                //pointLight.Name = "Point Light";
                //window.AddSceneObject(pointLight);
            }

            window.Run();
        }
    }
}
