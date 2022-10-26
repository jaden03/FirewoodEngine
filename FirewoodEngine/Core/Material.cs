using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace FirewoodEngine.Core
{
    internal class Material
    {
        public string texturePath;
        public Shader shader;
        public Texture texture;
        public Color color;
        public bool rgb = false;

        public Material()
        {
            color = Color.White;
        }

        public void SetTexture(string path)
        {
            texturePath = path;
            texture = Texture.LoadFromFile("../../../Textures/" + path);
        }

    }
}
