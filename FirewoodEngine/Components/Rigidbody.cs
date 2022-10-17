using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine
{
    class Rigidbody : Component
    {
        public float mass = 1f;
        public bool useGravity = true;
        public bool kinematic = false;

        public Rigidbody()
        {
            Physics.AddRigidbody(this);
        }
    }
}
