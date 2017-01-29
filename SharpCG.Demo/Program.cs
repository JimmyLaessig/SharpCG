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

using SharpCG.Base.Scenegraph;

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
                Camera.Main.SetProjectionMatrix(60, (float)window.Width / (float)window.Height, 0.1f, 10000);
                Camera.Main.Transform.Position = new vec3(0f, 2.5f, 5f);
                Camera.Main.Transform.Rotation = quat.FromAxisAngle(glm.Radians(-30f), vec3.UnitX);
                Camera.Main.SceneObject.AddComponent<CameraController>();
                window.AddSceneObject(Camera.Main.SceneObject);
            }



            RenderPass geometryPass     = RenderPass.Main;
            RenderPass beforeGeometry   = RenderPass.Before(geometryPass, "before Geometry");
           
            RenderPass lightingPass     = RenderPass.After(geometryPass, "LightingPass");
            RenderPass beforeLighting   = RenderPass.Before(lightingPass, "before lighting");


            // Create GBuffer

            Framebuffer gBuffer = new Framebuffer();
           
            gBuffer.AddRenderTarget(Texture2D.Empty(window.Width, window.Height), FramebufferAttachment.ColorAttachment0, new vec4(0)); // Diffuse
            gBuffer.AddRenderTarget(Texture2D.Empty(window.Width, window.Height), FramebufferAttachment.ColorAttachment1, new vec4(0)); // Specular
            gBuffer.AddRenderTarget(Texture2D.Empty(window.Width, window.Height), FramebufferAttachment.ColorAttachment2, new vec4(0)); // Position
            gBuffer.AddRenderTarget(Texture2D.Empty(window.Width, window.Height), FramebufferAttachment.ColorAttachment3, new vec4(0)); // Normals

            gBuffer.AddRenderTarget(Texture2D.Depth(window.Width, window.Height), FramebufferAttachment.DepthAttachment, new vec4(1)); // Depth

            // //var clearAction =
            window.RenderControl.AddImmediateGLEvent(geometryPass, new Action(() => gBuffer.Clear()));


            {
                SceneObject skybox = new SceneObject();
                skybox.Name = "Skybox";
                skybox.AddComponent(MeshExtensions.UnitCube);
                var skyboxMat = skybox.AddComponent<SkyboxMaterial>();
                skyboxMat.CubeMapTexture = TextureCubeMap.Load("Assets/skybox/skybox_left2048.png",
                                                                "Assets/skybox/skybox_right2048.png",
                                                                "Assets/skybox/skybox_top2048.png",
                                                                "Assets/skybox/skybox_bottom2048.png",
                                                                "Assets/skybox/skybox_front2048.png",
                                                                "Assets/skybox/skybox_back2048.png");

                var renderer = skybox.AddComponent<SkyboxRenderer>();
                //renderer.Framebuffer = gBuffer;
                renderer.RenderPass = lightingPass;


                window.AddSceneObject(skybox);
            }


            // Copy depth texture to back buffer
            {
                SceneObject obj = new SceneObject();
                obj.Transform.ToIdentity();
                obj.AddComponent<Mesh>(MeshExtensions.FullscreenQuad);

                var mat = obj.AddComponent<TextureToDepthMaterial>();
                mat.DepthTexture = gBuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);

                var renderer        = obj.AddComponent<TextureToDepthRenderer>();
                renderer.RenderPass = beforeLighting;                                

               
                window.AddSceneObject(obj);
            }

            {
                SceneObject container1 = MeshExtensions.Load("Assets/model/container.fbx", MeshExtensions.Materials.Deferred);
                container1.Name = "Container1";

                var r2 = container1.Children[0].AddComponent<DeferredRenderer>();
                r2.Framebuffer = gBuffer;
                r2.RenderPass = geometryPass;
                r2.Stage = Stage.Geometry;


                container1.Children[0].Transform.ToIdentity();
                container1.Children[0].AddComponent<Rotator>();
                window.AddSceneObject(container1);
            }
            {
                SceneObject container2 = MeshExtensions.Load("Assets/model/container.fbx", MeshExtensions.Materials.Deferred);
                container2.Name = "Container2";


                var renderer = container2.Children[0].AddComponent<DeferredRenderer>();
                renderer.Framebuffer = gBuffer;
                renderer.RenderPass = geometryPass;
                renderer.Stage = Stage.Geometry;


                container2.Children[0].Transform.ToIdentity();
                container2.Children[0].Transform.Position = new vec3(2, 0, 0);


                SceneObject pivot = new SceneObject();
                pivot.Name = "Pivot";

                pivot.AddChild(container2);
                pivot.AddComponent<Rotator>();
                window.AddSceneObject(pivot);
            }

            // Add A Light
            {
                SceneObject lightObject = new SceneObject();
                lightObject.Name = "Ambient Light";
                var light = lightObject.AddComponent<AmbientLight>();
                light.Color = new vec3(0.25f, 0.25f, 0.25f);
                var renderer = lightObject.AddComponent<DeferredRenderer>();
                renderer.RenderPass = lightingPass;
                renderer.Stage = Stage.Lighting;
                renderer.GBuffer = gBuffer;
                var material = lightObject.AddComponent<LightingPassMaterial>();


                window.AddSceneObject(lightObject);
            }

            // Add A Light
            {
                //SceneObject lightObject = new SceneObject();
                //lightObject.Name = "Ambient Light";
                //var light = lightObject.AddComponent<AmbientLight>();
                //light.Color = new vec3(0.25f, 0.25f, 0.25f);
                //var renderer = lightObject.AddComponent<DeferredRenderer>();
                //renderer.RenderPass = lightingPass;
                //renderer.Stage = Stage.Lighting;
                //renderer.GBuffer = gBuffer;
                //var material = lightObject.AddComponent<LightingPassMaterial>();


                //window.AddSceneObject(lightObject);
            }

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

            obj.Components.ForEach(c =>
            {
                Console.WriteLine(componentTabbing + c.GetType());
            });

            obj.Children.ForEach(c => printSceneGraph(c, level + 1));
        }
    }  
}
