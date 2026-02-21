using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter1_foundation.day06_animation;

[Tool]
public partial class Animation : Node
{
    public enum AnimationModeType
    {
        Scrolling,
        PulsingCircle,
        Rotation,
        ColorCycle,
        PingPong
    }

    [Export]
    public AnimationModeType AnimationMode
    {
        get => _animationMode;
        set
        {
            _animationMode = value;
            UpdateShaderParam("animation_mode", (int)value);
            NotifyPropertyListChanged();
        }
    }
    private AnimationModeType _animationMode = AnimationModeType.Scrolling;
    
    [Export(PropertyHint.Range, "0.0,5.0,")]
    public float Speed
    {
        get => _speed;
        set
        {
            _speed = value;
            UpdateShaderParam("speed", value);
        }
    }
    private float _speed = 1.0f;
    
    public enum TransitionMode {
        Linear,
        Quadratic,
        Cubic,
        Sine,
        Back
    }
    [Export]
    public TransitionMode Transition
    {
        get => _transition;
        set
        {
            _transition = value;
            UpdateShaderParam("transition_mode", (int)value);
        }
    }
    private TransitionMode _transition = TransitionMode.Linear;
    
    public enum EasingMode {
        EaseIn,
        EaseOut,
        EaseInOut,
        EaseOutIn
    }

    [Export]
    public EasingMode Easing
    {
        get => _easing;
        set
        {
            _easing = value;
            UpdateShaderParam("easing_mode", (int)value);
        }
    }
    private EasingMode _easing = EasingMode.EaseIn;

    public override void _ValidateProperty(Dictionary property)
    {
        string name = property["name"].AsStringName().ToString();
        bool hide = false;
        
        hide |= name is nameof(Transition) && _animationMode != AnimationModeType.PingPong;
        hide |= name is nameof(Easing) && _animationMode != AnimationModeType.PingPong;
        
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