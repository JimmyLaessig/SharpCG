using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCG
{
    public class ShadowMapMaterial : Material
    {

        public override void OnStart()
        {
            Shader = Shader.Find("ShadowMap");
            base.OnStart();
        }
    }
}
