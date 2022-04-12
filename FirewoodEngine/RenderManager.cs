using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Diagnostics;

namespace FirewoodEngine
{
    class RenderManager
    {
        public static List<Renderer> renderers;
        static int VertexBufferObject;

        public static void Initialize()
        {
            Console.WriteLine("Renderer Initialized");

            GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            VertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            renderers = new List<Renderer>();
        }

        public static void AddRenderer(Renderer rend)
        {
            renderers.Add(rend);
        }

        public static void Render(Matrix4 view, Matrix4 projection, Stopwatch stopwatch)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (Renderer rend in renderers)
            {
                rend.Render(view, projection, stopwatch.Elapsed.TotalSeconds);
            }
        }


        public static void UnInitialize()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
        }
        
        
    }
}
