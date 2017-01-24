using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace SharpCG
{
    public struct BlendMode
    {       
        BlendEquationMode equation;
        BlendingFactorSrc src;
        BlendingFactorDest dest;
    }

    public struct StencilMode
    {
        StencilFunction stencilFun;
        StencilOp fail;
        StencilOp zFail;
        StencilOp zPass;
        int @ref;
        int mask;         
    }

    public struct CullMode
    {
        bool faceCullEnabled;
        CullFaceMode mode;
    }

    public struct DepthMode
    {
        bool depthMaskEnabled;
        DepthFunction depthFun;       
    }

    public class RenderConfig
    {

        public RenderConfig()
        {
        }
    }
}
