using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter1_foundation.day01_uv_basics;

[Tool]
public partial class UvBasics : Node
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
    
    [Export(PropertyHint.Range, "1,16,")]
    public int TileRepeats
    {
        get => _tileRepeats;
        set
        {
            _tileRepeats = value;
            UpdateShaderParam("tile_repeats", value);
        }
    }
    private int _tileRepeats = 4;

    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsStringName().ToString();
        bool hide = false;
        
        hide |= name == nameof(TileRepeats) && Mode != UVMode.Tiling;

        if (hide)
        {
            property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() & ~PropertyUsageFlags.Editor);
        }
    }

    private ColorRect? GetColorRect() => GetNodeOrNull<ColorRect>("ColorRect");

    private void UpdateShaderParam(StringName name, Variant value)
    {
        if (GetColorRect()?.Material is ShaderMaterial material)
        {
            material.SetShaderParameter(name, value);
        }
    }
}