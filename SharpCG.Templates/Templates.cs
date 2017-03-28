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


        public static SceneObject AmbientLight(dvec3 lightColor, Framebuffer gBuffer, RenderPass renderPass)
        {
            SceneObject obj     = new SceneObject();
            var light           = obj.AddComponent<AmbientLight>();
            light.Color         = lightColor; 

            var lightMaterial                   = obj.AddComponent<DeferredLightMaterial>();
            lightMaterial.Light                 = light;
            
            lightMaterial.DiffuseAlbedoTexture  = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment0);
            lightMaterial.SpecularAlbedoTexture = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment1);
            lightMaterial.WorldNormalTexture    = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment2);
            lightMaterial.DepthTexture          = gBuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);
            
            var geometry = obj.AddComponent(light.LightGeometry);
            var renderer = obj.AddComponent<MeshRenderer>();

            renderer.Material   = lightMaterial;  
            renderer.Mesh       = geometry;
            renderer.Camera     = null;
            renderer.RenderPass = renderPass;
            renderer.GLState.BlendFactorSrc     = BlendingFactorSrc.One;
            renderer.GLState.BlendFactorDest    = BlendingFactorDest.One;
            renderer.GLState.BlendingEnabled    = true;
            renderer.GLState.FaceCullingEnabled = false;
            renderer.GLState.DepthTestEnabled   = false;
            renderer.GLState.DepthWriteEnabled  = false;

            return obj;
        }
        
        public static SceneObject DirectionalLight(dvec3 lightColor, dvec3 direction, Framebuffer gBuffer, RenderPass renderPass)
        {
            SceneObject obj = new SceneObject();
            obj.Transform.LookAt(obj.Transform.WorldPosition, obj.Transform.WorldPosition + direction, dvec3.UnitY);

            var light   = obj.AddComponent<DirectionalLight>();
            light.Color = lightColor;

            var lightMaterial   = obj.AddComponent<DeferredLightMaterial>();
            lightMaterial.Light = light;

            lightMaterial.DiffuseAlbedoTexture  = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment0);
            lightMaterial.SpecularAlbedoTexture = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment1);
            lightMaterial.WorldNormalTexture    = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment2);
            lightMaterial.DepthTexture          = gBuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);

            var geometry = obj.AddComponent(light.LightGeometry);
            var renderer = obj.AddComponent<MeshRenderer>();

            renderer.Material = lightMaterial;
            renderer.Mesh = geometry;
            renderer.Camera = null;
            renderer.RenderPass = renderPass;
            renderer.GLState.BlendFactorSrc = BlendingFactorSrc.One;
            renderer.GLState.BlendFactorDest = BlendingFactorDest.One;
            renderer.GLState.BlendingEnabled = true;
            renderer.GLState.FaceCullingEnabled = false;
            renderer.GLState.DepthTestEnabled = false;
            renderer.GLState.DepthWriteEnabled = false;

            return obj;
        }









        public static SceneObject RenderDepthTexture(Texture2D source, Framebuffer target, RenderPass renderPass)
        {
            SceneObject obj = new SceneObject("Depth Texture Copy");
            obj.AddComponent(GeometryExtensions.FullscreenQuad);


            var mat             = obj.AddComponent<RenderDepthMaterial>();
            mat.DepthTexture    = source;

            var renderer = obj.AddComponent<MeshRenderer>();
            renderer.Framebuffer    = target;
            renderer.RenderPass     = renderPass;
            renderer.Camera         = null;
            renderer.GLState.DepthTestEnabled = false;
            renderer.GLState.DepthWriteEnabled = true;
            renderer.GLState.ColorWriteEnabled = false;
            return obj;
        }
        
    }
}
