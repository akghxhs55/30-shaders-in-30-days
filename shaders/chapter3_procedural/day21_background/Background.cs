using Godot;

namespace ShadersIn30Days.shaders.chapter3_procedural.day21_background;

[Tool]
public partial class Background : Node
{
	[Export]
	public Color NebulaColor1
	{
		get => _nebulaColor1;
		set
		{
			_nebulaColor1 = value;
			UpdateShaderParam("nebula_color1", value);
		}
	}
	private Color _nebulaColor1 = new Color(0.5f, 0.2f, 0.8f);

	[Export]
	public Color NebulaColor2
	{
		get => _nebulaColor2;
		set
		{
			_nebulaColor2 = value;
			UpdateShaderParam("nebula_color2", value);
		}
	}
	private Color _nebulaColor2 = new Color(0.145f, 0.604f, 0.588f);

	[Export]
	public Color NebulaColor3
	{
		get => _nebulaColor3;
		set
		{
			_nebulaColor3 = value;
			UpdateShaderParam("nebula_color3", value);
		}
	}
	private Color _nebulaColor3 = new Color(1.0f, 0.3f, 0.2f);

	[Export(PropertyHint.Range, "0.0, 2.0")]
	public float NebulaBrightness
	{
		get => _nebulaBrightness;
		set
		{
			_nebulaBrightness = value;
			UpdateShaderParam("nebula_brightness", value);
		}
	}
	private float _nebulaBrightness = 1.3f;

	[Export(PropertyHint.Range, "0.0, 5.0")]
	public float StarOcclusion
	{
		get => _starOcclusion;
		set
		{
			_starOcclusion = value;
			UpdateShaderParam("star_occlusion", value);
		}
	}
	private float _starOcclusion = 2.0f;
	
	private ColorRect? GetColorRect()
	{
		return GetNodeOrNull<ColorRect>("ColorRect");
	}

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetColorRect()?.Material is ShaderMaterial material) material.SetShaderParameter(name, value);
	}
}