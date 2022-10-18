using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewoodEngine.Scripts
{
    class Generation : Component
    {
        List<float> vertices;
        List<float> triangles;

        float[][] noise;

        void AddQuadAtPosition(Vector2 pos)
        {
            Vector2 noisePos = new Vector2(pos.X + .5f, pos.Y + .5f);

            float bottomLeftNoise = noise[(int)Math.Round(noisePos.X - .5f)][(int)Math.Round(noisePos.Y - .5f)] * 3;
            //bottomLeftNoise = (float)Math.Round(bottomLeftNoise * 3) / 3;

            vertices.Add(pos.X - .5f);
            vertices.Add(bottomLeftNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(bottomLeftNoise);
            triangles.Add(pos.Y - .5f);

            float topLeftNoise = noise[(int)Math.Round(noisePos.X - .5f)][(int)Math.Round(noisePos.Y + .5f)] * 3;
            //topLeftNoise = (float)Math.Round(topLeftNoise * 3) / 3;

            vertices.Add(pos.X - .5f);
            vertices.Add(topLeftNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(topLeftNoise);
            triangles.Add(pos.Y + .5f);


            float topRightNoise = noise[(int)Math.Round(noisePos.X + .5f)][(int)Math.Round(noisePos.Y + .5f)] * 3;
            //topRightNoise = (float)Math.Round(topRightNoise * 3) / 3;

            vertices.Add(pos.X + .5f);
            vertices.Add(topRightNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(topRightNoise);
            triangles.Add(pos.Y + .5f);


            vertices.Add(pos.X + .5f);
            vertices.Add(topRightNoise);
            vertices.Add(pos.Y + .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(topRightNoise);
            triangles.Add(pos.Y + .5f);


            float bottomRightNoise = noise[(int)Math.Round(noisePos.X + .5f)][(int)Math.Round(noisePos.Y - .5f)] * 3;
            //bottomRightNoise = (float)Math.Round(bottomRightNoise * 3) / 3;

            vertices.Add(pos.X + .5f);
            vertices.Add(bottomRightNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X + .5f);
            triangles.Add(bottomRightNoise);
            triangles.Add(pos.Y - .5f);

            vertices.Add(pos.X - .5f);
            vertices.Add(bottomLeftNoise);
            vertices.Add(pos.Y - .5f);
            triangles.Add(pos.X - .5f);
            triangles.Add(bottomLeftNoise);
            triangles.Add(pos.Y - .5f);
        }

        List<float> RecalculateNormals(List<float> vertices)
        {
            List<float> newVerticesList;
            newVerticesList = new List<float>();

            for (int i = 0; i < vertices.Count; i += 9)
            {
                Vector3 v1 = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
                Vector3 v2 = new Vector3(vertices[i + 3], vertices[i + 4], vertices[i + 5]);
                Vector3 v3 = new Vector3(vertices[i + 6], vertices[i + 7], vertices[i + 8]);

                Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1);
                normal.Normalize();

                newVerticesList.Add(vertices[i]);
                newVerticesList.Add(vertices[i + 1]);
                newVerticesList.Add(vertices[i + 2]);
                newVerticesList.Add(normal.X);
                newVerticesList.Add(normal.Y);
                newVerticesList.Add(normal.Z);

                newVerticesList.Add(vertices[i + 3]);
                newVerticesList.Add(vertices[i + 4]);
                newVerticesList.Add(vertices[i + 5]);
                newVerticesList.Add(normal.X);
                newVerticesList.Add(normal.Y);
                newVerticesList.Add(normal.Z);

                newVerticesList.Add(vertices[i + 6]);
                newVerticesList.Add(vertices[i + 7]);
                newVerticesList.Add(vertices[i + 8]);
                newVerticesList.Add(normal.X);
                newVerticesList.Add(normal.Y);
                newVerticesList.Add(normal.Z);
            }

            return newVerticesList;
        }

        public void SetupTerrain()
        {
            // terrain material
            Material terrainMat = new Material();
            terrainMat.color = Color.Green;
            terrainMat.shader = Shader.colorShader;

            // terrain gameobject
            GameObject terrain = new GameObject();
            terrain.name = "Procedural Terrain";
            terrain.transform.position.Y = -3;

            // terrain renderer
            Renderer terrainRenderer = new Renderer();
            terrainRenderer.material = terrainMat;
            terrain.AddComponent(terrainRenderer);

            vertices = new List<float>();
            triangles = new List<float>();

            int size = 50;
            terrain.transform.position.X = -size / 2;
            terrain.transform.position.Z = -size / 2;

            noise = Noise.GeneratePerlinNoise(Noise.GenerateWhiteNoise(size * 10, size * 10), 3);
            for (int x = 0; x <= size; x++)
            {
                for (int y = 0; y <= size; y++)
                {
                    AddQuadAtPosition(new Vector2(x, y));
                }
            }

            vertices = RecalculateNormals(vertices);
            terrainRenderer.vertices = vertices.ToArray();
            terrainRenderer.triangles = triangles.ToArray();
        }
    }
}
