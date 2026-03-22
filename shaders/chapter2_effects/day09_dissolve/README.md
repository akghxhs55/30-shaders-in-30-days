# Day 09: Dissolve Effect

## Overview

Dissolve is a transition effect where a sprite disappears (or appears) pixel by pixel, driven by a threshold value. The key idea is a **map-then-edge** pipeline: first compute a `map_value` per pixel that determines when that pixel dissolves, then apply edge processing to control the visual style of the boundary.

---

## Dissolve Modes

### Noise

![Noise Dissolve](./previews/noise.gif)

The `map_value` is sampled from a noise texture. Pixels with low noise values dissolve first, creating an organic, irregular boundary.

```gdshader
map_value = texture(noise_texture, UV).r;
```

Assign a `NoiseTexture2D` (with a `FastNoiseLite` inside) to `noise_texture` in the Inspector. Adjusting the noise frequency and type changes the granularity of the dissolve pattern.

### Directional

![Directional Dissolve](./previews/directional.gif)

The `map_value` is the projection of the UV coordinate onto a direction vector. This produces a straight-line boundary that sweeps across the sprite at any angle.

```gdshader
map_value = dot(UV - 0.5, vec2(cos(angle), sin(angle))) + 0.5;
```

The `angle` parameter (0–2π) controls the direction. At 0, the sprite dissolves left-to-right; at π/2, bottom-to-top.

### Radial

![Radial Dissolve](./previews/radial.gif)

The `map_value` is the distance from the sprite's center. Dissolution expands outward from the center (or collapses inward when appearing).

```gdshader
map_value = length(UV - 0.5) * sqrt(2.0);
```

The `sqrt(2.0)` factor normalizes the range so that the sprite corners reach `map_value = 1.0`, ensuring the sprite fully dissolves when `threshold = 1.0`.

---

## Edge Modes

Edge modes are fully independent of dissolve mode — any combination works.

### Hard

![Hard Edge](./previews/hard.gif)

A sharp cutoff using `step()`. Pixels are either fully visible or fully invisible.

```gdshader
alpha = step(threshold, map_value);
```

### Soft

![Soft Edge](./previews/soft.gif)

A smooth fade using `smoothstep()`. Pixels near the threshold boundary fade out gradually.

```gdshader
float t = threshold * (1.0 + softness);
alpha = smoothstep(t - softness, t, map_value);
```

The `threshold` is scaled by `(1.0 + softness)` so that the sprite fully disappears at exactly `threshold = 1.0` regardless of the softness value.

- **Softness** — Width of the fade region (0.0 = same as Hard, 1.0 = widest fade)

### Burn

![Burn Edge](./previews/burn.gif)

A glowing burn effect at the dissolve boundary. Two smoothstep ranges create a narrow white band just inside the dissolving edge.

```gdshader
float t = mix(-burn_width * 2.0, 1.0, threshold);
float alpha = smoothstep(t, t + burn_width, map_value);
float fill  = smoothstep(t + burn_width, t + burn_width * 2.0, map_value);
original.rgb = mix(vec3(1.0), original.rgb, fill);
```

`alpha` controls visibility; `fill` controls the color transition from white to the original. The threshold is offset with `mix(-burn_width * 2.0, 1.0, threshold)` to ensure no burn band appears at `threshold = 0.0` and the sprite is fully dissolved at `threshold = 1.0`.

- **Burn Width** — Thickness of the burn band (0.0–1.0)

---

## Key Concepts

### 1. Threshold Compensation

Both Soft and Burn modes extend their smoothstep range beyond `[0, 1]` to ensure that `threshold = 0.0` means fully visible and `threshold = 1.0` means fully dissolved:

| Mode | Adjustment |
|------|-----------|
| Soft | `t = threshold * (1.0 + softness)` — extends upper bound |
| Burn | `t = mix(-burn_width * 2.0, 1.0, threshold)` — extends lower bound |

Without this, a softness or burn_width > 0 would cause partial dissolution at `threshold = 0` or incomplete dissolution at `threshold = 1`.

### 2. Radial Normalization

`length(UV - 0.5)` reaches a maximum of `sqrt(0.5) ≈ 0.707` at the sprite corners, not 1.0. Multiplying by `sqrt(2.0)` maps the corners to exactly 1.0, so the full dissolve range is used.

---

## Usage

1. Open `dissolve.tscn` in Godot
2. Select the root node
3. In the Inspector, configure:
   - **Dissolve Mode** — `Noise`, `Directional`, or `Radial`
   - **Edge Mode** — `Hard`, `Soft`, or `Burn`
   - Mode-specific parameters (`Noise Texture`, `Angle`, `Softness`, `Burn Width`)
   - **Duration** — Transition duration in seconds
4. Click **Dissolve** to preview the disappear animation, **Appear** to reverse it

## Files

- `dissolve.gdshader` — Shader with 3 dissolve modes × 3 edge modes
- `dissolve.tscn` — Test scene with the shared chapter sprite
- `Dissolve.cs` — C# wrapper with Tween-driven transition triggers
- `README.md` — This documentation
