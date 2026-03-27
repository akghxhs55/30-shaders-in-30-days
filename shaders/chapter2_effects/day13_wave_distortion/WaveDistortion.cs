using Godot;

namespace ShadersIn30Days.shaders.chapter2_effects.day13_wave_distortion;

[Tool]
public partial class WaveDistortion : Node
{
	public enum DirectionMode { Horizontal, Vertical, Both }

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

	[Export(PropertyHint.Range, "0.1, 20.0")]
	public float Frequency
	{
		get => _frequency;
		set
		{
			_frequency = value;
			UpdateShaderParam("frequency", value);
		}
	}
	private float _frequency = 3.0f;

	[Export(PropertyHint.Range, "0.0, 100.0")]
	public float Amplitude
	{
		get => _amplitude;
		set
		{
			_amplitude = value;
			UpdateShaderParam("amplitude", value);
		}
	}
	private float _amplitude = 10.0f;

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
	
	private Sprite2D? GetSprite2D() => GetNodeOrNull<Sprite2D>("Sprite2D");

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			material.SetShaderParameter(name, value);
		}
	}
}