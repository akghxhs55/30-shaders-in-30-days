using Godot;

namespace ShadersIn30Days.shaders.chapter2_effects.day08_sprite_flash;

[Tool]
public partial class SpriteFlash : Node
{
	[Export]
	public Color FlashColor
	{
		get => _flashColor;
		set
		{
			_flashColor = value;
			UpdateShaderParam("flash_color", value);
		}
	}
	private Color _flashColor = new(1.0f, 1.0f, 1.0f, 1.0f);
	
	[Export]
	private float FlashDuration { get; set; } = 0.2f;

	[Export(PropertyHint.Range, "0.0,1.0")] 
	private float FlashRatio { get; set; } = 0.25f;

	[ExportToolButton("Flash", Icon = "DirectionalLight2D")]
	private Callable FlashButton => Callable.From(TriggerFlash);

	private Sprite2D? GetSprite2D() => GetNodeOrNull<Sprite2D>("Sprite2D");

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			material.SetShaderParameter(name, value);
		}
	}

	private void TriggerFlash()
	{
		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			float attack = FlashDuration * FlashRatio;
			float release = FlashDuration * FlashRatio;
			
			var tween = CreateTween();
			tween.TweenProperty(material, "shader_parameter/flash_amount", 1.0, attack);
			tween.TweenProperty(material, "shader_parameter/flash_amount", 0.0, release);
		}
		
	}
}
