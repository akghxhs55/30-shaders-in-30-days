using Godot;

namespace ShadersIn30Days.shaders.chapter3_procedural.day17_fbm;

[Tool]
public partial class Fbm : Node
{
    public enum NoiseTypeMode { Value, Perlin, Simplex }

    [Export]
    public NoiseTypeMode NoiseType
    {
        get => _noiseType;
        set
        {
            _noiseType = value;
            UpdateShaderParam("noise_type", (int)value);
        }
    }
    private NoiseTypeMode _noiseType = NoiseTypeMode.Perlin;

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
            UpdateShaderParam("seed", value);
        }
    }
    private float _seed = 0.0f;

    [ExportToolButton("Randomize", Icon = "RandomNumberGenerator")]
    private Callable RandomizeSeedButton => Callable.From(RandomizeSeed);

    private void RandomizeSeed()
    {
        Seed = (float)GD.RandRange(0.0, 7.31);
    }

    [Export(PropertyHint.Range, "1, 8")]
    public int Octaves
    {
        get => _octaves;
        set
        {
            _octaves = value;
            UpdateShaderParam("octaves", value);
        }
    }
    private int _octaves = 4;
    
    [Export(PropertyHint.Range, "1.0, 4.0")]
    public float Lacunarity
    {
        get => _lacunarity;
        set
        {
            _lacunarity = value;
            UpdateShaderParam("lacunarity", value);
        }
    }
    private float _lacunarity = 2.0f;
    
    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Gain
    {
        get => _gain;
        set
        {
            _gain = value;
            UpdateShaderParam("gain", value);
        }
    }
    private float _gain = 0.5f;

    private ColorRect? GetColorRect()
    {
        return GetNodeOrNull<ColorRect>("ColorRect");
    }

    private void UpdateShaderParam(StringName name, Variant value)
    {
        if (GetColorRect()?.Material is ShaderMaterial material) material.SetShaderParameter(name, value);
    }
}