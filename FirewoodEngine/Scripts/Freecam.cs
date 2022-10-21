using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirewoodEngine.Core;
using FirewoodEngine.Componenents;

namespace FirewoodEngine
{
    using static Logging;
    class Freecam : Component
    {
        //Camera cam;
        
        Vector2 lastMousePos;
        //float lastMouseWheelPos = 0;
        
        float sensitivity = 0.0005f;
        //float fov = 90f;

        float speed = 4f;
        
        public void Start()
        {
            //cam = gameObject.GetComponent<Camera>();
        }

        public void Update(FrameEventArgs e)
        {
            if (Input.GetKey(Key.W))
            {
                gameObject.transform.position += transform.children[0].forward * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.S))
            {
                gameObject.transform.position -= transform.children[0].forward * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.A))
            {
                gameObject.transform.position -= Vector3.Normalize(Vector3.Cross(transform.children[0].forward, Vector3.UnitY)) * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.D))
            {
                gameObject.transform.position += Vector3.Normalize(Vector3.Cross(transform.children[0].forward, Vector3.UnitY)) * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.Space))
            {
                gameObject.transform.position += Vector3.UnitY * speed * (float)e.Time;
            }
            if (Input.GetKey(Key.ControlLeft))
            {
                gameObject.transform.position -= Vector3.UnitY * speed * (float)e.Time;
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

                deltaX = -deltaX;
                deltaY = -deltaY;

                transform.eulerAngles.Y += deltaX * sensitivity;
                if (transform.children[0].localEulerAngles.X > 89.0f)
                {
                    transform.children[0].localEulerAngles.X = 89.0f;
                }
                else if (transform.children[0].localEulerAngles.X < -89.0f)
                {
                    transform.children[0].localEulerAngles.X = -89.0f;
                }
                else
                {
                    transform.children[0].localEulerAngles.X -= deltaY * sensitivity;
                }
            }


            //if (lastMouseWheelPos == 0)
            //{
            //    lastMouseWheelPos = Input.GetMouseWheel();
            //}

            //var mouseWheelDelta = Input.GetMouseWheel() - lastMouseWheelPos;
            //if (fov - mouseWheelDelta >= 90.0f)
            //{
            //    fov = 90.0f;
            //}
            //else if (fov - mouseWheelDelta <= 1.0f)
            //{
            //    fov = 1.0f;
            //}
            //else
            //{
            //    fov -= mouseWheelDelta;
            //}
            //lastMouseWheelPos = Input.GetMouseWheel();

            //cam.fov = fov;
        }

    }
}
