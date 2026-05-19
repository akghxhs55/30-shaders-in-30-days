# 30 Shaders in 30 Days 🎨

A 30-day shader learning challenge using Godot 4 GDShader. Building practical, game-ready shader effects from the ground up.

## About

This project is a structured journey through shader programming, starting from the basics and progressively building up to complex effects used in real games. Each day focuses on a specific technique with working code and documentation.

## Progress

### Chapter 1: Foundation
| Day | Topic                                                                                    | Status |
|-----|------------------------------------------------------------------------------------------|--------|
| 01  | [UV Basics & Visualization](shaders/chapter1_foundation/day01_uv_basics)                 | ✅      |
| 02  | [Gradient](shaders/chapter1_foundation/day02_gradient)                                   | ✅      |
| 03  | [Stripes & Checkerboard Patterns](shaders/chapter1_foundation/day03_stripe_checkerboard) | ✅      |
| 04  | [Circle & Shapes (SDF Intro)](shaders/chapter1_foundation/day04_sdf_intro)               | ✅      |
| 05  | [Soft Shapes & Anti-aliasing](shaders/chapter1_foundation/day05_anti_aliasing)           | ✅      |
| 06  | [Animation Basics](shaders/chapter1_foundation/day06_animation)                          | ✅      |
| 07  | [**Mini Project**: Animated Loading Spinner](shaders/chapter1_foundation/day07_spinner)  | ✅      |

### Chapter 2: 2D Game Effects
| Day | Topic                                                                                            | Status |
|-----|--------------------------------------------------------------------------------------------------|--------|
| 08  | [Sprite Flash](shaders/chapter2_effects/day08_sprite_flash)                                      | ✅      |
| 09  | [Dissolve Effect](shaders/chapter2_effects/day09_dissolve)                                       | ✅      |
| 10  | [Outline Shader](shaders/chapter2_effects/day10_outline)                                         | ✅      |
| 11  | [Color Swap / Palette Shift](shaders/chapter2_effects/day11_color_swap)                          | ✅      |
| 12  | [Pixelation](shaders/chapter2_effects/day12_pixelation)                                          | ✅      |
| 13  | [Wave Distortion](shaders/chapter2_effects/day13_wave_distortion)                                | ✅      |
| 14  | [**Mini Project**: 2D Character Effects Combo](shaders/chapter2_effects/day14_character_effects) | ✅      |

### Chapter 3: Noise & Procedural Generation
| Day | Topic                                                                                   | Status |
|-----|-----------------------------------------------------------------------------------------|--------|
| 15  | [Value Noise](shaders/chapter3_procedural/day15_value_noise)                            | ✅      |
| 16  | [Perlin & Simplex Noise](shaders/chapter3_procedural/day16_perlin_simplex_noise)        | ✅      |
| 17  | [FBM](shaders/chapter3_procedural/day17_fbm)                                            | ✅      |
| 18  | [Cellular Noise](shaders/chapter3_procedural/day18_cellular_noise)                      | ✅      |
| 19  | [Procedural Fire](shaders/chapter3_procedural/day19_fire)                               | ✅      |
| 20  | [Procedural Water Surface](shaders/chapter3_procedural/day20_water)                     | ✅      |
| 21  | [**Mini Project**: Procedural Background](shaders/chapter3_procedural/day21_background) | ✅      |

### Chapter 4: 3D & Lighting
| Day | Topic                                                                                                      | Status |
|-----|------------------------------------------------------------------------------------------------------------|--------|
| 22  | [Diffuse Lighting (Lambert) + Specular (Blinn-Phong)](shaders/chapter4_lighting_3d/day22_diffuse_specular) | ✅      |
| 23  | [Rim Lighting](shaders/chapter4_lighting_3d/day23_rim)                                                     | ✅      |
| 24  | [Normal Mapping](shaders/chapter4_lighting_3d/day24_normal_mapping)                                        | ✅      |
| 25  | [Toon/Cel Shading](shaders/chapter4_lighting_3d/day25_toon_shading)                                        | ✅      |
| 26  | [Environment Mapping](shaders/chapter4_lighting_3d/day26_environment)                                      | ✅      |
| 27  | [Vertex Deformation](shaders/chapter4_lighting_3d/day27_vertex_deformation)                                | ✅      |
| 28  | **Mini Project**: Stylized 3D Object                                                                       | ⬜      |

### Chapter 5: Post-Processing & Finale
| Day | Topic                                                 | Status |
|-----|-------------------------------------------------------|--------|
| 29  | Screen-space Effects (Vignette, Chromatic Aberration) | ⬜      |
| 30  | **Final Project**: 2048 Shader Edition Prototype      | ⬜      |

## Project Structure

```
30-shaders-in-30-days/
├── shaders/
│   ├── chapter1_foundation/
│   │   ├── day01_uv_basics/
│   │   │   ├── uv_basics.gdshader
│   │   │   ├── uv_basics.tscn
│   │   │   ├── UvBasics.cs
│   │   │   └── README.md
│   │   └── ...
│   ├── chapter2_effects/
│   ├── chapter3_procedural/
│   ├── chapter4_lighting_3d/
│   └── chapter5_postprocess/
└── tools/
    ├── screenshot_capture
    └── uniform_wrapper_generator
```

## Key Concepts by Chapter

| Chapter | Theme           | Core Concepts                                                       |
|---------|-----------------|---------------------------------------------------------------------|
| 1       | Foundation      | UV coordinates, `mix()`, `smoothstep()`, SDF basics, TIME animation |
| 2       | 2D Effects      | Texture sampling, UV distortion, color manipulation                 |
| 3       | Procedural      | Noise functions, FBM, Voronoi, layering techniques                  |
| 4       | 3D & Lighting   | Normals, dot product, Fresnel, tangent space, vertex deformation    |
| 5       | Post-Processing | Screen-space coordinates, full-screen effects                       |

## Environment

- **Engine**: Godot Mono 4.6

## Resources

- [Godot Shading Language Docs](https://docs.godotengine.org/en/stable/tutorials/shaders/shader_reference/shading_language.html)

## License

This project is licensed under the **MIT License**.
