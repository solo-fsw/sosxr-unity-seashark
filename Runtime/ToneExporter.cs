using System.IO;
using System.Text;
using UnityEngine;


public static class ToneExporter
{
    public static void GenerateAndSaveWav(int frequency, float amplitude, int duration, string path)
    {
        var sampleRate = 44100;
        var sampleCount = Mathf.CeilToInt(sampleRate * duration / 1000f);
        var samples = new float[sampleCount];

        // generate sine wave
        double phase = 0;
        var increment = frequency * 2.0 * Mathf.PI / sampleRate;

        for (var i = 0; i < sampleCount; i++)
        {
            samples[i] = Mathf.Sin((float) phase) * amplitude;
            phase += increment;

            if (phase > 2.0 * Mathf.PI)
            {
                phase -= 2.0 * Mathf.PI;
            }
        }

        // convert to 16-bit PCM
        var bytes = new byte[sampleCount * 2];

        for (var i = 0; i < sampleCount; i++)
        {
            var val = (short) (samples[i] * short.MaxValue);
            bytes[i * 2] = (byte) (val & 0xff);
            bytes[i * 2 + 1] = (byte) ((val >> 8) & 0xff);
        }

        // write WAV header
        using var fs = new FileStream(path, FileMode.Create);

        using var bw = new BinaryWriter(fs);

        var subChunk2Size = bytes.Length;
        var chunkSize = 36 + subChunk2Size;

        bw.Write(Encoding.UTF8.GetBytes("RIFF"));
        bw.Write(chunkSize);
        bw.Write(Encoding.UTF8.GetBytes("WAVE"));
        bw.Write(Encoding.UTF8.GetBytes("fmt "));
        bw.Write(16);
        bw.Write((short) 1); // PCM
        bw.Write((short) 1); // mono
        bw.Write(sampleRate);
        bw.Write(sampleRate * 2); // byte rate
        bw.Write((short) 2); // block align
        bw.Write((short) 16); // bits per sample
        bw.Write(Encoding.UTF8.GetBytes("data"));
        bw.Write(subChunk2Size);
        bw.Write(bytes);
    }
}