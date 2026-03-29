using Godot;

namespace ShadersIn30Days.shaders.chapter2_effects.day14_character_effects;

[Tool]
public partial class CharacterEffects : Node
{
	public enum CharacterCondition { None, Frozen, Burned }

	[Export]
	public CharacterCondition Condition
	{
		get => _condition;
		set
		{
			RevertCondition(_condition);
			_condition = value;
			ApplyCondition(_condition);
		}
	}
	private CharacterCondition _condition = CharacterCondition.None;
	
	public enum CharacterState { Idle, Died };
	public CharacterState State { get; private set; } = CharacterState.Idle;

	[ExportToolButton("Die", Icon = "Skeleton2D")] 
	private Callable DieButton => Callable.From(TriggerDie);

	[ExportToolButton("Resurrect", Icon = "Heart")]
	private Callable ResurrectButton => Callable.From(TriggerResurrect);

	[ExportToolButton("Hit", Icon = "Generic6DOFJoint3D")]
	private Callable HitButton => Callable.From(TriggerHit);
	
	private Sprite2D? GetSprite2D() => GetNodeOrNull<Sprite2D>("Sprite2D");

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			material.SetShaderParameter(name, value);
		}
	}

	private void ApplyCondition(CharacterCondition condition)
	{
		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			switch (condition)
			{
				case CharacterCondition.Frozen: 
					var frozenTween = CreateTween();
					frozenTween.TweenProperty(material, "shader_parameter/frozen_progress", 1.0f, 0.5f);
					break;
				
				case CharacterCondition.Burned:
					var burnedTween = CreateTween();
					burnedTween.TweenProperty(material, "shader_parameter/burned_progress", 1.0f, 0.5f);
					break;
			}
		}
	}
	
	private void RevertCondition(CharacterCondition condition)
	{
		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			switch (condition)
			{
				case CharacterCondition.Frozen:
					var frozenTween = CreateTween();
					frozenTween.TweenProperty(material, "shader_parameter/frozen_progress", 0.0f, 0.5f);
					break;
				
				case CharacterCondition.Burned:
					var burnedTween = CreateTween();
					burnedTween.TweenProperty(material, "shader_parameter/burned_progress", 0.0f, 0.5f);
					break;
			}
		}
	}
	
	private void TriggerDie()
	{
		if (State == CharacterState.Died) return;
		State = CharacterState.Died;

		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			var tween = CreateTween().SetParallel();
			tween.TweenProperty(material, "shader_parameter/threshold", 1.0f, 1.0f)
				.From(0.0f);
			tween.TweenProperty(material, "shader_parameter/pixelation_size", 64.0f, 1.0f)
				.From(1.0f);
		}
	}

	private void TriggerResurrect()
	{
		if (State == CharacterState.Idle) return;
		State = CharacterState.Idle;

		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			var tween = CreateTween().SetParallel();
			tween.TweenProperty(material, "shader_parameter/threshold", 0.0f, 1.0f)
				.From(1.0f);
			tween.TweenProperty(material, "shader_parameter/pixelation_size", 1.0f, 1.0f)
				.From(64.0f);

			var flashTween = CreateTween();
			flashTween.TweenProperty(material, "shader_parameter/resurrect_flash_amount", 1.0f, 0.8f)
				.From(0.0f);
			flashTween.TweenProperty(material, "shader_parameter/resurrect_flash_amount", 0.0f, 0.3f);
		}
	}

	private void TriggerHit()
	{
		var sprite = GetSprite2D();
		if (sprite?.Material is ShaderMaterial material)
		{
			var tween = CreateTween();
			tween.TweenProperty(material, "shader_parameter/hit_flash_amount", 1.0f, 0.05f);
			tween.TweenProperty(material, "shader_parameter/hit_flash_amount", 0.0f, 0.15f);

			var scaleTween = CreateTween();
			scaleTween.TweenProperty(sprite, "scale", Vector2.One * 1.2f, 0.05f);
			scaleTween.TweenProperty(sprite, "scale", Vector2.One, 0.15f);
		}
	}
}