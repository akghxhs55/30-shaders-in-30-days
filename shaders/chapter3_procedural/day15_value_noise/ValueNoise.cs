using Godot;

namespace ShadersIn30Days.shaders.chapter3_procedural.day15_value_noise;

[Tool]
public partial class ValueNoise : Node
{
    public enum InterpolationMode { Linear, Smoothstep, Cubic }

    [Export]
    public InterpolationMode Interpolation
    {
        get => _interpolation;
        set
        {
            _interpolation = value;
            UpdateShaderParam("interpolation", (int)value);
        }
    }
    private InterpolationMode _interpolation = InterpolationMode.Smoothstep;

    [Export(PropertyHint.Range, "1.0, 64.0")]
    public float Frequency
    {
        get => _frequency;
        set
        {
            _frequency = value;
            UpdateShaderParam("frequency", value);
        }
    }
    private float _frequency = 4.0f;

    [Export(PropertyHint.Range, "0.0, 7.31")]
    public float Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            UpdateShaderParam("seed", _seed);
        }
    }
    private float _seed = 0.0f;
    
    [ExportToolButton("Randomize", Icon = "RandomNumberGenerator")]
    private Callable RandomizeSeedButton => Callable.From(RandomizeSeed);

    private ColorRect? GetColorRect()
    {
        return GetNodeOrNull<ColorRect>("ColorRect");
    }

    private void UpdateShaderParam(StringName name, Variant value)
    {
        if (GetColorRect()?.Material is ShaderMaterial material) material.SetShaderParameter(name, value);
    }

    private void RandomizeSeed()
    {
        _seed = (float)GD.RandRange(0.0, 7.31);
        UpdateShaderParam("seed", _seed);
    }
}