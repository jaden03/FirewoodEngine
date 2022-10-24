using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Security.Policy;

namespace FirewoodEngine.Core
{
    using static Logging;
    class Input
    {
        public Application app;

        public static bool LockCursor = false;
        public static bool HideCursor = false;

        static List<Key> keysDownThisFrame = new List<Key>();
        static List<Key> keysDownLastFrame = new List<Key>();

        static List<Key> keyUps = new List<Key>();
        static List<Key> keyDowns = new List<Key>();

        static Vector2 currentMousePos = Vector2.Zero;

        //KeyboardState currentInput;


        //public void Update()
        //{
        //    currentInput = Keyboard.GetState();
        //}

        public static KeyboardState GetKeyboardState()
        {
            var currentInput = Keyboard.GetState();
            return currentInput;
        }
        public static MouseState GetMouseState()
        {
            var currentMouse = Mouse.GetState();
            return currentMouse;
        }


        public static void SetMousePosition(Vector2 pos)
        {
            currentMousePos = pos;
        }



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
        public static Vector2 GetMouseCoords()
        {
            return new Vector2(currentMousePos.X, currentMousePos.Y);
        }

        public static float GetMouseWheel()
        {
            var currentMouse = Mouse.GetState();
            return currentMouse.WheelPrecise;
        }

        public void Update(FrameEventArgs e)
        {
            var currentInput = Keyboard.GetState();

            keyUps.Clear();
            keyDowns.Clear();

            foreach (Key key in Enum.GetValues(typeof(Key)))
            {
                if (currentInput.IsKeyDown(key) && !keysDownThisFrame.Contains(key))
                {
                    keysDownThisFrame.Add(key);
                }
                else if (!currentInput.IsKeyDown(key) && keysDownThisFrame.Contains(key))
                {
                    keysDownThisFrame.Remove(key);
                }

                if (keysDownThisFrame.Contains(key) && !keysDownLastFrame.Contains(key))
                {
                    keyDowns.Add(key);
                }
                if (!keysDownThisFrame.Contains(key) && keysDownLastFrame.Contains(key))
                {
                    keyUps.Add(key);
                }

                if (currentInput.IsKeyDown(key) && !keysDownLastFrame.Contains(key))
                {
                    keysDownLastFrame.Add(key);
                }
                else if (!currentInput.IsKeyDown(key) && keysDownLastFrame.Contains(key))
                {
                    keysDownLastFrame.Remove(key);
                }
            }
        }


        public static bool GetKeyDown(Key key)
        {
            return keyDowns.Contains(key);
        }

        public static bool GetKeyUp(Key key)
        {
            return keyUps.Contains(key);
        }


    }
}
