using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCG.Core
{

    public class RenderPass
    {
        private RenderPass next;

        private RenderPass previous;

        private static RenderPass main;

        private double sortKey;

        private string name;


        public static RenderPass First
        {
            get
            {
                var current = Main;
                while (current.previous != null)
                    current = current.previous;
                return current;
            }
        }


        public static RenderPass Main
        {
            get
            {
                if (main == null)
                    main = Between(null, null, "Main");
                
                return main;
            }
        }


        public string Name
        {
            get => name;
            set => name = value;
        }


        public double SortKey
        {
            get => sortKey;
        }


        private static RenderPass Between (RenderPass previous, RenderPass next, string name)
        {
            RenderPass pass =  new RenderPass();
            pass.name = name;

            double previousKey  = 0.0;
            double nextKey      = 1.0;

            if (previous != null)
            {
                previous.next = pass;
                pass.previous = previous;
                previousKey = previous.sortKey;
            }

            if (next != null)
            {
                next.previous = pass;
                pass.next = next;
                nextKey = next.SortKey;
            }

            pass.sortKey = (nextKey - previousKey) / 2.0 + previousKey;
            return pass;

        }

        public static RenderPass After(RenderPass renderPass, string name)
        {           
            return Between(renderPass, renderPass.next, name);
        }

        public static RenderPass Before(RenderPass renderpass, string name)
        {
            return Between(renderpass.previous, renderpass, name);
        }


        public class Comparer : IComparer<RenderPass>
        {
            public int Compare(RenderPass x, RenderPass y)
            {
                if (x.SortKey > y.SortKey) return 1;
                if (x.SortKey < y.SortKey) return -1;
                return 0;
            }
        }
    }
}
