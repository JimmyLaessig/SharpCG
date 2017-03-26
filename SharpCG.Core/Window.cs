using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;


namespace SharpCG.Core
{
    public class Window : GameWindow
    {

        private Runtime runtime;
        private SceneObject root;

        private List<SceneObject> newObjects = new List<SceneObject>();


        public Runtime Runtime
        {
            get => runtime; 
        }


        public SceneObject Root
        {
            get => root; 
        }


        public static Window CreateSimpleWindow(int width, int height)
        {
            var window = new Window(width, height)
            {
                runtime = new Runtime()
            };
            window.runtime.ClearColorFlag   = true;
            window.runtime.ClearColor       = Color4.White;
            window.runtime.ClearDepthFlag   = true;
            window.runtime.ClearDepth       = 1.0f;
            window.runtime.ClearStencilFlag = false;
            window.runtime.ClearStencil     = 0;
            window.runtime.Viewport         = new Rectangle(0, 0, window.Size.Width, window.Size.Height);

            return window;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private Window(int width, int height) : base(width, height)
        {
            root = new SceneObject()
            {
                Name = "root"
            };
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            SceneObject.PrintHierarchy(root);
        }



        protected override void OnKeyPress(OpenTK.KeyPressEventArgs e)
        {


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            this.Title = "FPS: " + 1.0 / e.Time;
            runtime.Render();
            
            this.SwapBuffers();
        }


        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {            
            base.OnKeyUp(e);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            newObjects.ForEach(obj => SceneObject.TraverseAndExecute<Component>(obj, c => c.OnStart()));
            newObjects.Clear();

            var glComponents = Component.All.OfType<GLComponent>().ToList();
            glComponents.ForEach(c => { if (c.IsDirty) c.InitGL(); });
            glComponents.ForEach(c => { if (c.IsDirty) c.LateInitGL(); });


            Component.All.ForEach(c => c.Update(e.Time));
        }


        public void registerResizeCallback()
        {

        }


        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {

            base.OnResize(e);
        }



        public void AddSceneObject(SceneObject obj)
        {
            obj.Runtime = this;

            SceneObject.TraverseAndExecute<Renderer>(obj, r => runtime.AddRenderer(r));

            SceneObject.TraverseAndExecute(obj, newObj => 
            {
                newObj.Runtime = this;
                newObjects.Add(newObj);
            });

            root.AddChild(obj);           
        }      
    }
}
