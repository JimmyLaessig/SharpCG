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

           
            RenderPass skyBoxPass = RenderPass.Before(RenderPass.Main, "SkyboxRenderPass");


            SceneObject cameraObject = Camera.Main.SceneObject;
            cameraObject.Name = "Camera";
            var controller = cameraObject.AddComponent<CameraController>();
            controller.MoveSpeed = 2;
            controller.RotationSpeed = 0.2f;
            controller.Camera = Camera.Main;
            window.AddSceneObject(cameraObject);



            SceneObject skybox = new SceneObject();
            skybox.Name = "Skybox";
            skybox.AddComponent(MeshExtensions.UnitCube);
            var material = skybox.AddComponent<SkyboxMaterial>();
            material.CubeMapTexture = TextureCubeMap.Load("Assets/skybox/skybox_left2048.png",
                                                            "Assets/skybox/skybox_right2048.png",
                                                            "Assets/skybox/skybox_top2048.png",
                                                            "Assets/skybox/skybox_bottom2048.png",
                                                            "Assets/skybox/skybox_front2048.png",
                                                            "Assets/skybox/skybox_back2048.png");
            var r1 = skybox.AddComponent<SkyboxRenderer>();
            r1.RenderPass = skyBoxPass;
            window.AddSceneObject(skybox);


            SceneObject container = MeshExtensions.Load("Assets/model/container.fbx");
            container.Name = "Container";
            var r2 = container.Children[0].AddComponent<MeshRenderer>();
            r2.RenderPass = RenderPass.Main;
            container.Children[0].Transform.Position = new vec3(0, 0, -3);
            container.Children[0].Transform.Scale = vec3.Ones;
            container.Children[0].AddComponent<Rotator>();
            window.AddSceneObject(container);

            Framebuffer fb = new Framebuffer();
            var color0 = Texture2D.Empty((uint)window.Width, (uint)window.Height);
            var color1 = Texture2D.Empty((uint)window.Width, (uint)window.Height);
            var color2 = Texture2D.Empty((uint)window.Width, (uint)window.Height);
            var color3 = Texture2D.Empty((uint)window.Width, (uint)window.Height);

            fb.AddRenderTarget(color0, OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment0, new vec4(1, 0, 0, 1));
            fb.AddRenderTarget(color1, OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment1, new vec4(0, 1, 0, 1));
            fb.AddRenderTarget(color2, OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment2, new vec4(0, 0, 1, 1));
            fb.AddRenderTarget(color3, OpenTK.Graphics.OpenGL4.FramebufferAttachment.ColorAttachment3, new vec4(1, 1, 1, 1));

            fb.InitGL();
            fb.Clear();
            window.Run();

            fb.Clear();

        }
       
    }  
}
