
using Godot;

namespace ShadersIn30Days.shaders.chapter3_procedural.day19_fire;

[Tool]
public partial class Fire : Node
{
	[Export(PropertyHint.Range, "0.0, 10.0")]
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

	[Export(PropertyHint.Range, "1.0, 16.0")]
	public float Frequency
	{
		get => _frequency;
		set
		{
			_frequency = value;
			UpdateShaderParam("frequency", value);
		}
	}
	private float _frequency = 4.0f;

	[Export(PropertyHint.Range, "0.0, 5.0")]
	public float WarpStrength
	{
		get => _warpStrength;
		set
		{
			_warpStrength = value;
			UpdateShaderParam("warp_strength", value);
		}
	}
	private float _warpStrength = 2.0f;

	[Export(PropertyHint.Range, "0.5, 5.0")]
	public float Intensity
	{
		get => _intensity;
		set
		{
			_intensity = value;
			UpdateShaderParam("intensity", value);
		}
	}
	private float _intensity = 2.0f;

	[Export(PropertyHint.Range, "0.0, 1.0")]
	public float Threshold
	{
		get => _threshold;
		set
		{
			_threshold = value;
			UpdateShaderParam("threshold", value);
		}
	}
	private float _threshold = 0.2f;

	[Export]
	public Color ColorHot
	{
		get => _colorHot;
		set
		{
			_colorHot = value;
			UpdateShaderParam("color_hot", value);
		}
	}
	private Color _colorHot = new Color(0.922f, 0.922f, 0.424f, 1.0f);

	[Export]
	public Color ColorMid
	{
		get => _colorMid;
		set
		{
			_colorMid = value;
			UpdateShaderParam("color_mid", value);
		}
	}
	private Color _colorMid = new Color(1.0f, 0.5f, 0.0f, 1.0f);

	[Export]
	public Color ColorCool
	{
		get => _colorCool;
		set
		{
			_colorCool = value;
			UpdateShaderParam("color_cool", value);
		}
	}
	private Color _colorCool = new Color(1.0f, 0.0f, 0.0f, 1.0f);
	
	private ColorRect? GetColorRect()
	{
		return GetNodeOrNull<ColorRect>("ColorRect");
	}

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetColorRect()?.Material is ShaderMaterial material) material.SetShaderParameter(name, value);
	}
}