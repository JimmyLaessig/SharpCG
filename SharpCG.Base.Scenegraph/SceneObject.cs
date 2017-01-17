using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace SharpCG.Base.Scenegraph
{
    public class SceneObject
    {
        private Transform transform;
        private List<Component> components;

        public List<SceneObject> Children;

        public SceneObject()
        {
            transform   = new Transform();
            components  = new List<Component>();
            Children    = new List<SceneObject>();
        }

        public T AddComponent<T> (T component) where T: Component
        {            
            components.Add(component);
            component.SceneObject = this;
            return component;
        }
        public T AddComponent<T>() where T : Component, new()
        {

            var component = new T();

            components.Add(component);
            component.SceneObject = this;

            return component;
            
        }

        public List<Component> Components
        {
            get{ return components; }
            set{ components = value; }
        }

        public Transform Transform
        {
            get{ return transform; }
            set{ transform = value; }
        }




    }
}
