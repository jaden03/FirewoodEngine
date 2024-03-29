﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FirewoodEngine.Core
{
    internal class Material
    {
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
            texture = Texture.LoadFromFile("../../Textures/" + path);
        }

    }
}
