using UnityEngine;

namespace dev.xrlab.NarraGol.Recording
{
    public class MicrophoneMonitor : MonoBehaviour
    {
        [SerializeField] private int sampleRate = 44100;
        [SerializeField] private int sampleWindow = 128;

        public string CurrentMicrophoneName { get; private set; }
        public float CurrentLevel { get; private set; }
        public bool IsListening { get; private set; }

        private AudioClip microphoneClip;

        public void StartListening()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No microphone detected.");
                return;
            }

            CurrentMicrophoneName = Microphone.devices[0];

            microphoneClip = Microphone.Start(
                CurrentMicrophoneName,
                true,
                1,
                sampleRate
            );

            IsListening = true;

            Debug.Log($"Microphone monitoring started: {CurrentMicrophoneName}");
        }

        public void StopListening()
        {
            if (!IsListening)
                return;

            Microphone.End(CurrentMicrophoneName);

            IsListening = false;
            CurrentLevel = 0f;

            Debug.Log("Microphone monitoring stopped.");
        }

        private void Update()
        {
            if (!IsListening || microphoneClip == null)
                return;

            CurrentLevel = GetMicrophoneLevel();
        }

        private float GetMicrophoneLevel()
        {
            int micPosition = Microphone.GetPosition(CurrentMicrophoneName);

            if (micPosition < sampleWindow)
                return 0f;

            float[] samples = new float[sampleWindow];

            microphoneClip.GetData(
                samples,
                micPosition - sampleWindow
            );

            float level = 0f;

            for (int i = 0; i < sampleWindow; i++)
            {
                level += Mathf.Abs(samples[i]);
            }

            return level / sampleWindow;
        }

        private void OnDestroy()
        {
            StopListening();
        }
    }
}