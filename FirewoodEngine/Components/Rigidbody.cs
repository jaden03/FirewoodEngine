using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine
{
    public class Rigidbody : Component
    {
        public float mass = 1f;
        public bool useGravity = false;
        public bool kinematic = false;
        public Vector3 velocity;

        public List<Rigidbody> collidingBodies;

        public Rigidbody()
        {
            collidingBodies = new List<Rigidbody>();
            Physics.AddRigidbody(this);
        }
    }
}
