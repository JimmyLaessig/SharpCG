using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics;


namespace SharpCG
{ 
    
    public class RenderPassEvents
    {
        public RenderPassEvents()
        {
            drawEvents      = new List<Action>();
            immediateEvents = new List<Action>();
        }

        public List<Action> immediateEvents;
        public List<Action> drawEvents;
    }

    class RenderPassComparer : IComparer<RenderPass>
    {
        public int Compare(RenderPass x, RenderPass y)
        {
            if (x.SortKey > y.SortKey) return 1;
            if (x.SortKey < y.SortKey) return -1;
            return 0;
        }
    }


    public class RenderControl
    {
        

        private Rectangle viewport;
        private Color4 clearColor;
        private double clearDepth;
        private int clearStencil;

        public bool ClearColorFlag;
        public bool ClearDepthFlag;
        public bool ClearStencilFlag;

        public bool isDirty;


        public SortedDictionary<RenderPass, RenderPassEvents> renderEvents = new SortedDictionary<RenderPass, RenderPassEvents>(new RenderPassComparer());

        

        public Rectangle Viewport
        {
            get {return viewport;}
            set { isDirty = true; viewport = value;}
        }


        /// <summary>
        /// Adds a Action that contains GL calls to the be executed in the given render pass (e.g. Draw-Functions)
        /// </summary>
        public void AddRenderGLEvent(RenderPass renderPass, Action drawEvent)
        {
            if (!renderEvents.ContainsKey(renderPass))
            {
                var events = new RenderPassEvents();
                renderEvents[renderPass] = events;
            }
            renderEvents[renderPass].drawEvents.Add(drawEvent);
        }


        /// <summary>
        /// Adds a Action that contains GL calls to the be executed immediately when this Renderpass begins (e.g. Clear-Functions, State functions)
        /// </summary>
        public void AddImmediateGLEvent(RenderPass renderPass, Action immediateEvent)
        {
            if (!renderEvents.ContainsKey(renderPass))
            {
                var events = new RenderPassEvents();
                renderEvents[renderPass] = events;
            }
            renderEvents[renderPass].immediateEvents.Add(immediateEvent);
        }


        public Color4 ClearColor
        {
            get {return clearColor;}
            set { isDirty = true; clearColor = value;}
        }


        public double ClearDepth
        {
            get { return clearDepth;}
            set { isDirty = true; clearDepth = value;}
        }


        public int ClearStencil
        {
            get {return clearStencil;}
            set{isDirty = true;clearStencil = value;}
        }
    }
}
