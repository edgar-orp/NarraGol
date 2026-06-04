using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using dev.xrlab.NarraGol.Core;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.UI
{
    public class VideoThumbnailController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private NarraGolClipSessionData sessionData;

        [Header("UI")]
        [SerializeField] private RawImage targetRawImage;

        [Header("Video Render Texture")]
        [SerializeField] private RenderTexture videoRenderTexture;

        [Header("File Settings")]
        [SerializeField] private string thumbnailPrefix = "thumbnail_";
        [SerializeField] private string thumbnailExtension = ".png";

        private Texture2D loadedThumbnailTexture;

        public void LoadThumbnail()
        {
            if (sessionData == null)
            {
                Debug.LogError("Session Data not assigned.");
                return;
            }

            if (targetRawImage == null)
            {
                Debug.LogError("Target RawImage not assigned.");
                return;
            }

            string index = sessionData.SelectedVideoIndex;

            if (string.IsNullOrWhiteSpace(index))
            {
                Debug.LogError("Selected video index is empty.");
                return;
            }

            string fileName = $"{thumbnailPrefix}{index}{thumbnailExtension}";
            string path = NarraGolPaths.GetImagePath(fileName);

            if (!File.Exists(path))
            {
                Debug.LogError($"Thumbnail not found: {path}");
                return;
            }

            StartCoroutine(LoadThumbnailRoutine(path));
        }

        public void ShowVideoRenderTexture()
        {
            if (targetRawImage == null)
            {
                Debug.LogError("Target RawImage not assigned.");
                return;
            }

            if (videoRenderTexture == null)
            {
                Debug.LogError("Video RenderTexture not assigned.");
                return;
            }

            targetRawImage.texture = videoRenderTexture;

            Debug.Log("Video RenderTexture assigned to RawImage.");
        }

        public void Clear()
        {
            if (targetRawImage != null)
                targetRawImage.texture = null;
        }

        private IEnumerator LoadThumbnailRoutine(string path)
        {
            byte[] data = File.ReadAllBytes(path);

            Texture2D texture = new Texture2D(2, 2);

            if (!texture.LoadImage(data))
            {
                Debug.LogError($"Could not load thumbnail: {path}");
                yield break;
            }

            loadedThumbnailTexture = texture;
            targetRawImage.texture = loadedThumbnailTexture;

            Debug.Log($"Thumbnail loaded: {path}");

            yield return null;
        }

        private void OnDestroy()
        {
            if (loadedThumbnailTexture != null)
            {
                Destroy(loadedThumbnailTexture);
                loadedThumbnailTexture = null;
            }
        }
    }
}