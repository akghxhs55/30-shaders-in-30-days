using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter2_effects.day12_pixelation;

[Tool]
public partial class Pixelation : Node
{
	public enum PixelationModeType { Snap, Average, Dot }

	[Export]
	public PixelationModeType PixelationMode
	{
		get => _pixelationMode;
		set
		{
			_pixelationMode = value;
			UpdateShaderParam("pixelation_mode", (int)value);
			NotifyPropertyListChanged();
		}
	}
	private PixelationModeType _pixelationMode = PixelationModeType.Snap;

	[Export(PropertyHint.Range, "1, 512")]
	public int PixelCount
	{
		get => _pixelCount;
		set
		{
			_pixelCount = value;
			UpdateShaderParam("pixel_count", value);
		}
	}
	private int _pixelCount = 64;

	[Export(PropertyHint.Range, "1, 16")]
	public int SampleCount
	{
		get => _sampleCount;
		set
		{
			_sampleCount = value;
			UpdateShaderParam("sample_count", value);
		}
	}
	private int _sampleCount = 4;

	[Export(PropertyHint.Range, "0.0, 1.0")]
	public float DotSize
	{
		get => _dotSize;
		set
		{
			_dotSize = value;
			UpdateShaderParam("dot_size", value);
		}
	}
	private float _dotSize = 0.5f;
	
	public override void _ValidateProperty(Dictionary property)
	{
		string name = property["name"].AsStringName().ToString();
		bool hide = false;

		hide |= name is nameof(SampleCount) && _pixelationMode != PixelationModeType.Average;
		hide |= name is nameof(DotSize) && _pixelationMode != PixelationModeType.Dot;

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