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
using System.Drawing;
using System.Timers;
using System.Threading;

namespace FirewoodEngine
{
    using static Logging;
    class Application : GameWindow
    {
        public Application(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) {  }

        public Stopwatch stopwatch = new Stopwatch();

        public Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        public List<object> activeScripts;

        //------------------------\\

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused)
                return;

            foreach (object script in activeScripts)
            {
                //script.Update(e);
                script.GetType().GetMethod("Update").Invoke(script, new[] { e });
            }

            Physics.Update(e);

            base.OnUpdateFrame(e);
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (!Focused) return;

            if (Input.LockCursor)
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
                CursorGrabbed = true;
            }
            else
            {
                CursorGrabbed = false;
            }
            
            if (Input.HideCursor)
                CursorVisible = false;
            else
                CursorVisible = true;

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }


        



        protected override void OnLoad(EventArgs e)
        {
            RenderManager.Initialize();

            Location = new System.Drawing.Point(80, 45);

            //Thread physicsThread = new Thread(new ThreadStart(startPhysics));
            //physicsThread.Start();
            startPhysics();

            stopwatch.Start();

            activeScripts = new List<object>();

            var game = new Game();
            game.app = this;
            activeScripts.Add(game);
            game.Start();

            var input = new Input();
            input.app = this;
            activeScripts.Add(input);

            base.OnLoad(e);
        }

        void startPhysics()
        {
            Physics.Initialize();

            //System.Timers.Timer physicsTimer = new System.Timers.Timer();
            //physicsTimer.Interval = 1;
            //physicsTimer.Elapsed += updateTick;
            //physicsTimer.Enabled = true;
        }

        //void updateTick(Object source, EventArgs e)
        //{
        //    Physics.Update();
        //}

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //Matrix4 view = Matrix4.LookAt(camPosition, camPosition + front, up);
            //Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)Width / Height, 0.1f, 1000.0f);
            //Context.SwapBuffers();

            //RenderManager.Render(view, projection, stopwatch, _lightPos, camPosition);

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.Ortho(-1.0f * Width / Height, 1.0f * Width / Height, -1.0f, 1.0f, .1f, 100f);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            Shader.colorShader.Dispose();
            Shader.textureShader.Dispose();
            RenderManager.Uninitialize();
            base.OnUnload(e);
        }

    }
}
