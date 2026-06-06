# NarraGol Configuration Guide

This document explains how to configure NarraGol without opening Unity or rebuilding the application.

---

## Configuration File Location

The configuration file is located at:

```text
Config/config.json
```

Open this file using any text editor such as Notepad.

---

## Important Notes

* Always close NarraGol before editing the configuration file.
* After saving changes, launch NarraGol normally.
* If a value is invalid, NarraGol will attempt to use safe defaults.
* Do not remove commas, quotes, braces, or property names.

---

## Example Configuration

```json
{
    "ffmpeg": {
        "frameRate": 30,
        "outputWidth": 1080,
        "outputHeight": 1920,
        "audioOffsetSeconds": -0.2,
        "microphoneDeviceName": ""
    },
    "webcam": {
        "preferredDeviceName": "",
        "requestedWidth": 1080,
        "requestedHeight": 1920,
        "requestedFPS": 30
    },
    "waveform": {
        "sensitivity": 25,
        "smoothSpeed": 12
    }
}
```

---

# Audio Settings

## audioOffsetSeconds

Adjusts audio synchronization.

Examples:

```json
"audioOffsetSeconds": -0.10
```

Advance audio by approximately 100 ms.

```json
"audioOffsetSeconds": -0.20
```

Advance audio by approximately 200 ms.

```json
"audioOffsetSeconds": 0.00
```

No audio adjustment.

Recommended adjustment range:

```text
-0.10 to -0.30
```

---

## microphoneDeviceName

Optional microphone override for FFmpeg recording.

Examples:

```json
"microphoneDeviceName": ""
```

Use automatic microphone detection.

```json
"microphoneDeviceName": "Microphone (Realtek(R) Audio)"
```

Force a specific microphone.

Leave empty unless troubleshooting microphone selection.

---

# Webcam Settings

## preferredDeviceName

Optional webcam override.

Examples:

```json
"preferredDeviceName": ""
```

Use the first webcam detected by the system.

```json
"preferredDeviceName": "8Mega AutoFocus Webcam"
```

Force a specific webcam.

Leave empty unless multiple webcams are connected.

---

## requestedWidth

Requested webcam capture width.

Recommended:

```json
"requestedWidth": 1080
```

---

## requestedHeight

Requested webcam capture height.

Recommended:

```json
"requestedHeight": 1920
```

---

## requestedFPS

Requested webcam framerate.

Recommended:

```json
"requestedFPS": 30
```

---

# Waveform Settings

## sensitivity

Controls microphone waveform responsiveness.

Examples:

```json
"sensitivity": 15
```

Less movement.

```json
"sensitivity": 25
```

Recommended.

```json
"sensitivity": 40
```

More movement.

---

## smoothSpeed

Controls waveform animation smoothing.

Examples:

```json
"smoothSpeed": 8
```

Slower movement.

```json
"smoothSpeed": 12
```

Recommended.

```json
"smoothSpeed": 20
```

Faster movement.

---

# Troubleshooting

## Audio appears delayed

Reduce:

```json
"audioOffsetSeconds"
```

Example:

```json
"audioOffsetSeconds": -0.25
```

---

## Wrong webcam is displayed

Set:

```json
"preferredDeviceName"
```

to the webcam name shown in the application logs.

---

## Waveform moves too little

Increase:

```json
"sensitivity"
```

Example:

```json
"sensitivity": 35
```

---

## Waveform moves too aggressively

Reduce:

```json
"sensitivity"
```

Example:

```json
"sensitivity": 15
```

---

# Restore Defaults

Replace the contents of config.json with the default example shown in this document.
