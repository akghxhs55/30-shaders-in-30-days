# Screenshot Capture Tool

A simple editor tool for capturing shader output at exact resolution in Godot 4.

## Purpose

When documenting shaders, you often need screenshots that match the exact resolution of your shader output. This tool allows you to capture a SubViewport's contents directly to a PNG file from within the Godot editor.

## Scene Structure

```
ScreenshotCapture (screenshot_capture.tscn)
└── SubViewportContainer
    └── SubViewport (set your desired resolution, e.g., 512×512)
        └── NodeToCapture (e.g., ColorRect with your shader)
```

## Usage

1. Open screenshot_capture.tscn
2. Set the node you want to capture as a child of the SubViewport (e.g., a ColorRect with your shader)
3. Configure the SubViewport size (e.g., 512×512)
4. Set the output directory and file name in the Inspector
5. Click the **Capture** button in the Inspector

## Inspector Properties

| Property         | Description                                               | Default      |
|------------------|-----------------------------------------------------------|--------------|
| Output Directory | Folder to save the PNG file (supports folder picker)      | `res://`     |
| File Name        | Name of the output file (`.png` auto-appended if missing) | `output.png` |

## Example Workflow

Capturing all modes for Day 01 UV Basics:

1. Set `Output Directory` to `res://shaders/week1_foundation/day01_uv_basics`
2. Set shader `mode` to 0 in the Inspector
3. Set `File Name` to `mode0_rgb.png`
4. Click **Capture**
5. Repeat for modes 1–5, updating the `File Name` each time
