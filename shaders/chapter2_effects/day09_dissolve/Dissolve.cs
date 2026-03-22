using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter2_effects.day09_dissolve;

[Tool]
public partial class Dissolve : Node
{
    public enum DissolveModeType
    {
        Noise,
        Directional,
        Radial
    }

    [Export]
    public DissolveModeType DissolveMode
    {
        get => _dissolveMode;
        set
        {
            _dissolveMode = value;
            UpdateShaderParam("dissolve_mode", (int)value);
            NotifyPropertyListChanged();
        }
    }

    private DissolveModeType _dissolveMode = DissolveModeType.Noise;

    [Export]
    public Texture2D? NoiseTexture
    {
        get => _noiseTexture;
        set
        {
            _noiseTexture = value;
            if (value != null) UpdateShaderParam("noise_texture", value);
        }
    }

    private Texture2D? _noiseTexture;

    [Export(PropertyHint.Range, "0.0, 6.283")]
    public float Angle
    {
        get => _angle;
        set
        {
            _angle = value;
            UpdateShaderParam("angle", value);
        }
    }

    private float _angle = 0.0f;

    public enum EdgeModeType
    {
        Hard,
        Soft,
        Burn
    }

    [Export]
    public EdgeModeType EdgeMode
    {
        get => _edgeMode;
        set
        {
            _edgeMode = value;
            UpdateShaderParam("edge_mode", (int)value);
            NotifyPropertyListChanged();
        }
    }

    private EdgeModeType _edgeMode = EdgeModeType.Hard;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float Softness
    {
        get => _softness;
        set
        {
            _softness = value;
            UpdateShaderParam("softness", value);
        }
    }

    private float _softness = 0.5f;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    public float BurnWidth
    {
        get => _burnWidth;
        set
        {
            _burnWidth = value;
            UpdateShaderParam("burn_width", value);
        }
    }

    private float _burnWidth = 0.1f;

    [Export(PropertyHint.Range, "0.0, 5.0")]
    public double Duration { get; set; } = 1.0f;

    [ExportToolButton("Dissolve", Icon = "GuiVisibilityHidden")]
    private Callable DissolveButton => Callable.From(PlayDissolve);

    [ExportToolButton("Appear", Icon = "GuiVisibilityVisible")]
    private Callable AppearButton => Callable.From(PlayAppear);

    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsStringName().ToString();
        bool hide = false;

        hide |= name is nameof(NoiseTexture) && _dissolveMode != DissolveModeType.Noise;
        hide |= name is nameof(Angle) && _dissolveMode != DissolveModeType.Directional;

        hide |= name is nameof(Softness) && _edgeMode != EdgeModeType.Soft;
        hide |= name is nameof(BurnWidth) && _edgeMode != EdgeModeType.Burn;

        if (hide)
        {
            property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() & ~PropertyUsageFlags.Editor);
        }
    }

    private Sprite2D? GetSprite2D() => GetNodeOrNull<Sprite2D>("Sprite2D");

    private void UpdateShaderParam(StringName name, Variant value)
    {
        if (GetSprite2D()?.Material is ShaderMaterial material)
        {
            material.SetShaderParameter(name, value);
        }
    }

    private void PlayDissolve()
    {
        if (GetSprite2D()?.Material is ShaderMaterial material)
        {
            var tween = CreateTween();
            tween.TweenProperty(material, "shader_parameter/threshold", 1.0f, Duration)
                .From(0.0f);
        }
    }

    private void PlayAppear()
    {
        if (GetSprite2D()?.Material is ShaderMaterial material)
        {
            var tween = CreateTween();
            tween.TweenProperty(material, "shader_parameter/threshold", 0.0f, Duration)
                .From(1.0f);
        }
    }
}