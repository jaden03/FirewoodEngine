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
    using static Logging;
    class LineRenderer : Component
    {
        public Vector3 position1;
        public Vector3 position2;
        public Color color;
        public bool useLocal = false;

        public LineRenderer()
        {
            position1 = Vector3.Zero;
            position2 = Vector3.One;
            color = Color.White;
        }

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
            
            if (useLocal && gameobject != null)
            {
                GL.Vertex3(position1 + gameobject.transform.position);
                GL.Vertex3(position2 + gameobject.transform.position);
            }
            else if (!useLocal)
            {
                GL.Vertex3(position1);
                GL.Vertex3(position2);
            }
            
            GL.End();
        }
    }
}
