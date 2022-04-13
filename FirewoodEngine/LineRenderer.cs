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
        public bool useLocal = false;
        public Material material;
        public bool rgb = false;

        public LineRenderer()
        {
            position1 = Vector3.Zero;
            position2 = Vector3.One;
        }

        public LineRenderer(Vector3 _position1, Vector3 _position2)
        {
            position1 = _position1;
            position2 = _position2;
        }

        public void Draw(Matrix4 view, Matrix4 projection, double timeValue, Vector3 lightPos, Vector3 camPos)
        {
            Matrix4 model =
            (
                Matrix4.CreateScale(gameobject.transform.scale)
                * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(gameobject.transform.eulerAngles.X, gameobject.transform.eulerAngles.Y, gameobject.transform.eulerAngles.Z))
                * Matrix4.CreateTranslation(gameobject.transform.position)
            );

            material.shader.Use();

            if (rgb == true)
                material.color = Colors.ColorFromHSV((timeValue * 100) % 255, 1, 1);

            int modelLocation = GL.GetUniformLocation(material.shader.Handle, "model");
            GL.UniformMatrix4(modelLocation, true, ref model);

            int viewLocation = GL.GetUniformLocation(material.shader.Handle, "view");
            GL.UniformMatrix4(viewLocation, true, ref view);

            int projectionLocation = GL.GetUniformLocation(material.shader.Handle, "projection");
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            int colorLocation = GL.GetUniformLocation(material.shader.Handle, "color");
            GL.Uniform3(colorLocation, (float)material.color.R / 255, (float)material.color.G / 255, (float)material.color.B / 255);

            int vertexLocation = GL.GetAttribLocation(material.shader.Handle, "aPos");
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vertexLocation);

            int normalLocation = GL.GetAttribLocation(material.shader.Handle, "aNormal");
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(normalLocation);

            int lightColorLocation = GL.GetUniformLocation(material.shader.Handle, "lightColor");
            GL.Uniform3(lightColorLocation, 1.0f, 1.0f, 1.0f);

            int lightLocation = GL.GetUniformLocation(material.shader.Handle, "lightPos");
            GL.Uniform3(lightLocation, lightPos.X, lightPos.Y, lightPos.Z);

            int camPosLocation = GL.GetUniformLocation(material.shader.Handle, "viewPos");
            GL.Uniform3(camPosLocation, camPos.X, camPos.Y, camPos.Z);

            GL.Begin(PrimitiveType.Lines);
            
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
