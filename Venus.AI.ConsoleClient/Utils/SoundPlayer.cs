using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Venus.AI.ConsoleClient.Utils
{
    public static class SoundPlayer
    {
        static WaveFileReader wav;
        static WaveOutEvent output;
        public static void Play(string path)
        {
            wav = new WaveFileReader(path);
            output = new WaveOutEvent();
            output.Init(wav);
            output.PlaybackStopped += Output_PlaybackStopped;
            output.Play();
        }

        private static void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            output.Stop();
            output.Dispose();
            output.Dispose();
            wav.Dispose();
        }
    }
}
