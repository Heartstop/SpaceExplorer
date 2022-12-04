using Godot;

namespace SpaceExplorer.Scripts.CelestialBodies;

public class Planet : CelestialBody
{
	public override void _Ready()
	{
		GetNode<Area2D>("GravityField").Gravity = 2 * Mathf.Pow(Scale.Length(), 3);
	}
}
