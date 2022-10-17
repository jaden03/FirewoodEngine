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
namespace FirewoodEngine
{
    class Camera : Component
    {
        public Application app;

        public Vector3 front;
        public Vector3 up;
        public float fov = 90f;
        
        public void Update(FrameEventArgs e)
        {
            Matrix4 view = Matrix4.LookAt(gameObject.transform.position, gameObject.transform.position + front, up);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(fov), (float)app.Width / app.Height, 0.1f, 1000.0f);
            app.Context.SwapBuffers();

            RenderManager.Render(view, projection, app.stopwatch, app._lightPos, gameObject.transform.position);
        }
        

    }
}
