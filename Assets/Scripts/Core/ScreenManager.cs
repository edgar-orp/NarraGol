using System;
using UnityEngine;
using dev.xrlab.NarraGol.Video;

namespace dev.xrlab.NarraGol.Core
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private GameObject idleScreen;
        [SerializeField] private GameObject tutorialScreen;
        [SerializeField] private GameObject selectionScreen;
        [SerializeField] private GameObject previewScreen;
        [SerializeField] private GameObject recordingScreen;
        [SerializeField] private GameObject resultScreen;

        [Header("Scripts")]
        [SerializeField] private PreviewVideoController previewVideoController;
        public AppScreen CurrentScreen { get; private set; }

        private void Start()
        {
            ShowIdle();
        }

        public void ShowIdle()
        {
            SetScreen(AppScreen.Idle);
        }

        public void ShowTutorial()
        {
            SetScreen(AppScreen.Tutorial);
        }

        public void ShowSelection()
        {
            SetScreen(AppScreen.Selection);
        }

        public void ShowPreview()
        {
            SetScreen(AppScreen.Preview);

            previewVideoController?.LoadSelectedVideo();
        }

        public void ShowRecording()
        {
            previewVideoController?.StopVideo();

            SetScreen(AppScreen.Recording);
        }

        public void ShowResult()
        {
            SetScreen(AppScreen.Result);
        }

        private void SetScreen(AppScreen screen)
        {
            CurrentScreen = screen;

            if (idleScreen != null)
                idleScreen.SetActive(screen == AppScreen.Idle);

            if (tutorialScreen != null)
                tutorialScreen.SetActive(screen == AppScreen.Tutorial);

            if (selectionScreen != null)
                selectionScreen.SetActive(screen == AppScreen.Selection);

            if (previewScreen != null)
                previewScreen.SetActive(screen == AppScreen.Preview);

            if (recordingScreen != null)
                recordingScreen.SetActive(screen == AppScreen.Recording);

            if (resultScreen != null)
                resultScreen.SetActive(screen == AppScreen.Result);

            Debug.Log($"Current Screen: {CurrentScreen}");
        }
    }
}