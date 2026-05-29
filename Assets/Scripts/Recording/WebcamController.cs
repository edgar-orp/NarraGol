using UnityEngine;
using UnityEngine.UI;

namespace dev.xrlab.NarraGol.Recording
{
    public class WebcamController : MonoBehaviour
    {
        [Header("UI Target")]
        [SerializeField] private RawImage webcamRawImage;

        [Header("Settings")]
        [SerializeField] private int requestedWidth = 1280;
        [SerializeField] private int requestedHeight = 720;
        [SerializeField] private int requestedFPS = 30;

        private WebCamTexture webcamTexture;

        public void StartWebcam()
        {
            if (webcamRawImage == null)
            {
                Debug.LogError("Webcam RawImage not assigned.");
                return;
            }

            if (WebCamTexture.devices.Length == 0)
            {
                Debug.LogError("No webcam detected.");
                return;
            }

            WebCamDevice selectedDevice = WebCamTexture.devices[0];

            Debug.Log($"Using webcam: {selectedDevice.name}");

            webcamTexture = new WebCamTexture(
                selectedDevice.name,
                requestedWidth,
                requestedHeight,
                requestedFPS
            );

            webcamRawImage.texture = webcamTexture;

            webcamTexture.Play();

            Debug.Log("Webcam started.");
        }

        public void StopWebcam()
        {
            if (webcamTexture == null)
                return;

            if (webcamTexture.isPlaying)
                webcamTexture.Stop();

            webcamRawImage.texture = null;

            Destroy(webcamTexture);
            webcamTexture = null;

            Debug.Log("Webcam stopped.");
        }

        private void OnDestroy()
        {
            StopWebcam();
        }
    }
}