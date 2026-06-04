using UnityEngine;
using System.IO;
using dev.xrlab.NarraGol.Core;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.UI
{
    public class VideoSelectionButton : MonoBehaviour
    {
        [Header("Video Index")]
        [SerializeField] private string videoIndex = "01";

        [Header("File Settings")]
        [SerializeField] private string previewPrefix = "preview_";
        [SerializeField] private string recordingPrefix = "recording_";
        [SerializeField] private string videoExtension = ".webm";

        [Header("Data")]
        [SerializeField] private NarraGolClipSessionData sessionData;

        public void SelectVideo()
        {
            if (string.IsNullOrWhiteSpace(videoIndex))
            {
                Debug.LogError($"Video index is empty on button: {gameObject.name}");
                return;
            }

            if (sessionData == null)
            {
                Debug.LogError($"Session Data not assigned on button: {gameObject.name}");
                return;
            }

            string previewFileName = $"{previewPrefix}{videoIndex}{videoExtension}";
            string recordingFileName = $"{recordingPrefix}{videoIndex}{videoExtension}";

            string previewVideoPath = NarraGolPaths.GetVideoPath(previewFileName);
            string recordingVideoPath = NarraGolPaths.GetVideoPath(recordingFileName);

            Debug.Log($"Trying to select preview video: {previewFileName}");
            Debug.Log($"Preview path: {previewVideoPath}");
            Debug.Log($"Trying to select recording video: {recordingFileName}");
            Debug.Log($"Recording path: {recordingVideoPath}");

            if (!File.Exists(previewVideoPath))
            {
                Debug.LogError($"Preview video not found: {previewVideoPath}");
                return;
            }

            if (!File.Exists(recordingVideoPath))
            {
                Debug.LogError($"Recording video not found: {recordingVideoPath}");
                return;
            }

            sessionData.SetSelectedVideos(
                videoIndex,
                previewVideoPath,
                recordingVideoPath
            );

            Debug.Log("================================");
            Debug.Log("VIDEOS SELECTED");
            Debug.Log($"Index: {videoIndex}");
            Debug.Log($"Preview: {previewVideoPath}");
            Debug.Log($"Recording: {recordingVideoPath}");
            Debug.Log("================================");
        }
    }
}