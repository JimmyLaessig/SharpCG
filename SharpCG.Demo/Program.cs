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
                Camera.Main.SetProjectionMatrix(60, (float)window.Width / (float)window.Height, 0.1f, 100000);
                Camera.Main.Transform.Position = new vec3(0f, 2.5f, 5f);
                Camera.Main.Transform.Rotation = quat.FromAxisAngle(glm.Radians(-30f), vec3.UnitX);

                Camera.Main.SceneObject.AddComponent<CameraController>();
                window.AddSceneObject(Camera.Main.SceneObject);
            }
            {
                var sphereObj   = new SceneObject();
                sphereObj.Name  = "TesselatedSphere";
                sphereObj.AddComponent(GeometryExtensions.Sphere(vec3.Zero, 1, 20));
                //sphereObj.AddComponent(GeometryExtensions.FullscreenQuad);
                sphereObj.AddComponent<SimpleRenderer>();
                var mat = sphereObj.AddComponent<ColoredMaterial>();
                mat.Color = new vec4(1, 0, 0, 1);

                window.AddSceneObject(sphereObj);

            }
            {
                SceneObject skybox = new SceneObject();
                skybox.Name = "Skybox";
                skybox.AddComponent(GeometryExtensions.UnitCube);
                var skyboxMat           = skybox.AddComponent<SkyboxMaterial>();
                skyboxMat.CubeMapTexture = TextureCubeMap.Load("Assets/skybox/skybox_left2048.png",
                                                                "Assets/skybox/skybox_right2048.png",
                                                                "Assets/skybox/skybox_top2048.png",
                                                                "Assets/skybox/skybox_bottom2048.png",
                                                                "Assets/skybox/skybox_front2048.png",
                                                                "Assets/skybox/skybox_back2048.png");

                var renderer = skybox.AddComponent<SkyboxRenderer>();
                //renderer.Framebuffer = gBuffer;
                renderer.RenderPass = RenderPass.Before(RenderPass.Main , "skybox");


                window.AddSceneObject(skybox);
            }
            //RenderPass geometryPass     = RenderPass.Main;

            //RenderPass beforeGeometry   = RenderPass.Before(geometryPass, "before Geometry");

            //RenderPass shadowPass       = RenderPass.After(geometryPass, "shadowPass");

            //RenderPass lightingPass     = RenderPass.After(shadowPass, "LightingPass");
            //RenderPass beforeLighting   = RenderPass.Before(lightingPass, "before lighting");
            //RenderPass afterLighting    = RenderPass.After(lightingPass, "aftér lighting");

            //var gBuffer = Framebuffer.GBuffer;
            //window.RenderControl.AddImmediateGLEvent(beforeGeometry, (() => gBuffer.Clear()));

            //{
            //    SceneObject skybox = new SceneObject();
            //    skybox.Name = "Skybox";
            //    skybox.AddComponent(GeometryExtensions.UnitCube);
            //    var skyboxMat = skybox.AddComponent<SkyboxMaterial>();
            //    skyboxMat.CubeMapTexture = TextureCubeMap.Load("Assets/skybox/skybox_left2048.png",
            //                                                    "Assets/skybox/skybox_right2048.png",
            //                                                    "Assets/skybox/skybox_top2048.png",
            //                                                    "Assets/skybox/skybox_bottom2048.png",
            //                                                    "Assets/skybox/skybox_front2048.png",
            //                                                    "Assets/skybox/skybox_back2048.png");

            //    var renderer = skybox.AddComponent<SkyboxRenderer>();
            //    renderer.Framebuffer = gBuffer;
            //    renderer.RenderPass = beforeLighting;


            //    window.AddSceneObject(skybox);
            //}


            //// Copy depth texture to back buffer
            //{
            //    var obj = SceneObjectExtensions.CopyDepthBuffer(gBuffer, null, beforeLighting);               
            //    window.AddSceneObject(obj);
            //}

            //{
            //    SceneObject plane = GeometryExtensions.Load("Assets/plane/plane.fbx", GeometryExtensions.Materials.Deferred);
            //    plane.Name = "plane1";
            //    plane.Transform.ToIdentity();
            //    plane.Transform.Scale = new vec3(5);


            //    SceneObject.TraverseAndExecute<Geometry>(plane, m =>
            //    {
            //        var obj = m.SceneObject;
            //        var scale = obj.Transform.Scale;
            //        obj.Transform.ToIdentity();

            //        var renderers = obj.FindComponents<DeferredMeshRenderer>();
            //        renderers.ForEach(r =>
            //        {
            //            r.RenderPass = geometryPass;
            //            r.Framebuffer = gBuffer;
            //        });

            //    });

            //    window.AddSceneObject(plane);
            //}
            //{
            //    SceneObject figure = GeometryExtensions.Load("Assets/tal16/tal16.fbx", GeometryExtensions.Materials.Deferred);
            //    figure.Name = "tal16";
            //    figure.Transform.Scale = new vec3(0.01f);
            //    figure.Transform.Translate(new vec3(0, 1, 0));
            //    SceneObject.TraverseAndExecute<Geometry>(figure, m =>
            //    {
            //        var obj         = m.SceneObject;
            //        var scale       = obj.Transform.Scale;
            //        var renderers   = obj.FindComponents<DeferredMeshRenderer>();
            //        renderers.ForEach(r =>
            //        {
            //            r.RenderPass = geometryPass;
            //            r.Framebuffer = gBuffer;
            //            r.GeometryPassMaterial.NormalMappingEnabled = false;
            //        });
            //    });

            //    window.AddSceneObject(figure);
            //}


            //// Add Ambient Light
            //{
            //    SceneObject lightObject = new SceneObject();
            //    lightObject.Name = "Ambient Light";
            //    var light = lightObject.AddComponent<AmbientLight>();
            //    light.Color = new vec3(0.25f, 0.25f, 0.25f);
            //    var material = lightObject.AddComponent<LightingPassMaterial>();

            //    var renderer = lightObject.AddComponent<DeferredMeshRenderer>();
            //    renderer.RenderPass = lightingPass;
            //    renderer.Stage = Stage.Lighting;
            //    renderer.LightingPassMaterial = material;
            //    renderer.GBuffer = gBuffer;

            //    window.AddSceneObject(lightObject);
            //}


            //// Add Directional Light
            //{
            //    SceneObject lightObject = new SceneObject();
            //    lightObject.Name = "Directional Light";

            //    var light = lightObject.AddComponent<DirectionalLight>();

            //    light.Color = new vec3(1);
            //    light.Direction = new vec3(0f, -1f, 0f);
            //    var x = light.Direction;
            //    var renderer = lightObject.AddComponent<DeferredMeshRenderer>();
            //    renderer.RenderPass = lightingPass;
            //    renderer.Stage = Stage.Lighting;
            //    renderer.GBuffer = gBuffer;

            //    var material = lightObject.AddComponent<LightingPassMaterial>();
            //    renderer.LightingPassMaterial = material;


            //    // Shadows
            //    var framebuffer = new Framebuffer();
            //    framebuffer.AddRenderTarget(Texture2D.Depth(2000, 2000), FramebufferAttachment.DepthAttachment, vec4.Ones);
            //    window.RenderControl.AddImmediateGLEvent(shadowPass, (() => framebuffer.Clear()));


            //    var shadows = lightObject.AddComponent<ShadowMapRenderer>();
            //    shadows.RenderPass = shadowPass;
            //    shadows.Framebuffer = framebuffer;

            //    window.AddSceneObject(lightObject);
            //}


            Console.WriteLine("---------- Scenegraph ----------");
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
