using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter1_foundation.day05_anti_aliasing;

[Tool]
public partial class AntiAliasing : Node
{
    #region Edge Mode
    
    public enum EdgeModeType { Hard, Soft, AutoAA }
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
    
    [Export(PropertyHint.Range, "0.0,0.2,")]
    public float Softness
    {
        get => _softness;
        set
        {
            _softness = value;
            UpdateShaderParam("softness", value);
        }
    }
    private float _softness = 0.05f;
    
    #endregion

    #region Glow
    
    [Export]
    public bool GlowEnabled
    {
        get => _glowEnabled;
        set
        {
            _glowEnabled = value;
            UpdateShaderParam("glow_enabled", value);
            NotifyPropertyListChanged();
        }
    }
    private bool _glowEnabled = false;
    
    [ExportGroup("Glow Parameters")]
    [Export(PropertyHint.Range, "0.0,32.0,")]
    public float GlowFalloff
    {
        get => _glowFalloff;
        set
        {
            _glowFalloff = value;
            UpdateShaderParam("glow_falloff", value);
        }
    }
    private float _glowFalloff = 16.0f;
    
    [Export]
    public Color GlowColor
    {
        get => _glowColor;
        set
        {
            _glowColor = value;
            UpdateShaderParam("glow_color", value);
        }
    }

    private Color _glowColor = new(0.0f, 0.655f, 1.0f);
    
    #endregion

    #region Shadow
    
    [ExportGroup("")]
    [Export]
    public bool ShadowEnabled
    {
        get => _shadowEnabled;
        set
        {
            _shadowEnabled = value;
            UpdateShaderParam("shadow_enabled", value);
            NotifyPropertyListChanged();
        }
    }
    private bool _shadowEnabled = false;
    
    [ExportGroup("Shadow Parameters")]
    [Export]
    public Vector2 ShadowOffset
    {
        get => _shadowOffset;
        set
        {
            _shadowOffset = value;
            UpdateShaderParam("shadow_offset", value);
        }
    }

    private Vector2 _shadowOffset = new(0.02f, 0.02f);
    
    [Export(PropertyHint.Range, "0.0,0.2,")]
    public float ShadowSoftness
    {
        get => _shadowSoftness;
        set
        {
            _shadowSoftness = value;
            UpdateShaderParam("shadow_softness", value);
        }
    }
    private float _shadowSoftness = 0.05f;
    
    [Export]
    public Color ShadowColor
    {
        get => _shadowColor;
        set
        {
            _shadowColor = value;
            UpdateShaderParam("shadow_color", value);
        }
    }
    private Color _shadowColor = new(0.0f, 0.0f, 0.0f, 0.5f);
    
    #endregion
    
    [ExportGroup("")]
    [Export(PropertyHint.Range, "0.0,0.5,")]
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = value;
            UpdateShaderParam("radius", value);
        }
    }
    private float _radius = 0.3f;

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
    private Color _shapeColor = Colors.Black;
    
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
    private Color _backgroundColor = Colors.White;
    
    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsStringName().ToString();
        bool hide = false;

        hide |= name is nameof(Softness) && _edgeMode is not EdgeModeType.Soft;
        
        hide |= name is nameof(GlowFalloff) && !_glowEnabled;
        hide |= name is nameof(GlowColor) && !_glowEnabled;
        
        hide |= name is nameof(ShadowOffset) && !_shadowEnabled;
        hide |= name is nameof(ShadowSoftness) && !_shadowEnabled;
        hide |= name is nameof(ShadowColor) && !_shadowEnabled;
        
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