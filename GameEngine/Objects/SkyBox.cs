using OpenGL_Game.Components;
using OpenTK;

namespace OpenGL_Game.Objects
{
    class SkyBox
    {
        //moves the skybox to the camera's position
        public void Move(Entity entity, Vector3 cameraPos)
        {
            IComponent positionComponent = entity.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
            });
            Vector3 position = ((ComponentPosition)positionComponent).Position;
            ((ComponentPosition)positionComponent).Position = cameraPos;
        }
    }
}
