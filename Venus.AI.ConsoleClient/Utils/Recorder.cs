using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.ConsoleClient.Utils
{
    static class Recorder
    {
        private static WaveInEvent waveIn;
        private static WaveFileWriter writer;
        private static string _outputFilename = "demo.wav";

        public static void StartRecord()
        {
            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.DataAvailable += waveIn_DataAvailable;
            waveIn.RecordingStopped += new EventHandler<StoppedEventArgs>(waveIn_RecordingStopped);
            waveIn.WaveFormat = new WaveFormat(16000, 1);
            writer = new WaveFileWriter(_outputFilename, waveIn.WaveFormat);
            waveIn.StartRecording();
        }

        public static void StopRecord()
        {
            waveIn.StopRecording();
        }

        private static void waveIn_RecordingStopped(object sender, EventArgs e)
        {
            waveIn.Dispose();
            waveIn = null;
            writer.Close();
            writer.Dispose();
            writer = null;
        }

        private static void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            writer.Write(e.Buffer, 0, e.BytesRecorded);
            writer.Flush();
        }
    }
}
