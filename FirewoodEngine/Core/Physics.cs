using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using FirewoodEngine.Componenents;

namespace FirewoodEngine.Core
{
    using static Logging;
    class Physics
    {
        static Application app;
        public static List<Rigidbody> rbs;

        public static void Initialize(Application _app)
        {
            app = _app;
            
            rbs = new List<Rigidbody>();
            
            Console.WriteLine("Physics Initialized");
        }

        public static void AddRigidbody(Rigidbody rb)
        {
            rbs.Add(rb);
        }

        public static void RemoveRigidbody(Rigidbody rb)
        {
            rbs.Remove(rb);
        }

        public static void Update(FrameEventArgs e)
        {
            if (!app.isPlaying || app.isPaused) { return; }
            
            foreach (Rigidbody rb in rbs)
            {
                var lastPos = rb.gameObject.transform.position;
                
                rb.gameObject.transform.position += rb.velocity;
                if (rb.useGravity)
                {
                    rb.velocity -= new Vector3(0, .1f, 0) * (float)e.Time;
                }

                var pos = rb.gameObject.transform.position;


                float radius = 1;

                if (rb.gameObject.GetComponent<BoxCollider>() != null)
                {
                    float largestColliderAxisValue = 0;
                    BoxCollider thisBoxCollider = rb.gameObject.GetComponent<BoxCollider>();
                    if (thisBoxCollider.size.X > largestColliderAxisValue)
                    {
                        largestColliderAxisValue = thisBoxCollider.size.X;
                    }
                    if (thisBoxCollider.size.Y > largestColliderAxisValue)
                    {
                        largestColliderAxisValue = thisBoxCollider.size.Y;
                    }
                    if (thisBoxCollider.size.Z > largestColliderAxisValue)
                    {
                        largestColliderAxisValue = thisBoxCollider.size.Z;
                    }
                    radius = largestColliderAxisValue / 2;


                    float largestAxisValue = 0;
                    if (rb.gameObject.transform.scale.X > largestAxisValue)
                    {
                        largestAxisValue = rb.gameObject.transform.scale.X;
                    }
                    if (rb.gameObject.transform.scale.Y > largestAxisValue)
                    {
                        largestAxisValue = rb.gameObject.transform.scale.Y;
                    }
                    if (rb.gameObject.transform.scale.Z > largestAxisValue)
                    {
                        largestAxisValue = rb.gameObject.transform.scale.Z;
                    }
                    radius *= largestAxisValue;
                }
                else if (rb.gameObject.GetComponent<SphereCollider>() != null)
                {
                    radius = rb.gameObject.GetComponent<SphereCollider>().radius;
                }


                foreach (Rigidbody rb2 in rbs)
                {
                    if (rb2 != rb)
                    {
                        var pos2 = rb2.gameObject.transform.position;
                        
                        
                        float radius2 = 1;

                        if (rb2.gameObject.GetComponent<BoxCollider>() != null)
                        {
                            float largestColliderAxisValue2 = 0;
                            BoxCollider thisBoxCollider2 = rb2.gameObject.GetComponent<BoxCollider>();
                            if (thisBoxCollider2.size.X > largestColliderAxisValue2)
                            {
                                largestColliderAxisValue2 = thisBoxCollider2.size.X;
                            }
                            if (thisBoxCollider2.size.Y > largestColliderAxisValue2)
                            {
                                largestColliderAxisValue2 = thisBoxCollider2.size.Y;
                            }
                            if (thisBoxCollider2.size.Z > largestColliderAxisValue2)
                            {
                                largestColliderAxisValue2 = thisBoxCollider2.size.Z;
                            }
                            radius = largestColliderAxisValue2 / 2;


                            float largestAxisValue2 = 0;
                            if (rb2.gameObject.transform.scale.X > largestAxisValue2)
                            {
                                largestAxisValue2 = rb2.gameObject.transform.scale.X;
                            }
                            if (rb2.gameObject.transform.scale.Y > largestAxisValue2)
                            {
                                largestAxisValue2 = rb2.gameObject.transform.scale.Y;
                            }
                            if (rb2.gameObject.transform.scale.Z > largestAxisValue2)
                            {
                                largestAxisValue2 = rb2.gameObject.transform.scale.Z;
                            }
                            radius2 *= largestAxisValue2;
                        }
                        else if (rb2.gameObject.GetComponent<SphereCollider>() != null)
                        {
                            radius2 = rb2.gameObject.GetComponent<SphereCollider>().radius;
                        }


                        // If the two objects are close enough to collide
                        var distance = Vector3.Distance(pos, pos2);
                        var largerRadius = radius > radius2 ? radius : radius2;

                        if (distance < largerRadius)
                        {

                            // AABB AABB Check \\
                            if (rb.gameObject.GetComponent<BoxCollider>() != null && rb2.gameObject.GetComponent<BoxCollider>() != null)
                            {
                                // Get the box collider
                                var bc = rb.gameObject.GetComponent<BoxCollider>();
                                var bc2 = rb2.gameObject.GetComponent<BoxCollider>();

                                // Get the size of the box colider
                                var size = bc.size;
                                var size2 = bc2.size;

                                // Get the center of the box collider (used for offsetting the colliders position)
                                var center = bc.center;
                                var center2 = bc2.center;

                                // Determine the min and max positions in the world that the box collider takes up
                                var min = pos + center - (size / 2);
                                var max = pos + center + (size / 2);
                                var min2 = pos2 + center2 - (size2 / 2);
                                var max2 = pos2 + center2 + (size2 / 2);

                                // Check if the two box colliders are colliding
                                if (min.X < max2.X && max.X > min2.X &&
                                    min.Y < max2.Y && max.Y > min2.Y &&
                                    min.Z < max2.Z && max.Z > min2.Z)
                                {
                                    // Check if the rigidbody is in the list of colliding bodies
                                    if (!rb.collidingBodies.Contains(rb2))
                                    {
                                        // Add the rigidbody to the list of colliding bodies
                                        rb.collidingBodies.Add(rb2);

                                        // If both colliders are not triggers, fire OnCollisionEnter
                                        if (!bc.isTrigger && !bc2.isTrigger)
                                        {
                                            bc.OnCollisionEnter(rb2);
                                        }
                                        else
                                        {
                                            // If one of the colliders is a trigger, fire OnTriggerEnter
                                            bc.OnTriggerEnter(rb2);
                                        }
                                    }
                                    if (!rb2.collidingBodies.Contains(rb))
                                    {
                                        rb2.collidingBodies.Add(rb);

                                        if (!bc.isTrigger && !bc2.isTrigger)
                                        {
                                            bc2.OnCollisionEnter(rb);
                                        }
                                        else
                                        {
                                            bc2.OnTriggerEnter(rb);
                                        }
                                    }


                                    //Warn("Collision Detected!" + e.Time);

                                    // If both colliders are not triggers
                                    if (bc.isTrigger == false && bc2.isTrigger == false)
                                    {
                                        // If they are colliding, move the object back to its last position
                                        rb.velocity = Vector3.Zero;
                                        rb.gameObject.transform.position = lastPos;
                                        
                                        // If the two objects are colliding, call the OnCollisionStay event
                                        bc.OnCollisionStay(rb2);
                                        bc2.OnCollisionStay(rb);
                                    }
                                    else
                                    {
                                        // If the two objects are colliding, call the OnTriggerStay event
                                        bc.OnTriggerStay(rb2);
                                        bc2.OnTriggerStay(rb);
                                    }
                                }
                                else
                                {
                                    // If the two objects are in the list of colliding bodies, remove them
                                    if (rb.collidingBodies.Contains(rb2))
                                    {
                                        rb.collidingBodies.Remove(rb2);

                                        // If both colliders are not triggers, fire OnCollisionExit
                                        if (!bc.isTrigger && !bc2.isTrigger)
                                        {
                                            bc.OnCollisionExit(rb2);
                                        }
                                        else
                                        {
                                            // If one of the colliders is a trigger, fire OnTriggerExit
                                            bc.OnTriggerExit(rb2);
                                        }
                                    }
                                    if (rb2.collidingBodies.Contains(rb))
                                    {
                                        rb2.collidingBodies.Remove(rb);

                                        if (!bc.isTrigger && !bc2.isTrigger)
                                        {
                                            bc2.OnCollisionExit(rb);
                                        }
                                        else
                                        {
                                            bc2.OnTriggerExit(rb);
                                        }
                                    }
                                }
                            }

                            // Sphere Sphere Check \\
                            if (rb.gameObject.GetComponent<SphereCollider>() != null && rb2.gameObject.GetComponent<SphereCollider>() != null)
                            {
                                // Get the sphere collider
                                var sc = rb.gameObject.GetComponent<SphereCollider>();
                                var sc2 = rb2.gameObject.GetComponent<SphereCollider>();

                                // Get the radius of the sphere collider
                                var radius3 = sc.radius;
                                var radius4 = sc2.radius;

                                // Check if the two sphere colliders are colliding
                                if (distance < radius3 + radius4)
                                {
                                    // Check if the rigidbody is in the list of colliding bodies
                                    if (!rb.collidingBodies.Contains(rb2))
                                    {
                                        // Add the rigidbody to the list of colliding bodies
                                        rb.collidingBodies.Add(rb2);

                                        // If both colliders are not triggers, fire OnCollisionEnter
                                        if (!sc.isTrigger && !sc2.isTrigger)
                                        {
                                            sc.OnCollisionEnter(rb2);
                                        }
                                        else
                                        {
                                            // If one of the colliders is a trigger, fire OnTriggerEnter
                                            sc.OnTriggerEnter(rb2);
                                        }
                                    }
                                    if (!rb2.collidingBodies.Contains(rb))
                                    {
                                        rb2.collidingBodies.Add(rb);

                                        if (!sc.isTrigger && !sc2.isTrigger)
                                        {
                                            sc2.OnCollisionEnter(rb);
                                        }
                                        else
                                        {
                                            sc2.OnTriggerEnter(rb);
                                        }
                                    }

                                    //Warn("Collision Detected!" + e.Time);

                                    // If both colliders are not triggers
                                    if (sc.isTrigger == false && sc2.isTrigger == false)
                                    {
                                        // If they are colliding, move the object back to its last position
                                        rb.velocity = Vector3.Zero;
                                        rb.gameObject.transform.position = lastPos;

                                        // If the two objects are colliding, call the OnCollisionStay event
                                        sc.OnCollisionStay(rb2);
                                        sc2.OnCollisionStay(rb);
                                    }
                                    else
                                    {
                                        // If the two objects are colliding, call the OnTriggerStay event
                                        sc.OnTriggerStay(rb2);
                                        sc2.OnTriggerStay(rb);
                                    }
                                }
                                else
                                {
                                    // If the two objects are in the list of colliding bodies, remove them
                                    if (rb.collidingBodies.Contains(rb2))
                                    {
                                        rb.collidingBodies.Remove(rb2);

                                        // If both colliders are not triggers, fire OnCollisionExit
                                        if (!sc.isTrigger && !sc2.isTrigger)
                                        {
                                            sc.OnCollisionExit(rb2);
                                        }
                                        else
                                        {
                                            // If one of the colliders is a trigger, fire OnTriggerExit
                                            sc.OnTriggerExit(rb2);
                                        }
                                    }
                                    if (rb2.collidingBodies.Contains(rb))
                                    {
                                        rb2.collidingBodies.Remove(rb);

                                        if (!sc.isTrigger && !sc2.isTrigger)
                                        {
                                            sc2.OnCollisionExit(rb);
                                        }
                                        else
                                        {
                                            sc2.OnTriggerExit(rb);
                                        }
                                    }
                                }
                            }

                            // Box Sphere Check \\
                            if (rb.gameObject.GetComponent<BoxCollider>() != null && rb2.gameObject.GetComponent<SphereCollider>() != null)
                            {
                                // Get the box collider
                                var bc = rb.gameObject.GetComponent<BoxCollider>();
                                var sc = rb2.gameObject.GetComponent<SphereCollider>();

                                // Get the size of the box colider
                                var size = bc.size;

                                // Get the center of the box collider (used for offsetting the colliders position)
                                var center = bc.center;

                                // Determine the min and max positions in the world that the box collider takes up
                                var min = pos + center - (size / 2);
                                var max = pos + center + (size / 2);

                                // Get the radius of the sphere collider
                                var radius3 = sc.radius;

                                // Get the closest point on the box collider to the sphere collider
                                var closestPoint = new Vector3(
                                    Math.Max(min.X, Math.Min(pos2.X, max.X)),
                                    Math.Max(min.Y, Math.Min(pos2.Y, max.Y)),
                                    Math.Max(min.Z, Math.Min(pos2.Z, max.Z))
                                );

                                // Check if the closest point is within the radius of the sphere collider
                                if (Vector3.Distance(pos2, closestPoint) < radius3)
                                {
                                    // Check if the rigidbody is in the list of colliding bodies
                                    if (!rb.collidingBodies.Contains(rb2))
                                    {
                                        // Add the rigidbody to the list of colliding bodies
                                        rb.collidingBodies.Add(rb2);

                                        // If both colliders are not triggers, fire OnCollisionEnter
                                        if (!bc.isTrigger && !sc.isTrigger)
                                        {
                                            bc.OnCollisionEnter(rb2);
                                        }
                                        else
                                        {
                                            // If one of the colliders is a trigger, fire OnTriggerEnter
                                            bc.OnTriggerEnter(rb2);
                                        }
                                    }
                                    if (!rb2.collidingBodies.Contains(rb))
                                    {
                                        rb2.collidingBodies.Add(rb);

                                        if (!bc.isTrigger && !sc.isTrigger)
                                        {
                                            sc.OnCollisionEnter(rb);
                                        }
                                        else
                                        {
                                            sc.OnTriggerEnter(rb);
                                        }
                                    }

                                    //Warn("Collision Detected!" + e.Time);

                                    // If both colliders are not triggers
                                    if (bc.isTrigger == false && sc.isTrigger == false)
                                    {
                                        // If they are colliding, move the object back to its last position
                                        rb.velocity = Vector3.Zero;
                                        rb.gameObject.transform.position = lastPos;

                                        // If the two objects are colliding, call the OnCollisionStay event
                                        bc.OnCollisionStay(rb2);
                                        sc.OnCollisionStay(rb);
                                    }
                                    else
                                    {
                                        // If the two objects are colliding, call the OnTriggerStay event
                                        bc.OnTriggerStay(rb2);
                                        sc.OnTriggerStay(rb);
                                    }
                                }
                                else
                                {
                                    // If the two objects are in the list of colliding bodies, remove them
                                    if (rb.collidingBodies.Contains(rb2))
                                    {
                                        rb.collidingBodies.Remove(rb2);

                                        // If both colliders are not triggers, fire OnCollisionExit
                                        if (!bc.isTrigger && !sc.isTrigger)
                                        {
                                            bc.OnCollisionExit(rb2);
                                        }
                                        else
                                        {
                                            // If one of the colliders is a trigger, fire OnTriggerExit
                                            bc.OnTriggerExit(rb2);
                                        }
                                    }
                                    if (rb2.collidingBodies.Contains(rb))
                                    {
                                        rb2.collidingBodies.Remove(rb);

                                        if (!bc.isTrigger && !sc.isTrigger)
                                        {
                                            sc.OnCollisionExit(rb);
                                        }
                                        else
                                        {
                                            sc.OnTriggerExit(rb);
                                        }
                                    }
                                }
                            }

                            // Sphere Box Check \\
                            if (rb.gameObject.GetComponent<SphereCollider>() != null && rb2.gameObject.GetComponent<SphereCollider>() != null)
                            {
                                // Get the sphere collider
                                var sc = rb.gameObject.GetComponent<SphereCollider>();
                                var bc = rb2.gameObject.GetComponent<BoxCollider>();

                                // Get the radius of the sphere collider
                                var radius3 = sc.radius;

                                // Get the size of the box colider
                                var size = bc.size;

                                // Get the center of the box collider (used for offsetting the colliders position)
                                var center = bc.center;

                                // Determine the min and max positions in the world that the box collider takes up
                                var min = pos2 + center - (size / 2);
                                var max = pos2 + center + (size / 2);

                                // Get the closest point on the box collider to the sphere collider
                                var closestPoint = new Vector3(
                                    Math.Max(min.X, Math.Min(pos.X, max.X)),
                                    Math.Max(min.Y, Math.Min(pos.Y, max.Y)),
                                    Math.Max(min.Z, Math.Min(pos.Z, max.Z))
                                );

                                // Check if the closest point is within the radius of the sphere collider
                                if (Vector3.Distance(pos, closestPoint) < radius3)
                                {
                                    // Check if the rigidbody is in the list of colliding bodies
                                    if (!rb.collidingBodies.Contains(rb2))
                                    {
                                        // Add the rigidbody to the list of colliding bodies
                                        rb.collidingBodies.Add(rb2);

                                        // If both colliders are not triggers, fire OnCollisionEnter
                                        if (!sc.isTrigger && !bc.isTrigger)
                                        {
                                            sc.OnCollisionEnter(rb2);
                                        }
                                        else
                                        {
                                            // If one of the colliders is a trigger, fire OnTriggerEnter
                                            sc.OnTriggerEnter(rb2);
                                        }
                                    }
                                    if (!rb2.collidingBodies.Contains(rb))
                                    {
                                        rb2.collidingBodies.Add(rb);

                                        if (!sc.isTrigger && !bc.isTrigger)
                                        {
                                            bc.OnCollisionEnter(rb);
                                        }
                                        else
                                        {
                                            bc.OnTriggerEnter(rb);
                                        }
                                    }

                                    //Warn("Collision Detected!" + e.Time);

                                    // If both colliders are not triggers
                                    if (sc.isTrigger == false && bc.isTrigger == false)
                                    {
                                        // If they are colliding, move the object back to its last position
                                        rb.velocity = Vector3.Zero;
                                        rb.gameObject.transform.position = lastPos;

                                        // If the two objects are colliding, call the OnCollisionStay event
                                        sc.OnCollisionStay(rb2);
                                        bc.OnCollisionStay(rb);
                                    }
                                    else
                                    {
                                        // If the two objects are colliding, call the OnTriggerStay event
                                        sc.OnTriggerStay(rb2);
                                        bc.OnTriggerStay(rb);
                                    }
                                }
                                else
                                {
                                    // If the two objects are in the list of colliding bodies, remove them
                                    if (rb.collidingBodies.Contains(rb2))
                                    {
                                        rb.collidingBodies.Remove(rb2);

                                        // If both colliders are not triggers, fire OnCollisionExit
                                        if (!sc.isTrigger && !bc.isTrigger)
                                        {
                                            sc.OnCollisionExit(rb2);
                                        }
                                        else
                                        {
                                            // If one of the colliders is a trigger, fire OnTriggerExit
                                            sc.OnTriggerExit(rb2);
                                        }
                                    }
                                    if (rb2.collidingBodies.Contains(rb))
                                    {
                                        rb2.collidingBodies.Remove(rb);

                                        if (!sc.isTrigger && !bc.isTrigger)
                                        {
                                            bc.OnCollisionExit(rb);
                                        }
                                        else
                                        {
                                            bc.OnTriggerExit(rb);
                                        }
                                    }
                                }
                            }
                        }
                        //else
                        //{
                        //    Warn(rb.gameObject.name + " is too far away from " + rb2.gameObject.name + " to bother checking collision!");
                        //}








                        //var rend = rb.gameObject.GetComponent("Renderer") as Renderer;
                        //var rend2 = rb2.gameObject.GetComponent("Renderer") as Renderer;

                        //for (int i = 0; i < rend.triangles.Length; i += 9)
                        //{
                        //    // Triangle 1

                        //    var p1 = new Vector3(rend.triangles[i], rend.triangles[i + 1], rend.triangles[i + 2]);
                        //    var p2 = new Vector3(rend.triangles[i + 3], rend.triangles[i + 4], rend.triangles[i + 5]);
                        //    var p3 = new Vector3(rend.triangles[i + 6], rend.triangles[i + 7], rend.triangles[i + 8]);

                        //    //if (rend.name == "Green Cube")
                        //    //    Console.WriteLine("BEFORE: " + p1);

                        //    //if (rend.name == "Green Cube")
                        //    //    Console.WriteLine("EULER: " + rend.eulerAngles);

                        //    var P1Matrix = Matrix4.CreateScale(rend.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.transform.eulerAngles.X, rend.transform.eulerAngles.Y, rend.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p1.X, p1.Y, p1.Z) * Matrix4.CreateTranslation(rend.transform.position.X, rend.transform.position.Y, rend.transform.position.Z);
                        //    var P2Matrix = Matrix4.CreateScale(rend.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.transform.eulerAngles.X, rend.transform.eulerAngles.Y, rend.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p2.X, p2.Y, p2.Z) * Matrix4.CreateTranslation(rend.transform.position.X, rend.transform.position.Y, rend.transform.position.Z);
                        //    var P3Matrix = Matrix4.CreateScale(rend.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.transform.eulerAngles.X, rend.transform.eulerAngles.Y, rend.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p3.X, p3.Y, p3.Z) * Matrix4.CreateTranslation(rend.transform.position.X, rend.transform.position.Y, rend.transform.position.Z);


                        //    //if (rend.name == "Green Cube")
                        //    //    Console.WriteLine("AFTER_ROT: " + P1Matrix.ExtractRotation());


                        //    p1 = P1Matrix.ExtractTranslation();
                        //    p2 = P2Matrix.ExtractTranslation();
                        //    p3 = P3Matrix.ExtractTranslation();

                        //    //if (rend.name == "Green Cube")
                        //    //    Console.WriteLine("AFTER: " + p1);

                        //    //p1 += rend.position;
                        //    //p2 += rend.position;
                        //    //p3 += rend.position;

                        //    var A = p2 - p1;
                        //    var B = p3 - p1;
                        //    var triangleNormalX = A.Y * B.Z - A.Z * B.Y;
                        //    var triangleNormalY = A.Z * B.X - A.X * B.Z;
                        //    var triangleNormalZ = A.X * B.Y - A.Y * B.X;
                        //    var triangleNormal = new Vector3(triangleNormalX, triangleNormalY, triangleNormalZ);

                        //    for (int z = 0; z < rend2.triangles.Length; z += 9)
                        //    {
                        //        // p1 p2 p3        A = p2 - p1           B = p3 - p1       N = A x B

                        //        // Triangle 2
                        //        var p1_2 = new Vector3(rend2.triangles[z], rend2.triangles[z + 1], rend2.triangles[z + 2]);
                        //        var p2_2 = new Vector3(rend2.triangles[z + 3], rend2.triangles[z + 4], rend2.triangles[z + 5]);
                        //        var p3_2 = new Vector3(rend2.triangles[z + 6], rend2.triangles[z + 7], rend2.triangles[z + 8]);

                        //        //Console.WriteLine(p1_2);
                        //        //Console.WriteLine(p2_2);
                        //        //Console.WriteLine(p3_2);

                        //        var P1Matrix_2 = Matrix4.CreateScale(rend2.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.transform.eulerAngles.X, rend2.transform.eulerAngles.Y, rend2.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p1_2.X, p1_2.Y, p1_2.Z) * Matrix4.CreateTranslation(rend2.transform.position.X, rend2.transform.position.Y, rend2.transform.position.Z);
                        //        var P2Matrix_2 = Matrix4.CreateScale(rend2.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.transform.eulerAngles.X, rend2.transform.eulerAngles.Y, rend2.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p2_2.X, p2_2.Y, p2_2.Z) * Matrix4.CreateTranslation(rend2.transform.position.X, rend2.transform.position.Y, rend2.transform.position.Z); ;
                        //        var P3Matrix_2 = Matrix4.CreateScale(rend2.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.transform.eulerAngles.X, rend2.transform.eulerAngles.Y, rend2.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p3_2.X, p3_2.Y, p3_2.Z) * Matrix4.CreateTranslation(rend2.transform.position.X, rend2.transform.position.Y, rend2.transform.position.Z); ;


                        //        p1_2 = P1Matrix_2.ExtractTranslation();
                        //        p2_2 = P2Matrix_2.ExtractTranslation();
                        //        p3_2 = P3Matrix_2.ExtractTranslation();

                        //        //p1_2 += rend2.position;
                        //        //p2_2 += rend2.position;
                        //        //p3_2 += rend2.position;


                        //        A = p2_2 - p1_2;
                        //        B = p3_2 - p1_2;
                        //        triangleNormalX = A.Y * B.Z - A.Z * B.Y;
                        //        triangleNormalY = A.Z * B.X - A.X * B.Z;
                        //        triangleNormalZ = A.X * B.Y - A.Y * B.X;
                        //        var triangleNormal2 = new Vector3(triangleNormalX, triangleNormalY, triangleNormalZ);

                        //        // Check if second triangle intersects first triangle 

                        //        var dir = (p1_2 - p2_2);
                        //        var dotNumerator = Vector3.Dot((p1 - p1_2), triangleNormal);
                        //        var dotDenominator = Vector3.Dot(dir, triangleNormal);

                        //        var length = dotNumerator / dotDenominator;
                        //        var direction = dir * length;
                        //        var impactPos = p1_2 + direction;

                        //        var impactDistance = Vector3.Distance(p1_2, impactPos);
                        //        var lineDistance = Vector3.Distance(p1_2, p2_2);

                        //        var directionToImpact = (p1_2 - impactPos).Normalized();
                        //        var directionOfLine = (p1_2 - p2_2).Normalized();

                        //        var edge1 = p2 - p1;
                        //        var edge2 = p3 - p2;
                        //        var edge3 = p1 - p3;

                        //        var c1 = impactPos - p1;
                        //        var c2 = impactPos - p2;
                        //        var c3 = impactPos - p3;



                        //        //var dir2 = (p2_2 - p3_2);
                        //        //var dotNumerator2 = Vector3.Dot((p1 - p2_2), triangleNormal);
                        //        //var dotDenominator2 = Vector3.Dot(dir2, triangleNormal);

                        //        //var length2 = dotNumerator2 / dotDenominator2;
                        //        //var direction2 = dir2 * length2;
                        //        //var impactPos2 = p2_2 + direction2;

                        //        //var impactDistance2 = Vector3.Distance(p2_2, impactPos2);
                        //        //var lineDistance2 = Vector3.Distance(p2_2, p3_2);

                        //        //var edge1_2 = p2 - p1;
                        //        //var edge2_2 = p3 - p2;
                        //        //var edge3_2 = p1 - p3;

                        //        //var c1_2 = impactPos2 - p1;
                        //        //var c2_2 = impactPos2 - p2;
                        //        //var c3_2 = impactPos2 - p3;



                        //        //var dir3 = (p1_2 - p3_2);
                        //        //var dotNumerator3 = Vector3.Dot((p1 - p1_2), triangleNormal);
                        //        //var dotDenominator3 = Vector3.Dot(dir3, triangleNormal);

                        //        //var length3 = dotNumerator3 / dotDenominator3;
                        //        //var direction3 = dir3 * length3;
                        //        //var impactPos3 = p1_2 + direction3;

                        //        //var impactDistance3 = Vector3.Distance(p1_2, impactPos3);
                        //        //var lineDistance3 = Vector3.Distance(p1_2, p3_2);

                        //        //var edge1_3 = p2 - p1;
                        //        //var edge2_3 = p3 - p2;
                        //        //var edge3_3 = p1 - p3;

                        //        //var c1_3 = impactPos3 - p1;
                        //        //var c2_3 = impactPos3 - p2;
                        //        //var c3_3 = impactPos3 - p3;

                        //        if (Vector3.Dot(triangleNormal, Vector3.Cross(edge1, c1)) > 0 &&
                        //            Vector3.Dot(triangleNormal, Vector3.Cross(edge2, c2)) > 0 &&
                        //            Vector3.Dot(triangleNormal, Vector3.Cross(edge3, c3)) > 0 &&
                        //            lineDistance > impactDistance && directionOfLine == directionToImpact)
                        //        {
                        //            var distanceInside = Vector3.Distance(impactPos, p2_2);
                        //            var d = (impactPos - p2_2).Normalized();

                        //            //if (rend.anchored == false)
                        //            //{

                        //            //    rend.position += triangleNormal.Normalized() * distanceInside;
                        //            //    rend.velocity = Vector3.Zero;

                        //            //}
                        //            //if (rend2.anchored == false)
                        //            //{

                        //            //    rend2.position += triangleNormal.Normalized() * distanceInside;
                        //            //    rend2.velocity = Vector3.Zero;

                        //            //}
                        //            rend.transform.position = lastPos;

                        //            Warn(rend2.gameObject.name + " has collided with " + rend.gameObject.name + " at " + impactPos);
                        //        }

                        //    }
                        //}



                    }
                }

                    
            }


            //foreach (Renderer rend in renderers)
            //{

            //    foreach (Renderer rend2 in renderers)
            //    {
            //        if (rend != rend2)
            //        {

            //            var rendPos = rend.position;
            //            var rend2Pos = rend2.position;

            //            var rendRad = rend.radius * rend.scale;
            //            var rend2Rad = rend2.radius * rend.scale;

            //            var distance = Vector3.Distance(rendPos, rend2Pos);
            //            var addedRad = rendRad + rend2Rad;

            //            if (distance < addedRad)
            //            {

            //                //Console.WriteLine(rend.name + " is colliding with " + rend2.name);
            //                ;
            //                for (int i = 0; i < rend.triangles.Length; i += 9)
            //                {
            //                    // Triangle 1

            //                    var p1 = new Vector3(rend.triangles[i], rend.triangles[i + 1], rend.triangles[i + 2]);
            //                    var p2 = new Vector3(rend.triangles[i + 3], rend.triangles[i + 4], rend.triangles[i + 5]);
            //                    var p3 = new Vector3(rend.triangles[i + 6], rend.triangles[i + 7], rend.triangles[i + 8]);

            //                    //if (rend.name == "Green Cube")
            //                    //    Console.WriteLine("BEFORE: " + p1);

            //                    //if (rend.name == "Green Cube")
            //                    //    Console.WriteLine("EULER: " + rend.eulerAngles);

            //                    var P1Matrix = Matrix4.CreateScale(rend.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.eulerAngles.X, rend.eulerAngles.Y, rend.eulerAngles.Z)) * Matrix4.CreateTranslation(p1.X, p1.Y, p1.Z) * Matrix4.CreateTranslation(rend.position.X, rend.position.Y, rend.position.Z);
            //                    var P2Matrix = Matrix4.CreateScale(rend.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.eulerAngles.X, rend.eulerAngles.Y, rend.eulerAngles.Z)) * Matrix4.CreateTranslation(p2.X, p2.Y, p2.Z) * Matrix4.CreateTranslation(rend.position.X, rend.position.Y, rend.position.Z);
            //                    var P3Matrix = Matrix4.CreateScale(rend.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.eulerAngles.X, rend.eulerAngles.Y, rend.eulerAngles.Z)) * Matrix4.CreateTranslation(p3.X, p3.Y, p3.Z) * Matrix4.CreateTranslation(rend.position.X, rend.position.Y, rend.position.Z);


            //                    //if (rend.name == "Green Cube")
            //                    //    Console.WriteLine("AFTER_ROT: " + P1Matrix.ExtractRotation());


            //                    p1 = P1Matrix.ExtractTranslation();
            //                    p2 = P2Matrix.ExtractTranslation();
            //                    p3 = P3Matrix.ExtractTranslation();

            //                    //if (rend.name == "Green Cube")
            //                    //    Console.WriteLine("AFTER: " + p1);

            //                    //p1 += rend.position;
            //                    //p2 += rend.position;
            //                    //p3 += rend.position;


            //                    var A = p2 - p1;
            //                    var B = p3 - p1;
            //                    var triangleNormalX = A.Y * B.Z - A.Z * B.Y;
            //                    var triangleNormalY = A.Z * B.X - A.X * B.Z;
            //                    var triangleNormalZ = A.X * B.Y - A.Y * B.X;
            //                    var triangleNormal = new Vector3(triangleNormalX, triangleNormalY, triangleNormalZ);

            //                    for (int z = 0; z < rend2.triangles.Length; z += 9)
            //                    {
            //                        // p1 p2 p3        A = p2 - p1           B = p3 - p1       N = A x B

            //                        // Triangle 2
            //                        var p1_2 = new Vector3(rend2.triangles[z], rend2.triangles[z + 1], rend2.triangles[z + 2]);
            //                        var p2_2 = new Vector3(rend2.triangles[z + 3], rend2.triangles[z + 4], rend2.triangles[z + 5]);
            //                        var p3_2 = new Vector3(rend2.triangles[z + 6], rend2.triangles[z + 7], rend2.triangles[z + 8]);

            //                        //Console.WriteLine(p1_2);
            //                        //Console.WriteLine(p2_2);
            //                        //Console.WriteLine(p3_2);

            //                        var P1Matrix_2 = Matrix4.CreateScale(rend2.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.eulerAngles.X, rend2.eulerAngles.Y, rend2.eulerAngles.Z)) * Matrix4.CreateTranslation(p1_2.X, p1_2.Y, p1_2.Z) * Matrix4.CreateTranslation(rend2.position.X, rend2.position.Y, rend2.position.Z);
            //                        var P2Matrix_2 = Matrix4.CreateScale(rend2.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.eulerAngles.X, rend2.eulerAngles.Y, rend2.eulerAngles.Z)) * Matrix4.CreateTranslation(p2_2.X, p2_2.Y, p2_2.Z) * Matrix4.CreateTranslation(rend2.position.X, rend2.position.Y, rend2.position.Z); ;
            //                        var P3Matrix_2 = Matrix4.CreateScale(rend2.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.eulerAngles.X, rend2.eulerAngles.Y, rend2.eulerAngles.Z)) * Matrix4.CreateTranslation(p3_2.X, p3_2.Y, p3_2.Z) * Matrix4.CreateTranslation(rend2.position.X, rend2.position.Y, rend2.position.Z); ;


            //                        p1_2 = P1Matrix_2.ExtractTranslation();
            //                        p2_2 = P2Matrix_2.ExtractTranslation();
            //                        p3_2 = P3Matrix_2.ExtractTranslation();

            //                        //p1_2 += rend2.position;
            //                        //p2_2 += rend2.position;
            //                        //p3_2 += rend2.position;

            //                        var triangleDistance = Vector3.Distance(rendPos, p1_2);
            //                        if (triangleDistance < rendRad)
            //                        {

            //                            A = p2_2 - p1_2;
            //                            B = p3_2 - p1_2;
            //                            triangleNormalX = A.Y * B.Z - A.Z * B.Y;
            //                            triangleNormalY = A.Z * B.X - A.X * B.Z;
            //                            triangleNormalZ = A.X * B.Y - A.Y * B.X;
            //                            var triangleNormal2 = new Vector3(triangleNormalX, triangleNormalY, triangleNormalZ);

            //                            // Check if second triangle intersects first triangle 

            //                            var dir = (p1_2 - p2_2);
            //                            var dotNumerator = Vector3.Dot((p1 - p1_2), triangleNormal);
            //                            var dotDenominator = Vector3.Dot(dir, triangleNormal);

            //                            var length = dotNumerator / dotDenominator;
            //                            var direction = dir * length;
            //                            var impactPos = p1_2 + direction;

            //                            var impactDistance = Vector3.Distance(p1_2, impactPos);
            //                            var lineDistance = Vector3.Distance(p1_2, p2_2);

            //                            var directionToImpact = (p1_2 - impactPos).Normalized();
            //                            var directionOfLine = (p1_2 - p2_2).Normalized();

            //                            var edge1 = p2 - p1;
            //                            var edge2 = p3 - p2;
            //                            var edge3 = p1 - p3;

            //                            var c1 = impactPos - p1;
            //                            var c2 = impactPos - p2;
            //                            var c3 = impactPos - p3;



            //                            //var dir2 = (p2_2 - p3_2);
            //                            //var dotNumerator2 = Vector3.Dot((p1 - p2_2), triangleNormal);
            //                            //var dotDenominator2 = Vector3.Dot(dir2, triangleNormal);

            //                            //var length2 = dotNumerator2 / dotDenominator2;
            //                            //var direction2 = dir2 * length2;
            //                            //var impactPos2 = p2_2 + direction2;

            //                            //var impactDistance2 = Vector3.Distance(p2_2, impactPos2);
            //                            //var lineDistance2 = Vector3.Distance(p2_2, p3_2);

            //                            //var edge1_2 = p2 - p1;
            //                            //var edge2_2 = p3 - p2;
            //                            //var edge3_2 = p1 - p3;

            //                            //var c1_2 = impactPos2 - p1;
            //                            //var c2_2 = impactPos2 - p2;
            //                            //var c3_2 = impactPos2 - p3;



            //                            //var dir3 = (p1_2 - p3_2);
            //                            //var dotNumerator3 = Vector3.Dot((p1 - p1_2), triangleNormal);
            //                            //var dotDenominator3 = Vector3.Dot(dir3, triangleNormal);

            //                            //var length3 = dotNumerator3 / dotDenominator3;
            //                            //var direction3 = dir3 * length3;
            //                            //var impactPos3 = p1_2 + direction3;

            //                            //var impactDistance3 = Vector3.Distance(p1_2, impactPos3);
            //                            //var lineDistance3 = Vector3.Distance(p1_2, p3_2);

            //                            //var edge1_3 = p2 - p1;
            //                            //var edge2_3 = p3 - p2;
            //                            //var edge3_3 = p1 - p3;

            //                            //var c1_3 = impactPos3 - p1;
            //                            //var c2_3 = impactPos3 - p2;
            //                            //var c3_3 = impactPos3 - p3;

            //                            if (Vector3.Dot(triangleNormal, Vector3.Cross(edge1, c1)) > 0 &&
            //                                Vector3.Dot(triangleNormal, Vector3.Cross(edge2, c2)) > 0 &&
            //                                Vector3.Dot(triangleNormal, Vector3.Cross(edge3, c3)) > 0 &&
            //                                lineDistance > impactDistance && directionOfLine == directionToImpact)
            //                            {
            //                                var distanceInside = Vector3.Distance(impactPos, p2_2);
            //                                var d = (impactPos - p2_2).Normalized();

            //                                if (rend.anchored == false)
            //                                {

            //                                    rend.position += triangleNormal.Normalized() * distanceInside;
            //                                    rend.velocity = Vector3.Zero;

            //                                }
            //                                if (rend2.anchored == false)
            //                                {

            //                                    rend2.position += triangleNormal.Normalized() * distanceInside;
            //                                    rend2.velocity = Vector3.Zero;

            //                                }

            //                                //Console.WriteLine(rend2.name + " has collided with " + rend.name + " at " + impactPos);
            //                            }

            //                            //if (Vector3.Dot(triangleNormal, Vector3.Cross(edge1_2, c1_2)) > 0 &&
            //                            //    Vector3.Dot(triangleNormal, Vector3.Cross(edge2_2, c2_2)) > 0 &&
            //                            //    Vector3.Dot(triangleNormal, Vector3.Cross(edge3_2, c3_2)) > 0 &&
            //                            //    lineDistance2 > impactDistance2)
            //                            //{
            //                            //    Console.WriteLine("Collided at " + impactPos2);
            //                            //}

            //                            //if (Vector3.Dot(triangleNormal, Vector3.Cross(edge1_3, c1_3)) > 0 &&
            //                            //    Vector3.Dot(triangleNormal, Vector3.Cross(edge2_3, c2_3)) > 0 &&
            //                            //    Vector3.Dot(triangleNormal, Vector3.Cross(edge3_3, c3_3)) > 0 &&
            //                            //    lineDistance3 > impactDistance3)
            //                            //{
            //                            //    Console.WriteLine("Collided at " + impactPos3);
            //                            //}
            //                        }

            //                    }

            //                }

            //            }

            //        }

            //    }
            //}




            //stopwatch.Restart();

        }

    }
}
