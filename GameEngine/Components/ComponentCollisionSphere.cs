using OpenGL_Game.Managers;
using OpenGL_Game.OBJLoader;
using OpenTK;


namespace OpenGL_Game.Components
{
    //gets the radius of the collision sphere
    class ComponentCollisionSphere : IComponent
    {
        float radius;

        public ComponentCollisionSphere(float r)
        {
            radius = r;
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_COLLISION_SPHERE; }
        }
    }
}
