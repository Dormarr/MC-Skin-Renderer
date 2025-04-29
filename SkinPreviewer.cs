using MC_Skin_Aseprite_Previewer;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;

class SkinPreviewer : GameWindow
{
    private int textureID;
    private string skinPath = "D:/Ryan/DesktopSH/Misc/Minecraft_Skins/skin.png"; // Change this!
    private string triggerFile = "preview_trigger.txt";
    private DateTime lastFileCheck = DateTime.MinValue;

    private OrbitalCamera camera = new OrbitalCamera();
    private MouseState prevMouse;
    private float AspectRatio;

    public SkinPreviewer(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws)
    {
        VSync = VSyncMode.On;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.Enable(EnableCap.DepthTest);
        GL.ClearColor(Color4.CornflowerBlue);
        GL.DepthFunc(DepthFunction.Less);
        GL.Disable(EnableCap.CullFace);
        GL.Enable(EnableCap.Texture2D);


        textureID = LoadTexture(skinPath);
        Console.WriteLine($"Loading texture from: {skinPath}");

        if (textureID == 0 || textureID == -1)
            Console.WriteLine("Failed to load texture!");
        else
            Console.WriteLine($"Texture loaded successfully with ID: {textureID}");

        
        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();

        AspectRatio = Size.X / (float)Size.Y;

        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            AspectRatio,
            0.1f,
            100f
        );
        GL.MultMatrix(ref projection);

