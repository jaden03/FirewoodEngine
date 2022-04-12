using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace FirewoodEngine
{
    class LineRenderer
    {
        public Vector3 position1;
        public Vector3 position2;
        public Color color;

        public LineRenderer(Vector3 _position1, Vector3 _position2, Color _color)
        {
            position1 = _position1;
            position2 = _position2;
            color = _color;
        }

        public void Draw()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(color);
            GL.Vertex3(position1);
            GL.Vertex3(position2);
            GL.End();
        }
    }
}
