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

        int VertexBufferObject;

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

        List<Renderer> renderers;

        private readonly Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);


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

            //if (input.IsKeyDown(Key.W))
            //{
            //    camPosition += front * speed * (float)e.Time;
            //}
            //if (input.IsKeyDown(Key.S))
            //{
            //    camPosition -= front * speed * (float)e.Time;
            //}
            //if (input.IsKeyDown(Key.A))
            //{
            //    camPosition -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time;
            //}
            //if (input.IsKeyDown(Key.D))
            //{
            //    camPosition += Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time;
            //}
            //if (input.IsKeyDown(Key.Space))
            //{
            //    camPosition += up * speed * (float)e.Time;
            //}
            //if (input.IsKeyDown(Key.ControlLeft))
            //{
            //    camPosition -= up * speed * (float)e.Time;
            //}

            foreach (Renderer rend in renderers)
            {
                if (rend.name == "Player")
                {
                    //camPosition = rend.position + new Vector3(0, 3, 0);
                    rend.eulerAngles = new Vector3(0, MathHelper.DegreesToRadians(-yaw), 0);

                    if (input.IsKeyDown(Key.W))
                    {
                        var addedVec = front * speed * (float)e.Time;
                        addedVec.Y = 0;
                        rend.position += addedVec;
                    }
                    if (input.IsKeyDown(Key.A))
                    {
                        var addedVec = Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time;
                        addedVec.Y = 0;
                        rend.position -= addedVec;
                    }
                    if (input.IsKeyDown(Key.S))
                    {
                        var addedVec = front * speed * (float)e.Time;
                        addedVec.Y = 0;
                        rend.position -= addedVec;
                    }
                    if (input.IsKeyDown(Key.D))
                    {
                        var addedVec = Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time;
                        addedVec.Y = 0;
                        rend.position += addedVec;
                    }
                }
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
                foreach (Renderer rend in renderers)
                {
                    if (rend.name == "House")
                    {
                        rend.position.X += 5f * (float)e.Time;
                    }
                }
            }
            if (input.IsKeyDown(Key.Right))
            {
                foreach (Renderer rend in renderers)
                {
                    if (rend.name == "House")
                    {
                        rend.position.X -= 5f * (float)e.Time;
                    }
                }
            }
            if (input.IsKeyDown(Key.Down))
            {
                foreach (Renderer rend in renderers)
                {
                    if (rend.name == "House")
                    {
                        rend.position.Z -= 5f * (float)e.Time;
                    }
                }
            }
            if (input.IsKeyDown(Key.Up))
            {
                foreach (Renderer rend in renderers)
                {
                    if (rend.name == "House")
                    {
                        rend.position.Z += 5f * (float)e.Time;
                    }
                }
            }
            if (input.IsKeyDown(Key.E))
            {
                foreach (Renderer rend in renderers)
                {
                    if (rend.name == "House")
                    {
                        rend.position.Y -= 5f * (float)e.Time;
                    }
                }
            }
            if (input.IsKeyDown(Key.Q))
            {
                foreach (Renderer rend in renderers)
                {
                    if (rend.name == "House")
                    {
                        rend.position.Y += 5f * (float)e.Time;
                    }
                }
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
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            CursorVisible = false;
            CursorGrabbed = true;

            VertexBufferObject = GL.GenBuffer();
            colorShader = new Shader("C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Shaders/color.vert", "C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Shaders/color.frag");
            textureShader = new Shader("C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Shaders/texture.vert", "C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Shaders/texture.frag");

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

            Thread physicsThread = new Thread(new ThreadStart(startPhysics));
            physicsThread.Start();

            renderers = new List<Renderer>();

            //Renderer newSquare = new Renderer("Test Model", "untitled.obj", new Vector3(0, -2, 0), "Untitled.png", textureShader, _lightPos, camPosition, 1f);
            //renderers.Add(newSquare);
            //Physics.addRenderer(newSquare);

            //Renderer newSquare = new Renderer("Donut", "donut.obj", new Vector3(0, -2, 0), new Vector3(0, 0, 0), Color.Gray, colorShader, _lightPos, camPosition, 1f);
            //renderers.Add(newSquare);
            //Physics.addRenderer(newSquare);

            //Renderer blueCube = new Renderer("Blue Cube", "cube.obj", new Vector3(0, 1f, 0), Color.Blue, colorShader,_lightPos, camPosition, 1f);
            //renderers.Add(blueCube);
            //Physics.addRenderer(blueCube);

            Renderer greenCube = new Renderer("Green Cube", "cube.obj", new Vector3(-5, 0, 0), new Vector3(0, 0, 0), Color.Blue, colorShader, _lightPos, camPosition, 1f);
            renderers.Add(greenCube);
            //greenCube.anchored = false;
            //greenCube.gravity = true;
            greenCube.wireframe = true;
            //Physics.addRenderer(greenCube);

            Renderer ground = new Renderer("Ground", "ground.obj", new Vector3(0, -5, 0), new Vector3(0, 0, 0), Color.Green, colorShader, _lightPos, camPosition, 1f);
            renderers.Add(ground);
            //Physics.addRenderer(ground);

            //Renderer player = new Renderer("Player", "cube.obj", new Vector3(0, 1, 0), new Vector3(0, 0, 0), Color.FromArgb(0, 0, 0, 0), colorShader, _lightPos, camPosition, 1f);
            //renderers.Add(player);
            ////player.anchored = false;
            ////player.gravity = true;
            //Physics.addRenderer(player);

            Renderer laCubis = new Renderer("House", "house.obj", new Vector3(-5, 0, 0), new Vector3(0, 0, 0), "House.png", textureShader, _lightPos, camPosition, 1f);
            renderers.Add(laCubis);
            //Physics.addRenderer(laCubis);


            //GL.Frustum(100, 100, 100, 100, 0.01f, 1000);
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
            //Console.WriteLine(position);
            //Console.WriteLine(yaw);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 view = Matrix4.LookAt(camPosition, camPosition + front, up);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), Width / Height, 0.1f, 100.0f);

            foreach (Renderer rend in renderers)
            {
                rend.Render(view, projection, stopwatch.Elapsed.TotalSeconds);
            }

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
            colorShader.Dispose();
            textureShader.Dispose();
            base.OnUnload(e);
        }

    }
}
