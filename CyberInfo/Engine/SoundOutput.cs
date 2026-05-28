using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace CyberInfo.Engine
{
    /// <summary>
    /// Plays the WAV greeting file.
    /// Adapted from Part 1 SoundOutput.cs — silent on failure (WPF has no console output).
    /// </summary>
    public static class SoundOutput
    {
        public static void PlayGreetingAsync()
        {
            string? wav = FindWav();
            if (wav is null) return;

            // Run on background thread so UI doesn't block
            Task.Run(() =>
            {
                try
                {
                    using var player = new SoundPlayer(wav);
                    player.PlaySync();
                }
                catch { /* Silent failure — audio is non-critical */ }
            });
        }

        private static string? FindWav()
        {
            string res = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            foreach (string n in new[] { "Greeting AI .wav", "Greeting AI.wav", "GreetingAI.wav", "greeting ai.wav" })
            {
                string p = Path.Combine(res, n);
                if (File.Exists(p)) return p;
            }
            if (Directory.Exists(res))
                foreach (string f in Directory.EnumerateFiles(res, "*.wav"))
                    return f;
            return null;
        }
    }
}
