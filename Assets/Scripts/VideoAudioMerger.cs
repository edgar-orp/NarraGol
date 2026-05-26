using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using dev.xrlab.NarraGol.Core;

namespace dev.xrlab.NarraGol.Export
{
    public class VideoAudioMerger : MonoBehaviour
    {
        public static event Action<string> OnVideoCreated;
        public static event Action<string> OnVideoMergeFailed;

        [Header("FFmpeg")]
        [SerializeField] private string ffmpegPath = "ffmpeg";

        private void OnEnable()
        {
            NarraGolSessionController.OnSessionFilesReady += HandleSessionFilesReady;
        }

        private void OnDisable()
        {
            NarraGolSessionController.OnSessionFilesReady -= HandleSessionFilesReady;
        }

        private void HandleSessionFilesReady(
            string videoPath,
            string audioPath,
            string outputPath
        )
        {
            MergeVideoAndAudio(videoPath, audioPath, outputPath);
        }

        public void MergeVideoAndAudio(
            string videoPath,
            string audioPath,
            string outputPath
        )
        {
            if (!ValidateInput(videoPath, audioPath, outputPath))
                return;

            string outputDirectory = Path.GetDirectoryName(outputPath);

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                UnityEngine.Debug.Log($"Created output directory: {outputDirectory}");
            }

            string arguments =
                $"-y " +
                $"-i \"{videoPath}\" " +
                $"-i \"{audioPath}\" " +
                $"-map 0:v:0 " +
                $"-map 1:a:0 " +
                $"-c:v copy " +
                $"-c:a aac " +
                $"-b:a 192k " +
                $"-shortest " +
                $"\"{outputPath}\"";

            RunFFmpeg(arguments, outputPath);
        }

        private bool ValidateInput(
            string videoPath,
            string audioPath,
            string outputPath
        )
        {
            if (string.IsNullOrWhiteSpace(videoPath))
            {
                Fail("Video path is empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(audioPath))
            {
                Fail("Audio path is empty.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Fail("Output path is empty.");
                return false;
            }

            if (!File.Exists(videoPath))
            {
                Fail($"Video file not found: {videoPath}");
                return false;
            }

            if (!File.Exists(audioPath))
            {
                Fail($"Audio file not found: {audioPath}");
                return false;
            }

            return true;
        }

        private void RunFFmpeg(string arguments, string outputPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                UnityEngine.Debug.Log("================================");
                UnityEngine.Debug.Log("STARTING VIDEO + AUDIO MERGE");
                UnityEngine.Debug.Log($"FFmpeg Path: {ffmpegPath}");
                UnityEngine.Debug.Log($"Arguments: {arguments}");
                UnityEngine.Debug.Log("================================");

                using Process process = new Process();
                process.StartInfo = startInfo;

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        UnityEngine.Debug.Log(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        UnityEngine.Debug.Log(e.Data);
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                if (process.ExitCode == 0 && File.Exists(outputPath))
                {
                    FileInfo fileInfo = new FileInfo(outputPath);

                    UnityEngine.Debug.Log("================================");
                    UnityEngine.Debug.Log("FINAL VIDEO CREATED SUCCESSFULLY");
                    UnityEngine.Debug.Log($"Path: {outputPath}");
                    UnityEngine.Debug.Log($"Size: {fileInfo.Length / 1024f / 1024f:F2} MB");
                    UnityEngine.Debug.Log("================================");

                    OnVideoCreated?.Invoke(outputPath);
                }
                else
                {
                    Fail($"FFmpeg failed. Exit Code: {process.ExitCode}");
                }
            }
            catch (Exception e)
            {
                Fail($"FFmpeg execution error: {e.Message}");
            }
        }

        private void Fail(string message)
        {
            UnityEngine.Debug.LogError(message);
            OnVideoMergeFailed?.Invoke(message);
        }
    }
}