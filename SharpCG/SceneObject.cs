using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;

namespace SharpCG
{
    public class SceneObject
    {

        private Transform transform;
        private List<Component> components;


        private SceneObject parent;
        private List<SceneObject> children;


        private string name;

        public SceneObject()
        {
            components  = new List<Component>();
            children    = new List<SceneObject>();
            transform   = this.AddComponent<Transform>();
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

            children.ForEach(c => list.AddRange(c.FindComponentsInChildren<T>()));

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



        public void AddChild(SceneObject child)
        {
            children.Add(child);

            // register this as parent
            child.parent = this;
        }


        public void RemoveChild(SceneObject child)
        {                      
            if( children.Remove(child))
            {
                child.Parent = null;
            }
        }
        public List<SceneObject> Children
        {
            get{return children; }
        }

        public SceneObject Parent
        {
            get{return parent;}
            set{parent = value;}
        }
    }
}
