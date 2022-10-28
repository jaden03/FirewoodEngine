using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine.Core
{
    public class Component
    {
        public GameObject gameObject;
        public Transform transform;
        public object linkedComponent;

        public Component()
        {
            ComponentManager.AddComponent(this);
        }
        
        public virtual void Update(FrameEventArgs e) {}
    }
}
