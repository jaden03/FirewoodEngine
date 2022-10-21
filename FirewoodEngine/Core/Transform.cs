using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine.Core
{
    using static Logging;
    public class Transform
    {
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
        
        //Example of simple Hierarchy system
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
        
        public GameObject gameObject;
        public Transform parent;
        public List<Transform> children;

        public Vector3 forward;
        public Vector3 right;
        public Vector3 up;

        public Transform()
        {
            position = Vector3.Zero;
            eulerAngles = Vector3.Zero;
            scale = Vector3.One;
            
            localPosition = Vector3.Zero;
            localEulerAngles = Vector3.Zero;
            gameObject = null;
            parent = null;
            children = new List<Transform>();
        }

        public void Update(FrameEventArgs e)
        {
            forward = Forward();
            right = Right();
            up = Up();

            foreach (Transform child in children)
            {
                child.position = position + ((child.localPosition.Z * forward) + (child.localPosition.Y * up) + (-child.localPosition.X * right));

                child.eulerAngles = eulerAngles + (child.localEulerAngles);


                // This converts a Quaternion to EulerAngles should probably put this somewhere accessible in the engine at some point \\

                //Quaternion q = Quaternion.FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z) * Quaternion.FromEulerAngles(child.localEulerAngles.X, child.localEulerAngles.Y, child.localEulerAngles.Z);

                //Vector3 angles;

                //double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
                //double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
                //angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

                //// pitch / y
                //double sinp = 2 * (q.W * q.Y - q.Z * q.X);
                //if (Math.Abs(sinp) >= 1)
                //{
                //    angles.Y = (float)Math.PI / 2 * Math.Sign(sinp);
                //}
                //else
                //{
                //    angles.Y = (float)Math.Asin(sinp);
                //}

                //// yaw / z
                //double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
                //double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
                //angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

                child.Update(e);
            }
        }
        
        public void SetParent(Transform parent)
        {
            if (parent == null)
            {
                Error("Parent is null!");
                return;
            }

            if (parent == this)
            {
                Error("Can't set parent to self!");
                return;
            }

            if (parent.children.Contains(this))
            {
                return;
            }

            if (this.parent != null)
                this.parent.RemoveChild(this);
            
            this.parent = parent;
            parent.children.Add(this);
        }

        public void AddChild(Transform child)
        {
            if (child == null)
            {
                Error("Child is null!");
                return;
            }

            if (child == this)
            {
                Error("Can't add self as child!");
                return;
            }

            if (children.Contains(child))
            {
                return;
            }

            if (child.parent != null)
                child.parent.RemoveChild(child);

            children.Add(child);
            child.parent = this;
        }

        public void RemoveChild(Transform child)
        {
            if (child == null)
            {
                return;
            }

            if (!children.Contains(child))
            {
                return;
            }

            children.Remove(child);
            child.parent = null;
        }

        public Vector3 Forward()
        {
            var rot = Quaternion.FromEulerAngles(eulerAngles);
            return rot * Vector3.UnitZ;
        }

        public Vector3 Right()
        {
            var rot = Quaternion.FromEulerAngles(eulerAngles);
            return rot * -Vector3.UnitX;
        }

        public Vector3 Up()
        {
            var rot = Quaternion.FromEulerAngles(eulerAngles);
            return rot * Vector3.UnitY;
        }

    }
}
