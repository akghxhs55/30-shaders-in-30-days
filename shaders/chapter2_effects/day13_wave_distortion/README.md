# Day 13: Wave Distortion

## Overview

Wave Distortion shifts UV coordinates using a sine wave before sampling the sprite texture. The result is a rippling, undulating effect that alters only the position each pixel is read from, leaving colors intact.

---

## Modes

### Horizontal

![Horizontal](./previews/horizontal.gif)

UV.x is displaced by a sine wave driven by UV.y. Each row of pixels shifts left or right by an amount that varies along the vertical axis, creating a side-to-side ripple.

```gdshader
float wave_x = sin(UV.y * frequency * TAU + t) * amplitude * TEXTURE_PIXEL_SIZE.x;
offset = vec2(wave_x, 0.0);
```

Typical use: water surface reflections, heat haze, transparent glass distortion.

### Vertical

![Vertical](./previews/vertical.gif)

UV.y is displaced by a sine wave driven by UV.x. Each column shifts up or down, producing a top-to-bottom ripple.

```gdshader
float wave_y = sin(UV.x * frequency * TAU + t) * amplitude * TEXTURE_PIXEL_SIZE.y;
offset = vec2(0.0, wave_y);
```

Typical use: flags, curtains, cloth, seaweed swaying in the wind.

### Both

![Both](./previews/both.gif)

Both axes are displaced simultaneously. The sprite warps in two directions at once, creating a more chaotic, turbulent distortion.

```gdshader
offset = vec2(wave_x, wave_y);
```

Typical use: portals, heat shimmer over large areas, underwater distortion.

---

## Key Concepts

### 1. UV Displacement

Instead of sampling `texture(TEXTURE, UV)`, the shader samples at a shifted position:

```gdshader
vec2 distorted_uv = UV + offset;
COLOR = texture(TEXTURE, distorted_uv);
```

The original sprite is untouched — only the lookup coordinates change. This means distortion has no effect on color or alpha, only on perceived shape.

### 2. Pixel-Space Amplitude

`amplitude` is expressed in pixels and converted to UV space using `TEXTURE_PIXEL_SIZE`:

```gdshader
wave_x = sin(...) * amplitude * TEXTURE_PIXEL_SIZE.x;
```

`TEXTURE_PIXEL_SIZE` is the size of one texel in UV space (`1.0 / texture_size`). This keeps `amplitude = 10.0` meaning "10 pixels" regardless of texture resolution.

---

## Parameters

| Parameter     | Range                        | Description                             |
|---------------|------------------------------|-----------------------------------------|
| **Direction** | Horizontal / Vertical / Both | Axis of distortion                      |
| **Frequency** | 0.1–20.0                     | Number of wave cycles across the sprite |
| **Amplitude** | 0.0–100.0                    | Displacement strength in pixels         |
| **Speed**     | 0.0–10.0                     | Wave travel speed                       |

---

## Usage

1. Open `wave_distortion.tscn` in Godot
2. Select the root node
3. In the Inspector, configure Direction, Frequency, Amplitude, and Speed
4. The effect animates automatically in-editor via `TIME`

## Files

- `wave_distortion.gdshader` — Shader with Horizontal, Vertical, and Both modes
- `wave_distortion.tscn` — Test scene with the shared chapter sprite
- `WaveDistortion.cs` — C# wrapper exposing all shader parameters
- `README.md` — This documentation
