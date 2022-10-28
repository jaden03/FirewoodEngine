using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Input;
using FirewoodEngine.Scripts;
using FirewoodEngine.Core;
using FirewoodEngine.Components;

namespace FirewoodEngine
{
    using static Logging;
    class Game
    {
        public Application app;
        
        GameObject house;
        GameObject sphere;

        GameObject testSoundObject;

        // Fires on the first frame of the game
        public void Start()
        {
            // Hide the cursor and lock it to the center of the screen
            //Input.HideCursor = true;
            //Input.LockCursor = true;

            // Create a new GameObject
            var cameraObject = new GameObject();
            cameraObject.transform.position = new Vector3(0, 5, -5);
            cameraObject.name = "Camera";

            var cameraObjectChild = new GameObject();
            cameraObjectChild.transform.SetParent(cameraObject.transform);

            // Create a camera component
            var camera = new Camera();
            // Add the camera component to the GameObject
            cameraObjectChild.AddComponent(camera);
            // Add the camera component to the active scripts so the update function will work (will be refactored so you dont have to do this)

            // Create a freecam component
            var freecam = new Freecam();
            // Add the freecam component to the GameObject
            cameraObject.AddComponent(freecam);
            // Fire the Start function before anything else so execution order doesnt destroy you
            freecam.Start();
            // Add the freecam component to the active scripts so the update function will work (will be refactored so you dont have to do this)


            // Audio Listener \\
            var audioListener = new AudioListener();
            cameraObjectChild.AddComponent(audioListener);
            audioListener.Start();




            // Skybox material
            var skyboxMat = new Material();
            skyboxMat.SetTexture("skybox.png");
            skyboxMat.shader = Shader.textureShader;

            // Skybox object
            var skybox = new GameObject();

            // Skybox renderer
            var skyboxRenderer = new Renderer();
            skyboxRenderer.SetOBJ("skyboxCube.obj", skyboxMat.texture != null);
            skyboxRenderer.material = skyboxMat;

            // Add the renderer to the skybox and scale it up  (the skybox is literally just a big cube with inverted normals and a texture)
            skybox.AddComponent(skyboxRenderer);
            skybox.transform.scale = new Vector3(100, 100, 100);

            

            // line object
            var lineObject = new GameObject();
            lineObject.name = "Lines";
            lineObject.transform.position = new Vector3(5, 0, 5);

            // line material
            var lineMat = new Material();
            lineMat.shader = Shader.colorShader;
            lineMat.color = Color.Blue;

            // line renderer
            var line = new LineRenderer(new Vector3(-8.8f, 0.3f, -2.8f), new Vector3(-8, 3, 0));
            line.material = lineMat;
            line.useLocal = true;
            lineObject.AddComponent(line);



            // house gameobject
            house = new GameObject();
            house.name = "House";
            house.transform.position = new Vector3(0, 5, 0);
            //house.transform.eulerAngles = new Vector3(0, 45, 0);

            // house material
            Material houseMat = new Material();
            houseMat.SetTexture("House.png");
            houseMat.shader = Shader.textureShader;

            // house renderer
            Renderer houseRenderer = new Renderer();
            houseRenderer.SetOBJ("house.obj", houseMat.texture != null);
            houseRenderer.material = houseMat;
            house.AddComponent(houseRenderer);

            // Rigidbody on the house for physics (no collision rn)
            var houseRB = new Rigidbody();
            houseRB.useGravity = false;
            house.AddComponent(houseRB);

            // Box collider on the house
            BoxCollider houseCollider = new BoxCollider();
            house.AddComponent(houseCollider);
            houseCollider.CalculateBoundsFromMesh();
            //houseCollider.DebugBounds();



            // terrain material
            Material terrainMat = new Material();
            terrainMat.color = Color.Green;
            terrainMat.shader = Shader.colorShader;

            // terrain gameobject
            GameObject terrain = new GameObject();
            terrain.name = "Terrain";
            terrain.transform.position.Y = -1.5f;

            // terrain renderer
            Renderer terrainRenderer = new Renderer();
            terrainRenderer.SetOBJ("flatGround.obj", false);
            terrainRenderer.material = terrainMat;
            terrain.AddComponent(terrainRenderer);

            // terrain rigidbody
            var terrainRB = new Rigidbody();
            terrainRB.useGravity = false;
            terrain.AddComponent(terrainRB);

            // terrain collider
            var terrainCollider = new BoxCollider();
            terrain.AddComponent(terrainCollider);
            terrainCollider.CalculateBoundsFromMesh();




            sphere = new GameObject();
            sphere.name = "Sphere";
            sphere.transform.scale = new Vector3(.2f, .2f, .2f);

            Material sphereMat = new Material();
            sphereMat.shader = Shader.textureShader;
            sphereMat.SetTexture("sphere.png");

            Renderer sphereRenderer = new Renderer();
            sphereRenderer.material = sphereMat;
            sphereRenderer.SetOBJ("sphere.obj", true);
            sphere.AddComponent(sphereRenderer);

            var sphereCollider = new SphereCollider();
            sphere.AddComponent(sphereCollider);
            sphereCollider.CalculateBoundsFromMesh();
            sphereCollider.DebugBounds();
            sphereCollider.radius = .5f;
            sphereCollider.isTrigger = true;

            sphereCollider.triggerEnter += (Rigidbody otherBody) =>
            {
                Warn(otherBody.gameObject.name + " entered the sphere's trigger");
            };

            sphereCollider.triggerExit += (Rigidbody otherBody) =>
            {
                Warn(otherBody.gameObject.name + " exited the sphere's trigger");
            };

            sphereCollider.triggerStay += (Rigidbody otherBody) =>
            {
                Warn("Sphere triggering " + otherBody.gameObject.name);
            };
            

            //sphereCollider.DebugBounds();

            var sphereRB = new Rigidbody();
            sphereRB.useGravity = false;
            sphere.AddComponent(sphereRB);


            sphere.transform.localPosition = new Vector3(2, 0, 2);
            sphere.transform.SetParent(house.transform);





            testSoundObject = new GameObject();
            testSoundObject.name = "Test Sound";

            var testSound = new AudioSource("testMono.wav");
            testSoundObject.AddComponent(testSound);
            testSound.directional = true;



            GameObject amogus = new GameObject();

            Renderer amogusRenderer = new Renderer();
            amogusRenderer.SetOBJ("amogus.obj", true);
            amogus.AddComponent(amogusRenderer);

            Material amogusMat = new Material();
            amogusMat.shader = Shader.textureShader;
            amogusMat.SetTexture("amogus.png");

            amogusRenderer.material = amogusMat;

            amogus.transform.position = new Vector3(5, 4, 0);
            amogus.transform.eulerAngles = new Vector3(0, 180, 0);

            //var generation = new Generation();
            //generation.SetupTerrain();
        }

