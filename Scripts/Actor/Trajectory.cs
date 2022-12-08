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
	public float TimeStep = 0.5f;

	private ImmutableArray<TrajectoryPoint> _trajectoryPoints;

	public override void _Ready()
	{
		var pointStack = new List<TrajectoryPoint>(Points) { new() };
		for (var i = 1; i < Points; i++)
		{
			var point = new TrajectoryPoint
			{
				NextPoint = pointStack.Last()
			};
			pointStack.Add(point);
		}

		_trajectoryPoints = pointStack.AsEnumerable().Reverse().ToImmutableArray();
		foreach (var trajectoryPoint in _trajectoryPoints)
		{
			trajectoryPoint.TimeStep = TimeStep;
			AddChild(trajectoryPoint);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		_trajectoryPoints[0].PointVelocity = GetParent<RigidBody2D>().LinearVelocity;
		Update();
	}

	public override void _Draw()
	{
		DrawPolyline(_trajectoryPoints.Select(point => point.Position).ToArray(), Color, Width, true);
	}
}
