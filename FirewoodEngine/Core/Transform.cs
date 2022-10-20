using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine.Core
{
    public class Transform
    {
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
        
        //Example of simple Hierarchy system
        public Vector3 localPosition;
        public Vector3 localRotation;
        public Vector3 localScale;
        public GameObject gameObject;
        public GameObject parent;
        public GameObject[] children;

        public Transform()
        {
            position = Vector3.Zero;
            eulerAngles = Vector3.Zero;
            scale = Vector3.One;
            
            localPosition = Vector3.Zero;
            localRotation = Vector3.Zero;
            localScale = Vector3.One;
            gameObject = null;
            parent = null;
            children = null;
        }
        
    }
}
