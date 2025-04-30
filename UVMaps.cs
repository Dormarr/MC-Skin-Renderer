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
        // Do the rest later.
        Body_Front,
        Body_Back,
        Body_Left,
        Body_Right,
        Body_Top,
        Body_Bottom,
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
            MapDict.Add(ModelFace.Head_Front,   new UV(8, 8, 8, 8));
            MapDict.Add(ModelFace.Head_Back,    new UV(24, 8, 8, 8));
            MapDict.Add(ModelFace.Head_Left,    new UV(16, 8, 8, 8));
            MapDict.Add(ModelFace.Head_Right,   new UV(0, 8, 8, 8));
            MapDict.Add(ModelFace.Head_Top,     new UV(8, 0, 8, 8));
            MapDict.Add(ModelFace.Head_Bottom,  new UV(16, 0, 8, 8));

            MapDict.Add(ModelFace.Body_Front, new UV(20, 20, 8, 12));
            MapDict.Add(ModelFace.Body_Back, new UV(32, 20, 8, 12));
            MapDict.Add(ModelFace.Body_Left, new UV(16, 20, 4, 12));
            MapDict.Add(ModelFace.Body_Right, new UV(28, 20, 4, 12));
            MapDict.Add(ModelFace.Body_Top, new UV(20, 16, 8, 4));
            MapDict.Add(ModelFace.Body_Bottom, new UV(28, 16, 8, 4));
        }
        
    }
}
