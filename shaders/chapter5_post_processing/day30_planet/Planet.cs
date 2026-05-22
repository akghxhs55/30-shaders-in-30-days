using Godot;

namespace ShadersIn30Days.shaders.chapter5_post_processing.day30_planet;

[Tool]
public partial class Planet : Node
{
    [Export]
    public Color ColorBottom1
    {
        get => _colorBottom1;
        set
        {
            _colorBottom1 = value;
            UpdateShaderParam("color_bottom1", value);
        }
    }
    private Color _colorBottom1 = new Color(0.008f, 0.4f, 0.702f);

    [Export]
    public Color ColorMiddle1
    {
        get => _colorMiddle1;
        set
        {
            _colorMiddle1 = value;
            UpdateShaderParam("color_middle1", value);
        }
    }
    private Color _colorMiddle1 = new Color(0.537f, 0.702f, 0.929f);

    [Export]
    public Color ColorTop1
    {
        get => _colorTop1;
        set
        {
            _colorTop1 = value;
            UpdateShaderParam("color_top1", value);
        }
    }
    private Color _colorTop1 = new Color(0.875f, 0.906f, 1.0f);

    [Export]
    public Color ColorBottom2
    {
        get => _colorBottom2;
        set
        {
            _colorBottom2 = value;
            UpdateShaderParam("color_bottom2", value);
        }
    }
    private Color _colorBottom2 = new Color(0.0f, 0.157f, 0.31f);

    [Export]
    public Color ColorMiddle2
    {
        get => _colorMiddle2;
        set
        {
            _colorMiddle2 = value;
            UpdateShaderParam("color_middle2", value);
        }
    }
    private Color _colorMiddle2 = new Color(0.02f, 0.133f, 0.341f);

    [Export]
    public Color ColorTop2
    {
        get => _colorTop2;
        set
        {
            _colorTop2 = value;
            UpdateShaderParam("color_top2", value);
        }
    }
    private Color _colorTop2 = new Color(0.11f, 0.208f, 0.38f);

    [Export(PropertyHint.Range, "1.0, 128.0")]
    public float SpecularPower
    {
        get => _specularPower;
        set
        {
            _specularPower = value;
            UpdateShaderParam("specular_power", value);
        }
    }
    private float _specularPower = 32.0f;

    [Export(PropertyHint.Range, "0.0, 2.0")]
    public float SpecularIntensity
    {
        get => _specularIntensity;
        set
        {
            _specularIntensity = value;
            UpdateShaderParam("specular_intensity", value);
        }
    }
    private float _specularIntensity = 0.15f;

    [Export(PropertyHint.Range, "0.1, 8.0")]
    public float RimPower
    {
        get => _rimPower;
        set
        {
            _rimPower = value;
            UpdateShaderParam("rim_power", value);
        }
    }
    private float _rimPower = 6.0f;

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
    private float _rimIntensity = 0.5f;

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
    private Color _rimColor = new Color(0.522f, 0.753f, 0.871f, 1.0f);
    

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float RotationSpeed { get; set; } = 0.1f;

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
