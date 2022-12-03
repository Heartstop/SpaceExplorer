using Godot;
using SpaceExplorer.Scripts.CelestialBodies;

namespace SpaceExplorer.Scripts.Minable;

public class Minable : RigidBody2D
{
	[Export]
	public NodePath? CelestialBody;
	public override void _PhysicsProcess(float delta)
	{
		if(CelestialBody is null)
			return;

		LinearVelocity += delta * Physics.CalculateForce(this, GetNode<CelestialBody>(CelestialBody));
	}
}
