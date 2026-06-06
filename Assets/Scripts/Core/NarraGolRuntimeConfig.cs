using System;

namespace dev.xrlab.NarraGol.Core
{
    [Serializable]
    public class NarraGolRuntimeConfig
    {
        public FFmpegConfig ffmpeg = new FFmpegConfig();
        public WebcamConfig webcam = new WebcamConfig();
        public WaveformConfig waveform = new WaveformConfig();
    }

    [Serializable]
    public class FFmpegConfig
    {
        public int frameRate = 30;
        public int outputWidth = 1080;
        public int outputHeight = 1920;
        public float audioOffsetSeconds = -0.20f;
        public string microphoneDeviceName = "";
    }

    [Serializable]
    public class WebcamConfig
    {
        public string preferredDeviceName = "";
        public int requestedWidth = 1080;
        public int requestedHeight = 1920;
        public int requestedFPS = 30;
    }

    [Serializable]
    public class WaveformConfig
    {
        public float sensitivity = 25f;
        public float smoothSpeed = 12f;
    }
}