using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine.Scripts
{
    using static Logging;
    internal class CameraRotation : Component
    {
        Camera cam;

        Vector2 lastMousePos;

        public float pitch = 0;
        public float yaw = 90;
        float sensitivity = 0.05f;
        float fov = 90f;

        public Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        public void Start()
        {
            cam = gameObject.GetComponent("Camera") as Camera;
        }

        public void Update(FrameEventArgs e)
        {
            var mousePos = Input.GetMousePos();
            if (lastMousePos == null)
            {
                lastMousePos = new Vector2(mousePos.X, mousePos.Y);
            }
            else
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
                    pitch -= deltaY * sensitivity;
                }
            }

            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            front = Vector3.Normalize(front);

            cam.front = front;
            cam.up = up;
            cam.fov = fov;
        }
    }
}
