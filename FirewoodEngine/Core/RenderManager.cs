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
using FirewoodEngine.Componenents;

namespace FirewoodEngine.Core
{
    using static Logging;
    class RenderManager
    {
        public static List<Renderer> renderers;
        public static List<LineRenderer> lineRenderers;
        static int VertexBufferObject;

        public static void Initialize()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Renderer Initialized");
            Console.ForegroundColor = ConsoleColor.White;

            GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            VertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            renderers = new List<Renderer>();
            lineRenderers = new List<LineRenderer>();
        }

        public static void AddRenderer(Renderer rend)
        {
            renderers.Add(rend);
        }

        public static void RemoveRenderer(Renderer rend)
        {
            renderers.Remove(rend);
        }

        public static void AddRenderer(LineRenderer rend)
        {
            lineRenderers.Add(rend);
        }

        public static void RemoveRenderer(LineRenderer rend)
        {
            lineRenderers.Remove(rend);
        }

        public static void Render(Matrix4 view, Matrix4 projection, Stopwatch stopwatch, Vector3 _lightpos, Vector3 _camPos)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (Renderer rend in renderers)
            {
                rend.Render(view, projection, stopwatch.Elapsed.TotalSeconds, _lightpos, _camPos, VertexBufferObject);
            }

            foreach (LineRenderer rend in lineRenderers)
            {
                rend.Draw(view, projection, stopwatch.Elapsed.TotalSeconds, _lightpos, _camPos, VertexBufferObject);
            }

        }


        public static void Uninitialize()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
        }
        
        
    }
}
