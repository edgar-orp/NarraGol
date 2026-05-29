using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.IO;
using dev.xrlab.NarraGol.Core;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.Recording
{
    public class RecordingSceneController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private NarraGolClipSessionData sessionData;

        [Header("Video")]
        [SerializeField] private VideoPlayer selectedVideoPlayer;

        [Header("Controllers")]
        [SerializeField] private WebcamController webcamController;
        [SerializeField] private MicrophoneMonitor microphoneMonitor;
        [SerializeField] private FFmpegScreenCaptureRecorder screenCaptureRecorder;

        [Header("Recording Settings")]
        [SerializeField] private int countdownSeconds = 3;
        [SerializeField] private int recordingDurationSeconds = 10;

        private bool isSequenceRunning;
        private bool isScreenRecording;
        private string currentOutputPath;

        private void Start()
        {
            InitializeRecordingScene();
        }

        private void InitializeRecordingScene()
        {
            webcamController?.StartWebcam();

            microphoneMonitor?.StartListening();

            LoadSelectedVideo();

            SetSelectedVideoVisible(false);

            Debug.Log("Recording scene initialized.");

            StartPreRecordingCountdown();
        }

        private void LoadSelectedVideo()
        {
            if (sessionData == null)
            {
                Debug.LogError("Session Data not assigned.");
                return;
            }

            if (selectedVideoPlayer == null)
            {
                Debug.LogError("Selected VideoPlayer not assigned.");
                return;
            }

            if (string.IsNullOrWhiteSpace(sessionData.SelectedVideoPath))
            {
                Debug.LogError("Selected video path is empty.");
                return;
            }

            selectedVideoPlayer.gameObject.SetActive(true);

            selectedVideoPlayer.Stop();
            selectedVideoPlayer.source = VideoSource.Url;
            selectedVideoPlayer.url = sessionData.SelectedVideoPath;
            selectedVideoPlayer.isLooping = false;
            selectedVideoPlayer.Prepare();

            Debug.Log($"Recording video loaded: {sessionData.SelectedVideoPath}");
        }

        public void StartPreRecordingCountdown()
        {
            if (isSequenceRunning)
            {
                Debug.LogWarning("Recording sequence already running.");
                return;
            }

            StartCoroutine(PreRecordingCountdownRoutine());
        }

        private IEnumerator PreRecordingCountdownRoutine()
        {
            isSequenceRunning = true;

            SetSelectedVideoVisible(false);

            Debug.Log("================================");
            Debug.Log("PRE-RECORDING COUNTDOWN STARTED");
            Debug.Log($"Countdown: {countdownSeconds} seconds");
            Debug.Log("================================");

            yield return new WaitForSeconds(countdownSeconds);

            ActivateSelectedVideoAndPlay();

            StartScreenRecording();
        }

        private void ActivateSelectedVideoAndPlay()
        {
            if (selectedVideoPlayer == null)
            {
                Debug.LogError("Cannot play selected video. VideoPlayer not assigned.");
                return;
            }

            SetSelectedVideoVisible(true);

            selectedVideoPlayer.Stop();
            selectedVideoPlayer.time = 0;
            selectedVideoPlayer.Play();

            Debug.Log("Selected video activated and playing.");
        }

        private void StartScreenRecording()
        {
            if (isScreenRecording)
            {
                Debug.LogWarning("Screen recording already running.");
                return;
            }

            currentOutputPath = GenerateFinalVideoPath();

            screenCaptureRecorder?.StartCapture(
                currentOutputPath,
                recordingDurationSeconds
            );

            isScreenRecording = true;

            Debug.Log("================================");
            Debug.Log("SCREEN RECORDING STARTED");
            Debug.Log($"Duration: {recordingDurationSeconds} seconds");
            Debug.Log($"Output Path: {currentOutputPath}");
            Debug.Log("================================");

            StartCoroutine(ScreenRecordingDurationRoutine());
        }

        private IEnumerator ScreenRecordingDurationRoutine()
        {
            yield return new WaitForSeconds(recordingDurationSeconds);

            FinishRecordingSequence();
        }

        private void FinishRecordingSequence()
        {
            selectedVideoPlayer?.Stop();

            isScreenRecording = false;
            isSequenceRunning = false;

            Debug.Log("================================");
            Debug.Log("RECORDING SEQUENCE FINISHED");
            Debug.Log($"Output Path: {currentOutputPath}");
            Debug.Log("================================");
        }

        private void SetSelectedVideoVisible(bool visible)
        {
            if (selectedVideoPlayer == null)
                return;

            selectedVideoPlayer.gameObject.SetActive(visible);
        }

        private string GenerateFinalVideoPath()
        {
            string outputFolder = NarraGolPaths.GetOutputFolder();

            sessionData.GenerateFinalVideoFileName();

            return Path.Combine(
                outputFolder,
                sessionData.FinalVideoFileName
            );
        }
    }
}