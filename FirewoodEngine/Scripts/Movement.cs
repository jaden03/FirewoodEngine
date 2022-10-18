using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FirewoodEngine.Scripts
{
    internal class Movement : Component
    {
        bool grounded = false;
        Rigidbody rb;
        BoxCollider bc;

        float xAxis = 0;
        float yAxis = 0;

        CameraRotation camRotScript;

        public void Start()
        {
            rb = gameObject.GetComponent("Rigidbody") as Rigidbody;
            bc = gameObject.GetComponent("BoxCollider") as BoxCollider;
            camRotScript = gameObject.GetComponent("CameraRotation") as CameraRotation;
        }

        public void Update(FrameEventArgs e)
        {
            if (Input.GetKey(Key.W))
            {
                yAxis = 1;
            }
            if (Input.GetKey(Key.S))
            {
                yAxis = -1;
            }
            if (Input.GetKey(Key.A))
            {
                xAxis = 1;
            }
            if (Input.GetKey(Key.D))
            {
                xAxis = -1;
            }
            if (Input.GetKey(Key.Space))
            {
                if (grounded)
                {
                    rb.velocity = new Vector3(rb.velocity.X, 5, rb.velocity.Z) * (float)e.Time;
                    grounded = false;
                }
            }
            if (!Input.GetKey(Key.W) && !Input.GetKey(Key.S))
            {
                yAxis = 0;
            }
            if (!Input.GetKey(Key.A) && !Input.GetKey(Key.D))
            {
                xAxis = 0;
            }

            var xVel = xAxis * 7 * (float)e.Time;
            var yVel = yAxis * 7 * (float)e.Time;
            
            Vector3 forwardVector;

            forwardVector.X = (float)Math.Cos(MathHelper.DegreesToRadians(camRotScript.pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(camRotScript.yaw));
            forwardVector.Y = 0;
            forwardVector.Z = (float)Math.Cos(MathHelper.DegreesToRadians(camRotScript.pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(camRotScript.yaw));
            forwardVector = Vector3.Normalize(forwardVector);

            Vector3 rightVector;

            // take the cross between forward and up

            rightVector.X = forwardVector.Z;

            rightVector.Y = 0;

            rightVector.Z = -forwardVector.X;

            rb.velocity = new Vector3(forwardVector.X * yVel + rightVector.X * xVel, rb.velocity.Y, forwardVector.Z * yVel + rightVector.Z * xVel);

            
            bc.collisionEnter += (object sender, CollisionEventArgs args) =>
            {
                {
                    if (args.OtherBody.gameObject.name == "Terrain")
                    {
                        grounded = true;
                    }
                };
            };
        }

    }
}
