using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter1_foundation.day03_stripe_checkerboard;

[Tool]
public partial class StripeCheckerboard : Node
{
    public enum PatternMode { Stripes, Checkerboard, Grid, Dots }
    [Export]
    public PatternMode Pattern
    {
        get => _pattern;
        set
        {
            _pattern = value;
            UpdateShaderParam("pattern", (int)value);
            NotifyPropertyListChanged();
        }
    }
    private PatternMode _pattern = PatternMode.Stripes;
    
    [Export(PropertyHint.Range, "1.0,32.0,")]
    public float Frequency
    {
        get => _frequency;
        set
        {
            _frequency = value;
            UpdateShaderParam("frequency", value);
        }
    }
    private float _frequency = 8.0f;
    
    public enum DirectionMode { Horizontal, Vertical, Diagonal }
    [Export]
    public DirectionMode Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            UpdateShaderParam("direction", (int)value);
        }
    }
    private DirectionMode _direction = DirectionMode.Horizontal;

    [Export(PropertyHint.Range, "0.0,1.0,")]
    public float DutyCycle
    {
        get => _dutyCycle;
        set
        {
            _dutyCycle = value;
            UpdateShaderParam("duty_cycle", value);
        }
    }
    private float _dutyCycle = 0.5f;
    
    [Export(PropertyHint.Range, "0.0,1.0,")]
    public float LineThickness
    {
        get => _lineThickness;
        set
        {
            _lineThickness = value;
            UpdateShaderParam("line_thickness", value);
        }
    }
    private float _lineThickness = 0.1f;
    
    [Export(PropertyHint.Range, "0.0,0.5,")]
    public float DotRadius
    {
        get => _dotRadius;
        set
        {
            _dotRadius = value;
            UpdateShaderParam("dot_radius", value);
        }
    }
    private float _dotRadius = 0.2f;
    
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

    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsStringName().ToString();
        bool hide = false;
        
        hide |= name is nameof(Direction) or nameof(DutyCycle) && _pattern != PatternMode.Stripes;
        hide |= name is nameof(LineThickness) && _pattern != PatternMode.Grid;
        hide |= name is nameof(DotRadius) && _pattern != PatternMode.Dots;
        
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