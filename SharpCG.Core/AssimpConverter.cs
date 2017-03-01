using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using GlmSharp;


namespace SharpCG.Core
{
    public static class AssimpConverter
    {

        public static void Convert(Assimp.Mesh aiMesh, ref Geometry mesh)
        {
            uint[] indices = aiMesh.GetUnsignedIndices();
            int numIndices = indices.Length;
            int numVertices = aiMesh.VertexCount;

            float[] positions   = new float[numVertices * 3];
            float[] colors      = new float[numVertices * 4];
            float[] normals     = new float[numVertices * 3];
            float[] tangents    = new float[numVertices * 3];
            float[] bitangents  = new float[numVertices * 3];
            float[] texcoords0  = new float[numVertices * 2];

            // Read vertex information
            if (aiMesh.HasVertices)
            {
                int i = 0;
                foreach (var vertex in aiMesh.Vertices)
                {
                    positions[i++] = vertex.X;
                    positions[i++] = vertex.Y;
                    positions[i++] = vertex.Z;
                }
            }

            // Read normal information
            if (aiMesh.HasNormals)
            {
                Console.WriteLine("Loading Normals...");
                var i = 0;
                foreach (var normal in aiMesh.Normals)
                {
                    normals[i++] = normal.X;
                    normals[i++] = normal.Y;
                    normals[i++] = normal.Z;
                }
            }

            // Read Tangents and Bitangents
            if (aiMesh.HasTangentBasis)
            {
                Console.WriteLine("Loading Tangents and Bitangents...");
                int i = 0;
                foreach (var tangent in aiMesh.Tangents)
                {
                    tangents[i++] = tangent.X;
                    tangents[i++] = tangent.Y;
                    tangents[i++] = tangent.Z;
                }

                i = 0;
                foreach (var bitangent in aiMesh.BiTangents)
                {
                    bitangents[i++] = bitangent.X;
                    bitangents[i++] = bitangent.Y;
                    bitangents[i++] = bitangent.Z;
                }
            }

            // Read Texture coordinates           
            if (aiMesh.HasTextureCoords(0))
            {

                var i = 0;
                foreach (var uv in aiMesh.TextureCoordinateChannels[0])
                {
                    texcoords0[i++] = uv.X;
                    texcoords0[i++] = uv.Y;
                }
            }

            // Read color information               
            if (aiMesh.HasVertexColors(0))
            {

                int i = 0;
                foreach (var color in aiMesh.VertexColorChannels[0])
                {
                    colors[i + 0] = color.R;
                    colors[i + 1] = color.G;
                    colors[i + 2] = color.B;
                    colors[i + 3] = color.A;
                }
            }

            // Compose mesh structure
            mesh.Name = aiMesh.Name;
            mesh.Indices = ArrayBuffer<uint>.Create(BufferTarget.ElementArrayBuffer, indices);

            // ArrayBuffer.Create()
            mesh.SetAttribute(DefaultAttributeName.Position,    ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, positions, 3), 0);
            mesh.SetAttribute(DefaultAttributeName.Normal,      ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, normals, 3), 1);
            mesh.SetAttribute(DefaultAttributeName.Tangent,     ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, tangents, 3), 2);
            mesh.SetAttribute(DefaultAttributeName.Bitangent,   ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, bitangents, 3), 3);
            mesh.SetAttribute(DefaultAttributeName.Color,       ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, colors, 4), 4);
            mesh.SetAttribute(DefaultAttributeName.Texcoord0,   ArrayBuffer<float>.Create(BufferTarget.ArrayBuffer, texcoords0, 2), 5);

        }


        public static void Convert(Assimp.Material aiMaterial, string path, ref AssimpMaterial material)
        {
            
            if (aiMaterial.HasColorDiffuse)
            {
                material.DiffuseAmount = new dvec4(aiMaterial.ColorDiffuse.R, aiMaterial.ColorDiffuse.G, aiMaterial.ColorDiffuse.B, aiMaterial.ColorDiffuse.A);
            }
            if (aiMaterial.HasOpacity)
            {
                material.DiffuseAmount = new dvec4(material.DiffuseAmount.rgb, aiMaterial.Opacity);
            }
            if (aiMaterial.HasColorSpecular)
            {
                material.SpecularAmount = new dvec4(aiMaterial.ColorSpecular.R, aiMaterial.ColorSpecular.G, aiMaterial.ColorSpecular.B, 8) * 2.0f;
            }
            if (aiMaterial.HasShininess)
            {
                material.SpecularAmount = new dvec4(material.SpecularAmount.xyz, 1 + (aiMaterial.Shininess) * 5.10f);
            }
            if (aiMaterial.HasTextureDiffuse)
            {
                material.DiffuseMapTexture = Texture2D.Load(path + "/" + aiMaterial.TextureDiffuse.FilePath, true);
            }
            if (aiMaterial.HasTextureNormal)
            {
                material.NormalMapTexture = Texture2D.Load(path + "/" + aiMaterial.TextureNormal.FilePath, true);
            }
            if (aiMaterial.HasTextureHeight)
            {
                material.NormalMapTexture = Texture2D.Load(path + "/" + aiMaterial.TextureHeight.FilePath, true);
            }
            if (aiMaterial.HasTextureSpecular)
            {
                material.SpecularMapTexture = Texture2D.Load(path + "/" + aiMaterial.TextureSpecular.FilePath, true);
            }
            if (aiMaterial.HasTextureDisplacement)
            {
                material.DisplacementMapTexture = Texture2D.Load(path + "/" + aiMaterial.TextureDisplacement.FilePath, true);
            }
            if (aiMaterial.HasTextureHeight)
            {

            }
        }
    }
}
