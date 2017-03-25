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
using SharpCG.Effects.Postprocessing;

namespace SharpCG.Demo
{
    class PostProcessingExample
    {

        static void Main(string[] args)
        {
            Window window = Window.CreateSimpleWindow(1024, 768);
            window.RenderControl.ClearColor = OpenTK.Graphics.Color4.Black;
            Shader.InitializeShaders();


            {
                // Create Camera          
                Camera.Main.SetProjectionMatrix(60.0, (double)window.Width / (double)window.Height, 0.1, 100000.0);
                Camera.Main.Transform.Position = new dvec3(0.0, 2.5, 5);
                Camera.Main.Transform.Rotation = dquat.FromAxisAngle(glm.Radians(-30.0), dvec3.UnitX);
                Camera.Main.SceneObject.AddComponent<CameraController>();
                window.AddSceneObject(Camera.Main.SceneObject);
            }

            var geometryPass = RenderPass.Main;
            var postprocessingPass = RenderPass.After(RenderPass.Main, "FXAA_Pass");

            int width = 1024;
            int height = 768;
            Framebuffer framebuffer = new Framebuffer();
            framebuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment0, new vec4(1, 1, 1, 1));
            framebuffer.AddRenderTarget(Texture2D.Depth(width, height), FramebufferAttachment.DepthAttachment, new vec4(1));    // Depth  
            window.RenderControl.AddImmediateGLEvent(geometryPass, () => framebuffer.Clear());
            framebuffer.Clear();
            //{
            //    var sphereObj = new SceneObject();
            //    sphereObj.Name = "TesselatedSphere";
            //    sphereObj.AddComponent(GeometryExtensions.Sphere(vec3.Zero, 1, 20));
            //    //sphereObj.AddComponent(GeometryExtensions.FullscreenQuad);
            //    sphereObj.AddComponent<SimpleRenderer>();
            //    var mat = sphereObj.AddComponent<ColoredMaterial>();
            //    mat.Color = new vec4(1, 0, 0, 1);

            //    window.AddSceneObject(sphereObj);
            //}




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

                //var renderer            = skybox.AddComponent<SkyboxRenderer>();
                //renderer.Framebuffer    = framebuffer;
                //renderer.RenderPass     = RenderPass.Main;


                //window.AddSceneObject(skybox);
            }
            {
                SceneObject plane = GeometryExtensions.Load("Assets/plane/plane.fbx");
                plane.Name = "plane";

                //plane.Transform.Scale = new dvec3(5);


                SceneObject.TraverseAndExecute<Geometry>(plane, m =>
                {
                    var renderer = m.SceneObject.AddComponent<MeshRenderer>();
                    renderer.Framebuffer = framebuffer;
                    renderer.RenderPass = RenderPass.Main;
                });

                window.AddSceneObject(plane);
            }
            {
                SceneObject trooper = GeometryExtensions.Load("Assets/stormtrooper/stormtrooper.fbx");
                trooper.Name = "stormtrooper";

                SceneObject.TraverseAndExecute<Geometry>(trooper, m =>
                {
                    var renderer = m.SceneObject.AddComponent<MeshRenderer>();
                    renderer.Framebuffer = framebuffer;
                    renderer.RenderPass = geometryPass;
                });


                window.AddSceneObject(trooper);
            }
            {
                SceneObject postprocessing = new SceneObject();
                postprocessing.AddComponent<PostprocessingMaterial>();
                var fxaaTechnique = postprocessing.AddComponent<FXAATechnique>();
                fxaaTechnique.ColorTexture = framebuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment0);
                fxaaTechnique.DepthTexture = framebuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);
                fxaaTechnique.RenderPass = postprocessingPass;


                window.AddSceneObject(postprocessing);
            }

            window.Run();
        }
    }
}
