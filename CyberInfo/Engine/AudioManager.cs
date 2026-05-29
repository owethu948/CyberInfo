using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace CyberInfo.Engine
{
    public static class AudioManager
    {
        private static string? _cachedWavPath;

        /// <summary>
        /// Asynchronously plays the greeting audio file (Greeting-AI.wav).
        /// Handles file location resolution and graceful fallback if file is not found.
        /// </summary>
        public static void PlayGreetingAsync()
        {
            string? wav = _cachedWavPath ??= FindWav();
            if (wav is null)
            {
                Debug.WriteLine("[AudioManager] Greeting audio file not found. Continuing without audio.");
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    using var player = new SoundPlayer(wav);
                    Debug.WriteLine($"[AudioManager] Playing greeting from: {wav}");
                    player.PlaySync();
                    Debug.WriteLine("[AudioManager] Greeting audio playback completed.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[AudioManager] Error playing audio: {ex.Message}");
                    // Silent fail - don't disrupt application startup
                }
            });
        }

        /// <summary>
        /// Locates the Greeting-AI.wav file using multiple strategies:
        /// 1. Check Resources folder next to the executable (deployed scenario)
        /// 2. Check hardcoded development path (local development scenario)
        /// </summary>
        private static string? FindWav()
        {
            // Strategy 1: Look in Resources folder relative to application base directory
            string resourcesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            if (Directory.Exists(resourcesDir))
            {
                Debug.WriteLine($"[AudioManager] Searching in: {resourcesDir}");

                // Primary search: exact filename
                foreach (string filename in new[] { "Greeting-AI.wav", "Greeting AI.wav", "Greeting AI .wav", "greeting.wav" })
                {
                    string fullPath = Path.Combine(resourcesDir, filename);
                    if (File.Exists(fullPath))
                    {
                        Debug.WriteLine($"[AudioManager] Found audio file: {fullPath}");
                        return fullPath;
                    }
                }

                // Secondary search: any .wav file in the folder
                var wavFiles = Directory.GetFiles(resourcesDir, "*.wav");
                if (wavFiles.Length > 0)
                {
                    Debug.WriteLine($"[AudioManager] Found fallback .wav file: {wavFiles[0]}");
                    return wavFiles[0];
                }
            }

            // Strategy 2: Fallback to explicit absolute path (development scenario)
            string devPath = @"C:\Users\Lisakhanya Jonas\source\repos\CyberInfo\CyberInfo\Resources\Greeting-AI.wav";
            if (File.Exists(devPath))
            {
                Debug.WriteLine($"[AudioManager] Found audio file at development path: {devPath}");
                return devPath;
            }

            Debug.WriteLine("[AudioManager] Audio file not found in any expected location.");
            return null;
        }
    }
}