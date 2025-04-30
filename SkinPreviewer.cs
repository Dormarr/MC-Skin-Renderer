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
        GL.Enable(EnableCap.CullFace);
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

        ModelFace[] headOrder = new ModelFace[]
        {
            ModelFace.Head_Front,
            ModelFace.Head_Back,
            ModelFace.Head_Left,
            ModelFace.Head_Right,
            ModelFace.Head_Top,
            ModelFace.Head_Bottom
        };

        ModelFace[] bodyOrder = new ModelFace[]
        {
            ModelFace.Body_Front,
            ModelFace.Body_Back,
            ModelFace.Body_Left,
            ModelFace.Body_Right,
            ModelFace.Body_Top,
            ModelFace.Body_Bottom
        };

        ModelFace[] armLeftOrder = new ModelFace[]
        {
            ModelFace.ArmLeft_Front,
            ModelFace.ArmLeft_Back,
            ModelFace.ArmLeft_Left,
            ModelFace.ArmLeft_Right,
            ModelFace.ArmLeft_Top,
            ModelFace.ArmLeft_Bottom
        };

        ModelFace[] armRightOrder = new ModelFace[]
        {
            ModelFace.ArmRight_Front,
            ModelFace.ArmRight_Back,
            ModelFace.ArmRight_Left,
            ModelFace.ArmRight_Right,
            ModelFace.ArmRight_Top,
            ModelFace.ArmRight_Bottom
        };

        ModelFace[] legLeftOrder = new ModelFace[]
        {
            ModelFace.LegLeft_Front,
            ModelFace.LegLeft_Back,
            ModelFace.LegLeft_Left,
            ModelFace.LegLeft_Right,
            ModelFace.LegLeft_Top,
            ModelFace.LegLeft_Bottom
        };

        ModelFace[] legRightOrder = new ModelFace[]
        {
            ModelFace.LegRight_Front,
            ModelFace.LegRight_Back,
            ModelFace.LegRight_Left,
            ModelFace.LegRight_Right,
            ModelFace.LegRight_Top,
            ModelFace.LegRight_Bottom
        };

        UVMaps uvMaps = new UVMaps();


        DrawCuboid(new Vector3(6, 16, 2), 8, 8, 8, headOrder, uvMaps, name: "Head"); // Head
        DrawCuboid(new Vector3(6, 6, 2), 8, 12, 4, bodyOrder, uvMaps, name: "Body"); // Body
        DrawCuboid(new Vector3(12, 6, 2), 4, 12, 4, armLeftOrder, uvMaps, name: "ArmLeft"); // Arm Left
        DrawCuboid(new Vector3(0, 6, 2), 4, 12, 4, armRightOrder, uvMaps, name: "ArmRight"); // Arm Right
        DrawCuboid(new Vector3(8, -6, 2), 4, 12, 4, legLeftOrder, uvMaps, name: "LegLeft"); // Leg Left
        DrawCuboid(new Vector3(4, -6, 2), 4, 12, 4, legRightOrder, uvMaps, name: "LegRight"); // Leg Right


        SwapBuffers();
    }

    void CheckGLError(string context)
    {
        ErrorCode err;
        while((err = GL.GetError()) != ErrorCode.NoError)
        {
            Console.WriteLine($"GL Error [{context}]: {err}");
        }
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

    void DrawCuboid(Vector3 position, float width, float height, float depth, ModelFace[] faceOrder, UVMaps uvMaps, float textureWidth = 64, float textureHeight = 64, string name = "Unnamed Cuboid")
    {
        float hw = width / 2f;
        float hh = height / 2f;
        float hd = depth / 2f;

        Vector3[][] faceVertices = new Vector3[][]
        {
            // Front (+Z)
            new Vector3[] {
                new Vector3(-hw,  hh,  hd),  // Top Left
                new Vector3(-hw, -hh,  hd), // Bottom Left
                new Vector3( hw, -hh,  hd), // Bottom Right
                new Vector3( hw,  hh,  hd), // Top Right
            },
            // Back (-Z)
            new Vector3[] {
                new Vector3( hw, -hh, -hd), // Bottom Right
                new Vector3(-hw, -hh, -hd), // Bottom Left
                new Vector3(-hw,  hh, -hd), // Top Left
                new Vector3( hw,  hh, -hd)  // Top Right
            },
            // Left (-X)
            new Vector3[] {
                new Vector3(-hw, -hh, -hd), // Bottom Back
                new Vector3(-hw, -hh,  hd), // Bottom Front
                new Vector3(-hw,  hh,  hd), // Top Front
                new Vector3(-hw,  hh, -hd)  // Top Back
            },
            // Right (+X)
            new Vector3[] {
                new Vector3( hw, -hh,  hd), // Bottom Front
                new Vector3( hw, -hh, -hd), // Bottom Back
                new Vector3( hw,  hh, -hd), // Top Back
                new Vector3( hw,  hh,  hd)  // Top Front
            },
            // Top (+Y)
            new Vector3[] {
                new Vector3(-hw,  hh,  hd), // Front Left
                new Vector3( hw,  hh,  hd), // Front Right
                new Vector3( hw,  hh, -hd), // Back Right
                new Vector3(-hw,  hh, -hd)  // Back Left
            },
            // Bottom (-Y)
            new Vector3[] {
                new Vector3(-hw, -hh, -hd), // Back Left
                new Vector3( hw, -hh, -hd), // Back Right
                new Vector3( hw, -hh,  hd), // Front Right
                new Vector3(-hw, -hh,  hd)  // Front Left
            }
        };

        for (int i = 0; i < faceOrder.Length; i++)
        {
            UV faceUV = uvMaps.MapDict[faceOrder[i]];
            Vector2[] uvs = faceUV.GetNormalisedUVs(textureWidth, textureHeight);

            switch (faceOrder[i])
            {
                case ModelFace.Head_Right:
                    uvs = RotateUVs(uvs, 2);
                    break;
                case ModelFace.Head_Bottom:
                    uvs = RotateUVs(uvs, 3);
                    break;
                case ModelFace.ArmLeft_Right:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.ArmRight_Left:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.ArmLeft_Back:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.ArmRight_Back:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.ArmLeft_Left:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.ArmRight_Right:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.LegRight_Right:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.LegRight_Left:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.LegRight_Back:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.LegLeft_Right:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.LegLeft_Left:
                    uvs = RotateUVs(uvs, 1);
                    break;
                case ModelFace.LegLeft_Back:
                    uvs = RotateUVs(uvs, 1);
                    break;
            }

            Vector3[] verts = faceVertices[i];

            AddQuad(
                position + verts[0],
                position + verts[1],
                position + verts[2],
                position + verts[3],
                uvs
            );
        }

        Console.WriteLine($"Drawing {name}.");
    }

    Vector2[] RotateUVs(Vector2[] uvs, int rotationSteps)
    {
        // rotationSteps: 0 = 0°, 1 = 90° CW, 2 = 180°, 3 = 270° CW
        Vector2[] rotated = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            rotated[i] = uvs[(i + rotationSteps) % 4];
        }
        return rotated;
    }


    void AddQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector2[] uvs)
    {
        GL.Begin(PrimitiveType.Triangles);
        CheckGLError("GL.Begin Quads");
        // Triangle 1
        AddVertex(v0, uvs[0]);
        AddVertex(v1, uvs[1]);
        AddVertex(v2, uvs[2]);

        // Triangle 2
        AddVertex(v0, uvs[0]);
        AddVertex(v2, uvs[2]);
        AddVertex(v3, uvs[3]);

        GL.End();
    }

    void AddVertex(Vector3 pos, Vector2 uv)
    {
        GL.TexCoord2(uv.X, uv.Y);
        GL.Vertex3(pos.X, pos.Y, pos.Z);
    }



    #endregion
}
