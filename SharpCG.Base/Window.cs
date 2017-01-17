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
using SharpCG.Base.Rendering;
using SharpCG.Base.Scenegraph;

using SharpCG.Base;
using SharpCG.Base.Input;
namespace SharpCG.Base
{
    public class Window : GameWindow
    {

        private RenderControl renderControl;
        private SceneObject root;


        public static Window CreateSimpleWindow(int width, int height)
        {
            var window = new Window(width, height);

            window.renderControl                    = new RenderControl();
            window.renderControl.ClearColorFlag     = true;
            window.renderControl.ClearColor         = Color4.White;
            window.renderControl.ClearDepthFlag     = true;
            window.renderControl.ClearDepth         = 0.0f;
            window.renderControl.ClearStencilFlag   = false;
            window.renderControl.ClearStencil       = 0;
            window.renderControl.Viewport           = new Rectangle(0, 0, window.Size.Width, window.Size.Height);


            //window.screenSize   = new Vector2(window.Width, window.Height);
            //window.mouse        = new Input.Mouse();
            //window.keyboard     = new Input.Keyboard();

            return window;
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private Window(int width, int height) : base(width, height)
        {
            root = new SceneObject();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            Traverse(root, c => c.OnStart());          
        }

        private static void Traverse(SceneObject obj, Action<Component> action)
        {
            // Execute Action for each SceneObject
            obj.Components.ForEach(action);

            // Traverse deeper
            if (obj.Children.Count > 0)
            {
                obj.Children.ForEach(c => Traverse(c, action));
            }          
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
            
            // Update render control values if changed
            if (renderControl.isDirty)
            {
                GL.Viewport(renderControl.Viewport);
                GL.ClearColor(renderControl.ClearColor);
                GL.ClearDepth(renderControl.ClearDepth);
                GL.ClearStencil(renderControl.ClearStencil);
                renderControl.isDirty = false;
            }

            // Clear back buffer
            {
                ClearBufferMask mask = ClearBufferMask.None;
                if (renderControl.ClearColorFlag) mask |= ClearBufferMask.ColorBufferBit;
                if (renderControl.ClearDepthFlag) mask |= ClearBufferMask.DepthBufferBit;
                if (renderControl.ClearStencilFlag) mask |= ClearBufferMask.StencilBufferBit;
                GL.Clear(mask);
            }

            

            // Draw all renderobjects
            renderControl.renderObjects.ForEach(delegate (RenderObject obj)
            {
                obj.renderable.Render();
            });


            this.SwapBuffers();
        }


        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            //var rnd = new System.Random();
            //var r   = (float)rnd.NextDouble();
            //var g   = (float)rnd.NextDouble();
            //var b   = (float)rnd.NextDouble();
           
            //renderControl.ClearColor = new Color4(r, g, b, 1.0f);
            base.OnKeyUp(e);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Traverse(root, c => c.Update(e.Time));
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
            // Get renderables from sceneobject
            var renderables     = Collect<Renderable>(obj);
            var renderObjects   = renderables.Select(r => new RenderObject((Renderable)r, ""));

            renderControl.renderObjects.AddRange(renderObjects);
            root.Children.Add(obj);
        }

        // Collects Components of type T
        public static List<Component> Collect<T>(SceneObject obj)
        {
            var ls = obj.Components.Where(c => c is T).ToList();

            if (obj.Children.Count > 0)
            {
                obj.Children.ForEach(child => ls.AddRange(Collect<T>(child)));
            }
            return ls;
        }
    }
}
