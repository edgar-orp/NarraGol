using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections;
using dev.xrlab.NarraGol.Core;

namespace dev.xrlab.NarraGol.Video
{
    public class PreviewVideoController : MonoBehaviour
    {
        public static event Action OnPreviewFinished;

        [Header("Data")]
        [SerializeField] private NarraGolClipSessionData sessionData;

        [Header("Video")]
        [SerializeField] private VideoPlayer videoPlayer;

        [Header("Preview Settings")]
        [SerializeField] private float previewDurationSeconds = 20f;

        private bool isPrepared;
        private bool hasStarted;
        private Coroutine previewRoutine;

        public void LoadSelectedVideo()
        {
            if (sessionData == null)
            {
                Debug.LogError("Session Data not assigned.");
                return;
            }

            if (videoPlayer == null)
            {
                Debug.LogError("VideoPlayer not assigned.");
                return;
            }

            if (string.IsNullOrWhiteSpace(sessionData.SelectedVideoPath))
            {
                Debug.LogError("Selected video path is empty.");
                return;
            }

            StopVideo();

            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = sessionData.SelectedVideoPath;
            videoPlayer.isLooping = false;

            isPrepared = false;
            hasStarted = false;

            videoPlayer.prepareCompleted -= HandlePrepareCompleted;
            videoPlayer.prepareCompleted += HandlePrepareCompleted;

            videoPlayer.Prepare();

            Debug.Log($"Preparing preview video: {sessionData.SelectedVideoPath}");
        }

        private void HandlePrepareCompleted(VideoPlayer source)
        {
            isPrepared = true;
            Debug.Log("Preview video prepared.");
        }

        public void PlayPreview()
        {
            if (videoPlayer == null)
                return;

            if (hasStarted)
            {
                Debug.LogWarning("Preview already started.");
                return;
            }

            if (!isPrepared)
            {
                Debug.LogWarning("Preview video is not prepared yet.");
                return;
            }

            hasStarted = true;

            videoPlayer.Stop();
            videoPlayer.time = 0;
            videoPlayer.Play();

            previewRoutine = StartCoroutine(PreviewDurationRoutine());

            Debug.Log("Preview video started.");
        }

        private IEnumerator PreviewDurationRoutine()
        {
            yield return new WaitForSeconds(previewDurationSeconds);

            StopVideo();

            Debug.Log("Preview finished.");

            OnPreviewFinished?.Invoke();
        }

        public void StopVideo()
        {
            if (previewRoutine != null)
            {
                StopCoroutine(previewRoutine);
                previewRoutine = null;
            }

            if (videoPlayer == null)
                return;

            videoPlayer.Stop();

            isPrepared = false;
            hasStarted = false;

            Debug.Log("Preview video stopped.");
        }

        private void OnDestroy()
        {
            if (videoPlayer != null)
                videoPlayer.prepareCompleted -= HandlePrepareCompleted;
        }
    }
}