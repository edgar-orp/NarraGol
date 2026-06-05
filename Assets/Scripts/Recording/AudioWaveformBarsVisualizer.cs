using UnityEngine;
using dev.xrlab.NarraGol.Recording;

namespace dev.xrlab.NarraGol.UI
{
    public class AudioWaveformBarsVisualizer : MonoBehaviour
    {
        [Header("Source")]
        [SerializeField] private MicrophoneMonitor microphoneMonitor;

        [Header("Bars")]
        [SerializeField] private RectTransform[] bars;

        [Header("Settings")]
        [SerializeField] private float sensitivity = 25f;
        [SerializeField] private float smoothSpeed = 12f;
        [SerializeField] private float minScaleY = 0.15f;
        [SerializeField] private float maxScaleY = 1.6f;
        [SerializeField] private float idleMovement = 0.08f;

        [Header("Voice Motion")]
        [SerializeField] private float waveSpeed = 8f;
        [SerializeField] private float randomAmount = 0.35f;

        private float[] currentScales;
        private float[] targetScales;
        private float[] offsets;

        private void Awake()
        {
            if (bars == null || bars.Length == 0)
                return;

            currentScales = new float[bars.Length];
            targetScales = new float[bars.Length];
            offsets = new float[bars.Length];

            for (int i = 0; i < bars.Length; i++)
            {
                currentScales[i] = minScaleY;
                targetScales[i] = minScaleY;
                offsets[i] = Random.Range(0f, 100f);

                if (bars[i] != null)
                    bars[i].localScale = new Vector3(1f, minScaleY, 1f);
            }
        }

        private void Update()
        {
            if (microphoneMonitor == null)
                return;

            if (bars == null || bars.Length == 0)
                return;

            float rawLevel = microphoneMonitor.CurrentLevel;
            float level = Mathf.Clamp01(rawLevel * sensitivity);

            for (int i = 0; i < bars.Length; i++)
            {
                if (bars[i] == null)
                    continue;

                float centerDistance = Mathf.Abs(i - (bars.Length - 1) * 0.5f);
                float centerFactor = 1f - (centerDistance / ((bars.Length - 1) * 0.5f));
                centerFactor = Mathf.Lerp(0.55f, 1f, centerFactor);

                float noise = Mathf.PerlinNoise(
                    Time.time * waveSpeed,
                    offsets[i]
                );

                float motion = Mathf.Lerp(
                    idleMovement,
                    noise,
                    level
                );

                float target = Mathf.Lerp(
                    minScaleY,
                    maxScaleY,
                    level * centerFactor
                );

                target += motion * randomAmount;

                target = Mathf.Clamp(
                    target,
                    minScaleY,
                    maxScaleY
                );

                currentScales[i] = Mathf.Lerp(
                    currentScales[i],
                    target,
                    Time.deltaTime * smoothSpeed
                );

                bars[i].localScale = new Vector3(
                    1f,
                    currentScales[i],
                    1f
                );
            }
        }

        public void ResetWaveform()
        {
            if (bars == null)
                return;

            for (int i = 0; i < bars.Length; i++)
            {
                currentScales[i] = minScaleY;
                targetScales[i] = minScaleY;

                if (bars[i] != null)
                    bars[i].localScale = new Vector3(1f, minScaleY, 1f);
            }
        }
    }
}