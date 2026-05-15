using Godot;

namespace ShadersIn30Days.shaders.chapter4_lighting_3d.day24_normal_mapping;

[Tool]
public partial class NormalMapping : Node
{
	[Export]
	public bool UseAlbedoMap
	{
		get => _useAlbedoMap;
		set
		{
			_useAlbedoMap = value;
			UpdateShaderParam("use_albedo_map", value);
		}
	}
	private bool _useAlbedoMap = true;

	[Export]
	public bool UseNormalMap
	{
		get => _useNormalMap;
		set
		{
			_useNormalMap = value;
			UpdateShaderParam("use_normal_map", value);
		}
	}
	private bool _useNormalMap = true;
	
	public override void _Ready()
	{
		UseAlbedoMap = _useAlbedoMap;
		UseNormalMap = _useNormalMap;
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