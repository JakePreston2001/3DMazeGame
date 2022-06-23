using OpenGL_Game.Components;
using OpenTK;
using System;

namespace OpenGL_Game.Objects
{
    class Wall
    {
        //gets the radius of the entity passed in
        public float Radius(Entity entity)
        {
            IComponent componentCollisionSphere = entity.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_COLLISION_SPHERE;
            });
            float radius = ((ComponentCollisionSphere)componentCollisionSphere).Radius;
            return radius;
        }
        //moves the person if they hit a wall
        public void Move(Entity entity, Camera camera) 
        {
            //gets the distance the camera is within the collision sphere of the wall
            IComponent collisionComponent = entity.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_COLLISION_SPHERE;
            });
            ComponentCollisionSphere collision = (ComponentCollisionSphere)collisionComponent;
            IComponent positionComponent = entity.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
            });
            ComponentPosition position = (ComponentPosition)positionComponent;
            //movest the camer back by that amount/15 to make it slightly smoother
            float distanceZ = (position.Position.Z - camera.cameraPosition.Z);
            float distanceX = (position.Position.X - camera.cameraPosition.X);
            camera.cameraPosition.X -= distanceX/15;
            camera.cameraPosition.Z -= distanceZ/15;   
        }
    }
}
