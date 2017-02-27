using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Core;
namespace SharpCG.Effects
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
