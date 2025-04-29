using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector2 = OpenTK.Mathematics.Vector2;

namespace MC_Skin_Aseprite_Previewer
{
    internal class OrbitalCamera
    {
        public Vector3 Target = Vector3.Zero;
        public float ZoomSpeed = 2.0f;
        public float RotationSpeed = 0.01f;

        private float yaw = -0.25f;
        private float pitch = -0.25f;
        public float radius = 30.0f;

        Vector2 lastMousePos;
        bool panning = false;
        float panSpeed = 0.01f;

        public void HandleInput(MouseState mouse, MouseState prevMouse, Vector2 scrollDelta, float aspectRatio)
        {
            if (mouse.IsButtonDown(MouseButton.Right))
            {
                var delta = mouse.Position - prevMouse.Position;
                yaw -= delta.X * RotationSpeed;
                pitch -= delta.Y * RotationSpeed;

                // Clamp to avoid flipping.
                pitch = Math.Clamp(pitch, -1.5f, 1.5f);

                Console.WriteLine($"Mouse Delta: {delta.ToString()}");
            }

            // Scroll Zoom
            radius -= scrollDelta.Y * ZoomSpeed;
            radius = Math.Max(0.1f, radius);

            Vector2 mousePos = mouse.Position;

            Vector3 cameraPosition = Target + new Vector3(
                radius * MathF.Cos(pitch) * MathF.Sin(yaw),
                radius * MathF.Sin(pitch),
                radius * MathF.Cos(pitch) * MathF.Cos(yaw)
            );


            Vector3 cameraDirection = Vector3.Normalize(Target - cameraPosition);

            if (mouse.IsButtonDown(MouseButton.Middle))
            {
                if (!panning)
                {
                    panning = true;
                    lastMousePos = mousePos;
                }
                else
                {

                    // Adjust to be consistent across different aspect ratios.

                    Vector2 delta = mousePos - lastMousePos;
                    lastMousePos = mousePos;

                    Vector3 right = Vector3.Normalize(Vector3.Cross(cameraDirection, Vector3.UnitY));
                    Vector3 up = Vector3.Normalize(Vector3.Cross(right, cameraDirection));

                    float panAdjustment = panSpeed * radius / 4;

                    Target += right * delta.X * panAdjustment * aspectRatio;
                    Target += up * delta.Y * panAdjustment;
                }
            }
            else
            {
                panning = false;
            }
        }

        public Matrix4 GetViewMatrix()
        {
            var direction = new Vector3(
                (float)(Math.Cos(pitch) * Math.Sin(yaw)),
                (float)(Math.Sin(pitch)),
                (float)(Math.Cos(pitch) * Math.Cos(yaw))
            );

            var position = Target - direction * radius;
            return Matrix4.LookAt(position, Target, Vector3.UnitY);
        }
    }
}
