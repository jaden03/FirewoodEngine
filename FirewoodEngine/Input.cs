﻿using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FirewoodEngine
{
    class Input
    {
        public Application app;

        //KeyboardState currentInput;


        //public void Update()
        //{
        //    currentInput = Keyboard.GetState();
        //}


        public static bool GetKey(Key key)
        {
            var currentInput = Keyboard.GetState();
            return currentInput.IsKeyDown(key);
        }

        public static Vector2 GetMousePos()
        {
            var currentMouse = Mouse.GetState();
            return new Vector2(currentMouse.X, currentMouse.Y);
        }

        public static float GetMouseWheel()
        {
            var currentMouse = Mouse.GetState();
            return currentMouse.WheelPrecise;
        }

        public void Update(FrameEventArgs e)
        {
        }
        }
}