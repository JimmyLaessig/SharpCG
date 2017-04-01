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
        public static Framebuffer GBuffer(int width, int height)
        {            

            var gBuffer = new Framebuffer();

            gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment0, new vec4(1));   // Diffuse
            gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment1, new vec4(0));   // Specular
            gBuffer.AddRenderTarget(Texture2D.Empty(width, height), FramebufferAttachment.ColorAttachment2, new vec4(0));   // Normals
            gBuffer.AddRenderTarget(Texture2D.Depth(width, height), FramebufferAttachment.DepthAttachment, new vec4(1));    // Depth               

            return gBuffer;         
        }

        public static SceneObject SkyBox(TextureCubeMap cubeMap, Framebuffer framebuffer, RenderPass renderPass)
        {
            SceneObject skybox  = new SceneObject();
            skybox.AddComponent<Skybox>();
            var cube            = skybox.AddComponent(GeometryExtensions.UnitCube);
            var material        = skybox.AddComponent<SkyboxMaterial>();           
            var renderer        = skybox.AddComponent<MeshRenderer>();

            renderer.Mesh                       = cube;
            renderer.Material                   = material;
            renderer.GLState.FaceCullingEnabled = false;
            renderer.GLState.DepthWriteEnabled  = false;
            renderer.GLState.DepthTestEnabled   = true;
            renderer.GLState.Drawbuffers        = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0 };
            renderer.RenderPass                 = renderPass;
            renderer.Framebuffer                = framebuffer;
            material.CubeMapTexture             = cubeMap;

            return skybox;
        }


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
                renderer.GLState.Drawbuffers = new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 };
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
            renderer.GLState.Drawbuffers = new DrawBuffersEnum[] {DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2};

            return obj;
        }

        public static SceneObject PointLight(dvec3 position, dvec3 color, dvec3 attenuation, Framebuffer gBuffer, RenderPass renderPass)
        {
            var obj = new SceneObject();
            obj.Transform.WorldPosition = position;

            var light           = obj.AddComponent<PointLight>();
            light.Color         = color;
            light.Attenuation   = attenuation;

            var lightMaterial       = obj.AddComponent<DeferredLightMaterial>();
            lightMaterial.Light     = light;

            lightMaterial.DiffuseAlbedoTexture  = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment0);
            lightMaterial.SpecularAlbedoTexture = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment1);
            lightMaterial.WorldNormalTexture    = gBuffer.GetRenderTarget(FramebufferAttachment.ColorAttachment2);
            lightMaterial.DepthTexture          = gBuffer.GetRenderTarget(FramebufferAttachment.DepthAttachment);

            var geometry = obj.AddComponent(light.LightGeometry);
            var renderer = obj.AddComponent<MeshRenderer>();
            renderer.Camera = Camera.Main;
            renderer.Material   = lightMaterial;
            renderer.Mesh       = geometry;
            renderer.RenderPass = renderPass;
            renderer.GLState.BlendFactorSrc = BlendingFactorSrc.One;
            renderer.GLState.BlendFactorDest = BlendingFactorDest.One;
            renderer.GLState.BlendingEnabled = true;
            renderer.GLState.FaceCullingEnabled = true;
            renderer.GLState.DepthTestEnabled = true;
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
