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
        public SoundWrapper(string in_sound_file)
        {
            sound_file = in_sound_file;
        }

        public void Play()
        {
            if (sound_file.EndsWith(".wav"))
            {
                SoundPlayer wav_player = new SoundPlayer(sound_file);
                wav_player.Play();
            }
            else if (sound_file.EndsWith(".mp3"))
            {
                Mp3FileReader mp3File = new Mp3FileReader(sound_file);
                WaveOutEvent mp3_player = new WaveOutEvent();
                mp3_player.Init(mp3File);
                mp3_player.Play();
            }
        }

        private string sound_file;
    }
}
