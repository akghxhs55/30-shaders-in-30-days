using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter1_foundation.day04_sdf_intro;

[Tool]
public partial class SdfIntro : Node
{
    public enum ShapeMode { Circle, Rectangle, RoundedRectangle, Ring, None }
    
    #region Shape A
    
    [ExportGroup("Shape A")]
    [Export]
    public ShapeMode ShapeA
    {
        get => _shapeA;
        set
        {
            _shapeA = value;
            UpdateShaderParam("shape_a", (int)value);
            NotifyPropertyListChanged();
        }
    }
    private ShapeMode _shapeA = ShapeMode.Circle;

    [Export]
    public Vector2 PositionA
    {
        get => _positionA;
        set
        {
            _positionA = value;
            UpdateShaderParam("position_a", value);
        }
    }
    private Vector2 _positionA = new(-0.15f, 0.0f);
    
    [Export(PropertyHint.Range, "0.0,0.5,")]
    public float RadiusA
    {
        get => _radiusA;
        set
        {
            _radiusA = value;
            UpdateShaderParam("radius_a", value);
        }
    }
    private float _radiusA = 0.2f;

    [Export]
    public Vector2 SizeA
    {
        get => _sizeA;
        set
        {
            _sizeA = value;
            UpdateShaderParam("size_a", value);
        }
    }
    private Vector2 _sizeA = new(0.3f, 0.2f);
    
    [Export(PropertyHint.Range, "0.0,0.2,")]
    public float CornerRadiusA
    {
        get => _cornerRadiusA;
        set
        {
            _cornerRadiusA = value;
            UpdateShaderParam("corner_radius_a", value);
        }
    }
    private float _cornerRadiusA = 0.05f;
    
    [Export(PropertyHint.Range, "0.0,0.2,")]
    public float RingThicknessA
    {
        get => _ringThicknessA;
        set
        {
            _ringThicknessA = value;
            UpdateShaderParam("ring_thickness_a", value);
        }
    }
    private float _ringThicknessA = 0.05f;
    
    #endregion

    #region Shape B
    
    [ExportGroup("Shape B")]
    [Export]
    public ShapeMode ShapeB
    {
        get => _shapeB;
        set
        {
            _shapeB = value;
            UpdateShaderParam("shape_b", (int)value);
            NotifyPropertyListChanged();
        }
    }
    private ShapeMode _shapeB = ShapeMode.Circle;

    [Export]
    public Vector2 PositionB
    {
        get => _positionB;
        set
        {
            _positionB = value;
            UpdateShaderParam("position_b", value);
        }
    }
    private Vector2 _positionB = new(0.15f, 0.0f);
    
    [Export(PropertyHint.Range, "0.0,0.5,")]
    public float RadiusB
    {
        get => _radiusB;
        set
        {
            _radiusB = value;
            UpdateShaderParam("radius_b", value);
        }
    }
    private float _radiusB = 0.2f;

    [Export]
    public Vector2 SizeB
    {
        get => _sizeB;
        set
        {
            _sizeB = value;
            UpdateShaderParam("size_b", value);
        }
    }
    private Vector2 _sizeB = new(0.3f, 0.2f);
    
    [Export(PropertyHint.Range, "0.0,0.2,")]
    public float CornerRadiusB
    {
        get => _cornerRadiusB;
        set
        {
            _cornerRadiusB = value;
            UpdateShaderParam("corner_radius_b", value);
        }
    }
    private float _cornerRadiusB = 0.05f;
    
    [Export(PropertyHint.Range, "0.0,0.2,")]
    public float RingThicknessB
    {
        get => _ringThicknessB;
        set
        {
            _ringThicknessB = value;
            UpdateShaderParam("ring_thickness_b", value);
        }
    }
    private float _ringThicknessB = 0.05f;

    #endregion
    
    public enum OperationMode { Union, Intersection, Subtraction }

    [ExportGroup("")]
    [Export]
    public OperationMode Operation
    {
        get => _operation;
        set
        {
            _operation = value;
            UpdateShaderParam("operation", (int)value);
        }
    }
    private OperationMode _operation = OperationMode.Union;
    
    public enum VisualizationMode { Solid, Distance, Stepped }
    
    [ExportGroup("")]
    [Export]
    public VisualizationMode Visualization
    {
        get => _visualization;
        set
        {
            _visualization = value;
            UpdateShaderParam("visualization", (int)value);
            NotifyPropertyListChanged();
        }
    }
    private VisualizationMode _visualization = VisualizationMode.Solid;
    
    [Export(PropertyHint.Range, "1.0,32.0,")]
    public float BandFrequency
    {
        get => _bandFrequency;
        set
        {
            _bandFrequency = value;
            UpdateShaderParam("band_frequency", value);
        }
    }
    private float _bandFrequency = 16.0f;
    
    [ExportGroup("Colors")]
    [Export]
    public Color ShapeColor
    {
        get => _shapeColor;
        set
        {
            _shapeColor = value;
            UpdateShaderParam("shape_color", value);
        }
    }
    private Color _shapeColor = Colors.White;
    
    [Export]
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            _backgroundColor = value;
            UpdateShaderParam("background_color", value);
        }
    }
    private Color _backgroundColor = Colors.Black;
    
    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsStringName().ToString();
        bool hide = false;

        hide |= name is nameof(PositionA) && _shapeA is ShapeMode.None;
        hide |= name is nameof(RadiusA) && _shapeA is not (ShapeMode.Circle or ShapeMode.Ring);
        hide |= name is nameof(SizeA) && _shapeA is not (ShapeMode.Rectangle or ShapeMode.RoundedRectangle);
        hide |= name is nameof(CornerRadiusA) && _shapeA is not ShapeMode.RoundedRectangle;
        hide |= name is nameof(RingThicknessA) && _shapeA is not ShapeMode.Ring;
        
        hide |= name is nameof(PositionB) && _shapeB is ShapeMode.None;
        hide |= name is nameof(RadiusB) && _shapeB is not (ShapeMode.Circle or ShapeMode.Ring);
        hide |= name is nameof(SizeB) && _shapeB is not (ShapeMode.Rectangle or ShapeMode.RoundedRectangle);
        hide |= name is nameof(CornerRadiusB) && _shapeB is not ShapeMode.RoundedRectangle;
        hide |= name is nameof(RingThicknessB) && _shapeB is not ShapeMode.Ring;
        
        hide |= name is nameof(BandFrequency) && _visualization != VisualizationMode.Stepped;
        
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