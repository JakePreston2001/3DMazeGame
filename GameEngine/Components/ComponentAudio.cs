using OpenGL_Game.Managers;
using OpenTK.Audio.OpenAL;
using OpenTK;
using System;

namespace OpenGL_Game.Components
{
    class ComponentAudio : IComponent
    {
        int audioBuffer;
        int audioSource;

        //creats an audio component or
        public ComponentAudio(string audioName)
        {
            audioBuffer = ResourceManager.LoadAudio(audioName);
            audioSource = AL.GenSource();
            AL.Source(audioSource, ALSourcei.Buffer, audioBuffer); // attach the buffer to a source
            AL.Source(audioSource, ALSourceb.Looping, true); // source loops infinitely, the default
        }
        //sets the audios position
        public void setPosition(Vector3 emitterPosition)
        {
            AL.Source(audioSource, ALSource3f.Position, ref emitterPosition);
        }
        //closes the audio
        public void Close()
        {
            AL.SourceStop(audioSource);
            AL.DeleteSource(audioSource);
            AL.DeleteBuffer(audioBuffer);
        }
        //plays the audio
        public void PlayAudio() 
        {
            AL.SourcePlay(audioSource); 
        }
        //pauses the audio
        public void PauseAudio()
        {
            AL.SourcePause(audioSource);
        }
        //turns looping on/off if needed
        public void LoopAudio(bool Loop)
        {
            AL.Source(audioSource, ALSourceb.Looping, Loop);
        }
        public int Audio
        {
            get { return Audio; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO; }
        }
    }
}
