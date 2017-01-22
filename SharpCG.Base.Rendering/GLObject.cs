using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCG.Base.Rendering
{
    public interface GLObject : IDisposable
    {
        void UpdateGPUResources();


        void FreeGPUResources();


        bool IsDirty
        {
            get;               
        }


    }
}
