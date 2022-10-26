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
using Microsoft.Win32;

namespace FirewoodEngine.Core
{
    using static Logging;
    public class EditorUI
    {
        static bool autoScrollConsole = true;
        public static Vector2 viewportSize = new Vector2(800, 600);

        static int renderTexture;
        static List<string> consoleOutput = new List<string>();
        public Application app;

        ImFontPtr font;

        static bool scene = true;
        static bool console = true;
        static bool assets = true;
        static bool inspector = true;
        static bool hierarchy = true;


        public void Initialize(ImGuiController cont)
        {
            var io = ImGui.GetIO();
            font = io.Fonts.AddFontFromFileTTF("../../../Core/Varela.ttf", 20);
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
            colors[(int)ImGuiCol.Header] = new Vector4(0.43f, 0.43f, 0.43f, 0.31f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.20f, 0.20f, 0.20f, 0.80f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.26f, 0.59f, 0.98f, 1.00f);
            colors[(int)ImGuiCol.Separator] = new Vector4(0.00f, 0.00f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.20f, 0.20f, 0.20f, 0.80f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.12f, 0.12f, 0.12f, 1.00f);
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


        public void UpdateUI(int _renderTexture, List<String> _consoleOutput)
        {
            //ImGui.ShowDemoWindow();
            ImGui.PushFont(font);

            renderTexture = _renderTexture;
            consoleOutput = _consoleOutput;

            MainMenuBar();

            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());

            if (scene)
                Scene();
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
            ImGui.Image((IntPtr)renderTexture, new Vector2(ImGui.GetWindowWidth(), ImGui.GetWindowHeight()), new Vector2(0, 1), new Vector2(1, 0));

            viewportSize.X = ImGui.GetWindowSize().X;
            viewportSize.Y = ImGui.GetWindowSize().Y;

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
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(350, 500), ImGuiCond.FirstUseEver);
            ImGui.Begin("Inspector", ref inspector, inspectorFlags);

            // Name Input
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2(10, 5));

            // The amount of pixels between the edges and the box
            var widthInverse = 10;
            var windowWidth = ImGui.GetWindowWidth();

            // Set the width to the window width minus the width of the padding
            ImGui.PushItemWidth(windowWidth - widthInverse);
            var test = "";

            // Set the X position of the cursor to have it be centered
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - widthInverse / 2);
            ImGui.InputTextWithHint("", "Name", ref test, 100);
            ImGui.PopStyleVar();
            ImGui.PopItemWidth();


            // Tag Text


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
    }
}
