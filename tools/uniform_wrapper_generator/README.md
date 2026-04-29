# Uniform Wrapper Generator — GDShader Uniform to C# Export Generator

A command-line tool that parses GDShader uniform declarations and generates C# wrapper property code for Godot `[Tool]` scripts.

## Purpose

Each shader in this project is wrapped by a C# `[Tool]` script that exposes uniform parameters as `[Export]` properties. Writing these properties by hand is repetitive — every uniform needs a backing field, a setter that calls `UpdateShaderParam()`, and the correct `[Export]` attribute. This tool automates that conversion.

## Usage

From the project root:

```bash
cargo run -- <path-to-gdshader>
```

Example:

```bash
cargo run -- shaders/chapter1_foundation/day04_sdf_intro/sdf_intro.gdshader
```

Output to file:

```bash
cargo run -- shader.gdshader > output.txt
```

Or with a release build:

```bash
cargo build --release
./target/release/uniform_wrapper_generator shader.gdshader
```

## Supported Uniform Types

| GDShader        | Hint              | C# Type          | Export                                |
|-----------------|-------------------|------------------|---------------------------------------|
| `int`           | `hint_enum(...)`  | Generated `enum` | `[Export]`                            |
| `int`           | `hint_range(...)` | `int`            | `[Export(PropertyHint.Range, "...")]` |
| `float`         | `hint_range(...)` | `float`          | `[Export(PropertyHint.Range, "...")]` |
| `float`         | —                 | `float`          | `[Export]`                            |
| `vec2`          | —                 | `Vector2`        | `[Export]`                            |
| `vec3` / `vec4` | `source_color`    | `Color`          | `[Export]`                            |
| `vec3`          | —                 | `Vector3`        | `[Export]`                            |
| `vec4`          | —                 | `Vector4`        | `[Export]`                            |
| `bool`          | —                 | `bool`           | `[Export]`                            |
| `sampler2D`     | —                 | `Texture2D`      | `[Export]`                            |

## Initializer Conversion

Default values are automatically converted from GDShader to C# syntax:

| GDShader                                    | C#                                  |
|---------------------------------------------|-------------------------------------|
| `0.3`                                       | `0.3f`                              |
| `vec2(-0.15, 0.0)`                          | `new Vector2(-0.15f, 0.0f)`         |
| `vec4(1.0, 1.0, 1.0, 1.0)` + `source_color` | `new Color(1.0f, 1.0f, 1.0f, 1.0f)` |
| `false`                                     | `false`                             |
| (none)                                      | `default`                           |

## Enum Generation

`hint_enum` uniforms generate both an enum type and a typed property:

```gdshader
uniform int shape : hint_enum("Circle", "Rectangle", "Rounded Rectangle") = 0;
```

Generates:

```csharp
public enum ShapeMode { Circle, Rectangle, RoundedRectangle }

[Export]
public ShapeMode Shape
{
    get => _shape;
    set
    {
        _shape = value;
        UpdateShaderParam("shape", (int)value);
        NotifyPropertyListChanged();
    }
}
private ShapeMode _shape = ShapeMode.Circle;
```

Display names with spaces or hyphens are converted to valid C# identifiers (`"Ease-In-Out"` → `EaseInOut`).

## Limitations

- **Arrays not supported**: `uniform vec4 colors[8]` will not generate correct code. Array uniforms must be wrapped manually.
- **Multi-hint not supported**: Only the first hint is recognized. `hint_range` combined with other hints may not parse correctly.
- **No class scaffolding**: Generates individual properties only, not the full wrapper class with `using` directives, namespace, `_ValidateProperty()`, or `UpdateShaderParam()`.
- **Initializer conversion is basic**: Complex expressions or nested constructors may require manual correction.
- **`sampler2D` subtypes**: Always generates `Texture2D`. Specific types like `CurveTexture` must be changed manually.

## Files

- `src/main.rs` — Tool implementation
- `Cargo.toml` — Rust project manifest
- `README.md` — This documentation