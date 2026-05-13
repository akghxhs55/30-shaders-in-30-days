using Godot;

namespace ShadersIn30Days.shaders.chapter4_lighting_3d.day23_rim;

[Tool]
public partial class Rim : Node
{
	public enum RimTypeMode { ViewOnly, Backlit }

	[Export]
	public RimTypeMode RimType
	{
		get => _rimType;
		set
		{
			_rimType = value;
			UpdateShaderParam("rim_type", (int)value);
		}
	}
	private RimTypeMode _rimType = RimTypeMode.ViewOnly;

	[Export]
	public Color RimColor
	{
		get => _rimColor;
		set
		{
			_rimColor = value;
			UpdateShaderParam("rim_color", value);
		}
	}
	private Color _rimColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

	[Export(PropertyHint.Range, "0.0, 16.0")]
	public float RimPower
	{
		get => _rimPower;
		set
		{
			_rimPower = value;
			UpdateShaderParam("rim_power", value);
		}
	}
	private float _rimPower = 4.0f;

	[Export(PropertyHint.Range, "0.0, 1.0")]
	public float RimIntensity
	{
		get => _rimIntensity;
		set
		{
			_rimIntensity = value;
			UpdateShaderParam("rim_intensity", value);
		}
	}
	private float _rimIntensity = 1.0f;

	public override void _Ready()
	{
		RimType = _rimType;
		RimColor = _rimColor;
		RimPower = _rimPower;
		RimIntensity = _rimIntensity;
	}

	private MeshInstance3D? GetMeshInstance()
	{
		return GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
	}

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetMeshInstance()?.MaterialOverride is ShaderMaterial material) material.SetShaderParameter(name, value);
	}
}