using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;
using SharpCG.Core;
using SharpCG.Effects;

namespace SharpCG.Templates
{
    public static class Templates
    {
        public static SceneObject CreateDeferred(SceneObject root, Framebuffer gBuffer, RenderPass renderPass)
        {
            // Remove old renderer components
            SceneObject.TraverseAndExecute(root, obj => obj.RemoveComponents<Renderer>());

            SceneObject.TraverseAndExecute<Geometry>(root, geometry =>
            {
                var obj   = geometry.SceneObject;    
                // Remove old material        
                obj.RemoveComponent(geometry.Material);
                // Add new Component
                var newMaterial = obj.AddComponent<DeferredGeometryMaterial>();
                // Convert old (Assimp) material to DeferredGeometryMaterial
                if (geometry.Material != null && geometry.Material is AssimpMaterial)
                {                    
                    newMaterial.ConvertFrom((AssimpMaterial)geometry.Material);      
                }


                geometry.Material       = newMaterial;
                var renderer            = obj.AddComponent<MeshRenderer>();
                renderer.Mesh           = geometry;
                renderer.Material       = geometry.Material;
                renderer.Framebuffer    = gBuffer;
                renderer.RenderPass     = renderPass;
            });
            return root;
        }


        public static SceneObject AmbientLight()
        {
            return null;
        }









        public static SceneObject RenderDepthTexture(Texture2D source, Framebuffer target, RenderPass renderPass)
        {
            SceneObject obj = new SceneObject("Depth Texture Copy");
            obj.AddComponent(GeometryExtensions.FullscreenQuad);


            var mat = obj.AddComponent<RenderDepthMaterial>();
            mat.DepthTexture = source;

            var renderer = obj.AddComponent<MeshRenderer>();
            renderer.Framebuffer = target;
            renderer.RenderPass = renderPass;
            return obj;
        }
        
    }
}
