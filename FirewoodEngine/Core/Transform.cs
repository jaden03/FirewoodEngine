using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine
{
    public class Transform
    {
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;

        public Transform()
        {
            position = Vector3.Zero;
            eulerAngles = Vector3.Zero;
            scale = Vector3.One;
        }

    }
}
