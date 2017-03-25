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
        
        // CullMode
        public bool FaceCullingEnabled;
        public CullFaceMode CullFaceMode;


        public GLState()
        {
            // Default Blend State
            BlendingEnabled = false;
            BlendEquation   = BlendEquationMode.FuncAdd;
            BlendFactorSrc  = BlendingFactorSrc.Src1Alpha;
            BlendFactorDest = BlendingFactorDest.OneMinusSrc1Alpha;

            // Default Face Culling State
            FaceCullingEnabled  = false;
            CullFaceMode        = CullFaceMode.Back;

            // Default Depth Test State
            DepthTestEnabled = true;
            DepthFunction       = DepthFunction.Lequal;
            
        }


        public void ApplyState()
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
        }

    }
}
