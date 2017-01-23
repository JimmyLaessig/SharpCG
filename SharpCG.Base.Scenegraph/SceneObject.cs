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

        private string name;

        public SceneObject()
        {
            transform   = new Transform();
            components  = new List<Component>();
            Children    = new List<SceneObject>();
            Name = "";
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


        public T FindComponent<T>() where T: Component
        {
            var c = components.OfType<T>();
            
            if (c.Count() > 0)
                return c.First();
            else
                return null;
        }

        public List<T> FindComponentsInChildren<T>() where T : Component
        {
            var list = components.OfType<T>().ToList();

            Children.ForEach(c => list.AddRange(c.FindComponentsInChildren<T>()));

            return list;
        }


        public void RemoveComponent(Component component)
        {
            components.Remove(component);
        }


        public string Name
        {
            get{ return name; }
            set{ name = value; }
        }
    }
}
