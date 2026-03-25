# Day 12: Pixelation

## Overview

Pixelation intentionally reduces the apparent resolution of a sprite by grouping pixels into blocks. The three modes differ in how each block's color is determined — from a single sample to a full average — and the Dot mode adds a visual style on top.

---

## Modes

### Snap

![Snap](./previews/snap.png)

The simplest form of pixelation. Each pixel's UV is snapped to the center of its block, so every pixel in the same block samples the same texel.

```gdshader
vec2 snapped = (floor(UV * float(pixel_count)) + 0.5) / float(pixel_count);
color = texture(TEXTURE, snapped);
```

The `+ 0.5` offset is important — without it, the block samples its top-left corner instead of its center, which can shift the apparent image slightly.

### Average

![Average](./previews/average.png)

Samples a regular NxN grid within each block and averages the results. This is a closer approximation of true downsampling — each block's color represents the average of its region rather than a single point.

```gdshader
vec2 cell = floor(UV * float(pixel_count)) / float(pixel_count);
vec2 cell_size = vec2(1.0) / float(pixel_count);

vec4 sum = vec4(0.0);
for (int x = 0; x < sample_count; x++) {
    for (int y = 0; y < sample_count; y++) {
        vec2 offset = (vec2(float(x), float(y)) + 0.5) / float(sample_count) * cell_size;
        sum += texture(TEXTURE, cell + offset);
    }
}
color = sum / float(sample_count * sample_count);
```

- **Sample Count** — Grid resolution per block (1–16). `sample_count = 1` is equivalent to Snap. Note that sample cost grows as `sample_count²`.

### Dot

![Dot](./previews/dot.png)

Applies a circular mask within each block, creating a retro LCD or CRT dot matrix appearance. The block color is sampled the same way as Snap, but pixels outside the circle are made transparent.

```gdshader
vec2 cell_uv = fract(UV * float(pixel_count)) * 2.0 - 1.0;
vec2 snapped = (floor(UV * float(pixel_count)) + 0.5) / float(pixel_count);
vec4 cell_color = texture(TEXTURE, snapped);

float dist = length(cell_uv);
float mask = step(dist, dot_size);
color = vec4(cell_color.rgb, cell_color.a * mask);
```

- **Dot Size** — Radius of the circle within each block (0.0–1.0). At 1.0, circles from adjacent blocks touch; below ~0.7, gaps between dots become visible.

---

## Key Concepts

### 1. UV Snapping

The core of pixelation is collapsing a range of UV values to a single point:

```gdshader
floor(UV * pixel_count) / pixel_count
```

`UV * pixel_count` maps the 0–1 UV range to 0–`pixel_count`. `floor()` truncates to the block index. Dividing back gives the block's starting UV. Adding `0.5 / pixel_count` shifts to the block center.

### 2. Average vs Snap

Snap picks one representative texel per block. Average integrates across the block. For low `pixel_count` values (large blocks), the difference is visible: Snap can misrepresent a block if its center texel is an outlier, while Average is more faithful to the original image's content.

However, Average mode can appear blurry. Because each block samples multiple points and averages them, color boundaries become softened — adjacent blocks with different colors blend at their edges. This is an inherent tradeoff of the averaging approach rather than a rendering artifact.

---

## Usage

1. Open `pixelation.tscn` in Godot
2. Select the root node
3. In the Inspector, configure:
   - **Pixelation Mode** — `Snap`, `Average`, or `Dot`
   - **Pixel Count** — Number of blocks across the texture (1–512); lower = more pixelated
   - **Sample Count** — Average mode only; NxN samples per block
   - **Dot Size** — Dot mode only; circle radius within each block
4. Adjust parameters in real time to preview

## Files

- `pixelation.gdshader` — Shader with Snap, Average, and Dot modes
- `pixelation.tscn` — Test scene with the shared chapter sprite
- `Pixelation.cs` — C# wrapper exposing all shader parameters
- `README.md` — This documentation
