using Godot;

namespace ShadersIn30Days.shaders.chapter4_lighting_3d.day28_crystal;

[Tool]
public partial class Crystal : Node
{
    [Export]
    public Color CrystalColor
    {
        get => _crystalColor;
        set
        {
            _crystalColor = value;
            UpdateShaderParam("crystal_color", value);
        }
    }

    private Color _crystalColor = new Color(0.282f, 0.51f, 0.718f);

    [Export(PropertyHint.Range, "0.1, 10.0")]
    public float FresnelPower
    {
        get => _fresnelPower;
        set
        {
            _fresnelPower = value;
            UpdateShaderParam("fresnel_power", value);
        }
    }

    private float _fresnelPower = 5.0f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float BaseAlpha
    {
        get => _baseAlpha;
        set
        {
            _baseAlpha = value;
            UpdateShaderParam("base_alpha", value);
        }
    }

    private float _baseAlpha = 0.4f;
    
    [Export(PropertyHint.Range, "0.0, 0.1")]
    public float RefractionStrength
    {
        get => _refractionStrength;
        set
        {
            _refractionStrength = value;
            UpdateShaderParam("refraction_strength", value);
        }
    }

    private float _refractionStrength = 0.02f;

    [Export]
    public Color RimColor
    {
        get => _rimColor;
        set
        {
            _rimColor = value;
            UpdateShaderParam("rim_color", value);
        }
    }

    private Color _rimColor = new Color(0.212f, 0.408f, 0.506f);

    [Export(PropertyHint.Range, "0.1, 10.0")]
    public float RimPower
    {
        get => _rimPower;
        set
        {
            _rimPower = value;
            UpdateShaderParam("rim_power", value);
        }
    }

    private float _rimPower = 3.0f;

    [Export(PropertyHint.Range, "0.0, 2.0")]
    public float RimIntensity
    {
        get => _rimIntensity;
        set
        {
            _rimIntensity = value;
            UpdateShaderParam("rim_intensity", value);
        }
    }

    private float _rimIntensity = 1.0f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float IridescenceIntensity
    {
        get => _iridescenceIntensity;
        set
        {
            _iridescenceIntensity = value;
            UpdateShaderParam("iridescence_intensity", value);
        }
    }

    private float _iridescenceIntensity = 0.1f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float SpecularStrength
    {
        get => _specularStrength;
        set
        {
            _specularStrength = value;
            UpdateShaderParam("specular_strength", value);
        }
    }

    private float _specularStrength = 1.0f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Roughness
    {
        get => _roughness;
        set
        {
            _roughness = value;
            UpdateShaderParam("roughness", value);
        }
    }

    private float _roughness = 0.05f;

    [Export]
    public Color InnerColor
    {
        get => _innerColor;
        set
        {
            _innerColor = value;
            UpdateShaderParam("inner_color", value);
        }
    }

    private Color _innerColor = new Color(0.18f, 0.553f, 0.902f);

    [Export] public float RotationSpeed { get; set; } = 1.0f;

    public override void _Ready()
    {
        CrystalColor = _crystalColor;
        FresnelPower = _fresnelPower;
        BaseAlpha = _baseAlpha;
        RefractionStrength = _refractionStrength;
        RimColor = _rimColor;
        RimPower = _rimPower;
        RimIntensity = _rimIntensity;
        IridescenceIntensity = _iridescenceIntensity;
        SpecularStrength = _specularStrength;
        Roughness = _roughness;
        InnerColor = _innerColor;
    }

    public override void _Process(double delta)
    {
        if (GetMeshInstance() is { } meshInstance)
        {
            meshInstance.GlobalRotate(Vector3.Up, RotationSpeed * (float)delta);
        }
    }

    private MeshInstance3D? GetMeshInstance()
    {
        return GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
    }
    
    private void UpdateShaderParam(StringName name, Variant value)
    {
        if (GetMeshInstance()?.MaterialOverride is ShaderMaterial material) material.SetShaderParameter(name, value);
    }
}