using Godot;

namespace ShadersIn30Days.shaders.week1_foundation.day01_uv_basics;

[Tool]
public partial class UvVisualization : Node
{
    public enum UVMode
    {
        RgbVisualization,
        HorizontalGradient,
        VerticalGradient,
        CenteredUV,
        Tiling,
        Radial
    }

    [Export]
    public UVMode Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            UpdateShaderParam("mode", (int)value);
        }
    }
    private UVMode _mode = UVMode.RgbVisualization;

    private ColorRect? GetColorRect() => GetNodeOrNull<ColorRect>("ColorRect");

    private void UpdateShaderParam(StringName name, Variant value)
    {
        GD.Print(GetColorRect());
        if (GetColorRect()?.Material is ShaderMaterial material)
        {
            material.SetShaderParameter(name, value);
        }
    }
}