using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirewoodEngine.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTK;
using System.Drawing;
using System.Reflection;

namespace FirewoodEngine.Core
{
    using static Logging;
    static class Editor
    {
        public static Application app;
        public static GameObject selectedObject = null;
        public static bool sceneFocused = false;

        private static JArray gameObjectsArray;

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
            
            BoxCollider terrainCollider = new BoxCollider();
            terrain.AddComponent(terrainCollider);
            terrainCollider.CalculateBoundsFromMesh();
            
            var terrinRB = new Rigidbody();
            terrinRB.useGravity = false;
            terrain.AddComponent(terrinRB);




            // house gameobject
            var house = new GameObject();
            house.name = "House";
            house.transform.position = new Vector3(0, 5, 0);
            //house.transform.eulerAngles = new Vector3(0, 45, 0);

            // house material
            Material houseMat = new Material();
            houseMat.SetTexture("House.png");
            houseMat.shader = Shader.textureShader;

            // house renderer
            Renderer houseRenderer = new Renderer();
            houseRenderer.SetOBJ("house.obj", houseMat.texture != null);
            houseRenderer.material = houseMat;
            house.AddComponent(houseRenderer);

            // Rigidbody on the house for physics (no collision rn)
            var houseRB = new Rigidbody();
            houseRB.useGravity = false;
            house.AddComponent(houseRB);

            // Box collider on the house
            BoxCollider houseCollider = new BoxCollider();
            house.AddComponent(houseCollider);
            houseCollider.CalculateBoundsFromMesh();
            //houseCollider.DebugBounds();


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
            // Add the camera component to the GameObject
            cameraObjectChild.AddComponent(camera);


            // Create a freecam component
            var freecam = new Freecam();
            // Add the freecam component to the GameObject
            cameraObject.AddComponent(freecam);


            // Audio Listener \\
            var audioListener = new AudioListener();
            cameraObjectChild.AddComponent(audioListener);
            

            // line object
            var lineObject = new GameObject();
            lineObject.name = "Lines";
            lineObject.transform.position = new Vector3(5, 0, 5);

            // line material
            var lineMat = new Material();
            lineMat.shader = Shader.colorShader;
            lineMat.color = Color.Blue;

            // line renderer
            var line = new LineRenderer(new Vector3(-8.8f, 0.3f, -2.8f), new Vector3(-8, 3, 0));
            line.material = lineMat;
            line.useLocal = true;
            lineObject.AddComponent(line);
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
                    case "FirewoodEngine.Components.Renderer":
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
                    case "FirewoodEngine.Components.AudioSource":
                        var audioSource = new AudioSource(component["properties"]["path"].ToString());
                        newGameObject.AddComponent(audioSource);
                        break;
                    case "FirewoodEngine.Components.Camera":
                        var camera = new Camera();
                        camera.fov = component["properties"]["fov"].ToObject<float>();
                        camera.near = component["properties"]["near"].ToObject<float>();
                        camera.far = component["properties"]["far"].ToObject<float>();
                        
                        var backgroundColorA = component["properties"]["backgroundColor"]["a"].ToObject<int>(); 
                        var backgroundColorR = component["properties"]["backgroundColor"]["r"].ToObject<int>();
                        var backgroundColorG = component["properties"]["backgroundColor"]["g"].ToObject<int>();
                        var backgroundColorB = component["properties"]["backgroundColor"]["b"].ToObject<int>();
                        
                        camera.backgroundColor = Color.FromArgb(backgroundColorA, backgroundColorR, backgroundColorG, backgroundColorB);

