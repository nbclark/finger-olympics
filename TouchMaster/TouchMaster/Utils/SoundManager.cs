using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace MobileSrc.FantasticFingerFun
{
    static class SoundManager
    {
        public static void Play(SoundEffect sound)
        {
            if (Settings.Instance.SoundsEnabled)
            {
                sound.Play();
            }
        }

        public static void Play(SoundEffect sound, float volume, float pitch, float pan)
        {
            if (Settings.Instance.SoundsEnabled)
            {
                sound.Play(volume, pitch, pan);
            }
        }

        public static void Play(SoundEffectInstance sound)
        {
            if (Settings.Instance.SoundsEnabled)
            {
                sound.Play();
            }
        }

        public static void Stop(SoundEffectInstance sound)
        {
            if (Settings.Instance.SoundsEnabled)
            {
                sound.Stop();
            }
        }
    }
}
