using OpenTK.Input;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using FirewoodEngine.Core;
using OpenTK.Graphics.OpenGL;

namespace FirewoodEngine.Componenents
{
    class Camera : Component
    {
        public Application app;
        public float fov = 90f;

        public void Update(FrameEventArgs e)
        {
            Matrix4 view = Matrix4.LookAt(gameObject.transform.position, gameObject.transform.position + transform.forward, transform.up);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)EditorUI.viewportSize.X / (float)EditorUI.viewportSize.Y, 0.01f, 1000.0f);
            app.Context.SwapBuffers();

            RenderManager.Render(view, projection, app.stopwatch, app._lightPos, gameObject.transform.position, app);
        }
        

    }
}
