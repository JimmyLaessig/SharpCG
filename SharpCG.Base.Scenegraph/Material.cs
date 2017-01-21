﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCG.Base.Rendering;
using OpenTK.Graphics.OpenGL4;
using GlmSharp;

namespace SharpCG.Base.Scenegraph
{

    public abstract class Material : Component
    {
        public struct RenderConfig
        {
            bool cullFaceEnabled;
            CullFaceMode cullFace;
            BlendEquationMode BlendEquationMode;
            BlendingFactorSrc serc;                   
            BlendingFactorDest dst;

            DepthFunction depthFunction;
            bool DepthMask;                       
        }


        protected Shader shader;
        protected Dictionary<string, int> uniformLocations = new Dictionary<string, int>();

        private RenderConfig renderConfig;

        private mat4 viewMatrix;
        private mat4 projectionMatrix;
        private mat4 worldMatrix;
        private mat4 wvpMatrix;
        private mat3 normalMatrix;

        public mat4 ViewMatrix
        {
            get
            {
                return viewMatrix;
            }

            set
            {
                viewMatrix = value;
            }
        }

        public mat4 ProjectionMatrix
        {
            get
            {
                return projectionMatrix;
            }

            set
            {
                projectionMatrix = value;
            }
        }

        public mat4 WorldMatrix
        {
            get
            {
                return worldMatrix;
            }

            set
            {
                worldMatrix = value;
            }
        }

        public mat4 WvpMatrix
        {
            get
            {
                return wvpMatrix;
            }

            set
            {
                wvpMatrix = value;
            }
        }

        public mat3 NormalMatrix
        {
            get
            {
                return normalMatrix;
            }

            set
            {
                normalMatrix = value;
            }
        }

        public Shader Shader
        {
            get
            {
                return shader;
            }

            set
            {
                shader = value;
            }
        }

        public Material()
        {
            InitUniformLocations();
        }

        protected virtual void InitUniformLocations()
        {           
            uniformLocations["mWorld"]  = GL.GetUniformLocation(Shader.ProgramHandle, "mWorld");
            uniformLocations["mView"]   = GL.GetUniformLocation(Shader.ProgramHandle, "mView");
            uniformLocations["mProj"]   = GL.GetUniformLocation(Shader.ProgramHandle, "mProj");
            uniformLocations["mNormal"] = GL.GetUniformLocation(Shader.ProgramHandle, "mNormal");
            uniformLocations["mWVP"]    = GL.GetUniformLocation(Shader.ProgramHandle, "mWVP");          
        }

        /// <summary>
        /// Returns all texture used in this material as a list. 
        /// This method is used by the render control to collect all textures and perform resource updates on them. 
        /// If textures are not returned with this function, updates must be performed manually. 
        /// </summary>
        /// <returns></returns>
        public virtual List<Texture> GetMaterialTextures()
        {
            return new List<Texture>();
        }


        public void BindRenderConfig()
        {
          //
        }

        /// <summary>
        /// Binds the material properties and the shader
        /// TextureUnit contains the next unactive texture.
        /// </summary>

        public virtual void Bind(ref uint textureUnit)
        {
            Shader.bind();            
            GL.UniformMatrix4(uniformLocations["mView"]     , 1, false, ViewMatrix.Values1D);
            GL.UniformMatrix4(uniformLocations["mProj"]     , 1, false, ProjectionMatrix.Values1D);
            GL.UniformMatrix4(uniformLocations["mWorld"]    , 1, false, WorldMatrix.Values1D);
            GL.UniformMatrix4(uniformLocations["mWVP"]      , 1, false, WvpMatrix.Values1D);
            GL.UniformMatrix3(uniformLocations["mNormal"]   , 1, false, NormalMatrix.Values1D);            
        }
    }
}
