using Godot;

namespace SpaceExplorer.Scripts.CelestialBodies;

[Tool]
public class Planet : CelestialBody
{
	[Export] float Density = 4;
	[Export] Color Color1 = new Color() { };

	[Export] Color Color2 = new Color();

	[Export] Color OutlineColor1 = new Color();

	[Export] Color OutlineColor2 = new Color();

	[Export] float BorderSize = 100;

	private ColorRect _colorRect = null!;

	public override void _Ready()
	{
		GetNode<Area2D>("GravityField").Gravity = Density * Mathf.Pi * Scale.x * Scale.x * Scale.x * 100f;
		_colorRect = GetNode<ColorRect>("ColorRect");
		SetShaderParams();
	}

	public override void _Process(float delta)
	{
		if (!Engine.EditorHint)
			return;

		SetShaderParams();
	}

	private void SetShaderParams()
	{
		var material = (ShaderMaterial)_colorRect.Material;
		material.SetShaderParam("PIXEL_SCALE", _colorRect.RectSize.x * Scale.x);
		material.SetShaderParam("COLOR_1", Color1);
		material.SetShaderParam("COLOR_2", Color2);
		material.SetShaderParam("OUTLINE_COLOR_1", OutlineColor1);
		material.SetShaderParam("OUTLINE_COLOR_2", OutlineColor2);
		material.SetShaderParam("BORDER_SIZE", BorderSize);
	}
}
