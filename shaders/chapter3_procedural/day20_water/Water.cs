using Godot;

namespace ShadersIn30Days.shaders.chapter3_procedural.day20_water;

[Tool]
public partial class Water : Node
{
	[Export(PropertyHint.Range, "0.0, 10.0")]
	public float WaveSpeed
	{
		get => _waveSpeed;
		set
		{
			_waveSpeed = value;
			UpdateShaderParam("wave_speed", value);
		}
	}
	private float _waveSpeed = 1.5f;

	[Export(PropertyHint.Range, "0.5, 32.0")]
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

	[Export]
	public Color ColorDeep
	{
		get => _colorDeep;
		set
		{
			_colorDeep = value;
			UpdateShaderParam("color_deep", value);
		}
	}
	private Color _colorDeep = new Color(0.118f, 0.545f, 0.651f);

	[Export]
	public Color ColorCaustic
	{
		get => _colorCaustic;
		set
		{
			_colorCaustic = value;
			UpdateShaderParam("color_caustic", value);
		}
	}
	private Color _colorCaustic = new Color(0.357f, 0.769f, 0.871f);

	[Export(PropertyHint.Range, "0.0, 5.0")]
	public float CausticBrightness
	{
		get => _causticBrightness;
		set
		{
			_causticBrightness = value;
			UpdateShaderParam("caustic_brightness", value);
		}
	}
	private float _causticBrightness = 0.4f;

	private ColorRect? GetColorRect()
	{
		return GetNodeOrNull<ColorRect>("ColorRect");
	}

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetColorRect()?.Material is ShaderMaterial material) material.SetShaderParameter(name, value);
	}
}