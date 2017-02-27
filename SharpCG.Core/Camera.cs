using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics;

using GlmSharp;

namespace SharpCG.Core
{
    public class Camera : Component
    {

        private double zNear;
        private double zFar;
        private double fov;
        private double aspect;    
        private dmat4 projectionMatrix;


        private static Camera main = null;

        public static Camera Main
        {
            get {
                if(main == null)
                {                   
                    SceneObject camObject = new SceneObject();
                    camObject.Name = "Camera";
                    Camera cam = camObject.AddComponent<Camera>();
                    cam.zNear = 0.1f;
                    cam.zFar = 1000;
                    cam.fov = 60.0f;
                    cam.aspect = 1024.0f / 768.0f;
                    cam.SetProjectionMatrix(cam.fov, cam.aspect, cam.zNear, cam.zFar);
                    main = cam;
                }
                return main;
            }
            set { main = value; }
        }

        

        public Camera()
        {
            projectionMatrix = dmat4.Identity;
        }

        public Transform Transform
        {
            get { return sceneObject.Transform; }
        }


        /// <summary>
        /// Creates a new projection matrix with the given values
        /// </summary>
        /// <param name="fov">Field of View</param>
        /// <param name="ratio">Aspect Ratio</param>
        /// <param name="zNear">Near Plane</param>
        /// <param name="zFar">Far Plane</param>
        public void SetProjectionMatrix(double fov, double aspect, double zNear, double zFar)
        {
            this.zNear  = zNear;
            this.zFar   = zFar;
            this.fov    = fov;
            this.aspect = aspect;
            
            projectionMatrix = dmat4.Perspective(glm.Radians(fov), aspect, zNear, zFar);
        }



        /// <summary>
        /// Returns the projection matrix.
        /// </summary>
        public dmat4 ProjectionMatrix
        {
            get { return projectionMatrix; }
        }



        /// <summary>
        /// Returns the matrix used to transform an object into view space
        /// </summary>
        public dmat4 ViewMatrix
        {
            get{ return Transform.InverseWorldMatrix; }
        }

        
        public void LookAt(dvec3 eye, dvec3 target, dvec3 up)
        {
            Transform.LookAt(eye, target, up);          
        }
    }
}
