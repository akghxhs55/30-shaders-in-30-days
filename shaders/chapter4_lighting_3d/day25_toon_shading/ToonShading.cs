using Godot;

namespace ShadersIn30Days.shaders.chapter4_lighting_3d.day25_toon_shading;

[Tool]
public partial class ToonShading : Node
{
	[Export(PropertyHint.Range, "1, 10")]
	public int DiffuseSteps
	{
		get => _diffuseSteps;
		set
		{
			_diffuseSteps = value;
			UpdateShaderParamFirst("diffuse_steps", value);
		}
	}
	private int _diffuseSteps = 4;

	[Export(PropertyHint.Range, "0.0, 1.0")]
	public float SpecularThreshold
	{
		get => _specularThreshold;
		set
		{
			_specularThreshold = value;
			UpdateShaderParamFirst("specular_threshold", value);
		}
	}
	private float _specularThreshold = 0.5f;
	
	[Export(PropertyHint.Range, "0.0, 0.3")]
	public float OutlineThickness
	{
		get => _outlineThickness;
		set
		{
			_outlineThickness = value;
			UpdateShaderParamSecond("outline_thickness", value);
		}
	}
	private float _outlineThickness = 0.03f;

	[Export]
	public Color OutlineColor
	{
		get => _outlineColor;
		set
		{
			_outlineColor = value;
			UpdateShaderParamSecond("outline_color", value);
		}
	}
	private Color _outlineColor = Colors.Black;
	
	public override void _Ready()
	{
		DiffuseSteps = _diffuseSteps;
		SpecularThreshold = _specularThreshold;
		OutlineThickness = _outlineThickness;
		OutlineColor = _outlineColor;
	}

	private MeshInstance3D? GetMeshInstance()
	{
		return GetNodeOrNull<MeshInstance3D>("MeshInstance3D");
	}

	private void UpdateShaderParamFirst(StringName name, Variant value)
	{
		if (GetMeshInstance()?.MaterialOverride is ShaderMaterial material) material.SetShaderParameter(name, value);
	}
	
	private void UpdateShaderParamSecond(StringName name, Variant value)
	{
		if (GetMeshInstance()?.MaterialOverride.NextPass is ShaderMaterial material) material.SetShaderParameter(name, value);
	}
}