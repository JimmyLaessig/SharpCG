using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCG
{
    public abstract class GLComponent : Component
    {
        protected bool isDirty;

        private static List<GLComponent> allGLComponents = new List<GLComponent>();

        public static List<GLComponent> All
        {
            get { return allGLComponents; }
        }


        protected GLComponent()
        {
            allGLComponents.Add(this);
        }


        /// <summary>
        /// Method to invoice OpenGL for initialization. 
        /// This method will be called on frame start, if the dirtyFlag (IsDirty) returns true. 
        /// </summary>
        public virtual void InitGL() {}


        /// <summary>
        /// Method to invoice OpenGL for initialization. This will be called after InitGL() for all GLComponents was called. 
        /// This method will be called on frame start, if the dirtyFlag (IsDirty) returns true. 
        /// </summary>
        public virtual void LateInitGL() {}


        /// <summary>
        /// Method do dereference all OpenGL-related content. 
        /// </summary>
        public virtual void DeInitGL() {}


        /// <summary>
        /// Dirty flag indicating that InitGL/AfterInitGL should be invoked on the beginning of the next frame. 
        /// </summary>
        public bool IsDirty
        {
            get{ return isDirty; }              
        }


        public override void Dispose()
        {
            DeInitGL();
        }
    }
}