        // Fires every frame
        public void Update(FrameEventArgs e)
        {
            if (Input.GetKeyDown(Key.L))
            {
                testSoundObject.GetComponent<AudioSource>().Play();
            }

            
            if (Input.GetKey(Key.X))
            {
                house.transform.eulerAngles.X += 1f;
                Warn(house.transform.eulerAngles);
                //house.transform.position.X += 0.1f;
            }
            if (Input.GetKey(Key.Y))
            {
                house.transform.eulerAngles.Y += 1f;
                Warn(house.transform.eulerAngles);
                //house.transform.position.Y += 0.1f;
            }
            if (Input.GetKey(Key.Z))
            {
                house.transform.eulerAngles.Z += 1f;
                Warn(house.transform.eulerAngles);
                //house.transform.position.Z += 0.1f;
            }

            if (Input.GetKey(Key.I))
            {
                sphere.transform.localPosition.X += 0.1f;
            }
            if (Input.GetKey(Key.J))
            {
                sphere.transform.localPosition.Y += 0.1f;
            }
            if (Input.GetKey(Key.K))
            {
                sphere.transform.localPosition.Z += 0.1f;
            }

            if (Input.GetKey(Key.F))
            {
                Debug.DrawLine(house.transform.position, house.transform.position + house.transform.forward * 10, Color.Blue);
            }
            if (Input.GetKey(Key.R))
            {
                Debug.DrawLine(house.transform.position, house.transform.position + house.transform.right * 10, Color.Red);
            }
            if (Input.GetKey(Key.U))
            {
                Debug.DrawLine(house.transform.position, house.transform.position + house.transform.up * 10, Color.Green);
            }


            if (!Input.GetKey(Key.Q) && !Input.GetKey(Key.E))
            {
                //(house.GetComponent("Rigidbody") as Rigidbody).velocity.Y = 0;
            }
            if (Input.GetKeyDown(Key.F))
            {
                Print("test");
                //(house.GetComponent<Rigidbody>()).useGravity = true;
            }
        }

    }
}
