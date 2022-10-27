using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using FirewoodEngine.Core;

namespace FirewoodEngine.Componenents
{
    using static Logging;
    class Renderer : Component
    {
        public float[] vertices;
        public float[] triangles;
        public bool wireframe = false;
        public int VertexArrayObject;
        public Material material;

        public Vector3 bounds;
        public Vector3 offset;
        public float radius;

        public string path;


        public Renderer()
        {
            linkedComponent = this;
            VertexArrayObject = GL.GenVertexArray();
            RenderManager.AddRenderer(this);
        }

        public void SetOBJ(string path, bool useTexture)
        {
            this.path = path;
            if (useTexture == true)
                OBJLoader.loadOBJFromFileWithTexture(path, out vertices, out triangles, out bounds, out offset, out radius);
            else
                OBJLoader.loadOBJFromFile(path, out vertices, out triangles, out bounds, out offset, out radius);
        }

        public void Render(Matrix4 view, Matrix4 projection, double timeValue, Vector3 lightPos, Vector3 camPos, int buffer, int frameBuffer, int renderTexture, int depthTexture)
        {
            Matrix4 model = Matrix4.Identity;
            model = 
                Matrix4.CreateScale(transform.scale) * 
                Matrix4.CreateFromQuaternion(transform.rotation) * 
                Matrix4.CreateTranslation(transform.position);

            //texture.Use(TextureUnit.Texture0);
            material.shader.Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthTexture);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.BindTexture(TextureTarget.Texture2D, renderTexture);

            int modelLocation = GL.GetUniformLocation(material.shader.Handle, "model");
            GL.UniformMatrix4(modelLocation, true, ref model);

            int viewLocation = GL.GetUniformLocation(material.shader.Handle, "view");
            GL.UniformMatrix4(viewLocation, true, ref view);

            int projectionLocation = GL.GetUniformLocation(material.shader.Handle, "projection");
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            
            if (material.rgb == true)
                material.color = Colors.ColorFromHSV((timeValue * 100) % 255, 1, 1);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VertexArrayObject);


            if (material.texture != null)
            {
                material.texture.Use(TextureUnit.Texture0);

                int vertexLocation = GL.GetAttribLocation(material.shader.Handle, "aPos");
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                GL.EnableVertexAttribArray(vertexLocation);

                int texCoordLocation = GL.GetAttribLocation(material.shader.Handle, "aTexCoord");
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(texCoordLocation);

                int normalLocation = GL.GetAttribLocation(material.shader.Handle, "aNormal");
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
                GL.EnableVertexAttribArray(normalLocation);

                int lightColorLocation = GL.GetUniformLocation(material.shader.Handle, "lightColor");
                GL.Uniform3(lightColorLocation, 1.0f, 1.0f, 1.0f);

                int lightLocation = GL.GetUniformLocation(material.shader.Handle, "lightPos");
                GL.Uniform3(lightLocation, lightPos.X, lightPos.Y, lightPos.Z);

                int camPosLocation = GL.GetUniformLocation(material.shader.Handle, "viewPos");
                GL.Uniform3(camPosLocation, camPos.X, camPos.Y, camPos.Z);
            }
            else
            {
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
            }
            if (wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length * 3);
            GL.Flush();
        }

    }
}