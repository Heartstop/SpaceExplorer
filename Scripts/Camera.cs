using Godot;

public class Camera : Camera2D
{
	public override void _Process(float delta)
	{
		var material = (ShaderMaterial)GetNode<ColorRect>("CanvasLayer/ColorRect").Material;
		material.SetShaderParam("ROTATION", GlobalRotation);
		material.SetShaderParam("ASPECT", GetViewportRect().Size.Aspect());
	}
}
