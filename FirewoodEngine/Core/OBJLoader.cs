﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace FirewoodEngine.Core
{
    class OBJLoader
    {
        public static void loadOBJFromFile(string path, out float[] vertices, out float[] triangles, out Vector3 bounds, out Vector3 center, out float radius)
        {
            int counter = 0;

            List<float> verticesList = new List<float>();
            List<string> faceArrayList = new List<string>();
            List<float> normalList = new List<float>();


            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 cent = new Vector3(0, 0, 0);
            float fartherestPoint = 0;

            foreach (string line in File.ReadLines("../../Models/" + path))
            {
                if (line.StartsWith("v "))
                {
                    string positions = line.Substring(2, line.Length - 2);
                    string[] positionsArray = positions.Split(' ');
                    for (int i = 0; i < positionsArray.Length; i++)
                    {
                        verticesList.Add(float.Parse(positionsArray[i]));
                    }

                    Vector3 pos = new Vector3(float.Parse(positionsArray[0]), float.Parse(positionsArray[1]), float.Parse(positionsArray[2]));
                    if (Vector3.Distance(Vector3.Zero, pos) > fartherestPoint)
                        fartherestPoint = Vector3.Distance(Vector3.Zero, pos);

                    if (pos.X < min.X)
                        min.X = pos.X;
                    if (pos.Y < min.Y)
                        min.Y = pos.Y;
                    if (pos.Z < min.Z)
                        min.Z = pos.Z;
                    if (pos.X > max.X)
                        max.X = pos.X;
                    if (pos.Y > max.Y)
                        max.Y = pos.Y;
                    if (pos.Z > max.Z)
                        max.Z = pos.Z;
                }
                if (line.StartsWith("vn "))
                {
                    string normals = line.Substring(3, line.Length - 3);
                    string[] normalsArray = normals.Split(' ');
                    for (int i = 0; i < normalsArray.Length; i++)
                    {
                        normalList.Add(float.Parse(normalsArray[i]));
                    }
                }
                if (line.StartsWith("f "))
                {
                    string face = line.Substring(2, line.Length - 2);
                    string[] faceArray = face.Split(' ');
                    for (int i = 0; i < faceArray.Length; i++)
                    {
                        faceArrayList.Add(faceArray[i]);
                    }

                }
                counter++;
            }

            List<float> calculatedVerticesList = new List<float>();
            List<float> calculatedTrianglesList = new List<float>();

            foreach (string f in faceArrayList)
            {
                string[] splitF = f.Split('/');
                float vertice = verticesList[int.Parse(splitF[0]) * 3 - 3];
                calculatedVerticesList.Add(vertice);
                calculatedTrianglesList.Add(vertice);
                vertice = verticesList[int.Parse(splitF[0]) * 3 - 3 + 1];
                calculatedVerticesList.Add(vertice);
                calculatedTrianglesList.Add(vertice);
                vertice = verticesList[int.Parse(splitF[0]) * 3 - 3 + 2];
                calculatedVerticesList.Add(vertice);
                calculatedTrianglesList.Add(vertice);

                vertice = normalList[int.Parse(splitF[2]) * 3 - 3];
                calculatedVerticesList.Add(vertice);
                vertice = normalList[int.Parse(splitF[2]) * 3 - 3 + 1];
                calculatedVerticesList.Add(vertice);
                vertice = normalList[int.Parse(splitF[2]) * 3 - 3 + 2];
                calculatedVerticesList.Add(vertice);
            }

            float[] verticesArray = new float[calculatedVerticesList.Count];

            for (int i = 0; i < verticesArray.Length; i++)
            {
                verticesArray[i] = calculatedVerticesList[i];
            }

            float[] trianglesArray = new float[calculatedTrianglesList.Count];

            for (int i = 0; i < trianglesArray.Length; i++)
            {
                trianglesArray[i] = calculatedTrianglesList[i];
            }

            vertices = verticesArray;
            triangles = trianglesArray;
            bounds = max - min;
            center = (max + min) / 2;
            radius = fartherestPoint;
        }


        public static void loadOBJFromFileWithTexture(string path, out float[] vertices, out float[] triangles, out Vector3 bounds, out Vector3 center, out float radius)
        {
            int counter = 0;

            List<float> verticesList = new List<float>();
            List<string> faceArrayList = new List<string>();
            List<float> textureCoordList = new List<float>();
            List<float> normalList = new List<float>();

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 cent = new Vector3(0, 0, 0);
            float fartherestPoint = 0;

            foreach (string line in File.ReadLines("../../Models/" + path))
            {
                if (line.StartsWith("v "))
                {
                    string positions = line.Substring(2, line.Length - 2);
                    string[] positionsArray = positions.Split(' ');

                    for (int i = 0; i < positionsArray.Length; i++)
                    {
                        verticesList.Add(float.Parse(positionsArray[i]));
                    }

                    Vector3 pos = new Vector3(float.Parse(positionsArray[0]), float.Parse(positionsArray[1]), float.Parse(positionsArray[2]));
                    if (Vector3.Distance(Vector3.Zero, pos) > fartherestPoint)
                        fartherestPoint = Vector3.Distance(Vector3.Zero, pos);

                    if (pos.X < min.X)
                        min.X = pos.X;
                    if (pos.Y < min.Y)
                        min.Y = pos.Y;
                    if (pos.Z < min.Z)
                        min.Z = pos.Z;
                    if (pos.X > max.X)
                        max.X = pos.X;
                    if (pos.Y > max.Y)
                        max.Y = pos.Y;
                    if (pos.Z > max.Z)
                        max.Z = pos.Z;
                }
                if (line.StartsWith("vt "))
                {
                    string coords = line.Substring(3, line.Length - 3);
                    string[] coordsArray = coords.Split(' ');
                    for (int i = 0; i < coordsArray.Length; i++)
                    {
                        textureCoordList.Add(float.Parse(coordsArray[i]));
                    }
                }
                if (line.StartsWith("vn "))
                {
                    string normals = line.Substring(3, line.Length - 3);
                    string[] normalsArray = normals.Split(' ');
                    for (int i = 0; i < normalsArray.Length; i++)
                    {
                        normalList.Add(float.Parse(normalsArray[i]));
                    }
                }
                if (line.StartsWith("f "))
                {
                    string face = line.Substring(2, line.Length - 2);
                    string[] faceArray = face.Split(' ');
                    for (int i = 0; i < faceArray.Length; i++)
                    {
                        faceArrayList.Add(faceArray[i]);
                    }

                }
                counter++;
            }

            List<float> calculatedVerticesList = new List<float>();
            List<float> calculatedTrianglesList = new List<float>();

            foreach (string f in faceArrayList)
            {
                string[] splitF = f.Split('/');
                float vertice = verticesList[int.Parse(splitF[0]) * 3 - 3];
                calculatedVerticesList.Add(vertice);
                calculatedTrianglesList.Add(vertice);
                vertice = verticesList[int.Parse(splitF[0]) * 3 - 3 + 1];
                calculatedVerticesList.Add(vertice);
                calculatedTrianglesList.Add(vertice);
                vertice = verticesList[int.Parse(splitF[0]) * 3 - 3 + 2];
                calculatedVerticesList.Add(vertice);
                calculatedTrianglesList.Add(vertice);

                vertice = textureCoordList[int.Parse(splitF[1]) * 2 - 2];
                calculatedVerticesList.Add(vertice);
                vertice = textureCoordList[int.Parse(splitF[1]) * 2 - 1];
                calculatedVerticesList.Add(vertice);

                vertice = normalList[int.Parse(splitF[2]) * 3 - 3];
                calculatedVerticesList.Add(vertice);
                vertice = normalList[int.Parse(splitF[2]) * 3 - 3 + 1];
                calculatedVerticesList.Add(vertice);
                vertice = normalList[int.Parse(splitF[2]) * 3 - 3 + 2];
                calculatedVerticesList.Add(vertice);
            }

            float[] verticesArray = new float[calculatedVerticesList.Count];

            for (int i = 0; i < verticesArray.Length; i++)
            {
                verticesArray[i] = calculatedVerticesList[i];
            }

            float[] trianglesArray = new float[calculatedTrianglesList.Count];

            for (int i = 0; i < trianglesArray.Length; i++)
            {
                trianglesArray[i] = calculatedTrianglesList[i];
            }

            vertices = verticesArray;
            triangles = trianglesArray;
            bounds = max - min;
            center = (max + min) / 2;
            radius = fartherestPoint;
        }
    }
}