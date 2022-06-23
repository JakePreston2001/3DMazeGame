using System.Collections.Generic;
using OpenGL_Game.Objects;

namespace OpenGL_Game.Managers
{
    enum COLLISIONTYPE 
    {
        SPHERE_SPHERE,
        LINE_LINE,
    }

    struct Collision 
    {
        public Entity entity;
        public COLLISIONTYPE collisionType;
    }

    abstract class CollisionManager
    {
        //creates a collision manifold and method to clear it
        protected List<Collision> collisionManifold = new List<Collision>();
        public void ClearManifold() { collisionManifold.Clear(); }
        //creates a method between the camera and collision sphere of the entity
        public void CollisionBetweenCamera(Entity entity, COLLISIONTYPE collisionTpye)
        {
            //checks to see if the entity collided with is the one in the list and adds it to the list
            foreach (Collision coll in collisionManifold)
            {
                if (coll.entity == entity) return;
            }
            Collision collision;
            collision.entity = entity;
            collision.collisionType = collisionTpye;
            collisionManifold.Add(collision);
        }

        public abstract void ProcessCollisions();

    }
}
