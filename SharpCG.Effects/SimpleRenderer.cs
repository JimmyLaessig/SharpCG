using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using OpenTK.Graphics.OpenGL4;

using SharpCG.Core;

namespace SharpCG.Effects
{
    public class ColoredMaterial : Material
    {
        private vec4 color;

        public vec4 Color
        {
            get{return color;  }
            set{color = value; }
        }

        public override void OnStart()
        {
            shader = Shader.Find("uniformColor");
            base.OnStart();
        }

        protected override void InitUniformLocations()
        {
            uniformLocations["vColor"] = GL.GetUniformLocation(shader.ProgramHandle, "vColor");

            base.InitUniformLocations();
        }

        public override void Bind(ref uint textureUnit)
        {
            shader.bind();
            GL.Uniform4(uniformLocations["vColor"], 1, color.Values);
            base.Bind(ref textureUnit);
        }
    }


    public class SimpleRenderer : Renderer
    {

        private Material material;
        private Geometry mesh;

        public override void OnStart()
        {
            base.OnStart();
            material    = this.sceneObject.FindComponent<Material>();
            mesh        = this.sceneObject.FindComponent<Geometry>();
        }


        public override void RenderGL()
        {
            Camera camera = Camera.Main;
            GL.Disable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.CullFace);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            material.WorldMatrix        = sceneObject.Transform.WorldMatrix;
            material.ProjectionMatrix   = camera.ProjectionMatrix;
            material.ViewMatrix         = camera.ViewMatrix;
            material.WvpMatrix          = material.ProjectionMatrix * material.ViewMatrix * material.WorldMatrix;

            uint unit = 0;
            material.Bind(ref unit);
            mesh.Bind();
            if (mesh.HasIndices)
            {
                GL.DrawElements(mesh.PrimitiveType, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                GL.DrawArrays(mesh.PrimitiveType, 0, mesh.TriangleCount * 3);
            }
        }
    }
}
