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
4. Set the desired output file path in the Inspector
5. Click the **Capture** button in the Inspector

## Inspector Properties

| Property         | Description                | Default            |
|------------------|----------------------------|--------------------|
| Output File Path | Where to save the PNG file | `res://output.png` |

## Example Workflow

Capturing all modes for Day 01 UV Basics:

1. Set shader `mode` to 0 in the Inspector
2. Set `Output File Path` to `res://shaders/week1_foundation/day01_uv_basics/mode0_rgb.png`
3. Click **Capture**
4. Repeat for modes 1–5, updating the filename each time
