﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine
{
    using static Logging;
    class Physics
    {

        public static List<Rigidbody> rbs;
        public static Stopwatch stopwatch = new Stopwatch();

        public static void Initialize()
        {
            rbs = new List<Rigidbody>();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Physics Initialized");
            Console.ForegroundColor = ConsoleColor.White;

            stopwatch.Start();
        }

        public static void AddRigidbody(Rigidbody rb)
        {
            rbs.Add(rb);
        }

        public static void RemoveRigidbody(Rigidbody rb)
        {
            rbs.Remove(rb);
        }

        public static void Update()
        {
            var deltaTime = stopwatch.ElapsedTicks * 0.0000001f;

            foreach (Rigidbody rb in rbs)
            {
                if (rb.useGravity)
                {
                    rb.gameObject.transform.position += new Vector3(0, -1f, 0) * deltaTime;
                }

                    var lastPos = rb.gameObject.transform.position;


                foreach (Rigidbody rb2 in rbs)
                {
                    if (rb2 != rb)
                    {
                        var rend = rb.gameObject.GetComponent("Renderer") as Renderer;
                        var rend2 = rb2.gameObject.GetComponent("Renderer") as Renderer;

                        for (int i = 0; i < rend.triangles.Length; i += 9)
                        {
                            // Triangle 1

                            var p1 = new Vector3(rend.triangles[i], rend.triangles[i + 1], rend.triangles[i + 2]);
                            var p2 = new Vector3(rend.triangles[i + 3], rend.triangles[i + 4], rend.triangles[i + 5]);
                            var p3 = new Vector3(rend.triangles[i + 6], rend.triangles[i + 7], rend.triangles[i + 8]);

                            //if (rend.name == "Green Cube")
                            //    Console.WriteLine("BEFORE: " + p1);

                            //if (rend.name == "Green Cube")
                            //    Console.WriteLine("EULER: " + rend.eulerAngles);

                            var P1Matrix = Matrix4.CreateScale(rend.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.transform.eulerAngles.X, rend.transform.eulerAngles.Y, rend.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p1.X, p1.Y, p1.Z) * Matrix4.CreateTranslation(rend.transform.position.X, rend.transform.position.Y, rend.transform.position.Z);
                            var P2Matrix = Matrix4.CreateScale(rend.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.transform.eulerAngles.X, rend.transform.eulerAngles.Y, rend.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p2.X, p2.Y, p2.Z) * Matrix4.CreateTranslation(rend.transform.position.X, rend.transform.position.Y, rend.transform.position.Z);
                            var P3Matrix = Matrix4.CreateScale(rend.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend.transform.eulerAngles.X, rend.transform.eulerAngles.Y, rend.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p3.X, p3.Y, p3.Z) * Matrix4.CreateTranslation(rend.transform.position.X, rend.transform.position.Y, rend.transform.position.Z);


                            //if (rend.name == "Green Cube")
                            //    Console.WriteLine("AFTER_ROT: " + P1Matrix.ExtractRotation());


                            p1 = P1Matrix.ExtractTranslation();
                            p2 = P2Matrix.ExtractTranslation();
                            p3 = P3Matrix.ExtractTranslation();

                            //if (rend.name == "Green Cube")
                            //    Console.WriteLine("AFTER: " + p1);

                            //p1 += rend.position;
                            //p2 += rend.position;
                            //p3 += rend.position;

                            var A = p2 - p1;
                            var B = p3 - p1;
                            var triangleNormalX = A.Y * B.Z - A.Z * B.Y;
                            var triangleNormalY = A.Z * B.X - A.X * B.Z;
                            var triangleNormalZ = A.X * B.Y - A.Y * B.X;
                            var triangleNormal = new Vector3(triangleNormalX, triangleNormalY, triangleNormalZ);

                            for (int z = 0; z < rend2.triangles.Length; z += 9)
                            {
                                // p1 p2 p3        A = p2 - p1           B = p3 - p1       N = A x B

                                // Triangle 2
                                var p1_2 = new Vector3(rend2.triangles[z], rend2.triangles[z + 1], rend2.triangles[z + 2]);
                                var p2_2 = new Vector3(rend2.triangles[z + 3], rend2.triangles[z + 4], rend2.triangles[z + 5]);
                                var p3_2 = new Vector3(rend2.triangles[z + 6], rend2.triangles[z + 7], rend2.triangles[z + 8]);

                                //Console.WriteLine(p1_2);
                                //Console.WriteLine(p2_2);
                                //Console.WriteLine(p3_2);

                                var P1Matrix_2 = Matrix4.CreateScale(rend2.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.transform.eulerAngles.X, rend2.transform.eulerAngles.Y, rend2.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p1_2.X, p1_2.Y, p1_2.Z) * Matrix4.CreateTranslation(rend2.transform.position.X, rend2.transform.position.Y, rend2.transform.position.Z);
                                var P2Matrix_2 = Matrix4.CreateScale(rend2.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.transform.eulerAngles.X, rend2.transform.eulerAngles.Y, rend2.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p2_2.X, p2_2.Y, p2_2.Z) * Matrix4.CreateTranslation(rend2.transform.position.X, rend2.transform.position.Y, rend2.transform.position.Z); ;
                                var P3Matrix_2 = Matrix4.CreateScale(rend2.transform.scale) * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rend2.transform.eulerAngles.X, rend2.transform.eulerAngles.Y, rend2.transform.eulerAngles.Z)) * Matrix4.CreateTranslation(p3_2.X, p3_2.Y, p3_2.Z) * Matrix4.CreateTranslation(rend2.transform.position.X, rend2.transform.position.Y, rend2.transform.position.Z); ;


                                p1_2 = P1Matrix_2.ExtractTranslation();
                                p2_2 = P2Matrix_2.ExtractTranslation();
                                p3_2 = P3Matrix_2.ExtractTranslation();

                                //p1_2 += rend2.position;
                                //p2_2 += rend2.position;
                                //p3_2 += rend2.position;


                                A = p2_2 - p1_2;
                                B = p3_2 - p1_2;
                                triangleNormalX = A.Y * B.Z - A.Z * B.Y;
                                triangleNormalY = A.Z * B.X - A.X * B.Z;
                                triangleNormalZ = A.X * B.Y - A.Y * B.X;
                                var triangleNormal2 = new Vector3(triangleNormalX, triangleNormalY, triangleNormalZ);

                                // Check if second triangle intersects first triangle 

                                var dir = (p1_2 - p2_2);
                                var dotNumerator = Vector3.Dot((p1 - p1_2), triangleNormal);
                                var dotDenominator = Vector3.Dot(dir, triangleNormal);

                                var length = dotNumerator / dotDenominator;
                                var direction = dir * length;
                                var impactPos = p1_2 + direction;

                                var impactDistance = Vector3.Distance(p1_2, impactPos);
                                var lineDistance = Vector3.Distance(p1_2, p2_2);

                                var directionToImpact = (p1_2 - impactPos).Normalized();
                                var directionOfLine = (p1_2 - p2_2).Normalized();

                                var edge1 = p2 - p1;
                                var edge2 = p3 - p2;
                                var edge3 = p1 - p3;

                                var c1 = impactPos - p1;
                                var c2 = impactPos - p2;
                                var c3 = impactPos - p3;



                                //var dir2 = (p2_2 - p3_2);
                                //var dotNumerator2 = Vector3.Dot((p1 - p2_2), triangleNormal);
                                //var dotDenominator2 = Vector3.Dot(dir2, triangleNormal);

                                //var length2 = dotNumerator2 / dotDenominator2;
                                //var direction2 = dir2 * length2;
                                //var impactPos2 = p2_2 + direction2;

                                //var impactDistance2 = Vector3.Distance(p2_2, impactPos2);
                                //var lineDistance2 = Vector3.Distance(p2_2, p3_2);

                                //var edge1_2 = p2 - p1;
                                //var edge2_2 = p3 - p2;
                                //var edge3_2 = p1 - p3;

                                //var c1_2 = impactPos2 - p1;
                                //var c2_2 = impactPos2 - p2;
                                //var c3_2 = impactPos2 - p3;



                                //var dir3 = (p1_2 - p3_2);
                                //var dotNumerator3 = Vector3.Dot((p1 - p1_2), triangleNormal);
                                //var dotDenominator3 = Vector3.Dot(dir3, triangleNormal);

                                //var length3 = dotNumerator3 / dotDenominator3;
                                //var direction3 = dir3 * length3;
                                //var impactPos3 = p1_2 + direction3;

                                //var impactDistance3 = Vector3.Distance(p1_2, impactPos3);
                                //var lineDistance3 = Vector3.Distance(p1_2, p3_2);

                                //var edge1_3 = p2 - p1;
                                //var edge2_3 = p3 - p2;
                                //var edge3_3 = p1 - p3;

                                //var c1_3 = impactPos3 - p1;
                                //var c2_3 = impactPos3 - p2;
                                //var c3_3 = impactPos3 - p3;

                                if (Vector3.Dot(triangleNormal, Vector3.Cross(edge1, c1)) > 0 &&
                                    Vector3.Dot(triangleNormal, Vector3.Cross(edge2, c2)) > 0 &&
                                    Vector3.Dot(triangleNormal, Vector3.Cross(edge3, c3)) > 0 &&
                                    lineDistance > impactDistance && directionOfLine == directionToImpact)
                                {
                                    var distanceInside = Vector3.Distance(impactPos, p2_2);
                                    var d = (impactPos - p2_2).Normalized();

                                    //if (rend.anchored == false)
                                    //{

                                    //    rend.position += triangleNormal.Normalized() * distanceInside;
                                    //    rend.velocity = Vector3.Zero;

                                    //}
                                    //if (rend2.anchored == false)
                                    //{

                                    //    rend2.position += triangleNormal.Normalized() * distanceInside;
                                    //    rend2.velocity = Vector3.Zero;

                                    //}
                                    rend.transform.position = lastPos;
                                    
                                    Warn(rend2.gameObject.name + " has collided with " + rend.gameObject.name + " at " + impactPos);
                                }
                                
                            }
                        }



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