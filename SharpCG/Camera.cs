using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics;

using GlmSharp;

namespace SharpCG
{
    public class Camera : Component
    {

        private float zNear;
        private float zFar;
        private float fov;
        private float aspect;    
        private mat4 projectionMatrix;


        private static Camera main = null;

        public static Camera Main
        {
            get {
                if(main == null)
                {                   
                    Camera cam  = new Camera();
                    cam.zNear   = 0.1f;
                    cam.zFar    = 1000;
                    cam.fov     = 60.0f;
                    cam.aspect  = 1024.0f / 768.0f;
                    cam.SetProjectionMatrix(cam.fov, cam.aspect, cam.zNear, cam.zFar);

                    SceneObject obj = new SceneObject();
                    obj.AddComponent(cam);

                    main = cam;
                }
                return main;
            }
            set { main = value; }
        }

        

        public Camera()
        {
            projectionMatrix = mat4.Identity;
        }

        public Transform Transform
        {
            get { return sceneObject.Transform; }
            set { sceneObject.Transform = value; }
        }


        /// <summary>
        /// Creates a new projection matrix with the given values
        /// </summary>
        /// <param name="fov">Field of View</param>
        /// <param name="ratio">Aspect Ratio</param>
        /// <param name="zNear">Near Plane</param>
        /// <param name="zFar">Far Plane</param>
        public void SetProjectionMatrix(float fov, float aspect, float zNear, float zFar)
        {
            this.zNear  = zNear;
            this.zFar   = zFar;
            this.fov    = fov;
            this.aspect = aspect;
            
            projectionMatrix = mat4.Perspective(glm.Radians(fov), aspect, zNear, zFar);
        }



        /// <summary>
        /// Returns the projection matrix.
        /// </summary>
        public mat4 ProjectionMatrix
        {
            get { return projectionMatrix; }
        }



        /// <summary>
        /// Returns the matrix used to transform an object into view space
        /// </summary>
        public mat4 ViewMatrix
        {
            get{ return Transform.InverseWorldMatrix; }
        }


        
        public void LookAt(vec3 eye, vec3 target, vec3 up)
        {
            //vec3 direction = (target - eye).Normalized;
            //vec3 rotation = vec3.Zero;

            //rotation.x = (float)Fun.Degrees(Math.Asin((double)-direction.y));
            //rotation.y = (float)Fun.Degrees(Math.Atan2((double)-direction.x, -direction.z));

            //Transform.Position = eye;
            //Transform.Rotation = new quat(rotation);


            var viewMat = GlmSharp.mat4.LookAt(eye, target, up);

            var translation = vec3.Zero;
            var rotation    = quat.Identity;
            var scale       = vec3.Ones;
            
            Fun.Decompose(viewMat, out translation, out rotation, out scale);

            Transform.Position  = translation;
            Transform.Rotation  = rotation;
            Transform.Scale     = scale;

        }

    }
}
