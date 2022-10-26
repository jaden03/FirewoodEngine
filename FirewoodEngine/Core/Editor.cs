using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirewoodEngine.Componenents;
using FirewoodEngine.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK;
using System.Drawing;

namespace FirewoodEngine.Core
{
    static class Editor
    {
        public static Application app;
        public static GameObject selectedObject = null;


        public static void Initialize(Application _app)
        {
            app = _app;

            // Create a new GameObject
            var cameraObject = new GameObject();
            cameraObject.transform.position = new Vector3(0, 5, -5);
            cameraObject.name = "Camera";

            var cameraObjectChild = new GameObject();
            cameraObjectChild.transform.SetParent(cameraObject.transform);

            // Create a camera component
            var camera = new Camera();
            // Set the app variable in Camera
            camera.app = app;
            // Add the camera component to the GameObject
            cameraObjectChild.AddComponent(camera);
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


            // Audio Listener \\
            var audioListener = new AudioListener();
            cameraObjectChild.AddComponent(audioListener);
            audioListener.Start();
            app.activeScripts.Add(audioListener);



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
            terrainMat.color = Color.Green;
            terrainMat.shader = Shader.colorShader;

            // terrain gameobject
            GameObject terrain = new GameObject();
            terrain.name = "Terrain";
            terrain.transform.scale = new Vector3(.25f, .25f, .25f);

            // terrain renderer
            Renderer terrainRenderer = new Renderer();
            terrainRenderer.SetOBJ("flatGround.obj", false);
            terrainRenderer.material = terrainMat;
            terrain.AddComponent(terrainRenderer);

        }


        public static void Update(FrameEventArgs e)
        {

        }
            


        public static void LoadScene(String path)
        {
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject jsonObject = (JObject)JToken.ReadFrom(reader);

                Console.WriteLine(jsonObject.ToString());

                Console.WriteLine(jsonObject["scene"][0]["name"]);
            }
        }

        
    }
}
