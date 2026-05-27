using UnityEngine;
using UnityEngine.Video;
using dev.xrlab.NarraGol.Core;

namespace dev.xrlab.NarraGol.Video
{
    public class PreviewVideoController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private NarraGolClipSessionData sessionData;

        [Header("Video")]
        [SerializeField] private VideoPlayer videoPlayer;

        private bool isPrepared;

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

            videoPlayer.Stop();

            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = sessionData.SelectedVideoPath;
            videoPlayer.isLooping = true;

            isPrepared = false;

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

        public void TogglePlayPause()
        {
            if (videoPlayer == null)
                return;

            if (!isPrepared)
            {
                Debug.LogWarning("Preview video is not prepared yet.");
                return;
            }

            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                Debug.Log("Preview video paused.");
            }
            else
            {
                videoPlayer.Play();
                Debug.Log("Preview video playing.");
            }
        }

        public void StopVideo()
        {
            if (videoPlayer == null)
                return;

            videoPlayer.Stop();
            isPrepared = false;

            Debug.Log("Preview video stopped.");
        }

        private void OnDestroy()
        {
            if (videoPlayer != null)
                videoPlayer.prepareCompleted -= HandlePrepareCompleted;
        }
    }
}