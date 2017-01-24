using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;

namespace SharpCG
{
    public abstract class Component : IDisposable
    {
        protected SceneObject sceneObject;


        protected bool enabled = true;
        protected string name = "";
        private static List<Component> allComponents = new List<Component>();

        public static List<Component> All
        {
            get { return allComponents; }
        }

        protected Component()
        {
            allComponents.Add(this);
        }

        public bool Enabled
        {
            get{ return enabled; }
            set{ enabled = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public SceneObject SceneObject
        {
            get{ return sceneObject; }
            set{ sceneObject = value; }
        }


        public virtual void OnStart() {}

        public virtual void Update(double deltaTime) {}

        public virtual void Dispose() {}


    }
}
