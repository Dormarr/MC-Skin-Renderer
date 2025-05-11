using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace MC_Skin_Aseprite_Previewer
{
    internal class BodyPartUIPanel
    {
        public class BodyPartUIRegion
        {
            public string Name;
            public Vector2 Position;
            public Vector2 Size;
            public bool Visible = true;

            public Vector2 BottomRight => Position + Size;

            // Check this because it's definitely a contain issue.
            public bool Contains(Vector2 point)
            {
                return point.X >= Position.X && point.X <= BottomRight.X &&
                    point.Y >= Position.Y && point.Y <= BottomRight.Y;
            }

            public void Toggle() => Visible = !Visible;

        }

        private List<BodyPartUIRegion> _regions = new();

        public BodyPartUIPanel()
        {
            Add("Head",     new Vector2(40, 0),     new Vector2(32, 32));
            Add("Body",     new Vector2(40, 32),    new Vector2(32, 48));
            Add("ArmLeft",  new Vector2(24, 32),     new Vector2(16, 48));
            Add("ArmRight", new Vector2(72, 32),    new Vector2(16, 48));
            Add("LegLeft",  new Vector2(40, 80),    new Vector2(16, 48));
            Add("LegRight", new Vector2(56, 80),    new Vector2(16, 48));
        }

        private void Add(string name, Vector2 pos, Vector2 size)
        {
            _regions.Add(new BodyPartUIRegion { Name = name, Position = pos, Size = size });
        }

        public void Render()
        {
            foreach(var part in _regions)
            {
                // Outer
                DrawRect(part.Position, part.Size, part.Visible ? new Vector3(1.0f, 0.0f, 0.0f) : new Vector3(0.4f, 0.4f, 0.4f));

                // Inner
                DrawRect(part.Position + new Vector2(4, 4), part.Size - new Vector2(8, 8), new Vector3(1.0f, 0.84f, 0.0f));

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
                    Console.WriteLine($"Clicked on {part.Name}");
                    part.Toggle();
                    break;
                }
            }
        }

        public Dictionary<string, bool> GetVisibleStates()
        {
            var result = new Dictionary<string, bool>();
            foreach(var part in _regions)
            {
                result[part.Name] = part.Visible;
            }
            return result;
        }
    }
}
