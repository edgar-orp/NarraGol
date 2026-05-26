using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace dev.xrlab.NarraGol.Recording
{
    public static class WavUtility
    {
        public static byte[] FromAudioClip(AudioClip clip)
        {
            using MemoryStream stream = new MemoryStream();

            int sampleCount = clip.samples * clip.channels;
            int frequency = clip.frequency;
            int channels = clip.channels;

            float[] samples = new float[sampleCount];
            clip.GetData(samples, 0);

            byte[] pcmData = ConvertToPCM16(samples);

            WriteWavHeader(stream, pcmData.Length, frequency, channels);
            stream.Write(pcmData, 0, pcmData.Length);

            return stream.ToArray();
        }

        private static byte[] ConvertToPCM16(float[] samples)
        {
            byte[] bytes = new byte[samples.Length * 2];

            for (int i = 0; i < samples.Length; i++)
            {
                float clampedSample = Mathf.Clamp(samples[i], -1f, 1f);
                short value = (short)(clampedSample * short.MaxValue);
                BitConverter.GetBytes(value).CopyTo(bytes, i * 2);
            }

            return bytes;
        }

        private static void WriteWavHeader(Stream stream, int pcmLength, int frequency, int channels)
        {
            int byteRate = frequency * channels * 2;

            using BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true);

            writer.Write(Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(36 + pcmLength);
            writer.Write(Encoding.ASCII.GetBytes("WAVE"));
            writer.Write(Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)channels);
            writer.Write(frequency);
            writer.Write(byteRate);
            writer.Write((short)(channels * 2));
            writer.Write((short)16);
            writer.Write(Encoding.ASCII.GetBytes("data"));
            writer.Write(pcmLength);
        }
    }
}