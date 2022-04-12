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
    class Game : GameWindow
    {
        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) {  }

        Stopwatch stopwatch = new Stopwatch();
        
        Shader colorShader;
        Shader textureShader;

        bool firstMove = true;
        Vector2 lastPos;
        float pitch = 0;
        float yaw = 90;
        float sensitivity = 0.25f;
        float fov = 90f;


        float speed = 4f;
        Vector3 camPosition = new Vector3(0.0f, 0.0f, -5.0f);
        public Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        private readonly Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        Renderer house;


        //------------------------\\



        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!Focused)
                return;

            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsKeyDown(Key.W))
            {
                camPosition += front * speed * (float)e.Time;
            }
            if (input.IsKeyDown(Key.S))
            {
                camPosition -= front * speed * (float)e.Time;
            }
            if (input.IsKeyDown(Key.A))
            {
                camPosition -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time;
            }
            if (input.IsKeyDown(Key.D))
            {
                camPosition += Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Space))
            {
                camPosition += up * speed * (float)e.Time;
            }
            if (input.IsKeyDown(Key.ControlLeft))
            {
                camPosition -= up * speed * (float)e.Time;
            }


            var mouse = Mouse.GetState();

            if (firstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                float deltaX = mouse.X - lastPos.X;
                float deltaY = mouse.Y - lastPos.Y;
                lastPos = new Vector2(mouse.X, mouse.Y);

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

            if (input.IsKeyDown(Key.Left))
            {
                house.position.X += 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Right))
            {
                house.position.X -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Down))
            {
                house.position.Z -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Up))
            {
                house.position.Z += 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.E))
            {
                house.position.Y -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Q))
            {
                house.position.Y += 5f * (float)e.Time;
            }


            base.OnUpdateFrame(e);
        }


        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused)
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (fov - e.DeltaPrecise >= 90.0f)
            {
                fov = 90.0f;
            }
            else if (fov - e.DeltaPrecise <= 1.0f)
            {
                fov = 1.0f;
            }
            else
            {
                fov -= e.DeltaPrecise;
            }


            base.OnMouseWheel(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            RenderManager.Initialize();
            CursorVisible = false;
            CursorGrabbed = true;

            Location = new System.Drawing.Point(80, 45);
            
            colorShader = new Shader("C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Shaders/color.vert", "C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Shaders/color.frag");
            textureShader = new Shader("C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Shaders/texture.vert", "C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Shaders/texture.frag");

            Thread physicsThread = new Thread(new ThreadStart(startPhysics));
            physicsThread.Start();

            LineRenderer firstLine = new LineRenderer(new Vector3(3, 3, 3), new Vector3(1, 3, 8), Color.White);
            RenderManager.AddRenderer(firstLine);

            Renderer greenCube = new Renderer("cube.obj", new Vector3(-5, 0, 0), new Vector3(0, 0, 0), Color.Blue, colorShader, _lightPos, camPosition, 1f);
            RenderManager.AddRenderer(greenCube);
            greenCube.wireframe = true;

            Renderer ground = new Renderer("ground.obj", new Vector3(0, -5, 0), new Vector3(0, 0, 0), Color.Green, colorShader, _lightPos, camPosition, 1f);
            RenderManager.AddRenderer(ground);

            house = new Renderer("house.obj", new Vector3(-5, 0, 0), new Vector3(0, 0, 0), "House.png", textureShader, _lightPos, camPosition, 1f);
            RenderManager.AddRenderer(house);

            stopwatch.Start();

            base.OnLoad(e);
        }

        void startPhysics()
        {
            Physics.Initialize();

            System.Timers.Timer physicsTimer = new System.Timers.Timer();
            physicsTimer.Interval = 10;
            physicsTimer.Elapsed += updateTick;
            physicsTimer.Enabled = true;
        }

        void updateTick(Object source, EventArgs e)
        {
            Physics.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Matrix4 view = Matrix4.LookAt(camPosition, camPosition + front, up);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), Width / Height, 0.1f, 100.0f);
            Context.SwapBuffers();

            RenderManager.Render(view, projection, stopwatch);

            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            colorShader.Dispose();
            textureShader.Dispose();
            RenderManager.Uninitialize();
            base.OnUnload(e);
        }

    }
}
