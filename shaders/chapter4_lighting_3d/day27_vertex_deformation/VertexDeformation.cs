using Godot;

namespace ShadersIn30Days.shaders.chapter4_lighting_3d.day27_vertex_deformation;

[Tool]
public partial class VertexDeformation : Node
{
	[Export(PropertyHint.Range, "0.0, 1.0")]
	public float Amplitude
	{
		get => _amplitude;
		set
		{
			_amplitude = value;
			UpdateShaderParam("amplitude", value);
		}
	}
	private float _amplitude = 0.2f;

	[Export(PropertyHint.Range, "0.0, 10.0")]
	public float Frequency
	{
		get => _frequency;
		set
		{
			_frequency = value;
			UpdateShaderParam("frequency", value);
		}
	}
	private float _frequency = 1.0f;

	[Export(PropertyHint.Range, "0.0, 2.0")]
	public float ScrollSpeed
	{
		get => _scrollSpeed;
		set
		{
			_scrollSpeed = value;
			UpdateShaderParam("scroll_speed", value);
		}
	}
	private float _scrollSpeed = 0.5f;

	[Export]
	public bool RecalculateNormals
	{
		get => _recalculateNormals;
		set
		{
			_recalculateNormals = value;
			UpdateShaderParam("recalculate_normals", value);
		}
	}
	private bool _recalculateNormals = true;

	public override void _Ready()
	{
		Amplitude = _amplitude;
		Frequency = _frequency;
		ScrollSpeed = _scrollSpeed;
		RecalculateNormals = _recalculateNormals;
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