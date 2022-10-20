using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine.Core
{
    public class GameObject
    {
        public string name;
        public List<Component> components;
        public Transform transform;

        public GameObject()
        {
            transform = new Transform();
            transform.gameObject = this;
            components = new List<Component>();
            GameObjectManager.AddGameObject(this);
        }

        public void AddComponent(Component component)
        {
            component.gameObject = this;
            component.transform = transform;
            components.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);
        }
        
        public T GetComponent<T>()
        {
            foreach (object component in components)
            {
                if (component.GetType() == typeof(T))
                {
                    return (T)component;
                }
            }

            return default(T);
        }

    }
}
