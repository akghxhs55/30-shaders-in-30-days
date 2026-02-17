# 30 Shaders in 30 Days 🎨

A 30-day shader learning challenge using Godot 4 GDShader. Building practical, game-ready shader effects from the ground up.

## About

This project is a structured journey through shader programming, starting from the basics and progressively building up to complex effects used in real games. Each day focuses on a specific technique with working code and documentation.

## Progress

### Chapter 1: Foundation
| Day | Topic                                                                                    | Status |
|-----|------------------------------------------------------------------------------------------|--------|
| 01 | [UV Basics & Visualization](shaders/chapter1_foundation/day01_uv_basics/)                | ✅ |
| 02 | [Gradient](shaders/chapter1_foundation/day02_gradient/)                                  | ✅ |
| 03 | [Stripes & Checkerboard Patterns](shaders/chapter1_foundation/day03_stripe_checkerboard) | ✅ |
| 04 | [Circle & Shapes (SDF Intro)](shaders/chapter1_foundation/day04_sdf_intro)               | ✅ |
| 05 | [Soft Shapes & Anti-aliasing](shaders/chapter1_foundation/day05_anti_aliasing)           | ✅ |
| 06 | Animation Basics                                                                         | ⬜ |
| 07 | **Mini Project**: Animated Loading Spinner                                               | ⬜ |

### Chapter 2: 2D Game Effects
| Day | Topic | Status |
|-----|-------|--------|
| 08 | Sprite Flash (Hit Effect) | ⬜ |
| 09 | Dissolve Effect | ⬜ |
| 10 | Outline Shader | ⬜ |
| 11 | Color Swap / Palette Shift | ⬜ |
| 12 | Pixelation | ⬜ |
| 13 | Wave Distortion | ⬜ |
| 14 | **Mini Project**: 2D Character Effects Combo | ⬜ |

### Chapter 3: Noise & Procedural Generation
| Day | Topic | Status |
|-----|-------|--------|
| 15 | Value Noise | ⬜ |
| 16 | Perlin/Simplex Noise | ⬜ |
| 17 | FBM (Fractal Brownian Motion) | ⬜ |
| 18 | Voronoi / Cellular Noise | ⬜ |
| 19 | Procedural Fire | ⬜ |
| 20 | Procedural Water Surface | ⬜ |
| 21 | **Mini Project**: Procedural Background | ⬜ |

### Chapter 4: 3D & Lighting
| Day | Topic | Status |
|-----|-------|--------|
| 22 | Diffuse Lighting (Lambert) | ⬜ |
| 23 | Specular (Blinn-Phong) | ⬜ |
| 24 | Rim Lighting | ⬜ |
| 25 | Normal Mapping | ⬜ |
| 26 | Toon/Cel Shading | ⬜ |
| 27 | Fresnel + Hologram Effect | ⬜ |
| 28 | **Mini Project**: Stylized 3D Object | ⬜ |

### Chapter 5: Post-Processing & Finale
| Day | Topic | Status |
|-----|-------|--------|
| 29 | Screen-space Effects (Vignette, Chromatic Aberration) | ⬜ |
| 30 | **Final Project**: 2048 Shader Edition Prototype | ⬜ |

## Project Structure

```
30-shaders-in-30-days/
├── shaders/
│   ├── chapter1_foundation/
│   │   ├── day01_uv_basics/
│   │   │   ├── uv_basics.gdshader
│   │   │   ├── uv_basics.tscn
│   │   │   └── README.md
│   │   └── ...
│   ├── chapter2_2d_effects/
│   ├── chapter3_noise/
│   ├── chapter4_lighting/
│   └── chapter5_postprocess/
├── assets/
│   ├── textures/
│   └── models/
├── previews/
├── common/
│   └── shader_utils.gdshaderinc
└── scenes/
	├── gallery.tscn
	└── test_scene.tscn
```

## Key Concepts by Chapter

| Chapter | Theme | Core Concepts |
|---------|-------|---------------|
| 1       | Foundation | UV coordinates, `mix()`, `smoothstep()`, SDF basics, TIME animation |
| 2       | 2D Effects | Texture sampling, UV distortion, color manipulation |
| 3       | Procedural | Noise functions, FBM, Voronoi, layering techniques |
| 4       | 3D & Lighting | Normals, dot product, Fresnel, tangent space |
| 5       | Post-Processing | Screen-space coordinates, full-screen effects |

## Environment

- **Engine**: Godot Mono 4.6

## Resources

- [Godot Shading Language Docs](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html)

## License

This project is licensed under the **MIT License**.
