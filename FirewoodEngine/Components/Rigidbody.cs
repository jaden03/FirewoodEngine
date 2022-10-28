using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirewoodEngine.Core;

namespace FirewoodEngine.Components
{
    public class Rigidbody : Component
    {
        public float mass = 1f;
        public bool useGravity = false;
        public bool kinematic = false;
        [ReadOnly]
        public Vector3 velocity;

        public List<Rigidbody> collidingBodies;

        public Rigidbody()
        {
            collidingBodies = new List<Rigidbody>();
            Physics.AddRigidbody(this);
        }
    }
}
