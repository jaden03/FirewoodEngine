using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine
{
    internal class SphereCollider : Component
    {
        public Vector3 center = Vector3.Zero;
        public float radius = 1f;
        public bool isTrigger = false;

        public void CalculateBoundsFromMesh()
        {
            if (gameObject == null)
            {
                Logging.Error("Add the sphere collider to a gameobject first!");
                return;
            }

            if (gameObject.GetComponent("Renderer") as Renderer == null)
            {
                Logging.Error("No renderer found on gameobject " + gameObject.name + "!");
                return;
            }

            Renderer rend = gameObject.GetComponent("Renderer") as Renderer;
            this.center = rend.offset;
            this.radius = rend.radius * gameObject.transform.scale.X;
        }

        public void DebugBounds()
        {
            if (gameObject == null)
            {
                Logging.Error("Add the sphere collider to a gameobject first!");
                return;
            }

            Renderer rend = gameObject.GetComponent("Renderer") as Renderer;

            Vector3 top = (this.center + new Vector3(0, radius, 0)) + gameObject.transform.position;
            Vector3 bottom = (this.center - new Vector3(0, radius, 0)) + gameObject.transform.position;
            Debug.DrawLine(top, bottom, Color.Red);

            Vector3 left = (this.center - new Vector3(radius, 0, 0)) + gameObject.transform.position;
            Vector3 right = (this.center + new Vector3(radius, 0, 0)) + gameObject.transform.position;
            Debug.DrawLine(left, right, Color.Red);

            Vector3 front = (this.center + new Vector3(0, 0, radius)) + gameObject.transform.position;
            Vector3 back = (this.center - new Vector3(0, 0, radius)) + gameObject.transform.position;
            Debug.DrawLine(front, back, Color.Red);
        }

        public void OnTriggerStay(Rigidbody otherBody)
        {
            
        }
    }
}
