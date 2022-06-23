using OpenTK;

namespace OpenGL_Game.Components
{
    class ComponentVelocity : IComponent
    {
        //sets the velocity of entities that need it
        Vector3 velocity;
        public ComponentVelocity(float x, float y, float z)
        {
            velocity = new Vector3(x, y, z);
        }

        public ComponentVelocity(Vector3 vel)
        {
            velocity = vel;
        }

        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_VELOCITY; }
        }
    }
}
