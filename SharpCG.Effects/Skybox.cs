using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

using OpenTK.Graphics.OpenGL4;

using GlmSharp;
using SharpCG.Core;



namespace SharpCG.Effects
{
    public class Skybox : Component
    {

        private Geometry mesh;
        private SkyboxMaterial material;

       

        public override void OnStart()
        {
            mesh = (Geometry)sceneObject.Components.Find(c => c is Geometry);
            material = (SkyboxMaterial)sceneObject.Components.Find(c => c is SkyboxMaterial);
        }

        public override void Update(double deltaTime)
        {
            this.sceneObject.Transform.WorldPosition = Camera.Main.Transform.WorldPosition;
        }
    }
}

