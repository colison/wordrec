using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Engine
{
    class MP3convert
    {
        public static void ConvertMp3ToWav(string _inPath_, string _outPath_)
        {
            using (Mp3FileReader mp3 = new Mp3FileReader(_inPath_))
            {
                using (WaveStream pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(_outPath_, pcm);
                    mp3.Close();
                }
            }
        }
    }
}
