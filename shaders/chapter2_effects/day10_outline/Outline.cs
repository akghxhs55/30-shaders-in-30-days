using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter2_effects.day10_outline;

[Tool]
public partial class Outline : Node
{
	public enum OutlineModeType { FourDirectional, EightDirectional, Radial }

	[Export]
	public OutlineModeType OutlineMode
	{
		get => _outlineMode;
		set
		{
			_outlineMode = value;
			UpdateShaderParam("outline_mode", (int)value);
			NotifyPropertyListChanged();
		}
	}
	private OutlineModeType _outlineMode = OutlineModeType.FourDirectional;

	[Export(PropertyHint.Range, "1, 64")]
	public int SampleCount
	{
		get => _sampleCount;
		set
		{
			_sampleCount = value;
			UpdateShaderParam("sample_count", value);
		}
	}
	private int _sampleCount = 16;

	public enum PlacementMode { Outer, Inner, Both }

	[Export]
	public PlacementMode Placement
	{
		get => _placement;
		set
		{
			_placement = value;
			UpdateShaderParam("placement", (int)value);
		}
	}
	private PlacementMode _placement = PlacementMode.Outer;

	[Export(PropertyHint.Range, "0.0, 40.0")]
	public float Thickness
	{
		get => _thickness;
		set
		{
			_thickness = value;
			UpdateShaderParam("thickness", value);
		}
	}
	private float _thickness = 10.0f;

	[Export]
	public Color OutlineColor
	{
		get => _outlineColor;
		set
		{
			_outlineColor = value;
			UpdateShaderParam("outline_color", value);
		}
	}
	private Color _outlineColor = new(1.0f, 1.0f, 1.0f);
	
	public override void _ValidateProperty(Dictionary property)
	{
		string name = property["name"].AsStringName().ToString();
		bool hide = false;

		hide |= name is nameof(SampleCount) && _outlineMode != OutlineModeType.Radial;

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
}