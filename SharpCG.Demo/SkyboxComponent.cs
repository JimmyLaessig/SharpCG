using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;


using SharpCG;
using SharpCG.Base;
using SharpCG.Base.Rendering;
using SharpCG.Base.Scenegraph;
using GlmSharp;


public class SkyboxComponent : Component, Renderable
{

    int cubeMapTexture;
    private Mesh mesh;
    public SkyboxComponent()
    {
        // Create Cube Map Texture
        var x = System.IO.Directory.GetCurrentDirectory();
        Bitmap posX = Bitmap.FromFile("Assets/skybox/skybox_left2048.png") as Bitmap;
        Bitmap negX = Bitmap.FromFile("Assets/skybox/skybox_right2048.png") as Bitmap;

        Bitmap posY = Bitmap.FromFile("Assets/skybox/skybox_top2048.png") as Bitmap;
        Bitmap negY = Bitmap.FromFile("Assets/skybox/skybox_bottom2048.png") as Bitmap;

        Bitmap posZ = Bitmap.FromFile("Assets/skybox/skybox_front2048.png") as Bitmap;
        Bitmap negZ = Bitmap.FromFile("Assets/skybox/skybox_back2048.png") as Bitmap;

        //posX.RotateFlip(RotateFlipType.RotateNoneFlipY);
        //negX.RotateFlip(RotateFlipType.RotateNoneFlipY);
        //posY.RotateFlip(RotateFlipType.RotateNoneFlipY);
        //negY.RotateFlip(RotateFlipType.RotateNoneFlipY);
        //posZ.RotateFlip(RotateFlipType.RotateNoneFlipY);
        //negZ.RotateFlip(RotateFlipType.RotateNoneFlipY);

        GL.GenTextures(1, out cubeMapTexture);
        GL.BindTexture(TextureTarget.TextureCubeMap, cubeMapTexture);

        BitmapData data;

        data = posX.LockBits(new Rectangle(0, 0, posX.Width, posX.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX, 0, PixelInternalFormat.Rgba, posX.Width, posX.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        posX.UnlockBits(data);

        data = negX.LockBits(new Rectangle(0, 0, negX.Width, negX.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TexImage2D(TextureTarget.TextureCubeMapNegativeX, 0, PixelInternalFormat.Rgba, negX.Width, negX.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        negX.UnlockBits(data);

        data = posY.LockBits(new Rectangle(0, 0, posY.Width, posY.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveY, 0, PixelInternalFormat.Rgba, posY.Width, posY.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        posY.UnlockBits(data);

        data = negY.LockBits(new Rectangle(0, 0, negY.Width, negY.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TexImage2D(TextureTarget.TextureCubeMapNegativeY, 0, PixelInternalFormat.Rgba, negY.Width, negY.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        negY.UnlockBits(data);

        data = posZ.LockBits(new Rectangle(0, 0, posZ.Width, posZ.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TexImage2D(TextureTarget.TextureCubeMapPositiveZ, 0, PixelInternalFormat.Rgba, posZ.Width, posZ.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        posZ.UnlockBits(data);

        data = negZ.LockBits(new Rectangle(0, 0, negZ.Width, negZ.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TexImage2D(TextureTarget.TextureCubeMapNegativeZ, 0, PixelInternalFormat.Rgba, negZ.Width, negZ.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        negZ.UnlockBits(data);

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        // Create Cube
        mesh = MeshExtensions.UnitCube;
    }


    public void Render()
    {
        Camera camera = Camera.Main;
       
        GL.Disable(EnableCap.CullFace);
        GL.DepthMask(false);

        Shader shader = Shader.Find("skybox");
        shader.bind();
        // Bind Cubemap Texture
        int location = GL.GetUniformLocation(shader.ProgramHandle, "cubeMap");
        GL.Uniform1(location, 0);
        GL.BindTexture(TextureTarget.TextureCubeMap, cubeMapTexture);

        mat4 W  = mat4.Translate(camera.Transform.Position);
        mat4 V  = camera.ViewMatrix;
        mat4 P  = camera.ProjectionMatrix;

        mat4 wvpMatrix = P * V * W;

        location = GL.GetUniformLocation(shader.ProgramHandle, "wvpMatrix");
        GL.UniformMatrix4(location, 1, false, wvpMatrix.Values1D);

        mesh.Bind();
        GL.DrawElements(BeginMode.Triangles, mesh.TriangleCount * 3, DrawElementsType.UnsignedInt, 0);


        shader.release();

        GL.Enable(EnableCap.CullFace);
        GL.DepthMask(true);

    }

    public override void Update(double deltaTime)
    {
       
    }


}

