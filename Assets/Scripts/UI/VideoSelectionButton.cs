using UnityEngine;
using System.IO;
using dev.xrlab.NarraGol.Core;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.UI
{
    public class VideoSelectionButton : MonoBehaviour
    {
        [Header("Video")]
        [SerializeField] private string videoFileName;

        [Header("Data")]
        [SerializeField] private NarraGolClipSessionData sessionData;

        public void SelectVideo()
        {
            if (string.IsNullOrWhiteSpace(videoFileName))
            {
                Debug.LogError($"Video file name is empty on button: {gameObject.name}");
                return;
            }

            if (sessionData == null)
            {
                Debug.LogError($"Session Data not assigned on button: {gameObject.name}");
                return;
            }

            string videoPath = NarraGolPaths.GetVideoPath(videoFileName);

            Debug.Log($"Trying to select video: {videoFileName}");
            Debug.Log($"Resolved path: {videoPath}");

            if (!File.Exists(videoPath))
            {
                Debug.LogError($"Video not found: {videoPath}");
                return;
            }

            sessionData.SetSelectedVideo(videoFileName, videoPath);

            Debug.Log("================================");
            Debug.Log("VIDEO SELECTED");
            Debug.Log($"File: {videoFileName}");
            Debug.Log($"Path: {videoPath}");
            Debug.Log("================================");
        }
    }
}