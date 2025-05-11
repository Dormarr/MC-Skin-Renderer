using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace MC_Skin_Aseprite_Previewer
{
    enum ModelFace
    {
        [ModelFaceGroup("Head")]Head_Front,
        [ModelFaceGroup("Head")]Head_Back,
        [ModelFaceGroup("Head")]Head_Left,
        [ModelFaceGroup("Head")]Head_Right,
        [ModelFaceGroup("Head")]Head_Top,
        [ModelFaceGroup("Head")]Head_Bottom,

        [ModelFaceGroup("Body")]Body_Front,
        [ModelFaceGroup("Body")]Body_Back,
        [ModelFaceGroup("Body")]Body_Left,
        [ModelFaceGroup("Body")]Body_Right,
        [ModelFaceGroup("Body")]Body_Top,
        [ModelFaceGroup("Body")]Body_Bottom,

        [ModelFaceGroup("ArmLeft")]ArmLeft_Front,
        [ModelFaceGroup("ArmLeft")]ArmLeft_Back,
        [ModelFaceGroup("ArmLeft")]ArmLeft_Left,
        [ModelFaceGroup("ArmLeft")]ArmLeft_Right,
        [ModelFaceGroup("ArmLeft")]ArmLeft_Top,
        [ModelFaceGroup("ArmLeft")]ArmLeft_Bottom,

        [ModelFaceGroup("ArmRight")]ArmRight_Front,
        [ModelFaceGroup("ArmRight")]ArmRight_Back,
        [ModelFaceGroup("ArmRight")]ArmRight_Left,
        [ModelFaceGroup("ArmRight")]ArmRight_Right,
        [ModelFaceGroup("ArmRight")]ArmRight_Top,
        [ModelFaceGroup("ArmRight")]ArmRight_Bottom,

        [ModelFaceGroup("LegLeft")]LegLeft_Front,
        [ModelFaceGroup("LegLeft")]LegLeft_Back,
        [ModelFaceGroup("LegLeft")]LegLeft_Left,
        [ModelFaceGroup("LegLeft")]LegLeft_Right,
        [ModelFaceGroup("LegLeft")]LegLeft_Top,
        [ModelFaceGroup("LegLeft")]LegLeft_Bottom,

        [ModelFaceGroup("LegRight")]LegRight_Front,
        [ModelFaceGroup("LegRight")]LegRight_Back,
        [ModelFaceGroup("LegRight")]LegRight_Left,
        [ModelFaceGroup("LegRight")]LegRight_Right,
        [ModelFaceGroup("LegRight")]LegRight_Top,
        [ModelFaceGroup("LegRight")]LegRight_Bottom,

        [ModelFaceGroup("OuterHead")]Outer_Head_Front,
        [ModelFaceGroup("OuterHead")]Outer_Head_Back,
        [ModelFaceGroup("OuterHead")]Outer_Head_Left,
        [ModelFaceGroup("OuterHead")]Outer_Head_Right,
        [ModelFaceGroup("OuterHead")]Outer_Head_Top,
        [ModelFaceGroup("OuterHead")]Outer_Head_Bottom,
        
        [ModelFaceGroup("OuterBody")]Outer_Body_Front,
        [ModelFaceGroup("OuterBody")]Outer_Body_Back,
        [ModelFaceGroup("OuterBody")]Outer_Body_Left,
        [ModelFaceGroup("OuterBody")]Outer_Body_Right,
        [ModelFaceGroup("OuterBody")]Outer_Body_Top,
        [ModelFaceGroup("OuterBody")]Outer_Body_Bottom,
        
        [ModelFaceGroup("OuterArmLeft")]Outer_ArmLeft_Front,
        [ModelFaceGroup("OuterArmLeft")]Outer_ArmLeft_Back,
        [ModelFaceGroup("OuterArmLeft")]Outer_ArmLeft_Left,
        [ModelFaceGroup("OuterArmLeft")]Outer_ArmLeft_Right,
        [ModelFaceGroup("OuterArmLeft")]Outer_ArmLeft_Top,
        [ModelFaceGroup("OuterArmLeft")]Outer_ArmLeft_Bottom,
        
        [ModelFaceGroup("OuterArmRight")]Outer_ArmRight_Front,
        [ModelFaceGroup("OuterArmRight")]Outer_ArmRight_Back,
        [ModelFaceGroup("OuterArmRight")]Outer_ArmRight_Left,
        [ModelFaceGroup("OuterArmRight")]Outer_ArmRight_Right,
        [ModelFaceGroup("OuterArmRight")]Outer_ArmRight_Top,
        [ModelFaceGroup("OuterArmRight")]Outer_ArmRight_Bottom,
        
        [ModelFaceGroup("OuterLegLeft")]Outer_LegLeft_Front,
        [ModelFaceGroup("OuterLegLeft")]Outer_LegLeft_Back,
        [ModelFaceGroup("OuterLegLeft")]Outer_LegLeft_Left,
        [ModelFaceGroup("OuterLegLeft")]Outer_LegLeft_Right,
        [ModelFaceGroup("OuterLegLeft")]Outer_LegLeft_Top,
        [ModelFaceGroup("OuterLegLeft")]Outer_LegLeft_Bottom,

        [ModelFaceGroup("OuterLegRight")]Outer_LegRight_Front,
        [ModelFaceGroup("OuterLegRight")]Outer_LegRight_Back,
        [ModelFaceGroup("OuterLegRight")]Outer_LegRight_Left,
        [ModelFaceGroup("OuterLegRight")]Outer_LegRight_Right,
        [ModelFaceGroup("OuterLegRight")]Outer_LegRight_Top,
        [ModelFaceGroup("OuterLegRight")]Outer_LegRight_Bottom,
    }

    class ModelFaceGroupAttribute : Attribute
    {
        public string GroupName { get; }
        public ModelFaceGroupAttribute(string groupName) =>  GroupName = groupName;
    }

    static class ModelFaceUtil
    {
        public static ModelFace[] GetModelFacesByGroup(string group)
        {
            return typeof(ModelFace).GetFields()
               .Where(f => f.GetCustomAttribute<ModelFaceGroupAttribute>()?.GroupName == group)
               .Select(f => (ModelFace)f.GetValue(null))
               .ToArray();
        }
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
