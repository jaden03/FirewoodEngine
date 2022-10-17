using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine
{
    internal class GameObject
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

        public Component GetComponent(string _type)
        {
            foreach (Component component in components)
            {
                if (component.GetType().Name == _type)
                {
                    return component;
                }
            }
            return null;
        }

    }
}