                        newGameObject.AddComponent(camera);
                        app.activeCoreScripts.Add(camera);
                        break;
                    case "FirewoodEngine.Components.AudioListener":
                        var audioListener = new AudioListener();
                        newGameObject.AddComponent(audioListener);
                        audioListener.Start();
                        break;
                    case "FirewoodEngine.Components.Rigidbody":
                        var rigidbody = new Rigidbody();
                        rigidbody.mass = component["properties"]["mass"].ToObject<float>();
                        rigidbody.useGravity = component["properties"]["useGravity"].ToObject<bool>();
                        rigidbody.kinematic = component["properties"]["kinematic"].ToObject<bool>();
                        newGameObject.AddComponent(rigidbody);
                        break;
                    case "FirewoodEngine.Components.BoxCollider":
                        var boxCollider = new BoxCollider();
                        boxCollider.size = component["properties"]["size"].ToObject<Vector3>();
                        boxCollider.center = component["properties"]["center"].ToObject<Vector3>();
                        boxCollider.isTrigger = component["properties"]["isTrigger"].ToObject<bool>();
                        newGameObject.AddComponent(boxCollider);
                        break;
                    case "FirewoodEngine.Components.SphereCollider":
                        var sphereCollider = new SphereCollider();
                        sphereCollider.radius = component["properties"]["radius"].ToObject<float>();
                        sphereCollider.center = component["properties"]["center"].ToObject<Vector3>();
                        sphereCollider.isTrigger = component["properties"]["isTrigger"].ToObject<bool>();
                        newGameObject.AddComponent(sphereCollider);
                        break;
                    case "FirewoodEngine.Components.LineRenderer":
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
                        lineRenderer.useLocal = component["properties"]["useLocal"].ToObject<bool>();
                        newGameObject.AddComponent(lineRenderer);
                        break;
                    
                    default:
                        
                        // get the type
                        var typeObject = Type.GetType(type);
                        
                        Print(typeObject);
                        
                        //create an instance of the type
                        var instance = Activator.CreateInstance(typeObject);
                        
                        // get the properties
                        var properties = component["properties"];

                        var fields = typeObject.GetFields();
                        
                        foreach (var field in fields)
                        {
                            Print(field.Name);
                            var value = properties[field.Name];
                            
                            if (value != null)
                            {
                                Print(value);
                                field.SetValue(instance, value.ToObject(field.FieldType));
                            }
                        }
                            
                        newGameObject.AddComponent(instance as Component);
                        
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
            
            LineRenderer[] lineRenderers = new LineRenderer[RenderManager.lineRenderers.Count];
            RenderManager.lineRenderers.CopyTo(lineRenderers);
            
            foreach (var lineRenderer in lineRenderers)
            {
                RenderManager.RemoveRenderer(lineRenderer);
            }
            
            Component[] components = new Component[ComponentManager.components.Count];
            ComponentManager.components.CopyTo(components);
            
            foreach (var component in components)
            {
                ComponentManager.RemoveComponent(component);
            }
            
            GameObject[] gameObjects = new GameObject[GameObjectManager.gameObjects.Count];
            GameObjectManager.gameObjects.CopyTo(gameObjects);

