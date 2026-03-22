using Godot;

namespace ShadersIn30Days.shaders.chapter1_foundation.day07_spinner;

[Tool]
public partial class Spinner : Node
{
	public enum ModeType { ArcSpinner, DotsSpinner, PulsingRing }

	[Export]
	public ModeType Mode
	{
		get => _mode;
		set
		{
			_mode = value;
			UpdateShaderParam("mode", (int)value);
		}
	}
	private ModeType _mode = ModeType.ArcSpinner;

	[Export(PropertyHint.Range, "0.0, 5.0")]
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
	
	private ColorRect? GetColorRect() => GetNodeOrNull<ColorRect>("ColorRect");
	
	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetColorRect()?.Material is ShaderMaterial material)
		{
			material.SetShaderParameter(name, value);
		}
	}
}
