using OpenGL_Game.Objects;
using OpenGL_Game.Scenes;
using System;


namespace OpenGL_Game.Managers
{
    class GameCollisionManager : CollisionManager
    {
        //goes through and checks to see what the user has collided with
        public override void ProcessCollisions()
        {
            foreach (Collision Coll in collisionManifold)
            {
                if (Coll.collisionType == COLLISIONTYPE.SPHERE_SPHERE) 
                {
                    //returns variables based on what the user has hit
                    if (Coll.entity.Name == "Drone" && GameScene.gameInstance.gameOver == false)
                    {
                        GameScene.gameInstance.LifeLost = true;
                    }
                    if (Coll.entity.Name.Contains("key"))
                    {
                        GameScene.gameInstance.keyCollected = true;
                        GameScene.gameInstance.KeyPicked = Coll.entity;
                    }
                    if (Coll.entity.Name == "PortalOn")
                    {
                        Console.WriteLine("Portal hit");
                        GameScene.gameInstance.portalTP = true;
                    }
                    if (Coll.entity.Name.Contains("wall"))
                    {
                        GameScene.gameInstance.wallCollision = true;
                        GameScene.gameInstance.WallHit = Coll.entity;
                    }
                }
            }
            //clears the manifold after each frame as it would keep collisions that shouldn't be there
            ClearManifold();
        }
    }
}
