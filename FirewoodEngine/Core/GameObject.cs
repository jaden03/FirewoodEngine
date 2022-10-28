using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using FirewoodEngine.Components;

namespace FirewoodEngine.Core
{
    public class GameObject
    {
        public string name = "GameObject";
        public string tag = "Untagged";
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

        public void AddComponent(Type type)
        {
            Component component = (Component)Activator.CreateInstance(type);
            component.gameObject = this;
            component.transform = transform;
            components.Add(component);
        }
        
        public Component AddComponent<T>() where T : Component
        {
            Component component = (Component)Activator.CreateInstance(typeof(T));
            component.gameObject = this;
            component.transform = transform;
            components.Add(component);
            return component;
        }


        public void RemoveComponent(Component component)
        {
            if (component is Renderer)
            {
                RenderManager.RemoveRenderer((Renderer)component);
            }
            else if (component is LineRenderer)
            {
                RenderManager.RemoveRenderer((LineRenderer)component);
            }
            
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
