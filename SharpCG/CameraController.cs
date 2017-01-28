using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;
using OpenTK.Input;



namespace SharpCG
{
    public class CameraController : Component
    {



        private Camera camera;

        int lastMousePosX;
        int lastMousePosY;

        public double MoveSpeed      = 2.0f;
        public double RotationSpeed  = 0.2f;

        private vec3 rotation;
        private vec3 position;

        /// <summary>
        /// Creates a new CameraController
        /// </summary>
        public CameraController()
        {
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
            var mouse = OpenTK.Input.Mouse.GetState();
            int mousePosX = mouse.X;
            int mousePosY = mouse.Y;

            if (Enabled)
            {
                GetKeyboardInput(deltaT);
                GetMouseInput(mousePosX, mousePosY);
            }

            lastMousePosX = mousePosX;
            lastMousePosY = mousePosY;

           
        }



        /// <summary>
        /// Handles user input for camera movement fom the keyboard
        /// </summary>
        private void GetKeyboardInput(double deltaT)
        {

            var keyboard = OpenTK.Input.Keyboard.GetState();
            if (keyboard.IsKeyDown(Key.W))
            {
                camera.Transform.Translate(camera.Transform.Forward * (float)(MoveSpeed * deltaT));
            }
            if (keyboard.IsKeyDown(Key.A))
            {
                camera.Transform.Translate(-camera.Transform.Right * (float)(MoveSpeed * deltaT));
            }
            if (keyboard.IsKeyDown(Key.S))
            {
                camera.Transform.Translate(-camera.Transform.Forward * (float)(MoveSpeed * deltaT));
            }
            if (keyboard.IsKeyDown(Key.D))
            {
                camera.Transform.Translate(camera.Transform.Right * (float)(MoveSpeed * deltaT));
            }
        }



        /// <summary>
        /// Handles user input for camera movement from the mouse
        /// </summary>
        private void GetMouseInput(int mousePosX, int mousePosY)
        {
            var mouse = OpenTK.Input.Mouse.GetState();

            if (mouse[MouseButton.Right])
            {
                int diffX = mouse.X - lastMousePosX;
                int diffY = mouse.Y - lastMousePosY;

                rotation.x += (float) (Fun.Radians(-diffY) * RotationSpeed);
                rotation.y += (float) (Fun.Radians(-diffX) * RotationSpeed);           
                rotation.z  = 0;
                rotation    = normalizeAngles(rotation);
                
                camera.Transform.Rotation = new quat(rotation).Normalized;
            }
        }

        private vec3 normalizeAngles(vec3 angles)
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
