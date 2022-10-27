using FirewoodEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine.Components
{
    using static Logging;
    internal class AudioListener : Component
    {
        private Vector3 lastPos = Vector3.Zero;

        public void Start()
        {
            AudioManager.SetListener(this);
        }

        public AudioListener()
        {
            linkedComponent = this;
        }
        
        public void Update(FrameEventArgs e)
        {
            var velocity = (gameObject.transform.position - lastPos) / (float)e.Time;
            
            AudioManager.UpdateListener(transform.position, velocity, transform.forward, transform.up);
            
            lastPos = transform.position;
        }

    }
}
