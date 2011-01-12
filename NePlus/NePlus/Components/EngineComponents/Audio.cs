using Microsoft.Xna.Framework.Audio;

namespace NePlus.Components.EngineComponents
{
    public class Audio : Component
    {
        private AudioEngine audioEngine;
        private SoundBank soundBank;
        private WaveBank waveBank;

        public Audio(Engine engine)
            : base(engine)
        {
            audioEngine = new AudioEngine("Content\\Audio\\NePlusAudio.xgs");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
        }

        public override void Update()
        {
            audioEngine.Update();
        }

        public Cue PlaySound(string sound)
        {
            Cue cue = soundBank.GetCue(sound);
            cue.Play();
            return cue;
        }
    }
}
