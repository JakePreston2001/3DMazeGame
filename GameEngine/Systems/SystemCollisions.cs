using System.Collections.Generic;
using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenGL_Game.Managers;


namespace OpenGL_Game.Systems
{
    class SystemCollisions : ISystem
    {
        const ComponentTypes MASK = (ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_COLLISION_SPHERE);

        //private ComponentCollisionSphere collComponent;
        private CollisionManager collisionManager;
        private Camera camera;

        public SystemCollisions() { }
        public SystemCollisions(CollisionManager collisionManager, Camera camera)
        {
    
            this.collisionManager = collisionManager;
            this.camera = camera;
        }
        
      
        public string Name
        {
            get { return "SystemCollisions"; }
        }



        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                List<IComponent> components = entity.Components;

                IComponent collComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_COLLISION_SPHERE;
                });
                ComponentCollisionSphere collision = (ComponentCollisionSphere)collComponent;

                IComponent positionComponent = components.Find(delegate (IComponent component)
                {
                    return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
                });
                ComponentPosition position = (ComponentPosition)positionComponent;
                Collision(entity, position, collision);       
            }
        }
   
        public void Collision(Entity entity, ComponentPosition position,
                                    ComponentCollisionSphere coll)
        {
            if ((position.Position - camera.cameraPosition).Length < coll.Radius + camera.Radius)
            {
                collisionManager.CollisionBetweenCamera(entity, COLLISIONTYPE.SPHERE_SPHERE);
            }
        }

    }
}
