using UnityEngine;
using System;
using System.IO;
using dev.xrlab.NarraGol.Recording;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.Core
{
    public class NarraGolSessionController : MonoBehaviour
    {
        [Header("Input Video")]
        [SerializeField] private string currentVideoFileName = "jugada_01.mp4";

        [Header("Output")]
        [SerializeField] private string outputFolderName = "NarraGol";
        [SerializeField] private string outputFilePrefix = "narra_gol_final";

        public string CurrentVideoFileName => currentVideoFileName;
        public string CurrentVideoPath { get; private set; }
        public string LastAudioPath { get; private set; }
        public string LastOutputVideoPath { get; private set; }

        public static event Action<string> OnSessionAudioReady;
        public static event Action<string> OnSessionOutputPathReady;
        public static event Action<string, string, string> OnSessionFilesReady;

        private void Awake()
        {
            ResolveCurrentVideoPath();
        }

        private void OnEnable()
        {
            MicRecorder.OnAudioFileSaved += HandleAudioFileSaved;
        }

        private void OnDisable()
        {
            MicRecorder.OnAudioFileSaved -= HandleAudioFileSaved;
        }

        public void SetCurrentVideoFileName(string videoFileName)
        {
            if (string.IsNullOrWhiteSpace(videoFileName))
            {
                Debug.LogError("Video file name is empty.");
                return;
            }

            currentVideoFileName = videoFileName;
            ResolveCurrentVideoPath();
        }

        public void ResolveCurrentVideoPath()
        {
            string videoPath = NarraGolPaths.GetVideoPath(currentVideoFileName);

            if (!File.Exists(videoPath))
            {
                Debug.LogError($"Video file does not exist: {videoPath}");
                CurrentVideoPath = string.Empty;
                return;
            }

            CurrentVideoPath = videoPath;

            Debug.Log($"Current video resolved: {CurrentVideoPath}");
        }

        private void HandleAudioFileSaved(string audioPath)
        {
            LastAudioPath = audioPath;

            if (string.IsNullOrWhiteSpace(CurrentVideoPath) || !File.Exists(CurrentVideoPath))
            {
                Debug.LogError("Cannot prepare session files. Current video path is invalid.");
                return;
            }

            LastOutputVideoPath = GenerateOutputVideoPath();

            OnSessionAudioReady?.Invoke(LastAudioPath);
            OnSessionOutputPathReady?.Invoke(LastOutputVideoPath);
            OnSessionFilesReady?.Invoke(CurrentVideoPath, LastAudioPath, LastOutputVideoPath);

            Debug.Log("================================");
            Debug.Log("SESSION FILES READY");
            Debug.Log($"Video Input: {CurrentVideoPath}");
            Debug.Log($"Audio Input: {LastAudioPath}");
            Debug.Log($"Output Video: {LastOutputVideoPath}");
            Debug.Log("================================");
        }

        private string GenerateOutputVideoPath()
        {
            string outputFolder = Path.Combine(
                WindowsFolders.GetDownloadsFolder(),
                outputFolderName,
                "FinalVideos"
            );

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
                Debug.Log($"Created final video folder: {outputFolder}");
            }

            string fileName = $"{outputFilePrefix}_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";

            return Path.Combine(outputFolder, fileName);
        }
    }
}