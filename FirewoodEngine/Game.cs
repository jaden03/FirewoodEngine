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

        GameObject lineObject;
        GameObject house;
        GameObject ground;

        Material lineMat;
        LineRenderer line;

        List<float> vertices;
        List<float> triangles;

        float[][] noise;

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
                line.position1.X += 5f * (float)e.Time;
                house.transform.position.X += 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Right))
            {
                line.position1.X -= 5f * (float)e.Time;
                house.transform.position.X -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Down))
            {
                line.position1.Z -= 5f * (float)e.Time;
                house.transform.position.Z -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Up))
            {
                line.position1.Z += 5f * (float)e.Time;
                house.transform.position.Z += 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.E))
            {
                line.position1.Y += 5f * (float)e.Time;
                house.transform.position.Y -= 5f * (float)e.Time;
            }
            if (input.IsKeyDown(Key.Q))
            {
                line.position1.Y -= 5f * (float)e.Time;
                house.transform.position.Y += 5f * (float)e.Time;
            }

            

            


            
            //if (line.position1.Y < 0)
            //{
            //    lineMat.color = Color.Red;
            //}
            //else
            //{
            //    lineMat.color = Color.Blue;
            //}



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


        void AddQuadAtPosition(Vector2 pos)
        {
            Vector2 noisePos = new Vector2(pos.X + .5f, pos.Y + .5f);

            float bottomLeftNoise = noise[(int)Math.Round(noisePos.X - .5f)][(int)Math.Round(noisePos.Y - .5f)] * 5;
            //bottomLeftNoise = (float)Math.Round(bottomLeftNoise * 3) / 3;
            
            vertices.Add(pos.X - .5f);
            vertices.Add(bottomLeftNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(bottomLeftNoise);
            triangles.Add(pos.Y - .5f);

            vertices.Add(0);
            vertices.Add(1);
            vertices.Add(0);

            float topLeftNoise = noise[(int)Math.Round(noisePos.X - .5f)][(int)Math.Round(noisePos.Y + .5f)] * 5;
            //topLeftNoise = (float)Math.Round(topLeftNoise * 3) / 3;

            vertices.Add(pos.X - .5f);
            vertices.Add(topLeftNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(topLeftNoise);
            triangles.Add(pos.Y + .5f);

            vertices.Add(0);
            vertices.Add(1);
            vertices.Add(0);

            float topRightNoise = noise[(int)Math.Round(noisePos.X + .5f)][(int)Math.Round(noisePos.Y + .5f)] * 5;
            //topRightNoise = (float)Math.Round(topRightNoise * 3) / 3;

            vertices.Add(pos.X + .5f);
            vertices.Add(topRightNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(topRightNoise);
            triangles.Add(pos.Y + .5f);

            vertices.Add(0);
            vertices.Add(1);
            vertices.Add(0);

            vertices.Add(pos.X + .5f);
            vertices.Add(topRightNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(topRightNoise);
            triangles.Add(pos.Y + .5f);

            vertices.Add(0);
            vertices.Add(1);
            vertices.Add(0);

            float bottomRightNoise = noise[(int)Math.Round(noisePos.X + .5f)][(int)Math.Round(noisePos.Y - .5f)] * 5;
            //bottomRightNoise = (float)Math.Round(bottomRightNoise * 3) / 3;

            vertices.Add(pos.X + .5f);
            vertices.Add(bottomRightNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(bottomRightNoise);
            triangles.Add(pos.Y - .5f);

            vertices.Add(0);
            vertices.Add(1);
            vertices.Add(0);

            vertices.Add(pos.X - .5f);
            vertices.Add(bottomLeftNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(bottomLeftNoise);
            triangles.Add(pos.Y - .5f);

            vertices.Add(0);
            vertices.Add(1);
            vertices.Add(0);

        }



        protected override void OnLoad(EventArgs e)
        {
            RenderManager.Initialize();
            CursorVisible = false;
            CursorGrabbed = true;

            Location = new System.Drawing.Point(80, 45);
            
            colorShader = new Shader("../../Shaders/color.vert", "../../Shaders/color.frag");
            textureShader = new Shader("../../Shaders/texture.vert", "../../Shaders/texture.frag");

            Thread physicsThread = new Thread(new ThreadStart(startPhysics));
            physicsThread.Start();

            lineMat = new Material();
            lineMat.shader = colorShader;
            lineMat.color = Color.Blue;

            lineObject = new GameObject();
            lineObject.name = "Lines";
            lineObject.transform.position = new Vector3(5, 0, 5);
            line = new LineRenderer(new Vector3(-8.8f, 0.3f, -2.8f), new Vector3(-8, 3, 0));
            line.material = lineMat;
            line.useLocal = true;
            lineObject.AddComponent(line);
            RenderManager.AddRenderer(line);

            //Material groundMat = new Material();
            //groundMat.SetTexture("ground.png");
            //groundMat.shader = textureShader;

            //ground = new GameObject();
            //ground.name = "Ground";
            //ground.transform.position.Y = -3;
            
            //Renderer groundRenderer = new Renderer();
            //groundRenderer.SetOBJ("ground.obj", groundMat.texture != null);
            //groundRenderer.material = groundMat;
            //ground.AddComponent(groundRenderer);
            //RenderManager.AddRenderer(groundRenderer);
            


            Material houseMat = new Material();
            houseMat.SetTexture("House.png");
            houseMat.shader = textureShader;

            house = new GameObject();
            house.name = "House";
            house.transform.position = new Vector3(0, 5, 0);
            Renderer houseRenderer = new Renderer();
            houseRenderer.SetOBJ("house.obj", houseMat.texture != null);
            houseRenderer.material = houseMat;
            house.AddComponent(houseRenderer);
            RenderManager.AddRenderer(houseRenderer);





            Material terrainMat = new Material();
            terrainMat.color = Color.Green;
            terrainMat.shader = colorShader;

            //GameObject plane = new GameObject();
            //plane.name = "Terrain";
            //plane.transform.position.Y = -3;
            //Renderer planeRenderer = new Renderer();
            //planeRenderer.SetOBJ("plane.obj", terrainMat.texture != null);
            //planeRenderer.material = terrainMat;
            //plane.AddComponent(planeRenderer);
            //RenderManager.AddRenderer(planeRenderer);

            //// loop through the vertices and log them
            //foreach (float v in planeRenderer.triangles)
            //{
            //    Console.WriteLine(v);
            //}

            GameObject terrain = new GameObject();
            terrain.name = "Terrain";
            terrain.transform.position.Y = -3;
            Renderer terrainRenderer = new Renderer();
            terrainRenderer.material = terrainMat;
            terrain.AddComponent(terrainRenderer);

            vertices = new List<float>();
            triangles = new List<float>();

            int size = 10;

            noise = Noise.GeneratePerlinNoise(Noise.GenerateWhiteNoise(size * 10, size * 10), 3);
            for (int x = 1; x < size; x++)
            {
                for (int y = 1; y < size; y++)
                {
                    AddQuadAtPosition(new Vector2(x, y));
                }
            }

            terrainRenderer.vertices = new float[vertices.Count];
            terrainRenderer.triangles = new float[triangles.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                terrainRenderer.vertices[i] = vertices[i];
            }
            for (int i = 0; i < triangles.Count; i++)
            {
                terrainRenderer.triangles[i] = triangles[i];
            }

            RenderManager.AddRenderer(terrainRenderer);








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
