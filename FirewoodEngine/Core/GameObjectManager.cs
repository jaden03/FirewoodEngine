using FirewoodEngine.Componenents;
using System;
using System.Collections.Generic;
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

    }
}