        GL.BindTexture(TextureTarget.Texture2D, textureID);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();

    }



    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        // Check if the preview trigger file has been updated
        if (File.Exists(triggerFile))
        {
            DateTime lastModified = File.GetLastWriteTime(triggerFile);
            if (lastModified > lastFileCheck)
            {
                lastFileCheck = lastModified;
                Console.WriteLine("Skin update detected. Reloading texture...");
                ReloadTexture();
            }
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.ClearColor(0.4f, 0.8f, 1.0f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.BindTexture(TextureTarget.Texture2D, textureID);


        // Setup the model-view matrix
        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();

        MouseState mouse = MouseState.GetSnapshot();

        

        camera.HandleInput(mouse, prevMouse, MouseState.ScrollDelta, AspectRatio);
        prevMouse = mouse;

        Matrix4 view = camera.GetViewMatrix();
        GL.LoadMatrix(ref view);


        // Draw the player model (each part is a cube)
        //DrawCubeSolid(0f, 0f, 0f, 2f, 2f, 2f, Color.Red); // Test

        //DrawCube(0f, 12f, -2f, 8f, 8f, 8f, 8, 8, 8, 8, name: "Head"); // Head

        ModelFace[] faceOrder = new ModelFace[]
        {
            ModelFace.Head_Front,
            ModelFace.Head_Back,
            ModelFace.Head_Left,
            ModelFace.Head_Right,
            ModelFace.Head_Top,
            ModelFace.Head_Bottom
        };

        UVMaps uvMaps = new UVMaps();

        GL.Begin(PrimitiveType.Quads);
        DrawCuboid(new Vector3(4, 16, 2), 8, 8, 8, faceOrder, uvMaps, 64, 64); // Head
        DrawCube(0f, 0f, 0f, 8f, 12f, 4f, 20, 20, 8, 12, name: "Body"); // Body
        /*
        DrawCube(-1f, -0.5f, 0, 4f, 12f, 4f, 44, 20, 4, 12, name: "Left Arm"); // Left Arm
        DrawCube(0.5f, -0.5f, 0, 4f, 12f, 4f, 44, 20, 4, 12, name: "Right Arm"); // Right Arm
        DrawCube(-0.3f, -1.5f, 0, 4f, 12f, 4f, 4, 20, 4, 12, name: "Left Leg"); // Left Leg
        DrawCube(0.3f, -1.5f, 0, 4f, 12f, 4f, 4, 20, 4, 12, name: "Right Leg"); // Right Leg
        
        
        GL.Begin(PrimitiveType.Triangles);
        GL.Color3(Color.Red);
        GL.Vertex3(-0.5f, -0.5f, -2f);
        GL.Color3(Color.Green);
        GL.Vertex3(0.5f, -0.5f, -2f);
        GL.Color3(Color.Blue);
        GL.Vertex3(0, 0.5f, -2f);
        */
        
        
        GL.End();
        SwapBuffers();

        ErrorCode err = GL.GetError();
        if (err != ErrorCode.NoError)
            Console.WriteLine("GL Error: " + err);

    }


    private int LoadTexture(string path)
    {
        if (!File.Exists(path)) return 0;

        int id = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, id);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

        Bitmap bitmap = new Bitmap(path);
        BitmapData data = bitmap.LockBits(
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        bitmap.UnlockBits(data);

        //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        return id;
    }

    private void ReloadTexture()
    {
        if (File.Exists(skinPath))
        {
            Bitmap bitmap = new Bitmap(skinPath);
            BitmapData data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bitmap.Width, bitmap.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);
            bitmap.Dispose();

            Console.WriteLine("Texture updated without recreation.");
        }
    }


    static void Main()
    {
        GameWindowSettings gws = GameWindowSettings.Default;
        NativeWindowSettings nws = new NativeWindowSettings()
        {
            Size = new Vector2i(800, 600),
            Title = "Minecraft Skin Previewer for Aseprite",
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Compatability,
            Flags = ContextFlags.ForwardCompatible,
            WindowBorder = WindowBorder.Resizable
        };

        using (var window = new SkinPreviewer(gws, nws))
        {
            window.Run();
        }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);

        AspectRatio = Size.X / (float)Size.Y;

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadIdentity();

        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            AspectRatio,
            0.1f,
            100f
        );
        GL.MultMatrix(ref projection);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.LoadIdentity();
    }

    #region Drawing

    void DrawCube(float x, float y, float z, float width, float height, float depth, float texX, float texY, float texWidth, float texHeight, float textureSize = 64f, string name = "Unnamed Cube")
    {

        Console.WriteLine($"Drawing {name}.");

        float x1 = x, x2 = x + width;
        float y1 = y, y2 = y + height;
        float z1 = z, z2 = z + depth;

        float u1 = texX / textureSize, u2 = (texX + texWidth) / textureSize;
        float v1 = texY / textureSize, v2 = (texY + texHeight) / textureSize;

        GL.Begin(PrimitiveType.Quads);

        // Front Face
        GL.TexCoord2(u1, v1); GL.Vertex3(x1, y1, z2);
        GL.TexCoord2(u2, v1); GL.Vertex3(x2, y1, z2);
        GL.TexCoord2(u2, v2); GL.Vertex3(x2, y2, z2);
        GL.TexCoord2(u1, v2); GL.Vertex3(x1, y2, z2);

        // Back Face
        GL.TexCoord2(u2, v1); GL.Vertex3(x1, y1, z1);
        GL.TexCoord2(u1, v1); GL.Vertex3(x2, y1, z1);
        GL.TexCoord2(u1, v2); GL.Vertex3(x2, y2, z1);
        GL.TexCoord2(u2, v2); GL.Vertex3(x1, y2, z1);

        // Top Face
        GL.TexCoord2(u1, v1); GL.Vertex3(x1, y2, z1);
        GL.TexCoord2(u2, v1); GL.Vertex3(x2, y2, z1);
        GL.TexCoord2(u2, v2); GL.Vertex3(x2, y2, z2);
        GL.TexCoord2(u1, v2); GL.Vertex3(x1, y2, z2);

        // Bottom Face
        GL.TexCoord2(u1, v1); GL.Vertex3(x1, y1, z1);
        GL.TexCoord2(u2, v1); GL.Vertex3(x2, y1, z1);
        GL.TexCoord2(u2, v2); GL.Vertex3(x2, y1, z2);
        GL.TexCoord2(u1, v2); GL.Vertex3(x1, y1, z2);

        // Left Face
        GL.TexCoord2(u1, v1); GL.Vertex3(x1, y1, z1);
        GL.TexCoord2(u2, v1); GL.Vertex3(x1, y1, z2);
        GL.TexCoord2(u2, v2); GL.Vertex3(x1, y2, z2);
        GL.TexCoord2(u1, v2); GL.Vertex3(x1, y2, z1);

        // Right Face
        GL.TexCoord2(u1, v1); GL.Vertex3(x2, y1, z1);
        GL.TexCoord2(u2, v1); GL.Vertex3(x2, y1, z2);
        GL.TexCoord2(u2, v2); GL.Vertex3(x2, y2, z2);
        GL.TexCoord2(u1, v2); GL.Vertex3(x2, y2, z1);

        GL.End();
    }

    void DrawFace(ModelFace face, UVMaps uvMaps, float textureWidth, float textureHeight)
    {
        Vector2[] uvs = uvMaps.MapDict[face].GetNormalisedUVs(textureWidth, textureHeight);
    }

    void DrawCuboid(Vector3 position, float width, float height, float depth, ModelFace[] faceOrder, UVMaps uvMaps, float textureWidth, float textureHeight)
    {
        float hw = width / 2f;
        float hh = height / 2f;
        float hd = depth / 2f;

        Vector3[][] faceVertices = new Vector3[][]
        {
            // Front - Counter-Clockwise
            new Vector3[] {
                new Vector3(-hw, -hh,  hd),
                new Vector3( hw, -hh,  hd),
                new Vector3( hw,  hh,  hd),
                new Vector3(-hw,  hh,  hd)
            },
            // Back
            new Vector3[] {
                new Vector3( hw, -hh, -hd),
                new Vector3(-hw, -hh, -hd),
                new Vector3(-hw,  hh, -hd),
                new Vector3( hw,  hh, -hd)
            },
            // Left
            new Vector3[] {
                new Vector3(-hw, -hh, -hd),
                new Vector3(-hw, -hh,  hd),
                new Vector3(-hw,  hh,  hd),
                new Vector3(-hw,  hh, -hd)
            },
            // Right
            new Vector3[] {
                new Vector3( hw, -hh,  hd),
                new Vector3( hw, -hh, -hd),
                new Vector3( hw,  hh, -hd),
                new Vector3( hw,  hh,  hd)
            },
            // Top
            new Vector3[] {
                new Vector3(-hw,  hh,  hd),
                new Vector3( hw,  hh,  hd),
                new Vector3( hw,  hh, -hd),
                new Vector3(-hw,  hh, -hd)
            },
            // Bottom
            new Vector3[] {
                new Vector3(-hw, -hh, -hd),
                new Vector3( hw, -hh, -hd),
                new Vector3( hw, -hh,  hd),
                new Vector3(-hw, -hh,  hd)
            }
        };

        for(int i = 0; i < 6; i++)
        {
            UV faceUV = uvMaps.MapDict[faceOrder[i]];
            Vector2[] uvs = faceUV.GetNormalisedUVs(textureWidth, textureHeight);
            Vector3[] verts = faceVertices[i];

            AddQuad(
                position + verts[0],
                position + verts[1],
                position + verts[2],
                position + verts[3],
                uvs
            );
        }
    }

    void AddQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector2[] uvs)
    {
        // Triangle 1
        AddVertex(v0, uvs[0]);
        AddVertex(v1, uvs[1]);
        AddVertex(v2, uvs[2]);

        // Triangle 2
        AddVertex(v2, uvs[2]);
        AddVertex(v3, uvs[3]);
        AddVertex(v0, uvs[0]);
    }

    void AddVertex(Vector3 pos, Vector2 uv)
    {       
        GL.TexCoord2(uv.X, uv.Y);   GL.Vertex3(pos.X, pos.Y, pos.Z);
    }



    #endregion
}
