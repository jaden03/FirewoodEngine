using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine
{
    public class GameObject
    {
        public string name;
        public List<Component> components;
        public Transform transform;

        public GameObject()
        {
            transform = new Transform();
            components = new List<Component>();
        }

        public void AddComponent(Component component)
        {
            component.gameObject = this;
            component.transform = transform;
            components.Add(component);
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
