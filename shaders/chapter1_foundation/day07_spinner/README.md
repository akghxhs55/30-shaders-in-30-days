# Day 07: Animated Loading Spinner

## Overview

The Chapter 1 mini project synthesizes concepts from Days 01–06 into three distinct loading spinners. Each spinner uses a different combination of techniques, demonstrating how the foundational building blocks compose into practical visual effects.

---

## Spinner Modes

### Arc Spinner
![Arc Spinner](./previews/arc_spinner.gif)

A rotating arc with dynamically changing length. Combines:

- **Arc SDF** — A Ring SDF (Day 04) restricted to an angular range, with endpoint distance for pixels outside the arc
- **Rotation matrix** — `mat2` rotates UV coordinates over time (Day 06)
- **Variable arc length** — `sin(TIME)` modulates the arc's half-angle, creating a breathing effect where the arc stretches and contracts as it spins (Day 06)
- **Auto AA** — `smoothstep(fwidth(sdf), -fwidth(sdf), sdf)` for resolution-independent anti-aliasing (Day 05)

The arc SDF handles two regions: pixels within the angular range use the standard ring distance, while pixels outside use Euclidean distance to the nearest endpoint. This produces clean rounded caps at both ends of the arc.

### Dots Spinner
![Dots Spinner](./previews/dots_spinner.gif)

Twelve dots arranged in a circle, with size, opacity, and position varying based on proximity to the active point. Combines:

- **Angular indexing** — `round(atan(uv) / spacing)` maps each pixel to its nearest dot without looping (Day 02)
- **Circular distance** — `mod()` computes wrap-around distance between dot index and active index (Day 03)
- **Intensity-driven properties** — Closer dots are larger, more opaque, and positioned at the full radius; distant dots shrink, fade, and pull inward
- **Circle SDF** — Each dot rendered as `length(uv - pos) - radius` (Day 04)

### Pulsing Ring
![Pulsing Ring](./previews/pulsing_ring.gif)

Rings expanding outward from the center with staggered timing, fading as they grow. Combines:

- **Ring SDF** — `abs(length(p) - radius) - thickness` (Day 04)
- **Ease-out expansion** — `1.0 - pow(1.0 - t, 2.0)` makes rings expand quickly at first, then decelerate (Day 06)
- **Fade-in and fade-out** — `smoothstep(0.0, 0.3, t) * (1.0 - t)` creates a smooth appear-and-vanish lifecycle
- **Duty cycle** — Rings animate during 60% of their period and rest for 40%, creating natural breathing rhythm (Day 03)
- **Phase offset** — Two rings at 0.5 offset ensure visual continuity while allowing moments where only one ring is visible

---

## Usage

1. Open `spinner.tscn` in Godot
2. Select the root node
3. In the Inspector, adjust:
   - **Mode**: Choose spinner style (Arc / Dots / Pulsing Ring)
   - **Speed**: Control animation speed

## Files

- `spinner.gdshader` — The shader implementation
- `spinner.tscn` — Test scene
- `Spinner.cs` — C# wrapper exposing shader parameters to the Inspector
- `README.md` — This documentation
