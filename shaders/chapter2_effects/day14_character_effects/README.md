# Day 14: 2D Character Effects Combo

## Overview

The Chapter 2 mini-project combines the effects from Days 08–13 into a single integrated shader. Rather than applying each effect as a separate material on separate layers, all effects share one shader that exposes every parameter as a uniform. The C# wrapper is responsible for translating game states and events into the correct parameter combinations.

---

## Architecture

### Shader Pipeline

Effects are applied in a fixed order inside `fragment()`:

```
1. apply_burned_distortion   → UV warping (heat haze)
2. apply_pixelation          → UV snapping (block pixelation)
3. texture()                 → sprite sampling
4. apply_frozen              → tint + shimmer
5. apply_burned              → tint + edge glow
6. apply_flash               → color overlay (hit / resurrect)
7. apply_dissolve            → alpha cutout
```

UV-modifying effects (distortion, pixelation) run before sampling so the downstream effects operate on already-warped pixels. Alpha-cutting effects (dissolve) run last so they affect the final result regardless of what other effects are active.

---

## States & Effects

### Conditions

Conditions are mutually exclusive persistent states managed via the `Condition` property.

#### None
Default state. All condition-related parameters are at 0.

#### Frozen

![Frozen](./previews/frozen.gif)

Blue tint spreads from bottom to top as `frozen_progress` increases. The tint preserves the original sprite's luminance to maintain shape detail.

```gdshader
float luminance = dot(color.rgb, vec3(0.299, 0.587, 0.114));
vec3 frozen_color = frozen_tint * luminance * 2.0;
float frozen_value = smoothstep(frozen_progress, frozen_progress - 0.3, 1.0 - uv.y);
color.rgb = mix(color.rgb, frozen_color, frozen_value);
```

A diagonal shimmer sweeps across the frozen region every 3 seconds, lasting 0.45 seconds per pass:

```gdshader
float cycle = fract(TIME / 3.0);
float t = cycle / 0.15;
float shimmer = smoothstep(0.0, 0.05, band) * smoothstep(0.1, 0.05, band) * step(t, 1.0);
color.rgb += shimmer * frozen_value;
```

#### Burned

![Burned](./previews/burned.gif)

Orange tint applied uniformly, heat-haze UV distortion, and an exponential radial glow around the sprite's edge.

The glow samples 5 radii (1, 2, 4, 8, 16 pixels) × 8 directions = 40 texture samples per fragment, with strength decreasing linearly with radius:

```gdshader
for (int r = 0; r < 5; r++) {
    float radius = pow(2.0, float(r));
    // sample 8 directions at this radius
    glow_strength = max(glow_strength, ring_alpha * (1.0 - float(r) / 5.0));
}
```

The glow extends outside the sprite's alpha boundary, so `color.a` is expanded accordingly.

---

### Triggered Effects

Triggered effects are one-shot or toggled independently of the condition state.

#### Die / Resurrect

![Die](./previews/die_resurrect.gif)

Die dissolves the sprite into noise while simultaneously pixelating it. Resurrect reverses the process and adds a green flash.

```
Die:        threshold 0→1, pixelation_size 1→64  (parallel, 1.0s)
Resurrect:  threshold 1→0, pixelation_size 64→1  (parallel, 1.0s)
            resurrect_flash_amount 0→1 (0.8s) → 0 (0.3s)
```

Die and Resurrect are guarded by `CharacterState` — Die does nothing if already `Died`, Resurrect does nothing if already `Idle`.

#### Hit

![Hit](./previews/hit.gif)

White flash + brief scale pulse. Triggered via the **Hit** button.

```csharp
tween: hit_flash_amount  0 → 1 (0.05s) → 0 (0.15s)
tween: sprite.scale      1.0 → 1.2 (0.05s) → 1.0 (0.15s)
````

---

## Key Concepts

### 1. Function-Per-Effect Pipeline

Each effect is isolated in its own function with a clear input/output contract:

```gdshader
vec2 apply_pixelation(vec2 uv, vec2 texture_size) → vec2
vec4 apply_frozen(vec2 uv, vec4 color)            → vec4
vec4 apply_burned(vec2 uv, vec4 color, ...)       → vec4
```

This makes the pipeline easy to read, reorder, and extend. `fragment()` reads as a sequence of named transformations rather than a wall of math.

### 2. Luminance-Preserving Tint

Both Frozen and Burned replace hue while preserving the sprite's original brightness:

```gdshader
float luminance = dot(color.rgb, vec3(0.299, 0.587, 0.114));
vec3 tinted = tint_color * luminance * 2.0;
```

A flat `mix(color, tint, t)` would wash out detail as `t` increases. Multiplying by luminance keeps dark areas dark and bright areas bright within the new color.

### 3. Condition Transition

Switching conditions animates the outgoing condition back to neutral before the incoming condition animates in:

```csharp
set {
    RevertCondition(_condition); // animate old condition → 0
    _condition = value;
    ApplyCondition(_condition);  // animate new condition → 1
}
```

Since `frozen_progress` and `burned_progress` are independent uniforms, there is no conflict if a transition is interrupted mid-animation.

---

## Usage

1. Open `character_effects.tscn` in Godot
2. Select the root node
3. In the Inspector:
    - **Condition** — Switch between `None`, `Frozen`, and `Burned`
    - **Hit** button — Trigger white flash + scale pulse
    - **Die** button — Dissolve + pixelate (requires `Idle` state)
    - **Resurrect** button — Reverse dissolve + green flash (requires `Died` state)

## Files

- `character_effects.gdshader` — Combined shader with all Chapter 2 effects
- `character_effects.tscn` — Test scene
- `CharacterEffects.cs` — C# wrapper managing states, conditions, and Tween sequences
- `README.md` — This documentation