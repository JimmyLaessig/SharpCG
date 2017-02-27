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
    class Program
    {
        static void Main(string[] args)
        {
            Window window = Window.CreateSimpleWindow(1024, 768);
            
            Shader.InitializeShaders();

           

            {
                // Create Camera
                Camera.Main.SetProjectionMatrix(60.0, (double)window.Width / (double)window.Height, 0.1, 100000.0);
                Camera.Main.Transform.Position = new dvec3(0.0, 2.5, 5);
                Camera.Main.Transform.Rotation = dquat.FromAxisAngle(glm.Radians(-30.0), dvec3.UnitX);

                Camera.Main.SceneObject.AddComponent<CameraController>();
                window.AddSceneObject(Camera.Main.SceneObject);
            }
            {
                //var sphereObj   = new SceneObject();
                //sphereObj.Name  = "TesselatedSphere";
                //sphereObj.AddComponent(GeometryExtensions.Sphere(vec3.Zero, 1, 20));
                ////sphereObj.AddComponent(GeometryExtensions.FullscreenQuad);
                //sphereObj.AddComponent<SimpleRenderer>();
                //var mat = sphereObj.AddComponent<ColoredMaterial>();
                //mat.Color = new vec4(1, 0, 0, 1);

                //window.AddSceneObject(sphereObj);

            }
            //{
            //    SceneObject skybox = new SceneObject();
            //    skybox.Name = "Skybox";
            //    skybox.AddComponent(GeometryExtensions.UnitCube);
            //    var skyboxMat           = skybox.AddComponent<SkyboxMaterial>();
            //    skyboxMat.CubeMapTexture = TextureCubeMap.Load( "Assets/skybox/skybox_left2048.png",
            //                                                    "Assets/skybox/skybox_right2048.png",
            //                                                    "Assets/skybox/skybox_top2048.png",
            //                                                    "Assets/skybox/skybox_bottom2048.png",
            //                                                    "Assets/skybox/skybox_front2048.png",
            //                                                    "Assets/skybox/skybox_back2048.png");

            //    var renderer = skybox.AddComponent<SkyboxRenderer>();
            //    //renderer.Framebuffer = gBuffer;
            //    renderer.RenderPass = RenderPass.Before(RenderPass.Main , "skybox");


            //    window.AddSceneObject(skybox);
            //}
            //RenderPass geometryPass = RenderPass.Main;

            //RenderPass beforeGeometry = RenderPass.Before(geometryPass, "before Geometry");

            //RenderPass shadowPass = RenderPass.After(geometryPass, "shadowPass");

            //RenderPass lightingPass = RenderPass.After(shadowPass, "LightingPass");
            //RenderPass beforeLighting = RenderPass.Before(lightingPass, "before lighting");
            //RenderPass afterLighting = RenderPass.After(lightingPass, "aftér lighting");

            //var gBuffer = Framebuffer.GBuffer;
            //window.RenderControl.AddImmediateGLEvent(beforeGeometry, (() => gBuffer.Clear()));

            {
                //SceneObject skybox = new SceneObject();
                //skybox.Name = "Skybox";
                //skybox.AddComponent(GeometryExtensions.UnitCube);
                //var skyboxMat = skybox.AddComponent<SkyboxMaterial>();
                //skyboxMat.CubeMapTexture = TextureCubeMap.Load("Assets/skybox/skybox_left2048.png",
                //                                                "Assets/skybox/skybox_right2048.png",
                //                                                "Assets/skybox/skybox_top2048.png",
                //                                                "Assets/skybox/skybox_bottom2048.png",
                //                                                "Assets/skybox/skybox_front2048.png",
                //                                                "Assets/skybox/skybox_back2048.png");

                //var renderer = skybox.AddComponent<SkyboxRenderer>();
                //renderer.Framebuffer = gBuffer;
                //renderer.RenderPass = beforeLighting;


                //window.AddSceneObject(skybox);
            }


            // Copy depth texture to back buffer
            {
                //var obj = SceneObjectExtensions.CopyDepthBuffer(gBuffer, null, beforeLighting);
                //window.AddSceneObject(obj);
            }

            {               

                SceneObject plane = GeometryExtensions.Load("Assets/stormtrooper/stormtrooper.fbx");
                plane.Name = "stormtrooper";
                
                //plane.Transform.Scale = new dvec3(5);


                SceneObject.TraverseAndExecute<Geometry>(plane, m =>
                {
                    var obj = m.SceneObject;
                    obj.AddComponent<MeshRenderer>();
                });


                window.AddSceneObject(plane);
            }


            // Add Ambient Light
            {
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
            }


            // Add Directional Light
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
            }

            // Add Point Light
            {
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
            }

            
            printSceneGraph(window.Root, 0);

            window.Run();
        }


       
   
        public static void printSceneGraph(SceneObject obj, int level)
        {
            var oldC = Console.ForegroundColor;
            
            
            string sceneObjectTabbing   = string.Concat(Enumerable.Repeat("|  ", Math.Max(0,(level - 1)))) + "|-- ";
            string componentTabbing     = string.Concat(Enumerable.Repeat("|  ", Math.Max(0, (level )))) + "|-- ";
                
            if (level == 0)
            {
                sceneObjectTabbing = "";
                componentTabbing = "|-- ";
            }
           
            Console.Write(sceneObjectTabbing);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj.Name);
            Console.ForegroundColor = oldC;

            Console.ForegroundColor = ConsoleColor.Green;
            obj.Tags.ToList().ForEach(c =>
            {
                Console.WriteLine(componentTabbing + c);
            });
            Console.ForegroundColor = oldC;

            obj.Components.ForEach(c =>
            {
                Console.WriteLine(componentTabbing + c.GetType());
            });

            obj.Children.ForEach(c => printSceneGraph(c, level + 1));
        }
    }  
}
