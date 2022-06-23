 using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using System.Drawing;
using OpenTK.Audio.OpenAL;
using System;

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class MainMenuScene : Scene
    {
        //Delta time initiated
        public static float dt = 0;

        //Managers created
        EntityManager entityManager;
        SystemManager systemManager;

        //Entities created
        Entity newEntity;
        AudioManager Music;

        //Camera instantiated
        public Camera camera;

        //GameScene instantiated
        public static MainMenuScene MenuInstance;

        //creates bools used for playing the music
        bool playing = false;
        bool closing = false;

        public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
        {
            //creates an instance of this scene and managers
            MenuInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            // Set the title of the window
            sceneManager.Title = "Game";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            sceneManager.mouseDelegate += Mouse_BottonPressed;
            // Set Keyboard events to go to a method in this class


            // Enable Depth Testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            //background colour
            GL.ClearColor(0.2f, 0.2f, 0.5f, 1.0f);

            // Set Camera
            camera = new Camera(new Vector3(0, 1.7f, 1), new Vector3(0, 1.7f, 0), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 100f);

            CreateEntities();
            CreateSystems();

            // TODO: Add your initialisation logic here
            // Setup Audio Source from the Audio Buffer

        }

        private void CreateEntities()
        {
            newEntity = new Entity("Music");
            newEntity.AddComponent(new ComponentPosition(0, 1.7f, 1));
            newEntity.AddComponent(new ComponentAudio("Game/Audio/oblivion2.wav"));
            entityManager.AddEntity(newEntity);
            Music = new AudioManager(newEntity);
        }
        //creates the systems needed for this scene
        private void CreateSystems()
        {
            ISystem newSystem;

            newSystem = new SystemRender();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemAudio();
            systemManager.AddSystem(newSystem);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Update(FrameEventArgs e)
        {
            

            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed)
                sceneManager.Exit();

            // TODO: Add your update logic here

            AL.Listener(ALListener3f.Position, ref camera.cameraPosition);
            AL.Listener(ALListenerfv.Orientation, ref camera.cameraDirection, ref camera.cameraUp);
            //plays the noise
            if (!playing)
            {
                Music.PlayNoise();
                Music.LoopNoise(false);
                playing = true;
            }
            //closes the noise
            if (closing)
            {
                Close();
                sceneManager.ChangeScene(SceneTypes.SCENE_GAME);
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.CornflowerBlue;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.Label(new Rectangle(0, (int)(fontSize / 2f), (int)width, (int)(fontSize * 2f)), "Main Menu", (int)fontSize, StringAlignment.Center);

            GUI.Render();
        }

        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            //clears and closes assets
            Music.close();
            ResourceManager.RemoveAllAssets();
            sceneManager.mouseDelegate -= Mouse_BottonPressed;
        }

        public void Mouse_BottonPressed(MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                //checks to see if mouse button has been clicked.
                case MouseButton.Left:
                    closing = true;
                    break;
            }
        }
    }
}