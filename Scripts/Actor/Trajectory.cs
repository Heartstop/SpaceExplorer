using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

namespace SpaceExplorer.Scripts.Actor;

public class Trajectory : Node2D
{
	[Export]
	public int Points = 10;

	[Export]
	public Color Color = Colors.Gold;

	[Export]
	public float Width = 5f;

	[Export]
	public float StepSize = 10;

	private Vector2[] _pathPoints = {};

	public override void _PhysicsProcess(float delta)
	{
		var gravityFields = GetTree().GetNodesInGroup("gravity_field").Cast<Area2D>().ToImmutableArray();
		var points = new List<(Vector2 GlobalPos, Vector2 Velocity)>(Points){(GlobalPos: GlobalPosition, Velocity: GetParent<RigidBody2D>().LinearVelocity)};
		for (var i = 0; i < Points; i++)
		{
			var lastNode = points.Last();
			var force = Physics.CalculateGravity(gravityFields, lastNode.GlobalPos);

			var timeStep = Mathf.Min(StepSize / force.Length(), 1f);
			points.Add((
				GlobalPos: timeStep * lastNode.Velocity + lastNode.GlobalPos,
				Velocity: timeStep * force + lastNode.Velocity));
		}

		_pathPoints = points.Select(p => ToLocal(p.GlobalPos)).ToArray();
		Update();
	}

	public override void _Draw()
	{
		DrawPolyline(_pathPoints, Color, Width, true);
	}
}
