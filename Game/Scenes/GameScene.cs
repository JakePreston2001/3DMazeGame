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
    class GameScene : Scene
    {
        //Delta time initiated
        public static float dt = 0;

        //Entity Managers created
        EntityManager entityManager;
        SystemManager systemManager;
        CollisionManager collisionGameManager;
        MapManager mapManager;

        //Audio Managers created
        AudioManager droneAudio;
        AudioManager portalAudio;
        AudioManager KeyAudio;
        AudioManager lifeLostAudio;
        AudioManager gameMusic;

        //Entities created
        Entity newEntity;
        public Entity droneEntity;
        Entity portalEntity;
        public Entity KeyPicked;
        public Entity WallHit;

        //objects created
        Entity Skybox;
        SkyBox skybox;
        Wall wall;

        //Camera instantiated
        public Camera camera;

        //GameScene instantiated
        public static GameScene gameInstance;
        public static GameOverScene gameOverInstance;

        //for movement
        bool[] keysPressed = new bool[255];
        float turnSpeed = 2;
        float moveSpeed = 5;
        bool soundPlaying = false;

        //variables needed for the players lives
        int lives = 3;
        public bool gameOver = false;
        public bool LifeLost = false;

        //variables needed for the key collection
        public bool keyCollected = false;
        int keys = 0;
        int KeysToWin;

        //creates a vector to save the portal position for when it gets switched on
        Vector3 PortalPos;

        //creates variables used for the drone moving
        public Vector3 droneTarget;
        Vector3 dronePos;
        float droneSpeed = 0.08f;

        //used for when the game has finished
        bool closing;

        //used for wall collisions
        bool collideWithWall = true;
        public bool wallCollision = false;
        public bool collided = false;

        bool toggleDrone = false;
        bool droneMovement = true;
        bool toggleCollisionWall = true;
        public string mapName = "Game/Maps/map1.txt";
        float stamina = 5f;
        float maxStamina = 5f;
        bool running = false;
        bool charged = true;
        float runSpeed = 10;
        float walkSpeed = 5;

        //used for portal activation
        bool portalActive = false;
        bool portalSound = false;
        public bool portalTP = false;

        //creates an instance of the map
        public char[,] map;

        //creates a vector to keep track of the camera's start position
        Vector3 CameraStartPos;
        
        //creates a float for use in shaders (when using delta time, it turned everything red for some reason)
        float Time;

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            //sets up the managers used
            gameInstance = this;
            gameOverInstance = GameOverScene();
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            collisionGameManager = new GameCollisionManager();
            mapManager = new MapManager();
            skybox = new SkyBox();
            wall = new Wall();

            //loads the map into a 2D array
            try
            {
                if (GameOverScene.GameOverInstance.gameFinished || GameOverWinScene.GameOverWinInstance.gameFinished)
                {
                    mapName = "Game/Maps/map2.txt";
                }
            }
            catch { }
            map = mapManager.loadMap(mapName);

            // Set the title of the window
            sceneManager.Title = "Game";

            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;

            // Set Keyboard events to go to a method in this class
            sceneManager.keyboardDownDelegate += Keyboard_KeyDown;
            sceneManager.keyboardUpDelegate += Keyboard_KeyUp;

            // Enable Depth Testing
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            //background colour
            GL.ClearColor(0.2f, 0.2f, 0.5f, 1.0f);

            // Set Camera
            camera = new Camera(new Vector3(0, 1.2f, 1), new Vector3(0, 1.2f, 0), (float)(sceneManager.Width) / (float)(sceneManager.Height), 0.1f, 10000f);

            //creates the entities needed at the start of the game.
            CreateEntities();
            CreateSystems();
        }

        private void CreateEntities()
        {
            //creates counters needed to make sure the map gets created to the right size
            int WallCounter = 0;
            int FloorCounter = 0;
            int KeyCounter = 0;
            float x = 0f;
            float z = 0f;
            //gets the mid point of the map, each block is 2 units wide so getting the mid point is just the length and width
            Vector3 mid = new Vector3(map.GetLength(0), 0, map.GetLength(1));
            //a for loop goes through the map and checks to see what each point is
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    //if the position is an 'X' it means there should be a wall there
                    if (map[i, j] == 'X')
                    {
                        //entity created for the wall.
                        newEntity = new Entity("wall" + WallCounter);
                        newEntity.AddComponent(new ComponentPosition(x, 0.0f, z));
                        newEntity.AddComponent(new ComponentGeometry("Game/Geometry/jakewall/wall.obj"));
                        newEntity.AddComponent(new ComponentCollisionSphere(1f));
                        newEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(newEntity);
                        //wall counter increased so that each entity gets a unique identifier
                        WallCounter++;
                    }
                    //if the position in the map is not a wall, a floor block is placed,
                    if (map[i, j] != 'X')
                    {
                        //entity for floor created
                        newEntity = new Entity("floor" + FloorCounter);
                        newEntity.AddComponent(new ComponentPosition(x, -2.0f, z));
                        newEntity.AddComponent(new ComponentGeometry("Game/Geometry/Floor/floor.obj"));
                        newEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(newEntity);
                        //floor counter increased
                        FloorCounter++;
                    }
                    //checks to see if a key should be placed here
                    if (map[i, j] == 'K')
                    {
                        //creates a key entity
                        newEntity = new Entity("key" + KeyCounter);
                        newEntity.AddComponent(new ComponentPosition(x, 0.0f, z));
                        newEntity.AddComponent(new ComponentGeometry("Game/Geometry/Key/key.obj"));
                        newEntity.AddComponent(new ComponentAudio("Game/Audio/pickUp.wav"));
                        newEntity.AddComponent(new ComponentCollisionSphere(2f));
                        newEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(newEntity);
                        //increases the key counter and number of keys needed to activate the portal
                        KeyCounter++;
                        KeysToWin++;
                    }
                    //checks to see where the drone entity should be placed
                    if (map[i, j] == 'D')
                    {
                        //drone entity created
                        droneEntity = new Drone("Drone");
                        droneEntity.AddComponent(new ComponentPosition(x, 1.2f, z));
                        droneEntity.AddComponent(new ComponentGeometry("Game/Geometry/Drone/drone.obj"));
                        droneEntity.AddComponent(new ComponentAudio("Game/Audio/droneNoise.wav"));
                        droneEntity.AddComponent(new ComponentCollisionSphere(1f));
                        droneEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(droneEntity);
                        droneEntity = entityManager.ReturnEntity("Drone");
                        //audio manager for drone created
                        droneAudio = new AudioManager(droneEntity);
                        dronePos = new Vector3(x, 1.2f, z);
                    }
                    //checks to see whether the portal should be created here
                    if (map[i, j] == 'P')
                    {
                        newEntity = new Entity("Portal");
                        newEntity.AddComponent(new ComponentPosition(x, -1, z));
                        newEntity.AddComponent(new ComponentGeometry("Game/Geometry/Portal/portalOff.obj"));
                        newEntity.AddComponent(new ComponentAudio("Game/Audio/portalNoise.wav"));
                        newEntity.AddComponent(new ComponentCollisionSphere(1f));
                        newEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(newEntity);
                        portalEntity = entityManager.ReturnEntity("Portal");
                        portalAudio = new AudioManager(portalEntity);
                        PortalPos = new Vector3(x, -1, z);
                    }
                    //checks to see where the camera should be translated to
                    if (map[i, j] == 'C')
                    {
                        Vector3 cameraPos = new Vector3(x, 0, z);
                        CameraStartPos = cameraPos;
                        camera.Translate(cameraPos);
                    }
                    //sets walls around the map, creating an illusion of being stuck in a maze
                    if (j == 0 || j == (map.GetLength(1) - 1))
                    {
                        newEntity = new Entity("wall" + WallCounter);
                        newEntity.AddComponent(new ComponentPosition(x, 2.0f, z));
                        newEntity.AddComponent(new ComponentGeometry("Game/Geometry/jakewall/wall.obj"));
                        newEntity.AddComponent(new ComponentCollisionSphere(1f));
                        newEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(newEntity);
                        WallCounter++;
                        newEntity = new Entity("wall" + WallCounter);
                        newEntity.AddComponent(new ComponentPosition(x, 4.0f, z));
                        newEntity.AddComponent(new ComponentGeometry("Game/Geometry/jakewall/wall.obj"));
                        newEntity.AddComponent(new ComponentCollisionSphere(1f));
                        newEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(newEntity);
                        WallCounter++;
                    }
                    if (i == 0 || i == (map.GetLength(0) - 1))
                    {
                        newEntity = new Entity("wall" + WallCounter);
                        newEntity.AddComponent(new ComponentPosition(x, 2.0f, z));
                        newEntity.AddComponent(new ComponentGeometry("Game/Geometry/jakewall/wall.obj"));
                        newEntity.AddComponent(new ComponentCollisionSphere(0.8f));
                        newEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(newEntity);
                        WallCounter++;
                        newEntity = new Entity("wall" + WallCounter);
                        newEntity.AddComponent(new ComponentPosition(x, 4.0f, z));
                        newEntity.AddComponent(new ComponentGeometry("Game/Geometry/jakewall/wall.obj"));
                        newEntity.AddComponent(new ComponentCollisionSphere(0.8f));
                        newEntity.AddComponent(new ComponentShaderDefault());
                        entityManager.AddEntity(newEntity);
                        WallCounter++;
                    }
                    //increased the x position by 2 so that the entities don't overlap
                    x += 2f;
                }
                //sets x back to 0 so the entities are created at the right position
                x = 0f;
                //increases z by 2 - moving a row down in the map
                z += 2f;
            }
            
            //creates the entity for the skybox, it has an audio component for game music.
            newEntity = new Entity("SkyBox");
            newEntity.AddComponent(new ComponentPosition(mid));
            newEntity.AddComponent(new ComponentGeometry("Game/Geometry/Skybox/skyboxLarge.obj"));
            newEntity.AddComponent(new ComponentShaderNoLights());
            newEntity.AddComponent(new ComponentAudio("Game/Audio/Secunda.wav"));
            entityManager.AddEntity(newEntity);
            Skybox = entityManager.ReturnEntity("SkyBox");
            gameMusic = new AudioManager(Skybox);

            //creates the audio entity for losing a life
            newEntity = new Entity("LifeLost");
            newEntity.AddComponent(new ComponentPosition(0, 0, 0));
            newEntity.AddComponent(new ComponentAudio("Game/Audio/lifeLost.wav"));
            entityManager.AddEntity(newEntity);
            lifeLostAudio = new AudioManager(newEntity);
        }
        //creates a method to create an entite for when the portal gets activated
        public void TurnPortalOn()
        {
            newEntity = new Entity("PortalOn");
            newEntity.AddComponent(new ComponentPosition(PortalPos));
            newEntity.AddComponent(new ComponentGeometry("Game/Geometry/Portal/portalOn.obj"));
            newEntity.AddComponent(new ComponentCollisionSphere(2f));
            newEntity.AddComponent(new ComponentShaderDefault());
            entityManager.AddEntity(newEntity);
        }
        //creates the systems used
        private void CreateSystems()
        {
            ISystem newSystem;

            newSystem = new SystemRender();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemPhysics();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemCollisions(collisionGameManager, camera);
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
            dt = (float)e.Time;
            //System.Console.WriteLine("fps=" + (int)(1.0/dt));

            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed)
                sceneManager.Exit();

            // TODO: Add your update logic here
            collisionGameManager.ProcessCollisions();

            AL.Listener(ALListener3f.Position, ref camera.cameraPosition);
            AL.Listener(ALListenerfv.Orientation, ref camera.cameraDirection, ref camera.cameraUp);


            //moves the skybox to the position of the player, to give the illusion of a large space
            skybox.Move(Skybox, camera.cameraPosition);
            #region Key presses
            //checks to see if the user is pressing the left shift key, and increased their speed
            if (keysPressed[(char)Key.ShiftLeft] && charged) {  running = true; }
            if (running)
            {
                if (stamina <= 0)
                { 
                    running = false;
                    moveSpeed = walkSpeed;
                    charged = false;
                }
                else if (charged)
                {
                    moveSpeed = runSpeed;
                    stamina -= (float)e.Time;
                }
            }
            if (!running)
            {
                stamina += (float)e.Time;
                if (stamina >= maxStamina) 
                { 
                    stamina = maxStamina;
                    charged = true;
                }
            }
            //checks to see the users key inputs for camera movement
            if (keysPressed[(char)Key.W])
            {
                camera.MoveForward(moveSpeed * dt);
            }
            if (keysPressed[(char)Key.S])
            {
                camera.MoveForward(-moveSpeed * dt);
            }
            if (keysPressed[(char)Key.A])
            {
                camera.RotateY(-turnSpeed * dt);
            }
            if (keysPressed[(char)Key.D])
            {
                camera.RotateY(turnSpeed * dt);
            }
            //used to close the game,
            if (keysPressed[(char)Key.M])
            {
                closing = true;
            }
            //used to activate the portal without needing all the keys
            if (keysPressed[(char)Key.C])
            {
                portalActive = true;
            }
            if (keysPressed[(char)Key.Number1] && !toggleDrone)
            {
                if (droneMovement)
                {
                    droneMovement = false;
                }
                else if (!droneMovement)
                {
                    droneMovement = true;
                }
                toggleDrone = true;
            }

            if (keysPressed[(char)Key.Number2] && !toggleCollisionWall)
            {
                if (collideWithWall)
                {
                    collideWithWall = false;
                }
                else if (!collideWithWall)
                {
                    collideWithWall = true;
                }
                toggleCollisionWall = true;
            }

            #endregion
            #region other Updates
            //checks to see if a wall has been hit
            if (wallCollision && collideWithWall)
            {
                hitWall(WallHit);
                wallCollision = false;
            }
            //checks to see if the game is closing
            if (closing)
            {
                Close();
                sceneManager.ChangeScene(SceneTypes.SCENE_GAME_OVER);
            }
            //checks to see if the audio for the game and drone that needs playing from the beginning
            if (!soundPlaying)
            {
                droneAudio.PlayNoise();
                gameMusic.PlayNoise();
                soundPlaying = true;
            }
            //checks to see if the portal is active and sound is off to start playing the portal noise
            if (portalActive && !portalSound)
            {
                portalAudio.PlayNoise();
                TurnPortalOn();
                portalSound = true;
            }
            //checks to see if a key has been picked up
            if (keyCollected && KeyPicked != null)
            {
                //plays the noise for picking up the key
                KeyAudio = new AudioManager(KeyPicked);
                KeyAudio.LoopNoise(false);
                KeyAudio.PlayNoise();
                entityManager.RemoveEntity(KeyPicked);
                keys += 1;
                keyCollected = false;
                KeyPicked = null;
            }
            //checks to see if the user has lost a life
            if (LifeLost == true)
            {
                loseLife();
                LifeLost = false;
            }
            //checks to see if the user has lost the game
            if (lives == 0)
            {
                Close();
                sceneManager.ChangeScene(SceneTypes.SCENE_GAME_OVER);
            }
            //checks to see if the player has the keys to activate the portal
            if (keys == KeysToWin)
            {
                portalActive = true;
            }
            //checks to see if the portal is active and has been used.
            if (portalTP && portalActive)
            {
                closing = true;
                Close();
                sceneManager.ChangeScene(SceneTypes.SCENE_GAME_WON);
            }
            #endregion
            //moves the drone towards the player
            if (droneMovement)
            {
                moveDrone();
            }

        }

        public void moveDrone() 
        {
            //creates a drone entity
            Entity findDrone = entityManager.ReturnEntity("Drone");
            //gets its position component
            IComponent positionComponent = findDrone.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
            });
            //sets a the current position of the drone and holds it
            Vector3 currentPos = ((ComponentPosition)positionComponent).Position;

            Drone droneEntity = (Drone)findDrone;
            //choses the next location on the map
            droneEntity.getNextGrid(map, camera.cameraPosition);
            //moves the drone in the right direction
            if (currentPos.X < droneTarget.X)
            {
                currentPos.X += droneSpeed;
                ((ComponentPosition)positionComponent).Position = currentPos;
            }
            else if (currentPos.X > droneTarget.X)
            {
                currentPos.X -= droneSpeed;
                ((ComponentPosition)positionComponent).Position = currentPos;
            }
            if (currentPos.Z < droneTarget.Z)
            {
                currentPos.Z += droneSpeed;
                ((ComponentPosition)positionComponent).Position = currentPos;
            }
            else if (currentPos.Z > droneTarget.Z)
            {
                currentPos.Z -= droneSpeed;
                ((ComponentPosition)positionComponent).Position = currentPos;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            //increases the time by 1 each call
            Time += 1;
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            // Action ALL systems
            systemManager.ActionSystems(entityManager);
            
            // Render the number of keys collected and the number of lives left.
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.clearColour = Color.Transparent;
            if (lives == 3)
            {
                GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), "Lives: " + lives, 18, StringAlignment.Near, Color.Green);
            }
            if (lives == 2)
            {
                GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), "Lives: " + lives, 18, StringAlignment.Near, Color.Yellow);
            }
            if (lives == 1)
            {
                GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), "Lives: " + lives, 18, StringAlignment.Near, Color.Red);
            }
            if (keys == KeysToWin)
            {
                GUI.Label(new Rectangle(0, 30, (int)width, (int)(fontSize * 2f)), "Keys: " + keys, 18, StringAlignment.Near, Color.Green);
            }
            else 
            {
                GUI.Label(new Rectangle(0, 30, (int)width, (int)(fontSize * 2f)), "Keys: " + keys, 18, StringAlignment.Near, Color.Gold);
            }
            Rectangle rectangle = new Rectangle(10, 10, 10, 10);
           
            GUI.Label(new Rectangle(0, 60, (int)width, (int)(fontSize * 2f)), "Stamina: " + Math.Round(stamina, 2), 18, StringAlignment.Near, Color.Blue);
            
            
            //sets the variable "time" in the fragment shader
            GL.VertexAttrib1(4, Time);
            GUI.Render();
        }
        
        public void loseLife() 
        {
            //removes a life
            lives -= 1;
            Entity findDrone = entityManager.ReturnEntity("Drone");
            IComponent positionComponent = findDrone.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
            });
            ComponentPosition position = (ComponentPosition)positionComponent;
            position.Position = dronePos;
            stamina = maxStamina;
            running = false;
            moveSpeed = walkSpeed;
            //creates a vector for the camera to be reset
            Vector3 newCamPos = new Vector3(0,0,0);
            //resets the camera
            newCamPos.X = CameraStartPos.X - camera.cameraPosition.X;
            newCamPos.Y = 0;
            newCamPos.Z = CameraStartPos.Z - camera.cameraPosition.Z;
            //translates the camera back to the start
            camera.Translate(newCamPos);
            //makes sure the life lost noise doesn't loop
            lifeLostAudio.LoopNoise(false);
            //sets the audio position to the new camera position
            lifeLostAudio.setPosition(CameraStartPos);
            //plays the noise
            lifeLostAudio.PlayNoise();
        }
        
        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        { 
            //removes the keyboard delegates
            sceneManager.keyboardDownDelegate -= Keyboard_KeyDown;
            sceneManager.keyboardUpDelegate -= Keyboard_KeyUp;
            //closes the games audio
            gameMusic.close();
            droneAudio.close();
            portalAudio.close();
            lifeLostAudio.close();
            //removes the assets from the resource manager
            ResourceManager.RemoveAllAssets();
            //clears all current collisions
            collisionGameManager.ClearManifold();
        }
        
        public void Keyboard_KeyUp(KeyboardKeyEventArgs e)
        {
            //checks to see if a key has been released
            keysPressed[(char)e.Key] = false;
            //sets the movement speed to the default of 10
            moveSpeed = walkSpeed;
            running = false;
            //sets the toggles back to false so that the button can only be pressed once before it can be used again, holding down the
            //button won't constantly change the collisions
            toggleDrone = false;
            toggleCollisionWall = false;
        }
        public void Keyboard_KeyDown(KeyboardKeyEventArgs e)
        {
            //checks to see if a key has been pressed
            keysPressed[(char)e.Key] = true;
        }

        public void hitWall(Entity entity)
        {
            //calls the "move" function to move the player back from the wall
            wall.Move(entity, camera);
        }
    }
}