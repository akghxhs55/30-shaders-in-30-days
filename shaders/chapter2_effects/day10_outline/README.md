# Day 10: Outline Shader

## Overview

Outline renders a colored border around a sprite's silhouette. The core idea is **neighbor sampling**: for each pixel, sample surrounding pixels to determine if there is an alpha boundary nearby, then color the boundary accordingly.

The two concerns are independent — the outline mode defines *how* neighbors are sampled, and the placement mode defines *where* the outline appears relative to the sprite's edge.

---

## Outline Modes

### Four Directional

![Four Directional](./previews/four_dir.png)

Samples four neighbors: up, down, left, right. Fast and sufficient for most use cases

```gdshader
vec2 offsets[4] = { vec2(0,1), vec2(0,-1), vec2(1,0), vec2(-1,0) };
```

At large thickness values, the outline may show slight gaps at diagonal corners since those directions are not sampled.

### Eight Directional

![Eight Directional](./previews/eight_dir.png)

Adds the four diagonal directions. Produces a more complete outline at the cost of four extra samples.

```gdshader
// Diagonal offsets normalized to match cardinal distance
vec2(1, 1) / sqrt(2.0), vec2(1, -1) / sqrt(2.0), ...
```

The `1.0 / sqrt(2.0)` normalization ensures diagonal offsets stay at the same distance as cardinal offsets. Without it, diagonal samples would reach farther, making the outline uneven.

### Radial

![Radial](./previews/radial.png)

Samples `sample_count` points evenly distributed around a circle. Produces a smooth, uniform outline at any thickness.

```gdshader
for (int i = 0; i < sample_count; i++) {
    float angle = float(i) / float(sample_count) * TAU;
    vec2 offset_uv = UV + vec2(cos(angle), sin(angle)) * thickness * TEXTURE_PIXEL_SIZE;
    ...
}
```

Higher `sample_count` values produce smoother outlines at the cost of more texture samples per fragment.

- **Sample Count** — Number of radial samples

---

## Placement Modes

### Outer

![Outer](./previews/outer.png)

The outline is drawn outside the sprite. Transparent pixels adjacent to opaque sprite pixels are colored.

```gdshader
outline_mask = (1.0 - alpha_mask) * max_neighbor_alpha;
```

### Inner

![Inner](./previews/inner.png)

The outline is drawn inside the sprite. Opaque pixels adjacent to transparent areas are colored.

```gdshader
outline_mask = alpha_mask * (1.0 - min_neighbor_alpha);
```

Inner requires tracking the *minimum* neighbor alpha — a pixel is on the inner edge if at least one neighbor is transparent (`min_neighbor_alpha < 1.0`).

### Both

![Both](./previews/both.png)

Draws outlines both inside and outside the sprite edge simultaneously.

```gdshader
outline_mask = max(
    (1.0 - alpha_mask) * max_neighbor_alpha,
    alpha_mask * (1.0 - min_neighbor_alpha)
);
```

---

## Key Concepts

### 1. Neighbor Sampling Pipeline

```gdshader
float max_neighbor_alpha = 0.0; // Is there an opaque neighbor? → Outer
float min_neighbor_alpha = 1.0; // Is there a transparent neighbor? → Inner

// Both computed in a single sampling loop
max_neighbor_alpha = max(max_neighbor_alpha, texture(TEXTURE, offset_uv).a);
min_neighbor_alpha = min(min_neighbor_alpha, texture(TEXTURE, offset_uv).a);
```

### 2. Alpha Binarization

Sprites with anti-aliased edges have semi-transparent pixels at the boundary. Without binarization, these pixels satisfy both the inner and outer conditions simultaneously, creating a thin artifact line in Both mode.

```gdshader
float alpha_mask = step(0.5, original.a);
```

Snapping to 0 or 1 ensures each pixel is classified as strictly inside or outside the sprite.

### 3. Alpha Extension (Outer Placement)

Outer placement draws outline pixels in areas that were originally transparent. The final alpha must account for this:

```gdshader
COLOR.a = max(original.a, outline_mask * outline_color.a);
```

`max()` preserves the sprite's original alpha while extending it outward wherever the outline appears.

---

## Usage

1. Open `outline.tscn` in Godot
2. Select the root node
3. In the Inspector, configure:
   - **Outline Mode** — `Four Directional`, `Eight Directional`, or `Radial`
   - **Placement** — `Outer`, `Inner`, or `Both`
   - **Thickness** — Outline width in pixels (0–40)
   - **Outline Color** — Color and alpha of the outline
   - **Sample Count** — Radial mode only; higher = smoother outline
4. Adjust parameters in real time to preview the effect

## Files

- `outline.gdshader` — Shader with 3 outline modes × 3 placement modes
- `outline.tscn` — Test scene with the shared chapter sprite
- `Outline.cs` — C# wrapper exposing all shader parameters
- `README.md` — This documentation
