using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace NePlus.Components.EngineComponents
{
    public class Audio : Component
    {
        List<Cue> cues;

        private AudioEngine audioEngine;
        private SoundBank soundBank;
        private WaveBank waveBank;

        bool paused;

        public Audio(Engine engine) : base(engine)
        {
            cues = new List<Cue>();

            audioEngine = new AudioEngine("Content\\Audio\\NePlusAudio.xgs");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");

            paused = false;

            Engine.AddComponent(this);
        }

        public Cue GetCue(string sound)
        {
            Cue cue = soundBank.GetCue(sound);
            cues.Add(cue);

            return cue;
        }

        public override void Update(GameTime gameTime)
        {
            int idx = 0;
            while (idx < cues.Count)
            {
                if (cues[idx].IsStopped)
                {
                    cues[idx].Dispose();
                    cues.RemoveAt(idx);

                    continue;
                }

                ++idx;
            }

            if (paused)
            {
                // resume all cues
                foreach (Cue cue in cues)
                {
                    if (cue.IsPaused)
                    {
                        cue.Resume();
                    }
                }
            }

            audioEngine.Update();
        }

        public void Pause()
        {
            foreach (Cue cue in cues)
            {
                if (cue.IsPlaying)
                {
                    cue.Pause();
                }
            }

            paused = true;
        }
    }
}
