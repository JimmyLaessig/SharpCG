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
        private Window runtime;
        

        private HashSet<string> tags;

        private Transform transform;
        private List<Component> components;


        private SceneObject parent;
        private List<SceneObject> children;


        private string name;


        public SceneObject()
        {
            tags        = new HashSet<string>();
            components  = new List<Component>();
            children    = new List<SceneObject>();
            transform   = this.AddComponent<Transform>();
            Name = "";
            
        }


        public SceneObject(string name) : this()
        {
            Name = name;
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


        public List<T> FindComponents<T>() where T : Component
        {
            return components.OfType<T>().ToList();           
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

        public HashSet<string> Tags
        {
            get{return tags;}
        }

        public Window Runtime
        {
            get
            {
                return runtime;
            }

            set
            {
                runtime = value;
            }
        }

        public void AddTag(string tag)
        {
            tags.Add(tag);
        }


        public void RemoveTag(string tag)
        {
            tags.Remove(tag);
        }


        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }


        public static void TraverseAndExecute<T>(SceneObject obj, Action<T> action) where T : Component
        {
            obj.Components.OfType<T>().ToList().ForEach(action);
            obj.Children.ForEach(c => TraverseAndExecute(c, action));
        }


        public static void TraverseAndExecute(SceneObject obj, Action<SceneObject> action)
        {
            action.Invoke(obj);
            obj.Children.ForEach(c => TraverseAndExecute(c, action));
        }


        // Collects Components of type T
        public static List<T> Collect<T>(SceneObject obj) where T : Component
        {
            var ls = obj.Components.OfType<T>().ToList();
            obj.Children.ForEach(child => ls.AddRange(Collect<T>(child)));
            return ls;
        }


        public static List<T> CollectWhere<T>(SceneObject obj, Func<SceneObject, bool> f) where T : Component
        {
            var ls = new List<T>();

            if (f(obj)) ls.AddRange(obj.FindComponents<T>());
            
            obj.Children.ForEach(child => ls.AddRange(CollectWhere<T>(child, f)));
            return ls;
        }
    }
}
