using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using FirewoodEngine.Core;

namespace FirewoodEngine.Componenents
{
    using static Logging;
    class LineRenderer : Component
    {
        public Vector3 position1;
        public Vector3 position2;
        public bool useLocal = false;
        
        [HideInInspector]
        public Material material;

        public LineRenderer()
        {
            linkedComponent = this;
            position1 = Vector3.Zero;
            position2 = Vector3.One;
            RenderManager.AddRenderer(this);
        }

        public LineRenderer(Vector3 _position1, Vector3 _position2)
        {
            linkedComponent = this;
            position1 = _position1;
            position2 = _position2;
            RenderManager.AddRenderer(this);
        }

        public void Draw(Matrix4 view, Matrix4 projection, double timeValue, Vector3 lightPos, Vector3 camPos, int buffer, int frameBuffer, int renderTexture, int depthTexture)
        {
            Matrix4 model = Matrix4.Identity;
            model = 
                Matrix4.CreateScale(transform.scale * transform.localScale) * 
                Matrix4.CreateFromQuaternion(transform.rotation) * 
                Matrix4.CreateTranslation(transform.position);

            material.shader.Use();

            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthTexture);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);
            GL.BindTexture(TextureTarget.Texture2D, renderTexture);

            if (material.rgb == true)
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
            
            if (useLocal && gameObject != null)
            {
                GL.Vertex3(position1 + gameObject.transform.position);
                GL.Vertex3(position2 + gameObject.transform.position);
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
