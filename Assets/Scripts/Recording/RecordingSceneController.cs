using dev.xrlab.NarraGol.Core;
using dev.xrlab.NarraGol.UI;
using dev.xrlab.NarraGol.Utility;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace dev.xrlab.NarraGol.Recording
{
    public class RecordingSceneController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private NarraGolClipSessionData sessionData;

        [Header("Video")]
        [SerializeField] private VideoPlayer selectedVideoPlayer;

        [Header("Screens")]
        [SerializeField] private GameObject farewellScreen;

        [Header("Controllers")]
        [SerializeField] private WebcamController webcamController;
        [SerializeField] private MicrophoneMonitor microphoneMonitor;
        [SerializeField] private FFmpegScreenCaptureRecorder screenCaptureRecorder;
        [SerializeField] private VideoThumbnailController videoThumbnailController;

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
            farewellScreen?.SetActive(false);

            webcamController?.StartWebcam();
            microphoneMonitor?.StartListening();

            LoadSelectedVideo();

            videoThumbnailController?.LoadThumbnail();

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

            if (string.IsNullOrWhiteSpace(sessionData.SelectedRecordingVideoPath))
            {
                Debug.LogError("Selected recording video path is empty.");
                return;
            }

            selectedVideoPlayer.Stop();
            selectedVideoPlayer.source = VideoSource.Url;
            selectedVideoPlayer.url = sessionData.SelectedRecordingVideoPath;
            selectedVideoPlayer.isLooping = false;
            selectedVideoPlayer.Prepare();

            Debug.Log($"Recording video loaded: {sessionData.SelectedRecordingVideoPath}");
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

            videoThumbnailController?.ShowVideoRenderTexture();

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
            SetSelectedVideoVisible(false);

            webcamController?.StopWebcam();
            microphoneMonitor?.StopListening();

            isScreenRecording = false;
            isSequenceRunning = false;

            farewellScreen?.SetActive(true);

            Debug.Log("================================");
            Debug.Log("RECORDING SEQUENCE FINISHED");
            Debug.Log($"Output Path: {currentOutputPath}");
            Debug.Log("FAREWELL SCREEN ACTIVATED");
            Debug.Log("================================");
        }

        public void ReloadApplication()
        {
            SceneManager.LoadScene(0);
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