using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace FirewoodEngine
{
    class Renderer
    {
        public readonly float[] vertices;
        public Vector3 position;
        public Vector3 eulerAngles;
        public Color color;
        public float scale;
        public readonly float[] triangles;

        public bool anchored = true;
        public bool gravity = false;
        public Vector3 velocity = new Vector3(0,0,0);
        public Vector3 angularVelocity = new Vector3(0,0,0);

        public bool wireframe = false;


        public readonly float radius;

        Vector3 lightPos;
        Vector3 camPos;

        public readonly int VertexArrayObject;

        public readonly Shader shader;

        Texture texture;


        public Renderer(float[] _vertices, Vector3 _position, Vector3 _eulerAngles, Color _color, Shader _shader, Vector3 _lightPos, Vector3 _camPos, float _scale)
        {
            vertices = _vertices;
            position = _position;
            eulerAngles = _eulerAngles;
            color = _color;
            shader = _shader;
            lightPos = _lightPos;
            camPos = _camPos;
            scale = _scale;

            VertexArrayObject = GL.GenVertexArray();
        }

        public Renderer(string _modelPath, Vector3 _position, Vector3 _eulerAngles, Color _color, Shader _shader, Vector3 _lightPos, Vector3 _camPos, float _scale)
        {
            OBJLoader.loadOBJFromFile(_modelPath, out vertices, out radius, out triangles);
            position = _position;
            eulerAngles = _eulerAngles;
            color = _color;
            shader = _shader;
            lightPos = _lightPos;
            camPos = _camPos;
            scale = _scale;

            VertexArrayObject = GL.GenVertexArray();
        }

        public Renderer(string _modelPath, Vector3 _position, Vector3 _eulerAngles, string _texturePath, Shader _shader, Vector3 _lightPos, Vector3 _camPos, float _scale)
        {
            OBJLoader.loadOBJFromFileWithTexture(_modelPath, out vertices, out radius, out triangles);
            position = _position;
            eulerAngles = _eulerAngles;
            texture = Texture.LoadFromFile("C:/Users/PC/source/repos/FirewoodEngine/FirewoodEngine/Textures/" + _texturePath);
            shader = _shader;
            lightPos = _lightPos;
            camPos = _camPos;
            scale = _scale;

            VertexArrayObject = GL.GenVertexArray();
        }

        public void Render(Matrix4 view, Matrix4 projection, double timeValue)
        {
            Matrix4 model =  (Matrix4.CreateScale(scale) 
                * Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z))
                * Matrix4.CreateTranslation(position));

            //texture.Use(TextureUnit.Texture0);
            shader.Use();

            int modelLocation = GL.GetUniformLocation(shader.Handle, "model");
            GL.UniformMatrix4(modelLocation, true, ref model);

            int viewLocation = GL.GetUniformLocation(shader.Handle, "view");
            GL.UniformMatrix4(viewLocation, true, ref view);

            int projectionLocation = GL.GetUniformLocation(shader.Handle, "projection");
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            //Color color;
            //color = Colors.ColorFromHSV((timeValue * 100) % 255, 1, 1);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VertexArrayObject);


            if (texture != null)
            {
                texture.Use(TextureUnit.Texture0);

                int vertexLocation = GL.GetAttribLocation(shader.Handle, "aPos");
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                GL.EnableVertexAttribArray(vertexLocation);

                int texCoordLocation = GL.GetAttribLocation(shader.Handle, "aTexCoord");
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(texCoordLocation);

                int normalLocation = GL.GetAttribLocation(shader.Handle, "aNormal");
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
                GL.EnableVertexAttribArray(normalLocation);

                int lightColorLocation = GL.GetUniformLocation(shader.Handle, "lightColor");
                GL.Uniform3(lightColorLocation, 1.0f, 1.0f, 1.0f);

                int lightLocation = GL.GetUniformLocation(shader.Handle, "lightPos");
                GL.Uniform3(lightLocation, lightPos.X, lightPos.Y, lightPos.Z);

                int camPosLocation = GL.GetUniformLocation(shader.Handle, "viewPos");
                GL.Uniform3(camPosLocation, camPos.X, camPos.Y, camPos.Z);
            }
            else
            {
                int colorLocation = GL.GetUniformLocation(shader.Handle, "color");
                GL.Uniform3(colorLocation, (float)color.R / 255, (float)color.G / 255, (float)color.B / 255);

                int vertexLocation = GL.GetAttribLocation(shader.Handle, "aPos");
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
                GL.EnableVertexAttribArray(vertexLocation);

                int normalLocation = GL.GetAttribLocation(shader.Handle, "aNormal");
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(normalLocation);

                int lightColorLocation = GL.GetUniformLocation(shader.Handle, "lightColor");
                GL.Uniform3(lightColorLocation, 1.0f, 1.0f, 1.0f);

                int lightLocation = GL.GetUniformLocation(shader.Handle, "lightPos");
                GL.Uniform3(lightLocation, lightPos.X, lightPos.Y, lightPos.Z);

                int camPosLocation = GL.GetUniformLocation(shader.Handle, "viewPos");
                GL.Uniform3(camPosLocation, camPos.X, camPos.Y, camPos.Z);
            }
            if (wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length * 3);
        }

    }
}
