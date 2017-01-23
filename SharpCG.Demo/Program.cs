using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Input;
using SharpCG.Base;
using System.IO;
using SharpCG;
using GlmSharp;
using SharpCG.Base.Rendering;
using SharpCG.Base.Scenegraph;
using OpenTK;


namespace SharpCG.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Window window = Window.CreateSimpleWindow(1024, 768);

            Shader.InitializeShaders();

            // Create Camera
            Camera.Main.SetProjectionMatrix(60, (float)window.Width / (float)window.Height, 0.1f, 10000);
            //Camera.Main.LookAt(vec3.Zero, vec3.UnitZ, vec3.UnitY);

           
            RenderPass skyBoxPass   = RenderPass.Before(RenderPass.Main, "SkyboxRenderPass");
            RenderPass geometryPass = RenderPass.After(RenderPass.Main, "GeometryPass");
            RenderPass lightingPass = RenderPass.After(geometryPass, "LightingPass");
            {
                SceneObject cameraObject = Camera.Main.SceneObject;
                cameraObject.Name = "Camera";
                var controller = cameraObject.AddComponent<CameraController>();
                controller.MoveSpeed = 2;
                controller.RotationSpeed = 0.2f;
                controller.Camera = Camera.Main;
                window.AddSceneObject(cameraObject);
            }

            // Create GBuffer
            
            Framebuffer gBuffer = new Framebuffer();
            gBuffer.AddRenderTarget(Texture2D.Empty((uint)window.Width, (uint)window.Height), OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment0, new vec4(0, 0, 0, 0));    // Pos
            gBuffer.AddRenderTarget(Texture2D.Empty((uint)window.Width, (uint)window.Height), OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment1, new vec4(0, 0, 0, 0));    // Normals
            gBuffer.AddRenderTarget(Texture2D.Empty((uint)window.Width, (uint)window.Height), OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment2, new vec4(0, 0, 0, 0));    // Diffuse
            gBuffer.AddRenderTarget(Texture2D.Empty((uint)window.Width, (uint)window.Height), OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment3, new vec4(0, 0, 0, 0));    // Specular
            gBuffer.InitGL();
            
            gBuffer.Clear();


            {
                //SceneObject skybox = new SceneObject();
                //skybox.Name = "Skybox";
                //skybox.AddComponent(MeshExtensions.UnitCube);
                //var skyboxMat = skybox.AddComponent<SkyboxMaterial>();
                //skyboxMat.CubeMapTexture = TextureCubeMap.Load("Assets/skybox/skybox_left2048.png",
                //                                                "Assets/skybox/skybox_right2048.png",
                //                                                "Assets/skybox/skybox_top2048.png",
                //                                                "Assets/skybox/skybox_bottom2048.png",
                //                                                "Assets/skybox/skybox_front2048.png",
                //                                                "Assets/skybox/skybox_back2048.png");
                //var renderer = skybox.AddComponent<SkyboxRenderer>();
                //renderer.RenderPass = skyBoxPass;

                
                //window.AddSceneObject(skybox);
            }
            {
                SceneObject container1 = MeshExtensions.Load("Assets/model/container.fbx", MeshExtensions.Materials.Deferred);
                container1.Name = "Container1";

                var r2 = container1.Children[0].AddComponent<DeferredRenderer>();
                r2.Framebuffer  = gBuffer;
                r2.RenderPass   = geometryPass;
                r2.Stage        = Stage.Geometry;


                //var renderer = container1.Children[0].AddComponent<MeshRenderer>();
                //renderer.RenderPass = RenderPass.Main;

                container1.Children[0].Transform.Position = new vec3(-1.5f, 0f, -3f);
                container1.Children[0].Transform.Scale = vec3.Ones;
                container1.Children[0].AddComponent<Rotator>();
                window.AddSceneObject(container1);
            }
            {
                SceneObject container2 = MeshExtensions.Load("Assets/model/container.fbx", MeshExtensions.Materials.Deferred);
                container2.Name = "Container2";

                //var renderer = container2.Children[0].AddComponent<MeshRenderer>();
                //renderer.RenderPass = RenderPass.After(RenderPass.Main, "tmp");

                var renderer = container2.Children[0].AddComponent<DeferredRenderer>();
                renderer.Framebuffer = gBuffer;
                renderer.RenderPass = geometryPass;
                renderer.Stage = Stage.Geometry;


                container2.Children[0].Transform.Position = new vec3(1.5f, 0f, -3f);
                container2.Children[0].Transform.Scale = vec3.Ones;
                container2.Children[0].AddComponent<Rotator>();
                window.AddSceneObject(container2);
            }
            // Add A Light
            {
                SceneObject lightObject = new SceneObject();
                var light           = lightObject.AddComponent<AmbientLight>();
                light.Color         = new vec3(0.25f, 0.25f, 0.25f);
                var renderer        = lightObject.AddComponent<DeferredRenderer>();
                renderer.RenderPass = lightingPass;
                renderer.Stage      = Stage.Lighting;
                renderer.GBuffer    = gBuffer;
                var material        = lightObject.AddComponent<LightingPassMaterial>();


                window.AddSceneObject(lightObject);
            }

            window.Run();            
        }
       
    }  
}
