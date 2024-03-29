﻿using System;
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
        public Quaternion rotation;
        public Vector3 eulerAngles;
        public Vector3 scale;
        
        public Vector3 localPosition;
        public Quaternion localRotation;
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
            rotation = Quaternion.Identity;
            eulerAngles = Vector3.Zero;
            scale = Vector3.One;
            
            localPosition = Vector3.Zero;
            localRotation = Quaternion.Identity;
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

            if (parent != null)
            {
                rotation = parent.rotation * localRotation;
            }
            else
            {
                rotation = Quaternion.FromEulerAngles(eulerAngles);
            }
            localRotation = Quaternion.FromEulerAngles(localEulerAngles);
            
            foreach (Transform child in children)
            {
                child.position = position + ((child.localPosition.Z * forward) + (child.localPosition.Y * up) + (-child.localPosition.X * right));
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
        {;
            return rotation * Vector3.UnitZ;
        }

        public Vector3 Right()
        {
            return rotation * -Vector3.UnitX;
        }

        public Vector3 Up()
        {
            return rotation * Vector3.UnitY;
        }

    }
}
