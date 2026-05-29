using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.Recording
{
    public class FFmpegScreenCaptureRecorder : MonoBehaviour
    {
        public static event Action<string> OnCaptureFinished;
        public static event Action<string> OnCaptureFailed;

        [Header("FFmpeg")]
        [SerializeField] private string ffmpegPath = "ffmpeg.exe";

        [Header("Capture Settings")]
        [SerializeField] private int frameRate = 30;

        [Tooltip("Leave empty to use the default Windows microphone.")]
        [SerializeField] private string microphoneDeviceName;

#if !UNITY_EDITOR
        [Header("Build Window Capture")]
        [SerializeField] private string buildWindowTitle = "NarraGol";
#endif

        private Process ffmpegProcess;
        private string currentOutputPath;

        public bool IsCapturing => ffmpegProcess != null && !ffmpegProcess.HasExited;

        public void StartCapture(string outputPath, int durationSeconds)
        {
            if (IsCapturing)
            {
                UnityEngine.Debug.LogWarning("FFmpeg capture is already running.");
                return;
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                Fail("Output path is empty.");
                return;
            }

            if (durationSeconds <= 0)
            {
                Fail("Duration must be greater than 0.");
                return;
            }

            string outputDirectory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
                UnityEngine.Debug.Log($"Created output directory: {outputDirectory}");
            }

            currentOutputPath = outputPath;

            string videoInput = GetVideoInputArguments();
            string resolvedMicrophoneName = GetResolvedMicrophoneName();

            string audioInput =
                $"-f dshow -i audio=\"{resolvedMicrophoneName}\" ";

            string audioCodec =
                "-c:a aac -b:a 192k ";

            string arguments =
                $"-y " +
                $"{videoInput}" +
                $"{audioInput}" +
                $"-t {durationSeconds} " +
                $"-c:v libx264 " +
                $"-preset veryfast " +
                $"-pix_fmt yuv420p " +
                $"{audioCodec}" +
                $"\"{outputPath}\"";

            StartFFmpeg(arguments);
        }

        public void StopCapture()
        {
            if (!IsCapturing)
                return;

            try
            {
                ffmpegProcess.StandardInput.WriteLine("q");
                UnityEngine.Debug.Log("FFmpeg stop signal sent.");
            }
            catch
            {
                try
                {
                    ffmpegProcess.Kill();
                    UnityEngine.Debug.LogWarning("FFmpeg process killed.");
                }
                catch (Exception e)
                {
                    Fail($"Could not stop FFmpeg: {e.Message}");
                }
            }
        }

        private string GetVideoInputArguments()
        {
#if UNITY_EDITOR
            return
                $"-f gdigrab " +
                $"-framerate {frameRate} " +
                $"-i desktop ";
#else
            return
                $"-f gdigrab " +
                $"-framerate {frameRate} " +
                $"-i title=\"{buildWindowTitle}\" ";
#endif
        }

        private string GetResolvedMicrophoneName()
        {
            if (string.IsNullOrWhiteSpace(microphoneDeviceName))
            {
                UnityEngine.Debug.Log("Using default Windows microphone for FFmpeg capture.");
                return "default";
            }

            UnityEngine.Debug.Log($"Using configured FFmpeg microphone: {microphoneDeviceName}");
            return microphoneDeviceName;
        }

        private void StartFFmpeg(string arguments)
        {
            string resolvedFFmpegPath = GetResolvedFFmpegPath();

            if (!File.Exists(resolvedFFmpegPath))
            {
                Fail($"FFmpeg executable not found: {resolvedFFmpegPath}");
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = resolvedFFmpegPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            ffmpegProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            ffmpegProcess.OutputDataReceived += HandleOutputData;
            ffmpegProcess.ErrorDataReceived += HandleErrorData;
            ffmpegProcess.Exited += HandleProcessExited;

            try
            {
                UnityEngine.Debug.Log("================================");
                UnityEngine.Debug.Log("STARTING SCREEN CAPTURE");
                UnityEngine.Debug.Log($"FFmpeg Path: {resolvedFFmpegPath}");
#if UNITY_EDITOR
                UnityEngine.Debug.Log("Capture Target: Desktop / Unity Editor");
#else
                UnityEngine.Debug.Log($"Capture Target: Window Title / {buildWindowTitle}");
#endif
                UnityEngine.Debug.Log($"Arguments: {arguments}");
                UnityEngine.Debug.Log("================================");

                ffmpegProcess.Start();
                ffmpegProcess.BeginOutputReadLine();
                ffmpegProcess.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                CleanupProcess();
                Fail($"Failed to start FFmpeg: {e.Message}");
            }
        }

        private string GetResolvedFFmpegPath()
        {
            if (string.IsNullOrWhiteSpace(ffmpegPath))
                return NarraGolPaths.GetFFmpegPath();

            if (Path.IsPathRooted(ffmpegPath))
                return ffmpegPath;

            return Path.Combine(
                NarraGolPaths.GetAppRootFolder(),
                "FFmpeg",
                ffmpegPath
            );
        }

        private void HandleOutputData(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                UnityEngine.Debug.Log(e.Data);
        }

        private void HandleErrorData(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                UnityEngine.Debug.Log(e.Data);
        }

        private void HandleProcessExited(object sender, EventArgs e)
        {
            int exitCode = ffmpegProcess.ExitCode;
            string outputPath = currentOutputPath;

            CleanupProcess();

            if (exitCode == 0 && File.Exists(outputPath))
            {
                UnityEngine.Debug.Log("================================");
                UnityEngine.Debug.Log("SCREEN CAPTURE FINISHED");
                UnityEngine.Debug.Log($"Output: {outputPath}");
                UnityEngine.Debug.Log("================================");

                OnCaptureFinished?.Invoke(outputPath);
            }
            else
            {
                Fail($"FFmpeg failed. Exit Code: {exitCode}. Check FFmpeg log above.");
            }
        }

        private void CleanupProcess()
        {
            if (ffmpegProcess == null)
                return;

            ffmpegProcess.OutputDataReceived -= HandleOutputData;
            ffmpegProcess.ErrorDataReceived -= HandleErrorData;
            ffmpegProcess.Exited -= HandleProcessExited;

            ffmpegProcess.Dispose();
            ffmpegProcess = null;
        }

        private void Fail(string message)
        {
            UnityEngine.Debug.LogError(message);
            OnCaptureFailed?.Invoke(message);
        }

        private void OnDestroy()
        {
            StopCapture();
            CleanupProcess();
        }
    }
}