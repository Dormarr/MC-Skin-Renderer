using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace MC_Skin_Aseprite_Previewer
{
    enum ModelFace
    {
        Head_Front,
        Head_Back,
        Head_Left,
        Head_Right,
        Head_Top,
        Head_Bottom,
        
        Body_Front,
        Body_Back,
        Body_Left,
        Body_Right,
        Body_Top,
        Body_Bottom,

        ArmLeft_Front,
        ArmLeft_Back,
        ArmLeft_Left,
        ArmLeft_Right,
        ArmLeft_Top,
        ArmLeft_Bottom,

        ArmRight_Front,
        ArmRight_Back,
        ArmRight_Left,
        ArmRight_Right,
        ArmRight_Top,
        ArmRight_Bottom,

        LegLeft_Front,
        LegLeft_Back,
        LegLeft_Left,
        LegLeft_Right,
        LegLeft_Top,
        LegLeft_Bottom,

        LegRight_Front,
        LegRight_Back,
        LegRight_Left,
        LegRight_Right,
        LegRight_Top,
        LegRight_Bottom,

        Outer_Head_Front,
        Outer_Head_Back,
        Outer_Head_Left,
        Outer_Head_Right,
        Outer_Head_Top,
        Outer_Head_Bottom,
        
        Outer_Body_Front,
        Outer_Body_Back,
        Outer_Body_Left,
        Outer_Body_Right,
        Outer_Body_Top,
        Outer_Body_Bottom,
        
        Outer_ArmLeft_Front,
        Outer_ArmLeft_Back,
        Outer_ArmLeft_Left,
        Outer_ArmLeft_Right,
        Outer_ArmLeft_Top,
        Outer_ArmLeft_Bottom,
        
        Outer_ArmRight_Front,
        Outer_ArmRight_Back,
        Outer_ArmRight_Left,
        Outer_ArmRight_Right,
        Outer_ArmRight_Top,
        Outer_ArmRight_Bottom,
        
        Outer_LegLeft_Front,
        Outer_LegLeft_Back,
        Outer_LegLeft_Left,
        Outer_LegLeft_Right,
        Outer_LegLeft_Top,
        Outer_LegLeft_Bottom,
        
        Outer_LegRight_Front,
        Outer_LegRight_Back,
        Outer_LegRight_Left,
        Outer_LegRight_Right,
        Outer_LegRight_Top,
        Outer_LegRight_Bottom,
    }

    public class UV
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float W { get; private set; }
        public float H { get; private set; }

        public UV(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public Vector2[] GetNormalisedUVs(float textureWidth, float textureHeight)
        {
            float u1 = X / textureWidth;
            float v1 = Y / textureHeight;
            float u2 = (X + W) / textureWidth;
            float v2 = (Y + H) / textureHeight;

            // Note: Flip vertically if needed
            return new Vector2[]
            {
                new Vector2(u1, v1), // Top Left
                new Vector2(u1, v2), // Bottom Left
                new Vector2(u2, v2), // Bottom Right
                new Vector2(u2, v1), // Top Right
            };

        }
    }

    internal class UVMaps
    {
        public Dictionary<ModelFace, UV> MapDict { get; } = new Dictionary<ModelFace, UV>();

        public UVMaps()
        {
            // Head
            MapDict.Add(ModelFace.Head_Front,   new UV(8, 8, 8, 8));
            MapDict.Add(ModelFace.Head_Back,    new UV(24, 8, 8, 8));
            MapDict.Add(ModelFace.Head_Right,    new UV(16, 8, 8, 8));
            MapDict.Add(ModelFace.Head_Left,   new UV(0, 8, 8, 8));
            MapDict.Add(ModelFace.Head_Top,     new UV(8, 0, 8, 8));
            MapDict.Add(ModelFace.Head_Bottom,  new UV(16, 0, 8, 8));

            // Body
            MapDict.Add(ModelFace.Body_Front, new UV(20, 20, 8, 12));
            MapDict.Add(ModelFace.Body_Back, new UV(32, 20, 8, 12));
            MapDict.Add(ModelFace.Body_Left, new UV(16, 20, 4, 12));
            MapDict.Add(ModelFace.Body_Right, new UV(28, 20, 4, 12));
            MapDict.Add(ModelFace.Body_Top, new UV(20, 16, 8, 4));
            MapDict.Add(ModelFace.Body_Bottom, new UV(28, 16, 8, 4));

            // Right Leg
            MapDict.Add(ModelFace.LegRight_Front, new UV(4, 20, 4, 12));
            MapDict.Add(ModelFace.LegRight_Back, new UV(12, 20, 4, 12));
            MapDict.Add(ModelFace.LegRight_Left, new UV(0, 20, 4, 12));
            MapDict.Add(ModelFace.LegRight_Right, new UV(8, 20, 4, 12));
            MapDict.Add(ModelFace.LegRight_Top, new UV(4, 16, 4, 4));
            MapDict.Add(ModelFace.LegRight_Bottom, new UV(8, 16, 4, 4));

            // Left Leg
            MapDict.Add(ModelFace.LegLeft_Front, new UV(20, 52, 4, 12));
            MapDict.Add(ModelFace.LegLeft_Back, new UV(28, 52, 4, 12));
            MapDict.Add(ModelFace.LegLeft_Left, new UV(16, 52, 4, 12));
            MapDict.Add(ModelFace.LegLeft_Right, new UV(24, 52, 4, 12));
            MapDict.Add(ModelFace.LegLeft_Top, new UV(20, 48, 4, 4));
            MapDict.Add(ModelFace.LegLeft_Bottom, new UV(24, 48, 4, 4));

            // Right Arm
            MapDict.Add(ModelFace.ArmRight_Front, new UV(44, 20, 4, 12));
            MapDict.Add(ModelFace.ArmRight_Back, new UV(52, 20, 4, 12));
            MapDict.Add(ModelFace.ArmRight_Right, new UV(48, 20, 4, 12));
            MapDict.Add(ModelFace.ArmRight_Left, new UV(40, 20, 4, 12));
            MapDict.Add(ModelFace.ArmRight_Top, new UV(44, 16, 4, 4));
            MapDict.Add(ModelFace.ArmRight_Bottom, new UV(48, 16, 4, 4));

            // Left Arm
            MapDict.Add(ModelFace.ArmLeft_Front, new UV(36, 52, 4, 12));
            MapDict.Add(ModelFace.ArmLeft_Back, new UV(44, 52, 4, 12));
            MapDict.Add(ModelFace.ArmLeft_Right, new UV(40, 52, 4, 12));
            MapDict.Add(ModelFace.ArmLeft_Left, new UV(32, 52, 4, 12));
            MapDict.Add(ModelFace.ArmLeft_Top, new UV(36, 48, 4, 4));
            MapDict.Add(ModelFace.ArmLeft_Bottom, new UV(40, 48, 4, 4));

            // Outer Head
            MapDict.Add(ModelFace.Outer_Head_Front, new UV(40, 8, 8, 8));
            MapDict.Add(ModelFace.Outer_Head_Back, new UV(56, 8, 8, 8));
            MapDict.Add(ModelFace.Outer_Head_Right, new UV(48, 8, 8, 8));
            MapDict.Add(ModelFace.Outer_Head_Left, new UV(32, 8, 8, 8));
            MapDict.Add(ModelFace.Outer_Head_Top, new UV(40, 0, 8, 8));
            MapDict.Add(ModelFace.Outer_Head_Bottom, new UV(48, 0, 8, 8));

            // Outer Body Layer
            MapDict.Add(ModelFace.Outer_Body_Front, new UV(20, 36, 8, 12));
            MapDict.Add(ModelFace.Outer_Body_Back, new UV(32, 36, 8, 12));
            MapDict.Add(ModelFace.Outer_Body_Right, new UV(28, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_Body_Left, new UV(16, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_Body_Top, new UV(20, 32, 8, 4));
            MapDict.Add(ModelFace.Outer_Body_Bottom, new UV(28, 32, 8, 4));

            // Outer Right Arm
            MapDict.Add(ModelFace.Outer_ArmRight_Front, new UV(44, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_ArmRight_Back, new UV(52, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_ArmRight_Right, new UV(48, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_ArmRight_Left, new UV(40, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_ArmRight_Top, new UV(44, 32, 4, 4));
            MapDict.Add(ModelFace.Outer_ArmRight_Bottom, new UV(48, 32, 4, 4));

            // Outer Left Arm
            MapDict.Add(ModelFace.Outer_ArmLeft_Front, new UV(52, 52, 4, 12));
            MapDict.Add(ModelFace.Outer_ArmLeft_Back, new UV(60, 52, 4, 12));
            MapDict.Add(ModelFace.Outer_ArmLeft_Right, new UV(56, 52, 4, 12));
            MapDict.Add(ModelFace.Outer_ArmLeft_Left, new UV(48, 52, 4, 12));
            MapDict.Add(ModelFace.Outer_ArmLeft_Top, new UV(52, 48, 4, 4));
            MapDict.Add(ModelFace.Outer_ArmLeft_Bottom, new UV(56, 48, 4, 4));


            // Outer Right Leg
            MapDict.Add(ModelFace.Outer_LegRight_Front, new UV(4, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_LegRight_Back, new UV(12, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_LegRight_Right, new UV(8, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_LegRight_Left, new UV(0, 36, 4, 12));
            MapDict.Add(ModelFace.Outer_LegRight_Top, new UV(4, 32, 4, 4));
            MapDict.Add(ModelFace.Outer_LegRight_Bottom, new UV(8, 32, 4, 4));

            // Outer Left Leg
            MapDict.Add(ModelFace.Outer_LegLeft_Front, new UV(4, 52, 4, 12));
            MapDict.Add(ModelFace.Outer_LegLeft_Back, new UV(12, 52, 4, 12));
            MapDict.Add(ModelFace.Outer_LegLeft_Right, new UV(8, 52, 4, 12));
            MapDict.Add(ModelFace.Outer_LegLeft_Left, new UV(0, 52, 4, 12));
            MapDict.Add(ModelFace.Outer_LegLeft_Top, new UV(4, 48, 4, 4));
            MapDict.Add(ModelFace.Outer_LegLeft_Bottom, new UV(8, 48, 4, 4));
        }

    }
}
