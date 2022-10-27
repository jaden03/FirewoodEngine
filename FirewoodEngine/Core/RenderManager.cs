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
using OpenTK.Platform.MacOS;

namespace FirewoodEngine.Core
{
    using static Logging;
    class RenderManager
    {
        public static List<Renderer> renderers;
        public static List<LineRenderer> lineRenderers;
        static int VertexBufferObject;

        static int FrameBufferObject;
        static int RenderTextureObject;
        static int DepthBufferObject;

        static int FrameBufferObjectEditor;
        static int RenderTextureObjectEditor;
        static int DepthBufferObjectEditor;

        public static void Initialize(Application app)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Renderer Initialized");
            Console.ForegroundColor = ConsoleColor.White;

            GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);


            FrameBufferObject = GL.GenFramebuffer();
            RenderTextureObject = GL.GenTexture();
            DepthBufferObject = GL.GenRenderbuffer();

            FrameBufferObjectEditor = GL.GenFramebuffer();
            RenderTextureObjectEditor = GL.GenTexture();
            DepthBufferObjectEditor = GL.GenRenderbuffer();

            app.RenderTexture = RenderTextureObject;
            app.RenderTextureEditor = RenderTextureObjectEditor;


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

        public static void Render(Matrix4 view, Matrix4 projection, Stopwatch stopwatch, Vector3 _lightpos, Vector3 _camPos, Application app)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBufferObject);
            GL.BindTexture(TextureTarget.Texture2D, RenderTextureObject);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, app.Width, app.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBufferObject);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, app.Width, app.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DepthBufferObject);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, RenderTextureObject, 0);


            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("ERROR");


            foreach (Renderer rend in renderers)
            {
                rend.Render(view, projection, stopwatch.Elapsed.TotalSeconds, _lightpos, _camPos, VertexBufferObject, FrameBufferObject, RenderTextureObject, DepthBufferObject);
            }

            foreach (LineRenderer rend in lineRenderers)
            {
                rend.Draw(view, projection, stopwatch.Elapsed.TotalSeconds, _lightpos, _camPos, VertexBufferObject, FrameBufferObject, RenderTextureObject, DepthBufferObject);
            }
            GL.ReadBuffer(ReadBufferMode.Back);
            GL.BlitFramebuffer(0, 0, app.Width, app.Height, 0, 0, app.Width, app.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }


        public static void RenderEditor(Matrix4 view, Matrix4 projection, Stopwatch stopwatch, Vector3 _lightpos, Vector3 _camPos, Application app)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FrameBufferObjectEditor);
            GL.BindTexture(TextureTarget.Texture2D, RenderTextureObjectEditor);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, app.Width, app.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, DepthBufferObjectEditor);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, app.Width, app.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, DepthBufferObjectEditor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, RenderTextureObjectEditor, 0);


            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("ERROR");


            foreach (Renderer rend in renderers)
            {
                rend.Render(view, projection, stopwatch.Elapsed.TotalSeconds, _lightpos, _camPos, VertexBufferObject, FrameBufferObjectEditor, RenderTextureObjectEditor, DepthBufferObjectEditor);
            }

            foreach (LineRenderer rend in lineRenderers)
            {
                rend.Draw(view, projection, stopwatch.Elapsed.TotalSeconds, _lightpos, _camPos, VertexBufferObject, FrameBufferObjectEditor, RenderTextureObjectEditor, DepthBufferObjectEditor);
            }
            GL.ReadBuffer(ReadBufferMode.Back);
            GL.BlitFramebuffer(0, 0, app.Width, app.Height, 0, 0, app.Width, app.Height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }



        public static void Uninitialize()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
        }
        
        
    }
}
