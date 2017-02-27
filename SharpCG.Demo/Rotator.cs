using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GlmSharp;
using SharpCG.Core;
using SharpCG.Effects;

namespace SharpCG.Demo
{
    class Rotator : Component
    {

        public override void Update(double deltaTime)
        {
            this.sceneObject.Transform.Rotate(vec3.UnitY, glm.Radians(90.0) * deltaTime);
            
            base.Update(deltaTime);
        }
    }
}