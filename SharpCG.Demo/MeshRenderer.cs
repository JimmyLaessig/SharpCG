﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using OpenTK.Graphics.OpenGL4;
using SharpCG.Base;
using SharpCG.Base.Rendering;
using SharpCG.Base.Scenegraph;


class MeshRenderer : Component, Renderable
{
    private Mesh mesh;
    private SimpleLightingMaterial material;

    public override void OnStart()
    {
        mesh = (Mesh)sceneObject.Components.Find(c => c is Mesh);
        material = (SimpleLightingMaterial)sceneObject.Components.Find(c => c is SimpleLightingMaterial);
    }

    public void Render()
    {
        Camera camera = Camera.Main;

        GL.Enable(EnableCap.CullFace);
        
        //GL.DepthMask(true);

        // Uniforms for matrices
        material.WorldMatrix        = sceneObject.Transform.WorldMatrix;
        material.ProjectionMatrix   = camera.ProjectionMatrix;
        material.ViewMatrix         = camera.ViewMatrix;
        material.WvpMatrix          = material.ProjectionMatrix * material.ViewMatrix * material.WorldMatrix;
        material.NormalMatrix       = sceneObject.Transform.NormalMatrix;

        material.NormalMappingEnabled = true;

        // Set lighting parameters
        material.ViewPosition       = camera.Transform.Position;
        
        material.LightDirection     = new vec3(0, -1, 0);
        material.LightColor         = new vec3(0.7f, 0.7f, 0.6f);
        material.LightAmbientColor  = new vec3(0.4f, 0.4f, 0.4f);
        
        uint unit = 0;
        material.Bind(ref unit);

        mesh.Bind();
        GL.DrawElements(BeginMode.Triangles, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);

        material.Shader.release();

        //GL.Enable(EnableCap.CullFace);
        //GL.DepthMask(true);
    }
}

