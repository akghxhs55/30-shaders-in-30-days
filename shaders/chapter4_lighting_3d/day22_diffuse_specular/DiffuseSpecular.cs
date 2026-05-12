using Godot;

namespace ShadersIn30Days.shaders.chapter4_lighting_3d.day22_diffuse_specular;

[Tool]
public partial class DiffuseSpecular : Node
{
	public enum ShadingModeMode { Phong, BlinnPhong }

	[Export]
	public ShadingModeMode ShadingMode
	{
		get => _shadingMode;
		set
		{
			_shadingMode = value;
			UpdateShaderParam("shading_mode", (int)value);
		}
	}
	private ShadingModeMode _shadingMode = ShadingModeMode.Phong;

	[Export(PropertyHint.Range, "0.0, 1.0")]
	public float DiffuseIntensity
	{
		get => _diffuseIntensity;
		set
		{
			_diffuseIntensity = value;
			UpdateShaderParam("diffuse_intensity", value);
		}
	}
	private float _diffuseIntensity = 1.0f;

	[Export(PropertyHint.Range, "0.0, 1.0")]
	public float SpecularIntensity
	{
		get => _specularIntensity;
		set
		{
			_specularIntensity = value;
			UpdateShaderParam("specular_intensity", value);
		}
	}
	private float _specularIntensity = 1.0f;

	[Export(PropertyHint.Range, "0.0, 128.0")]
	public float SpecularPower
	{
		get => _specularPower;
		set
		{
			_specularPower = value;
			UpdateShaderParam("specular_power", value);
		}
	}
	private float _specularPower = 32.0f;

	[Export]
	public Color Color
	{
		get => _color;
		set
		{
			_color = value;
			UpdateShaderParam("color", value);
		}
	}
	private Color _color = new Color(0.063f, 0.408f, 0.89f);

	public override void _Ready()
	{
		ShadingMode = _shadingMode;
		DiffuseIntensity = _diffuseIntensity;
		SpecularIntensity = _specularIntensity;
		SpecularPower = _specularPower;
		Color = _color;
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
