using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;
namespace SharpCG.Demo
{
    class Rotator : Component
    {

        public override void Update(double deltaTime)
        {
            this.sceneObject.Transform.Rotate(vec3.UnitY, Fun.Radians(90.0f) * (float)deltaTime);
            
            base.Update(deltaTime);
        }
    }
}