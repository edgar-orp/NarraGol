using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Collections;
using dev.xrlab.NarraGol.Utility;

namespace dev.xrlab.NarraGol.Recording
{
    public class MicRecorder : MonoBehaviour
    {
        public static event Action OnRecordingStarted;
        public static event Action OnRecordingStopped;
        public static event Action<string> OnAudioFileSaved;

        public string LastAudioPath { get; private set; }

        [Header("Recording Settings")]
        [SerializeField] private int sampleRate = 44100;
        [SerializeField] private int recordDurationSeconds = 10;

        [Header("UI")]
        [SerializeField] private TMP_Text statusText;

        private const string OUTPUT_FOLDER = "NarraGol";

        private string microphoneName;
        private AudioClip recordedClip;
        private bool isRecording;
        private Coroutine recordingCoroutine;

        public void StartRecording()
        {
            if (isRecording)
            {
                Debug.LogWarning("Already recording.");
                return;
            }

            if (Microphone.devices.Length == 0)
            {
                SetStatus("No microphone detected.");
                Debug.LogError("No microphone detected.");
                return;
            }

            microphoneName = Microphone.devices[0];

            Debug.Log($"Using microphone: {microphoneName}");

            recordedClip = Microphone.Start(
                microphoneName,
                false,
                recordDurationSeconds,
                sampleRate
            );

            isRecording = true;

            SetStatus("Recording...");

            Debug.Log($"Recording started. Duration: {recordDurationSeconds} seconds");

            OnRecordingStarted?.Invoke();

            recordingCoroutine = StartCoroutine(AutoStopRecording());
        }

        private IEnumerator AutoStopRecording()
        {
            float timer = recordDurationSeconds;

            while (timer > 0f)
            {
                SetStatus($"Recording... {Mathf.CeilToInt(timer)}s");
                timer -= Time.deltaTime;
                yield return null;
            }

            StopRecording();
        }

        private void StopRecording()
        {
            if (!isRecording)
                return;

            int position = Microphone.GetPosition(microphoneName);

            Microphone.End(microphoneName);

            isRecording = false;

            OnRecordingStopped?.Invoke();

            if (recordingCoroutine != null)
            {
                StopCoroutine(recordingCoroutine);
                recordingCoroutine = null;
            }

            if (position <= 0)
            {
                SetStatus("Recording failed.");
                Debug.LogError("Recording failed. No audio captured.");
                return;
            }

            AudioClip trimmedClip = TrimClip(recordedClip, position);

            string fileName = $"narracion_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
            string savePath = Path.Combine(GetOutputFolder(), fileName);

            byte[] wavData = WavUtility.FromAudioClip(trimmedClip);

            File.WriteAllBytes(savePath, wavData);

            LastAudioPath = savePath;

            OnAudioFileSaved?.Invoke(savePath);

            Debug.Log("================================");
            Debug.Log("AUDIO SAVED SUCCESSFULLY");
            Debug.Log($"File: {fileName}");
            Debug.Log($"Path: {savePath}");
            Debug.Log($"Size: {wavData.Length / 1024f:F2} KB");
            Debug.Log("================================");

            SetStatus("Audio Saved");
        }

        private string GetOutputFolder()
        {
            string downloadsPath = WindowsFolders.GetDownloadsFolder();

            string outputPath = Path.Combine(
                downloadsPath,
                OUTPUT_FOLDER
            );

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                Debug.Log($"Created folder: {outputPath}");
            }

            Debug.Log($"Output folder: {outputPath}");

            return outputPath;
        }

        private AudioClip TrimClip(AudioClip clip, int samples)
        {
            float[] data = new float[samples * clip.channels];

            clip.GetData(data, 0);

            AudioClip trimmedClip = AudioClip.Create(
                clip.name + "_trimmed",
                samples,
                clip.channels,
                clip.frequency,
                false
            );

            trimmedClip.SetData(data, 0);

            return trimmedClip;
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }
    }
}