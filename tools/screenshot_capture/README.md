# Screenshot Capture Tool

An editor tool for capturing shader output in Godot 4 — both static PNG screenshots and animated frame sequences for GIF conversion.

## Purpose

When documenting shaders, you need captures that match the exact resolution of your shader output. This tool captures a SubViewport's contents directly from within the Godot editor, supporting both single-frame screenshots and multi-frame animation sequences.

## Scene Structure

```
ScreenshotCapture (screenshot_capture.tscn)
└── SubViewportContainer
    └── SubViewport (set your desired resolution, e.g., 512×512)
        └── NodeToCapture (e.g., ColorRect with your shader)
```

## Usage

### Static Screenshot

1. Open `screenshot_capture.tscn`
2. Set the node you want to capture as a child of the SubViewport
3. Configure the SubViewport size (e.g., 512×512)
4. Set the output directory and file name in the Inspector
5. Click the **Capture** button

### Animation Capture

1. Same setup as above
2. Set **Duration** (how many seconds to record) and **FPS** (frame rate)
3. Click the **Capture Animation** button
4. Wait for the capture to complete (progress is logged in the Output panel)
5. Convert the frame sequence to GIF using ffmpeg (see below)

## Inspector Properties

### Output Settings

| Property         | Description                                               | Default      |
|------------------|-----------------------------------------------------------|--------------|
| Output Directory | Folder to save files (supports folder picker)             | `res://`     |
| File Name        | Base name for output (`.png` auto-appended if missing)    | `output.png` |

### Animation Capture

| Property | Description                          | Default |
|----------|--------------------------------------|---------|
| Duration | Recording length in seconds          | `2.0`   |
| FPS      | Frames per second (10–30)            | `15`    |

## Output Format

- **Static**: `{FileName}.png`
- **Animation**: `{FileName}_0000.png`, `{FileName}_0001.png`, `{FileName}_0002.png`, ...

## Converting Frames to GIF

Use ffmpeg to combine the frame sequence into a GIF with optimized colors:

```bash
# Generate a color palette for better quality
ffmpeg -framerate 15 -i output_%04d.png -vf "palettegen" palette.png

# Create GIF using the palette
ffmpeg -framerate 15 -i output_%04d.png -i palette.png -lavfi "paletteuse" output.gif
```

Replace `15` with your chosen FPS and `output` with your file name.

## Example Workflow

### Capturing static modes for Day 01 UV Basics

1. Set `Output Directory` to `res://shaders/chapter1_foundation/day01_uv_basics/previews`
2. Set shader `mode` to 0 in the Inspector
3. Set `File Name` to `mode0_rgb`
4. Click **Capture**
5. Repeat for modes 1–5, updating the file name each time

### Capturing animation for Day 06

1. Set `Output Directory` to `res://shaders/chapter1_foundation/day06_animation/previews`
2. Set `File Name` to `anim_scrolling`
3. Set `Duration` to `2.0`, `FPS` to `15`
4. Click **Capture Animation**
5. Convert with ffmpeg: `ffmpeg -framerate 15 -i anim_scrolling_%04d.png -vf "palettegen" palette.png && ffmpeg -framerate 15 -i anim_scrolling_%04d.png -i palette.png -lavfi "paletteuse" anim_scrolling.gif`