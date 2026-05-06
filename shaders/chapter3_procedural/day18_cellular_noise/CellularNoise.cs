using Godot;

namespace ShadersIn30Days.shaders.chapter3_procedural.day18_cellular_noise;

[Tool]
public partial class CellularNoise : Node
{
	public enum CellModeType { Closest, Second, Border }

	[Export]
	public CellModeType CellMode
	{
		get => _cellMode;
		set
		{
			_cellMode = value;
			UpdateShaderParam("cell_mode", (int)value);
		}
	}
	private CellModeType _cellMode = CellModeType.Closest;

	[Export(PropertyHint.Range, "0.01, 32.0")]
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

	[Export(PropertyHint.Range, "0.0, 7.31")]
	public float Seed
	{
		get => _seed;
		set
		{
			_seed = value;
			UpdateShaderParam("seed", value);
		}
	}
	private float _seed = 0.0f;
	
	[ExportToolButton("Randomize", Icon = "RandomNumberGenerator")]
	private Callable RandomizeSeedButton => Callable.From(RandomizeSeed);

	private void RandomizeSeed()
	{
		Seed = (float)GD.RandRange(0.0, 7.31);
	}
	
	private ColorRect? GetColorRect()
	{
		return GetNodeOrNull<ColorRect>("ColorRect");
	}

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetColorRect()?.Material is ShaderMaterial material) material.SetShaderParameter(name, value);
	}
}