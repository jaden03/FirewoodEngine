using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace FirewoodEngine
{
    class OBJLoader
    {
        public static void loadOBJFromFile(string path, out float[] vertices, out float radius, out float[] triangles)
        {
            int counter = 0;

            List<float> verticesList = new List<float>();
            List<string> faceArrayList = new List<string>();
            List<float> normalList = new List<float>();

            float fartherestPoint = 0;

            foreach (string line in File.ReadLines("C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Models/" + path))
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

            radius = fartherestPoint;
            vertices = verticesArray;
            triangles = trianglesArray;

        }


        public static void loadOBJFromFileWithTexture(string path, out float[] vertices, out float radius, out float[] triangles)
        {
            int counter = 0;

            List<float> verticesList = new List<float>();
            List<string> faceArrayList = new List<string>();
            List<float> textureCoordList = new List<float>();
            List<float> normalList = new List<float>();

            float fartherestPoint = 0;

            foreach (string line in File.ReadLines("C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Models/" + path))
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

            radius = fartherestPoint;
            vertices = verticesArray;
            triangles = trianglesArray;

        }
    }
}