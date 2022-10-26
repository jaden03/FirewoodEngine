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
using Dear_ImGui_Sample;
using ImGuiNET;
using System.IO;

namespace FirewoodEngine.Core
{
    using static Logging;
    public class Application : GameWindow
    {
        public Application(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) {  }
        
        ImGuiController _controller;

        public Stopwatch stopwatch = new Stopwatch();

        public Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        public List<object> activeScripts;

        StringWriter consoleOut;
        public List<string> consoleOutput;

        public int RenderTexture;

        EditorUI editorUI;

        //------------------------\\

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            foreach (object script in activeScripts)
            {
                script.GetType().GetMethod("Update").Invoke(script, new[] { e });
            }

            foreach (GameObject gameObject in GameObjectManager.gameObjects)
            {
                gameObject.transform.Update(e);
            }

            _controller.Update(this, (float)e.Time);

            if (consoleOut.ToString() != "")
            {
                consoleOutput.Add(consoleOut.ToString());
            }
            consoleOut.GetStringBuilder().Clear();
            
            editorUI.UpdateUI(RenderTexture, consoleOutput);

            Input.focused = Focused;

            _controller.Render();

            ImGuiController.CheckGLError("End of frame");

            Physics.Update(e);

            base.OnUpdateFrame(e);
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (!Focused) return;

            if (Input.LockCursor)
            {
                Mouse.SetPosition(X + ClientSize.Width / 2f, Y + ClientSize.Height / 2f);
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

            Input.SetMousePosition(new Vector2(e.X, e.Y));

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _controller.MouseScroll(e.Delta);

            base.OnMouseWheel(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            _controller.PressChar((char)e.Key);

            base.OnKeyDown(e);
        }


        protected override void OnLoad(EventArgs e)
        {
            _controller = new ImGuiController(Width, Height);
            editorUI = new EditorUI();
            editorUI.Initialize(_controller);
            editorUI.app = this;

            consoleOut = new StringWriter();
            Console.SetOut(consoleOut);
            consoleOutput = new List<string>();
            
            GameObjectManager.Initialize();
            RenderManager.Initialize(this);

            WindowState = WindowState.Maximized;
            //Location = new System.Drawing.Point(80, 45);

            startPhysics();

            AudioManager.Init();

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
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.Ortho(-1.0f * ClientSize.Width / ClientSize.Height, 1.0f * ClientSize.Width / ClientSize.Height, -1.0f, 1.0f, .1f, 100f);

            _controller.WindowResized(Width, Height);

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
