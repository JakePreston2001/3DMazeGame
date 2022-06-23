using OpenGL_Game.Components;
using OpenGL_Game.Objects;
using OpenTK;


namespace OpenGL_Game.Managers
{
    class AudioManager
    {
        //creates an audio component and source position vector for the audio
        ComponentAudio audio;
        Vector3 sourcePos;
        public AudioManager(Entity entity)
        {
            //gets the position of the audio
            IComponent positionComponent = entity.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_POSITION;
            });
            //sets the source position
            sourcePos = ((ComponentPosition)positionComponent).Position;
            //gets the audio component
            IComponent audioComponent = entity.Components.Find(delegate (IComponent component)
            {
                return component.ComponentType == ComponentTypes.COMPONENT_AUDIO;
            });
            //sets the audio to be the entities audio
            audio = (ComponentAudio)audioComponent;
            audio.setPosition(sourcePos);
        }
        //calls methods to close, set position, pause/play and whether the audio should be looped or not
        public void close()
        {
            audio.Close();
        }
        public void setPosition(Vector3 sourcePos)
        {
            audio.setPosition(sourcePos);
        }
        public void PauseNoise()
        {
            audio.PauseAudio();
        }
        public void PlayNoise()
        {
            audio.PlayAudio();

        }
        public void LoopNoise(bool loop)
        {
            audio.LoopAudio(loop);
        }
    }
}
