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
        public Vector3 camPosition = new Vector3(0.0f, 0.0f, -5.0f);
        public Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);

        public Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);

        GameObject firstGameobject;
        GameObject house;
        GameObject ground;


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
                house.transform.position.X += 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Right))
            {
                house.transform.position.X -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Down))
            {
                house.transform.position.Z -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Up))
            {
                house.transform.position.Z += 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.E))
            {
                house.transform.position.Y -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Q))
            {
                house.transform.position.Y += 5f * (float)e.Time;
            }
            ground.transform.position.Y -= 2f * (float)e.Time;

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



            firstGameobject = new GameObject();
            firstGameobject.name = "Line";
            firstGameobject.transform.position = new Vector3(0, 10, 0);
            LineRenderer lRenderer = new LineRenderer(new Vector3(3, 12, 3), new Vector3(1, 3, 8), Color.White);
            lRenderer.useLocal = true;
            firstGameobject.AddComponent(lRenderer);
            RenderManager.AddRenderer(lRenderer);



            Material houseMat = new Material();
            houseMat.SetTexture("House.png");
            houseMat.shader = textureShader;

            house = new GameObject();
            house.name = "House";
            Renderer houseRenderer = new Renderer();
            houseRenderer.SetOBJ("house.obj", houseMat.texture != null);
            houseRenderer.material = houseMat;
            house.AddComponent(houseRenderer);
            RenderManager.AddRenderer(houseRenderer);



            Material groundMat = new Material();
            groundMat.shader = colorShader;
            groundMat.color = Color.Green;

            ground = new GameObject();
            ground.name = "Ground";
            Renderer groundRenderer = new Renderer();
            groundRenderer.SetOBJ("ground.obj", groundMat.texture != null);
            groundRenderer.material = groundMat;
            ground.AddComponent(groundRenderer);
            RenderManager.AddRenderer(groundRenderer);



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
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)Width / Height, 0.1f, 100.0f);
            Context.SwapBuffers();

            RenderManager.Render(view, projection, stopwatch, _lightPos, camPosition);

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
            colorShader.Dispose();
            textureShader.Dispose();
            RenderManager.Uninitialize();
            base.OnUnload(e);
        }

    }
}
