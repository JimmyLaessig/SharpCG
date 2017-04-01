using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;
using OpenTK.Input;

namespace SharpCG.Core
{
    public class CameraController : Component
    {

        private Camera camera;

        dvec2 lastMousePos;
       

        public double MoveSpeed      = 200.0;
        public double RotationSpeed  = 90.0f;

        private dvec3 rotation;
        private dvec3 position;



        private dvec3 translationForce   = dvec3.Zero;
        private dvec3 rotationForce      = dvec3.Zero;


        public override void OnStart()
        {
            camera = this.sceneObject.FindComponent<Camera>();
            this.rotation = camera.Transform.Rotation.EulerAngles;
            this.position = camera.Transform.Position;
        }


        /// <summary>
        /// Sets the camera object to the given camera
        /// </summary>
        public Camera Camera
        {
            set
            {
                this.camera = value;
                this.position = camera.Transform.Position;
                this.rotation = (vec3)camera.Transform.Rotation.EulerAngles;
            }
            get
            {
                return camera;
            }
        }

       
        

        /// <summary>
        /// Updates the CameraController
        /// </summary>
        public override void Update(double deltaT)
        {
            var mouseState  = OpenTK.Input.Mouse.GetState();
            var mousePosX   = new dvec2((float)mouseState.X / 1024.0f, (float)mouseState.Y / 768.0f);

            var delta = mousePosX - lastMousePos;


            if (Enabled)
            {
                translationForce += ControlWASD(deltaT, OpenTK.Input.Keyboard.GetState());
                translationForce += ControlPan(delta, mouseState);

                translationForce = glm.Clamp(translationForce, -dvec3.Ones, dvec3.Ones);
                
                camera.Transform.Translate(translationForce * MoveSpeed);



                //rotationForce += ControlRotation(delta, mouseState);
                //rotationForce = glm.Clamp(rotationForce, -dvec3.Ones, dvec3.Ones);
                //rotation += rotationForce * RotationSpeed;
                //rotation = normalizeAngles(rotation);


                rotation += ControlRotation(delta, mouseState) * RotationSpeed;
                //rotation = rotation.ClampCircular(glm.Radians(85.0), 0, 0);
                rotation = normalizeAngles(rotation);
                camera.Transform.Rotation = new dquat(rotation).Normalized;

                rotationForce       /= 1.1;
                translationForce    /= 1.1;

                if (rotationForce.Length <= 0.0001) rotationForce = dvec3.Zero;               
                if (translationForce.Length <= 0.0001) translationForce = dvec3.Zero;

            }

            
            lastMousePos = mousePosX;                    
        }



        private double translationForceFactor = 0.1f;
        private double rotationForceFactor = 1.0f;
        

        /// <summary>
        /// Returns the acceleration vector from WASD input
        /// </summary>
        private dvec3 ControlWASD(double deltaT, KeyboardState state)
        {
            // Decrease speed
            var relativeForce = translationForceFactor * deltaT;

            dvec3 force = vec3.Zero;

            if (state.IsKeyDown(Key.W)) force += camera.Transform.Forward   * relativeForce;           
            if (state.IsKeyDown(Key.A)) force -= camera.Transform.Right     * relativeForce;           
            if (state.IsKeyDown(Key.S)) force -= camera.Transform.Forward   * relativeForce;            
            if (state.IsKeyDown(Key.D)) force += camera.Transform.Right     * relativeForce;

            return force;  
        }


        private dvec3 ControlPan(dvec2 deltaPos, MouseState state)
        {

            var relativeForce = translationForceFactor * deltaPos;
            
            dvec3 force = dvec3.Zero;

            if (state[MouseButton.Middle])
            {
                force += camera.Transform.Right * relativeForce.x;
                force += -camera.Transform.Up   * relativeForce.y;
            }
            return force;
        }


        private dvec3 ControlRotation(dvec2 deltaPos, MouseState state)
        {
            var relativeForce = rotationForceFactor * deltaPos;

            dvec3 force = dvec3.Zero;
            if (state[MouseButton.Right])
            {
                force.x += glm.Radians(-relativeForce.y);
                force.y += glm.Radians(-relativeForce.x);
                force.z += 0;         
            }

            return force;
        }

        private dvec3 normalizeAngles(dvec3 angles)
        {
            float TWO_PI = (float)(Math.PI * 2);
            float MAX_VERTICAL_ANGLE = glm.Radians(85.0f);


            angles.x = angles.x % TWO_PI;
            //fmodf can return negative values, but this will make them all positive
            if (angles.y < 0.0f)
                angles.y += TWO_PI;
            if (angles.y > TWO_PI)
                angles.y -= TWO_PI;

            if (angles.x > MAX_VERTICAL_ANGLE)
                angles.x = MAX_VERTICAL_ANGLE;
            else if (angles.x < -MAX_VERTICAL_ANGLE)
                angles.x = -MAX_VERTICAL_ANGLE;

            angles.z = 0;
            return angles;
        }
    }
}
