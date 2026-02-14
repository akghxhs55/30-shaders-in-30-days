using System.Linq;
using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter1_foundation.day02_gradient;

[Tool]
public partial class Gradient : Node
{
    #region Shape
    
    public enum ShapeMode { Linear, Radial, Angular }
    [ExportGroup("")]
    [Export]
    public ShapeMode Shape
    {
        get => _shape;
        set
        {
            _shape = value;
            UpdateShaderParam("shape", (int)value);
        }
    }
    private ShapeMode _shape = ShapeMode.Linear;
    
    #endregion
    
    #region Interpolation
    
    public enum InterpolationMode { Raw, Smoothstep, Step, Custom }
    [ExportGroup("")]
    [Export]
    public InterpolationMode Interpolation
    {
        get => _interpolation;
        set
        {
            _interpolation = value;
            UpdateShaderParam("interpolation", (int)value);
            NotifyPropertyListChanged();
        }
    }
    private InterpolationMode _interpolation = InterpolationMode.Raw;
    
    [ExportGroup("Interpolation Parameters")]
    [Export(PropertyHint.Range, "0.0,1.0,")]
    public float Edge0
    {
        get => _edge0;
        set
        {
            _edge0 = value;
            UpdateShaderParam("edge0", value);
        }
    }
    private float _edge0 = 0.3f;
    
    [Export(PropertyHint.Range, "0.0,1.0,")]
    public float Edge1
    {
        get => _edge1;
        set
        {
            _edge1 = value;
            UpdateShaderParam("edge1", value);
        }
    }
    private float _edge1 = 0.7f;
    
    [Export(PropertyHint.Range, "0.0,1.0,")]
    public float Threshold
    {
        get => _threshold;
        set
        {
            _threshold = value;
            UpdateShaderParam("threshold", value);
        }
    }
    private float _threshold = 0.5f;
    
    [Export]
    public CurveTexture? CustomCurve
    {
        get => _customCurve;
        set
        {
            _customCurve = value;
            if (value?.Curve is Curve curve)
            {
                curve.MinValue = 0.0f;
                curve.MaxValue = 1.0f;
            }

            if (value != null) UpdateShaderParam("custom_curve", value);
        }
    }
    private CurveTexture? _customCurve;
    
    #endregion
    
    #region Color Mapping
    
    public enum ColorMapMode { TwoColor, MultiStop }
    [ExportGroup("")]
    [Export]
    public ColorMapMode ColorMap
    {
        get => _colorMap;
        set
        {
            _colorMap = value;
            UpdateShaderParam("color_mapping", (int)value);
            NotifyPropertyListChanged();
        }
    }
    private ColorMapMode _colorMap = ColorMapMode.TwoColor;

    [ExportGroup("Color Mapping Parameters")]
    [Export]
    public Color ColorA
    {
        get => _colorA;
        set
        {
            _colorA = value;
            UpdateShaderParam("color_a", value);
        }
    }
    private Color _colorA = Colors.Black;

    [Export]
    public Color ColorB
    {
        get => _colorB;
        set
        {
            _colorB = value;
            UpdateShaderParam("color_b", value);
        }
    }
    private Color _colorB = Colors.White;

    [Export]
    public Color[] StopColors
    {
        get => _stopColors;
        set
        {
            if (value.Length < 2)
            {
                while (value.Length < 2)
                {
                    value = value.Append(Colors.White).ToArray();
                }
            }
            if (value.Length > 8)
            {
                value = value[..8];
            }
            _stopColors = value;
            UpdateShaderParam("color_count", _stopColors.Length);
            UpdateShaderParam("stop_colors", _stopColors);
        }
    }
    private Color[] _stopColors = [Colors.Red, Colors.Green, Colors.Blue];
    
    #endregion

    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsStringName().ToString();
        bool hide = false;
        
        // Interpolation parameters
        hide |= name is nameof(Edge0) or nameof(Edge1) && _interpolation != InterpolationMode.Smoothstep;
        hide |= name is nameof(Threshold) && _interpolation != InterpolationMode.Step;
        hide |= name is nameof(CustomCurve) && _interpolation != InterpolationMode.Custom;
        
        // Color mapping parameters
        hide |= name is nameof(ColorA) or nameof(ColorB) && _colorMap != ColorMapMode.TwoColor;
        hide |= name is nameof(StopColors) && _colorMap != ColorMapMode.MultiStop;

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