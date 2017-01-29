using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;

namespace SharpCG.Base.Scenegraph
{


    public class TextureToDepthRenderer : Renderer
    {


        private Mesh fullscreenQuad;
        private TextureToDepthMaterial material;


        public override void OnStart()
        {
            fullscreenQuad  = sceneObject.FindComponent<Mesh>();
            material        = sceneObject.FindComponent<TextureToDepthMaterial>();
        }



        public override void InitGL()
        {
            base.InitGL();
        }



        public override void RenderGL()
        {
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            //GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.DepthFunc(DepthFunction.Lequal);
            material.WvpMatrix      = mat4.Identity;

            uint unit               = 0;
            material.Bind(ref unit);

            fullscreenQuad.Bind();
            GL.DrawElements(BeginMode.Triangles, fullscreenQuad.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            GL.DrawBuffer(DrawBufferMode.Back);
            GL.ReadBuffer(ReadBufferMode.Back);
            //GL.Enable(EnableCap.DepthTest);
        }
    }
}
