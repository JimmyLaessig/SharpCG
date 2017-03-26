using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;


namespace SharpCG.Core
{

    public class RenderTask
    {
        public RenderTask()
        {
            DrawEvents      = new List<Action>();
            ImmediateEvents = new List<Action>();
        }

        public List<Action> ImmediateEvents;
        public List<Action> DrawEvents;
    }


    public class Runtime
    {

        private Rectangle viewport;
        private Color4 clearColor;
        private double clearDepth;
        private int clearStencil;

        private bool clearColorFlag;
        private bool clearDepthFlag;
        private bool clearStencilFlag;


        public SortedDictionary<RenderPass, RenderTask> renderTasks = new SortedDictionary<RenderPass, RenderTask>(new RenderPass.Comparer());


        public void Render()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.Viewport(viewport);
            GL.ClearColor(clearColor);
            GL.ClearDepth(clearDepth);
            GL.ClearStencil(clearStencil);


            ClearBufferMask         mask = ClearBufferMask.None;
            if (clearColorFlag)     mask |= ClearBufferMask.ColorBufferBit;
            if (clearDepthFlag)     mask |= ClearBufferMask.DepthBufferBit;
            if (clearStencilFlag)   mask |= ClearBufferMask.StencilBufferBit;
            GL.Clear(mask);


            //var events = renderControl.renderEvents.ToList();

            renderTasks.Values.ToList().ForEach(renderTask =>
            {
                // Execute all immediate events on renderpass change
                renderTask.ImmediateEvents.ForEach(a => a.Invoke());
                // Execute Draw Calls
                renderTask.DrawEvents.ForEach(a => a.Invoke());
            });


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            var err = GL.GetError();
        }


        private void OnRenderGL(Renderer renderer)
        {            
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


        /// <summary>
        /// Adds a Renderer to the runtime to execute the draw code
        /// </summary>
        public void AddRenderer(Renderer renderer)
        {
            var renderPass = renderer.RenderPass;

            if (!renderTasks.ContainsKey(renderPass))
            {
                renderTasks[renderPass] = new RenderTask();
            }

            renderTasks[renderPass].DrawEvents.Add(() => OnRenderGL(renderer));
        }


        /// <summary>
        /// Adds a Action that contains GL calls to the be executed immediately when this Renderpass begins (e.g. Clear-Functions, State functions)
        /// </summary>
        public void AddRenderEvent(RenderPass renderPass, Action renderEvent)
        {
            if (!renderTasks.ContainsKey(renderPass))
            {
                var events = new RenderTask();
                renderTasks[renderPass] = events;
            }
            renderTasks[renderPass].ImmediateEvents.Add(renderEvent);
        }



        public Rectangle Viewport
        {
            get => viewport;
            set => viewport = value;
        }


        public Color4 ClearColor
        {
            get => clearColor;
            set => clearColor = value;
        }


        public double ClearDepth
        {
            get => clearDepth;
            set => clearDepth = value;
        }


        public int ClearStencil
        {
            get => clearStencil;
            set => clearStencil = value;
        }


        public bool ClearColorFlag
        {
            get => clearColorFlag;
            set => clearColorFlag = value;
        }


        public bool ClearDepthFlag
        {
            get => clearDepthFlag;
            set => clearDepthFlag = value;
        }


        public bool ClearStencilFlag
        {
            get => clearStencilFlag;
            set => clearStencilFlag = value;
        }
    }
}
