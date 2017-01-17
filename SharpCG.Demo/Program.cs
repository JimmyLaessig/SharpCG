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


            SceneObject cameraObject = Camera.Main.SceneObject;
            var controller = cameraObject.AddComponent<CameraController>();
            controller.MoveSpeed = 2;
            controller.RotationSpeed = 0.2f;
            controller.Camera = Camera.Main;
            window.AddSceneObject(cameraObject);


            SceneObject skybox = new SceneObject();
            skybox.AddComponent(new SkyboxComponent());
            window.AddSceneObject(skybox);


            SceneObject container = Mesh.Load("Assets/model/container.fbx");
            container.Children[0].AddComponent<MeshRenderer>();
            container.Children[0].Transform.Position = new vec3(0, 0, -3);
            container.Children[0].Transform.Scale = vec3.Ones;
            container.Children[0].AddComponent<Rotator>();
            window.AddSceneObject(container);


           
            window.Run();

        }



        public void MethodBodyExample(object arg)
        {
            // Define some local variables. In addition to these variables,
            // the local variable list includes the variables scoped to 
            // the catch clauses.
            int var1 = 42;
            string var2 = "Forty-two";

            try
            {
                // Depending on the input value, throw an ArgumentException or 
                // an ArgumentNullException to test the Catch clauses.
                if (arg == null)
                {
                    throw new ArgumentNullException("The argument cannot be null.");
                }
                if (arg.GetType() == typeof(string))
                {
                    throw new ArgumentException("The argument cannot be a string.");
                }
            }

            // There is no Filter clause in this code example. See the Visual 
            // Basic code for an example of a Filter clause.

            // This catch clause handles the ArgumentException class, and
            // any other class derived from Exception.
            catch (Exception ex)
            {
                Console.WriteLine("Ordinary exception-handling clause caught: {0}", ex.GetType());
            }
            finally
            {
                var1 = 3033;
                var2 = "Another string.";
            }
        }
    }  
}
