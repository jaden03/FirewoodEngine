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
    using static Logging;
    static class Editor
    {
        public static Application app;
        public static GameObject selectedObject = null;
        public static bool sceneFocused = false;


        public static void Initialize(Application _app)
        {
            app = _app;

            // Audio Listener \\
            //var audioListener = new AudioListener();
            //cameraObjectChild.AddComponent(audioListener);
            //audioListener.Start();
            //app.activeScripts.Add(audioListener);



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





            // Create a new GameObject
            var cameraObject = new GameObject();
            cameraObject.name = "CameraObject";
            cameraObject.transform.position = new Vector3(0, 5, -5);
            cameraObject.name = "Camera";

            var cameraObjectChild = new GameObject();
            cameraObjectChild.name = "CameraObjectChild";
            cameraObjectChild.transform.SetParent(cameraObject.transform);

            // Create a camera component
            var camera = new Camera();
            // Set the app variable in Camera
            camera.app = app;
            // Add the camera component to the GameObject
            cameraObjectChild.AddComponent(camera);
            // Add the camera component to the active scripts so the update function will work (will be refactored so you dont have to do this)
            app.activeScripts.Add(camera);

            selectedObject = cameraObjectChild;

        }


        public static void Update(FrameEventArgs e)
        {

        }




        public static void LoadGameObject(JToken gameObject, GameObject parent)
        {
            var name = gameObject["name"].ToString();
            var position = gameObject["position"].ToObject<Vector3>();
            var eulerAngles = gameObject["eulerAngles"].ToObject<Vector3>();
            var scale = gameObject["scale"].ToObject<Vector3>();

            var localPosition = gameObject["localPosition"].ToObject<Vector3>();
            var localEulerAngles = gameObject["localEulerAngles"].ToObject<Vector3>();

            var newGameObject = new GameObject();

            if (parent != null)
            {
                newGameObject.transform.SetParent(parent.transform);
            }

            newGameObject.name = name;
            newGameObject.transform.position = position;
            newGameObject.transform.eulerAngles = eulerAngles;
            newGameObject.transform.scale = scale;

            newGameObject.transform.localPosition = localPosition;
            newGameObject.transform.localEulerAngles = localEulerAngles;


            var children = gameObject["children"];

            foreach (var child in children)
            {
                LoadGameObject(child, newGameObject);
            }

            var components = gameObject["components"];

            foreach (var component in components)
            {
                var type = component["type"].ToString();

                switch (type)
                {
                    case "Renderer":
                        var shaderType = component["properties"]["material"]["shader"]["type"].ToString();

                        var shader = Shader.textureShader;
                        if (shaderType == "Color")
                            shader = Shader.colorShader;

                        var material = new Material();
                        material.shader = shader;
                        
                        var colorR = component["properties"]["material"]["color"]["r"].ToObject<int>();
                        var colorG = component["properties"]["material"]["color"]["g"].ToObject<int>();
                        var colorB = component["properties"]["material"]["color"]["b"].ToObject<int>();
                        var colorA = component["properties"]["material"]["color"]["a"].ToObject<int>();

                        material.color = Color.FromArgb(colorA, colorR, colorG, colorB);
                        
                        material.rgb = component["properties"]["material"]["rgb"].ToObject<bool>();

                        if (shaderType == "Texture")
                        {
                            material.SetTexture(component["properties"]["material"]["texturePath"].ToString());
                        }

                        var renderer = new Renderer();
                        renderer.material = material;
                        renderer.SetOBJ(component["properties"]["path"].ToString(), shaderType == "Texture");
                        newGameObject.AddComponent(renderer);
                        break;
                    case "AudioSource":
                        var audioSource = new AudioSource(component["properties"]["path"].ToString());
                        newGameObject.AddComponent(audioSource);
                        break;
                    case "Camera":
                        var camera = new Camera();
                        camera.app = app;
                        newGameObject.AddComponent(camera);
                        app.activeCoreScripts.Add(camera);
                        break;
                    case "AudioListener":
                        var audioListener = new AudioListener();
                        newGameObject.AddComponent(audioListener);
                        audioListener.Start();
                        app.activeScripts.Add(audioListener);
                        break;
                    case "Rigidbody":
                        var rigidbody = new Rigidbody();
                        rigidbody.mass = component["properties"]["mass"].ToObject<float>();
                        rigidbody.useGravity = component["properties"]["useGravity"].ToObject<bool>();
                        rigidbody.kinematic = component["properties"]["kinematic"].ToObject<bool>();
                        newGameObject.AddComponent(rigidbody);
                        break;
                    case "BoxCollider":
                        var boxCollider = new BoxCollider();
                        boxCollider.size = component["properties"]["size"].ToObject<Vector3>();
                        boxCollider.center = component["properties"]["center"].ToObject<Vector3>();
                        boxCollider.isTrigger = component["properties"]["isTrigger"].ToObject<bool>();
                        newGameObject.AddComponent(boxCollider);
                        break;
                    case "SphereCollider":
                        var sphereCollider = new SphereCollider();
                        sphereCollider.radius = component["properties"]["radius"].ToObject<float>();
                        sphereCollider.center = component["properties"]["center"].ToObject<Vector3>();
                        sphereCollider.isTrigger = component["properties"]["isTrigger"].ToObject<bool>();
                        newGameObject.AddComponent(sphereCollider);
                        break;
                    case "LineRenderer":
                        var lineRenderer = new LineRenderer();
                        lineRenderer.material = new Material();

                        var lineColorR = component["properties"]["material"]["color"]["r"].ToObject<int>();
                        var lineColorG = component["properties"]["material"]["color"]["g"].ToObject<int>();
                        var lineColorB = component["properties"]["material"]["color"]["b"].ToObject<int>();
                        var lineColorA = component["properties"]["material"]["color"]["a"].ToObject<int>();

                        lineRenderer.material.color = Color.FromArgb(lineColorA, lineColorR, lineColorG, lineColorB);

                        lineRenderer.material.shader = Shader.colorShader;
                        lineRenderer.material.rgb = component["properties"]["material"]["rgb"].ToObject<bool>();
                        lineRenderer.position1 = component["properties"]["position1"].ToObject<Vector3>();
                        lineRenderer.position2 = component["properties"]["position2"].ToObject<Vector3>();
                        newGameObject.AddComponent(lineRenderer);
                        break;

                }
            }
        }


        public static void ClearScene()
        {
            Renderer[] renderers = new Renderer[RenderManager.renderers.Count];
            RenderManager.renderers.CopyTo(renderers);

            foreach (var renderer in renderers)
            {
                RenderManager.RemoveRenderer(renderer);
            }


            GameObject[] gameObjects = new GameObject[GameObjectManager.gameObjects.Count];
            GameObjectManager.gameObjects.CopyTo(gameObjects);

            foreach (GameObject gameObject in gameObjects)
            {
                GameObjectManager.RemoveGameObject(gameObject);
            }
            app.activeScripts.Clear();
        }
        

        
        public static void LoadScene(String path)
        {
            Console.WriteLine("Loading scene: " + path);

            ClearScene();
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject jsonObject = (JObject)JToken.ReadFrom(reader);

                var gameObjects = jsonObject["gameObjects"];

                Print(GameObjectManager.gameObjects.Count);

                foreach (var gameObject in gameObjects)
                {
                    LoadGameObject(gameObject, null);
                }

                Print(GameObjectManager.gameObjects.Count);
            }
        }

        
    }
}
