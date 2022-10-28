using OpenTK.Input;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using FirewoodEngine.Core;
using OpenTK.Graphics.OpenGL;

namespace FirewoodEngine.Components
{
    public class Camera : Component
    {
        public float fov = 90f;
        
        public float near = 0.01f;
        public float far = 1000f;
        
        public Color backgroundColor = Color.Black;
    }
}
