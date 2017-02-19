using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;


namespace SharpCG
{
    public class Shader : IDisposable
    {
        int programHandle = 0;
        int vertexShader = 0;
        int fragmentShader = 0;
        int geometryShader = 0;
        int evaluationShader = 0;
        int controlShader = 0;
        int computeShader = 0;

        /// <summary>
        /// Map that stores all shaders created on Initialization
        /// </summary>
        private static Dictionary<string, Shader> Shaders = new Dictionary<string, Shader>();


        private static string ERROR_MESSAGE = "";

        /// <summary>
        /// The Directory of the Shader Files
        /// </summary>
        private static string SHADER_FILE_DIRECTORY = "Shader";



        /// <summary>
        /// Returns the program handle to this shader object
        /// </summary>
        public int ProgramHandle
        {
            get { return programHandle; }
        }



        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            GL.DeleteProgram(programHandle);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(geometryShader);
            GL.DeleteShader(evaluationShader);
            GL.DeleteShader(controlShader);
            GL.DeleteShader(computeShader);

        }



        public void bind()
        {
            GL.UseProgram(programHandle);
        }



        public void release()
        {
            GL.UseProgram(0);
        }



        /// <summary>
        /// Links the shader programs of the shader.
        /// </summary>
        /// <returns>True if the programs were linked successfully</returns>
        private bool link()
        {
            if (vertexShader != 0)
                GL.AttachShader(programHandle, vertexShader);
            if (fragmentShader != 0)
                GL.AttachShader(programHandle, fragmentShader);
            if (geometryShader != 0)
                GL.AttachShader(programHandle, geometryShader);
            if (evaluationShader != 0)
                GL.AttachShader(programHandle, evaluationShader);
            if (controlShader != 0)
                GL.AttachShader(programHandle, controlShader);

            GL.LinkProgram(programHandle);

            int linked;
            GL.GetProgram(programHandle, GetProgramParameterName.LinkStatus, out linked);

            if (linked == 0)
            {
                string message = "";
                GL.GetProgramInfoLog(programHandle, out message);

                ERROR_MESSAGE += "Failed to link Shader Program" + message;
                return false;
            }
            return true;
        }



        /// <summary>
        /// Loads a complete shader program with the given name. 
        /// The procedure searches in SHADER_DIRECTORY for files with the given base name and common line endings. Therefore it automatically detects all shader programs of a routine. 
        /// For the shader to find the files correctly use the following file extensions: .vert, .frag, .geom, .tesc, .tese. 
        /// </summary>
        /// <param name="name">The base name of the shader routine</param>
        /// <returns>True if the shader was compiled and linked correctly or a previous (linked) version is available, false if the shader failed to compile and no previous version was stored. </returns>

        public static bool LoadShader(string name)
        {
            ERROR_MESSAGE = "\n";
            bool error = false;

            int programhandle = GL.CreateProgram();

            if (programhandle == 0)
            {
                Console.WriteLine("Failed to create Shader Program");
                return false;
            }
            
            // Create new Shader Object
            Shader shader = new Shader();
            shader.programHandle = programhandle;

            // Load Shader Programs
            string path = SHADER_FILE_DIRECTORY + "/" + name;
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            Console.WriteLine("------ Loading Shader: " + name + " ------");

            // Search the directory for Vertex Shader
            bool loaded = LoadShaderProgram(path + ".vert", ShaderType.VertexShader, out shader.vertexShader);
            string sign = (loaded) ? "+" : "-";
            Console.WriteLine(sign + " VERTEX SHADER");

            // Search the directory for Geometry Shader
            loaded = LoadShaderProgram(path + ".geom", ShaderType.GeometryShader, out shader.geometryShader);
            sign = (loaded) ? "+" : "-";
            Console.WriteLine(sign + " GEOMETRY SHADER");

            // Search the directory for Tesselation Control Shader
            loaded = LoadShaderProgram(path + ".tesc", ShaderType.TessControlShader, out shader.controlShader);
            sign = (loaded) ? "+" : "-";
            Console.WriteLine(sign + " TESSELATION CONTROL SHADER");

            // Search the directory for Tesselation Evaluation Shader
            loaded = LoadShaderProgram(path + ".tese", ShaderType.TessEvaluationShader, out shader.evaluationShader);
            sign = (loaded) ? "+" : "-";
            Console.WriteLine(sign + " TESSELATION EVALUATION SHADER");

            // Search the directory for Fragment Shader
            loaded = LoadShaderProgram(path + ".frag", ShaderType.FragmentShader, out shader.fragmentShader);
            sign = (loaded) ? "+" : "-";
            Console.WriteLine(sign + " FRAGMENT SHADER");


            // Only link shader if compilation successful
            if (!error)
                error = !shader.link();

            // Return if linking failed
            if (error)
            {
                Console.Write(ERROR_MESSAGE);
            }

            // At this point we have a valid shader
            // Search for previous instance and replace if neccessary

            if (error && Shaders.ContainsKey(path))
            {
                Console.WriteLine("Failed to load " + name + " Shader! Falling back to previous Shader...");
                return true;
            }
            if (error && !Shaders.ContainsKey(path))
            {
                Console.WriteLine("Failed to load " + name + " Shader! Exiting Program...");
                return false;
            }

            // New Shader compiled successfully
            Console.WriteLine(name + " Shader successfully linked!");

            // Put Shader into Map
            Shaders[name] = shader;

            return true;
        }



        /// <summary>
        /// Helper function to load a shaderprogramm from the file and stores the handle.
        /// </summary>
        /// <param name="file">the file of the shader program</param>
        /// <param name="shaderType">The type of the shader</param>
        /// <param name="handle">the handle to store the program</param>
        /// <returns>True if the the shader was loaded</returns>
        private static bool LoadShaderProgram(string path, ShaderType shaderType, out int handle)
        {
            string code = "";
            try
            {
                code = System.IO.File.ReadAllText(path);
            }
            catch (System.Exception ex)
            {
                code = "";
            }
            if (code == "")
            {
                handle = 0;
                return false;
            }

            handle = GL.CreateShader(shaderType);

            if (handle == 0)
            {
                return false;
            }

            GL.ShaderSource(handle, code);
            GL.CompileShader(handle);

            int compiled;
            GL.GetShader(handle, ShaderParameter.CompileStatus, out compiled);

            if (compiled == 0 || !GL.IsShader(handle))
            {
                string info = "";
                int logSize;
                GL.GetShader(handle, ShaderParameter.InfoLogLength, out logSize);
                GL.GetShaderInfoLog(handle, out info);

                ERROR_MESSAGE += path +": "+ info + "\n";
                return false;
            }
            return true;
        }


        /// <summary>
        /// Finds the shader for the given key. 
        /// </summary>
        /// <param name="key">The name of the shader</param>
        /// <returns>Returns the shader object if found, otherwhise null.</returns>
        public static Shader Find(string key)
        {
            if (Shaders.ContainsKey(key))
                return Shaders[key];

            if (!LoadShader(key))
                throw new Exception("Shader not available" + key);

            return Shaders[key];
        }


        public static bool InitializeShaders()
        {
            Console.WriteLine("Loading Shaders!");

            if (!LoadShader("skybox")) return false;          
           // if (!LoadShader("simpleLighting")) return false;
            if (!LoadShader("deferredGeometryPass")) return false;
            if (!LoadShader("deferredLightingPass")) return false;
            if (!LoadShader("textureToDepth")) return false;
            if (!LoadShader("shadowMap")) return false;
            if (!LoadShader("uniformColor")) return false;
            return true;
        }
    }
}