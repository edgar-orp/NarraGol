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
        [SerializeField] private string selectedVideoFileName;
        [SerializeField] private string selectedVideoPath;

        [Header("Generated Audio")]
        [SerializeField] private string generatedWavPath;

        [Header("Final Video")]
        [SerializeField] private string finalVideoFileName;

        public string SelectedVideoFileName => selectedVideoFileName;
        public string SelectedVideoPath => selectedVideoPath;
        public string GeneratedWavPath => generatedWavPath;
        public string FinalVideoFileName => finalVideoFileName;

        public void SetSelectedVideo(string videoFileName, string videoPath)
        {
            selectedVideoFileName = videoFileName;
            selectedVideoPath = videoPath;
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
            selectedVideoFileName = string.Empty;
            selectedVideoPath = string.Empty;
            generatedWavPath = string.Empty;
            finalVideoFileName = string.Empty;
        }
    }
}