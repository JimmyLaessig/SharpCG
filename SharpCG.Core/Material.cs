using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using GlmSharp;
using System.Reflection;

namespace SharpCG.Core
{

    public abstract class Material : Component
    {

        protected Shader shader;
        protected Dictionary<string, int> uniformLocations = new Dictionary<string, int>();
        protected HashSet<UniformAttribute> uniformAttributes = new HashSet<UniformAttribute>();

        private RenderConfig renderConfig;               
        private dmat4 viewMatrix;        
        private dmat4 projectionMatrix;       
        private dmat4 worldMatrix;      
        private dmat4 wvpMatrix;       
        private dmat3 normalMatrix;


        public override void OnStart()
        {
            base.OnStart();

            InitUniformLocations();
        }

        [Uniform(Name = "mView")]
        public dmat4 ViewMatrix
        {
            get { return viewMatrix; }
            set { viewMatrix = value; }
        }

        [Uniform(Name = "mProj")]
        public dmat4 ProjectionMatrix
        {
            get { return projectionMatrix; }
            set { projectionMatrix = value; }
        }

        [Uniform(Name = "mWorld")]
        public dmat4 WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        [Uniform(Name = "mWVP")]
        public dmat4 WvpMatrix
        {
            get { return wvpMatrix; }
            set { wvpMatrix = value; }
        }

        [Uniform(Name = "mNormal")]
        public dmat3 NormalMatrix
        {
            get { return normalMatrix; }
            set { normalMatrix = value; }
        }

        public Shader Shader
        {
            get { return shader; }
            set { shader = value; }
        }


        public Material()
        {

        }


        private static IEnumerable<PropertyInfo> GetAllProperties(Type t, Type baseType)
        {
            BindingFlags flags =    BindingFlags.Public     |
                                    BindingFlags.NonPublic  |
                                    BindingFlags.Static     |
                                    BindingFlags.Instance   |
                                    BindingFlags.DeclaredOnly;

            if (t == null) return Enumerable.Empty<PropertyInfo>();
            if (t == baseType) return t.GetProperties(flags);

           

            return t.GetProperties(flags).Concat(GetAllProperties(t.BaseType, baseType));
        }


        private void InitUniformLocations()
        {
            if (shader == null)
                return;

            var fields = GetAllProperties(this.GetType(), typeof(Material)).Where(field => field.IsDefined(typeof(UniformAttribute), false)).ToList();
            fields.ElementAt(0).GetCustomAttribute<UniformAttribute>();

            fields.ForEach(field => {
                var attribute = field.GetCustomAttribute<UniformAttribute>();
                attribute.Location = GL.GetUniformLocation(Shader.ProgramHandle, attribute.Name);
                attribute.Value = field.GetValue(this);
                attribute.Property = field;
                if(attribute.Location >= 0)
                {                                                     
                    uniformAttributes.Add(attribute);
                }                             
            });


        }

