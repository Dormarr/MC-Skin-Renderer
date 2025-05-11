using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace MC_Skin_Aseprite_Previewer
{
    enum RenderState
    {
        AllVisible,
        InnerOnly,
        None
    }

    internal class BodyPartUIPanel
    {
        public class BodyPartUIRegion
        {
            public string Name;
            public Vector2 Position;
            public Vector2 Size;
            public RenderState State = RenderState.AllVisible;

            public Vector2 BottomRight => Position + Size;

            // Check this because it's definitely a contain issue.
            public bool Contains(Vector2 point)
            {
                return point.X >= Position.X && point.X <= BottomRight.X &&
                    point.Y >= Position.Y && point.Y <= BottomRight.Y;
            }

            public void CycleState()
            {
                State = (RenderState)(((int)State + 1) % 3);
            }

        }

        private List<BodyPartUIRegion> _regions = new();

        public BodyPartUIPanel()
        {
            Add("Head",     new Vector2(40, 64),     new Vector2(32, 32));
            Add("Body",     new Vector2(40, 96),    new Vector2(32, 48));
            Add("ArmRight",  new Vector2(24, 96),     new Vector2(16, 48));
            Add("ArmLeft", new Vector2(72, 96),    new Vector2(16, 48));
            Add("LegRight",  new Vector2(40, 144),    new Vector2(16, 48));
            Add("LegLeft", new Vector2(56, 144),    new Vector2(16, 48));
        }

        private void Add(string name, Vector2 pos, Vector2 size)
        {
            _regions.Add(new BodyPartUIRegion { Name = name, Position = pos, Size = size });
        }

        public void Render()
        {
            foreach (var part in _regions)
            {
                switch (part.State)
                {
                    case RenderState.AllVisible:
                        DrawRect(part.Position, part.Size, new Vector3(1.0f, 0.0f, 0.2f)); // Red
                        DrawRect(part.Position + new Vector2(4, 4), part.Size - new Vector2(8, 8), new Vector3(1.0f, 0.84f, 0.0f)); // Yellow (inner)
                        break;

                    case RenderState.InnerOnly:
                        DrawRect(part.Position, part.Size, new Vector3(0.8f, 0.4f, 0.5f)); // Dim red/orange outer
                        DrawRect(part.Position + new Vector2(4, 4), part.Size - new Vector2(8, 8), new Vector3(1.0f, 0.84f, 0.0f)); // Yellow (inner)
                        break;

                    case RenderState.None:
                        DrawRect(part.Position, part.Size, new Vector3(0.3f, 0.3f, 0.3f)); // Grey
                        break;
                }

                GL.End();
            }
        }


        private void DrawRect(Vector2 pos, Vector2 size, Vector3 color)
        {
            GL.Color3(color);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(pos.X, pos.Y);
            GL.Vertex2(pos.X + size.X, pos.Y);
            GL.Vertex2(pos.X + size.X, pos.Y + size.Y);
            GL.Vertex2(pos.X, pos.Y + size.Y);
        }

        public void HandleClick(Vector2 mousePos)
        {
            foreach(var part in _regions)
            {
                if (part.Contains(mousePos))
                {
                    part.CycleState();
                    Console.WriteLine($"Clicked on {part.Name}, making it {part.State}.");
                    break;
                }
            }
        }

        public Dictionary<string, RenderState> GetRenderStates()
        {
            return _regions.ToDictionary(p => p.Name, p => p.State);
        }

    }
}
