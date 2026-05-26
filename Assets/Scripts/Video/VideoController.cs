using UnityEngine;
using UnityEngine.Video;

namespace dev.xrlab.NarraGol.Recording
{
    public class VideoController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;

        private void OnEnable()
        {
            MicRecorder.OnRecordingStarted += PlayVideo;
            MicRecorder.OnRecordingStopped += StopVideo;
        }

        private void OnDisable()
        {
            MicRecorder.OnRecordingStarted -= PlayVideo;
            MicRecorder.OnRecordingStopped -= StopVideo;
        }

        private void PlayVideo()
        {
            if (videoPlayer == null)
            {
                Debug.LogError("VideoPlayer not assigned.");
                return;
            }

            videoPlayer.Stop();
            videoPlayer.time = 0;
            videoPlayer.Play();

            Debug.Log("Video started by recording event.");
        }

        private void StopVideo()
        {
            if (videoPlayer == null)
                return;

            videoPlayer.Stop();

            Debug.Log("Video stopped by recording event.");
        }
    }
}