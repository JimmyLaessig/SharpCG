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


namespace SharpCG
{
    public class Window : GameWindow
    {

        private RenderControl renderControl;
        private SceneObject root;

        private List<SceneObject> newObjects = new List<SceneObject>();

        public RenderControl RenderControl
        {
            get{return renderControl;}
        }

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
            root.Name = "root";
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            Traverse<Component>(root, c => c.OnStart());          
        }


        private static void Traverse<T>(SceneObject obj, Action<T> action) where T : Component
        {         
            obj.Components.OfType<T>().ToList().ForEach(action);
            obj.Children.ForEach(c => Traverse(c, action));           
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

            //var events = renderControl.renderEvents.ToList();



            renderControl.renderEvents.Values.ToList().ForEach(x => 
            {
                // Execute all immediate events on renderpass change
                x.immediateEvents.ForEach(a => a.Invoke());
                // Execute Draw Calls
                x.drawEvents.ForEach(a => a.Invoke());
            });



            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            var err = GL.GetError();
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
            newObjects.ForEach(obj => Traverse<Component>(obj, c => c.OnStart()));
            newObjects.Clear();

            //Traverse<GLComponent>(root, c => { if (c.IsDirty) c.InitGL(); });
            //Traverse<GLComponent>(root, c => { if (c.IsDirty) c.AfterInitGL();});
            //Traverse<Component>(root, c => c.Update(e.Time));    

            Component.All.ForEach(c => c.Update(e.Time));

            var glComponents = Component.All.OfType<GLComponent>().ToList();
            glComponents.ForEach(c => { if (c.IsDirty) c.InitGL();});
            glComponents.ForEach(c => { if (c.IsDirty) c.AfterInitGL();});            
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


        private void OnRenderGL(Renderer renderer)
        {
            // TODO: Do some more elaborate stuff for state optimization

            var fb = renderer.Framebuffer;
            //Bind Framebuffer
            if (fb != null)
            {
                fb.BindForWriting();
            }
            //Bind Default (Back)Buffer
            else
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
            // Execute GLCommand
            renderer.RenderGL();
       
        }


        public void AddSceneObject(SceneObject obj)
        {
            Traverse<Renderer>(obj, r =>renderControl.AddRenderGLEvent(r.RenderPass, () => this.OnRenderGL(r)));
            root.Children.Add(obj);
        }


        // Collects Components of type T
        public static List<T> Collect<T>(SceneObject obj)
        {
            var ls = obj.Components.OfType<T>().ToList();
            obj.Children.ForEach(child => ls.AddRange(Collect<T>(child)));            
            return ls;
        }
    }
}
