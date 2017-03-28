using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;


using dvec2 = GlmSharp.dvec2;
using dvec3 = GlmSharp.dvec3;

using mat2 = GlmSharp.dmat2;
using dmat3 = GlmSharp.dmat3;
using dmat4 = GlmSharp.dmat4;

using quat = GlmSharp.dquat;


namespace SharpCG.Core
{
    public class Transform : Component
    {

        private dvec3 position;
        private quat rotation;
        private dvec3 scale;

        private dmat4 worldMatrix;
        private dmat4 inverseWorldMatrix;
        private dmat3 normalMatrix;

        private bool dirty;

        private dmat4 previousWorldMatrix;


        public Transform()
        {
            position    = dvec3.Zero;
            rotation    = quat.Identity;
            scale       = dvec3.Ones;

            worldMatrix         = dmat4.Identity;
            inverseWorldMatrix  = dmat4.Identity;
            previousWorldMatrix = dmat4.Identity;
        }

        
        public dvec3 WorldScale
        {
            get
            {
                if (Parent == null)
                    return scale;
                return Parent.WorldScale * scale;
            }
            set
            {
                if (Parent == null)
                    scale = value;
                else
                    scale = value / Parent.WorldScale;
                dirty = true;
            }
        }


        public dvec3 WorldPosition
        {
            get
            {
                if (Parent == null)
                    return position;
                return Parent.WorldPosition + position;
            }
            set
            {
                if (Parent == null)
                    position = value;
                else
                    position = value - Parent.WorldPosition;
                dirty = true;
            }
        }


        public dquat WorldRotation
        {
            get
            {
                if (Parent == null)
                    return rotation;
                return rotation * Parent.WorldRotation;
            }
            set
            {
                if (Parent == null)
                    rotation = value;
                else
                {
                    rotation = value;
                    
                    //var angles = value.EulerAngles - Parent.WorldRotation.EulerAngles;
                    //rotation = new quat((vec3)angles);
                }
                dirty = true;
            }
        }


        public void Rotate(dvec3 axis, double angle)
        {
            var rot = new dquat(axis * angle);
            rotation = rot * rotation;
            
            dirty = true;
        }


        public void Translate(dvec3 translation)
        {          
            position    = position + translation;
            dirty       = true;
        }


        public dvec3 Scale
        {
            get => scale; 
            set { scale = value; dirty = true;}
        }

    
        public quat Rotation
        {
            get => rotation; 
            set { rotation = value; dirty = true; }
        }


        public dvec3 Position
        {          
            get => position; 
            set { position = value; dirty = true;}
        }


        public dmat4 WorldMatrix
        {
            get
            {
                if (dirty) UpdateMatrices();

                if (Parent != null)
                    return Parent.WorldMatrix * worldMatrix;

                return worldMatrix;
            }
        }


        public dmat4 InverseWorldMatrix
        {
            get
            {
                if (dirty) UpdateMatrices(); 
                return inverseWorldMatrix;
            }
        }


        public dmat3 NormalMatrix
        {
            get
            {
                if (dirty) UpdateMatrices();
                return normalMatrix;
            }
        }


        /// <summary>
        /// Returns a vector pointing in the forward direction of this object in world space
        /// </summary>
        public dvec3 Forward
        {
            get => -(rotation.ToMat3 * dvec3.UnitZ).Normalized;
        }


        /// <summary>
        /// Returns a vector pointing in the up direction of this object in world space
        /// </summary>
        public dvec3 Up
        {
            get => (rotation.ToMat3 * dvec3.UnitY).Normalized; 
        }


        /// <summary>
        /// Returns a vector pointing in the right direction of this object in world space
        /// </summary>
        public dvec3 Right
        {
            get=> (rotation.ToMat3 * dvec3.UnitX).Normalized;
        }


        public void ToIdentity()
        {
            Position    = dvec3.Zero;
            Scale       = dvec3.Ones;
            Rotation    = quat.Identity;
        }


        public Transform Parent
        {
            get
            {
                if (this.sceneObject == null || this.sceneObject.Parent == null)
                    return null;
                return this.sceneObject.Parent.Transform;
            }            
        }


        public dmat4 PreviousWorldMatrix
        {
            get => previousWorldMatrix;
        }


        public void LookAt(dvec3 eye, dvec3 target, dvec3 up)
        {
            
            var viewMat = dmat4.LookAt(eye, target, up);
            
            var translation = dvec3.Zero;
            var rotation = quat.Identity;
            var scale = dvec3.Ones;

            viewMat.Decompose(out translation, out rotation, out scale);

            WorldPosition   = translation;
            Rotation        = rotation;
            WorldScale      = scale;
        }


        private void UpdateMatrices()
        {
            dmat4 T = dmat4.Translate(WorldPosition);
            dmat4 R = rotation.ToMat4;
            dmat4 S = dmat4.Scale(WorldScale);
           

            worldMatrix         = T * R * S;
            inverseWorldMatrix  = worldMatrix.Inverse;
            normalMatrix        = new dmat3(inverseWorldMatrix.Transposed);
            dirty               = false;
        }


        public override void LateUpdate(double deltaTime)
        {
            previousWorldMatrix = this.WorldMatrix;
        }
    }
}