using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;



namespace SharpCG
{
    public class Transform : Component
    {

        private vec3 position;
        private quat rotation;
        private vec3 scale;

        private mat4 worldMatrix;
        private mat4 inverseWorldMatrix;
        private mat3 normalMatrix;

        private bool dirty;


        public static Transform Identity
        {
            get { return  new Transform(); }
            }

        public Transform()
        {
            position    = vec3.Zero;
            rotation    = quat.Identity;
            scale       = vec3.Ones;


            worldMatrix = mat4.Identity;
            inverseWorldMatrix = mat4.Identity;

        }



        public void Rotate(vec3 axis, float angle)
        {
            var rot = new quat(axis * angle);
            rotation = rot * rotation;
            
            dirty = true;
        }


        public void Translate(vec3 translation)
        {          
            position    = position + translation;
            dirty       = true;
        }



        public vec3 Scale
        {
            get {return scale; }
            set{ scale = value; dirty = true;}
        }


        public quat Rotation
        {
            get { return rotation; }
            set { rotation = value; dirty = true; }
        }


        public vec3 Position
        {          
            get { return position; }
            set { position = value; dirty = true;}
        }


        public mat4 WorldMatrix
        {
            get
            {
                if (dirty) UpdateMatrices();


                if (Parent != null)
                    return Parent.WorldMatrix * worldMatrix;

                return worldMatrix;
            }
        }


        public mat4 InverseWorldMatrix
        {
            get
            {
                if (dirty) UpdateMatrices(); 
                return WorldMatrix.Inverse;
            }
        }



        public mat3 NormalMatrix
        {
            get
            {
                if (dirty) UpdateMatrices();
                return new mat3(InverseWorldMatrix.Transposed);
            }
        }


        public vec3 Forward
        {
            get{ return -(rotation.ToMat3 * vec3.UnitZ).Normalized;}
        }


        /// <summary>
        /// Returns a vector pointing in the up direction of this object in world space
        /// </summary>
        public vec3 Up
        {
            get{ return (rotation.ToMat3 * vec3.UnitY).Normalized; }
        }


        /// <summary>
        /// Returns a vector pointing in the right direction of this object in world space
        /// </summary>
        public vec3 Right
        {
            get{ return (rotation.ToMat3 * vec3.UnitX).Normalized; }
        }

        public void ToIdentity()
        {
            Position    = vec3.Zero;
            Scale       = vec3.Ones;
            Rotation    = quat.Identity;
        }

        public Transform Parent
        {
            get
            {
                if (this.sceneObject.Parent == null)
                    return null;
                return this.sceneObject.Parent.Transform;
            }            
        }

        private void UpdateMatrices()
        {
            mat4 T = mat4.Translate(position);
            mat4 R = rotation.ToMat4;
            mat4 S = mat4.Scale(scale);
           

            worldMatrix         = T * R * S;
            //inverseWorldMatrix  = worldMatrix.Inverse;
            //normalMatrix        = new mat3(inverseWorldMatrix.Transposed);
            dirty               = false;
        }
    }
}