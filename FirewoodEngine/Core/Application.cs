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
using FirewoodEngine.Components;

namespace FirewoodEngine.Core
{
    using static Logging;
    public class Application : GameWindow
    {
        public Application(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) {  }
        
        ImGuiController _controller;

        public Stopwatch stopwatch = new Stopwatch();

        public Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);
        
        public List<object> activeCoreScripts;

        StringWriter consoleOut;
        public List<string> consoleOutput;

        public int RenderTexture;
        public int RenderTextureEditor;

        EditorUI editorUI;

        public Camera gameCamera = null;
        
        public bool isPlaying = false;
        public bool isPaused = true;

        //------------------------\\

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            EditorCamera.Update(e);

            var gameObjects = GameObjectManager.gameObjects;
            var components = ComponentManager.components;
            
            var foundCamera = false;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].GetComponent<Camera>() != null)
                {
                    gameCamera = gameObjects[i].GetComponent<Camera>();
                    foundCamera = true;
                }
                

                // for (int j = 0; j < gameObjects[i].components.Count; j++)
                // {
                //     try
                //     {
                //          gameObjects[i].components[j].GetType().GetMethod("Update").Invoke(gameObjects[i].components[j], new object[] { e });
                //     }
                //     catch (Exception exception)
                //     {
                //         Console.WriteLine(exception);
                //     }
                // }
            }
            
            if (!foundCamera)
                gameCamera = null;
            
            foreach (object script in activeCoreScripts)
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
            
            editorUI.UpdateUI(RenderTexture, consoleOutput, RenderTextureEditor);

            Input.focused = Focused;

            _controller.Render();

            ImGuiController.CheckGLError("End of frame");
            
            Physics.Update(e);
            
            Context.SwapBuffers();
            
            if (!isPlaying || isPaused) { return; }
                
            foreach (Component component in components)
            {
                component.GetType().GetMethod("Update").Invoke(component, new object[] { e });
            }

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
            // Im not sure how to handle caps lock
            
            var key = e.Key.ToString();
            
            if (key == "Minus")
                key = "-";
            
            if (key == "Plus")
                key = "+";
            
            if (key == "Period")
                key = ".";
            
            if (key == "Comma")
                key = ",";

            if (key.StartsWith("Number") || key.StartsWith("Keypad"))
            {
                // remove "Number" from the string
                key = key.Remove(0, 6);
            }

            if (key == "Space")
            {
                key = " ";
                _controller.PressChar((char)key.ToCharArray()[0]);
            }
            
            if (key.Length > 1)
                return;
            
            if (Input.GetKey(Key.ShiftLeft))
                key = key.ToUpper();
            else
                key = key.ToLower();
            
            _controller.PressChar((char)key.ToCharArray()[0]);

            base.OnKeyDown(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            consoleOut = new StringWriter();
            Console.SetOut(consoleOut);
            consoleOutput = new List<string>();

            startPhysics();
            _controller = new ImGuiController(Width, Height);
            editorUI = new EditorUI();
            editorUI.Initialize(_controller);
            editorUI.app = this;

            WindowState = WindowState.Maximized;
            
            activeCoreScripts = new List<object>();

            EditorCamera.app = this;
            ComponentManager.Initialize();
            GameObjectManager.Initialize();
            RenderManager.Initialize(this);
            AudioManager.Init();
            Editor.Initialize(this);

            stopwatch.Start();

            //var game = new Game();
            //game.app = this;
            //activeScripts.Add(game);
            //game.Start();

            var input = new Input();
            input.app = this;
            activeCoreScripts.Add(input);

            base.OnLoad(e);
        }

        void startPhysics()
        {
            Physics.Initialize(this);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (gameCamera != null)
            {
                Matrix4 view = Matrix4.LookAt(gameCamera.transform.position, gameCamera.transform.position + gameCamera.transform.forward, gameCamera.transform.up);
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(gameCamera.fov),
                    (float)EditorUI.gameSize.X / (float)EditorUI.gameSize.Y, gameCamera.near, gameCamera.far);

                GL.ClearColor(gameCamera.backgroundColor);
                RenderManager.Render(view, projection, stopwatch, _lightPos, gameCamera.transform.position, this);
            }

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
