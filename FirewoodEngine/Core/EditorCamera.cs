using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace FirewoodEngine.Core
{
    static class EditorCamera
    {
        public static Application app;

        static Vector2 lastMousePos;
        static float sensitivity = .1f;
        static float speed = 4f;
        static float fov = 90;

        static Vector3 position = new Vector3(0, 3, -8);
        static Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        static float pitch = -10;
        static float yaw = 90;

        public static void Update(FrameEventArgs e)
        {
            if (Input.GetMouseButton(MouseButton.Right))
            {
                if (Input.GetKey(Key.W))
                {
                    position += front * speed * (float)e.Time;
                }
                if (Input.GetKey(Key.S))
                {
                    position -= front * speed * (float)e.Time;
                }
                if (Input.GetKey(Key.A))
                {
                    position -= Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY)) * speed * (float)e.Time;
                }
                if (Input.GetKey(Key.D))
                {
                    position += Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY)) * speed * (float)e.Time;
                }
                if (Input.GetKey(Key.Space))
                {
                    position += Vector3.UnitY * speed * (float)e.Time;
                    position += Vector3.UnitY * speed * (float)e.Time;
                }
                if (Input.GetKey(Key.ControlLeft))
                {
                    position -= Vector3.UnitY * speed * (float)e.Time;
                }
            }


            var mousePos = Input.GetMousePos();
            if (Input.GetMouseButton(MouseButton.Right))
            {
                float deltaX = mousePos.X - lastMousePos.X;
                float deltaY = mousePos.Y - lastMousePos.Y;
                lastMousePos = new Vector2(mousePos.X, mousePos.Y);

                yaw += deltaX * sensitivity;
                if (pitch > 89.0f)
                {
                    pitch = 89.0f;
                }
                else if (pitch < -89.0f)
                {
                    pitch = -89.0f;
                }
                else
                {
                    pitch += -deltaY * sensitivity;
                }
            }
            else
            {
                lastMousePos = new Vector2(mousePos.X, mousePos.Y);
            }

            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            front = Vector3.Normalize(front);

            Matrix4 view = Matrix4.LookAt(position, position + front, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)EditorUI.viewportSize.X / (float)EditorUI.viewportSize.Y, 0.01f, 1000.0f);

            RenderManager.Render(view, projection, app.stopwatch, app._lightPos, position, app);
        }
    }
}
