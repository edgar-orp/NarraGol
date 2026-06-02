using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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

        [Tooltip("Leave empty to auto-detect the first FFmpeg DirectShow audio device.")]
        [SerializeField] private string microphoneDeviceName;

        [Header("Output Resolution")]
        [SerializeField] private int outputWidth = 1080;
        [SerializeField] private int outputHeight = 1920;

        private Process ffmpegProcess;
        private string currentOutputPath;
        private readonly StringBuilder ffmpegLogBuilder = new StringBuilder();

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
            ffmpegLogBuilder.Clear();

            string videoInput = GetVideoInputArguments();
            string resolvedMicrophoneName = GetResolvedMicrophoneName();

            string audioInput = string.IsNullOrWhiteSpace(resolvedMicrophoneName)
                ? string.Empty
                : $"-f dshow -i audio=\"{resolvedMicrophoneName}\" ";

            string audioCodec = string.IsNullOrWhiteSpace(resolvedMicrophoneName)
                ? string.Empty
                : "-c:a aac -b:a 192k ";

            string mapArguments = string.IsNullOrWhiteSpace(resolvedMicrophoneName)
                ? "-map 0:v:0 "
                : "-map 0:v:0 -map 1:a:0 ";

            string arguments =
                $"-y " +
                $"-loglevel verbose " +
                $"{videoInput}" +
                $"{audioInput}" +
                $"-t {durationSeconds} " +
                $"{mapArguments}" +
                $"-vf \"scale={outputWidth}:{outputHeight}\" " +
                $"-c:v libx264 " +
                $"-preset veryfast " +
                $"-pix_fmt yuv420p " +
                $"{audioCodec}" +
                $"-movflags +faststart " +
                $"\"{outputPath}\"";

            StartFFmpeg(arguments);
        }

        private string GetVideoInputArguments()
        {
            return
                $"-f gdigrab " +
                $"-framerate {frameRate} " +
                $"-video_size {outputWidth}x{outputHeight} " +
                $"-offset_x 0 " +
                $"-offset_y 0 " +
                $"-i desktop ";
        }

        private string GetResolvedMicrophoneName()
        {
            if (!string.IsNullOrWhiteSpace(microphoneDeviceName))
            {
                UnityEngine.Debug.Log($"Using configured FFmpeg microphone: {microphoneDeviceName}");
                return microphoneDeviceName;
            }

            string detectedMicrophone = GetFirstDirectShowAudioDevice();

            if (!string.IsNullOrWhiteSpace(detectedMicrophone))
            {
                UnityEngine.Debug.Log($"Using auto-detected FFmpeg microphone: {detectedMicrophone}");
                return detectedMicrophone;
            }

            UnityEngine.Debug.LogWarning("No FFmpeg microphone detected. Capture will continue without audio.");
            return string.Empty;
        }

        private string GetFirstDirectShowAudioDevice()
        {
            string resolvedFFmpegPath = GetResolvedFFmpegPath();

            if (!File.Exists(resolvedFFmpegPath))
            {
                Fail($"FFmpeg executable not found while listing devices: {resolvedFFmpegPath}");
                return string.Empty;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = resolvedFFmpegPath,
                Arguments = "-hide_banner -list_devices true -f dshow -i dummy",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using Process process = new Process();
                process.StartInfo = startInfo;

                process.Start();

                string output = process.StandardError.ReadToEnd();
                process.WaitForExit();

                UnityEngine.Debug.Log("================================");
                UnityEngine.Debug.Log("FFMPEG DIRECTSHOW DEVICES");
                UnityEngine.Debug.Log(output);
                UnityEngine.Debug.Log("================================");

                foreach (string rawLine in output.Split('\n'))
                {
                    string line = rawLine.Trim();

                    if (!line.Contains("(audio)"))
                        continue;

                    if (line.Contains("Alternative name"))
                        continue;

                    int firstQuote = line.IndexOf('"');
                    int secondQuote = line.IndexOf('"', firstQuote + 1);

                    if (firstQuote >= 0 && secondQuote > firstQuote)
                    {
                        string deviceName = line.Substring(
                            firstQuote + 1,
                            secondQuote - firstQuote - 1
                        );

                        UnityEngine.Debug.Log($"First FFmpeg audio device found: {deviceName}");
                        return deviceName;
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Could not list FFmpeg devices: {e.Message}");
            }

            return string.Empty;
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
                AppendFFmpegLog("================================");
                AppendFFmpegLog("STARTING SCREEN CAPTURE");
                AppendFFmpegLog($"FFmpeg Path: {resolvedFFmpegPath}");
                AppendFFmpegLog("Capture Target: Single fullscreen display");
                AppendFFmpegLog($"Capture Size: {outputWidth}x{outputHeight}");
                AppendFFmpegLog($"Arguments: {arguments}");
                AppendFFmpegLog("================================");

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
                AppendFFmpegLog(e.Data);
        }

        private void HandleErrorData(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
                AppendFFmpegLog(e.Data);
        }

        private void HandleProcessExited(object sender, EventArgs e)
        {
            int exitCode = ffmpegProcess.ExitCode;
            string outputPath = currentOutputPath;

            CleanupProcess();
            WriteFFmpegLogToFile();

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
                Fail($"FFmpeg failed. Exit Code: {exitCode}. Check Output/ffmpeg_capture_log.txt");
            }
        }

        private void AppendFFmpegLog(string message)
        {
            ffmpegLogBuilder.AppendLine(message);
            UnityEngine.Debug.Log(message);
        }

        private void WriteFFmpegLogToFile()
        {
            try
            {
                string logPath = Path.Combine(
                    NarraGolPaths.GetOutputFolder(),
                    "ffmpeg_capture_log.txt"
                );

                File.WriteAllText(logPath, ffmpegLogBuilder.ToString());

                UnityEngine.Debug.Log($"FFmpeg log written to: {logPath}");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Could not write FFmpeg log file: {e.Message}");
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