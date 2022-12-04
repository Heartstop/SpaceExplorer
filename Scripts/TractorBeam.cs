using System.Linq;
using Godot;
using SpaceExplorer.Scripts.Minable;

public class TractorBeam : Area2D
{
	public float PullForce => 50f;
	public override void _PhysicsProcess(float delta)
	{
		foreach (var minable in GetOverlappingBodies().OfType<Minable>())
		{
			minable.LinearVelocity += delta * PullForce * minable.GlobalPosition.DirectionTo(GlobalPosition);
		}
	}
}
