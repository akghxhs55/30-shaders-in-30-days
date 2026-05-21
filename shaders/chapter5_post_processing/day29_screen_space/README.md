# Day 29: Screen-Space Effects

## Overview

Implements four post-processing effects as a `canvas_item` shader on a full-screen `ColorRect` inside a `CanvasLayer`. Each effect can be toggled independently. Effects are applied in a fixed pipeline order — Barrel Distortion first (UV modification), then Chromatic Aberration (texture sampling), then Vignette and Film Grain (color modification).

---

## Barrel Distortion

![Barrel Distortion](./previews/barrel_distortion.png)

Warps the screen UV radially outward from the center, simulating the lens distortion of a wide-angle or fisheye lens. Negative values produce pincushion distortion. Used in horror games, FPS games for scope/lens effects, and retro CRT aesthetics.

```gdshader
vec2 centered = uv * 2.0 - 1.0;
float r2 = dot(centered, centered);
centered *= 1.0 + r2 * barrel_strength;
uv = centered * 0.5 + 0.5;
```

---

## Chromatic Aberration

![Chromatic Aberration](./previews/chromatic_aberration.png)

Samples the R, G, B channels from different screen positions, simulating the color fringing of real optical lenses. The offset direction radiates from the screen center — strongest at the edges, zero at the center. Commonly used for hit effects, psychedelic transitions, and sci-fi displays.

```gdshader
vec2 dir = uv - 0.5;
float r = texture(screen_texture, uv + dir * strength).r;
float g = texture(screen_texture, uv).g;
float b = texture(screen_texture, uv - dir * strength).b;
color = vec3(r, g, b);
```

---

## Vignette

![Vignette](./previews/vignette.png)

Darkens the screen toward the edges using a radial falloff, drawing focus toward the center. Widely used in cinematic rendering, horror games, and photo editing.

```gdshader
vec2 centered = uv - 0.5;
float dist = dot(centered, centered) * 2.0;
float vignette = 1.0 - smoothstep(vignette_radius, vignette_radius + 0.5, dist * vignette_strength);
color *= vignette;
```

---

## Film Grain

![Film Grain](./previews/film_grain.png)

Adds per-frame random noise to every pixel, simulating the grain of analog film. `fract(TIME)` seeds the hash function differently each frame while keeping the input range bounded to avoid floating-point precision loss. Used in cinematic post-processing, retro aesthetics, and night-vision effects.

```gdshader
float random(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453);
}

float grain = random(uv + fract(TIME)) * 2.0 - 1.0;
color += grain * grain_strength;
```

---

## Key Concepts

### Scene structure

Unlike previous days, the shader is on a `ColorRect` inside a `CanvasLayer`, not a `SubViewport`. Scene content sits outside the `CanvasLayer` and is captured by `hint_screen_texture`:

```
Node
├── [scene content]     ← rendered first, captured by screen_texture
└── CanvasLayer
    └── ColorRect       ← full screen, ShaderMaterial applied here
```

### Effect pipeline order

UV-modifying effects must precede texture sampling. Color-modifying effects follow:

```
① Barrel Distortion  → modifies uv
② Chromatic Aberration → samples screen_texture using modified uv
③ Vignette           → multiplies color
④ Film Grain         → adds to color
```

### `hint_screen_texture`

Provides the rendered frame up to the point where this shader runs. Since `CanvasLayer` renders after the scene, all scene content is available in the texture.

---

## Parameters

| Parameter                         | Range    | Default | Description                                 |
|-----------------------------------|----------|---------|---------------------------------------------|
| **Use Barrel Distortion**         | bool     | false   | Toggle lens warp                            |
| **Barrel Strength**               | -0.2–1.0 | 0.1     | Distortion intensity; negative = pincushion |
| **Use Chromatic Aberration**      | bool     | false   | Toggle color fringing                       |
| **Chromatic Aberration Strength** | 0.0–0.1  | 0.005   | RGB channel separation distance             |
| **Use Vignette**                  | bool     | false   | Toggle edge darkening                       |
| **Vignette Strength**             | 0.0–2.0  | 1.0     | Falloff intensity                           |
| **Vignette Radius**               | 0.0–1.0  | 0.5     | Where darkening begins                      |
| **Use Film Grain**                | bool     | false   | Toggle analog noise                         |
| **Grain Strength**                | 0.0–0.1  | 0.03    | Noise amplitude                             |

*Sub-parameters are hidden in the Inspector when their effect is disabled.*

---

## Usage

1. Open `screen_space.tscn` in Godot
2. Add scene content (sprites, SubViewport, etc.) as siblings of the `CanvasLayer`
3. Toggle effects in the Inspector and adjust parameters

---

## Files

- `screen_space.gdshader` — Four post-processing effects in a single canvas_item shader
- `screen_space.tscn` — Scene with CanvasLayer and full-screen ColorRect
- `ScreenSpace.cs` — C# wrapper with per-effect toggles and conditional Inspector visibility
- `README.md` — This documentation
