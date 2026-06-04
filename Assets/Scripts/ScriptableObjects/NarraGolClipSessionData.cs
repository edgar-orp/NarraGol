using UnityEngine;
using System;

namespace dev.xrlab.NarraGol.Core
{
    [CreateAssetMenu(
        fileName = "NarraGolClipSessionData",
        menuName = "Narra Gol/Clip Session Data"
    )]
    public class NarraGolClipSessionData : ScriptableObject
    {
        [Header("Selected Video")]
        [SerializeField] private string selectedVideoIndex;
        [SerializeField] private string selectedPreviewVideoPath;
        [SerializeField] private string selectedRecordingVideoPath;

        [Header("Generated Audio")]
        [SerializeField] private string generatedWavPath;

        [Header("Final Video")]
        [SerializeField] private string finalVideoFileName;

        public string SelectedVideoIndex => selectedVideoIndex;
        public string SelectedPreviewVideoPath => selectedPreviewVideoPath;
        public string SelectedRecordingVideoPath => selectedRecordingVideoPath;
        public string GeneratedWavPath => generatedWavPath;
        public string FinalVideoFileName => finalVideoFileName;

        public void SetSelectedVideos(
            string videoIndex,
            string previewVideoPath,
            string recordingVideoPath
        )
        {
            selectedVideoIndex = videoIndex;
            selectedPreviewVideoPath = previewVideoPath;
            selectedRecordingVideoPath = recordingVideoPath;
        }

        public void SetGeneratedWavPath(string wavPath)
        {
            generatedWavPath = wavPath;
        }

        public void GenerateFinalVideoFileName()
        {
            finalVideoFileName = $"narra_gol_final_{DateTime.Now:yyyyMMdd_HHmmss}.mp4";
        }

        public void Clear()
        {
            selectedVideoIndex = string.Empty;
            selectedPreviewVideoPath = string.Empty;
            selectedRecordingVideoPath = string.Empty;
            generatedWavPath = string.Empty;
            finalVideoFileName = string.Empty;
        }
    }
}