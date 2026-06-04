using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using dev.xrlab.NarraGol.Core;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.UI
{
    public class PreviewTitleImageController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private NarraGolClipSessionData sessionData;

        [Header("UI")]
        [SerializeField] private Image targetImage;

        [Header("File Settings")]
        [SerializeField] private string imagePrefix = "videotitle_";
        [SerializeField] private string imageExtension = ".png";

        public void LoadTitleImage()
        {
            if (sessionData == null)
            {
                Debug.LogError("Session Data not assigned.");
                return;
            }

            if (targetImage == null)
            {
                Debug.LogError("Target Image not assigned.");
                return;
            }

            string index = sessionData.SelectedVideoIndex;

            if (string.IsNullOrWhiteSpace(index))
            {
                Debug.LogError("Selected video index is empty.");
                return;
            }

            string imageFileName = $"{imagePrefix}{index}{imageExtension}";
            string imagePath = NarraGolPaths.GetImagePath(imageFileName);

            if (!File.Exists(imagePath))
            {
                Debug.LogError($"Preview title image not found: {imagePath}");
                return;
            }

            StartCoroutine(LoadImageRoutine(imagePath));
        }

        private IEnumerator LoadImageRoutine(string imagePath)
        {
            byte[] imageData = File.ReadAllBytes(imagePath);

            Texture2D texture = new Texture2D(2, 2);

            if (!texture.LoadImage(imageData))
            {
                Debug.LogError($"Could not load image: {imagePath}");
                yield break;
            }

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            targetImage.sprite = sprite;

            Debug.Log($"Preview title image loaded: {imagePath}");

            yield return null;
        }
    }
}