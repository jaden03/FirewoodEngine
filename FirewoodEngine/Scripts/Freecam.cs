using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine
{
    using static Logging;
    class Freecam : Component
    {
        Camera cam;
        
        Vector2 lastMousePos;
        float lastMouseWheelPos = 0;
        
        float pitch = 0;
        float yaw = 90;
        float sensitivity = 0.05f;
        float fov = 90f;

        float speed = 4f;
        public Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        
        public void Start()
        {
            cam = gameObject.GetComponent<Camera>();
        }

        public void Update(FrameEventArgs e)
        {
            if (Input.GetKey(Key.W))
            {
                gameObject.transform.position += front * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.S))
            {
                gameObject.transform.position -= front * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.A))
            {
                gameObject.transform.position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.D))
            {
                gameObject.transform.position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.Space))
            {
                gameObject.transform.position += up * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.ControlLeft))
            {
                gameObject.transform.position -= up * speed * (float)e.Time;
            }


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


            if (lastMouseWheelPos == 0)
            {
                lastMouseWheelPos = Input.GetMouseWheel();
            }

            var mouseWheelDelta = Input.GetMouseWheel() - lastMouseWheelPos;
            if (fov - mouseWheelDelta >= 90.0f)
            {
                fov = 90.0f;
            }
            else if (fov - mouseWheelDelta <= 1.0f)
            {
                fov = 1.0f;
            }
            else
            {
                fov -= mouseWheelDelta;
            }
            lastMouseWheelPos = Input.GetMouseWheel();

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
