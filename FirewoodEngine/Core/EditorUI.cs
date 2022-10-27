using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using System.Numerics;
using Dear_ImGui_Sample;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using OpenTK.Input;
using Vector3 = OpenTK.Vector3;

namespace FirewoodEngine.Core
{
    using static Logging;
    public class EditorUI
    {
        static bool autoScrollConsole = true;
        public static Vector2 viewportSize = new Vector2(800, 600);
        public static Vector2 gameSize = new Vector2(800, 600);
        public float titleBarHeight;

        static int renderTexture;
        static int renderTextureEditor;
        static List<string> consoleOutput = new List<string>();
        public Application app;

        ImFontPtr font;
        ImFontPtr fontBig;

        static bool scene = true;
        static bool game = true;
        static bool console = true;
        static bool assets = true;
        static bool inspector = true;
        static bool hierarchy = true;


        public void Initialize(ImGuiController cont)
        {
            var io = ImGui.GetIO();
            font = io.Fonts.AddFontFromFileTTF("../../../Core/Varela.ttf", 20);
            fontBig = io.Fonts.AddFontFromFileTTF("../../../Core/Varela.ttf", 50);
            cont.RecreateFontDeviceTexture();
            
            RangeAccessor<Vector4> colors = ImGui.GetStyle().Colors;
            colors[(int)ImGuiCol.Text] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.08f, 0.08f, 0.08f, 0.94f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.06f, 0.06f, 0.06f, 0.54f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.06f, 0.06f, 0.06f, 0.40f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.26f, 0.59f, 0.98f, 0.67f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.00f, 0.00f, 0.00f, 0.51f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.02f, 0.02f, 0.02f, 0.53f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(0.43f, 0.43f, 0.43f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.43f, 0.43f, 0.43f, 1.00f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.43f, 0.43f, 0.43f, 0.40f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.06f, 0.53f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.Separator] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.20f, 0.20f, 0.20f, 0.80f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.26f, 0.59f, 0.98f, 0.20f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.20f, 0.20f, 0.20f, 0.67f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.26f, 0.59f, 0.98f, 0.95f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.00f, 0.00f, 0.00f, 0.86f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.20f, 0.20f, 0.20f, 0.80f);
            colors[(int)ImGuiCol.TabActive] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.00f, 0.00f, 0.00f, 0.86f);
            colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
            colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.26f, 0.59f, 0.98f, 0.70f);
            colors[(int)ImGuiCol.DockingEmptyBg] = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.PlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.19f, 0.19f, 0.20f, 1.00f);
            colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.31f, 0.31f, 0.35f, 1.00f);
            colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.23f, 0.23f, 0.25f, 1.00f);
            colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);
            colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.00f, 1.00f, 1.00f, 0.06f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.26f, 0.59f, 0.98f, 0.35f);
            colors[(int)ImGuiCol.DragDropTarget] = new Vector4(1.00f, 1.00f, 0.00f, 0.90f);
            colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f);
            ImGui.GetStyle().WindowMenuButtonPosition = ImGuiDir.None;

            ImGui.GetStyle().WindowRounding = 0;
            ImGui.GetStyle().ChildRounding = 0;
            ImGui.GetStyle().FrameRounding = 6;
            ImGui.GetStyle().PopupRounding = 6;
            ImGui.GetStyle().GrabRounding = 3;
            ImGui.GetStyle().TabRounding = 6;

            ImGui.GetStyle().WindowBorderSize = 0;
            ImGui.GetStyle().ChildBorderSize = 0;

            ImGui.GetStyle().FramePadding = new Vector2(10, 6);
        }


        public void UpdateUI(int _renderTexture, List<String> _consoleOutput, int _renderTextureEditor)
        {
            //ImGui.ShowDemoWindow();
            ImGui.PushFont(font);

            renderTexture = _renderTexture;
            renderTextureEditor = _renderTextureEditor;
            consoleOutput = _consoleOutput;

            MainMenuBar();

            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());

            if (scene)
                Scene();
            if (game)
                GameV();
            if (console)
                Console();
            if (assets)
                Assets();
            if (inspector)
                Inspector();
            if (hierarchy)
                Hierarchy();

        }



        void MainMenuBar()
        {
            // Menu Bar
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(0, 10));
            if (ImGui.BeginMainMenuBar())
            {
                titleBarHeight = ImGui.GetWindowSize().Y;
                ImGui.PopStyleVar();

                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New Project"))
                    {
                        //GameObjectManager.gameObjects.Clear();
                    }
                    if (ImGui.MenuItem("Open Project"))
                    {
                        //GameObjectManager.gameObjects.Clear();
                        //GameObjectManager.Load();
                    }
                    if (ImGui.MenuItem("Save Project"))
                    {
                        //GameObjectManager.Save();
                    }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Exit"))
                    {
                        app.Exit();
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Scene"))
                {
                    if (ImGui.MenuItem("New Scene"))
                    {
                        //GameObjectManager.Play();
                    }
                    if (ImGui.MenuItem("Open Scene"))
                    {
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.FileName = "Scene";
                        openFileDialog.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Scenes");

                        Nullable<bool> result = openFileDialog.ShowDialog();

                        if (result == true)
                        {
                            Editor.LoadScene(openFileDialog.FileName);
                        }
                    }
                    if (ImGui.MenuItem("Save Scene"))
                    {
                        //GameObjectManager.Play();
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo", "CTRL+Z"))
                    {

                    }
                    if (ImGui.MenuItem("Redo", "CTRL+Y", false, false))
                    {

                    }  // Disabled item
                    ImGui.Separator();
                    if (ImGui.MenuItem("Cut", "CTRL+X"))
                    {

                    }
                    if (ImGui.MenuItem("Copy", "CTRL+C"))
                    {

                    }
                    if (ImGui.MenuItem("Paste", "CTRL+V"))
                    {

                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("View"))
                {
                    if (ImGui.MenuItem("Scene"))
                    {
                        scene = !scene;
                    }
                    if (ImGui.MenuItem("Game"))
                    {
                        game = !game;
                    }
                    if (ImGui.MenuItem("Console"))
                    {
                        console = !console;
                    }
                    if (ImGui.MenuItem("Assets"))
                    {
                        assets = !assets;
                    }
                    if (ImGui.MenuItem("Inspector"))
                    {
                        inspector = !inspector;
                    }
                    if (ImGui.MenuItem("Hierarchy"))
                    {
                        hierarchy = !hierarchy;
                    }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("GameObject"))
                {
                    if (ImGui.MenuItem("Create Empty"))
                    {
                        //GameObjectManager.CreateEmpty();
                    }
                    if (ImGui.MenuItem("Create Cube"))
                    {
                        //GameObjectManager.CreateCube();
                    }
                    if (ImGui.MenuItem("Create Sphere"))
                    {
                        //GameObjectManager.CreateSphere();
                    }
                    if (ImGui.MenuItem("Create Plane"))
                    {
                        //GameObjectManager.CreatePlane();
                    }
                    ImGui.EndMenu();
                }


                #region PlayButton
                var playButtonSize = new Vector2(30, 30);
                var windowSize = ImGui.GetWindowSize();
                // put the play button in the center
                var playButtonPos = new Vector2((windowSize.X / 2) - playButtonSize.X, (windowSize.Y / 2 ) - playButtonSize.Y + 15);
                ImGui.SetCursorPos(playButtonPos);
                if (ImGui.Button("", playButtonSize))
                {
                    //Editor.Play();
                }
                ImGui.GetWindowDrawList().AddTriangleFilled(
                        new Vector2(playButtonPos.X + 10, playButtonPos.Y + 10),
                        new Vector2(playButtonPos.X + 10, playButtonPos.Y + 20),
                        new Vector2(playButtonPos.X + 20, playButtonPos.Y + 15),
                        0xFFFFFFFF);


                #endregion

                #region PauseButton

                var pauseButtonSize = new Vector2(30, 30);
                var pauseButtonPos = new Vector2((windowSize.X / 2) - pauseButtonSize.X + 40, (windowSize.Y / 2) - pauseButtonSize.Y + 15);
                ImGui.SetCursorPos(pauseButtonPos);
                if (ImGui.Button("", pauseButtonSize))
                {
                    //Editor.Pause();
                }
                // draw two vertical boxes
                ImGui.GetWindowDrawList().AddRectFilled(
                        new Vector2(pauseButtonPos.X + 10, pauseButtonPos.Y + 10),
                        new Vector2(pauseButtonPos.X + 13, pauseButtonPos.Y + 20),
                        0xFFFFFFFF);
                ImGui.GetWindowDrawList().AddRectFilled(
                    new Vector2(pauseButtonPos.X + 17, pauseButtonPos.Y + 10),
                        new Vector2(pauseButtonPos.X + 20, pauseButtonPos.Y + 20),
                        0xFFFFFFFF);

                #endregion

                #region StopButton

                var stopButtonSize = new Vector2(30, 30);
                var stopButtonPos = new Vector2((windowSize.X / 2) - stopButtonSize.X - 40, (windowSize.Y / 2) - stopButtonSize.Y + 15);
                ImGui.SetCursorPos(stopButtonPos);
                if (ImGui.Button("", stopButtonSize))
                {
                    //Editor.Stop();
                }
                ImGui.GetWindowDrawList().AddRectFilled(
                        new Vector2(stopButtonPos.X + 10, stopButtonPos.Y + 10),
                        new Vector2(stopButtonPos.X + 20, stopButtonPos.Y + 20),
                        0xFFFFFFFF);

                #endregion

                ImGui.EndMainMenuBar();
            };

        }

        void Scene()
        {
            // Scene
            var sceneFlags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse;
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(viewportSize.X, viewportSize.Y), ImGuiCond.FirstUseEver);

            // Get rid of padding
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0, 0));

            ImGui.Begin("Scene", ref scene, sceneFlags);
            ImGui.PopStyleVar();
            ImGui.Image((IntPtr)renderTextureEditor, new Vector2(ImGui.GetWindowWidth(), ImGui.GetWindowHeight()), new Vector2(0, 1), new Vector2(1, 0));

            viewportSize.X = ImGui.GetWindowSize().X;
            viewportSize.Y = ImGui.GetWindowSize().Y;

            Editor.sceneFocused = ImGui.IsWindowFocused();

            ImGui.End();
        }


        void GameV()
        {
            // Scene
            var gameFlags = ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse;
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(gameSize.X, gameSize.Y), ImGuiCond.FirstUseEver);

            // Get rid of padding
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0, 0));

            ImGui.Begin("Game", ref scene, gameFlags);
            ImGui.PopStyleVar();
            
            if (app.gameCamera == null)
            {
                ImGui.PushFont(fontBig);
                var textSize = ImGui.CalcTextSize("No Camera Found In Scene");
                var windowSize = ImGui.GetWindowSize();
                var textPos = new Vector2(windowSize.X / 2 - textSize.X / 2, windowSize.Y / 2 - textSize.Y / 2);
                ImGui.SetCursorPos(textPos);
                ImGui.Text("No Camera Found In Scene");
                ImGui.PopFont();
            }
            else
            {
                ImGui.Image((IntPtr)renderTexture, new Vector2(ImGui.GetWindowWidth(), ImGui.GetWindowHeight()), new Vector2(0, 1), new Vector2(1, 0));
            }

            gameSize.X = ImGui.GetWindowSize().X;
            gameSize.Y = ImGui.GetWindowSize().Y;

            ImGui.End();
        }


        void Console()
        {
            // Console
            var consoleFlags = ImGuiWindowFlags.NoCollapse;
            ImGui.SetNextWindowSize(new Vector2(500, 350), ImGuiCond.FirstUseEver);

            ImGui.Begin("Console", ref console, consoleFlags);

            if (ImGui.Button("Clear"))
            {
                consoleOutput.Clear();
            }
            ImGui.SameLine();
            ImGui.Checkbox("Auto Scroll", ref autoScrollConsole);
            ImGui.Separator();
            ImGui.BeginChild("Scrolling");
            foreach (string output in consoleOutput)
            {
                ImGui.Text(output);
            }

            if (autoScrollConsole)
            {
                ImGui.SetScrollY(ImGui.GetScrollMaxY());
            }
            ImGui.EndChild();
            
                
            ImGui.End();
        }


        static void Assets()
        {
            // Assets
            var assetsFlags = ImGuiWindowFlags.NoCollapse;
            ImGui.SetNextWindowSize(new Vector2(500, 350), ImGuiCond.FirstUseEver);
            ImGui.Begin("Assets", ref assets, assetsFlags);


            ImGui.End();
        }


        static void Inspector()
        {
            // Inspector
            var inspectorFlags = ImGuiWindowFlags.NoCollapse;
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(450, 500), ImGuiCond.FirstUseEver);

            // This line doesnt work for some reason
            ImGui.SetNextWindowSizeConstraints(new Vector2(450, 0), new Vector2(10000, 10000));
            
            ImGui.Begin("Inspector", ref inspector, inspectorFlags);

            if (Editor.selectedObject == null) { ImGui.End(); return; }
            var selectedObject = Editor.selectedObject;

            var windowWidth = ImGui.GetWindowWidth();
            #region Name Input Box

            // Name Input
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(10, 5));
            var widthInverse = 15;
            ImGui.SetNextItemWidth(windowWidth - (widthInverse * 2));

            ImGui.SetCursorPosX(widthInverse);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
            
            ImGui.InputText("##name", ref selectedObject.name, 100);

            #endregion

            // LabelText
            var labelPixelsFromLeft = 50;

            // InputBox
            var inputBoxPixelsFromRight = 10;
            var inputBoxWidth = 250;
            var inputBoxXPos = windowWidth - inputBoxPixelsFromRight - inputBoxWidth;
            
            
            #region Tag

            // Tag Text
            var tagTextSize = ImGui.CalcTextSize("Tag");
            var tagTextPos = new Vector2(labelPixelsFromLeft - tagTextSize.X, ImGui.GetCursorPosY() + 10);
            
            ImGui.SetCursorPos(tagTextPos);
            ImGui.Text("Tag");

            ImGui.SameLine();

            // Tag Input
            ImGui.SetNextItemWidth(inputBoxWidth);
            ImGui.SetCursorPos(new Vector2(inputBoxXPos, ImGui.GetCursorPosY() - 5));
            
            ImGui.InputText("##tag", ref selectedObject.tag, 100);
            

            #endregion
            
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            ImGui.Separator();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

            #region Components

            var components = selectedObject.components;

            InspectorDrawComponent(selectedObject.transform);
            foreach (Component component in components)
            {
                InspectorDrawComponent(component.linkedComponent);
            }
            
            #endregion


            ImGui.End();
        }

        
        static void Hierarchy()
        {
            // Hierarchy
            var hierarchyFlags = ImGuiWindowFlags.NoCollapse;
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, 500), ImGuiCond.FirstUseEver);
            ImGui.Begin("Hierarchy", ref hierarchy, hierarchyFlags);
                
            if (ImGui.TreeNode("Scene"))
            {
                for (int i = 0; i < 10; i++)
                {
                    var treeNodeFlags = ImGuiTreeNodeFlags.OpenOnArrow;
                    var open = ImGui.TreeNodeEx("GameObject " + i, treeNodeFlags);

                    if (open)
                    {
                        ImGui.TreePush();
                        open = ImGui.TreeNodeEx("Child", treeNodeFlags);
                        if (open)
                        {
                            ImGui.TreePop();
                        }
                        ImGui.TreePop();
                        ImGui.TreePop();
                    }
                }
                ImGui.TreePop();
            }
            ImGui.End();
        }
        
        static void InspectorDrawComponent(object component)
        {
            var nameCapitalized = component.GetType().Name;
            nameCapitalized = nameCapitalized[0].ToString().ToUpper() + nameCapitalized.Substring(1);
            
            if (ImGui.CollapsingHeader(nameCapitalized, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.OpenOnArrow))
            {
                var properties = component.GetType().GetFields();
                
                foreach (var prop in properties)
                {
                    DrawField(prop, component);
                }



                // // Position Text
                // var positionTextSize = ImGui.CalcTextSize("Position");
                // var positionTextPos = new Vector2(labelPixelsFromLeft, ImGui.GetCursorPosY() + 10);
                // ImGui.SetCursorPos(positionTextPos);
                // ImGui.Text("Position");
                //
                // ImGui.SameLine();
                //
                // // Position Input
                // ImGui.SetNextItemWidth(inputBoxWidth);
                // ImGui.SetCursorPos(new Vector2(inputBoxXPos, ImGui.GetCursorPosY() - 5));
                //
                // var test = "";
                //
                // ImGui.InputText("##position", ref test, 100);
                //
                // ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            }
        }

        static void DrawField(FieldInfo field, object component)
        {
            var labelPixelsFromLeft = 50;
            var inputBoxPixelsFromRight = 10;
            var inputBoxWidth = 250;
            var windowWidth = ImGui.GetWindowWidth();
            var inputBoxXPos = windowWidth - inputBoxPixelsFromRight - inputBoxWidth;
            
            var nameCapitalized = field.Name;
            nameCapitalized = nameCapitalized[0].ToString().ToUpper() + nameCapitalized.Substring(1);
            
            var textSize = ImGui.CalcTextSize(field.Name);
            var textSizeTextPos = new Vector2(labelPixelsFromLeft, ImGui.GetCursorPosY() + 10);
            
            var attributes = field.GetCustomAttributes(false);
            if (attributes.Any(x => x.GetType() == typeof(HideInInspector)))
            {
                return;
            }
            
            var fieldType = field.FieldType;
            if (fieldType == (typeof(OpenTK.Vector3)))
            {
                // Text
                ImGui.SetCursorPos(textSizeTextPos);
                ImGui.Text(nameCapitalized);
                ImGui.SameLine();
                // Input
                ImGui.SetNextItemWidth(inputBoxWidth);
                ImGui.SetCursorPos(new Vector2(inputBoxXPos, ImGui.GetCursorPosY() - 5));

                System.Numerics.Vector3 retrievedVector = new System.Numerics.Vector3(((Vector3)field.GetValue(component)).X, ((Vector3)field.GetValue(component)).Y, ((Vector3)field.GetValue(component)).Z);
                if (ImGui.InputFloat3("##" + field.Name, ref retrievedVector) && Input.GetKeyDown(Key.Enter))
                {
                    field.SetValue(component, new Vector3(retrievedVector.X, retrievedVector.Y, retrievedVector.Z));
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            }
            else if (fieldType == (typeof(float)))
            {
                // Text
                ImGui.SetCursorPos(textSizeTextPos);
                ImGui.Text(nameCapitalized);
                ImGui.SameLine();
                // Input
                ImGui.SetNextItemWidth(inputBoxWidth);
                ImGui.SetCursorPos(new Vector2(inputBoxXPos, ImGui.GetCursorPosY() - 5));

                float retrievedFloat = (float)field.GetValue(component);
                if (ImGui.InputFloat("##" + field.Name, ref retrievedFloat) && Input.GetKeyDown(Key.Enter))
                {
                    field.SetValue(component, retrievedFloat);
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            }
            else if (fieldType == (typeof(int)))
            {
                // Text
                ImGui.SetCursorPos(textSizeTextPos);
                ImGui.Text(nameCapitalized);
                ImGui.SameLine();
                // Input
                ImGui.SetNextItemWidth(inputBoxWidth);
                ImGui.SetCursorPos(new Vector2(inputBoxXPos, ImGui.GetCursorPosY() - 5));

                int retrievedInt = (int)field.GetValue(component);
                if (ImGui.InputInt("##" + field.Name, ref retrievedInt) && Input.GetKeyDown(Key.Enter))
                {
                    field.SetValue(component, retrievedInt);
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            }
            else if (fieldType == (typeof(string)))
            {
                // Text
                ImGui.SetCursorPos(textSizeTextPos);
                ImGui.Text(nameCapitalized);
                ImGui.SameLine();
                // Input
                ImGui.SetNextItemWidth(inputBoxWidth);
                ImGui.SetCursorPos(new Vector2(inputBoxXPos, ImGui.GetCursorPosY() - 5));

                string retrievedString = (string)field.GetValue(component);
                if (ImGui.InputText("##" + field.Name, ref retrievedString, 100) && Input.GetKeyDown(Key.Enter))
                {
                    field.SetValue(component, retrievedString);
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            }
            else if (fieldType == (typeof(bool)))
            {
                // Text
                ImGui.SetCursorPos(textSizeTextPos);
                ImGui.Text(nameCapitalized);
                ImGui.SameLine();
                // Input
                ImGui.SetNextItemWidth(inputBoxWidth);
                ImGui.SetCursorPos(new Vector2(inputBoxXPos, ImGui.GetCursorPosY() - 5));

                bool retrievedBool = (bool)field.GetValue(component);
                if (ImGui.Checkbox("##" + field.Name, ref retrievedBool) && Input.GetKeyDown(Key.Enter))
                {
                    field.SetValue(component, retrievedBool);
                }

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            }
            else if (fieldType == (typeof(string)))
            {
                // Text
                ImGui.SetCursorPos(textSizeTextPos);
                ImGui.Text(nameCapitalized);
                ImGui.SameLine();
                // Input
                ImGui.SetNextItemWidth(inputBoxWidth);
                ImGui.SetCursorPos(new Vector2(inputBoxXPos, ImGui.GetCursorPosY() - 5));
                
                string retrievedString = (string)field.GetValue(component);
                if (ImGui.InputText("##" + field.Name, ref retrievedString, 100) && Input.GetKeyDown(Key.Enter))
                {
                    field.SetValue(component, retrievedString);
                }
                
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            }



        }
    }
}
