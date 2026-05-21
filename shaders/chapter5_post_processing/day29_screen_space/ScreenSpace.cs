using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter5_post_processing.day29_screen_space;

[Tool]
public partial class ScreenSpace : Node
{
    [Export]
    public bool UseBarrelDistortion
    {
        get => _useBarrelDistortion;
        set
        {
            _useBarrelDistortion = value;
            UpdateShaderParam("use_barrel_distortion", value);
            NotifyPropertyListChanged();
        }
    }
    private bool _useBarrelDistortion = false;

    [Export(PropertyHint.Range, "-0.2, 1.0")]
    public float BarrelStrength
    {
        get => _barrelStrength;
        set
        {
            _barrelStrength = value;
            UpdateShaderParam("barrel_strength", value);
        }
    }
    private float _barrelStrength = 0.1f;

    [Export]
    public bool UseChromaticAberration
    {
        get => _useChromaticAberration;
        set
        {
            _useChromaticAberration = value;
            UpdateShaderParam("use_chromatic_aberration", value);
            NotifyPropertyListChanged();
        }
    }
    private bool _useChromaticAberration = false;

    [Export(PropertyHint.Range, "0.0, 0.1")]
    public float ChromaticAberrationStrength
    {
        get => _chromaticAberrationStrength;
        set
        {
            _chromaticAberrationStrength = value;
            UpdateShaderParam("chromatic_aberration_strength", value);
        }
    }
    private float _chromaticAberrationStrength = 0.005f;

    [Export]
    public bool UseVignette
    {
        get => _useVignette;
        set
        {
            _useVignette = value;
            UpdateShaderParam("use_vignette", value);
            NotifyPropertyListChanged();
        }
    }
    private bool _useVignette = false;

    [Export(PropertyHint.Range, "0.0, 2.0")]
    public float VignetteStrength
    {
        get => _vignetteStrength;
        set
        {
            _vignetteStrength = value;
            UpdateShaderParam("vignette_strength", value);
        }
    }
    private float _vignetteStrength = 1.0f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float VignetteRadius
    {
        get => _vignetteRadius;
        set
        {
            _vignetteRadius = value;
            UpdateShaderParam("vignette_radius", value);
        }
    }
    private float _vignetteRadius = 0.5f;

    [Export]
    public bool UseFilmGrain
    {
        get => _useFilmGrain;
        set
        {
            _useFilmGrain = value;
            UpdateShaderParam("use_film_grain", value);
            NotifyPropertyListChanged();
        }
    }
    private bool _useFilmGrain = false;

    [Export(PropertyHint.Range, "0.0, 0.1")]
    public float GrainStrength
    {
        get => _grainStrength;
        set
        {
            _grainStrength = value;
            UpdateShaderParam("grain_strength", value);
        }
    }
    private float _grainStrength = 0.03f;

    public override void _Ready()
    {
        UseBarrelDistortion = _useBarrelDistortion;
        BarrelStrength = _barrelStrength;
        UseChromaticAberration = _useChromaticAberration;
        ChromaticAberrationStrength = _chromaticAberrationStrength;
        UseVignette = _useVignette;
        VignetteStrength = _vignetteStrength;
        VignetteRadius = _vignetteRadius;
        UseFilmGrain = _useFilmGrain;
        GrainStrength = _grainStrength;       
    }

    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsStringName().ToString();
        bool hide = false;
        
        hide |= name is nameof(BarrelStrength) && !_useBarrelDistortion;
        hide |= name is nameof(ChromaticAberrationStrength) && !_useChromaticAberration;
        hide |= name is nameof(VignetteStrength) && !_useVignette;
        hide |= name is nameof(VignetteRadius) && !_useVignette;
        hide |= name is nameof(GrainStrength) && !_useFilmGrain;
        
        if (hide)
        {
            property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() & ~PropertyUsageFlags.Editor);
        }
    }

    private ColorRect? GetColorRect()
    {
        return GetNodeOrNull<ColorRect>("CanvasLayer/ColorRect");
    }
    
    private void UpdateShaderParam(StringName name, Variant value)
    {
        if (GetColorRect()?.Material is ShaderMaterial material) material.SetShaderParameter(name, value);
    }
}