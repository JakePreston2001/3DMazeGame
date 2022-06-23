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
    class GameOverScene : Scene
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
        public static GameOverScene GameOverInstance;

        //bools used in playing the noise and closing the scene
        bool playing = false;
        bool closing = false;

        public GameOverScene(SceneManager sceneManager) : base(sceneManager)
        {
            //instantiates the scene and the managers
            GameOverInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();

            // Set the title of the window
            sceneManager.Title = "Game";

            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            sceneManager.mouseDelegate += Mouse_BottonPressed;

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

        }
        //creates an entity to play music
        private void CreateEntities()
        {
            newEntity = new Entity("Music");
            newEntity.AddComponent(new ComponentPosition(0, 1.7f, 1));
            newEntity.AddComponent(new ComponentAudio("Game/Audio/gameOverLose2.wav"));
            entityManager.AddEntity(newEntity);
            Music = new AudioManager(newEntity);
        }

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

            //checks to see if the music is playing
            if (!playing) 
            {
                //set the music looping to false
                Music.LoopNoise(false);
                Music.PlayNoise();
                playing = true;
            }
            if (closing)
            {
                //closes the scene
                Close();
                sceneManager.ChangeScene(SceneTypes.SCENE_MAIN_MENU);
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

            GUI.clearColour = Color.DarkRed;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.Label(new Rectangle(0, (int)(fontSize / 2f), (int)width, (int)(fontSize * 2f)), "Game Over", (int)fontSize, StringAlignment.Center, Color.White);
            GUI.Label(new Rectangle(0, 5*(int)(fontSize / 2f), (int)width, (int)(fontSize * 2f)), "You Lose", (int)fontSize, StringAlignment.Center, Color.White);


            GUI.Render();
        }

        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            //closes the audio
            Music.close();
            //removes the assets from the resource manager
            ResourceManager.RemoveAllAssets();
            sceneManager.mouseDelegate -= Mouse_BottonPressed;
        }

        public void Mouse_BottonPressed(MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                //checks to see if mouse has been clicked to close the scene
                case MouseButton.Left:
                    closing = true;
                    break;
            }
        }
    }
}