using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Quaternion = OpenTK.Quaternion;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using FirewoodEngine.Componenents;

namespace FirewoodEngine.Core
{
    public static class Debug
    {
        public static void DrawLine(Vector3 point1, Vector3 point2, Color color)
        {
            var newDebugObject = new GameObject();

            var lineMat = new Material();
            lineMat.shader = Shader.colorShader;
            lineMat.color = color;

            var lineRenderer = new LineRenderer(point1, point2);
            lineRenderer.material = lineMat;

            newDebugObject.AddComponent(lineRenderer);
        }
    }
}