        private void BindUniform(Object val, int location, Shader shader)
        {

            if (val == null)
                return;
            // elementar datatypes
            if      (val.GetType() == typeof(bool))     { GL.Uniform1(location, ((bool)val) ? 1 : 0); }
            else if (val.GetType() == typeof(int))      { GL.Uniform1(location, (int)val); }
            else if (val.GetType() == typeof(float))    { GL.Uniform1(location, (float)val); }       
            else if (val.GetType() == typeof(double))   { GL.Uniform1(location, (double)val); }
            // Vectors
            else if (val.GetType() == typeof(vec2))     { GL.Uniform2(location, 1, ((vec2)val).Values);}
            else if (val.GetType() == typeof(dvec2))    { GL.Uniform2(location, 1, ((vec2)val).Values);}
            else if (val.GetType() == typeof(vec3))     { GL.Uniform3(location, 1, ((vec3)val).Values);}
            else if (val.GetType() == typeof(dvec3))    { GL.Uniform3(location, 1, ((vec3)val).Values);}
            else if (val.GetType() == typeof(vec4))     { GL.Uniform4(location, 1, ((vec4)val).Values);}
            else if (val.GetType() == typeof(dvec4))    { GL.Uniform4(location, 1, ((vec4)val).Values);}
            // Matrices               {
            else if (val.GetType() == typeof(mat2))     { GL.UniformMatrix2(location, 1, false, ((mat2)val).Values1D); }
            else if (val.GetType() == typeof(dmat2))    { GL.UniformMatrix2(location, 1, false, ((dmat2)val).ToMat2().Values1D); }
            else if (val.GetType() == typeof(mat3))     { GL.UniformMatrix3(location, 1, false, ((mat3)val).Values1D); }
            else if (val.GetType() == typeof(dmat3))    { GL.UniformMatrix3(location, 1, false, ((dmat3)val).ToMat3().Values1D); }
            else if (val.GetType() == typeof(mat4))     { GL.UniformMatrix4(location, 1, false, ((mat4)val).Values1D); }
            else if (val.GetType() == typeof(dmat4))    { GL.UniformMatrix4(location, 1, false, ((dmat4)val).ToMat4().Values1D); }

            // Arrays 
            else if (val.GetType() == typeof(bool[]))
            {
                var x = ((bool[])val).ToList().ConvertAll(b => b ? 1 : 0).ToArray();
                GL.Uniform1(location, x.Length, x);
            }
            else if (val.GetType() == typeof(int[]))
            {
                var x = ((int[])val);
                GL.Uniform1(location, x.Length, x);
            }
            else if (val.GetType() == typeof(float[]))
            {
                var x = ((float[])val);
                GL.Uniform1(location, x.Length, x);
            }
            else if (val.GetType() == typeof(double[]))
            {
                var x = ((double[])val);
                GL.Uniform1(location, x.Length, x);
            }


            // Matrix Arrays
            else if (val.GetType() == typeof(mat2[]) )
            {
                    var x = ((mat2[])val).ToArray();
                    GL.UniformMatrix2(location, x.Length, false, x);
            }
            else if (val.GetType() == typeof(dmat2[]) )
            {
                    var x = ((dmat2[])val).ToFloatArray();
                    GL.UniformMatrix2(location, x.Length, false, x);
            }
            else if (val.GetType() == typeof(mat3[]) )
            {
                    var x = ((mat3[])val).ToArray();
                    GL.UniformMatrix3(location, x.Length, false, x);
            }
            else if (val.GetType() == typeof(dmat3[]) )
            {
                    var x = ((dmat3[])val).ToFloatArray();
                    GL.UniformMatrix3(location, x.Length, false, x);
            }
            else if (val.GetType() == typeof(mat4[]) )
            {
                    var x = ((mat4[])val).ToArray();
                    GL.UniformMatrix3(location, x.Length, false, x);
            }
            else if (val.GetType() == typeof(dmat4[]) )
            {
                    var x = ((dmat4[])val).ToFloatArray();
                    GL.UniformMatrix3(location, x.Length, false, x);
            }
            // Texture
            else if (val.GetType() == typeof(Texture2D))
            {
                var tex = (Texture2D)val;
                var unit = shader.NextFreeTextureUnit;
                tex.Bind(unit);
                GL.Uniform1(location, unit - TextureUnit.Texture0);
            }
            else if (val.GetType() == typeof(TextureCubeMap))
            {
                var tex = (TextureCubeMap)val;
                var unit = shader.NextFreeTextureUnit;
                tex.Bind(unit);
                GL.Uniform1(location, unit - TextureUnit.Texture0);
            }
            else
            {
                Console.WriteLine("Unsupported Uniform Type :" + val.GetType().Name);
            }
        }
        
        public void BindRenderConfig()
        {
            //
        }


        /// <summary>
        /// Binds the material properties and the shader
        /// TextureUnit contains the next unactive texture.
        /// </summary>
        public void Bind()
        {
            if (shader == null)
                return;

            shader.bind();

            uniformAttributes.ToList().ForEach(attr => 
            {
                var value = attr.Property.GetValue(this);
                if (value != null) BindUniform(value, attr.Location, shader);
            });
            
        }
    }



    [AttributeUsage(AttributeTargets.Property)]
    public class UniformAttribute : Attribute
    {
        public string Name  = "";
        public int Location = -1;
        public object Value;
        public PropertyInfo Property;
        //public Action uniformBindFunction;
    }
}
