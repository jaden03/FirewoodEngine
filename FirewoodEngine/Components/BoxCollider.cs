using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using FirewoodEngine.Core;

namespace FirewoodEngine.Componenents
{
    class BoxCollider : Component
    {
        [HideInInspector]
        public Vector3 center = Vector3.Zero;
        [HideInInspector]
        public Vector3 size = Vector3.One;
        public bool isTrigger = false;

        public BoxCollider()
        {
            linkedComponent = this;
        }
        
        public void CalculateBoundsFromMesh()
        {
            if (gameObject == null)
            {
                Logging.Error("Add the box collider to a gameobject first!");
                return;
            }

            if (gameObject.GetComponent<Renderer>() == null)
            {
                Logging.Error("No renderer found on gameobject " + gameObject.name + "!");
                return;
            }

            Renderer rend = gameObject.GetComponent<Renderer>();
            this.center = rend.offset;
            this.size = rend.bounds * gameObject.transform.scale;
        }

        public void DebugBounds()
        {
            if (gameObject == null)
            {
                Logging.Error("Add the box collider to a gameobject first!");
                return;
            }
            
            Vector3 top = (this.center + new Vector3(0, this.size.Y / 2, 0)) + gameObject.transform.position;
            Vector3 bottom = (this.center - new Vector3(0, this.size.Y / 2, 0)) + gameObject.transform.position;
            Debug.DrawLine(top, bottom, Color.Red);

            Vector3 left = (this.center - new Vector3(this.size.X / 2, 0, 0)) + gameObject.transform.position;
            Vector3 right = (this.center + new Vector3(this.size.X / 2, 0, 0)) + gameObject.transform.position;
            Debug.DrawLine(left, right, Color.Red);

            Vector3 front = (this.center + new Vector3(0, 0, this.size.Z / 2)) + gameObject.transform.position;
            Vector3 back = (this.center - new Vector3(0, 0, this.size.Z / 2)) + gameObject.transform.position;
            Debug.DrawLine(front, back, Color.Red);
        }

        public event Action<Rigidbody> triggerStay;
        public void OnTriggerStay(Rigidbody otherBody)
        {
            if (triggerStay != null)
                triggerStay(otherBody);
        }


        public event Action<Rigidbody> collisionStay;
        public void OnCollisionStay(Rigidbody otherBody)
        {
            if (collisionStay != null)
                collisionStay(otherBody);
        }

        public event Action<Rigidbody> triggerEnter;
        public void OnTriggerEnter(Rigidbody otherBody)
        {
            if (triggerEnter != null)
                triggerEnter(otherBody);
        }
        
        public event Action<Rigidbody> collisionEnter;
        public void OnCollisionEnter(Rigidbody otherBody)
        {
            if (collisionEnter != null)
                collisionEnter(otherBody);
        }

        public event Action<Rigidbody> triggerExit;
        public void OnTriggerExit(Rigidbody otherBody)
        {
            if (triggerExit != null)
                triggerExit(otherBody);
        }

        public event Action<Rigidbody> collisionExit;
        public void OnCollisionExit(Rigidbody otherBody)
        {
            if (collisionExit != null)
                collisionExit(otherBody);
        }
    }
}
