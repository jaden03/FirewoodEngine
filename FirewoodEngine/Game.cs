using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Input;
using FirewoodEngine.Scripts;

namespace FirewoodEngine
{
    using static Logging;
    class Game
    {
        public Application app;
        
        GameObject house;

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

            // give the camera object physics and a box collider
            var cameraRB = new Rigidbody();
            cameraRB.useGravity = true;

            cameraObject.AddComponent(cameraRB);

            cameraObject.transform.scale = new Vector3(.3f, 1, .3f);

            var cameraCollider = new BoxCollider();
            cameraObject.AddComponent(cameraCollider);
            cameraCollider.size = new Vector3(.3f, 1, .3f);
            cameraCollider.center = new Vector3(0, -.3f, 0);
            //cameraCollider.DebugBounds();

            var cameraRotation = new CameraRotation();
            cameraObject.AddComponent(cameraRotation);
            cameraRotation.Start();
            app.activeScripts.Add(cameraRotation);

            var movement = new Movement();
            cameraObject.AddComponent(movement);
            movement.Start();
            app.activeScripts.Add(movement);



            //// Create a freecam component
            //var freecam = new Freecam();
            //// Add the freecam component to the GameObject
            //cameraObject.AddComponent(freecam);
            //// Fire the Start function before anything else so execution order doesnt destroy you
            //freecam.Start();
            //// Add the freecam component to the active scripts so the update function will work (will be refactored so you dont have to do this)
            //app.activeScripts.Add(freecam);


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


            // terrain material
            Material terrainMat = new Material();
            terrainMat.shader = Shader.textureShader;
            terrainMat.SetTexture("flatGround.png");

            // terrain gameobject
            GameObject terrain = new GameObject();
            terrain.name = "Terrain";
            terrain.transform.position.Y = -1.5f;
            
            // terrain renderer
            Renderer terrainRenderer = new Renderer();
            terrainRenderer.SetOBJ("flatGround.obj", true);
            terrainRenderer.material = terrainMat;
            terrain.AddComponent(terrainRenderer);

            // terrain rigidbody
            var terrainRB = new Rigidbody();
            terrainRB.useGravity = false;
            terrain.AddComponent(terrainRB);

            // terrain collider
            var terrainCollider = new BoxCollider();
            terrain.AddComponent(terrainCollider);
            terrainCollider.CalculateBoundsFromMesh();
        }

        // Fires every frame
        public void Update(FrameEventArgs e)
        {
            // If escape is pressed, exit the game
            if (Input.GetKey(Key.Escape))
            {
                app.Exit();
            }


            
        }

    }
}
