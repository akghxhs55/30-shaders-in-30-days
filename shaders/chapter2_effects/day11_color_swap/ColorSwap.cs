using Godot;
using Godot.Collections;

namespace ShadersIn30Days.shaders.chapter2_effects.day11_color_swap;

[Tool]
public partial class ColorSwap : Node
{
	public enum SwapModeType { Direct, Palette }

	[Export]
	public SwapModeType SwapMode
	{
		get => _swapMode;
		set
		{
			_swapMode = value;
			UpdateShaderParam("swap_mode", (int)value);
			NotifyPropertyListChanged();
		}
	}
	private SwapModeType _swapMode = SwapModeType.Direct;

	[Export]
	public ColorPair?[] ColorPairs
	{
		get => _colorPairs;
		set
		{
			foreach (var pair in _colorPairs)
				pair?.Disconnect(Resource.SignalName.Changed, Callable.From(SyncColorPairs));

			_colorPairs = value.Length > 8 ? value[..8] : value;

			foreach (var pair in _colorPairs)
				pair?.Connect(Resource.SignalName.Changed, Callable.From(SyncColorPairs));

			SyncColorPairs();
		}
	}
	private ColorPair?[] _colorPairs = [];

	[Export(PropertyHint.Range, "0.0, 1.0")]
	public float Tolerance
	{
		get => _tolerance;
		set
		{
			_tolerance = value;
			UpdateShaderParam("tolerance", value);
		}
	}
	private float _tolerance = 0.1f;

	[Export]
	public Texture2D? IndexTexture
	{
		get => _indexTexture;
		set
		{
			_indexTexture = value;
			if (value != null) UpdateShaderParam("index_texture", value);
		}
	}
	private Texture2D? _indexTexture;

	[Export]
	public Texture2D? PaletteTexture
	{
		get => _paletteTexture;
		set
		{
			_paletteTexture = value;
			if (value != null) UpdateShaderParam("palette_texture", value);
		}
	}
	private Texture2D? _paletteTexture;
	
	public override void _ValidateProperty(Dictionary property)
	{
		string name = property["name"].AsStringName().ToString();
		bool hide = false;

		hide |= name is nameof(ColorPairs) && _swapMode != SwapModeType.Direct;
		hide |= name is nameof(Tolerance) && _swapMode != SwapModeType.Direct;
		
		hide |= name is nameof(IndexTexture) && _swapMode != SwapModeType.Palette;
		hide |= name is nameof(PaletteTexture) && _swapMode != SwapModeType.Palette;

		if (hide)
		{
			property["usage"] = (int)(property["usage"].As<PropertyUsageFlags>() & ~PropertyUsageFlags.Editor);
		}
	}

	private Sprite2D? GetSprite2D() => GetNodeOrNull<Sprite2D>("Sprite2D");

	private void UpdateShaderParam(StringName name, Variant value)
	{
		if (GetSprite2D()?.Material is ShaderMaterial material)
		{
			material.SetShaderParameter(name, value);
		}
	}

	private void SyncColorPairs()
	{
		int count = _colorPairs.Length;
		var paddedSource = new Color[8];
		var paddedTarget = new Color[8];
		int validCount = 0;
		for (int i = 0; i < count; i++)
		{
			var colorPair = _colorPairs[i];
			if (colorPair == null) continue;
			
			paddedSource[validCount] = colorPair.Source;
			paddedTarget[validCount] = colorPair.Target;
			validCount++;
		}
		UpdateShaderParam("source_colors", new Array<Color>(paddedSource));
		UpdateShaderParam("target_colors", new Array<Color>(paddedTarget));
		UpdateShaderParam("color_count", validCount);
	}
}
