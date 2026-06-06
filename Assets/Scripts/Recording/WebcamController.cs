using UnityEngine;
using UnityEngine.UI;
using dev.xrlab.NarraGol.Core;

namespace dev.xrlab.NarraGol.Recording
{
    public class WebcamController : MonoBehaviour
    {
        [Header("UI Target")]
        [SerializeField] private RawImage webcamRawImage;

        [Header("Settings")]
        [SerializeField] private string preferredDeviceName;
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

            ApplyConfig();

            string deviceName = GetSelectedWebcamDeviceName();

            if (string.IsNullOrWhiteSpace(deviceName))
            {
                Debug.LogError("No webcam selected.");
                return;
            }

            webcamTexture = new WebCamTexture(
                deviceName,
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

        private void ApplyConfig()
        {
            NarraGolRuntimeConfig config = NarraGolConfigLoader.Config;

            preferredDeviceName = config.webcam.preferredDeviceName;
            requestedWidth = config.webcam.requestedWidth;
            requestedHeight = config.webcam.requestedHeight;
            requestedFPS = config.webcam.requestedFPS;

            Debug.Log("================================");
            Debug.Log("WEBCAM CONFIG APPLIED");
            Debug.Log($"Preferred Device: {preferredDeviceName}");
            Debug.Log($"Requested Resolution: {requestedWidth}x{requestedHeight}");
            Debug.Log($"Requested FPS: {requestedFPS}");
            Debug.Log("================================");
        }

        private string GetSelectedWebcamDeviceName()
        {
            WebCamDevice[] devices = WebCamTexture.devices;

            if (devices == null || devices.Length == 0)
            {
                Debug.LogError("No webcam devices found.");
                return string.Empty;
            }

            Debug.Log("================================");
            Debug.Log("AVAILABLE WEBCAMS");

            foreach (WebCamDevice device in devices)
            {
                Debug.Log(device.name);
            }

            Debug.Log("================================");

            if (!string.IsNullOrWhiteSpace(preferredDeviceName))
            {
                foreach (WebCamDevice device in devices)
                {
                    if (device.name.Contains(preferredDeviceName))
                    {
                        Debug.Log($"Using preferred webcam: {device.name}");
                        return device.name;
                    }
                }

                Debug.LogWarning(
                    $"Preferred webcam not found: {preferredDeviceName}"
                );
            }

            Debug.Log($"Using default webcam: {devices[0].name}");

            return devices[0].name;
        }

        private void OnDestroy()
        {
            StopWebcam();
        }
    }
}