﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;


namespace SharpCG.Core
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
        

        public void RemoveComponents<T>() where T: Component
        {            
            components.RemoveAll(c => c is T);
        }

        public List<Component> Components
        {
            get => components; 
            set => components = value; 
        }


        public Transform Transform
        {
            
            get => transform;         
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
            get => name; 
            set => name = value;
        }


        public void AddChild(SceneObject child)
        {
            children.Add(child);
            var t = child.transform.WorldPosition;
            var s = child.transform.WorldScale;
            var r = child.transform.WorldRotation;
            // register this as parent
            child.parent = this;
            child.transform.WorldPosition = t;
            child.transform.WorldScale = s;
            child.transform.WorldRotation = r;
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
            get => children; 
        }


        public SceneObject Parent
        {
            get => parent;
            set => parent = value;
        }


        public HashSet<string> Tags
        {
            get => tags;
        }


        public Window Runtime
        {
            get => runtime;          
            set => runtime = value;
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


        public static void PrintHierarchy(SceneObject obj)
        {
            Console.WriteLine("---------- SceneObject Hierarchy ----------");
            PrintRec(obj, 0);
        }


        private static void PrintRec(SceneObject obj, int level)
        {
            var oldC = Console.ForegroundColor;


            string sceneObjectTabbing = string.Concat(Enumerable.Repeat("|  ", Math.Max(0, (level - 1)))) + "|-- ";
            string componentTabbing = string.Concat(Enumerable.Repeat("|  ", Math.Max(0, (level)))) + "|-- ";

            if (level == 0)
            {
                sceneObjectTabbing = "";
                componentTabbing = "|-- ";
            }

            Console.Write(sceneObjectTabbing);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj.Name);
            Console.ForegroundColor = oldC;

            Console.ForegroundColor = ConsoleColor.Green;
            obj.Tags.ToList().ForEach(c =>
            {
                Console.WriteLine(componentTabbing + c);
            });
            Console.ForegroundColor = oldC;

            obj.Components.ForEach(c =>
            {
                Console.WriteLine(componentTabbing + c.GetType());
            });

            obj.Children.ForEach(c => PrintRec(c, level + 1));
        }

    }
}
