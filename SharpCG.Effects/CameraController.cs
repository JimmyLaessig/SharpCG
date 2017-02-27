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
       

        public double MoveSpeed      = 10.0f;
        public double RotationSpeed  = 90.0f;

        private dvec3 rotation;
        private dvec3 position;


        public override void OnStart()
        {
            camera = this.sceneObject.FindComponent<Camera>();
            this.rotation = (vec3)camera.Transform.Rotation.EulerAngles;
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

        private dvec3 speed = dvec3.Zero;


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
                ControlWASD(deltaT, OpenTK.Input.Keyboard.GetState());
                ControlPan(delta, mouseState);
                ControlRotation(delta, mouseState);
            }

            
            lastMousePos = mousePosX;                    
        }

       

        /// <summary>
        /// Handles user input for camera movement fom the keyboard
        /// </summary>
        private void ControlWASD(double deltaT, KeyboardState state)
        {
            // Decrease speed
            speed = speed / 1.1f;
            if (speed.Length <= 0.0001) speed = dvec3.Zero;

            if (state.IsKeyDown(Key.W)) speed.x += 0.1f;           
            if (state.IsKeyDown(Key.A)) speed.y -= 0.1f;           
            if (state.IsKeyDown(Key.S)) speed.x -= 0.1f;            
            if (state.IsKeyDown(Key.D)) speed.y += 0.1f;
            

            speed = glm.Clamp(speed, -dvec3.Ones, dvec3.Ones);

            dvec3 translation = camera.Transform.Forward * speed.x + camera.Transform.Right * speed.y + camera.Transform.Up * speed.z;

            camera.Transform.Translate(translation * (MoveSpeed * deltaT));
        }


        private void ControlPan(dvec2 deltaPos, MouseState state)
        {
            if(state[MouseButton.Middle])
            {
                camera.Transform.Translate(camera.Transform.Right * deltaPos.x * (float) MoveSpeed);
                camera.Transform.Translate(-camera.Transform.Up * deltaPos.y * (float) MoveSpeed);
            }
        }


        private void ControlRotation(dvec2 deltaPos, MouseState state)
        {
            if (state[MouseButton.Right])
            {
                rotation.x += (glm.Radians(-deltaPos.y * RotationSpeed));
                rotation.y += (glm.Radians(-deltaPos.x * RotationSpeed));
                rotation.z = 0;
                rotation = normalizeAngles(rotation);

                camera.Transform.Rotation = new dquat(rotation).Normalized;
            }
        }

        private dvec3 normalizeAngles(dvec3 angles)
        {
            float TWO_PI                = (float)(Math.PI * 2);
            float MAX_VERTICAL_ANGLE    = glm.Radians(85.0f);


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
