using UnityEngine;
using System.IO;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.Core
{
    public static class NarraGolConfigLoader
    {
        private const string CONFIG_FOLDER = "Config";
        private const string CONFIG_FILE_NAME = "config.json";

        private static NarraGolRuntimeConfig config;

        public static NarraGolRuntimeConfig Config
        {
            get
            {
                if (config == null)
                    LoadConfig();

                return config;
            }
        }

        public static void ReloadConfig()
        {
            LoadConfig();
        }

        private static void LoadConfig()
        {
            string configPath = GetConfigPath();

            EnsureConfigFolderExists();

            if (!File.Exists(configPath))
            {
                config = new NarraGolRuntimeConfig();
                SaveConfig(configPath, config);

                UnityEngine.Debug.Log($"Config file created: {configPath}");
                return;
            }

            try
            {
                string json = File.ReadAllText(configPath);
                config = JsonUtility.FromJson<NarraGolRuntimeConfig>(json);

                if (config == null)
                {
                    UnityEngine.Debug.LogWarning("Config file was empty or invalid. Using default config.");
                    config = new NarraGolRuntimeConfig();
                    SaveConfig(configPath, config);
                }

                ValidateConfig();

                UnityEngine.Debug.Log($"Config file loaded: {configPath}");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Could not load config file: {e.Message}");
                config = new NarraGolRuntimeConfig();
            }
        }

        private static void ValidateConfig()
        {
            if (config.ffmpeg == null)
                config.ffmpeg = new FFmpegConfig();

            if (config.webcam == null)
                config.webcam = new WebcamConfig();

            if (config.waveform == null)
                config.waveform = new WaveformConfig();

            if (config.ffmpeg.frameRate <= 0)
                config.ffmpeg.frameRate = 30;

            if (config.ffmpeg.outputWidth <= 0)
                config.ffmpeg.outputWidth = 1080;

            if (config.ffmpeg.outputHeight <= 0)
                config.ffmpeg.outputHeight = 1920;

            if (config.webcam.requestedWidth <= 0)
                config.webcam.requestedWidth = 1080;

            if (config.webcam.requestedHeight <= 0)
                config.webcam.requestedHeight = 1920;

            if (config.webcam.requestedFPS <= 0)
                config.webcam.requestedFPS = 30;

            if (config.waveform.sensitivity <= 0f)
                config.waveform.sensitivity = 25f;

            if (config.waveform.smoothSpeed <= 0f)
                config.waveform.smoothSpeed = 12f;
        }

        private static void SaveConfig(string configPath, NarraGolRuntimeConfig configData)
        {
            string json = JsonUtility.ToJson(configData, true);
            File.WriteAllText(configPath, json);
        }

        private static void EnsureConfigFolderExists()
        {
            string folderPath = GetConfigFolderPath();

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        private static string GetConfigPath()
        {
            return Path.Combine(
                GetConfigFolderPath(),
                CONFIG_FILE_NAME
            );
        }

        private static string GetConfigFolderPath()
        {
            return Path.Combine(
                NarraGolPaths.GetAppRootFolder(),
                CONFIG_FOLDER
            );
        }
    }
}