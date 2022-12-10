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

	private Vector2[] _pathPoints = { };

	public override void _PhysicsProcess(float delta)
	{
		var gravityFields = GetTree().GetNodesInGroup("gravity_field").Cast<Area2D>().ToImmutableArray();

		_pathPoints = new Vector2[Points];
		_pathPoints[0] = Vector2.Zero;
		var lastGlobalPos = GlobalPosition;
		var lastVelocity = GetParent<RigidBody2D>().LinearVelocity;

		for (var i = 1; i < Points; i++)
		{
			var acceleration = Physics.CalculateGravity(gravityFields, lastGlobalPos);
			var timeStep = Mathf.Min(StepSize / acceleration.Length(), 10f);
			lastVelocity += timeStep * acceleration;
			lastGlobalPos += timeStep * lastVelocity;
			_pathPoints[i] = ToLocal(lastGlobalPos);
		}

		Update();
	}

	public override void _Draw()
	{
		DrawPolyline(
			points: _pathPoints,
			color: Color,
			width: Width,
			antialiased: false);
	}
}
