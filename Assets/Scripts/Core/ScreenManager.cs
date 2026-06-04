using dev.xrlab.NarraGol.UI;
using dev.xrlab.NarraGol.Video;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dev.xrlab.NarraGol.Core
{
    public class ScreenManager : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField] private GameObject idleScreen;
        [SerializeField] private GameObject tutorialScreen;
        [SerializeField] private GameObject selectionScreen;
        [SerializeField] private GameObject previewScreen;

        [Header("Scripts")]
        [SerializeField] private PreviewVideoController previewVideoController;

        [SerializeField] private PreviewTitleImageController previewTitleImageController;

        [SerializeField] private VideoThumbnailController videoThumbnailController;

        [Header("Scenes")]
        [SerializeField] private string recordingSceneName = "RecordingScene";

        public AppScreen CurrentScreen { get; private set; }

        private void OnEnable()
        {
            PreviewVideoController.OnPreviewFinished += LoadRecordingScene;
        }

        private void OnDisable()
        {
            PreviewVideoController.OnPreviewFinished -= LoadRecordingScene;
        }

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

            previewTitleImageController?.LoadTitleImage();
            videoThumbnailController?.LoadThumbnail();
            previewVideoController?.LoadSelectedVideo();
        }

        public void LoadRecordingScene()
        {
            previewVideoController?.StopVideo();

            Debug.Log($"Loading Scene: {recordingSceneName}");

            SceneManager.LoadScene(recordingSceneName);
        }

        private void SetScreen(AppScreen screen)
        {
            CurrentScreen = screen;

            idleScreen?.SetActive(screen == AppScreen.Idle);
            tutorialScreen?.SetActive(screen == AppScreen.Tutorial);
            selectionScreen?.SetActive(screen == AppScreen.Selection);
            previewScreen?.SetActive(screen == AppScreen.Preview);

            Debug.Log($"Current Screen: {CurrentScreen}");
        }
    }
}