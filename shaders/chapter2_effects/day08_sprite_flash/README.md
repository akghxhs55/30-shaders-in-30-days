# Day 08: Sprite Flash

## Overview

The first Chapter 2 shader introduces texture sampling. Unlike Chapter 1 where shapes were generated procedurally, Chapter 2 effects operate on existing sprite textures. Sprite Flash is the simplest texture effect: blending the original sprite color toward a target color, typically triggered when a character takes damage.

The shader itself is minimal — the interesting part is how it integrates with Godot's Tween system to create event-driven animation rather than the continuous `TIME`-based animation from Chapter 1.

---

## Effect

![Sprite Flash](./previews/sprite_flash.gif)

The sprite momentarily flashes to a solid color, then returns to its original appearance.

---

## Shader

The entire shader is a single `mix()` call:

```gdshader
vec4 original = texture(TEXTURE, UV);
COLOR = mix(original, flash_color, flash_amount);
COLOR.a = original.a;
```

`texture(TEXTURE, UV)` samples the sprite's original color at the current pixel. `mix()` blends between the original and the flash color based on `flash_amount` (0.0 = original, 1.0 = fully flashed). The alpha is preserved from the original so the sprite's silhouette remains intact — transparent areas stay transparent even during a flash.

**Parameters:**
- `flash_color` — The color to flash toward (default: white)
- `flash_amount` — Blend factor, driven externally by Tween (0.0–1.0)

---

## Tween-Driven Animation

Unlike Chapter 1 shaders that used `TIME` for continuous animation, Sprite Flash is event-driven. The C# wrapper uses Godot's Tween API to animate `flash_amount`:

```
flash_amount:  0.0 ───▶ 1.0 ───▶ 0.0
               ╰─attack─╯╰─release─╯
               ╰──── duration ─────╯
```

- **Duration** — Total length of the flash effect
- **Attack Ratio** — Proportion of duration spent going from 0→1. The remainder is the release (1→0). A low ratio (0.25) means a quick flash-in and slow fade-out, which feels like a sharp impact.

A `Flash` button in the Inspector triggers the effect for previewing without running the game.

---

## Key Concepts

### 1. Texture Sampling
```gdshader
vec4 original = texture(TEXTURE, UV);
```
`TEXTURE` is a built-in that refers to the sprite's texture. `texture()` samples a color at the given UV coordinates. This is the fundamental operation for all Chapter 2 effects — reading the original pixel, transforming it, and writing the result.

### 2. Alpha Preservation
```gdshader
COLOR.a = original.a;
```
After blending, the alpha channel is restored from the original texture. Without this, transparent areas would become visible during the flash, breaking the sprite's shape.

### 3. Event-Driven vs Time-Driven Animation
Chapter 1 used `TIME` for continuous procedural animation. Sprite Flash introduces a different pattern: the shader exposes a parameter (`flash_amount`), and external code (Tween) drives it in response to game events. This separation lets the same shader serve different contexts — hit feedback, buff indication, death flash — by varying the Tween configuration.

---

## Usage

1. Open `sprite_flash.tscn` in Godot
2. Select the root node
3. In the Inspector, adjust:
    - **Flash Color**: Color to flash toward
    - **Flash Duration**: Total effect length in seconds
    - **Flash Ratio**: Attack/release balance (0.0–1.0)
4. Click the **Flash** button to preview the effect

## Files

- `sprite_flash.gdshader` — The shader implementation
- `sprite_flash.tscn` — Test scene with Godot icon sprite
- `SpriteFlash.cs` — C# wrapper with Tween-driven flash trigger
- `README.md` — This documentation