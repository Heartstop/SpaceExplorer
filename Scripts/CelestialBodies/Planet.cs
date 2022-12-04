using Godot;

namespace SpaceExplorer.Scripts.CelestialBodies;

public class Planet : CelestialBody
{
	[Export] float GravityFactor = 4;
	public override void _Ready()
	{
		GetNode<Area2D>("GravityField").Gravity = GravityFactor * Mathf.Pow(Scale.Length(), 3);
	}
}
