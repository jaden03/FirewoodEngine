using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Input;

namespace FirewoodEngine
{
    using static Logging;
    class Game
    {
        public Application app;
        
        GameObject house;

        List<float> vertices;
        List<float> triangles;

        float[][] noise;
        
        void AddQuadAtPosition(Vector2 pos)
        {
            Vector2 noisePos = new Vector2(pos.X + .5f, pos.Y + .5f);

            float bottomLeftNoise = noise[(int)Math.Round(noisePos.X - .5f)][(int)Math.Round(noisePos.Y - .5f)] * 3;
            //bottomLeftNoise = (float)Math.Round(bottomLeftNoise * 3) / 3;

            vertices.Add(pos.X - .5f);
            vertices.Add(bottomLeftNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(bottomLeftNoise);
            triangles.Add(pos.Y - .5f);

            float topLeftNoise = noise[(int)Math.Round(noisePos.X - .5f)][(int)Math.Round(noisePos.Y + .5f)] * 3;
            //topLeftNoise = (float)Math.Round(topLeftNoise * 3) / 3;

            vertices.Add(pos.X - .5f);
            vertices.Add(topLeftNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(topLeftNoise);
            triangles.Add(pos.Y + .5f);
         
      
            float topRightNoise = noise[(int)Math.Round(noisePos.X + .5f)][(int)Math.Round(noisePos.Y + .5f)] * 3;
            //topRightNoise = (float)Math.Round(topRightNoise * 3) / 3;

            vertices.Add(pos.X + .5f);
            vertices.Add(topRightNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(topRightNoise);
            triangles.Add(pos.Y + .5f);


            vertices.Add(pos.X + .5f);
            vertices.Add(topRightNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(topRightNoise);
            triangles.Add(pos.Y + .5f);


            float bottomRightNoise = noise[(int)Math.Round(noisePos.X + .5f)][(int)Math.Round(noisePos.Y - .5f)] * 3;
            //bottomRightNoise = (float)Math.Round(bottomRightNoise * 3) / 3;

            vertices.Add(pos.X + .5f);
            vertices.Add(bottomRightNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(bottomRightNoise);
            triangles.Add(pos.Y - .5f);

            vertices.Add(pos.X - .5f);
            vertices.Add(bottomLeftNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(bottomLeftNoise);
            triangles.Add(pos.Y - .5f);
        }

        List<float> RecalculateNormals(List<float> vertices)
        {
            List<float> newVerticesList;
            newVerticesList = new List<float>();

            for (int i = 0; i < vertices.Count; i += 9)
            {
                Vector3 v1 = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
                Vector3 v2 = new Vector3(vertices[i + 3], vertices[i + 4], vertices[i + 5]);
                Vector3 v3 = new Vector3(vertices[i + 6], vertices[i + 7], vertices[i + 8]);

                Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1);
                normal.Normalize();

                newVerticesList.Add(vertices[i]);
                newVerticesList.Add(vertices[i + 1]);
                newVerticesList.Add(vertices[i + 2]);
                newVerticesList.Add(normal.X);
                newVerticesList.Add(normal.Y);
                newVerticesList.Add(normal.Z);

                newVerticesList.Add(vertices[i + 3]);
                newVerticesList.Add(vertices[i + 4]);
                newVerticesList.Add(vertices[i + 5]);
                newVerticesList.Add(normal.X);
                newVerticesList.Add(normal.Y);
                newVerticesList.Add(normal.Z);

                newVerticesList.Add(vertices[i + 6]);
                newVerticesList.Add(vertices[i + 7]);
                newVerticesList.Add(vertices[i + 8]);
                newVerticesList.Add(normal.X);
                newVerticesList.Add(normal.Y);
                newVerticesList.Add(normal.Z);
            }

            return newVerticesList;
        }

        // Fires on the first frame of the game
        public void Start()
        {
            // Hide the cursor and lock it to the center of the screen
            Input.HideCursor = true;
            Input.LockCursor = true;

            // Create a new GameObject
            var cameraObject = new GameObject();
            cameraObject.transform.position = new Vector3(0, 5, -5);
            cameraObject.name = "Camera";
            // Create a camera component
            var camera = new Camera();
            // Set the app variable in Camera
            camera.app = app;
            // Add the camera component to the GameObject
            cameraObject.AddComponent(camera);
            // Add the camera component to the active scripts so the update function will work (will be refactored so you dont have to do this)
            app.activeScripts.Add(camera);

            // Create a freecam component
            var freecam = new Freecam();
            // Add the freecam component to the GameObject
            cameraObject.AddComponent(freecam);
            // Fire the Start function before anything else so execution order doesnt destroy you
            freecam.Start();
            // Add the freecam component to the active scripts so the update function will work (will be refactored so you dont have to do this)
            app.activeScripts.Add(freecam);

            
            // Skybox material
            var skyboxMat = new Material();
            skyboxMat.SetTexture("skybox.png");
            skyboxMat.shader = Shader.textureShader;

            // Skybox object
            var skybox = new GameObject();
            
            // Skybox renderer
            var skyboxRenderer = new Renderer();
            skyboxRenderer.SetOBJ("skyboxCube.obj", skyboxMat.texture != null);
            skyboxRenderer.material = skyboxMat;

            // Add the renderer to the skybox and scale it up  (the skybox is literally just a big cube with inverted normals and a texture)
            skybox.AddComponent(skyboxRenderer);
            skybox.transform.scale = new Vector3(100, 100, 100);
            


            // line material
            var lineMat = new Material();
            lineMat.shader = Shader.colorShader;
            lineMat.color = Color.Blue;

            // line object
            var lineObject = new GameObject();
            lineObject.name = "Lines";
            lineObject.transform.position = new Vector3(5, 0, 5);

            // line renderer
            var line = new LineRenderer(new Vector3(-8.8f, 0.3f, -2.8f), new Vector3(-8, 3, 0));
            line.material = lineMat;
            line.useLocal = true;
            lineObject.AddComponent(line);

            
            
            // house material
            Material houseMat = new Material();
            houseMat.SetTexture("House.png");
            houseMat.shader = Shader.textureShader;

            // house gameobject
            house = new GameObject();
            house.name = "House";
            house.transform.position = new Vector3(0, 5, 0);
            house.transform.eulerAngles = new Vector3(0, 45, 0);

            // house renderer
            Renderer houseRenderer = new Renderer();
            houseRenderer.SetOBJ("house.obj", houseMat.texture != null);
            houseRenderer.material = houseMat;
            house.AddComponent(houseRenderer);

            // Rigidbody on the house for physics (no collision rn)
            var houseRB = new Rigidbody();
            houseRB.useGravity = false;
            house.AddComponent(houseRB);

            

            // terrain material
            Material terrainMat = new Material();
            terrainMat.color = Color.Green;
            terrainMat.shader = Shader.colorShader;

            // terrain gameobject
            GameObject terrain = new GameObject();
            terrain.name = "Terrain";
            terrain.transform.position.Y = -3;

            // terrain renderer
            Renderer terrainRenderer = new Renderer();
            terrainRenderer.material = terrainMat;
            terrain.AddComponent(terrainRenderer);

            vertices = new List<float>();
            triangles = new List<float>();

            int size = 50;
            terrain.transform.position.X = -size / 2;
            terrain.transform.position.Z = -size / 2;

            noise = Noise.GeneratePerlinNoise(Noise.GenerateWhiteNoise(size * 10, size * 10), 3);
            for (int x = 0; x <= size; x++)
            {
                for (int y = 0; y <= size; y++)
                {
                    AddQuadAtPosition(new Vector2(x, y));
                }
            }

            vertices = RecalculateNormals(vertices);
            terrainRenderer.vertices = vertices.ToArray();
            terrainRenderer.triangles = triangles.ToArray();;
        }


        // Fires every frame
        public void Update(FrameEventArgs e)
        {
            // If escape is pressed, exit the game
            if (Input.GetKey(Key.Escape))
            {
                app.Exit();
            }


            // Move the house up and down with Q and E
            if (Input.GetKey(Key.Q))
            {
                house.transform.position.Y -= 2f * (float)e.Time;
            }
            if (Input.GetKey(Key.E))
            {
                house.transform.position.Y += 2f * (float)e.Time;
            }
        }

    }
}
