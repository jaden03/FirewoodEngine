using FirewoodEngine.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine.Core
{
    class GameObjectManager
    {
        public static List<GameObject> gameObjects;

        public static void Initialize()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("GameObjects Initialized");
            Console.ForegroundColor = ConsoleColor.White;

            gameObjects = new List<GameObject>();
        }

        public static void AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        public static void RemoveGameObject(GameObject gameObject)
        {
            gameObjects.Remove(gameObject);
        }
        
        public static void CreateEmpty()
        {
            new GameObject();
        }
        
        public static void CreateCube()
        {
            GameObject cube = new GameObject();
            cube.name = "Cube";
            
            var cubeMat = new Material();
            cubeMat.shader = Shader.colorShader;
            cubeMat.color = Color.DarkGray;
            
            var cubeRenderer = new Renderer();
            cubeRenderer.SetOBJ("cube.obj", false);
            cubeRenderer.material = cubeMat;

            cube.AddComponent(cubeRenderer);
        }
        
        public static void CreateSphere()
        {
            GameObject sphere = new GameObject();
            sphere.name = "Sphere";
            
            var sphereMat = new Material();
            sphereMat.shader = Shader.colorShader;
            sphereMat.color = Color.DarkGray;
            
            var sphereRenderer = new Renderer();
            sphereRenderer.SetOBJ("sphere.obj", false);
            sphereRenderer.material = sphereMat;

            sphere.AddComponent(sphereRenderer);
        }
        
        public static void CreatePlane()
        {
            GameObject plane = new GameObject();
            plane.name = "Plane";
            
            var planeMat = new Material();
            planeMat.shader = Shader.colorShader;
            planeMat.color = Color.DarkGray;
            
            var planeRenderer = new Renderer();
            planeRenderer.SetOBJ("plane.obj", false);
            planeRenderer.material = planeMat;

            plane.AddComponent(planeRenderer);
        }

    }
}
