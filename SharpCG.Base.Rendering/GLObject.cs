using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCG.Base.Rendering
{
    public interface GLObject : IDisposable
    {

        void InitGL();


        void AfterUpdateGPUResources();


        void DeInitGL();


        bool IsDirty
        {
            get;               
        }


    }
}
