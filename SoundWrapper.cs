using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Lame;
using System.IO;
using System.Media;

namespace KarteiKartenLernen
{
    class SoundWrapper
    {
        public SoundWrapper(string sound_file)
        {
            if (sound_file.EndsWith(".wav"))
            {
                wav_player = new SoundPlayer(sound_file);
            }
            else if (sound_file.EndsWith(".mp3"))
            {
                Mp3FileReader mp3File = new Mp3FileReader(sound_file);
                mp3_player = new WaveOutEvent();
                mp3_player.Init(mp3File);
            }
        }

        public void Play()
        {
            wav_player?.Play();
            mp3_player?.Play();
        }

        private SoundPlayer wav_player = null;
        private WaveOutEvent mp3_player = null;

    }
}
