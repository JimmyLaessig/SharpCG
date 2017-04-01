using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
namespace SharpCG.Core
{
    public class GLState
    {
        // BlendMode
        public bool BlendingEnabled;
        public BlendEquationMode BlendEquation;
        public BlendingFactorSrc BlendFactorSrc;
        public BlendingFactorDest BlendFactorDest;
        
        // DepthTestMode
        public bool DepthTestEnabled;
        public DepthFunction DepthFunction;
        public bool DepthWriteEnabled;
        
        // CullMode
        public bool FaceCullingEnabled;
        public CullFaceMode CullFaceMode;

        public bool ColorWriteEnabled;


        public DrawBuffersEnum[] Drawbuffers;


        public GLState()
        {
            // Default Blend State
            BlendingEnabled = false;
            BlendEquation   = BlendEquationMode.FuncAdd;
            BlendFactorSrc  = BlendingFactorSrc.Src1Alpha;
            BlendFactorDest = BlendingFactorDest.OneMinusSrc1Alpha;

            // Default Face Culling State
            FaceCullingEnabled  = true;
            CullFaceMode        = CullFaceMode.Back;

            // Default Depth Test State
            DepthTestEnabled    = true;
            DepthFunction       = DepthFunction.Lequal;
            DepthWriteEnabled   = true;

            ColorWriteEnabled   = true;

            Drawbuffers         = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0};
        }


        public void Bind()
        {
            // Blending
            if (BlendingEnabled)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendEquation(BlendEquation);
                GL.BlendFunc(BlendFactorSrc, BlendFactorDest);
            }
            else
            {
                GL.Disable(EnableCap.Blend);
            }
            
            // Culling
            if (FaceCullingEnabled)
            {
                GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode);
            }
            else
            {
                GL.Disable(EnableCap.CullFace);
            }

            
            GL.DepthMask(DepthWriteEnabled);
            GL.ColorMask(ColorWriteEnabled, ColorWriteEnabled, ColorWriteEnabled, ColorWriteEnabled);
            
            // DepthTestMode
            if(DepthTestEnabled)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction);
            }
            else
            {
                GL.Disable(EnableCap.DepthTest);
            }
            if (Drawbuffers.Length == 0)
                GL.DrawBuffer(DrawBufferMode.None);
            else
            {
                GL.DrawBuffers(Drawbuffers.Length, Drawbuffers);
            }
        }

    }
}