            foreach (GameObject gameObject in gameObjects)
            {
                GameObjectManager.RemoveGameObject(gameObject);
            }
        }
        

        
        public static void LoadScene(String path)
        {
            ClearScene();
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject jsonObject = (JObject)JToken.ReadFrom(reader);

                var gameObjects = jsonObject["gameObjects"];
                
                foreach (var gameObject in gameObjects)
                {
                    LoadGameObject(gameObject, null);
                }

            }
        }
        
        public static void SaveScene(String path)
        {
            // create json object
            var currentFile = new JObject();

            // add version
            currentFile.Add("version", "0.69");

            // create gameObjects array
            gameObjectsArray = new JArray();

            // add gameObjects to json object
            currentFile.Add("gameObjects", gameObjectsArray);


            // loop through all gameObjects
            foreach (var gameObject in GameObjectManager.gameObjects)
            {
                if (gameObject.transform.parent == null)
                    SaveGameObject(gameObject, null);
            }
            
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, currentFile.ToString());
        }


        public static void SaveGameObject(GameObject gameObject, JArray parentsChildren)
        {
            JObject gameObjectJson = new JObject();

            gameObjectJson.Add("name", gameObject.name);

            JObject position = new JObject();
            position.Add("x", gameObject.transform.position.X);
            position.Add("y", gameObject.transform.position.Y);
            position.Add("z", gameObject.transform.position.Z);
            gameObjectJson.Add("position", position);

            JObject eulerAngles = new JObject();
            eulerAngles.Add("x", gameObject.transform.eulerAngles.X);
            eulerAngles.Add("y", gameObject.transform.eulerAngles.Y);
            eulerAngles.Add("z", gameObject.transform.eulerAngles.Z);
            gameObjectJson.Add("eulerAngles", eulerAngles);

            JObject scale = new JObject();
            scale.Add("x", gameObject.transform.scale.X);
            scale.Add("y", gameObject.transform.scale.Y);
            scale.Add("z", gameObject.transform.scale.Z);
            gameObjectJson.Add("scale", scale);

            JObject localPosition = new JObject();
            localPosition.Add("x", gameObject.transform.localPosition.X);
            localPosition.Add("y", gameObject.transform.localPosition.Y);
            localPosition.Add("z", gameObject.transform.localPosition.Z);
            gameObjectJson.Add("localPosition", localPosition);

            JObject localEulerAngles = new JObject();
            localEulerAngles.Add("x", gameObject.transform.localEulerAngles.X);
            localEulerAngles.Add("y", gameObject.transform.localEulerAngles.Y);
            localEulerAngles.Add("z", gameObject.transform.localEulerAngles.Z);
            gameObjectJson.Add("localEulerAngles", localEulerAngles);

            JObject localScale = new JObject();
            localScale.Add("x", gameObject.transform.localScale.X);
            localScale.Add("y", gameObject.transform.localScale.Y);
            localScale.Add("z", gameObject.transform.localScale.Z);
            gameObjectJson.Add("localScale", localScale);

            JArray components = new JArray();

            gameObjectJson.Add("components", components);

            foreach (var component in gameObject.components)
            {
                JObject componentJson = new JObject();

                componentJson.Add("type", component.GetType().FullName);

                JObject properties = new JObject();

                componentJson.Add("properties", properties);

                components.Add(componentJson);

                switch (component.GetType().FullName)
                {
                    case "FirewoodEngine.Components.Renderer":
                        Renderer renderer = (Renderer)component;

                        properties.Add("path", renderer.path);
                        properties.Add("wireframe", renderer.wireframe);

                        JObject material = new JObject();

                        properties.Add("material", material);

                        JObject color = new JObject();

                        material.Add("color", color);

                        color.Add("r", renderer.material.color.R);
                        color.Add("g", renderer.material.color.G);
                        color.Add("b", renderer.material.color.B);
                        color.Add("a", renderer.material.color.A);

                        material.Add("rgb", renderer.material.rgb);

                        material.Add("texturePath", renderer.material.texturePath);

                        JObject shader = new JObject();

                        material.Add("shader", shader);

                        shader.Add("type", renderer.material.texturePath == null ? "Color" : "Texture");

                        break;

                    case "FirewoodEngine.Components.Camera":
                        Camera camera = (Camera)component;

                        properties.Add("fov", camera.fov);
                        properties.Add("near", camera.near);
                        properties.Add("far", camera.far);

                        JObject backgroundColor = new JObject();
                        backgroundColor.Add("r", camera.backgroundColor.R);
                        backgroundColor.Add("g", camera.backgroundColor.G);
                        backgroundColor.Add("b", camera.backgroundColor.B);
                        backgroundColor.Add("a", camera.backgroundColor.A);
                        properties.Add("backgroundColor", backgroundColor);
                        break;

                    case "FirewoodEngine.Components.AudioSource":
                        AudioSource audioSource = (AudioSource)component;

                        properties.Add("path", audioSource.path);
                        properties.Add("volume", audioSource.volume);

                        break;
                    
                    case "FirewoodEngine.Components.AudioListener":
                        AudioListener audioListener = (AudioListener)component;
                        
                        break;

                    case "FirewoodEngine.Components.Rigidbody":
                        Rigidbody rigidBody = (Rigidbody)component;

                        properties.Add("mass", rigidBody.mass);
                        properties.Add("useGravity", rigidBody.useGravity);
                        properties.Add("kinematic", rigidBody.kinematic);
                        
                        break;

                    case "FirewoodEngine.Components.BoxCollider":
                        BoxCollider boxCollider = (BoxCollider)component;

                        JObject size = new JObject();
                        size.Add("x", boxCollider.size.X);
                        size.Add("y", boxCollider.size.Y);
                        size.Add("z", boxCollider.size.Z);
                        properties.Add("size", size);

                        JObject center = new JObject();
                        center.Add("x", boxCollider.center.X);
                        center.Add("y", boxCollider.center.Y);
                        center.Add("z", boxCollider.center.Z);
                        properties.Add("center", center);
                        
                        properties.Add("isTrigger", boxCollider.isTrigger);

                        break;

                    case "FirewoodEngine.Components.SphereCollider":
                        SphereCollider sphereCollider = (SphereCollider)component;

                        properties.Add("radius", sphereCollider.radius);

                        JObject center2 = new JObject();
                        center2.Add("x", sphereCollider.center.X);
                        center2.Add("y", sphereCollider.center.Y);
                        center2.Add("z", sphereCollider.center.Z);
                        properties.Add("center", center2);
                        
                        properties.Add("isTrigger", sphereCollider.isTrigger);

                        break;

                    case "FirewoodEngine.Components.LineRenderer":
                        LineRenderer lineRenderer = (LineRenderer)component;

                        JObject material2 = new JObject();

                        properties.Add("material", material2);

                        JObject color2 = new JObject();

                        material2.Add("color", color2);

                        color2.Add("r", lineRenderer.material.color.R);
                        color2.Add("g", lineRenderer.material.color.G);
                        color2.Add("b", lineRenderer.material.color.B);
                        color2.Add("a", lineRenderer.material.color.A);

                        material2.Add("rgb", lineRenderer.material.rgb);

                        JObject position1 = new JObject();
                        position1.Add("x", lineRenderer.position1.X);
                        position1.Add("y", lineRenderer.position1.Y);
                        position1.Add("z", lineRenderer.position1.Z);
                        properties.Add("position1", position1);

                        JObject position2 = new JObject();
                        position2.Add("x", lineRenderer.position2.X);
                        position2.Add("y", lineRenderer.position2.Y);
                        position2.Add("z", lineRenderer.position2.Z);
                        properties.Add("position2", position2);
                        
                        properties.Add("useLocal", lineRenderer.useLocal);

                        break;

                    default:
                        // get all of the properties
                        FieldInfo[] properties2 = component.GetType().GetFields();
 
                        // loop through all of the properties
                        foreach (FieldInfo property in properties2)
                        {
                            // get the value of the property
                            object value = property.GetValue(component);

                            // if the value is a float
                            if (value.GetType() == typeof(float))
                            {
                                // add the property to the json
                                properties.Add(property.Name, (float)value);
                            }
                            // if the value is a int
                            else if (value.GetType() == typeof(int))
                            {
                                // add the property to the json
                                properties.Add(property.Name, (int)value);
                            }
                            // if the value is a bool
                            else if (value.GetType() == typeof(bool))
                            {
                                // add the property to the json
                                properties.Add(property.Name, (bool)value);
                            }
                            // if the value is a string
                            else if (value.GetType() == typeof(string))
                            {
                                // add the property to the json
                                properties.Add(property.Name, (string)value);
                            }
                            // if the value is a vector3
                            else if (value.GetType() == typeof(Vector3))
                            {
                                // add the property to the json
                                JObject vector3 = new JObject();
                                vector3.Add("x", ((Vector3)value).X);
                                vector3.Add("y", ((Vector3)value).Y);
                                vector3.Add("z", ((Vector3)value).Z);
                                properties.Add(property.Name, vector3);
                            }
                            // if the value is a vector2
                            else if (value.GetType() == typeof(Vector2))
                            {
                                // add the property to the json
                                JObject vector2 = new JObject();
                                vector2.Add("x", ((Vector2)value).X);
                                vector2.Add("y", ((Vector2)value).Y);
                                properties.Add(property.Name, vector2);
                            }
                            // if the value is a color
                            else if (value.GetType() == typeof(Color))
                            {
                                // add the property to the json
                                JObject color3 = new JObject();
                                color3.Add("r", ((Color)value).R);
                                color3.Add("g", ((Color)value).G);
                                color3.Add("b", ((Color)value).B);
                                color3.Add("a", ((Color)value).A);
                                properties.Add(property.Name, color3);
                            }
                            // if the value is a quaternion
                            else if (value.GetType() == typeof(Quaternion))
                            {
                                // add the property to the json
                                JObject quaternion = new JObject();
                                quaternion.Add("x", ((Quaternion)value).X);
                                quaternion.Add("y", ((Quaternion)value).Y);
                                quaternion.Add("z", ((Quaternion)value).Z);
                                quaternion.Add("w", ((Quaternion)value).W);
                                properties.Add(property.Name, quaternion);
                            }
                        }
                        
                        break;
                    

                }

            }
            

            var children = new JArray();
            gameObjectJson.Add("children", children);

            if (parentsChildren == null)
                gameObjectsArray.Add(gameObjectJson);
            else
                parentsChildren.Add(gameObjectJson);

            
            foreach (Transform child in gameObject.transform.children)
            {
                SaveGameObject(child.gameObject, children);
            }
        }

    }
}
