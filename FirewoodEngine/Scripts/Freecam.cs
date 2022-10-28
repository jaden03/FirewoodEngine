using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FirewoodEngine.Core;
using FirewoodEngine.Components;
using Key = OpenTK.Input.Key;

namespace FirewoodEngine
{
    using static Logging;
    class Freecam : Component
    {
        //Camera cam;
        
        Vector2 lastMousePos;
        //float lastMouseWheelPos = 0;
        
        public float sensitivity = 0.1f;
        //float fov = 90f;

        public float speed = 4f;

        public override void Start()
        {
            //cam = gameObject.GetComponent<Camera>();
            Input.HideCursor = true;
            Input.LockCursor = true;
        }

        public override void Update(FrameEventArgs e)
        {
            if (Input.GetKeyDown(Key.Escape))
            {
                Input.HideCursor = !Input.HideCursor;
                Input.LockCursor = !Input.LockCursor;
            }
            
            
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

            float deltaX = mousePos.X - lastMousePos.X;
            float deltaY = mousePos.Y - lastMousePos.Y;

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

            lastMousePos = new Vector2(mousePos.X, mousePos.Y);
        }

    }
}
